using UnityEngine;
using UnityEngine.Rendering;

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
    /// callback type
    /// </summary>
    public delegate void OnTrigger(Collider2D o);
    /// <summary>
    /// callback when OnTriggerEnter2D is called with a object that match collisionMask
    /// </summary>
    public OnTrigger onTriggerEnter2D;
    /// <summary>
    /// callback when OnTriggerExit2D is called with a object that match collisionMask
    /// </summary>
    public OnTrigger onTriggerExit2D;
    /// <summary>
    /// callback when OnTriggerStay2D is called with a object that match collisionMask
    /// </summary>
    public OnTrigger onTriggerStay2D;
    /// <summary>
    /// BoxCollider2D size
    /// </summary>
    public Vector2 size {
      get { return _size; }
      set {
        _size = size;
        if (box2d != null) {
          box2d.size = size;
        }
      }
    }
    /// <summary>
    /// Allowed mask, if something in the mask collide, trigger will fire!
    /// </summary>
    public LayerMask collisionMask;
    /// <summary>
    /// BoxCollider2D size
    /// </summary>
    [SerializeField]
    private Vector2 _size;
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
      if (Application.isPlaying) return;
      //Handles.Label(transform.position + new Vector3(-_size.x * 0.5f, _size.y * 0.5f, 0), "Trigger: " + this.name);

      Vector3 pos = transform.position;
      Vector3[] verts = new Vector3[] {
        new Vector3(pos.x - _size.x * 0.5f, pos.y - _size.y * 0.5f, pos.z),
        new Vector3(pos.x - _size.x * 0.5f, pos.y + _size.y * 0.5f, pos.z),
        new Vector3(pos.x + _size.x * 0.5f, pos.y + _size.y * 0.5f, pos.z),
        new Vector3(pos.x + _size.x * 0.5f, pos.y - _size.y * 0.5f, pos.z)
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
      box2d.hideFlags = HideFlags.HideInInspector;
      box2d.size = _size;

      rb2d = gameObject.GetOrAddComponent<Rigidbody2D>();
      rb2d.isKinematic = true;
      rb2d.gravityScale = 0;
      rb2d.hideFlags = HideFlags.HideInInspector;
    }

    void OnTriggerEnter2D(Collider2D o) {
      //Debug.Log("OnTriggerEnter2D " + o.gameObject.name);
      if (onTriggerEnter2D != null && collisionMask.Contains(o.gameObject.layer)) {
        onTriggerEnter2D(o);
      }
    }

    void OnTriggerExit2D(Collider2D o) {
      //Debug.Log("OnTriggerExit2D " + o.gameObject.name);

      if (onTriggerExit2D != null && collisionMask.Contains(o.gameObject.layer)) {
        onTriggerExit2D(o);
      }
    }

    void OnTriggerStay2D(Collider2D o) {
      //Debug.Log("OnTriggerStay2D " + o.gameObject.name);

      if (onTriggerStay2D != null && collisionMask.Contains(o.gameObject.layer)) {
        onTriggerStay2D(o);
      }
    }
  }
}
