using System;
using UnityEngine;

namespace UnityPlatformer {
  [Serializable]
  public class JumpVariableHeightProperties {
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    [Comment("Time allowed to jump after leave ground")]
    public float graceJumpTime = 0.15f;
    [Comment("Time to reach maxJumpHeight")]
    public float timeToJumpApex = 0.4f;
    [Comment("The amount of time may spend hanging in midair at the apex of her jump (while the jump is not canceled).")]
    public float hangTime = 0.0f;
  };

  /// <summary>
  /// Math behind the Jump
  /// </summary>
  public class JumpVariableHeight : Jump {
    public float maxJumpVelocity;
    public float minJumpVelocity;
    public int hangFrames;
    public int apexFrames;

    // TODO FIXME maxJumpHeight is not used!!!
    public JumpVariableHeight(Character _character, float timeToJumpApex, float minJumpHeight, float maxJumpHeight, float hangTime) {
      character = _character;
      Init(timeToJumpApex, minJumpHeight, maxJumpHeight, hangTime);
    }

    public JumpVariableHeight(Character _character, JumpVariableHeightProperties jp) {
      character = _character;
      Init(jp.timeToJumpApex, jp.minJumpHeight, jp.maxJumpHeight, jp.hangTime);
    }

    public void Init(float timeToJumpApex, float minJumpHeight, float maxJumpHeight, float hangTime) {
      maxJumpVelocity = Mathf.Abs(character.gravity.y) * timeToJumpApex;
      minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (character.gravity.y) * minJumpHeight);
      apexFrames = UpdateManager.instance.GetFrameCount(timeToJumpApex);
      hangFrames = UpdateManager.instance.GetFrameCount(hangTime);
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

    public override bool Jumping(ref Vector3 velocity) {
      base.Jumping(ref velocity);
      // jumps frames

      if (IsBeforeApex()) {
        // jumping
        return true;
      }
      if (IsHanging()) {
        // hang time, opose Y gravity
        // TODO REVIEW disable gravity ?
        velocity.y = -character.gravity.y * Time.fixedDeltaTime;
        return true;
      }

      return false;
    }
  }
}
