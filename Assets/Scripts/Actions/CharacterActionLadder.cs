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
      // enter ladder condition
      if (character.IsOnArea(Character.Areas.Ladder) &&
        input.GetAxisRawY() != 0 &&
        !character.IsOnState(Character.States.Ladder)
      ) {
        character.state |= Character.States.Ladder;
        moveToCenterNow = moveToCenter;
        return priority;
      }

      // in ladder state
      if (character.IsOnState(Character.States.Ladder)) {
        return 10;
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
        // instant move to the center of the ladder!
        character.velocity.x = (character.ladderCenter - controller.GetComponent<BoxCollider2D>().bounds.center.x) / delta;
        moveToCenterNow = false;
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.NONE;
    }
  }
}
