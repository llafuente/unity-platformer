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
using System.Collections.Generic;

namespace UnityPlatformer {
  /// <summary>
  /// Handle collisions
  /// </summary>
  public class PlatformerCollider2D : RaycastController {
    #region public

    [Comment("Override Configuration.gravity, (0,0) means use global.")]
    public Vector2 gravityOverride = Vector2.zero;

    public Vector2 gravity {
      get {
        return gravityOverride == Vector2.zero ? Configuration.instance.gravity : gravityOverride;
      }
    }

    public float maxClimbAngle = 45.0f;
    public float maxDescendAngle = 45.0f;
    public bool enableSlopes = true;
    ///<summary>
    /// NOTE should be greater than skinWidth
    ///</summary>
    public float verticalRayLength = 0.2f;
    ///<summary>
    /// NOTE should be greater than skinWidth
    ///</summary>
    public float horizontalRayLength = 0.2f;
    ///<summary>
    /// This prevent unwanted micro changes in orientation/falling for example...
    ///</summary>
    public float minTranslation = 0.01f;

    /// <summary>
    /// TODO REMOVE
    /// This is experimental staff, If we use RigidBody2D we are sure to never
    /// enter another object by accident and we handle less collisions
    /// but has many drawbacks, that can't be solved atm, like OneWayPlatforms
    /// </summary>
    bool useRigidbody2D = false;

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

    #endregion

    int previousLayer;
    Rigidbody2D rigidBody2D;

    public override void Start() {
      base.Start ();
      collisions = new CollisionInfo();
      collisions.faceDir = 1;

      if (useRigidbody2D) {
        rigidBody2D = GetComponent<Rigidbody2D>();
      }
    }

    /// <summary>
    /// Attempt to move the character to position + velocity.
    /// Any colliders in the way will cause velocity to be modified
    /// NOTE collisions.velocity has the real velocity applied
    /// </summary>
    public void Move(Vector3 velocity, float delta) {
      // swap layers, this makes possible to collide with something inside my own layer
      // like boxes
      previousLayer = gameObject.layer;
      gameObject.layer = 2; // Ignore Raycast

      UpdateRaycastOrigins ();
      // set previous collisions and reset current one
      pCollisions = collisions.Clone();
      collisions.Reset ();

      // facing need to be reviewed. We should not rely on velocity.x
      if (velocity.x > 0.0f) {
        collisions.faceDir = 1;
      } else if (velocity.x < 0.0f) {
        collisions.faceDir = -1;
      } // else, leave the last one :)

      // Climb or descend a slope if in range
      if (enableSlopes) {
        UpdateCurrentSlope(ref velocity);

        // TODO PERF add: pCcollisions.below, so wont be testing while falling
        // if (collisions.slopeAngle != 0 && pCollisions.below) {
        if (collisions.slopeAngle != 0) {
          ClimbSlope(ref velocity);
          DescendSlope(ref velocity);
        }
      }

      // be sure we stay outside others colliders
      if (!disableWorldCollisions) {
        //DiagonalCollisions(ref velocity);

        HorizontalCollisions (-1, DoLeftRays(skinWidth, ref velocity), ref velocity);
        HorizontalCollisions (1, DoRightRays(skinWidth, ref velocity), ref velocity);

        if (velocity.y > 0) {
          VerticalCollisions (1, DoHeadRays(verticalRayLength, ref velocity), ref velocity);
        } else if (velocity.y < 0) {
          VerticalCollisions (-1, DoFeetRays(verticalRayLength, ref velocity), ref velocity);
        }
      }

      if (Math.Abs(velocity.x) < minTranslation) {
        velocity.x = 0;
      }
      if (Math.Abs(velocity.y) < minTranslation) {
        velocity.y = 0;
      }

      if (useRigidbody2D) {
        rigidBody2D.velocity = velocity / delta;
      } else {
        transform.Translate (velocity);
      }
      collisions.velocity = velocity;
      ConsolidateCollisions ();

      gameObject.layer = previousLayer;
    }

