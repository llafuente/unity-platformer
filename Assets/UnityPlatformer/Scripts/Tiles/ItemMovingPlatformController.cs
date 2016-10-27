using UnityEngine;
using System.Collections.Generic;

namespace UnityPlatformer {
  /// <summary>
  /// Item to control Moving Platforms
  /// </summary>
  [RequireComponent (typeof (BoxCollider2D))]
  public class ItemMovingPlatformController : Item {
    /// <summary>
    /// Action that Moving Platform will do
    /// </summary>
    public MovingPlatformActions onUse = MovingPlatformActions.Nothing;
    /// <summary>
    /// Moving Platforms that will be affected
    /// </summary>
    public List<MovingPlatform> targets;

    /// <summary>
    /// Can be used by any character
    /// </summary>
    public override bool IsUsableBy(Character p) {
      return true;
    }

    /// <summary>
    /// When use operate over given targets
    /// </summary>
    public override void Use(Character p) {
      PositionCharacter(p);
      p.SetOverrideAnimation(animationName, true);
      foreach (MovingPlatform target in targets) {
        target.DoAction(onUse);
      }
    }
  }
}
