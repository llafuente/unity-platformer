using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Grab 'something' and freeze.
  /// </summary>
  public class CharacterActionGrab: CharacterAction {
    #region public

    [Comment("Max speed to snap to the center.")]
    public float towardsSpeed = 32;
    [Comment("Time to reach the center (if towardsSpeed is fast enough).")]
    public float towardsTime = 0.1f;
    [Comment("Move character to the center of the grabArea")]
    public bool moveToCenter = false;
    [Comment("Character offset position for centering")]
    public Vector3 grabbingOffset = new Vector3(0, 0, 0);
    public bool dismountPressingDown = true;
    public bool dismountJumping = true;
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

    public override void Start() {
      base.Start();
      actionJump = character.GetAction<CharacterActionJump>();
      canGrab = new Cooldown(grabAgainCooldown);
    }

    public override int WantsToUpdate(float delta) {
      bool onGrabbableArea = character.IsOnArea(Areas.Grabbable);
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

      character.EnterState(States.Grabbing);
      centering = true;
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

      float x = input.GetAxisRawX();
      int faceDir;
      //TODO REVIEW this lead to some problems with orientation...
      if (x == 0) {
        faceDir = 0;
      } else {
        character.pc2d.collisions.faceDir = faceDir = (int) Mathf.Sign(x);
      }

      // check for dismount conditions
      if (dismountJumping && input.IsActionHeld(actionJump.action)) {
        canGrab.Reset();
        character.ExitState(States.Grabbing);

        actionJump.Jump(new JumpConstant(character,
          jumpOff.Clone(faceDir)
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
