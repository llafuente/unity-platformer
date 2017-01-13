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
  /// Handle collisions with the world
  /// </summary>
  public class PlatformerCollider2D : RaycastController {
    /// <summary>
    /// Override Configuration.gravity
    /// </summary>
    [Comment("Override Configuration.gravity, (0,0) means use global.")]
    public Vector2 gravityOverride = Vector2.zero;
    /// <summary>
    /// Retrievethe real gravity for this Collider
    /// </summary>
    public Vector2 gravity {
      get {
        return gravityOverride == Vector2.zero ? Configuration.instance.gravity : gravityOverride;
      }
    }
    /// <summary>
    /// Maximum angle the collider can walk-up
    /// </summary>
    public float maxClimbAngle = 45.0f;
    /// <summary>
    /// Maximum angle the collider can walk-down
    /// </summary>
    public float maxDescendAngle = 45.0f;
    /// <summary>
    /// Greater than wallAngle will be considered a Wall, not a slope
    /// </summary>
    public float wallAngle = 89.9f; // 90 to disable
    /// <summary>
    /// Enable slopes
    ///
    /// Slopes modify Y position of the collider, even when velocity is really
    /// X only, to give a smooth movement
    /// </summary>
    public bool enableSlopes = true;
    /// <summary>
    /// Prevent unwanted micro changes in orientation/falling.
    ///
    /// Makes the Collider more stable to changes.\n
    /// This is not a magnitude, handle X/Y separately
    /// </summary>
    public float minTranslation = 0.01f;
    /// <summary>
    /// This is experimental staff, If we use RigidBody2D we are sure to never
    /// enter another object by accident and we handle less collisions
    /// but has many drawbacks, that can't be solved atm, like OneWayPlatforms
    /// </summary>
    bool useRigidbody2D = false;
    /// <summary>
    /// Collider hit a wall on right side
    /// </summary>
    public Action onRightWall;
    /// <summary>
    /// Collider hit a wall on left side
    /// </summary>
    public Action onLeftWall;
    /// <summary>
    /// Collider hit the ground
    /// </summary>
    public Action onLanding;
    /// <summary>
    /// Collider leave ground
    /// </summary>
    public Action onLeaveGround;
    /// <summary>
    /// Collider hit something with the 'Head'
    /// </summary>
    public Action onTop;
    /// <summary>
    /// Ignore the decend angle, so always decend.
    /// Used to slide down a slope.
    /// <summary>
    internal bool ignoreDescendAngle = false;
    /// <summary>
    /// Current collision data.
    /// <summary>
    internal CollisionInfo collisions;
    /// <summary>
    /// Previous collision data.
    /// <summary>
    internal CollisionInfo pCollisions;
    /// <summary>
    /// Disable all world collisions, just skip.
    /// It's used in ladders, fences that you don't won't
    /// your world to mess with 'action movement'
    /// <summary>
    internal bool disableWorldCollisions = false;
    /// <summary>
    /// Do not force Collider to stay on ground.
    /// When a collider jump while on a slope, the slope modify Y velocity
    /// and can reduce Y velocity that jump have...
    /// true to allow player to leave ground
    /// <summary>
    internal bool leavingGround = false;
    /// <summary>
    /// Copy of last layer. While moving some callback could fire
    /// This solve problems when something that i can collide is my children
    /// So while moving the Collider is at: Ignore Raycast
    /// <summary>
    int previousLayer;
    /// <summary>
    /// Rigidbody2D
    /// <summary>
    Rigidbody2D rigidBody2D;
    /// <summary>
    /// Cache of slope rays
    /// <summary>
    RaycastHit2D[] slopeRays;
    /// <summary>
    /// Initialization
    /// <summary>
    public virtual void Start() {
      collisions = new CollisionInfo();

      if (useRigidbody2D) {
        rigidBody2D = GetComponent<Rigidbody2D>();
      }
    }
    /// <summary>
    /// More Initialization
    /// <summary>
    public override void OnEnable() {
      base.OnEnable();
      slopeRays = new RaycastHit2D[verticalRayCount + 2];
    }

    /// <summary>
    /// Attempt to move the character to current position + velocity.
    ///
    /// Any colliders in our way will cause velocity to be modified
    /// ex: wall -> stop. slope -> modifiy velocity.\n
    /// NOTE collisions.velocity has the real velocity applied
    /// </summary>
    /// <param name="velocity">Amount to be moved (velocity * delta)</param>
    /// <param name="delta">Time since last update</param>
    /// <returns>Real velocity after collisions</returns>
    public Vector3 Move(Vector3 velocity, float delta) {
      Log.Silly("(PlatformerCollider2D) Move({0}, {1}, {2})", gameObject.name, velocity.ToString("F4"), delta);
      // swap layers, this makes possible to collide with something inside
      // my own layer like boxes
      previousLayer = gameObject.layer;
      gameObject.layer = 2; // Ignore Raycast

      UpdateRaycastOrigins();

      // debug
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

      // Climb or descend a slope if in range
      if (enableSlopes) {
        UpdateCurrentSlope(velocity);

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

      Log.Silly("(PlatformerCollider2D) Moved({0}, {1}, {2})", gameObject.name, velocity.ToString("F4"), delta);

      return velocity;
    }

    /// <summary>
    /// Launch rays and get the maximum slope found
    /// </summary>
    /// <param name="velocity">Amount to be moved</param>
    void UpdateCurrentSlope(Vector3 velocity) {
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

      if (
        index != -1 && // slope is found
        !Mathf.Approximately(slopeAngle, 90) && // slope not close to 90
        !Mathf.Approximately(slopeAngle, 0) && // slope not close to 0
        slopeAngle < wallAngle // slope cannot be considered a wall
      ) {
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
    /// <summary>
    /// Callback for ForeachLeftRay/ForeachRightRay
    /// </summary>
    /// <param name="ray">result of Raycast horizontally</param>
    /// <param name="velocity">Amount to be moved</param>
    /// <param name="dir">Direction</param>
    /// <param name="idx">ray index</param>
    void HorizontalCollisions(ref RaycastHit2D ray, ref Vector3 velocity, int dir, int idx) {
      // hit anything?
      if (!ray || ray.distance == 0) {
        return;
      }

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

      collisions.PushContact(ray, dir == -1 ? Directions.Left : Directions.Right);

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

          velocity.x = dir == 1 ?
            Mathf.Min(velocity.x, (ray.distance - minDistanceToEnv) * dir) :
            Mathf.Max(velocity.x, (ray.distance - minDistanceToEnv) * dir);

          Log.Silly("(PlatformerCollider2D) HorizontalCollisions new velocity {0}", velocity.ToString("F4"));
        }
      }
    }
    /// <summary>
    /// Callback for ForeachHeadRay/ForeachFeetRay
    /// </summary>
    /// <param name="ray">result of Raycast vertically</param>
    /// <param name="velocity">Amount to be moved</param>
    /// <param name="dir">Direction</param>
    /// <param name="idx">ray index</param>
    void VerticalCollisions(ref RaycastHit2D ray, ref Vector3 velocity, int dir, int idx) {
      // hit anything?
      if (!ray || ray.distance == 0) {
        return;
      }

      // when climb/descend a slope we want continuous collisions
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

      collisions.PushContact(ray, dir == 1 ? Directions.Top : Directions.Bottom);

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

          Log.Silly("(PlatformerCollider2D) VerticalCollisions new velocity {0}", velocity.ToString("F4"));
        }
      }

      collisions.above = dir == 1;
    }

    /// <summary>
    /// Unused. This may be redundant still experimental to fix some edge cases...
    /// </summary>
    /// <param name="velocity">Amount to be moved</param>
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
    /// <summary>
    /// Modify velocity to smooth climb current slope
    /// </summary>
    /// <param name="velocity">Amount to be moved</param>
    void ClimbSlope(ref Vector3 velocity) {
      if (!collisions.climbingSlope) {
        return;
      }
      // can climb the slope?
      if (collisions.slopeAngle > maxClimbAngle) {
        velocity.x = 0;
        return;
      }

      Vector3 slopeDir = GetDownSlopeDir();
      //Debug.DrawRay(raycastOrigins.bottomCenter, slopeDir * 10, Color.blue);
      velocity.x *= Mathf.Abs(slopeDir.x);
      velocity.y = Mathf.Abs(velocity.x * slopeDir.y);
      collisions.below = true;

      Log.Silly("(PlatformerCollider2D) ClimbSlope new velocity {0}", velocity.ToString("F4"));
    }
    /// <summary>
    /// Modify velocity to smooth descend current slope
    /// </summary>
    /// <param name="velocity">Amount to be moved</param>
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

        Log.Silly("(PlatformerCollider2D) DescendSlope new velocity {0}", velocity.ToString("F4"));
      }
    }

    /// <summary>
    /// Disable slopes, so no more ClimbSlope/DescendSlope
    ///
    /// This is important, because while on slope, velocity.y will be modified
    /// If you need your velocity to remain, you must disable slopes.\n
    /// NOTE use it for jumping over a slope
    /// </summary>
    /// <param name="resetDelay">0 means no reset</param>
    public void DisableSlopes(float resetDelay = 0.5f) {
      enableSlopes = false;
      if (resetDelay > 0) {
        UpdateManager.SetTimeout(EnableSlopes, resetDelay);
      }
    }
    /// <summary>
    /// same as: 'enableSlopes=true' to be used with: UpdateManager.SetTimeout
    /// </summary>
    public void EnableSlopes() {
      enableSlopes = true;
    }
    /// <summary>
    /// fallingThroughPlatform = true during resetDelay time
    /// </summary>
    /// <param name="resetDelay">0 means no reset</param>
    public void FallThroughPlatform(float resetDelay = 0.5f) {
      // defense!
      if (collisions.fallingThroughPlatform) {
        return;
      }

      collisions.fallingThroughPlatform = true;
      if (resetDelay > 0) {
        UpdateManager.SetTimeout(ResetFallingThroughPlatform, resetDelay);
      }
    }
    /// <summary>
    /// same as: 'fallingThroughPlatform=false' to be used with: UpdateManager.SetTimeout
    /// </summary>
    public void ResetFallingThroughPlatform() {
      collisions.fallingThroughPlatform = false;
    }
    /// <summary>
    /// leavingGround = true during resetDelay time
    /// </summary>
    /// <param name="resetDelay">0 means no reset</param>
    public void EnableLeaveGround(float resetDelay = 0.5f) {
      leavingGround = true;

      if (resetDelay > 0) {
        UpdateManager.SetTimeout(DisableLeaveGround, resetDelay);
      }
    }
    /// <summary>
    /// same as: 'leavingGround=false' to be used with: UpdateManager.SetTimeout
    /// </summary>
    public void DisableLeaveGround() {
      leavingGround = false;
    }
    /// <summary>
    /// Collider is/was on ground.
    ///
    /// This is useful to give player some help, no pixel precision jumps :)
    /// </summary>
    /// <param name="graceFrames">0 means current frame</param>
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
      } else {
        collisions.belowFrames = -1;
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

    /// <summary>
    /// Contact class
    /// </summary>
    [Serializable]
    public class Contacts {
      /// <summary>
      /// RaycastHit2D data
      /// </summary>
      public RaycastHit2D hit;
      /// <summary>
      /// Direction
      /// </summary>
      public Directions dir;
      /// <summary>
      /// Constructor
      /// </summary>
      public Contacts(RaycastHit2D _hit, Directions _dir) {
        hit = _hit;
        dir = _dir;
      }
    }

    /// <summary>
    /// Collision info
    /// </summary>
    [Serializable]
    public class CollisionInfo {
      /// <summary>
      /// There is a collision at top
      /// </summary>
      public bool above;
      /// <summary>
      /// There is a collision at bottom
      /// </summary>
      public bool below;
      /// <summary>
      /// There is a collision at left
      /// </summary>
      public bool left;
      /// <summary>
      /// There is a collision at right
      /// </summary>
      public bool right;
      /// <summary>
      /// There is a wall at left
      /// </summary>
      public bool leftIsWall;
      /// <summary>
      /// There is a wall at right
      /// </summary>
      public bool rightIsWall;
      /// <summary>
      /// Slope angle
      /// </summary>
      public float slopeAngle;
      /// <summary>
      /// Slope normal
      /// </summary>
      public Vector3 slopeNormal;
      /// <summary>
      /// Real velocity Collider has moved
      /// </summary>
      public Vector3 velocity;
      /// <summary>
      /// frames since last top collision
      /// </summary>
      public int lastAboveFrame;
      /// <summary>
      /// frames since last bottom collision
      /// </summary>
      public int lastBelowFrame;
      /// <summary>
      /// frames since last left collision
      /// </summary>
      public int lastLeftFrame;
      /// <summary>
      /// frames since last right collision
      /// </summary>
      public int lastRightFrame;
      /// <summary>
      /// consecutive frame that collision below
      /// if !below then -1
      /// </summary>
      public int belowFrames;
      /// <summary>
      /// Collider is climbing a slope
      /// </summary>
      public bool climbingSlope;
      /// <summary>
      /// Collider is descending a slope
      /// </summary>
      public bool descendingSlope;
      /// <summary>
      /// Distance to slope
      /// </summary>
      public float slopeDistance;
      /// <summary>
      /// Is Collider able to fall through a platform
      /// </summary>
      public bool fallingThroughPlatform;
      /// <summary>
      /// Slope GameObject
      /// </summary>
      public GameObject slope;
      /// <summary>
      /// Maximum contact to store
      /// </summary>
      const int MAX_CONTACTS = 3;
      /// <summary>
      /// Contact list
      /// </summary>
      public Contacts[] contacts;
      /// <summary>
      /// Contact counter
      /// </summary>
      public int contactsCount = 0;
      /// <summary>
      /// Constructor
      /// </summary>
      public CollisionInfo() {
        contacts = new Contacts[MAX_CONTACTS];
      }
      /// <summary>
      /// Clone
      /// </summary>
      public CollisionInfo Clone() {
        return (CollisionInfo)MemberwiseClone();
      }
      /// <summary>
      /// Reset current collision object
      /// </summary>
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

        ++belowFrames;

        slopeAngle = 0;
        slopeNormal = Vector3.zero;
        slopeDistance = 0;

        contactsCount = 0;
      }
      /// <summary>
      /// Add contact to given direction
      /// </summary>
      public void PushContact(RaycastHit2D hit, Directions dir) {
        if (contactsCount == MAX_CONTACTS) {
          return; // max reached
        }

        // no duplicates
        for (int i = 0; i < contactsCount; ++i) {
          if (contacts[i].hit.collider == hit.collider && contacts[i].dir == dir) {
            return;
          }
        }

        contacts[contactsCount] = new Contacts(hit, dir);

        ++contactsCount;
      }
    }
  }
}
