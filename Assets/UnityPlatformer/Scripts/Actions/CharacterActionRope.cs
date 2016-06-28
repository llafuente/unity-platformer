using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Rope climb
  /// </summary>
  public class CharacterActionRope: CharacterAction {
    #region public

    public float climbSpeed = 6;
    [Comment("Max speed to snap to the center.")]
    public float towardsSpeed = 32;
    [Comment("Time to reach the center (if towardsSpeed is fast enough).")]
    public float towardsTime = 0.1f;
    [Comment("Move character to the center of the grabArea")]
    public bool moveToCenter = false;
    [Comment("Character offset position for centering")]
    public Vector3 grabbingOffset = new Vector3(0, 0, 0);
    //public bool dismountPressingDown = true;
    //public bool dismountJumping = true;
    [Comment("Jump with no direction pressed.")]
    public JumpConstantProperties jumpOff = new JumpConstantProperties(new Vector2(20, 20));

    [Space(10)]
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;

    public float grabAgainCooldown = 0.25f;

    #endregion

    CharacterActionJump actionJump;
    bool centering = false;
    Cooldown canGrab;
    float positionOfSection = 0.5f;

    public override void OnEnable() {
      base.OnEnable();

      actionJump = character.GetAction<CharacterActionJump>();
      canGrab = new Cooldown(grabAgainCooldown);
    }

    public override int WantsToUpdate(float delta) {
      bool onGrabbableArea = character.IsOnArea(Areas.Rope);
      canGrab.Increment();

      if (onGrabbableArea && canGrab.Ready()) {
        return priority;
      }
      return 0;
    }

    /// <summary>
    /// EnterState and start centering
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.Rope);
      centering = true;
      positionOfSection = 0.5f;
    }

    public override void PerformAction(float delta) {
      Vector3 ori = character.transform.position;
      Vector3 ropePosition = character.rope.sections[character.ropeIndex].GetComponent<RopeSection>().GetPositionInSection(positionOfSection);
      Vector3 target = ropePosition + character.rope.faceDir * grabbingOffset;

      // close enough to our target position ?
      if (centering && Vector3.Distance(ori, target) < 0.1f) {
        character.velocity = target - ori;
        centering = false;
      } else if (centering) {
        // centering phase: close the gap a little more
        Vector3.SmoothDamp(
          ori,
          target,
          ref character.velocity,
          towardsTime,
          towardsSpeed, //Mathf.Infinity,
          delta);
      } else {
        // once we center, follow the rope movement
        character.velocity = (target - ori) / delta;
      }

      // climb / descend the rope
      float offsetSeed = character.rope.SpeedToSectionOffset(climbSpeed) * delta;
      float y = input.GetAxisRawY();

      if (y > 0) {
        positionOfSection = Mathf.Clamp01(positionOfSection + offsetSeed);
        if (positionOfSection == 1) {
          // go to up if possible
          if (character.ropeIndex != 0) {
            --character.ropeIndex;
            positionOfSection = 0;
          }
        }
      } else if (y < 0) {
        positionOfSection = Mathf.Clamp01(positionOfSection - offsetSeed);
        if (positionOfSection == 0) {
          // go to up if possible
          if (character.ropeIndex != character.rope.segments - 1) {
            ++character.ropeIndex;
            positionOfSection = 1;
          } else {
            // TODO maybe we can dismount here :D
          }
        }
      }


      // check for dismount conditions
      if (/*dismountJumping && */input.IsActionHeld(actionJump.action)) {
        canGrab.Reset();
        character.ExitState(States.Rope);

        actionJump.Jump(new JumpConstant(character,
          jumpOff.Clone(character.pc2d.collisions.faceDir)
        ));
      }

    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.NONE;
    }
  }
}
