using System;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// Patrol (left-right) input for automated tests
  /// </summary>
  public class TestInputPatrol : MonoBehaviour {
    /// <summary>
    /// Character AIInput
    /// </summary>
    internal AIInput inputMgr;
    /// <summary>
    /// PlatformerCollider2D to listen collision callbacks
    /// </summary>
    internal PlatformerCollider2D pc2d;
    /// <summary>
    /// Listen InstancePrefab SendMessage and start logic
    /// </summary>
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
    /// <summary>
    /// Character hit a wall, move in the other direction
    /// </summary>
    void OnLeftWall() {
      inputMgr.SetX(1);
    }
    /// <summary>
    /// Character hit a wall, move in the other direction
    /// </summary>
    void OnRightWall() {
      inputMgr.SetX(-1);
    }
  }
}
