using System;
using UnityEngine;
using UnityPlatformer.Characters;

namespace UnityPlatformer.AI {
	/// TODO use this must use: CharacterActionGroundMovement
	public class AICannon: Enemy {
    #region public
    [Comment("Projectile that will be cloned.")]
    public Projectile projectile;
    [Comment("Reload time")]
    public float fireDelay = 5;

    #endregion

    #region private

		AIInput input;

    float time;

    #endregion

		public override void Start() {
      time = 0;
			base.Start();
		}

		public override void ManagedUpdate(float delta) {
      time += delta;
      //Debug.LogFormat("time {0} fireDelay {1}", time, fireDelay);
      if (time > fireDelay) {
        Debug.Log("FIRE!!!");
        time -= fireDelay;
        projectile.Fire(projectile.transform.position);
      }

			base.ManagedUpdate(delta);
		}
	}
}
