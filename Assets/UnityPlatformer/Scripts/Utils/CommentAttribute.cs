using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

// TODO fix List problems
namespace UnityPlatformer {
  /// <summary>
  /// Comment Attribute
  /// </summary>
  public class CommentAttribute : PropertyAttribute {
    /// <summary>
    /// Comment
    /// </summary>
    public readonly string comment;
    public readonly MessageType messageType = MessageType.Info;
    /// <summary>
    /// Constructor
    /// </summary>
    public CommentAttribute(string comment) { this.comment = comment; }
  }

  #if UNITY_EDITOR
  /// <summary>
  /// Editor Drawer
  /// </summary>
  [CustomPropertyDrawer(typeof(CommentAttribute))]
  public class CommentDrawer : DecoratorDrawer {
    /// <summary>
    /// Getter CommentAttribute
    /// </summary>
    CommentAttribute commentAttribute {
      get { return (CommentAttribute)attribute; }
    }
    /// <summary>
    /// Calc height needed, right now is always 20
    /// </summary>
    public override float GetHeight() {
      GUIStyle style = "HelpBox";

      return Mathf.Max(
        style.CalcHeight(
          new GUIContent(commentAttribute.comment),
          Screen.width
        ),
        20
      );
    }
    /// <summary>
    /// Draw
    /// </summary>
    public override void OnGUI(Rect position) {
      // this include an icon, not needed...
      //EditorGUI.HelpBox(position, commentAttribute.comment, commentAttribute.messageType);
      EditorGUI.LabelField(position, commentAttribute.comment, EditorStyles.helpBox);
    }
  }

  #endif
}
