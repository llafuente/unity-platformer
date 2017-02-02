using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Crounch
  /// </summary>
  public class CharacterActionCrounch: CharacterAction {
    /// <summary>
    /// Input action name
    /// </summary>
    [Comment("Must match something @PlatformerInput")]
    public String action = "Crounch";
    /// <summary>
    /// Input action name
    /// </summary>
    bool crounchHeld = false;

    public override void OnEnable() {
      base.OnEnable();

      if (action != "") {
        input.onActionUp += OnActionUp;
        input.onActionDown += OnActionDown;
      }
    }
    /// <summary>
    /// input.onActionDown
    /// </summary>
    public void OnActionDown(string _action) {
      if (_action == action) {
        crounchHeld = true;
      }
    }
    /// <summary>
    /// input.onActionUp
    /// </summary>
    public void OnActionUp(string _action) {
      // when button is up, reset, and allow a new jump
      if (_action == action) {
        crounchHeld = false;
      }
    }

    // TODO REVIEW this is not a problem, if player attack and crounch
    // 1 - animation could be wrong if forceAnimation is not set
    // 2 - hitboxes will be wrong, because it's not really crouching yet

    /// <summary>
    /// crounch if onground and no ladder below
    /// </summary>
    public override int WantsToUpdate(float delta) {
      float dir = input.GetAxisRawY();

      if (crounchHeld || (dir < 0 && character.ladderBottom == null)) {
        character.EnterStateGraceful(States.Crounch);
        return -1;
      }

      character.ExitStateGraceful(States.Crounch);

      return 0;
    }

    public override void PerformAction(float delta) {
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
