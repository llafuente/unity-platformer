using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  [AddComponentMenu("")] // hide, this is just internal
  public class RopeSection : MonoBehaviour {

    public int index;
    public Rope rope;

    // cache
    BoxCollider2D body;

    virtual public void Start() {
      body = GetComponent<BoxCollider2D>();
    }

    virtual public Vector3 GetTop() {
      return transform.TransformPoint((Vector3)body.offset + new Vector3(0, body.size.y * 0.5f, 0));
    }

    virtual public Vector3 GetBottom() {
      return transform.TransformPoint((Vector3)body.offset - new Vector3(0, body.size.y * 0.5f, 0));
    }

    virtual public Vector3 GetCenter() {
      return transform.TransformPoint(body.offset);
    }

    virtual public Vector3 GetPositionInSection(float position) {
      Vector3 b = GetBottom();
      return b + (GetTop() - b) * position;
    }

    virtual public void EnableRope(Character p) {
      // only the first one enable the rope
      if (p.rope == null) {
        p.EnterArea(Areas.Rope);
        p.rope = rope;
        p.ropeIndex = index;
      }
    }

    virtual public void Dismount(Character p) {
      p.ExitState(States.Rope);
    }

    virtual public void DisableRope(Character p) {
      // same as above, only diable if we leave the section we are grabbing
      if (p.ropeIndex == index) {
        Dismount(p);
        p.ExitArea(Areas.Rope);
        p.rope = null;
        p.ropeIndex = -1;
      }
    }

    public virtual void OnTriggerEnter2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        EnableRope(h.owner.character);
      }
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        DisableRope(h.owner.character);
      }
    }

    // debug
    //void OnDrawGizmos() {
    //  Utils.DrawPoint(GetCenter(), 0.2f);
    //  Utils.DrawPoint(GetTop(), 0.2f);
    //  Utils.DrawPoint(GetBottom(), 0.2f);
    //}
  }
}
