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
    // public float minMovement = 0.0001f;
    public float maxClimbAngle = 30;
    public float maxDescendAngle = 30;
    public bool enableSlopes = true;

    // callbacks
    public Action onRightWall;
    public Action onLeftWall;
    public Action onLanding;
    public Action onLeaveGround;
    public Action onTop;

    /// <summary>
    /// Ignore the decend angle, so always decend.
    /// <summary>
    [HideInInspector]
    public bool ignoreDescendAngle = false;


    [HideInInspector]
    public CollisionInfo collisions;
    [HideInInspector]
    public CollisionInfo pCollisions;

    [HideInInspector]
    public bool disableWorldCollisions = false;

    public override void Start() {
      base.Start ();
      collisions.faceDir = 1;
    }

    public void Move(Vector3 velocity) {
      UpdateRaycastOrigins ();
      pCollisions = collisions;
      collisions.Reset ();

      //Debug.Log("velocity: " + velocity.ToString("F4"));

      if (velocity.x != 0) {
        // collisions.faceDir = (int)Mathf.Sign(velocity.x);
        if (velocity.x > 0.0f) {
          collisions.faceDir = 1;
        } else if (velocity.x < 0.0f) {
          collisions.faceDir = -1;
        } /* else, leave the last one :) */
      }

      if (enableSlopes) {
        UpdateCurrentSlope(ref velocity);

        // TODO PERF add: pCcollisions.Below, so wont be testing while falling
        // if (collisions.slopeAngle != 0 && pCollisions.below) {
        if (collisions.slopeAngle != 0) {
          ClimbSlope(ref velocity);
          DescendSlope(ref velocity);
        }
      }

      if (!disableWorldCollisions) {
        HorizontalCollisions (ref velocity);
        if (velocity.y != 0) {
          VerticalCollisions (ref velocity);
        }
      }
/*
      if (velocity.x < minMovement) {
        velocity.x = 0;
      }

      if (velocity.y < minMovement) {
        velocity.y = 0;
      }
*/
      transform.Translate (velocity);
      collisions.velocity = velocity;
      ConsolidateCollisions ();
    }

    void UpdateCurrentSlope(ref Vector3 velocity) {
      float rayLength = Mathf.Abs (velocity.y) + skinWidth; // TODO configurable

      // get slopeAngle, should be inside 0-90
      // consider only the maximum
      float slopeAngle = 0.0f;
      Vector3? slopeNormal = null;
      RaycastHit2D? fhit = null;

      for (int i = 0; i < verticalRayCount; ++i) {
        RaycastHit2D hit = DoVerticalRay (-1, i, rayLength, ref velocity, Color.yellow);

        if (hit) {
          float a = Vector2.Angle(hit.normal, Vector2.up);
          if (a > slopeAngle) {
            fhit = hit;
            slopeAngle = a;
            slopeNormal = hit.normal;
          }
        }
      }

      rayLength = Mathf.Abs (velocity.x) + skinWidth;
      RaycastHit2D rhit = Raycast(raycastOrigins.bottomRight, Vector2.right, rayLength, collisionMask, Color.yellow);

      if (rhit) {
        float a = Vector2.Angle(rhit.normal, Vector2.up);
        if (a > slopeAngle) {
          fhit = rhit;
          slopeAngle = a;
        }
      }

      RaycastHit2D lhit = Raycast(raycastOrigins.bottomLeft, Vector2.left, rayLength, collisionMask, Color.yellow);

      if (lhit) {
        float a = Vector2.Angle(lhit.normal, Vector2.up);
        if (a > slopeAngle) {
          fhit = lhit;
          slopeAngle = a;
        }
      }

      if (fhit != null && !Mathf.Approximately(slopeAngle, 90) && !Mathf.Approximately(slopeAngle, 0)) {
        collisions.slopeAngle = slopeAngle;
        if (slopeNormal != null) {
          collisions.slopeNormal = slopeNormal.Value;
        }
        if (velocity.x != 0.0f) {
          int sloperDir = (int) Mathf.Sign(-fhit.Value.normal.x);
          collisions.climbingSlope = sloperDir == Mathf.Sign(velocity.x);
          collisions.descendingSlope = sloperDir != Mathf.Sign(velocity.x);
        }

        // handle the moment we change the slope
        // TODO REVIEW this may lead to problems when a platforms rotates.
        if (fhit != null && collisions.slopeAngle > pCollisions.slopeAngle) {
          collisions.distanceToSlopeStart = fhit.Value.distance - skinWidth;
        }
      } else {
        collisions.slopeAngle = 0.0f;
      }
    }

    void HorizontalCollisions(ref Vector3 velocity) {
      float directionX = collisions.faceDir;
      float rayLength = Mathf.Abs (velocity.x) + skinWidth;

      if (Mathf.Abs(velocity.x) < skinWidth) {
        rayLength = 2 * skinWidth;
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
            velocity.x = (hit.distance - skinWidth) * directionX;
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

      // this ray needs to be a bit longer...
      // this may need to be a parameter...
      float rayLength = Mathf.Abs (velocity.y) + skinWidth * 2;

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

          velocity.y = (hit.distance - minDistanceToEnv) * directionY;
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
        Vector3 slopedir = GetDownSlopeDir();
        velocity.y = Mathf.Abs(velocity.x) * slopedir.y;
        collisions.below = true;
      }
    }

    public void DisableSlopes(float resetDelay = 0.5f) {
      enableSlopes = false;
      Invoke("EnableSlopes", resetDelay);
    }

    public void EnableSlopes() {
      enableSlopes = true;
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

    public void ConsolidateCollisions() {
      if (collisions.right) {
        collisions.lastRightFrame = 0;
      }
      if (collisions.left) {
        collisions.lastLeftFrame = 0;
      }
      if (collisions.above) {
        collisions.lastAboveFrame = 0;
      }
      if (collisions.below) {
        collisions.lastBelowFrame = 0;
      }

      if (collisions.right && !pCollisions.right) {
        if (onRightWall != null) {
          onRightWall ();
        }
      }

      if (collisions.left && !pCollisions.left) {
        if (onLeftWall != null) {
          onLeftWall ();
        }
      }

      if (collisions.above && !pCollisions.above) {
        if (onTop != null) {
          onTop ();
        }
      }

      if (!collisions.below && pCollisions.below) {
        if (onLeaveGround != null) {
          onLeaveGround ();
        }
      }

      if (collisions.below && !pCollisions.below && onLanding != null) {
        onLanding ();
      }
    }

    public struct CollisionInfo {
      // current
      public bool above, below;
      public bool left, right;
      public float slopeAngle;
      public Vector3 slopeNormal;
      public Vector3 velocity;

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

      public void Reset() {
        above = below = false;
        left = right = false;
        climbingSlope = false;
        descendingSlope = false;

        ++lastAboveFrame;
        ++lastBelowFrame;
        ++lastLeftFrame;
        ++lastRightFrame;

        slopeAngle = 0;
        slopeNormal = Vector3.zero;
        distanceToSlopeStart = 0;
      }
    }
  }
}
