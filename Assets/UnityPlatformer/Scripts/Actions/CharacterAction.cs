using System;
using UnityEngine;

namespace UnityPlatformer {
  public enum PostUpdateActions {
    NONE = 0x00,
    APPLY_GRAVITY = 0x01,
    WORLD_COLLISIONS = 0x02,
  };
  /// <summary>
  /// Base class to perform an action over a character
  /// </summary>
  [Serializable]
  public abstract class CharacterAction : MonoBehaviour {
    #region public

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

    #endregion

    // cache
    protected PlatformerCollider2D pc2d;
    protected bool hasControl = false;

    // keep character.actions in sync
    public virtual void OnEnable() {
      if (character == null) {
        Debug.LogError("Action character property is null", this);
      }

      if (input == null) {
        Debug.LogError("Action input property is null", this);
      }

      pc2d = character.gameObject.GetComponent<PlatformerCollider2D>();

      hasControl = false;

      character.actions.Add(this);
    }

    public virtual void OnDisable() {
      if (character != null) {
        character.actions.Remove(this);
      }
    }

    /// <summary>
    /// Called (once) when this action is going to be 'PerformAction' for first time
    /// </summary>
    public virtual void GainControl(float delta) {
      if (!hasControl && onGrainControl != null) {
        onGrainControl();
      }

      hasControl = true;
    }

    /// <summary>
    /// Called (once) when other action 'WantsToUpdate' or this action don't
    /// Can be used to clean up things if necessary
    /// </summary>
    public virtual void LoseControl(float delta) {
      if (hasControl && onLoseControl != null) {
        onLoseControl();
      }
      hasControl = false;
    }

    /// <summary>
    /// Tells the character we want to take control
    /// * Positive numbers fight: Higher number wins
    /// * Negative numbers are used to ignore fight and force Character to call
    /// PerformAction, but! because it doesn't win the fight onLoseControl,
    /// onGrainControl and GetPostUpdateActions are ignored.
    /// NOTE can be used as a replace for UpdateManager.ManagedUpdate
    /// </summary>
    public abstract int WantsToUpdate(float delta);

    /// <summary>
    /// Do your action here.
    /// </summary>
    public abstract void PerformAction(float delta);

    /// <summary>
    /// Return what to do next
    /// by default: APPLY_GRAVITY | WORLD_COLLISIONS
    /// </summary>
    public abstract PostUpdateActions GetPostUpdateActions();
  }
}
