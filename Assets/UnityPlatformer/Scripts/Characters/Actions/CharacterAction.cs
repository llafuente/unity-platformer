using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityPlatformer {
  /// <summary>
  /// Actions to do after CharacterAction.PerformAction
  /// </summary>
  public enum PostUpdateActions {
    NONE = 0x00,
    APPLY_GRAVITY = 0x01,
    WORLD_COLLISIONS = 0x02,
  };
  /// <summary>
  /// Base class to perform an action over a character
  ///
  /// Basically an action modify character state and velocity
  /// </summary>
  [Serializable]
  public abstract class CharacterAction : MonoBehaviour {
    /// <summary>
    /// Target character that will be affected by this movement
    /// </summary>
    public Character character;
    /// <summary>
    /// Input to listen
    /// </summary>
    public PlatformerInput input;
    /// <summary>
    /// Callback when this movement WantsToUpdate and has highest priority.
    /// </summary>
    public Action onGrainControl;
    /// <summary>
    /// Callback when this movement don't WantsToUpdate or another movement has higher priority
    /// </summary>
    public Action onLoseControl;
    /// <summary>
    /// Callback when this movement don't WantsToUpdate or another movement has higher priority
    /// </summary>
    internal PlatformerCollider2D pc2d;
    /// <summary>
    /// Flag to know when the action has control and fire events
    /// </summary>
    private bool hasControl = false;
    /// <summary>
    /// Try to set character &amp; input from parent nodes
    /// </summary>
    public void Reset() {
      character = GetComponentInParent<Character>();
      input = GetComponentInParent<PlatformerInput>();
    }
    /// <summary>
    /// keep Character.actions in sync
    /// </summary>
    public virtual void OnEnable() {
      Assert.IsNotNull(character, "(CharacterAction) character is required: " + gameObject.GetFullName());
      Assert.IsNotNull(input, "(CharacterAction) input is required: " + gameObject.GetFullName());

      character.Awake();

      pc2d = character.gameObject.GetComponent<PlatformerCollider2D>();

      hasControl = false;

      character.actions.Add(this);
    }
    /// <summary>
    /// keep Character.actions in sync
    /// </summary>
    public virtual void OnDisable() {
      if (character != null) {
        character.actions.Remove(this);
      }
    }
    /// <summary>
    /// Called (once) when this action is going to be 'PerformAction' for first time
    ///
    /// Can be use to enter states
    /// </summary>
    public virtual void GainControl(float delta) {
      if (!hasControl && onGrainControl != null) {
        onGrainControl();
      }

      hasControl = true;
    }
    /// <summary>
    /// Called (once) when other action 'WantsToUpdate' or this action don't anymore
    ///
    /// Can be used to exit states and clean up
    /// </summary>
    public virtual void LoseControl(float delta) {
      if (hasControl && onLoseControl != null) {
        onLoseControl();
      }
      hasControl = false;
    }
    /// <summary>
    /// Tells the character we want to take control
    ///
    /// * Positive numbers fight: Higher number wins
    /// * Negative numbers are used to ignore fight and force Character to call
    /// PerformAction, but! because it doesn't win the fight onLoseControl,
    /// onGrainControl and GetPostUpdateActions are ignored.\n
    /// NOTE can be used as a replace for UpdateManager.PlatformerUpdate
    /// </summary>
    public abstract int WantsToUpdate(float delta);

    /// <summary>
    /// Do your action here.
    /// </summary>
    public abstract void PerformAction(float delta);

    /// <summary>
    /// Return what to do next.
    ///
    /// By default should be: APPLY_GRAVITY | WORLD_COLLISIONS
    /// </summary>
    public abstract PostUpdateActions GetPostUpdateActions();
  }
}
