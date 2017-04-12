using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Character Grab 'something', is centered and stop. ex: grab a ring
  /// </summary>
  public class CharacterActionGrab: CharacterAction {
    /// <summary>
    /// Maximum speed to snap to the center.
    /// </summary>
    [Comment("Max speed to snap to the center.")]
    public float towardsSpeed = 32;
    /// <summary>
    /// Time to reach the center (if towardsSpeed is fast enough).
    /// </summary>
    [Comment("Time to reach the center (if towardsSpeed is fast enough).")]
    public float towardsTime = 0.1f;
    /// <summary>
    /// Move character to the center of the grabArea
    /// </summary>
    [Comment("Move character to the center of the grabArea")]
    public bool moveToCenter = false;
    /// <summary>
    /// Character offset position while grabbing
    /// </summary>
    [Comment("Character offset position while grabbing")]
    public Vector3 grabbingOffset = new Vector3(0, 0, 0);
    /// <summary>
    /// Dismount 'Grab' pressing down axis
    /// </summary>
    public bool dismountPressingDown = true;
    /// <summary>
    /// Dismount 'Grab' jumping
    /// </summary>
    public bool dismountJumping = true;
    /// <summary>
    /// Dismount jump properties
    /// </summary>
    [Comment("Jump with no direction pressed.")]
    public JumpConstantProperties jumpOff = new JumpConstantProperties(new Vector2(20, 20));

    [Space(10)]

    /// <summary>
    /// Action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 20;
    /// <summary>
    /// Time after dismount before being able to grab again.
    /// </summary>
    public float grabAgainCooldown = 0.25f;
    /// <summary>
    /// CharacterActionJump
    /// </summary>
    CharacterActionJump actionJump;
    /// <summary>
    /// centering Character?
    /// </summary>
    bool centering = false;
    /// <summary>
    /// Cooldown for grabAgainCooldown
    /// </summary>
    Cooldown canGrab;

    public override void OnEnable() {
      base.OnEnable();

      actionJump = character.GetAction<CharacterActionJump>();
      canGrab = new Cooldown(grabAgainCooldown);
    }

    public override int WantsToUpdate(float delta) {
      bool onGrabbableArea = character.IsOnArea(Areas.Grabbable);

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

      character.EnterState(States.Grabbing);
      centering = true;
    }
    /// <summary>
    /// ExitState and no centering
    /// </summary>
    public override void LoseControl(float delta) {
      base.LoseControl(delta);

      character.ExitState(States.Grabbing);
      centering = false;
    }

    public override void PerformAction(float delta) {
      Vector3 ori = character.transform.position;
      Vector3 target = character.grab.GetCenter() - grabbingOffset;

      // once we center, stop moving!
      if (!centering) {
        character.velocity = Vector3.zero;
      }

      // close enough to our target position ?
      if (centering && Vector3.Distance(ori, target) < 0.1f) {
        character.velocity = target - ori;
        centering = false;
      }
      // close the gap a little more
      if (centering) {
        // centering phase
        Vector3.SmoothDamp(
          ori,
          target,
          ref character.velocity,
          towardsTime,
          towardsSpeed, //Mathf.Infinity,
          delta);
      }

      character.SetFacing(input.GetAxisRawX());

      // check for dismount conditions
      if (dismountJumping && input.IsActionHeld(actionJump.action)) {
        canGrab.Reset();
        character.ExitState(States.Grabbing);

        actionJump.Jump(new JumpConstant(character,
          jumpOff.Clone((int) character.faceDir)
        ));
      } else if (dismountPressingDown && input.GetAxisRawY() < 0) {
        // TODO Dismount down delay ?
        canGrab.Reset();
        character.ExitState(States.Grabbing);
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.NONE;
    }
  }
}
