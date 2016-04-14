using System;
using UnityEngine;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Actions {
  /// <summary>
  /// Perform an action over a character
  /// </summary>
  public class CharacterActionAirMovement: CharacterAction, IUpdateManagerAttach {
    #region public

    [Comment("Movement speed")]
    public float speed = 6;
    [Comment("Time to reach max speed")]
    public float accelerationTime = .2f;

    #endregion

    float velocityXSmoothing;

    public void Attach(UpdateManager um) {
    }

    /// <summary>
    /// Execute when no collision below.
    /// </summary>
    public override int WantsToUpdate() {
      if (controller.collisions.below) {
        velocityXSmoothing = 0;
        return 0;
      }
      return -1;
      // TODO REVIEW: return controller.IsOnGround(0) ? -1 : 0;
    }

    public override void PerformAction(float delta) {
      Vector2 in2d = input.GetAxisRaw();

      float targetVelocityX = in2d.x * speed;
      // (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne
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
