using System;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  public class TestInputPatrolLadder : MonoBehaviour {

    internal AIInput inputMgr;
    internal PlatformerCollider2D pc2d;
    internal Character character;

    Facing facing = Facing.Right;


    public void OnInstancePrefab(InstancePrefab prefab) {
      inputMgr = prefab.instance.GetComponentInChildren<AIInput>();
      if (inputMgr == null) {
        Debug.LogWarning("AIInput is expected in the prefab");
        return;
      }
      pc2d = prefab.instance.GetComponentInChildren<PlatformerCollider2D>();
      character = prefab.instance.GetComponentInChildren<Character>();


      inputMgr.SetX(1);
      character.onAreaChange += OnAreaChange;
      pc2d.onLeftWall += OnLeftWall;
      pc2d.onRightWall += OnRightWall;
    }

    void OnAreaChange(Areas before, Areas after) {
      if ((after & Areas.Ladder) == Areas.Ladder) {
        if (character.ladder.IsAboveTop(character, character.feet)) {
          inputMgr.SetY(-1);
        } else {
          inputMgr.SetY(1);
        }

        inputMgr.SetX(0);
        UpdateManager.instance.SetTimeout(ContinueMoving, 2.5f);
      } else {
        inputMgr.SetX((float)facing);
      }
    }

    // this prevent to get stuck in a vine
    void ContinueMoving() {
      inputMgr.SetX((float)facing);
    }

    void OnLeftWall() {
      facing = Facing.Right;
      inputMgr.SetX((float)facing);
    }

    void OnRightWall() {
      facing = Facing.Left;
      inputMgr.SetX((float)facing);
    }
  }
}
