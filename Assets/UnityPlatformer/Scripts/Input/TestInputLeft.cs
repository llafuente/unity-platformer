using System;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  //[RequireComponent (typeof (Character))]
  public class TestInputLeft : MonoBehaviour {

    internal AIInput inputMgr;

    public void OnInstancePrefab(InstancePrefab prefab) {
      inputMgr = prefab.instance.GetComponentInChildren<AIInput>();
      if (inputMgr == null) {
        Debug.LogWarning("AIInput is expected in the prefab");
        return;
      }
      inputMgr.SetX(-1);
    }
  }
}
