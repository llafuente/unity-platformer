using System;
using UnityEngine;

namespace UnityPlatformer {
  ///<summary>
  /// Static cannon Artificial inteligence.
  /// Projectile definition at CharacterActionProjectile
  /// NOTE do not move.
  ///</summary>
  [RequireComponent (typeof (Enemy))]
  [RequireComponent (typeof (CharacterActionProjectile))]
  public class AICannon: Enemy {
    #region public

    [Comment("Reload time")]
    public float fireDelay = 5;

    #endregion

    public override void Start() {
      input.EnableAction("Attack");

      base.Start();
    }
  }
}
