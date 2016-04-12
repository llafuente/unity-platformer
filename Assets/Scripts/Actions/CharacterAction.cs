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
  public abstract class CharacterAction : MonoBehaviour
  {
    protected PlatformerInput input;
    protected PlatformerCollider2D controller;
    protected Character character;

    virtual public void Start() {
      input = GetComponent<PlatformerInput>();
      controller = GetComponent<PlatformerCollider2D> ();
      character = GetComponent<Character> ();
    }

    /// <summary>
    /// Tells the character we want to take control
    /// Positive numbers fight: Higher number wins
    /// Negative numbers are used to ignore fight, and execute, but do not
    /// call GetPostUpdateActions().
    /// </summary>
    public abstract int WantsToUpdate();

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
