using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

// credits: http://answers.unity3d.com/questions/486694/default-editor-enum-as-flags-.html
namespace UnityPlatformer {
  /// <summary>
  /// Comment Attribute
  /// </summary>
  public class EnumFlagsAttribute : PropertyAttribute {
    /// <summary>
    /// Constructor
    /// </summary>
    public EnumFlagsAttribute() { }
  }

  #if UNITY_EDITOR
  /// <summary>
  /// Editor Drawer
  /// </summary>
  [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
  public class EnumFlagsAttributeDrawer : PropertyDrawer {
    /// <summary>
    /// Draw
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
    }
  }

  #endif
}
