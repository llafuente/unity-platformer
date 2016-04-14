using System;
using UnityEngine;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Actions {
  public enum PostUpdateActions {
    NONE = 0x00,
    APPLY_GRAVITY = 0x01,
    WORLD_COLLISIONS = 0x02,
  };
  /// <summary>
  /// Perform an action over a character
  /// </summary>
  [RequireComponent (typeof (Character))]
  [RequireComponent (typeof (PlatformerInput))]
  public abstract class CharacterAction : MonoBehaviour {
    public Action onGrainControl;
    public Action onLoseControl;

    protected PlatformerInput input;
    protected PlatformerCollider2D controller;
    protected Character character;
    protected bool hasControl = false;

    virtual public void Start() {
      input = GetComponent<PlatformerInput>();
      controller = GetComponent<PlatformerCollider2D> ();
      character = GetComponent<Character> ();

      hasControl = false;
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
