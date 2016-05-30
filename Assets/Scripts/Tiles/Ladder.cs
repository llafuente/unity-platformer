using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  [RequireComponent (typeof (BoxCollider2D))]
  public class Ladder : MonoBehaviour {
    // cache
    public BoxCollider2D body;

    virtual public void Start() {
      body = GetComponent<BoxCollider2D>();
    }

    virtual public Vector3 GetTop() {
      return body.bounds.center + new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }

    virtual public Vector3 GetBottom() {
      return body.bounds.center - new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }

    virtual public bool IsAboveTop(Character c) {
      float feetY = c.GetFeetPosition().y;
      float topY = GetTop().y - c.pc2d.skinWidth;

      return feetY > topY;
    }

    virtual public bool IsAtTop(Character c) {
      float feetY = c.GetFeetPosition().y;
      float topY = GetTop().y;

      return Mathf.Abs(feetY - topY) < c.pc2d.skinWidth;
    }

    virtual public bool IsBelowBottom(Character c) {
      float feetY = c.GetFeetPosition().y;
      float bottomY = GetBottom().y + c.pc2d.skinWidth;

      return feetY < bottomY;
    }

    virtual public bool IsAtBottom(Character c) {
      float feetY = c.GetFeetPosition().y;
      float bottomY = GetBottom().y;

      return Mathf.Abs(feetY - bottomY) < c.pc2d.skinWidth;
    }

    virtual public void EnableLadder(Character p) {
      p.EnterArea(Areas.Ladder);
      p.ladder = this;
    }

    virtual public void Dismount(Character p) {
      p.ExitState(States.Ladder);
    }

    virtual public void DisableLadder(Character p) {
      Dismount(p);
      p.ExitArea(Areas.Ladder);
      p.ladder = null;
    }

    public virtual void OnTriggerEnter2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        EnableLadder(h.owner.character);
      }
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        DisableLadder(h.owner.character);
      }
    }
  }
}
