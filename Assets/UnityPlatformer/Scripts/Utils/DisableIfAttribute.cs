using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// PropertyAttribute for DisableIfDrawer
  /// </summary>
  public class DisableIfAttribute : PropertyAttribute {
    /// <summary>
    /// Comment
    /// </summary>
    public readonly string flagProperty;
    /// <summary>
    /// Constructor
    /// </summary>
    public DisableIfAttribute(string property) {
      this.flagProperty = property;
    }
  }

  #if UNITY_EDITOR
  /// <summary>
  /// Disable Inspector property
  /// </summary>
  [CustomPropertyDrawer(typeof(DisableIfAttribute))]
  public class DisableIfDrawer : PropertyDrawer {
    /// <summary>
    /// Get control height
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      return EditorGUI.GetPropertyHeight(property, label, true);
    }
    /// <summary>
    /// Disable and Draw default control using EditorGUI.PropertyField
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      string flagProperty = ((DisableIfAttribute)attribute).flagProperty;
      SerializedProperty flag = property.serializedObject.FindProperty(flagProperty);

      if (flag == null) {
        Debug.LogError("DisableIf(" + flagProperty + ") property not found");
        return;
      }

      if (flag.boolValue) {
        GUI.enabled = false;
      }

      EditorGUI.PropertyField(position, property, label, true);

      if (flag.boolValue) {
        GUI.enabled = true;
      }
    }
  }

  #endif
}
