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
    public float liquidMinHeight = 0.75f;
    [Comment("Exit jump.")]
    public JumpConstantProperties jumpOff = new JumpConstantProperties(new Vector2(20, 20));

    #endregion

    internal CharacterActionJump actionJump;

    float velocityXSmoothing;
    bool insideWater = false;

    public override void OnEnable() {
      base.OnEnable();
      character.onStateChange += OnEnterState;
      actionJump = character.GetAction<CharacterActionJump>();
    }

    void OnEnterState(States before, States after) {

      int change = 0; // no
      if (
        ((before & States.Liquid) == States.Liquid) &&
        ((after & States.Liquid) != States.Liquid)
      ) {
        change = 1; // lave
      } else if (
        ((before & States.Liquid) != States.Liquid) &&
        ((after & States.Liquid) == States.Liquid)
      ) {
        change = 2; // enter
      }

      if (change > 0) {
        pc2d.enableSlopes = change == 1;
        pc2d.leavingGround = change == 2;
      }
    }

    /// <summary>
    /// Execute when collision below.
    /// </summary>
    public override int WantsToUpdate(float delta) {
      // NOTE if Air/Ground are very different maybe:
      // if (pc2d.IsOnGround(<frames>)) it's better
      if (character.liquid) {
        // liquid is very little
        if (character.liquid.body.size.y < liquidMinHeight) {
          character.SolfExitState(States.Liquid);
          return 0;
        }

        // character below surface, enter
        if (character.liquid.IsBelowSurface(character, surfaceLevel)) {
          // enter when 'below surface'
          // this allow to walk-small-water
          character.SolfEnterState(States.Liquid);
          return -1;
        } else if (character.pc2d.collisions.below) {
          // is in the water, touching floor, exit
          character.SolfExitState(States.Liquid);
          return 0;
        }
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
