using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityPlatformer {

  [RequireComponent (typeof (Collider2D))]
  public class Physhic2DMonoBehaviour : MonoBehaviour {
    /// <summary>
    /// Display collider as flat color
    /// </summary>
    public bool debug = false;

    public virtual void Start() {
      if (debug) {
        Utils.SetDebugCollider2D(gameObject, "Green");
      }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draw in the Editor mode
    /// </summary>
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    void OnDrawGizmos() {
      //begin 2D GUI block
      Handles.BeginGUI();
      //translate waypoint vector3 position in world space into a position on the screen
      var guiPoint = HandleUtility.WorldToGUIPoint(transform.position);
      //create rectangle with that positions and do some offset
      var rect = new Rect(guiPoint.x - 50.0f, guiPoint.y - 40, 100, 20);
      //draw box at position with current waypoint name
      GUI.Box(rect, gameObject.name);
      Handles.EndGUI(); //end GUI block

      Utils.DrawCollider2D(gameObject);
    }
#endif

  }
}
