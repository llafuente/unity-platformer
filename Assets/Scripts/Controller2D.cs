using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Handles collisions
/// </summary>
public class Controller2D : RaycastController {
	public const string PLAYER_TAG = "Player";
	public const string TROUGHT_TAG = "MovingPlatformThrough";
	public const string MOVINGPLATFORM_TAG = "MovingPlatform";
	public const string ENEMY_TAG = "Enemy";

	public const float MIN_DISTANCE_TO_ENV = 0.1f;

	// TODO this is buggy
	float maxClimbAngle = 30;
	float maxDescendAngle = 30;

	public CollisionInfo collisions;

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

	[HideInInspector]
	public Vector2 playerInput;

	[HideInInspector]
	public bool disableWorldCollisions = false;

	public override void Start() {
		base.Start ();
		collisions.faceDir = 1;

	}

	public void Move(Vector3 velocity, bool standingOnPlatform) {
		Move(velocity, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false) {
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.velocityOld = velocity;
		playerInput = input;

		if (velocity.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(velocity.x);
		}

		if (velocity.y < 0) {
			DescendSlope(ref velocity);
		}

		if (!disableWorldCollisions) {
			HorizontalCollisions (ref velocity);
			if (velocity.y != 0) {
				VerticalCollisions (ref velocity);
			}
		}

		if (standingOnPlatform) {
			//collisions.below = true;
		}

		transform.Translate (velocity);
		collisions.Consolidate ();
	}

	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		if (Mathf.Abs(velocity.x) < skinWidth) {
			rayLength = 2*skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

			if (hit) {

				if (hit.distance == 0) {
					continue;
				}

				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance-skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	public bool IsGroundOnLeft() {
		Vector3 v = new Vector3(0, 0, 0);
		float rayLength = skinWidth + MIN_DISTANCE_TO_ENV;
  	    RaycastHit2D hit = DoVerticalRay (-1.0f, 0, rayLength, ref v);

		return hit.collider != null;
	}

	public bool IsGroundOnRight() {
		Vector3 v = new Vector3(0, 0, 0);
		float rayLength = skinWidth + MIN_DISTANCE_TO_ENV;
  	    RaycastHit2D hit = DoVerticalRay (-1.0f, verticalRayCount - 1, rayLength, ref v);
		 
		return hit.collider != null;
	}

	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);

		float rayLength = Mathf.Abs (velocity.y) + skinWidth + MIN_DISTANCE_TO_ENV;

		for (int i = 0; i < verticalRayCount; i ++) {

			RaycastHit2D hit = DoVerticalRay (directionY, i, rayLength, ref velocity);

			if (hit) {
				if (hit.collider.tag == TROUGHT_TAG && !collisions.standingOnPlatform) {
					if (directionY == 1 || hit.distance == 0) {
						continue;
					}
					if (collisions.fallingThroughPlatform) {
						continue;
					}
					if (playerInput.y == -1) {
						collisions.fallingThroughPlatform = true;
						Invoke("ResetFallingThroughPlatform",.5f);
						continue;
					}
				}

				velocity.y = (hit.distance - skinWidth - MIN_DISTANCE_TO_ENV) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength,collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	void ResetFallingThroughPlatform() {
		collisions.fallingThroughPlatform = false;
	}

	public bool IsOnGround(int graceFrames = 0) {
		if (graceFrames == 0) {
			return collisions.below;
		}

		return collisions.below || collisions.lastBelowFrame < graceFrames;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public bool _above, _below;
		public bool _left, _right;

		public int lastAboveFrame;
		public int lastBelowFrame;
		public int lastLeftFrame;
		public int lastRightFrame;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;
		public int faceDir;
		public bool fallingThroughPlatform;
		public bool standingOnPlatform;

		public Action OnRightWall;
		public Action OnLeftWall;

		public void Reset() {
			_above = above;
			_below = below;
			_left = left;
			_right = right;

			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			++lastAboveFrame;
			++lastBelowFrame;
			++lastLeftFrame;
			++lastRightFrame;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}

		public void Consolidate() {
			if (!below && _below) {
				lastBelowFrame = 0;
			}
			if (right && !_right && OnRightWall != null) {
				OnRightWall ();
			}
			if (left && !_left && OnLeftWall != null) {
				OnLeftWall ();
			}
		}
	}

}
