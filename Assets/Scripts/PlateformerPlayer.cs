using UnityEngine;
using System.Collections;

/// <summary>
/// TODO double jump
/// TODO Dash
/// TODO Attack range
/// TODO Attack melee
/// TODO states and area are must be in Controller2D
/// TODO create a platformer input, to handle all input, and store information here, LateUpdate is messing events
/// </summary>
[RequireComponent (typeof (Controller2D))]
public class PlateformerPlayer : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float graceJumpTime = 0.25f;
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

	int _graceJumpFrames;

	/// <summary>
	/// This method precalculate some vars, but those value could change. This need to be refactored.
	/// Maybe setters are the appropiate method to refactor this.
	/// </summary>
	void Start() {
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
		print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
	}

	public void Attach(UpdateManager um) {
		_graceJumpFrames = um.GetFrameCount (graceJumpTime);
	}

	/// <summary>
	/// Managed update called by UpdateManager
	/// Transform Input into platformer magic :)
	/// </summary>
	public void ManagedUpdate(float delta) {
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		int wallDirX = (controller.collisions.left) ? -1 : 1;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

		if (IsOnLadder () && IsOnState(Controller2D.States.Ladder)) {
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
					timeToWallUnstick -= delta;
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
			Debug.Log ("Jump!!");

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
			//if (controller.collisions.below) {
			if (controller.IsOnGround(_graceJumpFrames)) {

				velocity.y = maxJumpVelocity;
			}
		} else if (Input.GetKeyUp (KeyCode.Space)) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}

		if (IsOnLadder () && input.y != 0 && !IsOnState (Controller2D.States.Ladder)) {
			controller.state |= Controller2D.States.Ladder;
			disableGravity = true;
			controller.disableWorldCollisions = true;
			// instant move to the center of the ladder!
			velocity.x = (ladderCenter - controller.GetComponent<BoxCollider2D>().bounds.center.x) / delta;
		}

		if (!disableGravity) {
			velocity.y += gravity * delta;
		}

		controller.Move(velocity * delta, input);

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
	}

	public bool IsOnState(Controller2D.States _state) {
		return (controller.state & _state) == _state;
	}

	public bool IsOnLadder() {
		return (controller.area & Controller2D.Areas.Ladder) == Controller2D.Areas.Ladder;
	}

	public void EnterLadderArea(Bounds b) {
		controller.area |= Controller2D.Areas.Ladder;
		ladderCenter = b.center.x;
	}

	public void ExitLadderArea(Bounds b) {
		controller.area &= ~Controller2D.Areas.Ladder;
		if (IsOnState (Controller2D.States.Ladder)) {
			controller.state &= ~Controller2D.States.Ladder;
			disableGravity = false;
			controller.disableWorldCollisions = false;
		}
	}
}
