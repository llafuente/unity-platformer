using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Internal Behaviour to create ropes.
  /// </summary>
  [AddComponentMenu("")] // hide, this is just internal
  public class RopeSection : BoxTileTrigger {
    /// <summary>
    /// Index in the rope
    /// </summary>
    public int index;
    /// <summary>
    /// Rope where this belong
    /// </summary>
    public Rope rope;
    /// <summary>
    /// Get real-world-coordinates top
    /// </summary>
    virtual public Vector3 GetTop() {
      return transform.TransformPoint((Vector3)body.offset + new Vector3(0, body.size.y * 0.5f, 0));
    }
    /// <summary>
    /// Get real-world-coordinates bottom
    /// </summary>
    virtual public Vector3 GetBottom() {
      return transform.TransformPoint((Vector3)body.offset - new Vector3(0, body.size.y * 0.5f, 0));
    }
    /// <summary>
    /// Get real-world-coordinates center
    /// </summary>
    override public Vector3 GetCenter() {
      return transform.TransformPoint(body.offset);
    }
    /// <summary>
    /// Get real-world-coordinates position given the position in the section
    /// from 0 to 1
    /// </summary>
    virtual public Vector3 GetPositionInSection(float position) {
      Vector3 b = GetBottom();
      return b + (GetTop() - b) * position;
    }

    override public void CharacterEnter(Character p) {
      // only the first one enable the rope
      if (p.rope == null) {
        base.CharacterEnter(p);

        p.EnterArea(Areas.Rope);
        p.rope = rope;
        p.ropeIndex = index;
      }
    }
    /// <summary>
    /// Dismount rope
    /// </summary>
    virtual public void Dismount(Character p) {
      p.ExitState(States.Rope);
    }

    override public void CharacterExit(Character p) {
      // same as above, only diable if we leave the section we are grabbing
      if (p.ropeIndex == index) {
        base.CharacterExit(p);

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
