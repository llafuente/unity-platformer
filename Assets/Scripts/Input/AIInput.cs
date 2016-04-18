using UnityEngine;
using System.Collections.Generic;
using System;

#if CN_INPUT_MANAGER
  using CnControls;
#endif


namespace UnityPlatformer {
  /// <summary>
  /// This class fake input allowing to manually(by code) control input.
  /// </summary>
  public class AIInput : PlatformerInput
  {
    Dictionary<string, bool> actions = new Dictionary<string, bool>();
    Vector2 axis = new Vector2(0, 0);

    public void EnableAction(string action) {
      actions[action] = true;
    }
    public void DisableAction(string action) {
      actions[action] = false;
    }
    public void SetAxis(Vector2 v) {
      axis = v;
    }
    public void SetX(float x) {
      axis.x = x;
    }
    public void SetY(float x) {
      axis.x = x;
    }
    public override bool IsActionHeld(string action) {
      return actions[action];
    }
    public override bool IsActionDown(string action) {
      return actions[action];
    }
    public override bool IsLeftDown() {
      return axis.x > 0;
    }
    public override bool IsRightDown() {
      return axis.x < 0;
    }
    public override bool IsUpDown() {
      return axis.y > 0;
    }
    public override bool IsDownDown() {
      return axis.y < 0;
    }
    public override float GetAxisRawX() {
      return axis.x;
    }
    public override float GetAxisRawY() {
      return axis.y;
    }
    public override Vector2 GetAxisRaw() {
      return axis;
    }
  }
}