    /// <summary>
    /// Launch rays below, left, right and get the maximum slope found
    /// </summary>
    void UpdateCurrentSlope(ref Vector3 velocity) {
      float rayLength = Mathf.Abs (velocity.y) + verticalRayLength;
      float slopeAngle = 0.0f;
      RaycastHit2D? fhit = null;

      for (int i = 0; i < verticalRayCount; ++i) {
        RaycastHit2D hit = DoVerticalRay (-1, i, rayLength, ref velocity, Color.yellow);

        if (hit) {
          float a = Vector2.Angle(hit.normal, Vector2.up);
          if (a > slopeAngle) {
            fhit = hit;
            slopeAngle = a;
          }
        }
      }

      rayLength = Mathf.Abs (velocity.x) + horizontalRayLength;
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
        collisions.slopeNormal = fhit.Value.normal;

        if (velocity.x != 0.0f) {
          int sloperDir = (int) Mathf.Sign(-fhit.Value.normal.x);
          collisions.climbingSlope = sloperDir == Mathf.Sign(velocity.x);
          collisions.descendingSlope = sloperDir != Mathf.Sign(velocity.x);
        }

        // handle the moment we change the slope
        // TODO REVIEW this may lead to problems when a platforms rotates.
        if (fhit != null && collisions.slopeAngle > pCollisions.slopeAngle && collisions.slope != fhit.Value.collider.gameObject) {
          collisions.distanceToSlopeStart = fhit.Value.distance - skinWidth;
        }
        collisions.slope = fhit.Value.collider.gameObject;
      } else {
        collisions.slope = null;
        collisions.slopeAngle = 0.0f;
      }
    }

    void HorizontalCollisions(int dir, RaycastHit2D[] rays, ref Vector3 velocity) {
      for (int i = 0; i < horizontalRayCount; i ++) {
        if (rays[i] && rays[i].distance != 0) {
          // ignore oneWayPlatformsUp/Down, aren't walls
          if (Configuration.IsOneWayPlatformUp(rays[i].collider) || Configuration.IsOneWayPlatformDown(rays[i].collider)) {
            continue;
          }

          if ((
            // ignore left wall while moving left
            Configuration.IsOneWayWallLeft(rays[i].collider) &&
            velocity.x < 0
            ) || (
            // ignore right wall while moving right
            Configuration.IsOneWayWallRight(rays[i].collider) &&
            velocity.x > 0
          )) {
            continue;
          }


          if (dir == -1) {
            collisions.PushLeftCollider(rays[i]);
          } else {
            collisions.PushRightCollider(rays[i]);
          }

          float slopeAngle = Vector2.Angle(rays[i].normal, Vector2.up);
          if (slopeAngle > maxClimbAngle) {
            if (dir == -1) {
              collisions.left = true;
              if (Mathf.Approximately(slopeAngle, 90)) {
                collisions.leftIsWall = true;
              }
            }

            if (dir == 1) {
              collisions.right = true;
              if (Mathf.Approximately(slopeAngle, 90)) {
                collisions.rightIsWall = true;
              }
            }
            // same dir.
            if (velocity.x == 0 || dir == Mathf.Sign(velocity.x)) {
              velocity.x = (rays[i].distance - skinWidth) * dir;
            }
          }
        }
      }
    }
    /// <summary>
    /// Tell you if there is something on the left side
    /// NOTE ray origin is raycastOrigins.bottomLeft
    /// </summary>
    public bool IsGroundOnLeft(float rayLengthFactor) {
      Vector3 v = new Vector3(0, 0, 0);
      float rayLength = verticalRayLength * rayLengthFactor;
      RaycastHit2D hit = DoVerticalRay (-1.0f, 0, rayLength, ref v);

      return hit.collider != null;
    }

    /// <summary>
    /// Tell you if there is something on the right side
    /// NOTE ray origin is raycastOrigins.bottomRight
    /// </summary>
    public bool IsGroundOnRight(float rayLengthFactor) {
      Vector3 v = new Vector3(0, 0, 0);
      float rayLength = verticalRayLength * rayLengthFactor;
      RaycastHit2D hit = DoVerticalRay (-1.0f, verticalRayCount - 1, rayLength, ref v);

      return hit.collider != null;
    }

    void VerticalCollisions(int dir, RaycastHit2D[] rays, ref Vector3 velocity) {
      for (int i = 0; i < verticalRayCount; i ++) {

        RaycastHit2D hit = rays[i];

        if (hit) {
          // fallingThroughPlatform ?
          if (
            Configuration.IsOneWayPlatformUp(hit.collider) &&
            collisions.fallingThroughPlatform
          ) {
            continue;
          }

          // left/right wall are ignored for vertical collisions
          if (Configuration.IsOneWayWallLeft(hit.collider) || Configuration.IsOneWayWallRight(hit.collider)) {
            continue;
          }

          if ((
            // ignore up platforms while moving up
            Configuration.IsOneWayPlatformUp(hit.collider) &&
            velocity.y > 0
            ) || (
            // ignore down platforms while moving down
            Configuration.IsOneWayPlatformDown(hit.collider) &&
            velocity.y < 0
          )) {
            continue;
          }

          velocity.y = (hit.distance - minDistanceToEnv) * dir;
          collisions.above = dir == 1;
          collisions.below = dir == -1;
        }
      }
    }

