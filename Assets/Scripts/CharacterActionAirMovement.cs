using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Perform an action over a character
  /// </summary>
  [RequireComponent (typeof (PlatformerController))]
  public class CharacterActionAirMovement: CharacterAction, UpdateManagerAttach {

    [Comment("Movement speed")]
    public float speed = 6;

    [Comment("Time to reach max speed")]
    public float accelerationTime = .2f;

    float velocityXSmoothing;

    public void Attach(UpdateManager um) {
    }

    /// <summary>
    /// Tells the character we want to take control
    /// Positive numbers fight: Higher number wins
    /// TODO REVIEW Negative numbers are used to ignore fight, and execute.
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
