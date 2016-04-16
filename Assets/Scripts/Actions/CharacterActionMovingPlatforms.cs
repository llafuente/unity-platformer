using System;
using UnityEngine;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Actions {
  /// <summary>
  /// Falling through platforms while pressing down
  /// </summary>
  public class CharacterActionMovingPlatforms: CharacterAction {
    /// <summary>
    /// TODO REVIEW jump changes when moved to action, investigate
    /// </summary>
    public override int WantsToUpdate(float delta) {
      if (
        character.platform &&
        input.GetAxisRawY() < 0 &&
        character.platform.gameObject.tag == Configuration.instance.movingPlatformThroughTag
      ) {
        return -1;
      }
      return 0;
    }

    public override void PerformAction(float delta) {
      character.controller.FallThroughPlatform();
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