    // this may be redundant
    // if you are very short on performance, you can remove it
    void DiagonalCollisions(ref Vector3 velocity) {
      if (velocity.x == 0) return;

      Vector3 origin = velocity.x > 0 ? raycastOrigins.bottomRight :  raycastOrigins.bottomLeft;
      Vector2 dir = new Vector2(Mathf.Sign(velocity.x), -1);

      RaycastHit2D hit = Physics2D.Raycast(origin, dir, skinWidth, collisionMask);
      Debug.DrawRay(origin, dir * skinWidth, Color.red);

      if (hit && hit.distance < skinWidth * skinWidth) {
        // ignore all oneWay
        if (
          Configuration.IsOneWayWallLeft(hit.collider) ||
          Configuration.IsOneWayWallRight(hit.collider) ||
          Configuration.IsOneWayPlatformUp(hit.collider) ||
          Configuration.IsOneWayPlatformDown(hit.collider)
          ) {
          return;
        }

        velocity.x = 0;
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
          Debug.Log("CLIMB!");
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
        Debug.Log("DECEND!!");
        velocity.y = Mathf.Abs(velocity.x) * slopedir.y;
        collisions.below = true;
      }
    }

    /// <summary>
    /// Disable slopes, so no more ClimbSlope/DescendSlope
    /// This is important, because while on slope, velocity.y will be modified
    /// If you need your velocity to remain, you must disable slopes.
    /// NOTE use it for jumping over a slope
    /// </summary>
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

    /// <summary>
    /// Vector pointing were to descend / slip
    /// </summary>
    public Vector3 GetDownSlopeDir() {
      if (collisions.slopeAngle == 0) {
        return Vector3.zero;
      }

      return new Vector3(
        Mathf.Sign(collisions.slopeNormal.x) * collisions.slopeNormal.y,
        -Math.Abs(collisions.slopeNormal.x),
        0
      );
    }

    /// <summary>
    /// After all work, notify changes
    /// </summary>
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

    //[Serializable]
    public class CollisionInfo {
      // current
      public bool above, below;
      public bool left, right;
      public bool leftIsWall, rightIsWall;
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

      // colliders
      public GameObject slope;
      const int MAX_COLLIDERS = 3;
      public RaycastHit2D[] leftHits;
      public int leftHitsIdx;
      public RaycastHit2D[] rightHits;
      public int rightHitsIdx;

      public CollisionInfo() {
        leftHits = new RaycastHit2D[MAX_COLLIDERS];
        rightHits = new RaycastHit2D[MAX_COLLIDERS];

        for (int i = 0; i < leftHitsIdx; ++i) {
          leftHits[i] = new RaycastHit2D();
          rightHits[i] = new RaycastHit2D();
        }
      }

      public CollisionInfo Clone() {
        return (CollisionInfo) MemberwiseClone();
      }

      public void Reset() {
        above = below = false;
        left = right = false;
        climbingSlope = false;
        descendingSlope = false;
        leftIsWall = rightIsWall = false;

        ++lastAboveFrame;
        ++lastBelowFrame;
        ++lastLeftFrame;
        ++lastRightFrame;

        slopeAngle = 0;
        slopeNormal = Vector3.zero;
        distanceToSlopeStart = 0;

        leftHitsIdx = 0;
        rightHitsIdx = 0;
      }

      public RaycastHit2D GetRightCollider(int i) {
        return i < rightHitsIdx ? rightHits[i] : new RaycastHit2D();
      }

      public RaycastHit2D GetLeftCollider(int i) {
        return i < leftHitsIdx ? leftHits[i] : new RaycastHit2D();
      }

      public void PushLeftCollider(RaycastHit2D c) {
        if (leftHitsIdx == MAX_COLLIDERS) {
          return; // max reached
        }

        // no duplicates
        for (int i = 0; i < leftHitsIdx; ++i) {
          if (leftHits[i].collider == c.collider) {
            return;
          }
        }

        leftHits[leftHitsIdx++] = c;
      }

      public void PushRightCollider(RaycastHit2D c) {
        if (rightHitsIdx == MAX_COLLIDERS) {
          return; // max reached
        }

        // no duplicates
        for (int i = 0; i < rightHitsIdx; ++i) {
          if (rightHits[i].collider == c.collider) {
            return;
          }
        }

        rightHits[rightHitsIdx++] = c;
      }
    }
  }
}
