using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

// TODO fix List problems
namespace UnityPlatformer {

  public class CommentAttribute : PropertyAttribute {
    public readonly string comment;

    public CommentAttribute(string comment) { this.comment = comment; }
  }

  #if UNITY_EDITOR

  [CustomPropertyDrawer(typeof(CommentAttribute))]
  public class CommentDrawer : PropertyDrawer {
    protected float textHeight = 20.0f;
    // clang-format on
    CommentAttribute commentAttribute {
      get { return (CommentAttribute)attribute; }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      // TODO find a way to fix it and support multiple lines
      // Rect labelRect = GUILayoutUtility.GetRect(new
      // GUIContent(commentAttribute.comment), "label");
      // textHeight = labelRect.height;
      // textHeight = GUIStyle.CalcHeight(new
      // GUIContent(commentAttribute.comment));

      //textHeight = 20;
      //return textHeight + base.GetPropertyHeight(property, label);

      return textHeight + EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      EditorGUI.LabelField(position, new GUIContent(commentAttribute.comment));
      EditorGUI.indentLevel = 0;
      position.y += textHeight;
      position.height -= textHeight;


      EditorGUI.PropertyField(position, property, label, true);
      // NOTE this is buggy! do not use it!
      //base.OnGUI(position, property, label);
    }
  }

  #endif
}
