using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  [RequireComponent (typeof (BoxCollider2D))]
  public class Grab : MonoBehaviour {
    // cache
    BoxCollider2D body;

    virtual public void Start() {
      body = GetComponent<BoxCollider2D>();
    }

    virtual public Vector3 GetTop() {
      return body.bounds.center + new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }

    virtual public Vector3 GetBottom() {
      return body.bounds.center - new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }

    virtual public Vector3 GetCenter() {
      return body.bounds.center;
    }

    virtual public void EnableGrab(Character p) {
      if (p == null) return;

      p.EnterArea(Areas.Grabbable);
      p.grab = this;
    }

    virtual public void Dismount(Character p) {
      p.ExitState(States.Grabbing);
    }

    virtual public void DisableGrab(Character p) {
      if (p == null) return;

      Dismount(p);
      p.ExitArea(Areas.Grabbable);
      p.grab = null;
    }

    public virtual void OnTriggerEnter2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        EnableGrab(h.owner.GetComponent<Character>());
      }
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        DisableGrab(h.owner.GetComponent<Character>());
      }
    }
  }
}
