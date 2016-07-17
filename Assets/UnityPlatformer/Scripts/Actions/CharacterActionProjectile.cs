using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Fire projectiles (All or one by one), handles orientation based on
  /// the character collisions.faceDir
  /// TODO use: CharacterActionTimed
  /// </summary>
  public class CharacterActionProjectile: CharacterAction {
    #region public

    [Serializable]
    public struct ProjectileCfg {
      public Projectile projectile;
      public Vector2 offset;
      // this is ignored when fireMode = false
      public float delay;
    };


    // TODO OnValidate check this!
    [Comment("Must match something in @PlatformerInput")]
    public String action = "Attack";
    public List<ProjectileCfg> projectiles = new List<ProjectileCfg>();
    [Space(10)]
    [Comment("Reload time")]
    public float fireDelay = 5;
    [Comment("checked: Fire all at once (with given delays). unchecked: Fire one by one")]
    public bool fireMode = false;

    [Space(10)]
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 5;

    public Action onFire;

    #endregion

    int currentIndex = 0;
    float time = 0;

    /// <summary>
    /// TODO REVIEW continous fire?
    /// </summary>
    public override int WantsToUpdate(float delta) {
      time += delta;
      if (time > fireDelay) {
        return input.IsActionHeld(action) ? priority : 0;
      }
      return 0;
    }

    public override void PerformAction(float delta) {
      time = 0; // reset timer
      Fire();
    }

    ///<summary>
    /// Fire projectiles, regardless the cooldown,
    /// Call it after all checks
    ///</summary>
    public virtual void Fire() {
      if (fireMode) {
        for (int i = 0; i < projectiles.Count; ++i) {
          UpdateManager.instance.SetTimeout(_Fire, projectiles[i].delay);
        }
      } else {
        ProjectileCfg p = projectiles[currentIndex];
        p.delay = 0; // force no delay
        _Fire();
      }

      if (onFire != null) {
        onFire();
      }
    }

    protected virtual void _Fire() {
      // select projectile
      ProjectileCfg p = projectiles[currentIndex++];
      // reach the end -> reset, do this fast :)
      if (currentIndex == projectiles.Count ) {
        currentIndex = 0;
      }

      int dir = pc2d.collisions.faceDir;
      Vector3 offset = (Vector3) p.offset;
      offset.x *= dir;

      // unlesh hell
      Projectile new_p;
      new_p = p.projectile.Fire(character.transform.position + offset);
      new_p.velocity.x *= dir;

      // TODO wire actions
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }

  }
}
