using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  [RequireComponent (typeof (BoxTrigger2D))]
  public abstract class Item : MonoBehaviour {
    // cache
    // BoxCollider2D body;

    public virtual void Start() {
      //body = GetComponent<BoxCollider2D>();
    }

    public virtual void Enter(Character p) {
      p.EnterArea(Areas.Item);
      p.item = this;
    }

    public virtual void Exit(Character p) {
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
        Enter(h.owner.character);
      }
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        Exit(h.owner.character);
      }
    }
  }
}
