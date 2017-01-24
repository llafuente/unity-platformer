using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Stick at walls. Also perform wall-jumps
  /// </summary>
  public class CharacterActionWallStick: CharacterAction {
    /// <summary>
    /// Vertical terminal velocity while stick
    /// </summary>
    [Comment("Vertical terminal velocity while stick")]
    public float wallSlideSpeedMax = 3;
    /// <summary>
    /// Time player need to oppose walkstick to leave / press in the other direction.
    /// </summary>
    [Comment("Time player need to oppose walkstick to leave / press in the other direction.")]
    public float wallStickLeaveTime = 0.25f;
    /// <summary>
    /// After wall-jump, time that CharacterActionWallStick will be disabled
    /// </summary>
    public float wallStickLeaveAgain = 0.25f;
    /// <summary>
    /// Character can use wall-jumps ?
    /// </summary>
    public bool enableWallJumps = true;
    /// <summary>
    /// CharacterActionJump
    /// </summary>
    public CharacterActionJump actionJump;
    /// <summary>
    /// Climb jump: Jump in the same direction as the wall.
    /// </summary>
    [Comment("Climb jump: Jump in the same direction as the wall.")]
    public JumpConstantProperties wallJumpClimb = new JumpConstantProperties(new Vector2(10, 35));
    /// <summary>
    /// Jump with no direction pressed.
    /// </summary>
    [Comment("Jump with no direction pressed.")]
    public JumpConstantProperties wallJumpOff = new JumpConstantProperties(new Vector2(20, 20));
    /// <summary>
    /// Jump in the opposite direction
    /// </summary>
    [Comment("Jump in the opposite direction")]
    public JumpConstantProperties wallLeap = new JumpConstantProperties(new Vector2(20, 20));

    [Space(10)]

    /// <summary>
    /// action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution. Tip: Higher than Jump")]
    public int priority = 7;
    /// <summary>
    /// Frame sliding
    /// </summary>
    internal int slidingFrames = 0;
    /// <summary>
    /// counter to compare against wallStickLeaveTime
    /// </summary>
    float timeToWallStickLeave;
    /// <summary>
    /// wallStickLeaveAgain converted to frames
    /// </summary>
    int wallStickLeaveAgainFrames;
    /// <summary>
    /// counter to compare against wallStickLeaveAgainFrames
    /// </summary>
    int wallStickLeaveAgainCounter;
    /// <summary>
    /// Initialization
    /// </summary>
    public override void OnEnable() {
      base.OnEnable();

      timeToWallStickLeave = wallStickLeaveTime;

      wallStickLeaveAgainFrames = UpdateManager.GetFrameCount (wallStickLeaveAgain);
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

      // disable when ladder is close or water...
      if (character.IsOnArea(Areas.Liquid) || character.IsOnArea(Areas.Ladder)) {
        return 0;
      }

      if ((
        (pc2d.collisions.left && pc2d.collisions.leftIsWall && character.faceDir == Facing.Left) ||
        (pc2d.collisions.right && pc2d.collisions.rightIsWall && character.faceDir == Facing.Right)
      ) &&
        !pc2d.collisions.below &&
        character.velocity.y < 0
      ) {
        return priority;
      }

      return 0;
    }
    /// <summary>
    /// Reset SmoothDamp and enter state States.WallSliding
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.WallSliding);
      slidingFrames = 0;
    }
    /// <summary>
    /// Stick, then slide down. Can dismount jumping if enableWallJumps
    /// </summary>
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
    /// <summary>
    /// default behaviour
    /// </summary>
    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
