using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Movement while on ground and not slipping
  /// </summary>
  public class CharacterActionCrounchMovement: CharacterActionHorizontalMovement {
    /// <summary>
    /// Execute when collision below.
    /// </summary>
    public override int WantsToUpdate(float delta) {
      // NOTE if Air/Ground are very different maybe:
      // if (character.IsOnGround(<frames>)) it's better
      if (character.collisions.below &&
        character.IsOnState(States.Crounch) &&
        !character.IsOnState(States.Slipping) &&
        !character.IsOnState(States.Pushing)) {
        return -1;
      }

      Clear();
      return 0;
    }
  }
}
