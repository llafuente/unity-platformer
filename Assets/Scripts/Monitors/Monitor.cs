using UnityEngine;
using UnityEngine.UI;

namespace UnityPlatformer {
  /// <summary>
  /// Extend this class to print debug-info at screen
  /// </summary>
  public abstract class Monitor : MonoBehaviour{
    public Color textColor = Color.white;
    public int debugFontSize = 15;
    public Vector2 position = Vector2.zero;

    protected string text;
    protected GUIStyle guiStyle = new GUIStyle();

    virtual public void OnGUI() {
      guiStyle.normal.textColor = textColor;

      GUILayout.BeginArea(new Rect(
        position.x,
        position.y,
        Screen.width - position.x,
        Screen.height - position.y
        )
      );

      guiStyle.fontSize = debugFontSize;
      GUILayout.Label(text, guiStyle);
      GUILayout.EndArea();
    }
  }
}
