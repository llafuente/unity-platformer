using UnityEngine;
using System.Collections.Generic;
using System;

#if CN_INPUT_MANAGER
  using CnControls;
#endif


namespace UnityPlatformer {
  /// <summary>
  /// Fake input for AI.
  ///
  /// Allow to manually (by code) control input/actions.\n
  /// This way a AI is exactly like a playable character
  /// so no code duplication :)
  /// </summary>
  public class AIInput : PlatformerInput {
    /// <summary>
    /// Input 4 axis
    /// </summary>
    Vector2 axis = new Vector2(0, 0);
    /// <summary>
    /// Enable action
    /// </summary>
    public void EnableAction(string action) {
      actions[action] = InputStates.On;
    }
    /// <summary>
    /// Disable action
    /// </summary>
    public void DisableAction(string action) {
      actions[action] = InputStates.Off2;
    }
    /// <summary>
    /// Modify axis
    /// </summary>
    public void SetAxis(Vector2 v) {
      axis = v;
    }
    /// <summary>
    /// Modify X axis
    /// </summary>
    public void SetX(float x) {
      axis.x = x;
    }
    /// <summary>
    /// Modify Y axis
    /// </summary>
    public void SetY(float y) {
      axis.y = y;
    }

    public override bool IsActionHeld(string action) {
      InputStates value;
      if (actions.TryGetValue(action, out value)) {
        return value > InputStates.Off;
      }
      return false;
    }

    public override bool IsActionDown(string action) {
      InputStates value;
      if (actions.TryGetValue(action, out value)) {
        return value == InputStates.On;
      }
      return false;
    }

    public override bool IsActionUp(string action) {
      InputStates value;
      if (actions.TryGetValue(action, out value)) {
        return value == InputStates.Off;
      }
      return false;
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
