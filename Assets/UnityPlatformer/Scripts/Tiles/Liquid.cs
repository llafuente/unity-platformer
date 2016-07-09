using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  [RequireComponent (typeof (BoxCollider2D))]
  public class Liquid : MonoBehaviour {
    /// <summary>
    /// Viscosity affect Character Liquid movement
    /// </summary>
    public float viscosity = 1;
    /// <summary>
    /// Velocity applied to the Character
    /// NOTE need to oppose gravity, so greater in other direction
    /// </summary>
    public Vector2 buoyancy = Vector2.zero;
    public float buoyancySurfaceFactor = 0.5f;
    /// <summary>
    /// </summary>
    public float surfaceOffset = 0;

    // cache
    internal BoxCollider2D body;

    virtual public void Start() {
      body = GetComponent<BoxCollider2D>();
    }

    virtual public Vector3 GetTop() {
      return body.bounds.center + new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }

    public bool IsBelowSurface(Character character, float offset) {
      float char_surface_level = character.feet.y + offset + surfaceOffset;
      return char_surface_level < GetTop().y;
    }

    public float DistanceToSurface(Character character, float offset) {
      float char_surface_level = character.feet.y + offset + surfaceOffset;
      return GetTop().y - char_surface_level;
    }

    public virtual void OnTriggerEnter2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        Character p = h.owner.character;
        p.liquid = this;
        p.EnterArea(Areas.Liquid);
      }
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        Character p = h.owner.character;
        if (p.liquid == this) { // REVIEW with this liquid should overlap
          p.liquid = null;
          p.ExitArea(Areas.Liquid);
        }
      }
    }
  }
}
