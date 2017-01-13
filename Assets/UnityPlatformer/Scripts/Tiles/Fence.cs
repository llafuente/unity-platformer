using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityPlatformer {
  /// <summary>
  /// Fence tile
  /// </summary>
  [RequireComponent (typeof (PolygonCollider2D))]
  public class Fence : MonoBehaviour {
    /// <summary>
    /// PolygonCollider2D
    /// </summary>
    internal PolygonCollider2D body;

    /// <summary>
    /// Force PolygonCollider2D to be a trigger
    /// </summary>
    virtual public void Start() {
      body = GetComponent<PolygonCollider2D>();
      body.isTrigger = true;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Set layer to Configuration.ropesMask
    /// </summary>
    void Reset() {
      gameObject.layer = Configuration.instance.fencesMask;
    }
#endif

    /// <summary>
    /// notify character that is in a Fence
    /// </summary>
    virtual public void EnableFence(Character p) {
      p.fence = this;
      p.EnterArea(Areas.Fence);
    }

    /// <summary>
    /// notify character that must exit Fence state
    /// </summary>
    virtual public void Dismount(Character p) {
      p.ExitState(States.Fence);
    }

    /// <summary>
    /// notify character that is out a Fence
    /// </summary>
    virtual public void DisableFence(Character p) {
      Dismount(p);
      p.ExitArea(Areas.Fence);
      p.fence = null;
    }

    /// <summary>
    /// Check user Bound (EnterAreas) is completely inside the Fence Area
    /// if it's true -> EnableFence
    /// if it's false -> DisableFence
    /// </summary>
    public virtual void OnTriggerStay2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();

      Debug.LogFormat("fence hit {0}", h);

      if (h && h.type == HitBoxType.EnterAreas) {
        Bounds b = h.GetComponent<BoxCollider2D>().bounds;
        Vector2 pmin = b.min;
        Vector2 pmax = b.max;

        // check if the body is completely inside
        if (
          body.Contains(pmin) &&
          body.Contains(pmax) &&
          body.Contains(new Vector2(pmin.x, pmax.y)) &&
          body.Contains(new Vector2(pmax.x, pmin.y))
        ) {
          EnableFence(h.owner.GetComponent<Character>());
        } else {
          DisableFence(h.owner.GetComponent<Character>());
        }
      }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draw in Editor mode
    /// </summary>
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    void OnDrawGizmos() {
      if (Application.isPlaying) return;

      body = GetComponent<PolygonCollider2D>();
      Vector3 size = body.bounds.size;
      Vector3 pos = transform.position;

      Handles.Label(pos + new Vector3(-size.x * 0.5f, size.y * 0.5f, 0), "Fence");

      Handles.color = new Color(1, 1, 0, 0.05f);
      Handles.DrawAAConvexPolygon(body.GetWorldPoints3());
    }
#endif


  }
}
