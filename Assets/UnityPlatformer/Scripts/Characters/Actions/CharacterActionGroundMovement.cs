using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Movement while on ground and not slipping
  /// </summary>
  public class CharacterActionGroundMovement: CharacterActionHorizontalMovement {
    /// <summary>
    /// Execute when collision below.
    /// </summary>
    public override int WantsToUpdate(float delta) {
      // NOTE if Air/Ground are very different maybe:
      // if (pc2d.IsOnGround(<frames>)) it's better
      if (pc2d.collisions.below &&
        !character.IsOnState(States.Slipping) &&
        !character.IsOnState(States.Pushing)) {
        return -1;
      }

      Reset();
      return 0;
    }
  }
}
