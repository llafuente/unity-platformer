using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer (typeof (SceneAttribute))]
public class SceneDrawer : PropertyDrawer {

  public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
    if (property.propertyType == SerializedPropertyType.Integer) {
      string[] names = new string[EditorBuildSettings.scenes.Length];
      int[] values = new int[EditorBuildSettings.scenes.Length];

      int i = 0;
      foreach (var editorScene in EditorBuildSettings.scenes) {
        values[i] = i;
        names[i] = editorScene.path;
        ++i;
      }

      EditorGUI.LabelField (position, label.text);
      property.intValue = EditorGUI.IntPopup(position, label.text, property.intValue, names, values);
    } else {
      EditorGUI.HelpBox(position, "[Scene] attribute must be used with int or int[].", MessageType.Error);
    }
  }
}
