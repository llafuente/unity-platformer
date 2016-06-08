using UnityEngine;
using System.Collections.Generic;
using UnityPlatformer;

// clang-format off
namespace UnityPlatformer {
  /// <summary>
  /// Perform actions over a MovingPlatform.
  /// </summary>
  [RequireComponent(typeof(Trigger2D))]
  public class MovingPlatformController : MonoBehaviour, ITriggerAble {
    #region public


    [Comment("Who can trigger me?")]
    public LayerMask mask;

    public List<MovingPlatform> targets;
    // enum On/Off/Nothing
    public MovingPlatformActions onEnter = MovingPlatformActions.Nothing;
    public MovingPlatformActions onExit = MovingPlatformActions.Nothing;
    public MovingPlatformActions onStay = MovingPlatformActions.Nothing;

    #endregion

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
      if (Utils.layermask_contains(mask, o.gameObject.layer)) {
        DoAction(onEnter);
      }
    }

    public void OnTriggerExit2D(Collider2D o) {
      //Debug.Log("MovingPlatformControllerTrigger.OnTriggerExit2D: " + name);
      if (Utils.layermask_contains(mask, o.gameObject.layer)) {
        DoAction(onExit);
      }
    }

    public void OnTriggerStay2D(Collider2D o) {
      // Debug.Log("MovingPlatformControllerTrigger.OnTriggerStay2D: " + name);
      if (Utils.layermask_contains(mask, o.gameObject.layer)) {
        DoAction(onStay);
      }
    }
  }
}
