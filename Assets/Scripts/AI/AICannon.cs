using System;
using UnityEngine;
using UnityPlatformer.Characters;
using UnityPlatformer.Actions;

namespace UnityPlatformer.AI {
  ///<summary>
  /// Static cannon Artificial inteligence.
  /// Projectile definition CharacterActionProjectile
  /// NOTE do not move.
  ///</summary>
  [RequireComponent (typeof (CharacterActionProjectile))]
  public class AICannon: Enemy {
    #region public

    [Comment("Reload time")]
    public float fireDelay = 5;

    #endregion

    #region private

    AIInput input;

    #endregion

    public override void Start() {
      input = GetComponent<AIInput>();
      input.EnableAction("Attack");

      base.Start();
    }
  }
}
