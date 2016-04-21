using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace UnityPlatformer {
  /// <summary>
  /// Shortcut to create a Trigger2D, configure: BoxCollider2D and Rigidbody2D.
  /// *NOTE* BoxCollider2D and Rigidbody2D will be hidden.
  /// </summary>
  [ExecuteInEditMode]
  public class BoxTrigger2D : Trigger2D {
    public Vector2 size {
      get { return _size; }
      set {
        _size = size;
        box2d.size = size;
      }
    }

    [SerializeField]
    protected Vector2 _size;

    BoxCollider2D box2d;
    Rigidbody2D rb2d;

    #if UNITY_EDITOR
    /// this is necesary to configure objects properly when imported
    // from a prefab and other edge cases.
    void OnValidate() {
      if (box2d) {
        box2d.hideFlags = HideFlags.HideInInspector;
        box2d.size = size;
        box2d.isTrigger = true;
        //box2d.hideFlags = HideFlags.None;
      }

      if (rb2d) {
        rb2d.isKinematic = true;
        rb2d.gravityScale = 0;
        rb2d.hideFlags = HideFlags.HideInInspector;
        //rb2d.hideFlags = HideFlags.None;
      }
    }
    #endif

    void Start() {
      box2d = GetComponent<BoxCollider2D>();
      if (box2d == null) {
        // create
        box2d = gameObject.AddComponent<BoxCollider2D>();
        box2d.isTrigger = true;
        box2d.hideFlags = HideFlags.HideInInspector;
      }

      rb2d = GetComponent<Rigidbody2D>();
      if (rb2d == null) {
        // create
        rb2d = gameObject.AddComponent<Rigidbody2D>();
        rb2d.isKinematic = true;
        rb2d.gravityScale = 0;
        rb2d.hideFlags = HideFlags.HideInInspector;
      }
    }
  }
}
