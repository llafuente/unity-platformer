using System;
using System.Reflection;
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
      Enum targetEnum = GetBaseProperty<Enum>(property);
   
      EditorGUI.BeginProperty(position, label, property);
      Enum enumNew = EditorGUI.EnumMaskField(position, label, targetEnum);
      property.intValue = (int) Convert.ChangeType(enumNew, targetEnum.GetType());
      EditorGUI.EndProperty();
    }

    static T GetBaseProperty<T>(SerializedProperty prop) {
      // Separate the steps it takes to get to this property
      string[] separatedPaths = prop.propertyPath.Split('.');
   
      // Go down to the root of this serialized property
      System.Object reflectionTarget = prop.serializedObject.targetObject as object;
      // Walk down the path to get the target object
      foreach (var path in separatedPaths) {
        FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
        reflectionTarget = fieldInfo.GetValue(reflectionTarget);
      }
      return (T) reflectionTarget;
    }
  }

  #endif
}
