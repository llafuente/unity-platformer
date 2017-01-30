using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Movement while airbone, despite beeing falling, jumping...
  /// </summary>
  public class CharacterActionAirMovement: CharacterActionHorizontalMovement {
    /// <summary>
    /// Execute when no collision below.
    /// </summary>
    public override int WantsToUpdate(float delta) {
      // NOTE if Air/Ground are very different maybe:
      // if (character.IsOnGround(<frames>)) it's better
      if (character.collisions.below || character.IsOnState(States.Grabbing)) {
        Clear();
        return 0;
      }
      return -1;
    }
  }
}
