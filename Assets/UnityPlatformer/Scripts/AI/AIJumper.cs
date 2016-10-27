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
  public class AIJumper: AIGoomba {
    /// <summary>
    /// Enable Jump and do Patrol staff
    /// </summary>
    public override void PlatformerUpdate(float delta) {
      if (!input.IsActionHeld("Jump")) {
        input.EnableAction("Jump");
      }

      base.PlatformerUpdate(delta);
    }
  }
}
