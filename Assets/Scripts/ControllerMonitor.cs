using UnityEngine;
using UnityEngine.UI;
namespace UnityPlatformer {
  public class ControllerMonitor : MonoBehaviour {
    public Color textColor = Color.white;
    public int debugFontSize = 15;
    public Vector2 position = Vector2.zero;

    private string text;

    private GUIStyle guiStyle = new GUIStyle();

    Controller2D control;

    // Use this for initialization
    virtual public void Start() {
		    control = GetComponent<Controller2D> ();
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
  			"above? {0} @ {13}\n"+
  			"below? {1} @ {14}\n"+
  			"left? {2} @ {15}\n"+
  			"right? {3} @ {16}\n"+
  			"state: {4}\n"+
  			"area: {5}\n"+
  			"climbingSlope? {6}\n"+
  			"descendingSlope? {7}\n"+
  			"slopeAngle: {8}\n"+
  			"slopeAngleOld: {9}\n"+
  			"faceDir: {10}\n"+
  			"fallingThroughPlatform: {11}\n"+
  			"standingOnPlatform: {12}\n",
        control.collisions.above,
        control.collisions.below,
        control.collisions.left,
        control.collisions.right,
  			control.state.ToString(),
  			control.area.ToString(),
  			control.collisions.climbingSlope,
  			control.collisions.descendingSlope,
  			control.collisions.slopeAngle,
  			control.collisions.slopeAngleOld,
  			control.collisions.faceDir,
  			control.collisions.fallingThroughPlatform,
  			control.collisions.standingOnPlatform,
  			control.collisions.lastAboveFrame,
  			control.collisions.lastBelowFrame,
  			control.collisions.lastLeftFrame,
  			control.collisions.lastRightFrame
      );
    }
  }
}
