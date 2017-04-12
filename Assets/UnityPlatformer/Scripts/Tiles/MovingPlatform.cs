using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityPlatformer {
  /// <summary>
  /// Action that MovingPlatform can perform
  /// </summary>
  public enum MovingPlatformActions {
    Nothing,
    Resume,
    ResumeAndStop,
    ReverseAndResume,
    ReverseAndResumeAndStop,
    Stop,
    StopOnNext,
    IfStoppedReverse
  };

  /// <summary>
  /// Moving Platform Tile
  /// </summary>
  public class MovingPlatform : RaycastController, IUpdateEntity {
    [Header("Moving platform")]
    /// <summary>
    /// Mask of things that can be a passanger
    /// </summary>
    [Comment("Who will move while in the platform.")]
    public LayerMask passengerMask;
    /// <summary>
    /// Path that MovingPlatform will follow
    /// </summary>
    public Line path;
    /// <summary>
    /// Speed along the path
    /// </summary>
    public float speed = 2;
    /// <summary>
    /// true: MovingPlatform will be looping
    /// false: MovingPlatform will go back and forth
    /// </summary>
    [Comment("Is a loop? (check) back and forth? (uncheck)")]
    public bool cyclic = false;
    /// <summary>
    /// Delay after each waypoint
    /// </summary>
    [Comment("Delay after each waypoint")]
    public float waitTime = 0;
    /// <summary>
    /// Ease movement
    /// </summary>
    [Range(0,2)]
    public float easeAmount = 0;
    /// <summary>
    /// downcast can move Character in unexpected manners
    /// but it's also necessary for one-way-moving-platforms...
    /// TODO kill player?
    /// </summary>
    public bool disableDownRayCast = false;
    /// <summary>
    /// Initial state stop?
    /// </summary>
    public bool startStopped = false;
    /// <summary>
    /// callback call when passenter enter/leave
    /// </summary>
    public delegate void PassengerEvent(Transform transform);
    /// <summary>
    /// callback call when reach any waypoint
    /// </summary>
    public delegate void WaypointEvent(int index);
    /// <summary>
    /// Called just after select next waypoint and before apply waitTime
    /// </summary>
    public WaypointEvent onWaypointEvent;
    /// <summary>
    /// Called when a Character start being a passenger
    /// </summary>
    public PassengerEvent onEnterPassenger;
    /// <summary>
    /// Called when a Character is not longer a passenger
    /// </summary>
    public PassengerEvent onExitPassenger;
    /// <summary>
    /// callback call when MovingPlatform is stopped
    /// </summary>
    public Action onStop;
    /// <summary>
    /// callback call when MovingPlatform is resume
    /// </summary>
    public Action onResume;

    #region private
    /// <summary>
    /// waypoints list taken from path
    /// </summary>
    [HideInInspector]
    public Vector3[] globalWaypoints;
    /// <summary>
    /// current waypoint index
    /// </summary>
    protected int fromWaypointIndex;
    /// <summary>
    /// current percentage (0-1) between previous/next waypoints
    /// </summary>
    protected float percentBetweenWaypoints;
    /// <summary>
    /// time since last waypoint start, used to calculate (waitTime) delay
    /// </summary>
    protected float nextMoveTime;
    /// <summary>
    /// List of passengers to move
    /// </summary>
    protected List<PassengerMovement> passengerMovement;
    /// <summary>
    /// List of previous passengers
    /// </summary>
    protected List<PassengerMovement> pPassengerMovement;
    /// <summary>
    /// List of passengers to moved
    /// </summary>
    protected HashSet<Transform> prevPassengers = null;
    /// <summary>
    /// Current speed
    /// </summary>
    protected float currentSpeed;
    /// <summary>
    /// Last position
    /// </summary>
    protected Vector3 lastPosition;

    #endregion

    /// <summary>
    /// Setup initial state
    /// </summary>
    public override void Start() {
      base.Start();

      // check that gameObject has a valid tag!
      if (!Configuration.IsMovingPlatform(gameObject)) {
        Debug.LogWarning("Found a MovingPlatform misstagged");
      }

      if (path) {
        Vector3[] localWaypoints = path.points;
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i =0; i < localWaypoints.Length; i++) {
          globalWaypoints[i] = localWaypoints[i] + path.transform.position;
        }
      } else {
        globalWaypoints = null;
      }

      lastPosition = transform.position;
      passengerMovement = new List<PassengerMovement> ();

      if (startStopped) {
        Stop();
      } else {
        Resume();
      }
    }
    /// <summary>
    /// Stop MovingPlatform
    /// </summary>
    public void Stop() {
      currentSpeed = 0;
      if (onStop != null) {
        onStop();
      }
    }
    /// <summary>
    /// TODO
    /// </summary>
    public void StopOn(int waypoints) {
      //stopOnWaypoint = waypoints;
    }
    /// <summary>
    /// Resume movement with default speed
    /// </summary>
    public void Resume() {
      currentSpeed = speed;
      if (onResume != null) {
        onResume();
      }
    }
    /// <summary>
    /// Reverse waypoints, current waypoint and percentBetweenWaypoints
    /// This is used when reach the end, so we don't need handle both logics
    /// </summary>
    public void Reverse() {
      System.Array.Reverse(globalWaypoints);
      fromWaypointIndex = globalWaypoints.Length - 2 - fromWaypointIndex;
      percentBetweenWaypoints = 1 - percentBetweenWaypoints;
    }
    /// <summary>
    /// Reverse waypoints, current waypoint and percentBetweenWaypoints
    /// This is used when reach the end, so we don't need handle both logics
    /// </summary>
    public bool IsStopped() {
      return currentSpeed == 0;
    }
    /// <summary>
    /// Do nothing
    /// </summary>
    public virtual void LatePlatformerUpdate(float delta) {}
    /// <summary>
    /// Get passenger list, pre update, move and post update
    /// </summary>
    public void PlatformerUpdate (float delta) {

      UpdateRaycastOrigins ();
      bounds.Draw(transform);

      // there are two ways to move the platform
      // built-in - speed/path based (path_velocity)
      // platform inherit movement from a parent node (offset_velocity)
      // we need both to be consistent

      Vector3 offset_velocity = transform.position - lastPosition;
      Vector3 path_velocity = Vector3.zero;

      if (globalWaypoints != null) {
        path_velocity += CalculatePlatformMovement(delta);
      }

      CalculatePassengerMovement(path_velocity + offset_velocity);

      MovePassengers(true);
      transform.Translate (path_velocity, Space.World);
      MovePassengers(false);

      lastPosition = transform.position;

      if (onEnterPassenger != null) {
        for (int i = 0; i < passengerMovement.Count; ++i) {
          bool f = false;
          for (int j = 0; j < pPassengerMovement.Count; ++j) {
            if (pPassengerMovement[j].transform == passengerMovement[i].transform) {
              f = true;
              break;
            }
          }
          if (!f) {
            onEnterPassenger(passengerMovement[i].transform);
          }
        }
      }

      if (onExitPassenger != null) {
        for (int i = 0; i < pPassengerMovement.Count; ++i) {
          bool f = false;
          for (int j = 0; j < passengerMovement.Count; ++j) {
            if (pPassengerMovement[i].transform == passengerMovement[j].transform) {
              f = true;
              break;
            }
          }
          if (!f) {
            onExitPassenger(pPassengerMovement[i].transform);
          }
        }
      }
    }
    /// <summary>
    /// Ease movement formula
    /// TODO use UnityPlatformer.Easing even when some configuration don't make sense...
    /// </summary>
    float Ease(float x) {
      float a = easeAmount + 1;
      return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
    }
    /// <summary>
    /// Calculate how much the platform will move
    /// NOTE This can't be called without moving the platform
    /// </summary>
    Vector3 CalculatePlatformMovement(float delta) {

      if (Time.time < nextMoveTime) {
        return Vector3.zero;
      }

      fromWaypointIndex %= globalWaypoints.Length;
      int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
      float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex]);
      percentBetweenWaypoints += delta * currentSpeed / distanceBetweenWaypoints;
      percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
      float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

      Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);

      /* TODO REVIEW this is not buggy, but Collider is with changing slopes, so disable
      Vector3 diff = globalWaypoints [toWaypointIndex] - globalWaypoints [fromWaypointIndex];
      diff.Normalize();
      float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
      transform.localRotation = Quaternion.Euler(0f, 0f, rot_z);
      if (diff.x < 0) {
        transform.localRotation *= Quaternion.Euler(180, 0, 0);
      }
      */

      if (percentBetweenWaypoints >= 1) {
        percentBetweenWaypoints = 0;
        ++fromWaypointIndex;

        if (!cyclic) {
          if (fromWaypointIndex >= globalWaypoints.Length - 1) {
            fromWaypointIndex = 0;
            System.Array.Reverse(globalWaypoints);
          }
        }

        if (onWaypointEvent != null) {
          onWaypointEvent(fromWaypointIndex);
        }

        nextMoveTime = Time.time + waitTime;
      }

      return newPos - transform.position;
    }
    /// <summary>
    /// Move passenger
    /// </summary>
    void MovePassengers(bool beforeMovePlatform) {
      foreach (PassengerMovement passenger in passengerMovement) {
        if (passenger.moveBeforePlatform == beforeMovePlatform) {
          Log.Debug("(MovingPlatform) move passenger {0} {1}", passenger.transform.gameObject.name, passenger.velocity.ToString("F4"));
          passenger.transform.GetComponent<PlatformerCollider2D>().transform.Translate (passenger.velocity, Space.World);
        }
      }
    }
    /// <summary>
    /// Calculate how much each passenger should move
    /// </summary>
    void CalculatePassengerMovement(Vector3 velocity) {
      HashSet<Transform> movedPassengers = new HashSet<Transform> ();
      pPassengerMovement = passengerMovement;
      passengerMovement = new List<PassengerMovement> ();

      float directionY = Mathf.Sign (velocity.y);
      float rayLength = skinWidth * 2 + Configuration.instance.minDistanceToEnv;

      collisionMask = passengerMask;

      // Passenger on top of a horizontally or downward moving platform
      if (velocity.y != 0 || velocity.x != 0) {
        ForeachHeadRay(rayLength, ref velocity, (ref RaycastHit2D ray, ref Vector3 vel, int dir, int idx) => {
          if (ray && ray.distance != 0) {
            if (!movedPassengers.Contains(ray.transform)) {
              movedPassengers.Add(ray.transform);

              passengerMovement.Add(
                new PassengerMovement(ray.transform,
                  new Vector3(vel.x, vel.y, 0), true, false));
            }
          }
        });
      }

      // Vertically moving platform
      // Disable for OWP moving down otherwise will push-down characters
      if (
        // up
        (directionY == 1)
        || // down
        (directionY == -1 && (!disableDownRayCast && !Configuration.IsOneWayPlatformUp(gameObject)))
      ) {
        RayItr itr = (ref RaycastHit2D ray, ref Vector3 vel, int dir, int idx) => {
          if (ray && ray.distance != 0) {
            if (!movedPassengers.Contains(ray.transform)) {
              movedPassengers.Add(ray.transform);
              float pushX = (directionY == 1) ? vel.x : 0;
              float pushY = vel.y - (ray.distance - skinWidth) * directionY;
              passengerMovement.Add(new PassengerMovement(ray.transform,
                new Vector3(pushX, pushY), directionY == 1, true));
            }
          }
        };

        if (directionY == 1) {
          ForeachHeadRay(rayLength, ref velocity, itr);
        } else {
          ForeachFeetRay(rayLength, ref velocity, itr);
        }
      }
/*
      // Horizontally moving platform
      // Cannot be a OWP otherwise will push-right|left characters
      if (velocity.x != 0 && !Configuration.IsOneWayPlatformUp(gameObject)) {
        RayItr itr = (ref RaycastHit2D ray, ref Vector3 vel, int dir, int idx) => {
          if (ray && ray.distance != 0) {
            if (!movedPassengers.Contains(ray.transform)) {
              movedPassengers.Add(ray.transform);
              passengerMovement.Add(new PassengerMovement(ray.transform,
                new Vector3(directionX * (ray.distance - skinWidth), 0), false, true));
            }
          }
        };

        rayLength = skinWidth;

        if (directionX == -1) {
          ForeachLeftRay(rayLength, ref velocity, itr);
        } else {
          ForeachRightRay(rayLength, ref velocity, itr);
        }
      }
*/
      if (prevPassengers != null) {
        // diff
        foreach (var i in movedPassengers) {
          prevPassengers.Remove(i);
        }
        // what stays is out of the platform now: null
        foreach (var i in prevPassengers) {
          Character c = i.gameObject.GetComponent<Character>();
          if (c == null) {
            Debug.LogWarning("Found a passenger that is not a character!", i.gameObject);
          } else {
            c.platform = null;
          }
        }
      }
      // what is updated, is on the platform: set
      foreach (var i in movedPassengers) {
        Character c = i.gameObject.GetComponent<Character>();
        if (c == null) {
          Debug.LogWarning("Found a passenger that is not a character!", i.gameObject);
        } else {
          c.platform = this;
        }
      }
      prevPassengers = movedPassengers;
    }
    /// <summary>
    /// Perform an action over MovingPlatform like: Resume, stop, reverse....
    /// </summary>
    public void DoAction(MovingPlatformActions action) {
      switch (action) {
      case MovingPlatformActions.Resume:
        Resume();
        break;
      case MovingPlatformActions.ResumeAndStop:
        Resume();
        StopOn(1);
        break;
      case MovingPlatformActions.ReverseAndResume:
        Reverse();
        Resume();
        break;
      case MovingPlatformActions.ReverseAndResumeAndStop:
        Reverse();
        Resume();
        StopOn(1);
        break;
      case MovingPlatformActions.Stop:
        Stop();
        break;
      case MovingPlatformActions.StopOnNext:
        StopOn(1);
        break;
      case MovingPlatformActions.IfStoppedReverse:
        if (IsStopped()) {
          Resume();
          StopOn(2);
        }
        break;
      }
    }
    /// <summary>
    /// notify UpdateManager
    /// </summary>
    public override void OnEnable() {
      base.OnEnable();
      UpdateManager.Push(this, Configuration.instance.defaultPriority);
    }
    /// <summary>
    /// notify UpdateManager
    /// </summary>
    void OnDisable() {
      UpdateManager.Remove(this);
    }
    /// <summary>
    /// Struct to store information about passenger movement
    /// </summary>
    public struct PassengerMovement {
      /// <summary>
      /// Transform renference
      /// </summary>
      public Transform transform;
      /// <summary>
      /// MovingPlatform velocity
      /// </summary>
      public Vector3 velocity;
      /// <summary>
      /// Character is on the platform, and the platform push the character up
      /// </summary>
      public bool standingOnPlatform;
      /// <summary>
      /// Flag to tell when to move the Character
      /// Vertical Platforms: true
      /// Horizontal Platforms: false
      /// </summary>
      public bool moveBeforePlatform;
      /// <summary>
      /// constructor
      /// </summary>
      public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform) {
        transform = _transform;
        velocity = _velocity;
        standingOnPlatform = _standingOnPlatform;
        moveBeforePlatform = _moveBeforePlatform;
      }
    }
  }
}
