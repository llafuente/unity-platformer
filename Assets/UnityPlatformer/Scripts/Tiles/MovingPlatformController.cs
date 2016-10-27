using UnityEngine;
using System.Collections.Generic;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// Perform actions over a MovingPlatform when something enter/stay/leave
  ///
  /// NOTE Stay will be executed many times, it's not recommended to use it...
  /// </summary>
  [RequireComponent(typeof(Trigger2D))]
  public class MovingPlatformController : MonoBehaviour, ITriggerAble {
    /// <summary>
    /// mask to trigger actions
    /// </summary>
    [Comment("Who can trigger me?")]
    public LayerMask mask;
    /// <summary>
    /// target Moving Platforms
    /// </summary>
    public List<MovingPlatform> targets;
    /// <summary>
    /// Action to perform when enter
    /// </summary>
    public MovingPlatformActions onEnter = MovingPlatformActions.Nothing;
    /// <summary>
    /// Action to perform when leave
    /// </summary>
    public MovingPlatformActions onExit = MovingPlatformActions.Nothing;
    /// <summary>
    /// Action to perform while stay
    /// </summary>
    public MovingPlatformActions onStay = MovingPlatformActions.Nothing;

    void DoAction(MovingPlatformActions action) {
      if (action == MovingPlatformActions.Nothing) {
        return;
      }

      //Debug.Log("MovingPlatformControllerTrigger.DoAction: " + action);

      foreach (MovingPlatform target in targets) {
        //Debug.Log("Do: " + action + " ON " + target.name);
        target.DoAction(action);
      }
    }

    public void OnTriggerEnter2D(Collider2D o) {
      //Debug.Log("MovingPlatformControllerTrigger.OnTriggerEnter2D: " + name);
      if (mask.Contains(o.gameObject.layer)) {
        DoAction(onEnter);
      }
    }

    public void OnTriggerExit2D(Collider2D o) {
      //Debug.Log("MovingPlatformControllerTrigger.OnTriggerExit2D: " + name);
      if (mask.Contains(o.gameObject.layer)) {
        DoAction(onExit);
      }
    }

    public void OnTriggerStay2D(Collider2D o) {
      // Debug.Log("MovingPlatformControllerTrigger.OnTriggerStay2D: " + name);
      if (mask.Contains(o.gameObject.layer)) {
        DoAction(onStay);
      }
    }
  }
}
