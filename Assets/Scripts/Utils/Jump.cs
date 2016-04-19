using System;
using UnityEngine;
using UnityPlatformer.Characters;

namespace UnityPlatformer {
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

    public Jump(Character _character, float timeToJumpApex, float minJumpHeight, float hangTime) {
      character = _character;

      maxJumpVelocity = Mathf.Abs(character.gravity.y) * timeToJumpApex;
      minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (character.gravity.y) * minJumpHeight);
      apexFrames = UpdateManager.instance.GetFrameCount(timeToJumpApex);
      hangFrames = UpdateManager.instance.GetFrameCount(hangTime);
    }

    public void StartJump(ref Vector3 velocity) {
      velocity.y = maxJumpVelocity;
      ticks = 0;
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
