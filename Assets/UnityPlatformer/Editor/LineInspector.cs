using UnityEditor;
using UnityEngine;
using System;

namespace UnityPlatformer {
  /// <summary>
  /// Line editor
  ///
  /// Many thanks for the tutorial to
  /// http://catlikecoding.com/unity/tutorials/curves-and-splines/
  /// </summary>
  [CustomEditor(typeof(Line))]
  public class LineInspector : Editor {
    /// <summary>
    /// On inspector draw helpers
    /// </summary>
    public override void OnInspectorGUI () {
      Line line = target as Line;

      if (GUILayout.Button("Add Point")) {
        Undo.RecordObject(line, "Add Point");
        Array.Resize(ref line.points, line.points.Length + 1);

        line.points[line.points.Length -1] = line.points[line.points.Length -2] + new Vector3(1, 0, 0);
        EditorUtility.SetDirty(line);
      }

      if (GUILayout.Button("Remove Last Point")) {
        Undo.RecordObject(line, "Remove Point");
        Array.Resize(ref line.points, line.points.Length - 1);
        EditorUtility.SetDirty(line);
      }

      DrawDefaultInspector ();
    }

    private void OnSceneGUI () {
      Line line = target as Line;
      Transform handleTransform = line.transform;
      Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

      Handles.color = Color.white;
      for (int i = 0; i < line.points.Length; ++i) {
        EditorGUI.BeginChangeCheck();
        Vector3 p = handleTransform.TransformPoint(line.points[i]);
        p = Handles.DoPositionHandle(p, handleRotation);
        if (EditorGUI.EndChangeCheck()) {
          Undo.RecordObject(line, "Move Point");
          EditorUtility.SetDirty(line);
          line.points[i] = handleTransform.InverseTransformPoint(p);
        }
      }

      for (int i = 0; i < line.points.Length - 1; ++i) {
        Vector3 p0 = handleTransform.TransformPoint(line.points[i]);
        Vector3 p1 = handleTransform.TransformPoint(line.points[i + 1]);
        Handles.DrawLine(p0, p1);
      }
    }
  }
}
