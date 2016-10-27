using UnityEngine;
using System.Collections;

// TODO use: BoxTileTrigger

namespace UnityPlatformer {
  /// <summary>
  /// Abstract class to create a Usable Item
  /// </summary>
  [RequireComponent (typeof (BoxCollider2D))]
  public abstract class Item : MonoBehaviour {
    /// <summary>
    /// Character offset when playing animation
    /// </summary>
    public Vector3 offset;
    /// <summary>
    /// Character facing when playing animation
    /// </summary>
    public Facing facing = Facing.None;
    /// <summary>
    /// animation name to play when using
    /// </summary>
    public string animationName;
    /// <summary>
    /// BoxCollider2D
    /// </summary>
    internal BoxCollider2D body;
    /// <summary>
    /// Get BoxCollider2D
    /// </summary>
    public virtual void Start() {
      body = GetComponent<BoxCollider2D>();
    }
    /// <summary>
    /// Get real-world-coordinates center
    /// </summary>
    public virtual Vector3 GetCenter() {
      return body.bounds.center;
    }
    /// <summary>
    /// Set the position/facing of the player to the desired values
    /// </summary>
    public virtual void PositionCharacter(Character p) {
      p.velocity = Vector3.zero;
      if (facing != Facing.None) {
        p.SetFacing(facing);
      }
      Vector3 pos = p.transform.position;
      pos.x = GetCenter().x + offset.x;
      p.transform.position = pos;
    }
    /// <summary>
    /// notify Character is in area item
    /// </summary>
    public virtual void Enter(Character p) {
      if (p == null) return;

      p.EnterArea(Areas.Item);
      p.item = this;
    }
    /// <summary>
    /// notify Character is out area item
    /// </summary>
    public virtual void Exit(Character p) {
      if (p == null) return;

      p.ExitArea(Areas.Item);
      p.item = null;
    }
    /// <summary>
    /// everybody can use this item
    /// otherwise, override
    /// </summary>
    public virtual bool IsUsableBy(Character p) {
      return false;
    }
    /// <summary>
    /// Implement this function with the action that need to performed
    /// </summary>
    public abstract void Use(Character p);
    /// <summary>
    /// When HitBox(EnterAreas) enter -> Enter
    /// </summary>
    public virtual void OnTriggerEnter2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        Enter(h.owner.GetComponent<Character>());
      }
    }
    /// <summary>
    /// When HitBox(EnterAreas) leave -> Exit
    /// </summary>
    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        Exit(h.owner.GetComponent<Character>());
      }
    }
  }
}
