using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

// TODO fix List problems
namespace UnityPlatformer {
  /// <summary>
  /// Help Attribute
  /// </summary>
  public class HelpAttribute : PropertyAttribute {
    /// <summary>
    /// Help
    /// </summary>
    public readonly string Help;
    public readonly MessageType messageType = MessageType.Info;
    /// <summary>
    /// Constructor
    /// </summary>
    public HelpAttribute(string Help) { this.Help = Help; }
  }

  #if UNITY_EDITOR
  /// <summary>
  /// Editor Drawer
  /// </summary>
  [CustomPropertyDrawer(typeof(HelpAttribute))]
  public class HelpDrawer : DecoratorDrawer {
    /// <summary>
    /// Getter HelpAttribute
    /// </summary>
    HelpAttribute HelpAttribute {
      get { return (HelpAttribute)attribute; }
    }
    /// <summary>
    /// Calc height needed, right now is always 20
    /// </summary>
    public override float GetHeight() {
      GUIStyle style = "HelpBox";

      return Mathf.Max(
        style.CalcHeight(
          new GUIContent(HelpAttribute.Help),
          Screen.width - 21
        ),
        20
      );
    }
    /// <summary>
    /// Draw
    /// </summary>
    public override void OnGUI(Rect position) {
      // this include an icon, not needed...
      EditorGUI.HelpBox(position, HelpAttribute.Help, HelpAttribute.messageType);
    }
  }

  #endif
}
