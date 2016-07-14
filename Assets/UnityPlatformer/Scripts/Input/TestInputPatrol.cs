using System;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  //[RequireComponent (typeof (Character))]
  public class TestInputPatrol : MonoBehaviour {

    internal AIInput inputMgr;
    internal PlatformerCollider2D pc2d;

    public void OnInstancePrefab(InstancePrefab prefab) {
      inputMgr = prefab.instance.GetComponentInChildren<AIInput>();
      if (inputMgr == null) {
        Debug.LogWarning("AIInput is expected in the prefab");
        return;
      }
      pc2d = prefab.instance.GetComponentInChildren<PlatformerCollider2D>();


      inputMgr.SetX(1);

      pc2d.onLeftWall += OnLeftWall;
      pc2d.onRightWall += OnRightWall;
    }

    void OnLeftWall() {
      inputMgr.SetX(1);
    }

    void OnRightWall() {
      inputMgr.SetX(-1);
    }
  }
}
