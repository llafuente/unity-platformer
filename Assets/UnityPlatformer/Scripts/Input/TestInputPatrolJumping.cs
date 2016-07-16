using System;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  //[RequireComponent (typeof (Character))]
  public class TestInputPatrolJumping : MonoBehaviour {

    internal AIInput inputMgr;
    internal PlatformerCollider2D pc2d;
    internal Character character;

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

    void OnAfterMove(Character character, float delta) {
      inputMgr.EnableAction("Jump");
    }

    void OnLeftWall() {
      inputMgr.SetX(1);
    }

    void OnRightWall() {
      inputMgr.SetX(-1);
    }
  }
}
