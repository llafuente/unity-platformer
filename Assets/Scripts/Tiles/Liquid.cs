using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  [RequireComponent (typeof (BoxTrigger2D))]
  public class Liquid : MonoBehaviour {
    /// <summary>
    /// Viscosity affect Character Liquid movement
    /// </summary>
    public float viscosity = 1;
    /// <summary>
    /// Velocity applied to the Character
    /// NOTE need to oppose gravity, so greater in other direction
    /// </summary>
    public Vector2 boyancy = Vector2.zero;
    public float boyancySurfaceFactor = 0.5f;
    /// <summary>
    /// </summary>
    public float surfaceOffset = 0;

    // cache
    BoxCollider2D body;

    virtual public void Start() {
      body = GetComponent<BoxCollider2D>();
    }

    virtual public Vector3 GetTop() {
      return body.bounds.center + new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }

    public bool IsBelowSurface(Character character, float offset) {
      float char_surface_level = character.GetFeetPosition().y + offset + surfaceOffset;
      return char_surface_level < GetTop().y;
    }

    public float DistanceToSurface(Character character, float offset) {
      float char_surface_level = character.GetFeetPosition().y + offset + surfaceOffset;
      return GetTop().y - char_surface_level;
    }


    public virtual void OnTriggerEnter2D(Collider2D o) {
      Character p = o.GetComponent<Character>();
      if (p) {
        p.liquid = this;
      }
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      Character p = o.GetComponent<Character>();
      if (p && p.liquid == this) {
        p.liquid = null;
      }
    }
  }
}
