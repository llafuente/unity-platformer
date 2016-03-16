using System;
using UnityEngine;


public class Jump {
  float maxJumpVelocity;
  float minJumpVelocity;

	public Jump(float gravity, float timeToJumpApex, float minJumpHeight) {
    maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

  public void StartJump(ref Vector3 velocity) {
    velocity.y = maxJumpVelocity;
  }

  public void Jumping(ref Vector3 velocity) {
    if (velocity.y > minJumpVelocity) {
      velocity.y = minJumpVelocity;
    }
  }
}
