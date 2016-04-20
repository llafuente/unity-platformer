using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Tiles {
  /// <summary>
  /// TODO Stop, Resume, Reverse, StopOnNextWaypoint
  /// </summary>
  public class MovingPlatform : RaycastController {

    public LayerMask passengerMask;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    public bool cyclic;
    public float waitTime;
    [Range(0,2)]
    public float easeAmount;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    List<PassengerMovement> passengerMovement;

    HashSet<Transform> prevPassengers = null;

    public override void Start () {
      // check that gameObject has a valid tag!
      if (this.tag != Configuration.instance.movingPlatformThroughTag &&
        this.tag != Configuration.instance.movingPlatformTag) {
        Debug.LogWarning("Found a MovingPlatform misstagged");
      }

      base.Start ();

      globalWaypoints = new Vector3[localWaypoints.Length];
      for (int i =0; i < localWaypoints.Length; i++) {
        globalWaypoints[i] = localWaypoints[i] + transform.position;
      }
    }

    public void ManagedUpdate (float delta) {

      UpdateRaycastOrigins ();

      Vector3 velocity = CalculatePlatformMovement(delta);

      CalculatePassengerMovement(velocity);

      MovePassengers (true);
      transform.Translate (velocity);
      MovePassengers (false);
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
      percentBetweenWaypoints += delta * speed/distanceBetweenWaypoints;
      percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
      float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

      Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);

      if (percentBetweenWaypoints >= 1) {
        percentBetweenWaypoints = 0;
        fromWaypointIndex ++;

        if (!cyclic) {
          if (fromWaypointIndex >= globalWaypoints.Length-1) {
            fromWaypointIndex = 0;
            System.Array.Reverse(globalWaypoints);
          }
        }
        nextMoveTime = Time.time + waitTime;
      }

      return newPos - transform.position;
    }

    void MovePassengers(bool beforeMovePlatform) {
      foreach (PassengerMovement passenger in passengerMovement) {
        if (passenger.moveBeforePlatform == beforeMovePlatform) {
          passenger.transform.GetComponent<PlatformerCollider2D>().transform.Translate (passenger.velocity);
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
          RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

          Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.green);

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
      if (velocity.y != 0) {
        float rayLength = Mathf.Abs (velocity.y) + skinWidth + Configuration.instance.minDistanceToEnv;

        for (int i = 0; i < verticalRayCount; i ++) {
          Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
          rayOrigin += Vector2.right * (verticalRaySpacing * i);
          RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

          if (hit && hit.distance != 0) {
            if (!movedPassengers.Contains(hit.transform)) {
              hit.collider.GetComponent<PlatformerCollider2D>().collisions.standingOnPlatform = true;
              movedPassengers.Add(hit.transform);
              float pushX = (directionY == 1)?velocity.x:0;
              float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
              passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), directionY == 1, true));
            }
          }
        }
      }

      // Horizontally moving platform
      if (velocity.x != 0) {
        float rayLength = Mathf.Abs (velocity.x) + skinWidth + Configuration.instance.minDistanceToEnv;

        for (int i = 0; i < horizontalRayCount; i ++) {
          Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
          rayOrigin += Vector2.up * (horizontalRaySpacing * i);
          RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

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
          c.platform = null;
        }
      }
      // what is updated, is on the platform: set
      foreach (var i in movedPassengers) {
        Character c = i.gameObject.GetComponent<Character>();
        c.platform = this;
      }
      prevPassengers = movedPassengers;
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

    void OnDrawGizmos() {
      if (localWaypoints != null) {
        Gizmos.color = Color.red;
        float size = .3f;

        for (int i =0; i < localWaypoints.Length; i ++) {
          Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i] : localWaypoints[i] + transform.position;
          Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
          Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
        }
      }
    }
  }
}
