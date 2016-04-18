using System;
using UnityEngine;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Actions {
  /// <summary>
  /// Perform an action over a character
  /// TODO Hangtime: The amount of time the player may spend hanging in
  /// midair at the apex of her jump (while the jump is not canceled).
  /// </summary>
  public class CharacterActionJump: CharacterAction {
    #region public

    // TODO OnValidate check this!
    [Comment("Must match something in @PlatformerInput")]
    public String action;
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 5;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float graceJumpTime = 0.25f;
    public float timeToJumpApex = .4f;

    #endregion

    #region private

    bool jumping = false;
    bool detachJumping = false;

    float gravity;
    Jump jump;
    int _graceJumpFrames = 10;

    #endregion

    public override void Start() {
      base.Start();

      gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
      jump = new Jump(gravity, timeToJumpApex, minJumpHeight);
      //!!! Debug.LogFormat("(CharacterActionJump) gravity {0} timeToJumpApex {1} minJumpHeight", gravity, timeToJumpApex, minJumpHeight);

      _graceJumpFrames = UpdateManager.instance.GetFrameCount (graceJumpTime);
      input.onActionUp += OnActionUp;
      input.onActionDown += OnActionDown;
    }

    public void OnActionDown(string action) {
      jumping = true;
    }

    public void OnActionUp(string action) {
      // when button is up, reset, and allow a new jump
      jumping = false;
      detachJumping = false;
    }

    /// <summary>
    /// TODO REVIEW jump changes when moved to action, investigate
    /// </summary>
    public override int WantsToUpdate(float delta) {
      return !detachJumping && jumping ? priority : 0;
    }

    public override void PerformAction(float delta) {
      if (controller.IsOnGround(_graceJumpFrames)) {
        jump.StartJump(ref character.velocity);
      } else {
        if (!jump.Jumping(ref character.velocity)) {
          detachJumping = true;
        }
      }
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
