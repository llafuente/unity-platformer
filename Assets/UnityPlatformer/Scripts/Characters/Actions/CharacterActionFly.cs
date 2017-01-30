using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// This is a Hacky Action to keep the Character from falling.\n
  /// The reason is that you cannot set 0,0 as gravity in the character, because
  /// it means use global configuration
  /// </summary>
  public class CharacterActionFly: CharacterAction {
    public override void OnEnable() {
      base.OnEnable();
    }
    /// <summary>
    /// Oppose gravity, do not fall
    /// </summary>
    public override void PerformAction(float delta) {
      character.velocity.y -= character.gravity.y * delta;
    }
    /// <summary>
    /// Always.
    /// </summary>
    public override int WantsToUpdate(float delta) {
      return -1;
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
