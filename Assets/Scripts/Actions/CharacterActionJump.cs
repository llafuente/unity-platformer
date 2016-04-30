using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Perform an action over a character
  /// TODO support for custom jumps.
  /// </summary>
  public class CharacterActionJump: CharacterAction {
    #region public

    public JumpProperties jumpProperties;

    // TODO OnValidate check this!
    [Space(10)]
    [Comment("Must match something @PlatformerInput")]
    public String action = "Jump";

    [Space(10)]
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 5;
    #endregion

    #region private

    bool jumpHeld = false;
    bool jumping = false;
    bool jumpStopped = false;
    bool customJump = false;

    Jump defaultJump;
    Jump currentJump;
    int _graceJumpFrames;

    #endregion

    public override void Start() {
      base.Start();

      Debug.Log("character" + character);
      defaultJump = new Jump(character, jumpProperties);

      _graceJumpFrames = UpdateManager.instance.GetFrameCount (jumpProperties.graceJumpTime);
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

    public void Jump(Jump j) {
      jumping = true;
      currentJump = j;
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

      if (jumpHeld) {
        if (controller.IsOnGround(_graceJumpFrames) && !jumping) {
          currentJump = defaultJump;
          return priority;
        }
        if (jumping) {
          return priority;
        }
      }

      // jump stopped? run one last time to reset
      if (jumpStopped) {
        return priority;
      }

      return 0;
    }

    public override void PerformAction(float delta) {
      // last update to set 'exit' velocity
      if (jumpStopped) {
        jumping = false;
        jumpStopped = false;
        currentJump.EndJump(ref character.velocity);
        currentJump.Reset();
      } else if (!jumping) {
        currentJump.StartJump(ref character.velocity);
        jumping = true;
        character.EnterState(States.Jumping);
      } else {
        if (currentJump.IsHanging()) {
          character.EnterState(States.Hanging);
        }

        if (!currentJump.Jumping(ref character.velocity) || character.velocity.y < 0) {
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
