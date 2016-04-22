using UnityEngine;
using UnityEngine.UI;

namespace UnityPlatformer {
  public class ControllerMonitor : Monitor {
    PlatformerCollider2D collider;
    // Use this for initialization
    virtual public void Start() {
        collider = GetComponent<PlatformerCollider2D> ();
    }

      // Update is called once per frame
    virtual public void Update() {
      text = string.Format(
        "above? {0} @ {4}\n"+
        "below? {1} @ {5}\n"+
        "left? {2} @ {6}\n"+
        "right? {3} @ {7}\n"+

        "climbingSlope? {8}\n"+
        "descendingSlope? {9}\n"+
        "slopeAngle: {10}\n"+
        "slopeAngleOld: {11}\n"+
        "faceDir: {12}\n"+
        "fallingThroughPlatform: {13}\n"+
        "standingOnPlatform: {14}\n",
        collider.collisions.above,
        collider.collisions.below,
        collider.collisions.left,
        collider.collisions.right,

        collider.collisions.lastAboveFrame,
        collider.collisions.lastBelowFrame,
        collider.collisions.lastLeftFrame,
        collider.collisions.lastRightFrame,

        collider.collisions.climbingSlope,
        collider.collisions.descendingSlope,
        collider.collisions.slopeAngle,
        collider.collisions.prevSlopeAngle,
        collider.collisions.faceDir,
        collider.collisions.fallingThroughPlatform,
        collider.collisions.standingOnPlatform
      );
    }
  }
}
