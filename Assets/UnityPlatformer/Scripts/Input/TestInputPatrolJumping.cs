using System;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// Unit test input to move a character
  /// * Move right until wall, then move left
  /// * always jumping
  /// </summary>
  public class TestInputPatrolJumping : MonoBehaviour {
    /// <summary>
    /// Character AIInput
    /// </summary>
    internal AIInput inputMgr;
    /// <summary>
    /// PlatformerCollider2D to listen collision callbacks
    /// </summary>
    internal PlatformerCollider2D pc2d;
    /// <summary>
    /// Character
    /// </summary>
    internal Character character;
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
      character = prefab.instance.GetComponentInChildren<Character>();


      inputMgr.SetX(1);
      character.onAfterMove += OnAfterMove;
      pc2d.onLeftWall += OnLeftWall;
      pc2d.onRightWall += OnRightWall;
    }
    /// <summary>
    /// Continuosly enable jump
    /// </summary>
    void OnAfterMove(Character character, float delta) {
      inputMgr.EnableAction("Jump");
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
