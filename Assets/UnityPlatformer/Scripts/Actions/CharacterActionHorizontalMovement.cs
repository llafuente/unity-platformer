using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Movement while on ground and not slipping
  /// </summary>
  public abstract class CharacterActionHorizontalMovement: CharacterAction {
    #region public

    [Header("Walk")]
    [Comment("Movement speed")]
    public float speed = 3;
    [Comment("Time to reach max speed")]
    public float accelerationTime = .1f;

    [Header("Run")]
    [Comment("Movement speed")]
    public float runSpeed = 6;
    [Comment("Time to reach max speed")]
    public float runAccelerationTime = .1f;

    public bool alwaysRun = true;
    public string runAction = "Run";

    #endregion

    float velocityXSmoothing;
    float runVelocityXSmoothing;
    bool running = false;

    public override void OnEnable() {
      base.OnEnable();

      input.onActionUp += OnActionUp;
      input.onActionDown += OnActionDown;
    }

    public void OnActionDown(string _action) {
      if (_action == runAction) {
        running = true;
      }
    }

    public void OnActionUp(string _action) {
      if (_action == runAction) {
        running = false;
      }
    }

    /// <summary>
    /// Reset SmoothDamp
    /// </summary>
    public void Reset() {
      velocityXSmoothing = 0;
      runVelocityXSmoothing = 0;
    }


    /// <summary>
    /// Do horizontal movement
    /// </summary>
    public override void PerformAction(float delta) {
      if (running || alwaysRun) {
        Move(runSpeed, ref runVelocityXSmoothing, runAccelerationTime);
      } else {
        Move(speed, ref velocityXSmoothing, accelerationTime);
      }
    }

    /// <summary>
    /// Horizontal movement based on current input
    /// </summary>
    public void Move(float spdy, ref float smoothing, float accTime) {
      float targetVelocityX = input.GetAxisRawX() * spdy;

      character.velocity.x = Mathf.SmoothDamp (
        character.velocity.x,
        targetVelocityX,
        ref velocityXSmoothing,
        accTime
      );
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
