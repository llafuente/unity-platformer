using UnityEngine;
using System.Collections;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Tiles {
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

    virtual public void EnableLadder(Character p) {
      p.EnterArea(Character.Areas.Ladder);
      p.ladder = this;
    }

    virtual public void Dismount(Character p) {
      p.ExitState(Character.States.Ladder);
    }

    virtual public void DisableLadder(Character p) {
      Dismount(p);
      p.ExitArea(Character.Areas.Ladder);
      p.ladder = null;
    }

    public virtual void OnTriggerEnter2D(Collider2D o) {
      Character p = o.GetComponent<Character>();
      if (p) {
        EnableLadder (p);
      }
    }

    public virtual void OnTriggerStay2D(Collider2D o) {
      Character p = o.GetComponent<Character>();
      if (p) {
        //EnableLadder (p);
      }
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      Character p = o.GetComponent<Character>();
      if (p) {
        //DisableLadder (p);
      }
    }
  }
}
