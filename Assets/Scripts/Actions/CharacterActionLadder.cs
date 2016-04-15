using System;
using UnityEngine;
using UnityPlatformer.Characters;
using UnityPlatformer.Tiles;

namespace UnityPlatformer.Actions {
  /// <summary>
  /// Perform an action over a character
  /// </summary>
  public class CharacterActionLadder: CharacterAction, IUpdateManagerAttach {
    #region public

    [Comment("Ladder movement speed.")]
    public float speed = 6;
    [Comment("Move character to the center of the ladder, instantly")]
    public bool moveToCenter = false;
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 10;

    #endregion

    bool moveToCenterNow = false;

    public void Attach(UpdateManager um) {
    }

    /// <summary>
    /// Enter in ladder mode when user is in a ladder area and pressing up/down
    /// </summary>
    public override int WantsToUpdate(float delta) {
      bool onLadderState = character.IsOnState(Character.States.Ladder);
      bool onLadderArea = character.IsOnArea(Character.Areas.Ladder);
      // this means below my feet there is a ladder
      Ladder ladder = null;

      if (!onLadderArea) {
        // check out feet, maybe there is a ladder below...
        RaycastHit2D hit = character.controller.DoFeetRay(
          Configuration.instance.minDistanceToEnv * 2,
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
        if (dir > 0 && ladder == null) {
          enter = true;
        } else if (dir < 0) {
          // moving down: entering from the top
          if (ladder != null) {
            ladder.EnableLadder(character);

            // move the player inside the ladder.
            Vector3 pos = character.gameObject.transform.position;
            pos.y -= Configuration.instance.minDistanceToEnv * 2;
            character.gameObject.transform.position = pos;

            enter = true;
          } else {
            // moving down: entering from the middle-bottom

            // check feet/bottom
            float bottomLadderY = character.ladder.GetBottom().y;
            // add minDistanceToEnv to give some extra margin
            float feetY = character.GetFeetPosition().y - Configuration.instance.minDistanceToEnv;

            // do not enter the ladder while on ground - pressing down
            if (feetY > bottomLadderY) {
              enter = true;
            }
          }
        }
      }

      if (enter) {
        character.EnterState(Character.States.Ladder);
        moveToCenterNow = moveToCenter;
        return priority;
      }

      return 0;
    }

    public override void PerformAction(float delta) {
      Vector2 in2d = input.GetAxisRaw();

      if (character.IsOnArea(Character.Areas.Ladder) && character.IsOnState(Character.States.Ladder)) {
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

      // check ladder top
      Vector3 topLadder = character.ladder.GetTop(); // TODO cache?
      Vector3 bottomLadder = character.ladder.GetBottom(); // TODO cache?

      //Utils.DrawPoint(topLadder);
      Utils.DrawPoint(character.GetFeetPosition());

      //Debug.LogFormat("feet: {0} ladder {1}", character.GetFeetPosition(), topLadder);
      if (character.GetFeetPosition().y > topLadder.y) {
        //Debug.LogFormat("TOP REACHED!! feet: {0} ladder {1}", character.GetFeetPosition(), topLadder);
        character.velocity = Vector2.zero;
        //Vector3 pos = character.gameObject.transform.position;
        //pos.y = topLadder.y;
        //character.gameObject.transform.position = pos;
        character.ladder.DisableLadder(character);
      }

      if (character.GetFeetPosition().y < bottomLadder.y) {
        //Debug.LogFormat("BOTTOM REACHED!! feet: {0} ladder {1}", character.GetFeetPosition(), bottomLadder);
        character.velocity = Vector2.zero;
        //Vector3 pos = character.gameObject.transform.position;
        //pos.y = topLadder.y;
        //character.gameObject.transform.position = pos;
        character.ladder.Dismount(character);
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.NONE;
    }
  }
}
