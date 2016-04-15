using System;
using UnityEngine;
using UnityPlatformer.Characters;

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
      // in ladder state
      if (character.IsOnState(Character.States.Ladder)) {
        return priority;
      }

      // enter ladder condition
      if (character.IsOnArea(Character.Areas.Ladder) &&
        !character.IsOnState(Character.States.Ladder)
      ) {
        float dir = input.GetAxisRawY();
        if (dir > 0) {
          character.EnterState(Character.States.Ladder);
          moveToCenterNow = moveToCenter;
          return priority;
        } else if (dir < 0) {
          // check feet/bottom
          float bottomLadderY = character.ladder.GetBottom().y;
          // add minDistanceToEnv to give some extra margin
          // and no enter ladder while on bottom ground
          float feetY = character.GetFeetPosition().y - Configuration.instance.minDistanceToEnv;

          if (feetY > bottomLadderY) {
            character.EnterState(Character.States.Ladder);
            moveToCenterNow = moveToCenter;
            return priority;
          }
        }
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
      //Utils.DrawPoint(character.GetFeetPosition());

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
