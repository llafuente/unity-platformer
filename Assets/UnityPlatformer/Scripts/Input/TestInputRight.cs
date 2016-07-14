using System;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  //[RequireComponent (typeof (Character))]
  public class TestInputRight : MonoBehaviour {

    internal AIInput inputMgr;

    public void OnInstancePrefab(InstancePrefab prefab) {
      inputMgr = prefab.instance.GetComponentInChildren<AIInput>();
      inputMgr.SetX(1);
    }
  }
}
