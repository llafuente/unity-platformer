using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Stick at walls. Also perform wall-jumps
  /// </summary>
  public class CharacterActionWallStick: CharacterAction, IUpdateManagerAttach {
    #region public

    [Comment("Vertical terminal velocity while stick")]
	  public float wallSlideSpeedMax = 3;
    [Comment("Time player need to oppose walkstick to leave / press in the other direction.")]
	  public float wallStickLeaveTime = 0.25f;
    // TODO REVIEW this may be a boolean, and a pointer to a CharacterActionJump
    [Comment("If you want to enable Jumping. Use empty string to disable. Must be the same as CharacterActionJump use")]
    public string jumpAction = "Jump";
    [Comment("Jump in the same direction as the wall. Climb")]
    public Vector2 wallJumpClimb = new Vector2(10, 35);
    [Comment("Jump with no direction pressed.")]
    public Vector2 wallJumpOff = new Vector2(20, 20);
    [Comment("Jump in the opposite direction")]
    public Vector2 wallLeap = new Vector2(20, 20);

    [Comment("Remember: Higher priority wins. Modify with caution. Tip: Higher than Jump")]
    public int priority = 7;

    #endregion

	  float timeToWallStickLeave;

    public override void Start() {
      base.Start();

      timeToWallStickLeave = wallStickLeaveTime;
    }

    public void Attach(UpdateManager um) {
    }

    /// <summary>
    /// When Character is colliding left or right but now below
    /// and falling! Stick!
    /// </summary>
    public override int WantsToUpdate(float delta) {
      return (
        (controller.collisions.left || controller.collisions.right) &&
        !controller.collisions.below &&
        character.velocity.y < 0
        ) ? priority : 0;
    }

    public override void PerformAction(float delta) {
      int wallDirX = (controller.collisions.left) ? -1 : 1;
      float x = input.GetAxisRawX();

      if (character.velocity.y < -wallSlideSpeedMax) {
        character.velocity.y = -wallSlideSpeedMax;
      }

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

      // jump
      if (jumpAction.Length != 0 && input.IsActionDown(jumpAction)) {
        if (wallDirX == x) {
          character.velocity.x = -wallDirX * wallJumpClimb.x;
          character.velocity.y = wallJumpClimb.y;
        } else if (x == 0) {
          character.velocity.x = -wallDirX * wallJumpOff.x;
          character.velocity.y = wallJumpOff.y;
        } else {
          character.velocity.x = -wallDirX * wallLeap.x;
          character.velocity.y = wallLeap.y;
        }
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
