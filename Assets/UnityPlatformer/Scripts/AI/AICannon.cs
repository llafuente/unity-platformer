using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Static cannon Artificial inteligence.
  ///
  /// This just fire projectiles enabling 'Attack' in the input every fireDelay
  /// To configure Projectile(s) you must add CharacterActionProjectile to your
  /// Enemy Character
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
