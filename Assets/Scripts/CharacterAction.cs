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
  //[RequireComponent (typeof (Character))]
  public interface CharacterAction
  {
    /// <summary>
    /// Tells the charcater we want to take control
    /// Positive numbers fight: Higher number wins
    /// TODO REVIEW Negative numbers are used to ignore fight, and execute.
    /// </summary>
    int WantsToUpdate();

    void PerformAction(float delta);

    PostUpdateActions GetPostUpdateActions();
  }
}
