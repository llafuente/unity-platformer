using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  [AddComponentMenu("")] // hide, this is just internal
  public class RopeSection : TileTrigger {
    public int index;
    public Rope rope;

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

    override public void CharacterEnter(Character p) {
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

    override public void CharacterExit(Character p) {
      // same as above, only diable if we leave the section we are grabbing
      if (p.ropeIndex == index) {
        Dismount(p);
        p.ExitArea(Areas.Rope);
        p.rope = null;
        p.ropeIndex = -1;
      }
    }

    // debug
    //void OnDrawGizmos() {
    //  GetCenter().Draw(0.2f);
    //  GetTop().Draw(0.2f);
    //  GetBottom().Draw(0.2f);
    //}
  }
}
