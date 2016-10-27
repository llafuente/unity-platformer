using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityPlatformer {
  /// <summary>
  /// Liquid tile.
  /// </summary>
  [RequireComponent (typeof (BoxCollider2D))]
  public class Liquid : MonoBehaviour {
    /// <summary>
    /// Viscosity affect Character Liquid movement
    /// </summary>
    public float viscosity = 1;
    /// <summary>
    /// Surface offset, used to adjust how much the character can submerge.
    /// </summary>
    public float surfaceOffset = 0;
    /// <summary>
    /// Velocity applied to the Character
    /// NOTE need to oppose gravity, so greater in other direction
    /// </summary>
    public Vector2 buoyancy = Vector2.zero;

    public float buoyancySurfaceFactor = 0.5f;
    /// <summary>
    /// BoxCollider2D
    /// </summary>
    internal BoxCollider2D body;
    /// <summary>
    /// Get BoxCollider2D
    /// </summary>
    virtual public void Start() {
      body = GetComponent<BoxCollider2D>();
    }
    /// <summary>
    /// Get real-world-coordinates water top
    /// </summary>
    virtual public Vector3 GetTop() {
      return body.bounds.center + new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }
    /// <summary>
    /// Return if given character(feet) + offset is below liquid surface
    /// </summary>
    public bool IsBelowSurface(Character character, float offset) {
      float char_surface_level = character.feet.y + offset + surfaceOffset;
      return char_surface_level < GetTop().y;
    }
    /// <summary>
    /// Return if given character is complety submerged
    /// </summary>
    public bool IsSubmerged(Character character) {
      return IsBelowSurface(character, character.height);
    }
    /// <summary>
    /// Get the distance between liquid surface and character(feet) + offset
    /// </summary>
    public float DistanceToSurface(Character character, float offset) {
      float char_surface_level = character.feet.y + offset + surfaceOffset;
      return GetTop().y - char_surface_level;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draw in Editor mode
    /// </summary>
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    void OnDrawGizmos() {
      if (Application.isPlaying) return;
      if (body) {
        body.bounds.Draw(transform);
      }
    }
#endif
    /// <summary>
    /// if a Hitbox(EnterAreas) enter -> enterArea
    /// </summary>
    public virtual void OnTriggerEnter2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        Character p = h.owner.GetComponent<Character>();
        p.liquid = this;
        p.EnterArea(Areas.Liquid);
      }
    }
    /// <summary>
    /// if a Hitbox(EnterAreas) enter -> exitArea
    /// </summary>
    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        Character p = h.owner.GetComponent<Character>();
        if (p.liquid == this) { // REVIEW with this liquid should overlap
          p.liquid = null;
          p.ExitArea(Areas.Liquid);
        }
      }
    }
  }
}
