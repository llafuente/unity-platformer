using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Crounch
  /// </summary>
  public class CharacterActionCrounch: CharacterAction {
    #region public

    // TODO OnValidate check this!
    [Space(10)]
    [Comment("Must match something @PlatformerInput")]
    public String action = "Crounch";
    public bool enablePressingDown = true;

    #endregion

    #region private

    bool crounchHeld = false;

    #endregion

    public override void OnEnable() {
      base.OnEnable();

      if (action != "") {
        input.onActionUp += OnActionUp;
        input.onActionDown += OnActionDown;
      }
    }

    public void OnActionDown(string _action) {
      if (_action == action) {
        crounchHeld = true;
      }
    }

    public void OnActionUp(string _action) {
      // when button is up, reset, and allow a new jump
      if (_action == action) {
        crounchHeld = false;
      }
    }

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
