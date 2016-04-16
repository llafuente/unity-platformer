using UnityEngine;
using UnityEngine.UI;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Monitors {
  public class ControllerMonitor : MonoBehaviour {
    public Color textColor = Color.white;
    public int debugFontSize = 15;
    public Vector2 position = Vector2.zero;

    protected string text;
    PlatformerCollider2D collider;

    private GUIStyle guiStyle = new GUIStyle();

    // Use this for initialization
    virtual public void Start() {
        collider = GetComponent<PlatformerCollider2D> ();
    }

    virtual public void OnGUI() {
      guiStyle.normal.textColor = textColor;

      GUILayout.BeginArea(new Rect(position.x, position.y, Screen.width - position.x, Screen.height - position.y));

      guiStyle.fontSize = debugFontSize;
      GUILayout.Label(text, guiStyle);
      GUILayout.EndArea();
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
