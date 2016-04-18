using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Math behind the Jump
  /// </summary>
  public class Jump {
    float maxJumpVelocity;
    float minJumpVelocity;
    int hangFrames;
    int ticks;

    public Jump(float gravity, float timeToJumpApex, float minJumpHeight, float hangTime = 0) {
      maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
      minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
      apexFrames = UpdateManager.instance.GetFrameCount(timeToJumpApex);
      hangFrames = UpdateManager.instance.GetFrameCount(hangTime);
    }

    public void StartJump(ref Vector3 velocity) {
      velocity.y = maxJumpVelocity;
      ticks = 0;
    }

    public bool Jumping(ref Vector3 velocity) {
      ++ticks;
      // jumps frames

      if (apexFrames > ticks) {
        // jumping
        if (velocity.y > minJumpVelocity) {
          velocity.y = minJumpVelocity;
        }
        return true;
      }
      if (apexFrames <= ticks) {
        // hang time
        velocity.y = 0; // TODO this may vary a bit up/down
        return true;
      }

      return false;
    }
  }
}
