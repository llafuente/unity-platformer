using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Rope climb
  /// </summary>
  public class CharacterActionRope: CharacterAction {
    /// <summary>
    /// Climb speed
    /// </summary>
    public float climbSpeed = 6;
    /// <summary>
    /// Max speed to snap to the center.
    /// </summary>
    [Comment("Max speed to snap to the center.")]
    public float towardsSpeed = 32;
    /// <summary>
    /// Time to reach the center (if towardsSpeed is fast enough).
    /// </summary>
    [Comment("Time to reach the center (if towardsSpeed is fast enough).")]
    public float towardsTime = 0.1f;
    /// <summary>
    /// Move character to the center of the Rope
    /// </summary>
    [Comment("Move character to the center of the Rope")]
    public bool moveToCenter = false;
    /// <summary>
    /// Character offset position while grabbing
    /// </summary>
    [Comment("Character offset position while grabbing")]
    public Vector3 grabbingOffset = new Vector3(0, 0, 0);
    /// <summary>
    /// Character can dismount pressing jump
    /// </summary>
    public bool dismountJumping = true;
    /// <summary>
    /// Dismount jump properties
    /// </summary>
    [Comment("Dismount jump properties")]
    public JumpConstantProperties jumpOff = new JumpConstantProperties(new Vector2(20, 20));

    [Space(10)]

    /// <summary>
    /// Action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;
    /// <summary>
    /// Time to wait before grab again a Rope, should be enough to let the
    /// player leave the Rope Area
    /// </summary>
    public float grabAgainCooldown = 0.25f;
    /// <summary>
    /// CharacterActionJump
    /// </summary>
    CharacterActionJump actionJump;
    /// <summary>
    /// is action centering the character in the rope
    /// </summary>
    bool centering = false;
    /// <summary>
    /// Grab again cooldown
    /// </summary>
    Cooldown canGrab;
    /// <summary>
    /// Current position in the SectionRope
    /// </summary>
    float positionOfSection = 0.5f;

    public override void OnEnable() {
      base.OnEnable();

      actionJump = character.GetAction<CharacterActionJump>();
      if (actionJump == null && dismountJumping) {
        Debug.LogError("CharacterActionJump is required in CharacterActionRope because dismountJumping is true");
      }

      canGrab = new Cooldown(grabAgainCooldown);
    }

    public override int WantsToUpdate(float delta) {
      bool onGrabbableArea = character.IsOnArea(Areas.Rope);

      if (onGrabbableArea && canGrab.Ready() && character.rope.passengers.Contains(character.gameObject.layer)) {
        return priority;
      }
      return 0;
    }
    /// <summary>
    /// EnterState and start centering and listen rope breakage
    ///
    /// TODO set positionOfSection = 0.5f, we should calc the closest...
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.Rope);
      centering = true;
      positionOfSection = 0.5f;

      character.rope.onBreak += OnBreakRope;
    }
    /// <summary>
    /// stop listenting rope break
    /// </summary>
    public override void LoseControl(float delta) {
      if (character.rope != null) {
        character.rope.onBreak -= OnBreakRope;
      }
    }
    /// <summary>
    /// Let the Character fall
    /// </summary>
    void OnBreakRope(Rope rope) {
      LoseControl(0.0f);
      character.ExitState(States.Rope);
      character.ExitArea(Areas.Rope);
      character.rope = null;
    }
    /// <summary>
    /// Move up/down and sync with Rope movement
    /// </summary>
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
      if (dismountJumping && input.IsActionHeld(actionJump.action)) {
        canGrab.Reset();
        character.ExitState(States.Rope);

        actionJump.Jump(new JumpConstant(character,
          jumpOff.Clone((int) character.faceDir)
        ));
      }

    }
    /// <summary>
    /// Do nothing, while on a Rope there shoud be no world to collide
    /// </summary>
    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.NONE;
    }
  }
}
