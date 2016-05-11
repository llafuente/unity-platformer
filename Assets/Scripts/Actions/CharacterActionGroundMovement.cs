using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Movement while on ground
  /// TODO slopeAccelerationFactor/slopeDeccelerationFactor
  /// </summary>
  public class CharacterActionGroundMovement: CharacterAction, IUpdateManagerAttach {
    #region public

    [Comment("Movement speed")]
    public float speed = 6;
    [Comment("Time to reach max speed")]
    public float accelerationTime = .1f;

    #endregion

    float velocityXSmoothing;

    public void Attach(UpdateManager um) {
    }

    /// <summary>
    /// Execute when collision below.
    /// </summary>
    public override int WantsToUpdate(float delta) {
      // NOTE if Air/Ground are very different maybe:
      // if (pc2d.IsOnGround(<frames>)) it's better
      if (pc2d.collisions.below) {
        return -1;
      }
      return 0;
    }

    /// <summary>
    /// Reset SmoothDamp
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);
      velocityXSmoothing = 0;
    }


    /// <summary>
    /// Horizontal movement
    /// </summary>
    public override void PerformAction(float delta) {
      Vector2 in2d = input.GetAxisRaw();

      float targetVelocityX = in2d.x * speed;

      character.velocity.x = Mathf.SmoothDamp (
        character.velocity.x,
        targetVelocityX,
        ref velocityXSmoothing,
        accelerationTime
      );
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
