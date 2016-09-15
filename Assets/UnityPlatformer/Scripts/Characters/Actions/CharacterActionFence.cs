using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Move in a fence. The character must be completely inside the fence to
  /// enter
  /// </summary>
  public class CharacterActionFence: CharacterAction {
    #region public

    [Comment("Ladder movement speed.")]
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

    CharacterActionJump actionJump;

    public override void OnEnable() {
      base.OnEnable();

      actionJump = character.GetAction<CharacterActionJump>();
      if (actionJump == null) {
        Debug.LogError("CharacterActionFence requires CharacterActionJump");
      }
    }

    /// <summary>
    /// Enter in ladder mode when user is in a ladder area and pressing up/down
    /// </summary>
    public override int WantsToUpdate(float delta) {
      bool onFenceState = character.IsOnState(States.Fence);
      bool onFenceArea = character.IsOnArea(Areas.Fence);

      if (onFenceState || onFenceArea) {
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
      if (character.ladder == null) {
        character.ExitState(States.Fence);
        return;
      }
      Ladder ladder = character.ladder;

      Vector2 in2d = input.GetAxisRaw();

      if (character.IsOnArea(Areas.Fence) && character.IsOnState(States.Fence)) {
        // disable x movement
        character.velocity.x = 0;
        character.velocity.y = speed * in2d.y;
      }

      if (ladder.topDismount && ladder.IsAtTop(character, character.feet) && in2d.y > 0) {
        // top dismount enabled and reached
        character.velocity = Vector2.zero;
        ladder.Dismount(character);
      } else if (ladder.bottomDismount && ladder.IsAtBottom(character, character.feet) && in2d.y < 0) {
        // bottom dismount enabled and reached
        character.velocity = Vector2.zero;
        ladder.Dismount(character);
      } else if (!ladder.topDismount && ladder.IsAboveTop(character, character.head) && in2d.y > 0) {
        // can't dismount (vine) don't let the head 'overflow' the ladder
        character.velocity = Vector2.zero;
      } else if (!ladder.bottomDismount && ladder.IsAtBottom(character, character.feet) && in2d.y < 0) {
        // can't dismount (vine) don't let the head 'overflow' the ladder
        // caveat: Vine cannot be near ground
        character.velocity = Vector2.zero;
      }

      character.SetFacing(in2d.x);

      // check for dismount conditions
      if (in2d.x != 0) {
        // do not allow to jump without telling the direction.
        // move up if you want it
        if (dismountJumping && input.IsActionHeld(actionJump.action)) {
          dismount.Reset();
          character.ladder.Dismount(character);
          character.ladder = null;

          actionJump.Jump(new JumpConstant(character,
            jumpOff.Clone((int) character.faceDir)
          ));
        } else if (dismount.IncReady()) {
          character.velocity = Vector2.zero;
          character.ladder.Dismount(character);
          character.ladder = null;
        }
      } else {
        dismount.Reset();
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.NONE;
    }
  }
}
