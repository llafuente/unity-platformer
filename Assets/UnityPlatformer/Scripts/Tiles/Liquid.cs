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
  public class Liquid : Physhic2DMonoBehaviour {
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
    /// <summary>
    /// How much buoyancy apply when close to surface
    /// </summary>
    public float buoyancySurfaceFactor = 0.5f;
    /// <summary>
    /// BoxCollider2D
    /// </summary>
    internal BoxCollider2D body;
    /// <summary>
    /// Get BoxCollider2D
    /// </summary>
    override public void Start() {
      base.Start();
      body = GetComponent<BoxCollider2D>();
    }
#if UNITY_EDITOR
    /// <summary>
    /// Set layer to Configuration.ropesMask
    /// </summary>
    void Reset() {
      gameObject.layer = Configuration.instance.liquidsMask;
    }
#endif
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
