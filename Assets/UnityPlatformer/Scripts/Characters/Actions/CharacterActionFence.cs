using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Move in a fence. The character must be completely inside the fence to
  /// enter
  /// </summary>
  public class CharacterActionFence: CharacterAction {
    #region public

    [Comment("Fence movement speed.")]
    public Vector2 speed = new Vector2(6, 6);

    [Space(10)]
    [Comment("Dismount pressing left/right")]
    public string action = "Pull";
    public bool dismountJumping = true;
    [Comment("Jump with no direction pressed.")]
    public JumpConstantProperties jumpOff = new JumpConstantProperties(new Vector2(20, 20));

    [Space(10)]
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 10;

    #endregion

    protected bool grabHeld;
    protected CharacterActionJump actionJump;

    public override void OnEnable() {
      base.OnEnable();

      actionJump = character.GetAction<CharacterActionJump>();
      if (actionJump == null) {
        Debug.LogError("CharacterActionFence requires CharacterActionJump");
      }

      input.onActionUp += OnActionUp;
      input.onActionDown += OnActionDown;
    }

    public void OnActionDown(string _action) {
      if (_action == action) {
        grabHeld = true;
      }
    }

    public void OnActionUp(string _action) {
      // when button is up, reset, and allow a new jump
      if (_action == action) {
        grabHeld = false;
      }
    }

    /// <summary>
    /// Enter in fence mode when user is in a fence area and pressing up/down
    /// </summary>
    public override int WantsToUpdate(float delta) {
      bool onFenceState = character.IsOnState(States.Fence);
      bool onFenceArea = character.IsOnArea(Areas.Fence);

      if (grabHeld && (onFenceState || onFenceArea)) {
        return priority;
      }

      return 0;
    }

    /// <summary>
    /// EnterState
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.Fence);
    }

    public override void PerformAction(float delta) {
      // guard: something goes wrong!
      if (character.fence == null) {
        character.ExitState(States.Fence);
        return;
      }
      Fence fence = character.fence;
      Vector2 in2d = input.GetAxisRaw();
      Vector3 velocity = new Vector3(speed.x * in2d.x, 0, 0);

      // is difficult to move a Bound, so just move min/max point instead
      Bounds b = character.enterAreas.body.bounds;
      Vector3 pmin = b.min;
      Vector3 pmax = b.max;

      // test move X, if NOK do not move X
      if (!character.fence.body.Contains(pmin + velocity * delta, pmax + velocity * delta)) {
        velocity.x = 0;
      }

      // test move Y, if NOK do not move Y
      velocity.y = speed.y * in2d.y;
      if (!character.fence.body.Contains(pmin + velocity * delta, pmax + velocity * delta)) {
        velocity.y = 0;
      }

      character.velocity = velocity;
      character.SetFacing(in2d.x);

      // check for dismount conditions
      // do not allow to jump without telling the direction.
      // move up if you want it
      if (dismountJumping && input.IsActionHeld(actionJump.action)) {
        //dismount.Reset();
        character.fence.Dismount(character);
        character.fence = null;

        actionJump.Jump(new JumpConstant(character,
          jumpOff.Clone((int) character.faceDir)
        ));
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.NONE;
    }
  }
}
