using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

/// <summary>
/// Shortcut to create a trigger (configure: BoxCollider2D and Rigidbody2D)
/// </summary>
[ExecuteInEditMode]
public class BoxTrigger2D : MonoBehaviour
{
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
  void OnValidate() {
    if (box2d) {
      box2d.hideFlags = HideFlags.HideInInspector;
      //box2d.hideFlags = HideFlags.None;
      box2d.size = size;
      box2d.isTrigger = true;
    }
    if (rb2d) {
      rb2d.isKinematic = true;
      rb2d.gravityScale = 0;
      rb2d.hideFlags = HideFlags.HideInInspector;
      //rb2d.hideFlags = HideFlags.None;
    }
  }
  #endif

  // clang-format on
  void Start() {
    box2d = GetComponent<BoxCollider2D>();
    if (box2d == null) {
      box2d = gameObject.AddComponent<BoxCollider2D>();
      box2d.isTrigger = true;
      box2d.hideFlags = HideFlags.HideInInspector;
    }

    rb2d = GetComponent<Rigidbody2D>();
    if (rb2d == null) {
      rb2d = gameObject.AddComponent<Rigidbody2D>();
      rb2d.isKinematic = true;
      rb2d.gravityScale = 0;
      rb2d.hideFlags = HideFlags.HideInInspector;
    }
  }
}
