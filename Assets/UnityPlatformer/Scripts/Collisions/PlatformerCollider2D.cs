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

using System;
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
    internal bool ignoreDescendAngle = false;
    internal CollisionInfo collisions;
    internal CollisionInfo pCollisions;
    internal bool disableWorldCollisions = false;
    internal bool leavingGround = false;

    #endregion

    int previousLayer;
    Rigidbody2D rigidBody2D;
    RaycastHit2D[] slopeRays;

    public virtual void Start() {
      collisions = new CollisionInfo();
      collisions.faceDir = 1;

      if (useRigidbody2D) {
        rigidBody2D = GetComponent<Rigidbody2D>();
      }
    }

    public override void OnEnable() {
      base.OnEnable();
      slopeRays = new RaycastHit2D[verticalRayCount + 2];
    }

    /// <summary>
    /// Attempt to move the character to position + velocity.
    /// Any colliders in the way will cause velocity to be modified
    /// NOTE collisions.velocity has the real velocity applied
    /// </summary>
    public Vector3 Move(Vector3 velocity, float delta) {
      // swap layers, this makes possible to collide with something inside my own layer
      // like boxes
      previousLayer = gameObject.layer;
      gameObject.layer = 2; // Ignore Raycast

      UpdateRaycastOrigins();

      var b = bounds;
      b.Draw(transform, new Color(1,1,1, 0.25f));
      b.center += velocity;
      b.Draw(transform, new Color(0,0,1, 0.5f));
      b.Expand(minDistanceToEnv * 2);
      //b.center -= new Vector3(minDistanceToEnv, minDistanceToEnv, 0);
      b.Draw(transform, new Color(0,0,1, 0.75f));

      // set previous collisions and reset current one
      pCollisions = collisions.Clone();
      collisions.Reset();

      // facing need to be reviewed. We should not rely on velocity.x
      if (velocity.x > 0.0f) {
        collisions.faceDir = 1;
      } else if (velocity.x < 0.0f) {
        collisions.faceDir = -1;
      } // else, leave the last one :)

      // Climb or descend a slope if in range
      if (enableSlopes) {
        GetCurrentSlope(ref velocity);

        // TODO PERF add: pCcollisions.below, so wont be testing while falling
        // if (collisions.slopeAngle != 0 && pCollisions.below) {
        if (collisions.slopeAngle != 0) {
          //Debug.Log("velocity before" + velocity.ToString("F4"));

          ClimbSlope(ref velocity);
          DescendSlope(ref velocity);

          //Debug.Log("velocity after" + velocity.ToString("F4"));
        }
      }



      // be sure we stay outside others colliders
      if (!disableWorldCollisions) {
        ForeachLeftRay(skinWidth, ref velocity, HorizontalCollisions);
        ForeachRightRay(skinWidth, ref velocity, HorizontalCollisions);

        if (velocity.y > 0) {
          ForeachFeetRay(skinWidth, ref velocity, VerticalCollisions);
          ForeachHeadRay(skinWidth, ref velocity, VerticalCollisions);
        } else {
          ForeachFeetRay(skinWidth, ref velocity, VerticalCollisions);
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
        transform.Translate(velocity);
      }
      collisions.velocity = velocity;
      ConsolidateCollisions();

      gameObject.layer = previousLayer;

      b = bounds;
      b.center += velocity;
      b.Draw(transform, new Color(0,1,1,0.25f));

      return velocity;
    }

    /// <summary>
    /// Launch rays and get the maximum slope found
    /// </summary>
    void GetCurrentSlope(ref Vector3 velocity) {
      // TODO perf
      float slopeAngle = 0.0f;
      int index = -1;

      Vector2[] dirs = new Vector2[4] {
        new Vector2(-1, -1),
        new Vector2(0, -1),
        new Vector2(1, -1),
        new Vector2(0, -1)
      };

      slopeRays[0] = Raycast(raycastOrigins.bottomLeft, dirs[0], skinWidth, collisionMask);
      slopeRays[1] = Raycast(raycastOrigins.bottomLeft, dirs[1], skinWidth, collisionMask);
      slopeRays[2] = Raycast(raycastOrigins.bottomRight, dirs[2], skinWidth, collisionMask);
      slopeRays[3] = Raycast(raycastOrigins.bottomRight, dirs[3], skinWidth, collisionMask);

      for (int i = 0; i < 4; ++i) {
        if (slopeRays[i].distance != 0) {
          //Debug.DrawRay(raycastOrigins.bottomCenter, slopeRays[i].normal * 5, new Color(0.2f * i, 0, 0, 1));
          //Debug.LogFormat("idx {0} normal {1} distance {2} direction", i, slopeRays[i].normal, slopeRays[i].distance, dirs[i]);
          float a = Vector2.Angle(slopeRays[i].normal, Vector2.up);
          if (a > slopeAngle) {
            index = i;
            slopeAngle = a;
          }
        }
      }

      if (index != -1 && !Mathf.Approximately(slopeAngle, 90) && !Mathf.Approximately(slopeAngle, 0)) {
        collisions.slopeAngle = slopeAngle;

        collisions.slopeNormal = slopeRays[index].normal;
        collisions.slopeDistance = slopeRays[index].distance;

        if (velocity.x != 0.0f) {
          int sloperDir = (int)Mathf.Sign(-slopeRays[index].normal.x);
          collisions.climbingSlope = sloperDir == Mathf.Sign(velocity.x);
          collisions.descendingSlope = sloperDir != Mathf.Sign(velocity.x);
        }

        collisions.slope = slopeRays[index].collider.gameObject;
      } else {
        collisions.slope = null;
        collisions.slopeAngle = 0.0f;
      }
    }

    void HorizontalCollisions(ref RaycastHit2D ray, ref Vector3 velocity, int dir, int idx) {
      if (ray && ray.distance != 0) {
        // ignore oneWayPlatformsUp/Down, both aren't walls
        if (Configuration.IsOneWayPlatformUp(ray.collider) || Configuration.IsOneWayPlatformDown(ray.collider)) {
          return;
        }

        if ((
          // ignore left wall while moving left
          Configuration.IsOneWayWallLeft(ray.collider) &&
          velocity.x < 0
          ) || (
          // ignore right wall while moving right
          Configuration.IsOneWayWallRight(ray.collider) &&
          velocity.x > 0
        )) {
          return;
        }


        if (dir == -1) {
          collisions.PushLeftCollider(ray);
        } else {
          collisions.PushRightCollider(ray);
        }

        float slopeAngle = Vector2.Angle(ray.normal, Vector2.up);
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
          // same direction
          // TODO REVIEW check variable-slope. while on ground i the slope push
          // the character strange things happens because of this
          if (velocity.x == 0 || dir == Mathf.Sign(velocity.x)) {
            velocity.x = (ray.distance - minDistanceToEnv) * dir;
          }
        }
      }
    }

    void VerticalCollisions(ref RaycastHit2D ray, ref Vector3 velocity, int dir, int idx) {
      // when climb/descend a slop we want continuous collisions
      // to do that we just need to separate a specific corner
      // if more test are perform it end up being unstable
      if (collisions.climbingSlope) {
        if (velocity.x > 0 && idx != horizontalRayCount - 1) return;
        if (velocity.x < 0 && idx != 0) return;
      }
      if (collisions.descendingSlope) {
        if (velocity.x > 0 && idx != 0) return;
        if (velocity.x < 0 && idx != horizontalRayCount - 1) return;
      }

      if (ray.distance != 0) {
        // fallingThroughPlatform ?
        if (
          Configuration.IsOneWayPlatformUp(ray.collider) &&
          collisions.fallingThroughPlatform
        ) {
          return;
        }

        // left/right wall are ignored for vertical collisions
        if (Configuration.IsOneWayWallLeft(ray.collider) || Configuration.IsOneWayWallRight(ray.collider)) {
          return;
        }

        if ((
          // ignore up platforms while moving up
          Configuration.IsOneWayPlatformUp(ray.collider) &&
          velocity.y > 0
          ) || (
          // ignore down platforms while moving down
          Configuration.IsOneWayPlatformDown(ray.collider) &&
          velocity.y < 0
        )) {
          return;
        }

        if (!leavingGround) {
          // Separate but only if we are not jumping
          velocity.y = (ray.distance - minDistanceToEnv) * dir;
          collisions.below = dir == -1;
        } else if (ray.distance < minDistanceToEnv * 0.5f) {
          // we just want to override if we are not separating enough from
          // ground also, do not set collision below until that moment or
          // current jump will be stopped
          float wanted = (ray.distance - minDistanceToEnv) * dir;
          if (velocity.y < wanted) {
            velocity.y = wanted;
            collisions.below = dir == -1;
          }
        }

        collisions.above = dir == 1;
      }
    }

    // this may be redundant
    // if you are very short on performance, you can remove it
    void DiagonalCollisions(ref Vector3 velocity) {
      if (velocity.x == 0) return;

      Vector3 origin = velocity.x > 0 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
      Vector2 dir = new Vector2(Mathf.Sign(velocity.x), -1);

      RaycastHit2D hit = Physics2D.Raycast(origin, dir, skinWidth, collisionMask);
      //Debug.DrawRay(origin, dir * skinWidth, Color.red);

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
        Vector3 slopeDir = GetDownSlopeDir();
        //Debug.DrawRay(raycastOrigins.bottomCenter, slopeDir * 10, Color.blue);
        velocity.x *= Mathf.Abs(slopeDir.x);
        velocity.y = Mathf.Abs(velocity.x * slopeDir.y);
        collisions.below = true;
      }
    }

    void DescendSlope(ref Vector3 velocity) {
      if (collisions.descendingSlope &&
        (collisions.slopeAngle <= maxDescendAngle || ignoreDescendAngle)
      ) {
        Vector3 slopeDir = GetDownSlopeDir();
        //Debug.DrawRay(raycastOrigins.bottomCenter, slopeDir * 10, Color.blue);
        //Debug.DrawRay(raycastOrigins.bottomCenter, collisions.slopeNormal * 10, Color.blue);
        velocity.x = velocity.x * Math.Abs(slopeDir.x);
        //velocity.y = (Mathf.Abs(velocity.x) + collisions.slopeDistance) * slopeDir.y;
        velocity.y = Mathf.Abs(velocity.x) * slopeDir.y;
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
          onRightWall();
        }
      }

      if (collisions.left && !pCollisions.left) {
        if (onLeftWall != null) {
          onLeftWall();
        }
      }

      if (collisions.above && !pCollisions.above) {
        if (onTop != null) {
          onTop();
        }
      }

      if (!collisions.below && pCollisions.below) {
        if (onLeaveGround != null) {
          onLeaveGround();
        }
      }

      if (collisions.below && !pCollisions.below && onLanding != null) {
        onLanding();
      }
    }

    [Serializable]
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
      public float slopeDistance;
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
        return (CollisionInfo)MemberwiseClone();
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
        slopeDistance = 0;

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
