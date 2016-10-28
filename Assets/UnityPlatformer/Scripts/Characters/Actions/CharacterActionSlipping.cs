using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Force character to slip down slope, when slopeAngle > maxClimbAngle
  ///
  /// NOTE do not apply gravity. Handle the velocity manually so the character
  /// is always colliding below
  /// </summary>
  public class CharacterActionSlipping: CharacterAction {
    /// <summary>
    /// Minimum speed
    /// </summary>
    public float minSpeed = 4;
    [Comment("Movement speed (decreased by slopeAngle)")]
    /// <summary>
    /// Maximum speed
    /// </summary>
    public float maxSpeed = 12;
    /// <summary>
    /// Time to reach maxSpeed
    /// </summary>
    [Comment("Time to reach max speed")]
    public float accelerationTime = .20f;

    [Space(10)]

    /// <summary>
    /// Action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 3;
    /// <summary>
    /// for use with Mathf.SmoothDamp
    /// </summary>
    float velocityXSmoothing;
    /// <summary>
    /// Not in a liquid. OnGround and slopeAngle > maxClimbAngle
    /// </summary>
    public override int WantsToUpdate(float delta) {
      if (character.IsOnState(States.Liquid)) {
        return 0;
      }

      // NOTE if Air/Ground are very different maybe:
      // if (pc2d.IsOnGround(<frames>)) it's better
      if (character.IsOnState(States.OnGround) && pc2d.collisions.slopeAngle > pc2d.maxClimbAngle) {
        return priority;
      }
      return 0;
    }
    /// <summary>
    /// Reset SmoothDamp
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);
      velocityXSmoothing = 0;
      // TODO REVIEW this is to be sure the character stay colliding below, any side effects?
      character.velocity.y = 0;
      character.velocity.x = minSpeed * Mathf.Sign(pc2d.collisions.slopeNormal.x);
      character.EnterState(States.Slipping);
      pc2d.ignoreDescendAngle = true;
    }
    /// <summary>
    /// Exit state
    /// </summary>
    public override void LoseControl(float delta) {
      base.LoseControl(delta);
      character.ExitState(States.Slipping);
      pc2d.ignoreDescendAngle = false;
    }
    /// <summary>
    /// Slipping down slope!
    /// </summary>
    public override void PerformAction(float delta) {
      Vector3 slopeDir = pc2d.GetDownSlopeDir();

      character.velocity.x = Mathf.SmoothDamp (
        character.velocity.x,
        maxSpeed * Mathf.Sign(pc2d.collisions.slopeNormal.x) * Mathf.Abs(slopeDir.y),
        ref velocityXSmoothing,
        accelerationTime
      );
      character.velocity.y = slopeDir.y * Mathf.Abs(character.velocity.x);
    }
    /// <summary>
    /// default actions
    /// </summary>
    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
