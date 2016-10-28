using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO use: CharacterActionTimed ?
// TODO OnValidate check this! CharacterActionProjectile.action value
// TODO wire actions

namespace UnityPlatformer {
  /// <summary>
  /// Fire projectiles (All or one by one), handles orientation based on
  /// the character collisions.faceDir
  /// </summary>
  public class CharacterActionProjectile: CharacterAction {
    /// <summary>
    /// Projectiles data
    /// </summary>
    [Serializable]
    public struct ProjectileCfg {
      /// <summary>
      /// Projectile object
      /// </summary>
      public Projectile projectile;
      /// <summary>
      /// Offset position when firing
      /// </summary>
      public Vector2 offset;
      /// <summary>
      /// delay between fires
      ///
      /// NOTE this is ignored when fireMode = false
      /// </summary>
      public float delay;
    };
    /// <summary>
    /// Input action name
    /// </summary>
    [Comment("Must match something in @PlatformerInput")]
    public String action = "Attack";
    /// <summary>
    /// List of projectiles
    /// </summary>
    public List<ProjectileCfg> projectiles = new List<ProjectileCfg>();

    [Space(10)]

    /// <summary>
    /// Reload time
    /// </summary>
    [Comment("Reload time")]
    public float fireDelay = 5;
    /// <summary>
    /// * true: Fire all at once (with given delays).
    /// * false: Fire one by one
    /// </summary>
    [Comment("checked: Fire all at once (with given delays). unchecked: Fire one by one")]
    public bool fireMode = false;

    [Space(10)]

    /// <summary>
    /// Action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 5;
    /// <summary>
    /// Fired when fire a projectile
    /// </summary>
    public Action onFire;
    /// <summary>
    /// Current projectile index
    /// </summary>
    int currentIndex = 0;
    /// <summary>
    /// Counter
    /// </summary>
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
    /// <summary>
    /// Fire projectiles, regardless the cooldown,
    /// Call it after all checks
    /// </summary>
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
    /// <summary>
    /// Real Fire function, for use with: UpdateManager.SetTimeout
    /// </summary>
    protected virtual void _Fire() {
      // select projectile
      ProjectileCfg p = projectiles[currentIndex++];
      // reach the end -> reset, do this fast :)
      if (currentIndex == projectiles.Count ) {
        currentIndex = 0;
      }

      int dir = (int) character.faceDir; // TODO REVIEW what happens when Facing.None ??!
      Vector3 offset = (Vector3) p.offset;
      offset.x *= dir;

      // unlesh hell
      Projectile new_p;
      new_p = p.projectile.Fire(character.transform.position + offset);
      new_p.velocity.x *= dir;
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }

  }
}
