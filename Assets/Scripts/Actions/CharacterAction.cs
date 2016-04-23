using System;
using UnityEngine;

namespace UnityPlatformer {
  public enum PostUpdateActions {
    NONE = 0x00,
    APPLY_GRAVITY = 0x01,
    WORLD_COLLISIONS = 0x02,
  };
  /// <summary>
  /// Perform an action over a character
  /// </summary>
  public abstract class CharacterAction : MonoBehaviour {
    public Character character;
    public PlatformerInput input;
    public Action onGrainControl;
    public Action onLoseControl;

    protected PlatformerCollider2D controller;
    protected bool hasControl = false;

    virtual public void Start() {
      if (character == null) {
        Debug.LogError(gameObject.name + " contains an action without character property set");
      }

      if (input == null) {
        Debug.LogError(gameObject.name + " contains an action without input property set");
      }

      controller = character.controller;

      hasControl = false;
    }

    virtual public void OnEnable() {
      character.actions.Add(this);
    }

    virtual public void OnDisable() {
      if (character != null) {
        character.actions.Remove(this);
      }
    }

    public void GainControl() {
      if (!hasControl && onGrainControl != null) {
        onGrainControl();
      }

      hasControl = true;
    }
    public void LoseControl() {
      if (hasControl && onLoseControl != null) {
        onLoseControl();
      }
      hasControl = false;
    }

    /// <summary>
    /// Tells the character we want to take control
    /// Positive numbers fight: Higher number wins
    /// Negative numbers are used to ignore fight, and execute, but do not
    /// call GetPostUpdateActions().
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
