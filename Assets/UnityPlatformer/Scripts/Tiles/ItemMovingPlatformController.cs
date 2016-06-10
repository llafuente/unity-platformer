using UnityEngine;
using System.Collections.Generic;

namespace UnityPlatformer {
  [RequireComponent (typeof (BoxTrigger2D))]
  public class ItemMovingPlatformController : Item {
    public MovingPlatformActions onUse = MovingPlatformActions.Nothing;
    public List<MovingPlatform> targets;

    public override bool IsUsableBy(Character p) {
      return true;
    }

    public override void Use(Character p) {
      PositionCharacter(p);
      p.SetOverrideAnimation(animationName, true);
      foreach (MovingPlatform target in targets) {
        target.DoAction(onUse);
      }
    }
  }
}
