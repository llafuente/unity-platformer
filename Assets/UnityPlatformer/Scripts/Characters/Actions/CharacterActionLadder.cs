using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Climb a ladder
  /// </summary>
  public class CharacterActionLadder: CharacterAction {
    #region public

    [Comment("Ladder movement speed.")]
    public float speed = 6;
    [Comment("Move character to the center of the ladder, instantly")]
    public bool moveToCenter = false;
    [Comment("max speed to snap to the center.")]
    public float towardsSpeed = 32;
    [Comment("time to reach the center (if towardsSpeed is fast enough).")]
    public float towardsTime = 0.1f;

    [Space(10)]
    [Comment("Dismount pressing left/right")]
    public bool leftRightDismount = true;
    [Comment("Time left/right need to be pressed to dismount")]
    public float dismountTime = 0.2f;
    public bool dismountJumping = true;
    [Comment("Jump with no direction pressed.")]
    public JumpConstantProperties jumpOff = new JumpConstantProperties(new Vector2(20, 20));

    [Space(10)]
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 10;

    #endregion

    CharacterActionJump actionJump;
    bool centering = false;
    Cooldown dismount;

    public override void OnEnable() {
      base.OnEnable();

      dismount = new Cooldown(dismountTime);
      actionJump = character.GetAction<CharacterActionJump>();
    }

    /// <summary>
    /// Enter in ladder mode when user is in a ladder area and pressing up/down
    /// </summary>
    public override int WantsToUpdate(float delta) {
      bool onLadderState = character.IsOnState(States.Ladder);
      bool onLadderArea = character.IsOnArea(Areas.Ladder);
      // this means below my feet there is a ladder
      Ladder ladder = null;

      if (!onLadderArea) {
        // check out feet, maybe there is a ladder below...
        RaycastHit2D hit = pc2d.DoFeetRay(
          pc2d.skinWidth * 2,
          Configuration.instance.laddersMask
        );
        if (hit) {
          ladder = hit.collider.gameObject.GetComponent<Ladder>();
          if (ladder == null) {
            Debug.LogWarning(hit.collider.gameObject + " in ladder mask don't have a Ladder component!");
          } else {
            Debug.Log("below there is a ladder!!");
            // deferred logic
            // if you EnableLadder now, you enter an area but there is no 'outisde-check'
            // so defer logic until also press down.
            onLadderArea = true;
          }
        }
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
