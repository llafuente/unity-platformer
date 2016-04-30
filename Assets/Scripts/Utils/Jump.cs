using System;
using UnityEngine;

namespace UnityPlatformer {
  [Serializable]
  public class JumpProperties {
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
  public class Jump {
    public float maxJumpVelocity;
    public float minJumpVelocity;
    public int hangFrames;
    public int apexFrames;
    public int ticks;

    Character character;
    // TODO FIXME maxJumpHeight is not used!!!
    public Jump(Character _character, float timeToJumpApex, float minJumpHeight, float maxJumpHeight, float hangTime) {
      Init(_character, timeToJumpApex, minJumpHeight, maxJumpHeight, hangTime);
    }

    public Jump(Character _character, JumpProperties jp) {
      Init(_character, jp.timeToJumpApex, jp.minJumpHeight, jp.maxJumpHeight, jp.hangTime);
    }

    public void Init(Character _character, float timeToJumpApex, float minJumpHeight, float maxJumpHeight, float hangTime) {
      character = _character;

      maxJumpVelocity = Mathf.Abs(character.gravity.y) * timeToJumpApex;
      minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (character.gravity.y) * minJumpHeight);
      apexFrames = UpdateManager.instance.GetFrameCount(timeToJumpApex);
      hangFrames = UpdateManager.instance.GetFrameCount(hangTime);
    }

    public void Reset() {
      ticks = 0;
    }

    public void StartJump(ref Vector3 velocity) {
      Reset();
      velocity.y = maxJumpVelocity;
    }

    public void EndJump(ref Vector3 velocity) {
      if (velocity.y > minJumpVelocity) {
        velocity.y = minJumpVelocity;
      }
    }

    public bool IsBeforeApex() {
      return apexFrames >= ticks;
    }

    public bool IsHanging() {
      return apexFrames < ticks && apexFrames + hangFrames >= ticks;
    }

    public bool Jumping(ref Vector3 velocity) {
      ++ticks;
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
