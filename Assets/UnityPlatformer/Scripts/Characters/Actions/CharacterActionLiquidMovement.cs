using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Movement while below water surface (buoyancy)
  /// TODO slopeAccelerationFactor/slopeDeccelerationFactor
  /// </summary>
  public class CharacterActionLiquidMovement: CharacterAction {
    /// <summary>
    /// Movement speed
    /// </summary>
    [Comment("Movement speed")]
    public float speed = 6;
    /// <summary>
    /// Time to reach max speed
    /// </summary>
    [Comment("Time to reach max speed")]
    public float accelerationTime = .1f;
    /// <summary>
    /// Distance from feet (up)
    /// </summary>
    [Comment("Distance from feet (up)")]
    public float surfaceLevel = 1.5f;
    /// <summary>
    /// Terminal velocity going up
    /// </summary>
    public float terminalYUP = 2f;
    /// <summary>
    /// Terminal velocity going down
    /// </summary>
    public float terminalYDown = 3f;
    /// <summary>
    /// Ignore liquids under liquidMinHeight
    /// </summary>
    public float liquidMinHeight = 0.75f;
    /// <summary>
    /// Exit jump properties
    /// </summary>
    [Comment("Exit jump.")]
    public JumpConstantProperties jumpOff = new JumpConstantProperties(new Vector2(20, 20));
    /// <summary>
    /// CharacterActionJump
    /// </summary>
    internal CharacterActionJump actionJump;
    /// <summary>
    /// Mathf.SmoothDamp
    /// </summary>
    float velocityXSmoothing;

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
        change = 1; // leave
      } else if (
        ((before & States.Liquid) != States.Liquid) &&
        ((after & States.Liquid) == States.Liquid)
      ) {
        change = 2; // enter
      }

      if (change > 0) {
        character.enableSlopes = change == 1;
        character.leavingGround = change == 2;
      }
    }
    /// <summary>
    /// Execute when collision below.
    /// </summary>
    public override int WantsToUpdate(float delta) {
      // NOTE if Air/Ground are very different maybe:
      // if (character.IsOnGround(<frames>)) it's better
      if (character.liquid) {
        // liquid is very little
        if (character.liquid.body.size.y < liquidMinHeight) {
          character.ExitStateGraceful(States.Liquid);
          return 0;
        }

        // character below surface, enter
        if (character.liquid.IsBelowSurface(character, surfaceLevel)) {
          // enter when 'below surface'
          // this allow to walk-small-water
          character.EnterStateGraceful(States.Liquid);
          return -1;
        } else if (character.collisions.below) {
          // is in the water, touching floor, exit
          character.ExitStateGraceful(States.Liquid);
          return 0;
        }
      }

      character.ExitStateGraceful(States.Liquid);
      return 0;
    }

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

      character.SetFacing(in2d.x);

      if (input.IsActionHeld(actionJump.action)) {
        character.ExitState(States.Liquid);

        actionJump.Jump(new JumpConstant(character,
          jumpOff.Clone((int) character.faceDir)
        ));
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
