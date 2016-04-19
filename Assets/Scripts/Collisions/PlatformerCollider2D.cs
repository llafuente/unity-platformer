using System;
using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Handles collisions
  /// </summary>
  public class PlatformerCollider2D : RaycastController {
    public float maxClimbAngle = 30;
    public float maxDescendAngle = 30;

    [HideInInspector]
    public CollisionInfo collisions;

    [HideInInspector]
    public bool disableWorldCollisions = false;

    public override void Start() {
      base.Start ();
      collisions.faceDir = 1;
    }

    public void Move(Vector3 velocity) {
      UpdateRaycastOrigins ();
      collisions.Reset ();
      collisions.prevVelocity = velocity;

      if (velocity.x != 0) {
        //Debug.LogFormat("MOVE?! {0}", velocity.x);
        collisions.faceDir = (int)Mathf.Sign(velocity.x);
      }

      // TODO PERF add: collisions.prevBelow, so wont be testing while falling
      if (velocity.y < 0) {
        DescendSlope(ref velocity);
      }

      if (!disableWorldCollisions) {
        HorizontalCollisions (ref velocity);
        if (velocity.y != 0) {
          VerticalCollisions (ref velocity);
        }
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
        RaycastHit2D hit = Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask, Color.red);

        if (hit && hit.distance != 0) {
          float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
          if (i == 0 && slopeAngle <= maxClimbAngle) {
            if (collisions.descendingSlope) {
              collisions.descendingSlope = false;
              velocity = collisions.prevVelocity;
            }
            float distanceToSlopeStart = 0;
            if (slopeAngle != collisions.prevSlopeAngle) {
              distanceToSlopeStart = hit.distance-skinWidth;
              velocity.x -= distanceToSlopeStart * directionX;
            }
            ClimbSlope(ref velocity, slopeAngle);
            velocity.x += distanceToSlopeStart * directionX;
          }

          if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
            // Min fix an edge case, when collider is pushing same direction a slope is moving
            // -> \
            // This introduce so many more problems... need REVIEW
            //velocity.x = Mathf.Min(velocity.x, (hit.distance - skinWidth) * directionX);
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

    public bool IsGroundOnLeft(float rayLengthFactor) {
      Vector3 v = new Vector3(0, 0, 0);
      float rayLength = skinWidth * rayLengthFactor;
      RaycastHit2D hit = DoVerticalRay (-1.0f, 0, rayLength, ref v);

      return hit.collider != null;
    }

    public bool IsGroundOnRight(float rayLengthFactor) {
      Vector3 v = new Vector3(0, 0, 0);
      float rayLength = skinWidth * rayLengthFactor;
      RaycastHit2D hit = DoVerticalRay (-1.0f, verticalRayCount - 1, rayLength, ref v);

      return hit.collider != null;
    }

    void VerticalCollisions(ref Vector3 velocity) {
      float directionY = Mathf.Sign (velocity.y);

      float rayLength = Mathf.Abs (velocity.y) + skinWidth;

      for (int i = 0; i < verticalRayCount; i ++) {

        RaycastHit2D hit = DoVerticalRay (directionY, i, rayLength, ref velocity);

        if (hit) {
          // fallingThroughPlatform ?
          if (
            hit.collider.tag == Configuration.instance.movingPlatformThroughTag &&
            collisions.standingOnPlatform &&
            collisions.fallingThroughPlatform
          ) {
            continue;
          }
          velocity.y = (hit.distance - skinWidth) * directionY;
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
        RaycastHit2D hit = Raycast(rayOrigin,Vector2.right * directionX,rayLength,collisionMask, Color.magenta);

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
      RaycastHit2D hit = Raycast (rayOrigin, -Vector2.up, 1000, collisionMask, Color.yellow);
      Utils.DrawPoint(rayOrigin);
      if (hit) {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
          bool movingDownSlope = Mathf.Sign(hit.normal.x) == directionX;
          bool slopeIsClose = hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
          if (movingDownSlope && slopeIsClose) {
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
    public void FallThroughPlatform(float resetDelay = 0.5f) {
      // defense!
      if (collisions.fallingThroughPlatform) {
        return;
      }

      collisions.fallingThroughPlatform = true;
      Invoke("ResetFallingThroughPlatform", resetDelay);
    }

    public void ResetFallingThroughPlatform() {
      collisions.fallingThroughPlatform = false;
    }

    public bool IsOnGround(int graceFrames = 0) {
      if (graceFrames == 0) {
        return collisions.below;
      }

      return collisions.below || collisions.lastBelowFrame < graceFrames;
    }

    public struct CollisionInfo {
      // current
      public bool above, below;
      public bool left, right;
      public float slopeAngle;

      // previous frame values
      public bool prevAbove, prevBelow;
      public bool prevLeft, prevRight;
      public float prevSlopeAngle;
      public Vector3 prevVelocity;

      // frame-counts
      public int lastAboveFrame;
      public int lastBelowFrame;
      public int lastLeftFrame;
      public int lastRightFrame;

      // other
      public bool climbingSlope;
      public bool descendingSlope;
      public int faceDir;
      public bool fallingThroughPlatform;
      public bool standingOnPlatform;

      // callbacks
      public Action onRightWall;
      public Action onLeftWall;
      public Action onLanding;
      public Action onLeaveGround;
      public Action onTop;

      public void Reset() {
        prevAbove = above;
        prevBelow = below;
        prevLeft = left;
        prevRight = right;

        above = below = false;
        left = right = false;
        climbingSlope = false;
        descendingSlope = false;

        ++lastAboveFrame;
        ++lastBelowFrame;
        ++lastLeftFrame;
        ++lastRightFrame;

        prevSlopeAngle = slopeAngle;
        slopeAngle = 0;
      }

      public void Consolidate() {
        if (right && !prevRight) {
          lastRightFrame = 0;
          if (onRightWall != null) {
            onRightWall ();
          }
        }
        if (left && !prevLeft) {
          lastLeftFrame = 0;
          if (onLeftWall != null) {
            onLeftWall ();
          }
        }
        if (above && !prevAbove) {
          lastAboveFrame = 0;
          if (onTop != null) {
            onTop ();
          }
        }
        if (!below && prevBelow) {
          lastBelowFrame = 0;
          if (onLeaveGround != null) {
            onLeaveGround ();
          }
        }
        if (below && !prevBelow && onLanding != null) {
          onLanding ();
        }
      }
    }
  }
}
