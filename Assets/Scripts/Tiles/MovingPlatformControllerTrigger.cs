using UnityEngine;
using System.Collections.Generic;
using UnityPlatformer;

// clang-format off
namespace UnityPlatformer.Tiles {
  [RequireComponent(typeof(BoxTrigger2D))]
  public class MovingPlatformControllerTrigger : MonoBehaviour {
    #region public

    public enum Actions {
      Nothing,
      Resume,
      ResumeAndStop,
      ReverseAndResume,
      ReverseAndResumeAndStop,
      Stop,
      StopOnNext,
      IfStoppedReverse
    };

    [Comment("Who can trigger me?")]
    public LayerMask mask;


    public List<MovingPlatform> targets;
    // enum On/Off/Nothing
    public Actions onEnter = Actions.Nothing;
    public Actions onExit = Actions.Nothing;
    public Actions onStay = Actions.Nothing;

    #endregion

    void DoAction(Actions action) {
      if (action == Actions.Nothing) {
        return;
      }

      Debug.Log("MovingPlatformControllerTrigger.DoAction: " + action);

      foreach (MovingPlatform target in targets) {
        Debug.Log("Do: " + action + " ON " + target.name);

        switch (action) {
        case Actions.Resume:
          target.Resume();
          break;
        case Actions.ResumeAndStop:
          target.Resume();
          target.StopOn(1);
          break;
        case Actions.ReverseAndResume:
          target.Reverse();
          target.Resume();
          break;
        case Actions.ReverseAndResumeAndStop:
          target.Reverse();
          target.Resume();
          target.StopOn(1);
          break;
        case Actions.Stop:
          target.Stop();
          break;
        case Actions.StopOnNext:
          target.StopOn(1);
          break;
        case Actions.IfStoppedReverse:
          if (target.IsStopped()) {
            target.Resume();
            target.StopOn(2);
          }
          break;
        }
      }
    }

    void OnTriggerEnter2D(Collider2D o) {
      //Debug.Log("MovingPlatformControllerTrigger.OnTriggerEnter2D: " + name);
      if (Utils.layermask_contains(mask, o.gameObject.layer)) {
        DoAction(onEnter);
      }
    }

    void OnTriggerExit2D(Collider2D o) {
      //Debug.Log("MovingPlatformControllerTrigger.OnTriggerExit2D: " + name);
      if (Utils.layermask_contains(mask, o.gameObject.layer)) {
        DoAction(onExit);
      }
    }

    void OnTriggerStay2D(Collider2D o) {
      // Debug.Log("MovingPlatformControllerTrigger.OnTriggerStay2D: " + name);
      if (Utils.layermask_contains(mask, o.gameObject.layer)) {
        DoAction(onStay);
      }
    }
  }
}
