using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Jump while on ground.
  /// The rest jumps, are managed here, but using jump properties/type
  /// elsewhere, just by calling Jump() / ForceJump()
  /// NOTE unity-platformer support multiple types of jumps.
  /// We just ship one type here, be free to extend!
  /// TODO force one jump per press, force to release before another jump
  /// </summary>
  public class CharacterActionJump: CharacterAction {
    #region public

    public JumpVariableHeightProperties jumpProperties;

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
    bool forceJump = false;

    Jump defaultJump;
    Jump currentJump;
    int _graceJumpFrames;

    #endregion

    public override void OnEnable() {
      base.OnEnable();

      defaultJump = new JumpVariableHeight(character, jumpProperties);

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

    public void ForceJump(Jump j) {
      Jump(j);
      forceJump = true;
    }

    public void Jump(Jump j) {
      jumping = true;
      customJump = true; // enable StartJump
      currentJump = j;
    }

    /// <summary>
    /// Listen input and Select the current Jump
    /// Other actions (with higher priority) can trigger Jumps
    /// using Jump(), and the next Frame CharacterActionJump
    /// willl take control!
    /// </summary>
    public override int WantsToUpdate(float delta) {
      /* DEBUG
      text = string.Format("jumpHeld: {0}\nonGround: {1}\njumping: {2}\njumpStopped: {3}\nCondition: {4}\n" +
      "maxJumpVelocity: {5}\nminJumpVelocity: {6}\nhangFrames: {7}\napexFrames: {8}\nticks: {9}",
        jumpHeld,
        pc2d.IsOnGround(_graceJumpFrames),
        jumping,
        jumpStopped,
        jumpStopped || (
          jumpHeld && (
            pc2d.IsOnGround(_graceJumpFrames) || jumping
          )
        ),

        jump.maxJumpVelocity,
        jump.minJumpVelocity,
        jump.hangFrames,
        jump.apexFrames,
        jump.ticks
      );
      */

      if (forceJump) {
        return priority;
      }

      // while on a liquid player can be onGround, so it can jump...
      // disable it
      if (character.IsOnState(States.Liquid)) {
        return 0;
      }

      if (jumpHeld) {
        if (pc2d.IsOnGround(_graceJumpFrames) && !jumping) {
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
      //Debug.LogFormat("jumpStopped {0} jumping {1} customJump {2}", jumpStopped, jumping, customJump);
      // last update to set 'exit' velocity
      if (pc2d.leavingGround && !pc2d.collisions.below) {
        pc2d.leavingGround = false;
      }

      if (jumpStopped) {
        jumping = false;
        jumpStopped = false;
        forceJump = false;
        currentJump.EndJump(ref character.velocity);
        currentJump.Reset();
      } else if (!jumping || (jumping && customJump)) {
        currentJump.StartJump(ref character.velocity);
        jumping = true;
        customJump = false;
        character.EnterState(States.Jumping);
        // leaving ground could lead to some inestability in the collider
        // use a max delay to leave ground in case jump is disabled too fast
        pc2d.EnableLeaveGround(0.25f);
      } else {
        if (currentJump.IsHanging()) {
          character.EnterState(States.Hanging);
        }

        if (!currentJump.Jumping(ref character.velocity, delta)) {
          jumping = false;
          forceJump = false;
          character.ExitState(States.Jumping);
        }
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
