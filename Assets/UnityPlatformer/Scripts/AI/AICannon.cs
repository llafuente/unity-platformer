using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Static Character that fire projectiles
  ///
  /// Like the rest of IA just use input modification.
  /// Enabling 'Attack' to continuosly fire projectiles.
  /// CharacterActionProjectile is required.
  /// </summary>
  public class AICannon: Enemy {
    /// <summary>
    /// Reload time, delay between fires
    /// </summary>
    [Comment("Reload time")]
    public float fireDelay = 5;
    /// <summary>
    /// Enable Attack Action
    ///
    /// TODO fireDelay?!
    /// </summary>
    public override void PlatformerUpdate(float delta) {
      if (!input.IsActionHeld("Attack")) {
        input.EnableAction("Attack");
      }

      base.PlatformerUpdate(delta);
    }
  }
}
