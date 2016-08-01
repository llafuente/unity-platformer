using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  public class Track : TileTrigger {
    public Vector3 velocity;

    override public void CharacterEnter(Character p) {
      // only the first one enable the rope
      if (p.track == null) {
        p.EnterArea(Areas.Track);
        p.track = this;
        p.worldVelocity += velocity;
      }
    }

    override public void CharacterExit(Character p) {
      // same as above, only diable if we leave the section we are grabbing
      if (p.track == this) {
        p.ExitArea(Areas.Track);
        p.track = null;
        p.worldVelocity -= velocity;
      }
    }
  }
}
