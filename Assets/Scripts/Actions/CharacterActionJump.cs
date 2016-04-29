using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Perform an action over a character
  /// TODO support for custom jumps.
  /// </summary>
  public class CharacterActionJump: CharacterAction {
    #region public

    // TODO OnValidate check this!
    [Comment("Must match something @PlatformerInput")]
    public String action = "Jump";
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    [Comment("Time allowed to jump after leave ground")]
    public float graceJumpTime = 0.15f;
    [Comment("Time to reach maxJumpHeight")]
    public float timeToJumpApex = 0.4f;
    [Comment("The amount of time may spend hanging in midair at the apex of her jump (while the jump is not canceled).")]
    public float hangTime = 0.0f;

    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 5;
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

      Debug.Log("character" + character);
      jump = new Jump(character, timeToJumpApex, minJumpHeight, maxJumpHeight, hangTime);

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
        if (jumping) { // jump stops?
          jumpStopped = true;
        }
      }
    }

    /// <summary>
    /// TODO REVIEW jump changes when moved to action, investigate
    /// </summary>
    public override int WantsToUpdate(float delta) {
      /* DEBUG
      text = string.Format("jumpHeld: {0}\nonGround: {1}\njumping: {2}\njumpStopped: {3}\nCondition: {4}\n" +
      "maxJumpVelocity: {5}\nminJumpVelocity: {6}\nhangFrames: {7}\napexFrames: {8}\nticks: {9}",
        jumpHeld,
        controller.IsOnGround(_graceJumpFrames),
        jumping,
        jumpStopped,
        jumpStopped || (
          jumpHeld && (
            controller.IsOnGround(_graceJumpFrames) || jumping
          )
        ),

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
        jumping = false;
        jumpStopped = false;
        jump.EndJump(ref character.velocity);
        jump.Reset();
      } else if (!jumping) {
        jump.StartJump(ref character.velocity);
        jumping = true;
        character.EnterState(States.Jumping);
      } else {
        if (jump.IsHanging()) {
          character.EnterState(States.Hanging);
        }

        if (!jump.Jumping(ref character.velocity) || character.velocity.y < 0) {
          jumping = false;
          character.ExitState(States.Jumping);
        }
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
