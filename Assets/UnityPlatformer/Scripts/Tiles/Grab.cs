using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Grab tile
  /// Character grab onto something and stop, like rings grabbing
  /// </summary>
  public class Grab : BoxTileTrigger {
    override public void CharacterEnter(Character p) {
      if (p == null) return;

      base.CharacterEnter(p);
      p.EnterArea(Areas.Grabbable);
      p.grab = this;
    }
    /// <summary>
    /// ExitState Grabbing
    /// </summary>
    virtual public void Dismount(Character p) {
      p.ExitState(States.Grabbing);
    }

    override public void CharacterExit(Character p) {
      if (p == null) return;

      base.CharacterExit(p);
      Dismount(p);
      p.ExitArea(Areas.Grabbable);
      p.grab = null;
    }
  }
}
