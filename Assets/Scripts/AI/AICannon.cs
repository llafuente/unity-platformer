using System;
using UnityEngine;
using UnityPlatformer.Characters;

namespace UnityPlatformer.AI {
  ///<summary>
  /// Static cannon Artificial inteligence.
  /// NOTE do not move.
  /// TODO projectile should be a list
  /// TODO offset list
  /// TODO fireModes{ALL, ONE_BY_ONE}
  ///</summary>
  public class AICannon: Enemy {
    #region public
    [Comment("Projectile that will be cloned.")]
    public Projectile projectile;
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
