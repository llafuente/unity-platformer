using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Climb a ladder
  /// </summary>
  public class CharacterActionLadder: CharacterAction {
    /// <summary>
    /// Ladder movement speed.
    /// </summary>
    [Comment("Ladder movement speed.")]
    public float speed = 6;
    /// <summary>
    /// Move character to the center of the ladder, instantly
    /// </summary>
    [Comment("Move character to the center of the ladder, instantly")]
    public bool moveToCenter = false;
    /// <summary>
    /// Maximum speed to snap to the center.
    /// </summary>
    [Comment("Maximum speed to snap to the center.")]
    public float towardsSpeed = 32;
    /// <summary>
    /// Time to reach the center (if towardsSpeed is fast enough).
    /// </summary>
    [Comment("Time to reach the center (if towardsSpeed is fast enough).")]
    public float towardsTime = 0.1f;

    [Space(10)]

    /// <summary>
    /// Dismount pressing left/right
    /// </summary>
    [Comment("Dismount pressing left/right")]
    public bool leftRightDismount = true;
    /// <summary>
    /// Time left/right need to be pressed to dismount
    /// </summary>
    [Comment("Time left/right need to be pressed to dismount")]
    public float dismountTime = 0.2f;
    /// <summary>
    /// Character can dismount ladders jumping?
    /// </summary>
    public bool dismountJumping = true;
    /// <summary>
    /// Jump with no direction pressed.
    /// </summary>
    [Comment("Jump with no direction pressed.")]
    public JumpConstantProperties jumpOff = new JumpConstantProperties(new Vector2(20, 20));

    [Space(10)]
    /// <summary>
    /// Action priority
    /// </summary>
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 10;
    /// <summary>
    /// CharacterActionJump
    /// </summary>
    CharacterActionJump actionJump;
    /// <summary>
    /// currently centering the Character?
    /// </summary>
    bool centering = false;
    /// <summary>
    /// Cooldown to enter in a ladder again
    /// </summary>
    Cooldown dismount;

    public override void OnEnable() {
      base.OnEnable();

      dismount = new Cooldown(dismountTime);
      actionJump = character.GetAction<CharacterActionJump>();
      if (actionJump == null) {
        Debug.LogError("CharacterActionLadder requires CharacterActionJump");
      }
    }
    /// <summary>
    /// Enter in ladder mode when user is in a ladder area and pressing up/down
    /// </summary>
    public override int WantsToUpdate(float delta) {
      bool onLadderState = character.IsOnState(States.Ladder);
      bool onLadderArea = character.IsOnArea(Areas.Ladder);
      // this means below my feet there is a ladder
      Ladder ladder = null;

      if (!onLadderArea && character.ladderBottom) {
        // deferred logic
        // if you EnableLadder now, you enter an area but there is no 'outisde-check'
        // so defer logic until also press down.
        onLadderArea = true;
        ladder = character.ladderBottom;
      }

      // in ladder state
      if (onLadderState) {
        return priority;
      }

      bool enter = false;
      // enter ladder condition
      if (onLadderArea && !onLadderState && (ladder != null || character.ladder != null)) {
        float dir = input.GetAxisRawY();
        // moving up, while inside a real LadderArea.
        if (dir > 0 && !(ladder ?? character.ladder).IsAtTop(character, character.feet)) {
          enter = true;
        } else if (dir < 0 && !(ladder ?? character.ladder).IsAtBottom(character, character.feet)) {
          // moving down: entering from the top
          if (ladder != null) {
            ladder.EnableLadder(character);

            // move the player inside the ladder.
            Vector3 pos = character.gameObject.transform.position;
            character.gameObject.transform.position = pos;

            enter = true;
          } else {
            // moving down: entering from the middle-bottom

            // check feet/bottom
            float bottomLadderY = character.ladder.GetBottom().y;
            float feetY = character.feet.y;

            // do not enter the ladder while on ground - pressing down
            if (feetY > bottomLadderY) {
              enter = true;
            }
          }
        }
      }

      if (enter) {
        return priority;
      }

      return 0;
    }
    /// <summary>
    /// EnterState and start centering
    /// </summary>
    public override void GainControl(float delta) {
      base.GainControl(delta);

      character.EnterState(States.Ladder);
      centering = moveToCenter;
      dismount.Reset();
    }

    public override void PerformAction(float delta) {
      // guard: something goes wrong!
      if (character.ladder == null) {
        character.ExitState(States.Ladder);
        return;
      }
      Ladder ladder = character.ladder;

      Vector2 in2d = input.GetAxisRaw();

      if (character.IsOnArea(Areas.Ladder) && character.IsOnState(States.Ladder)) {
        // disable x movement
        character.velocity.x = 0;
        character.velocity.y = speed * in2d.y;
      }

      // TODO transition
      if (centering) {
        float ladderCenter = ladder.GetComponent<BoxCollider2D>().bounds.center.x;
        float characterX = character.GetCenter().x;
        if (Math.Abs(characterX - ladderCenter) < 0.05) {
          centering = false;
          character.velocity.x = 0;
        } else {
          // instant move to the center of the ladder!
          Mathf.SmoothDamp(characterX, ladderCenter, ref character.velocity.x, towardsTime, towardsSpeed, delta);
        }
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
