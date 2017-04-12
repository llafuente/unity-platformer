using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Jump in a constant direction every jumpDelay then AIPatrol logic.
  ///
  /// *NOTE* if you want the Jumper to move on ground add: CharacterActionGroundMovement\n
  /// TODO follow player (turn always)\n
  /// TODO can turn during jump\n
  /// </summary>
  public class AIJumper: AIPatrol {
    /// <summary>
    /// Time between jumps
    /// </summary>
    float jumpDelay = 1.0f;
    /// <summary>
    /// Allow to move while not jumping in the same direction
    /// </summary>
    bool movingWhileNotJumping = false;
    /// <summary>
    /// When reach ground after jump reset X velocity.
    /// Otherwise Character will decelerate
    /// </summary>
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
      // so if someday we have double jump don't have problems
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
