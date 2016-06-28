using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityPlatformer {
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
  /// TODO Stop, Resume, Reverse, StopOnNextWaypoint
  /// </summary>
  public class MovingPlatform : RaycastController, IUpdateEntity {
    #region public

    [Comment("Who will move while in the platform.")]
    public LayerMask passengerMask;
    public Line path;
    public float speed = 2;
    [Comment("Is a loop? (check) back and forth? (uncheck)")]
    public bool cyclic = false;
    [Comment("Delay after each waypoint")]
    public float waitTime = 0;
    [Range(0,2)]
    public float easeAmount = 0;
    public bool disableDownRayCast = false;
    public bool startStopped = false;

    public delegate void ReachWaypoint(int index);
    /// <summary>
    /// It's called just after select next waypoint and before apply waitTime
    /// </summary>
    public ReachWaypoint onReachWaypoint;
    public Action onStop;
    public Action onResume;

    #endregion

    #region private

    [HideInInspector]
    public Vector3[] globalWaypoints;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;
    List<PassengerMovement> passengerMovement;
    HashSet<Transform> prevPassengers = null;
    float currentSpeed;

    Vector3 lastPosition;

    #endregion

    public override void Start () {
      // check that gameObject has a valid tag!
      if (!Configuration.IsMovingPlatformThrough(gameObject) &&
        !Configuration.IsMovingPlatform(gameObject)) {
        Debug.LogWarning("Found a MovingPlatform misstagged");
      }

      base.Start ();
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

    public void StopOn(int waypoints) {
      //stopOnWaypoint = waypoints;
    }

    public void Resume() {
      currentSpeed = speed;
      if (onResume != null) {
        onResume();
      }
    }

    public void Reverse() {
      System.Array.Reverse(globalWaypoints);
      fromWaypointIndex = globalWaypoints.Length - 2 - fromWaypointIndex;
      percentBetweenWaypoints = 1 - percentBetweenWaypoints;
    }

    public bool IsStopped() {
      return currentSpeed == 0;
    }

    public void ManagedUpdate (float delta) {

      UpdateRaycastOrigins ();

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

      MovePassengers (true);
      transform.Translate (path_velocity, Space.World);
      MovePassengers (false);

      lastPosition = transform.position;
    }

    float Ease(float x) {
      float a = easeAmount + 1;
      return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
    }

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

        if (onReachWaypoint != null) {
          onReachWaypoint(fromWaypointIndex);
        }

        nextMoveTime = Time.time + waitTime;
      }

      return newPos - transform.position;
    }

    void MovePassengers(bool beforeMovePlatform) {
      foreach (PassengerMovement passenger in passengerMovement) {
        if (passenger.moveBeforePlatform == beforeMovePlatform) {
          passenger.transform.GetComponent<PlatformerCollider2D>().transform.Translate (passenger.velocity, Space.World);
        }
      }
    }

    void CalculatePassengerMovement(Vector3 velocity) {
      HashSet<Transform> movedPassengers = new HashSet<Transform> ();
      passengerMovement = new List<PassengerMovement> ();

      float directionX = Mathf.Sign (velocity.x);
      float directionY = Mathf.Sign (velocity.y);

      // Passenger on top of a horizontally or downward moving platform
      if (directionY == -1 || velocity.y == 0 && velocity.x != 0) {
        float rayLength = skinWidth * 2 + Configuration.instance.minDistanceToEnv;

        for (int i = 0; i < verticalRayCount; i ++) {
          Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
          RaycastHit2D hit = Raycast(rayOrigin, Vector2.up, rayLength, passengerMask, Color.green);

          if (hit && hit.distance != 0) {
            if (!movedPassengers.Contains(hit.transform)) {
              movedPassengers.Add(hit.transform);

              passengerMovement.Add(
                new PassengerMovement(hit.transform,
                  new Vector3(velocity.x,velocity.y, 0), true, false));
            }
          }
        }
      }

      // Vertically moving platform
      // Disable for OWP moving down otherwise will push-down characters
      if (
        (velocity.y < 0 &&  !Configuration.IsOneWayPlatformUp(gameObject) && !disableDownRayCast) ||
        velocity.y > 0
      ) {
        float rayLength = Mathf.Abs (velocity.y) + skinWidth + Configuration.instance.minDistanceToEnv;

        for (int i = 0; i < verticalRayCount; i ++) {
          Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
          rayOrigin += Vector2.right * (verticalRaySpacing * i);
          RaycastHit2D hit = Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask, Color.red);

          if (hit && hit.distance != 0) {
            if (!movedPassengers.Contains(hit.transform)) {
              movedPassengers.Add(hit.transform);
              float pushX = (directionY == 1)?velocity.x:0;
              float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
              passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), directionY == 1, true));
            }
          }
        }
      }

      // Horizontally moving platform
      // Cannot be a OWP otherwise will push-right|left characters
      if (velocity.x != 0 && !Configuration.IsOneWayPlatformUp(gameObject)) {
        float rayLength = Mathf.Abs (velocity.x) + skinWidth + Configuration.instance.minDistanceToEnv;

        for (int i = 0; i < horizontalRayCount; i ++) {
          Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
          rayOrigin += Vector2.up * (horizontalRaySpacing * i);
          RaycastHit2D hit = Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask, Color.yellow);

          if (hit && hit.distance != 0) {
            if (!movedPassengers.Contains(hit.transform)) {
              movedPassengers.Add(hit.transform);
              float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
              float pushY = -skinWidth;
              passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), false, true));
            }
          }
        }
      }

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

    void OnEnable() {
      UpdateManager.instance.Push(this, Configuration.instance.defaultPriority);
    }

    void OnDisable() {
      UpdateManager.instance.Remove(this);
    }

    struct PassengerMovement {
      public Transform transform;
      public Vector3 velocity;
      public bool standingOnPlatform;
      public bool moveBeforePlatform;

      public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform) {
        transform = _transform;
        velocity = _velocity;
        standingOnPlatform = _standingOnPlatform;
        moveBeforePlatform = _moveBeforePlatform;
      }
    }
  }
}
