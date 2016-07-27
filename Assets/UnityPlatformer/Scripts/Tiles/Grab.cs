using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  public class Grab : TileTrigger {
    virtual public Vector3 GetTop() {
      return body.bounds.center + new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }

    virtual public Vector3 GetBottom() {
      return body.bounds.center - new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }

    virtual public Vector3 GetCenter() {
      return body.bounds.center;
    }

    override public void CharacterEnter(Character p) {
      if (p == null) return;

      p.EnterArea(Areas.Grabbable);
      p.grab = this;
    }

    virtual public void Dismount(Character p) {
      p.ExitState(States.Grabbing);
    }

    override public void CharacterExit(Character p) {
      if (p == null) return;

      Dismount(p);
      p.ExitArea(Areas.Grabbable);
      p.grab = null;
    }
  }
}
