/**!
The MIT License (MIT)

Copyright (c) 2015 Sebastian
Original file: https://github.com/SebLague/2DPlatformer-Tutorial/blob/master/Episode%2011/Controller2D.cs

Modifications (c) 2016 Luis Lafuente

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
**/

﻿using System;
using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Handle collisions
  /// </summary>
  public class PlatformerCollider2D : RaycastController {
    public float maxClimbAngle = 30;
    public float maxDescendAngle = 30;

    [HideInInspector]
    /// <summary>
    /// Ignore the decend angle, so always decend.
    /// <summary>
    public bool ignoreDescendAngle = false;

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
        // collisions.faceDir = (int)Mathf.Sign(velocity.x);
        if (velocity.x > 0.0f) {
          collisions.faceDir = 1;
        } else if (velocity.x < 0.0f) {
          collisions.faceDir = -1;
        } /* else, leave the last one :) */
      }

      UpdateCurrentSlope(ref velocity);

      // TODO PERF add: collisions.prevBelow, so wont be testing while falling
      if (collisions.slopeAngle != 0) {
        DescendSlope(ref velocity);

        ClimbSlope(ref velocity);
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

    void UpdateCurrentSlope(ref Vector3 velocity) {
      float rayLength = Mathf.Abs (velocity.y) + skinWidth; // TODO configurable

      // get slopeAngle, should be inside 0-90
      // consider only the maximum
      float slopeAngle = 0.0f;
      Vector3? slopeNormal = null;
      int sloperDir = 0;
      RaycastHit2D? fhit = null;

      for (int i = 0; i < verticalRayCount; ++i) {
        RaycastHit2D hit = DoVerticalRay (-1, i, rayLength, ref velocity, Color.yellow);

        if (hit) {
          float a = Vector2.Angle(hit.normal, Vector2.up);
          if (a > slopeAngle) {
            fhit = hit;
            slopeAngle = a;
            slopeNormal = hit.normal;
            sloperDir = (int)Mathf.Sign(-hit.normal.x);
          }
        }
      }

      rayLength = Mathf.Abs (velocity.x) + skinWidth; // TODO configurable

      float directionX = collisions.faceDir;
      for (int i = 0; i < 1; i ++) {
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
        rayOrigin += Vector2.up * (horizontalRaySpacing * i);
        RaycastHit2D hit = Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask, Color.yellow);

        if (hit) {
          float a = Vector2.Angle(hit.normal, Vector2.up);
          if (a > slopeAngle) {
            fhit = hit;
            slopeAngle = a;
            sloperDir = (int)Mathf.Sign(-hit.normal.x);
          }
        }
      }

      collisions.slopeAngle = slopeAngle;
      if (slopeNormal != null) {
        collisions.slopeNormal = slopeNormal.Value;
      }
      if (velocity.x != 0.0f) {
        collisions.climbingSlope = sloperDir == Mathf.Sign(velocity.x);
        collisions.descendingSlope = sloperDir != Mathf.Sign(velocity.x);
      }

      // handle the moment we change the slope
      // TODO REVIEW this may lead to problems when a platforms rotates.
      if (fhit != null && collisions.slopeAngle > collisions.prevSlopeAngle) {
        collisions.distanceToSlopeStart = fhit.Value.distance - skinWidth;
      }
    }

    void HorizontalCollisions(ref Vector3 velocity) {
      float directionX = collisions.faceDir;
      float rayLength = Mathf.Abs (velocity.x) + skinWidth;

      if (Mathf.Abs(velocity.x) < skinWidth) {
        rayLength = 2*skinWidth;
      }

      for (int i = 0; i < horizontalRayCount; i ++) {
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
        rayOrigin += Vector2.up * (horizontalRaySpacing * i);
        RaycastHit2D hit = Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask, Color.red);

        if (hit && hit.distance != 0) {
          // ignore oneWayPlatforms, aren't walls
          if (Configuration.IsOneWayPlatform(hit.collider)) {
            continue;
          }

          float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
          if (slopeAngle > maxClimbAngle) {
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
            Configuration.IsMovingPlatformThrough(hit.collider) &&
            collisions.standingOnPlatform &&
            collisions.fallingThroughPlatform
          ) {
            continue;
          }

          // ignore oneWayPlatforms when not falling
          if (
            Configuration.IsOneWayPlatform(hit.collider) &&
            velocity.y > 0
          ) {
            continue;
          }
          velocity.y = (hit.distance - skinWidth) * directionY;
          rayLength = hit.distance;

          collisions.below = directionY == -1;
          collisions.above = directionY == 1;
        }
      }
    }

    void ClimbSlope(ref Vector3 velocity) {
      if (collisions.climbingSlope) {
        if (collisions.slopeAngle > maxClimbAngle) {
          velocity.x = 0;
          return;
        }

        velocity.x -= collisions.distanceToSlopeStart * collisions.faceDir;

        float moveDistance = Mathf.Abs (velocity.x);
        float climbVelocityY = Mathf.Sin (collisions.slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY) {
          velocity.y = climbVelocityY;
          velocity.x = Mathf.Cos (collisions.slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
          collisions.below = true;
        }

        velocity.x += collisions.distanceToSlopeStart * collisions.faceDir;
      }
    }

    void DescendSlope(ref Vector3 velocity) {
      if (collisions.descendingSlope &&
        (collisions.slopeAngle <= maxDescendAngle || ignoreDescendAngle)
      ) {
        float moveDistance = Mathf.Abs(velocity.x);
        float descendVelocityY = Mathf.Sin (collisions.slopeAngle * Mathf.Deg2Rad) * moveDistance;
        velocity.x = Mathf.Cos (collisions.slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
        velocity.y -= descendVelocityY;
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

    public Vector3 GetDownSlopeDir() {
      if (collisions.slopeAngle == 0){
        return Vector3.zero;
      }

      return new Vector3(
        Mathf.Sign(collisions.slopeNormal.x) * collisions.slopeNormal.y,
        -Math.Abs(collisions.slopeNormal.x),
        0
      );
    }

    public struct CollisionInfo {
      // current
      public bool above, below;
      public bool left, right;
      public float slopeAngle;
      public Vector3 slopeNormal;

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
      public float distanceToSlopeStart;
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
        distanceToSlopeStart = 0;
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
