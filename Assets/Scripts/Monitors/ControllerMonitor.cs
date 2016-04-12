using UnityEngine;
using UnityEngine.UI;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Monitors {
  public class ControllerMonitor : MonoBehaviour {
    public Color textColor = Color.white;
    public int debugFontSize = 15;
    public Vector2 position = Vector2.zero;

    private string text;

    private GUIStyle guiStyle = new GUIStyle();

    Character character;

    // Use this for initialization
    virtual public void Start() {
		    character = GetComponent<Character> ();
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
        character.controller.collisions.above,
        character.controller.collisions.below,
        character.controller.collisions.left,
        character.controller.collisions.right,
  			character.state.ToString(),
  			character.area.ToString(),
  			character.controller.collisions.climbingSlope,
  			character.controller.collisions.descendingSlope,
  			character.controller.collisions.slopeAngle,
  			character.controller.collisions.slopeAngleOld,
  			character.controller.collisions.faceDir,
  			character.controller.collisions.fallingThroughPlatform,
  			character.controller.collisions.standingOnPlatform,
  			character.controller.collisions.lastAboveFrame,
  			character.controller.collisions.lastBelowFrame,
  			character.controller.collisions.lastLeftFrame,
  			character.controller.collisions.lastRightFrame
      );
    }
  }
}
