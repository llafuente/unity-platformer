using UnityEngine;
using UnityEngine.UI;

namespace UnityPlatformer {
  /// <summary>
  /// Extend this class to print debug-info at screen
  /// </summary>
  public abstract class Monitor : MonoBehaviour {
    /// <summary>
    /// Text color
    /// </summary>
    public Color textColor = Color.white;
    /// <summary>
    /// Font size
    /// </summary>
    public int debugFontSize = 15;
    /// <summary>
    /// Position in the screen in pixels
    /// </summary>
    public Vector2 position = Vector2.zero;
    /// <summary>
    /// Text to be displayed
    /// </summary>
    protected string text;
    /// <summary>
    /// Computed style
    /// </summary>
    protected GUIStyle guiStyle = new GUIStyle();
    /// <summary>
    /// Display text
    /// </summary>
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
