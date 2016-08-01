using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  [RequireComponent (typeof (BoxCollider2D))]
  public abstract class Item : MonoBehaviour {

    public Vector3 offset;
    public Facing facing = Facing.None;
    public string animationName;


    // cache
    BoxCollider2D body;

    public virtual void Start() {
      body = GetComponent<BoxCollider2D>();
    }

    public virtual Vector3 GetCenter() {
      return body.bounds.center;
    }

    public virtual void PositionCharacter(Character p) {
      p.velocity = Vector3.zero;
      if (facing != Facing.None) {
        p.SetFacing(facing);
      }
      Vector3 pos = p.transform.position;
      pos.x = GetCenter().x + offset.x;
      p.transform.position = pos;
    }

    public virtual void Enter(Character p) {
      if (p == null) return;

      p.EnterArea(Areas.Item);
      p.item = this;
    }

    public virtual void Exit(Character p) {
      if (p == null) return;

      p.ExitArea(Areas.Item);
      p.item = null;
    }

    public virtual bool IsUsableBy(Character p) {
      return false;
    }
    public abstract void Use(Character p);

    public virtual void OnTriggerEnter2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        Enter(h.owner.GetComponent<Character>());
      }
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        Exit(h.owner.GetComponent<Character>());
      }
    }
  }
}
