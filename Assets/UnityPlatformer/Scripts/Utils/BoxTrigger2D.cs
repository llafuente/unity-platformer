using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityPlatformer {
  /// <summary>
  /// Shortcut to create a Trigger2D, configure: BoxCollider2D and Rigidbody2D.
  /// *NOTE* BoxCollider2D and Rigidbody2D will be hidden.
  /// </summary>
  public class BoxTrigger2D : Trigger2D {
    /// <summary>
    /// BoxCollider2D
    /// </summary>
    internal BoxCollider2D box2d;
    /// <summary>
    /// Rigidbody2D
    /// </summary>
    internal Rigidbody2D rb2d;

    #if UNITY_EDITOR
    /// <summary>
    /// Draw on editor
    /// </summary>
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    void OnDrawGizmos() {
      if (Application.isPlaying || box2d == null) return;
      Vector3 size = box2d.size;
      //Handles.Label(transform.position + new Vector3(-size.x * 0.5f, size.y * 0.5f, 0), "Trigger: " + this.name);

      Vector3 pos = transform.position;
      Vector3[] verts = new Vector3[] {
        new Vector3(pos.x - size.x * 0.5f, pos.y - size.y * 0.5f, pos.z),
        new Vector3(pos.x - size.x * 0.5f, pos.y + size.y * 0.5f, pos.z),
        new Vector3(pos.x + size.x * 0.5f, pos.y + size.y * 0.5f, pos.z),
        new Vector3(pos.x + size.x * 0.5f, pos.y - size.y * 0.5f, pos.z)
      };
      Handles.DrawSolidRectangleWithOutline( verts, new Color( 0, 1, 0, 0.05f ), new Color( 0, 0, 0, 0.5f ) );
    }
    #endif
    /// <summary>
    /// Get/Create BoxCollider2D & Rigidbody2D, then configure
    /// </summary>
    void Start() {
      box2d = gameObject.GetOrAddComponent<BoxCollider2D>();
      box2d.isTrigger = true;

      rb2d = gameObject.GetOrAddComponent<Rigidbody2D>();
      rb2d.isKinematic = false;
      rb2d.gravityScale = 0;
    }
    void Reset() {
      Start();
    }

    void OnValidate() {
      Start();
    }
  }
}
