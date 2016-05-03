using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Climb a ladder
  /// TODO moveToCenterTime/Speed
  /// </summary>
  public class CharacterActionLadder: CharacterAction {
    #region public

    [Comment("Ladder movement speed.")]
    public float speed = 6;
    [Comment("Move character to the center of the ladder, instantly")]
    public bool moveToCenter = false;
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 10;

    #endregion

    bool moveToCenterNow = false;

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
        RaycastHit2D hit = character.controller.DoFeetRay(
          character.controller.skinWidth * 2,
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
      if (onLadderArea && !onLadderState) {
        float dir = input.GetAxisRawY();
        // moving up, while inside a real LadderArea.
        if (dir > 0 && !(ladder ?? character.ladder).IsAtTop(character)) {
          enter = true;
        } else if (dir < 0 && !(ladder ?? character.ladder).IsAtBottom(character)) {
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
            float feetY = character.GetFeetPosition().y;

            // do not enter the ladder while on ground - pressing down
            if (feetY > bottomLadderY) {
              enter = true;
            }
          }
        }
      }

      if (enter) {
        character.EnterState(States.Ladder);
        moveToCenterNow = moveToCenter;
        return priority;
      }

      return 0;
    }

    public override void PerformAction(float delta) {
      Vector2 in2d = input.GetAxisRaw();

      if (character.IsOnArea(Areas.Ladder) && character.IsOnState(States.Ladder)) {
        // disable x movement
        character.velocity.x = 0;
        character.velocity.y = speed * in2d.y;
      }

      // TODO transition
      if (moveToCenterNow) {
        float ladderCenter = character.ladder.body.bounds.center.x;
        // instant move to the center of the ladder!
        character.velocity.x = (ladderCenter - controller.GetComponent<BoxCollider2D>().bounds.center.x) / delta;
        moveToCenterNow = false;
      }

      if (character.ladder.IsAtTop(character) && in2d.y > 0) {
        character.velocity = Vector2.zero;
        character.ladder.Dismount(character);
      } else if (character.ladder.IsAtBottom(character) && in2d.y < 0) {
        character.velocity = Vector2.zero;
        character.ladder.Dismount(character);
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.NONE;
    }
  }
}
