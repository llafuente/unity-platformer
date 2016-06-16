using UnityEngine;
using UnityEngine.UI;

namespace UnityPlatformer {
  public class ControllerMonitor : Monitor {
    PlatformerCollider2D pc2d;
    // Use this for initialization
    virtual public void Start() {
        pc2d = GetComponent<PlatformerCollider2D> ();
    }

      // Update is called once per frame
    virtual public void Update() {
      text = string.Format(
        "above? {0} @ {4}\n"+
        "below? {1} @ {5}\n"+
        "left? {2}({15}) @ {6}\n"+
        "right? {3}({16}) @ {7}\n"+

        "climbingSlope? {8}\n"+
        "descendingSlope? {9}\n"+
        "slopeAngle: {10}\n"+
        "distanceToSlopeStart: {13}\n"+
        "slopeNormal: {14}\n"+
        "faceDir: {11}\n"+
        "fallingThroughPlatform: {12}\n",
        pc2d.collisions.above,
        pc2d.collisions.below,
        pc2d.collisions.left,
        pc2d.collisions.right,

        pc2d.collisions.lastAboveFrame,
        pc2d.collisions.lastBelowFrame,
        pc2d.collisions.lastLeftFrame,
        pc2d.collisions.lastRightFrame,

        pc2d.collisions.climbingSlope,
        pc2d.collisions.descendingSlope,
        pc2d.collisions.slopeAngle,
        pc2d.collisions.faceDir,
        pc2d.collisions.fallingThroughPlatform,

        pc2d.collisions.distanceToSlopeStart,
        pc2d.collisions.slopeNormal,
        pc2d.collisions.leftHitsIdx,
        pc2d.collisions.rightHitsIdx
      );
    }
  }
}
