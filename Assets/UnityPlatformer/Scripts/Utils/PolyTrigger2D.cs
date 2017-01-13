using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Rendering;

namespace UnityPlatformer {
  /// <summary>
  /// Shortcut to create a Trigger2D, configure: PolygonCollider2D and Rigidbody2D.
  /// *NOTE* Rigidbody2D will be hidden.
  /// </summary>
  [ExecuteInEditMode]
  public class PolyTrigger2D : Trigger2D {
    PolygonCollider2D poly2d;
    Rigidbody2D rb2d;

    #if UNITY_EDITOR
    /// this is necesary to configure objects properly when imported
    // from a prefab and other edge cases.
    void OnValidate() {
      if (poly2d) {
        //poly2d.hideFlags = HideFlags.HideInInspector;
        poly2d.isTrigger = true;
        //poly2d.hideFlags = HideFlags.None;
      }

      if (rb2d) {
        rb2d.isKinematic = true;
        rb2d.gravityScale = 0;
        //rb2d.hideFlags = HideFlags.HideInInspector;
        //rb2d.hideFlags = HideFlags.None;
      }
    }
    #endif

    void Start() {
      poly2d = GetComponent<PolygonCollider2D>();
      if (poly2d == null) {
        // create
        poly2d = gameObject.AddComponent<PolygonCollider2D>();
        poly2d.isTrigger = true;
        //poly2d.hideFlags = HideFlags.HideInInspector;
      }

      rb2d = GetComponent<Rigidbody2D>();
      if (rb2d == null) {
        // create
        rb2d = gameObject.AddComponent<Rigidbody2D>();
        rb2d.isKinematic = true;
        rb2d.gravityScale = 0;
        //rb2d.hideFlags = HideFlags.HideInInspector;
      }
    }
  }
}
