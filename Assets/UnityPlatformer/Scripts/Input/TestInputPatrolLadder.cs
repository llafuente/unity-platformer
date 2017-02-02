using System;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// Input for automated tests
  /// * Move right until wall, then move left
  /// * If found a ladder climb
  /// </summary>
  public class TestInputPatrolLadder : MonoBehaviour {
    /// <summary>
    /// Character AIInput
    /// </summary>
    internal AIInput inputMgr;
    /// <summary>
    /// Character
    /// </summary>
    internal Character character;
    /// <summary>
    /// where is facing
    /// </summary>
    Facing facing = Facing.Right;
    /// <summary>
    /// Listen InstancePrefab SendMessage and start logic
    /// </summary>
    public void OnInstancePrefab(InstancePrefab prefab) {
      inputMgr = prefab.instance.GetComponentInChildren<AIInput>();
      if (inputMgr == null) {
        Debug.LogWarning("AIInput is expected in the prefab");
        return;
      }

      character = prefab.instance.GetComponentInChildren<Character>();

      inputMgr.SetX(1);
      character.onAreaChange += OnAreaChange;
      character.onLeftWall += OnLeftWall;
      character.onRightWall += OnRightWall;
    }
    /// <summary>
    /// Listen area changes
    /// if enter ladder climb
    /// if leave ladder resume horizontal movement
    /// </summary>
    void OnAreaChange(Areas before, Areas after) {
      if ((after & Areas.Ladder) == Areas.Ladder) {
        if (character.ladder.IsAboveTop(character, character.feet)) {
          inputMgr.SetY(-1);
        } else {
          inputMgr.SetY(1);
        }

        inputMgr.SetX(0);
        UpdateManager.SetTimeout(ContinueMoving, 2.5f);
      } else {
        inputMgr.SetX((float)facing);
      }
    }

    /// <summary>
    /// resume horizontal movement
    /// this prevent to get stuck in a vine (where top is not reachable)
    /// </summary>
    void ContinueMoving() {
      inputMgr.SetX((float)facing);
    }
    /// <summary>
    /// Character hit a wall, move in the other direction
    /// </summary>
    void OnLeftWall() {
      facing = Facing.Right;
      inputMgr.SetX((float)facing);
    }
    /// <summary>
    /// Character hit a wall, move in the other direction
    /// </summary>
    void OnRightWall() {
      facing = Facing.Left;
      inputMgr.SetX((float)facing);
    }
  }
}
