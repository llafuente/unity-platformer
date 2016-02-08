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
			"above {0}\nbelow {1}\nleft {2}\nright {3}\nstate{4}\narea{5}\n",
            control.collisions.above,
            control.collisions.below,
            control.collisions.left,
            control.collisions.right,
			player ? player.state.ToString() : "null",
			player ? player.area.ToString() : "null"
        );
    }
}
