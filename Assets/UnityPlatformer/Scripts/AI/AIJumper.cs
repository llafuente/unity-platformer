using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Jump until obstacle.
  ///
  /// NOTE if you want the Jumper to move on ground add: CharacterActionGroundMovement\n
  /// TODO follow player (turn always)\n
  /// TODO jump delay\n
  /// TODO can turn during jump\n
  /// </summary>
  public class AIJumper: AIPatrol {
    float jumpDelay = 1.0f;
    bool movingWhileNotJumping = false;
    bool abruptStop = true;
    /// <summary>
    /// Enable Jump and do Patrol staff
    /// </summary>
    public override void PlatformerUpdate(float delta) {
      // jump after the delay
      if (
        collisions.belowFrames > UpdateManager.GetFrameCount(jumpDelay)
        &&
        !IsOnState(States.Jumping)
      ) {
        input.EnableAction("Jump");
      }
      // jump until start falling
      if (input.IsActionHeld("Jump") && IsOnState(States.Falling)) {
        input.DisableAction("Jump");
      }

      if (!movingWhileNotJumping && IsOnState(States.OnGround)) {
        Stop();
        if (abruptStop) {
          velocity.x = 0;
        }
      } else {
        Resume();
      }

      base.PlatformerUpdate(delta);
    }
  }
}
