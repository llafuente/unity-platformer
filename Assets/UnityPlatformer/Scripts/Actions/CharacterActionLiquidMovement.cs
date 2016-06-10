using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Movement while below water surface (buoyancy)
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
    public CharacterActionJump actionJump;
    [Comment("Exit jump.")]
    public JumpConstantProperties jumpOff = new JumpConstantProperties(new Vector2(20, 20));

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
        pc2d.enableSlopes = false;
        float factor = (1 + character.liquid.buoyancySurfaceFactor * d) * delta;
        //Debug.Log(factor);
        character.velocity.x += character.liquid.buoyancy.x * factor;
        character.velocity.y += character.liquid.buoyancy.y * factor;

        if (character.velocity.y > terminalYUP) {
          character.velocity.y = terminalYUP;
        }
        if (character.velocity.y < -terminalYDown) {
          character.velocity.y = terminalYDown;
        }
      } else {
        pc2d.enableSlopes = true;
      }

      //TODO REVIEW this lead to some problems with orientation...
      int faceDir;
      if (in2d.x == 0) {
        faceDir = 0;
      } else {
        character.pc2d.collisions.faceDir = faceDir = (int) Mathf.Sign(in2d.x);
      }

      if (input.IsActionHeld(actionJump.action)) {
        character.ExitState(States.Liquid);

        actionJump.Jump(new JumpConstant(character,
          jumpOff.Clone(faceDir)
        ));
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
