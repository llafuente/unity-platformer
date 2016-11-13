using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// </summary>
  public class IntegrationTestDuration : MonoBehaviour, IUpdateEntity {
    /// <summary>
    /// NOTE do not use the same GameObject that TestComponent
    /// </summary>
    public float testDuration = 300.0f;

    void OnEnable() {
      //Debug.Log("???");
      //Debug.Log(UpdateManager.instance.frameListenersCount);
      UpdateManager.Push(this, Configuration.instance.charactersPriority);
    }
    /// <summary>
    /// Event like FixedUpdate
    /// </summary>
    public void PlatformerUpdate(float delta) {
      if (UpdateManager.instance.runningTime > testDuration) {
        IntegrationTest.Pass(gameObject);
      }
    }
    /// <summary>
    /// Event fired after every entity recieve PlatformerUpdate
    /// </summary>
    public void LatePlatformerUpdate(float delta) {

    }
  }
}
