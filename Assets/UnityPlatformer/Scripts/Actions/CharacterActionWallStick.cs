using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Stick at walls. Also perform wall-jumps
  /// </summary>
  public class CharacterActionWallStick: CharacterAction {
    #region public

    [Comment("Vertical terminal velocity while stick")]
	  public float wallSlideSpeedMax = 3;
    [Comment("Time player need to oppose walkstick to leave / press in the other direction.")]
	  public float wallStickLeaveTime = 0.25f;
	  public float wallStickLeaveAgain = 0.25f;
    public bool enableWallJumps = true;
    public CharacterActionJump actionJump;
    [Comment("Jump in the same direction as the wall. Climb")]
    public JumpConstantProperties wallJumpClimb = new JumpConstantProperties(new Vector2(10, 35));
    [Comment("Jump with no direction pressed.")]
    public JumpConstantProperties wallJumpOff = new JumpConstantProperties(new Vector2(20, 20));
    [Comment("Jump in the opposite direction")]
    public JumpConstantProperties wallLeap = new JumpConstantProperties(new Vector2(20, 20));

    [Space(10)]
    [Comment("Remember: Higher priority wins. Modify with caution. Tip: Higher than Jump")]
    public int priority = 7;

    #endregion

    #region private

	  float timeToWallStickLeave;
	  int wallStickLeaveAgainFrames;
	  int wallStickLeaveAgainCounter;
	  int slidingFrames = 0;

    #endregion

    public override void Start() {
      base.Start();

      timeToWallStickLeave = wallStickLeaveTime;

      wallStickLeaveAgainFrames = UpdateManager.instance.GetFrameCount (wallStickLeaveAgain);
      wallStickLeaveAgainCounter = wallStickLeaveAgainFrames + 1; // can wallstick

      if (enableWallJumps && actionJump == null) {
        Debug.LogWarning("enableWallJumps is true but there is no actionJump selected!");
      }

    }

    /// <summary>
    /// When Character is colliding left or right but now below
    /// and falling! Stick!
    /// </summary>
    public override int WantsToUpdate(float delta) {
      ++wallStickLeaveAgainCounter;
      if (++wallStickLeaveAgainCounter < wallStickLeaveAgainFrames) {
        return 0;
      }

      if ((
        (pc2d.collisions.left && pc2d.collisions.leftIsWall) ||
        (pc2d.collisions.right && pc2d.collisions.rightIsWall)
      ) &&
        !pc2d.collisions.below &&
        character.velocity.y < 0
      ) {
        return priority;
      }

      return 0;
    }

    /// <summary>
    /// Reset SmoothDamp
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.WallSliding);
      slidingFrames = 0;
    }

    public override void PerformAction(float delta) {
      ++slidingFrames;

      int wallDirX = (pc2d.collisions.left) ? -1 : 1;
      float x = input.GetAxisRawX();

      // terminal velocity
      // NOTE apply -gravity to compensate
      if (character.velocity.y < -wallSlideSpeedMax) {
        character.velocity.y = -wallSlideSpeedMax - character.pc2d.gravity.y * delta;
      }

      // TODO manage in frames
      if (timeToWallStickLeave > 0) {
        character.velocity.x = 0;

        if (x != wallDirX && x != 0) {
          timeToWallStickLeave -= delta;
        }
        else {
          timeToWallStickLeave = wallStickLeaveTime;
        }
      }
      else {
        timeToWallStickLeave = wallStickLeaveTime;
      }

      // Jump
      if (enableWallJumps && input.IsActionHeld(actionJump.action)) {
        wallStickLeaveAgainCounter = 0; // reset!
        JumpConstant jump;
        character.ExitState(States.WallSliding);
        slidingFrames = 0;

        if (wallDirX == x) {
          jump = new JumpConstant(character,
            wallJumpClimb.Clone(-wallDirX)
          );
        } else if (x == 0) {
          jump = new JumpConstant(character,
            wallJumpOff.Clone(-wallDirX)
          );
        } else {
          jump = new JumpConstant(character,
            wallLeap.Clone(-wallDirX)
          );
        }
        actionJump.Jump(jump);
      }

    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
