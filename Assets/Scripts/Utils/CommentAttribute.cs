#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

// clang-format off

public class CommentAttribute : PropertyAttribute
{
  public readonly string comment;

  public CommentAttribute(string comment) { this.comment = comment; }
}

[CustomPropertyDrawer(typeof(CommentAttribute))] public class CommentDrawer : PropertyDrawer
{
  protected float textHeight;
  // clang-format on
  CommentAttribute commentAttribute
  {
    get { return (CommentAttribute)attribute; }
  }

  public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
  {
    // TODO find a way to fix it and support multiple lines
    // Rect labelRect = GUILayoutUtility.GetRect(new
    // GUIContent(commentAttribute.comment), "label");
    // textHeight = labelRect.height;
    // textHeight = GUIStyle.CalcHeight(new
    // GUIContent(commentAttribute.comment));
    textHeight = 20;

    return textHeight + base.GetPropertyHeight(prop, label);
  }

  public override void OnGUI(Rect position, SerializedProperty property,
                      GUIContent label)
  {
    EditorGUI.LabelField(position, new GUIContent(commentAttribute.comment));
    EditorGUI.indentLevel = 0;
    position.y += textHeight;
    position.height -= textHeight;

    EditorGUI.PropertyField(position, property, label, true);
    // NOTE this is buggy! do not use it!
    // base.OnGUI(position, prop, label);
  }
}
#endif
