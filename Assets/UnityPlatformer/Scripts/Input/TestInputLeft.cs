using System;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// Go left input for automated tests
  /// </summary>
  public class TestInputLeft : MonoBehaviour {
    /// <summary>
    /// Character AIInput
    /// </summary>
    internal AIInput inputMgr;
    /// <summary>
    /// Listen InstancePrefab SendMessage and start logic
    /// </summary>
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
