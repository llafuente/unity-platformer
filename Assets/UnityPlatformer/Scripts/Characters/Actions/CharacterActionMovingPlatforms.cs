using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Fall through platforms while pressing down
  /// </summary>
  public class CharacterActionMovingPlatforms: CharacterAction {

    public override int WantsToUpdate(float delta) {
      if (
        character.platform &&
        input.GetAxisRawY() < 0 &&
        Configuration.IsMovingPlatformThrough(character.platform.gameObject)
      ) {
        return -1;
      }
      return 0;
    }

    public override void PerformAction(float delta) {
      character.FallThroughPlatform();
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
