using System;
using UnityEngine;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Actions {
  /// <summary>
  /// Perform an action over a character
  /// TODO Hangtime: The amount of time the player may spend hanging in
  /// midair at the apex of her jump (while the jump is not canceled).
  /// </summary>
  public class CharacterActionJump: CharacterAction {
    #region public

    // TODO OnValidate check this!
    [Comment("Must match something in @PlatformerInput")]
    public String action;
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 5;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float graceJumpTime = 0.25f;
    public float timeToJumpApex = 0.4f;
    public float hangTime = 0.0f;

    #endregion

    #region private

    bool jumpHeld = false;
    bool jumping = false;
    bool jumpStopped = false;

    Jump jump;
    int _graceJumpFrames;

    #endregion

    public override void Start() {
      base.Start();

      jump = new Jump(character, timeToJumpApex, minJumpHeight, hangTime);

      _graceJumpFrames = UpdateManager.instance.GetFrameCount (graceJumpTime);
      input.onActionUp += OnActionUp;
      input.onActionDown += OnActionDown;
    }

    public void OnActionDown(string _action) {
      if (_action == action) {
        jumpHeld = true;
      }
    }

    public void OnActionUp(string _action) {
      // when button is up, reset, and allow a new jump
      if (_action == action) {
        jumpHeld = false;
        jumping = false; // jump stops
        jumpStopped = true;
        //character.ExitState(Character.States.Jumping);
      }
    }

    /// <summary>
    /// TODO REVIEW jump changes when moved to action, investigate
    /// </summary>
    public override int WantsToUpdate(float delta) {
      /* DEBUG
      text = string.Format("jumpHeld {0}\n, onGround {1}\n, jumping {2}\n\n" +
      "maxJumpVelocity: {3}\nminJumpVelocity: {4}\nhangFrames: {5}\napexFrames: {6}\nticks: {7}",
        jumpHeld,
        controller.IsOnGround(_graceJumpFrames),
        jumping,

        jump.maxJumpVelocity,
        jump.minJumpVelocity,
        jump.hangFrames,
        jump.apexFrames,
        jump.ticks
      );
      */

      return jumpStopped || (
        jumpHeld && (
          controller.IsOnGround(_graceJumpFrames) || jumping
        )
      ) ? priority : 0;
    }

    public override void PerformAction(float delta) {
      // last update to set 'exit' velocity
      if (jumpStopped) {
        jumpStopped = false;
        jump.EndJump(ref character.velocity);
      }

      if (!jumping) {
        jump.StartJump(ref character.velocity);
        jumping = true;
        character.EnterState(Character.States.Jumping);
      } else {
        if (jump.IsHanging()) {
          character.EnterState(Character.States.Hanging);
        }

        if (!jump.Jumping(ref character.velocity) || character.velocity.y < 0) {
          jumping = false;
          character.ExitState(Character.States.Jumping);
        }
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
