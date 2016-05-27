using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Movement while on ground and not slipping
  /// TODO slopeAccelerationFactor/slopeDeccelerationFactor
  /// </summary>
  public class CharacterActionLiquidMovement: CharacterAction {
    #region public

    [Comment("Movement speed")]
    public float speed = 6;
    [Comment("Time to reach max speed")]
    public float accelerationTime = .1f;
    [Comment("Distance from feet (up)")]
    public float surfaceLevel = 1.5f;
    public float terminalYUP = 2f;
    public float terminalYDown = 3f;

    #endregion

    float velocityXSmoothing;

    /// <summary>
    /// Execute when collision below.
    /// </summary>
    public override int WantsToUpdate(float delta) {
      // NOTE if Air/Ground are very different maybe:
      // if (pc2d.IsOnGround(<frames>)) it's better
      if (character.liquid) {
        character.SolfEnterState(States.Liquid);
        return -1;
      }
      character.SolfExitState(States.Liquid);
      return 0;
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

      // Debug.Log("-->" + character.liquid.IsBelowSurface(character, surfaceLevel));

      float d = character.liquid.DistanceToSurface(character, surfaceLevel);
      if (d > 0) { // below
        float factor = (1 + character.liquid.boyancySurfaceFactor * d) * delta;
        //Debug.Log(factor);
        character.velocity.x += character.liquid.boyancy.x * factor;
        character.velocity.y += character.liquid.boyancy.y * factor;

        if (character.velocity.y > terminalYUP) {
          character.velocity.y = terminalYUP;
        }
        if (character.velocity.y < -terminalYDown) {
          character.velocity.y = terminalYDown;
        }

      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
