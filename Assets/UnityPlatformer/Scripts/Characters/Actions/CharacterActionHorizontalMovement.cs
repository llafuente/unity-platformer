using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Movement while on ground and not slipping
  /// </summary>
  public abstract class CharacterActionHorizontalMovement: CharacterAction {
    [Header("Walk")]
    /// <summary>
    /// (Walking) Movement speed
    /// </summary>
    [Comment("Movement speed")]
    public float speed = 3;
    /// <summary>
    /// (Walking) Time to reach max speed
    /// </summary>
    [Comment("Time to reach max speed")]
    public float accelerationTime = .1f;

    [Header("Run")]
    /// <summary>
    /// (Running) Movement speed
    /// </summary>
    [Comment("Movement speed")]
    public float runSpeed = 6;
    /// <summary>
    /// (Running) Time to reach max speed
    /// </summary>
    [Comment("Time to reach max speed")]
    public float runAccelerationTime = .1f;
    /// <summary>
    /// Do not walk?
    /// </summary>
    public bool alwaysRun = true;
    /// <summary>
    /// Input action name for start running (not used if alwaysRun=true)
    /// </summary>
    public string runAction = "Run";
    /// <summary>
    /// (Walking) Mathf.SmoothDamp
    /// </summary>
    float velocityXSmoothing;
    /// <summary>
    /// (Running) Mathf.SmoothDamp
    /// </summary>
    float runVelocityXSmoothing;
    /// <summary>
    /// Currently running?
    /// </summary>
    bool running = false;

    public override void OnEnable() {
      base.OnEnable();

      input.onActionUp += OnActionUp;
      input.onActionDown += OnActionDown;
    }
    /// <summary>
    /// input.onActionDown
    /// </summary>
    public void OnActionDown(string _action) {
      if (_action == runAction) {
        running = true;
      }
    }
    /// <summary>
    /// input.onActionUp
    /// </summary>
    public void OnActionUp(string _action) {
      if (_action == runAction) {
        running = false;
      }
    }
    /// <summary>
    /// Reset SmoothDamp
    /// </summary>
    public void Clear() {
      velocityXSmoothing = 0;
      runVelocityXSmoothing = 0;
    }
    /// <summary>
    /// Do horizontal movement
    /// </summary>
    public override void PerformAction(float delta) {
      if (running || alwaysRun) {
        Move(runSpeed, ref runVelocityXSmoothing, runAccelerationTime);
        if (character.velocity.x != 0) {
          character.EnterStateGraceful(States.Running);
          character.ExitStateGraceful(States.Walking);
        }
      } else {
        Move(speed, ref velocityXSmoothing, accelerationTime);
        if (character.velocity.x != 0) {
          character.ExitStateGraceful(States.Running);
          character.EnterStateGraceful(States.Walking);
        }
      }
      if (character.velocity.x == 0) {
        character.ExitStateGraceful(States.Running);
        character.ExitStateGraceful(States.Walking);
      }
    }
    /// <summary>
    /// Horizontal movement based on current input
    /// </summary>
    public void Move(float spdy, ref float smoothing, float accTime) {
      float x = input.GetAxisRawX();
      if (x != 0) { // do not modify if no key is pressed
        character.SetFacing(x);
      }

      float targetVelocityX = x * spdy;

      character.velocity.x = Mathf.SmoothDamp (
        character.velocity.x,
        targetVelocityX,
        ref smoothing,
        accTime
      );
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
