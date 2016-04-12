using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Perform an action over a character
  /// </summary>
  [RequireComponent (typeof (PlatformerController))]
  [RequireComponent (typeof (Controller2D))]
  [RequireComponent (typeof (Character))]
  public class CharacterActionLadder: CharacterAction, UpdateManagerAttach {

    public float speed = 6;

    bool moveToCenter = false;

    public void Attach(UpdateManager um) {
    }

    /// <summary>
    /// Tells the character we want to take control
    /// Positive numbers fight: Higher number wins
    /// TODO REVIEW Negative numbers are used to ignore fight, and execute.
    /// </summary>
    public override int WantsToUpdate() {
      // enter ladder condition
      if (character.IsOnLadder() &&
        input.GetAxisRawY() != 0 &&
        !character.IsOnState(Character.States.Ladder)
      ) {
        character.state |= Character.States.Ladder;
        moveToCenter = true;
        return 10;
      }

      // in ladder state
      if (character.IsOnState(Character.States.Ladder)) {
        return 10;
      }

      return 0;
    }

    public override void PerformAction(float delta) {
      Vector2 in2d = input.GetAxisRaw();

      if (character.IsOnLadder () && character.IsOnState(Character.States.Ladder)) {
        // disable x movement
	      character.velocity.x = 0;
	      character.velocity.y = speed * in2d.y;
	    }

      // TODO transition
      if (moveToCenter) {
	      // instant move to the center of the ladder!
	      character.velocity.x = (character.ladderCenter - controller.GetComponent<BoxCollider2D>().bounds.center.x) / delta;
        moveToCenter = false;
	    }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.NONE;
    }
  }
}
