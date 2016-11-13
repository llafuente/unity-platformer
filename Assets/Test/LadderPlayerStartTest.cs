using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using UnityPlatformer;

namespace UnityPlatformer.Test {
  /// <summary>
  /// Input for automated tests
  /// * Move right until wall, then move left
  /// * If found a ladder climb
  /// </summary>
  public class LadderPlayerStartTest : PlayerStart {
    /// <summary>
    /// PlatformerCollider2D to listen collision callbacks
    /// </summary>
    internal PlatformerCollider2D pc2d;
    /// <summary>
    /// where is facing
    /// </summary>
    Facing facing = Facing.Right;

    public float checkTimeSinceLastLadderArea = 10.0f;
    public float checkTimeSinceLastLadderState = 10.0f;

    [HideInInspector]
    public AIInput aiInput;


    public override void OnAwake(bool notify = true) {
      base.OnAwake(notify);


      aiInput = instance.GetComponentInChildren<AIInput>();
      if (aiInput == null) {
        Debug.LogWarning("AIInput is expected in the prefab");
        return;
      }
      pc2d = instance.GetComponentInChildren<PlatformerCollider2D>();
      character = instance.GetComponentInChildren<Character>();


      aiInput.SetX(1);
      character.onAreaChange += OnAreaChange;
      character.onStateChange += OnStateChange;
      pc2d.onLeftWall += OnLeftWall;
      pc2d.onRightWall += OnRightWall;
    }

    void OnStateChange(States before, States after) {
      if ((before & States.Ladder) == States.Ladder &&
          (after & States.OnGround) == States.OnGround) {
        aiInput.SetY(0);
        aiInput.SetX((float)facing);
      }
    }
    /// <summary>
    /// Listen area changes
    /// if enter ladder climb
    /// if leave ladder resume horizontal movement
    /// </summary>
    void OnAreaChange(Areas before, Areas after) {
      if ((after & Areas.Ladder) == Areas.Ladder) {
        if (character.ladder.IsAboveTop(character, character.feet)) {
          aiInput.SetY(-1);
        } else {
          aiInput.SetY(1);
        }

        aiInput.SetX(0);
        UpdateManager.SetTimeout(ContinueMoving, 2.5f);
      }
    }

    /// <summary>
    /// resume horizontal movement
    /// this prevent to get stuck in a vine (where top is not reachable)
    /// </summary>
    void ContinueMoving() {
      aiInput.SetX((float)facing);
    }
    /// <summary>
    /// Character hit a wall, move in the other direction
    /// </summary>
    void OnLeftWall() {
      facing = Facing.Right;
      aiInput.SetX((float)facing);
    }
    /// <summary>
    /// Character hit a wall, move in the other direction
    /// </summary>
    void OnRightWall() {
      facing = Facing.Left;
      aiInput.SetX((float)facing);
    }

    // TEST
    float timeSinceLastLadderArea = 0.0f;
    float timeSinceLastLadderState = 0.0f;

    public void OnEnable() {
      UpdateManager.SetInterval(OnEverySecond, -0.1f);
    }

    void OnEverySecond() {
      float delta = UpdateManager.GetCurrentDelta();
      timeSinceLastLadderArea += delta;
      timeSinceLastLadderState += delta;

      if (character.IsOnState(States.Ladder)) {
        timeSinceLastLadderState = 0.0f;
      }

      if (character.IsOnArea(Areas.Ladder)) {
        timeSinceLastLadderArea = 0.0f;
      }

      if (timeSinceLastLadderArea > checkTimeSinceLastLadderArea) {
        Assert.IsTrue(false, "checkTimeSinceLastLadderArea seconds without touching a Ladder");
      }
      if (timeSinceLastLadderState > checkTimeSinceLastLadderState) {
        Assert.IsTrue(false, "checkTimeSinceLastLadderState seconds being in a Ladder");
      }
    }
  }
}
