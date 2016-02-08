using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class PlateformerPlayer : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;
	float ladderMoveSpeed = 4;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;
	bool disableGravity = false;
	float ladderCenter;

	Controller2D controller;

	public enum States
	{
		None = 0,             // 0000000
		OnGround = 1,         // 0000001
		OnMovingPlatform = 3, // 0000011
		OnSlope = 5,          // 0000100
		Jumping = 8,          // 0001000
		Falling = 16,         // 0010000
		FallingFast = 48,     // 0110000
		Ladder = 64,          // 1000000
		//WallSliding,
		//WallSticking,
		//Dashing,
		//Frozen,
		//Slipping,
		//FreedomState
	}
	public States state = States.None;

	public enum Areas
	{
		None = 0x0,
		Ladder = 0x01
	}
	public Areas area = Areas.None;

	void Start() {
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
		print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
	}

	public void ManagedUpdate() {
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		int wallDirX = (controller.collisions.left) ? -1 : 1;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

		if (IsOnLadder () && IsOnState(States.Ladder)) {
			velocity.x = 0; // disable x movement
			velocity.y = ladderMoveSpeed * input.y;
		}

		bool wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (input.x != wallDirX && input.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
			}
			else {
				timeToWallUnstick = wallStickTime;
			}
		}

		// jump
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (wallSliding) {
				if (wallDirX == input.x) {
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				}
				else if (input.x == 0) {
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;
				}
				else {
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
				}
			}
			if (controller.collisions.below) {
				velocity.y = maxJumpVelocity;
			}
		} else if (Input.GetKeyUp (KeyCode.Space)) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}

		if (IsOnLadder () && input.y != 0 && !IsOnState (States.Ladder)) {
			state |= States.Ladder;
			disableGravity = true;
			controller.disableWorldCollisions = true;
			// instant move to the center of the ladder!
			velocity.x = (ladderCenter - controller.GetComponent<BoxCollider2D>().bounds.center.x) / Time.deltaTime;
		}

		if (!disableGravity) {
			velocity.y += gravity * Time.deltaTime;
		}

		controller.Move(velocity * Time.deltaTime, input);

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
	}

	public bool IsOnState(States _state) {
		return (state & _state) == _state;
	}

	public bool IsOnLadder() {
		return (area & Areas.Ladder) == Areas.Ladder;
	}

	public void EnterLadderArea(Bounds b) {
		area |= Areas.Ladder;
		ladderCenter = b.center.x;
	}

	public void ExitLadderArea(Bounds b) {
		area &= ~Areas.Ladder;
		if (IsOnState (States.Ladder)) {
			state &= ~States.Ladder;
			disableGravity = false;
			controller.disableWorldCollisions = false;
		}
	}
}
