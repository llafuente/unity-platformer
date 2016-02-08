using UnityEngine;
using UnityEngine.UI;

public class ControllerMonitor : MonoBehaviour
{
    public Color textColor = Color.white;
    public int debugFontSize = 15;
    public Vector2 position = Vector2.zero;

    private string text;

    private GUIStyle guiStyle = new GUIStyle();

    Controller2D control;
	PlateformerPlayer player;

    // Use this for initialization
    void Start()
    {
		control = GetComponent<Controller2D> ();
		player = GetComponent<PlateformerPlayer> ();
    }

    void OnGUI() {
        guiStyle.normal.textColor = textColor;

        GUILayout.BeginArea(new Rect(position.x, position.y, Screen.width - position.x, Screen.height - position.y));

        guiStyle.fontSize = debugFontSize;
        GUILayout.Label(text, guiStyle);
        GUILayout.EndArea();
    }

    // Update is called once per frame
    void Update()
    {
        text = string.Format(
			"above {0}\n"+
			"below {1}\n"+
			"left {2}\n"+
			"right {3}\n"+
			"state{4}\n"+
			"area{5}\n"+
			"climbingSlope {6}\n"+
			"descendingSlope {7}\n"+
			"slopeAngle {8}\n"+
			"slopeAngleOld {9}\n"+
			"faceDir {10}\n"+
			"fallingThroughPlatform {11}\n"+
			"standingOnPlatform {12}\n",
            control.collisions.above,
            control.collisions.below,
            control.collisions.left,
            control.collisions.right,
			player ? player.state.ToString() : "null",
			player ? player.area.ToString() : "null",
			control.collisions.climbingSlope,
			control.collisions.descendingSlope,
			control.collisions.slopeAngle,
			control.collisions.slopeAngleOld,
			control.collisions.faceDir,
			control.collisions.fallingThroughPlatform,
			control.collisions.standingOnPlatform

        );
    }
}
