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
      p.EnterArea(Areas.Grabbable);
      p.grab = this;
    }

    virtual public void Dismount(Character p) {
      p.ExitState(States.Grabbing);
    }

    virtual public void DisableGrab(Character p) {
      Dismount(p);
      p.ExitArea(Areas.Grabbable);
      p.grab = null;
    }

    public virtual void OnTriggerEnter2D(Collider2D o) {
      Hitbox h = o.GetComponent<Hitbox>();
      if (h) {
        EnableGrab (h.owner.character);
      } else {
        Character p = o.GetComponent<Character>();
        if (p) {
          EnableGrab (p);
        }
      }
    }

    public virtual void OnTriggerStay2D(Collider2D o) {
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      Hitbox h = o.GetComponent<Hitbox>();
      if (h) {
        DisableGrab (h.owner.character);
      } else {
        Character p = o.GetComponent<Character>();
        if (p) {
          DisableGrab (p);
        }
      }
    }
  }
}
