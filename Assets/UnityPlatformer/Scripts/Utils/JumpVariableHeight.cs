using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Serializable class to configure JumpVariableHeight in the Editor
  /// </summary>
  [Serializable]
  public class JumpVariableHeightProperties {
    /// <summary>
    /// Max Jump height
    /// </summary>
    public float maxJumpHeight = 4;
    /// <summary>
    /// Min Jump height
    /// </summary>
    public float minJumpHeight = 1;
    /// <summary>
    /// Time allowed to jump after leave ground
    /// </summary>
    [Comment("Time allowed to jump after leave ground")]
    public float graceJumpTime = 0.15f;
    /// <summary>
    /// Time to reach maxJumpHeight
    /// </summary>
    [Comment("Time to reach maxJumpHeight")]
    public float timeToJumpApex = 0.4f;
    /// <summary>
    /// Amount of time may spend hanging in midair at the apex of her jump
    /// (while the jump is not canceled).
    /// </summary>
    [Comment("Amount of time may spend hanging in midair at the apex of her jump (while the jump is not canceled).")]
    public float hangTime = 0.0f;
  };

  /// <summary>
  /// Variable Jump. Use to create a Min/Max jump depending on how much time
  /// the key is pressed
  /// </summary>
  public class JumpVariableHeight : Jump {
    /// <summary>
    /// Max jump velocity
    /// </summary>
    public float maxJumpVelocity;
    /// <summary>
    /// Min jump velocity
    /// </summary>
    public float minJumpVelocity;
    /// <summary>
    /// Frame count hanging
    /// </summary>
    public int hangFrames;
    /// <summary>
    /// Frame count until apex
    /// </summary>
    public int apexFrames;

    // TODO FIXME maxJumpHeight is not used!!!
    /// <summary>
    /// Constructor
    /// </summary>
    public JumpVariableHeight(Character _character, float timeToJumpApex, float minJumpHeight, float maxJumpHeight, float hangTime) {
      character = _character;
      Init(timeToJumpApex, minJumpHeight, maxJumpHeight, hangTime);
    }
    /// <summary>
    /// Constructor
    /// </summary>
    public JumpVariableHeight(Character _character, JumpVariableHeightProperties jp) {
      character = _character;
      Init(jp.timeToJumpApex, jp.minJumpHeight, jp.maxJumpHeight, jp.hangTime);
    }
    /// <summary>
    /// Constructor
    /// </summary>
    public void Init(float timeToJumpApex, float minJumpHeight, float maxJumpHeight, float hangTime) {
      maxJumpVelocity = Mathf.Abs(character.pc2d.gravity.y) * timeToJumpApex;
      minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (character.pc2d.gravity.y) * minJumpHeight);
      apexFrames = UpdateManager.GetFrameCount(timeToJumpApex);
      hangFrames = UpdateManager.GetFrameCount(hangTime);
    }


    public override void StartJump(ref Vector3 velocity) {
      Reset();
      velocity.y = maxJumpVelocity;
    }

    public override void EndJump(ref Vector3 velocity) {
      if (velocity.y > minJumpVelocity) {
        velocity.y = minJumpVelocity;
      }
    }

    public override bool IsBeforeApex() {
      return apexFrames >= ticks;
    }

    public override bool IsHanging() {
      return apexFrames < ticks && apexFrames + hangFrames >= ticks;
    }

    public override bool Jumping(ref Vector3 velocity, float delta) {
      base.Jumping(ref velocity, delta);
      // jumps frames

      if (IsBeforeApex()) {
        // jumping
        return true;
      }
      if (IsHanging()) {
        // hang time, opose Y gravity
        // TODO REVIEW disable gravity ?
        velocity.y = -character.pc2d.gravity.y * Time.fixedDeltaTime;
        return true;
      }

      return false;
    }
  }
}
