using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Perform an action over a character
  /// </summary>
  [RequireComponent (typeof (PlatformerController))]
  [RequireComponent (typeof (Controller2D))]
  [RequireComponent (typeof (Character))]
  public class CharacterActionJump: MonoBehaviour, CharacterAction, UpdateManagerAttach {
    // TODO OnValidate check this!
    [Comment("Must match something in @PlatformerController")]
    public String action;

    public float maxJumpHeight = 4;
		public float minJumpHeight = 1;
		public float graceJumpTime = 0.25f;
		public float timeToJumpApex = .4f;

    float gravity;

    Jump jump;

    PlatformerController input;
    Controller2D controller;
    int _graceJumpFrames = 10;

    public void Start() {
      input = GetComponent<PlatformerController>();
      controller = GetComponent<Controller2D> ();

      gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
			jump = new Jump(gravity, timeToJumpApex, minJumpHeight);
      Debug.LogFormat("(CharacterActionJump) gravity {0} timeToJumpApex {1} minJumpHeight", gravity, timeToJumpApex, minJumpHeight);
    }

    public void Attach(UpdateManager um) {
      _graceJumpFrames = um.GetFrameCount (graceJumpTime);
      Debug.Log("(CharacterActionJump) Attached" + _graceJumpFrames);
    }

    /// <summary>
    /// Tells the charcater we want to take control
    /// Positive numbers fight: Higher number wins
    /// TODO REVIEW Negative numbers are used to ignore fight, and execute.
    /// </summary>
    public int WantsToUpdate() {
      return input.IsActionButtonDown(action) ? 5 : 0;
    }

    public void PerformAction(float delta) {
      Vector3 velocity = new Vector3 (0, 0, 0);
      //if (controller.collisions.below) {
      if (controller.IsOnGround(_graceJumpFrames)) {
        Debug.Log("StartJump");
        jump.StartJump(ref velocity);
      } else {
        Debug.Log("Jumping");
				jump.Jumping(ref velocity);
			}
Debug.Log(velocity);
      controller.Move(velocity * delta, Vector2.zero);
    }

    public PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.DO_BASE_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
