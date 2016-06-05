using UnityEngine;
using System.Collections.Generic;
using System;

namespace UnityPlatformer {
  public enum InputStates {
    Off = 0,
    On = 1,
    Held = 2,
  }

  /// <summary>
  /// Handle input for: Players and IA
  /// The Action concept is the same as button for keyboards
  /// but it's abstracted to handle the rest with just a single name.
  /// for example: Jump, keyboard: press Space. PS4: X etc...
  /// You map everything to the Jump action, extend the PlatformerInput to
  /// read input properly :)
  /// NOTE onActionUp, onActionDown it's the recommended way to handle
  /// input rather than calling IsAction*, because you can miss a frame and
  /// don't catch IsActionDown. Or you can use a more reliable version: IsActionHeld
  /// TODO IsActionActionUp
  /// </summary>
  public abstract class PlatformerInput : MonoBehaviour
  {

    [Comment("List of actions that will fire events")]
    public List<string> listenActions = new List<string> {"Jump", "Attack"};

    public delegate void InputActionDelegate(string button);
    public InputActionDelegate onActionUp;
    public InputActionDelegate onActionDown;

    // cache
    protected Dictionary<string, InputStates> actions = new Dictionary<string, InputStates>();

    public void Start() {
      foreach (var button in listenActions) {
        actions[button] = InputStates.Off;
      }
    }

    public virtual void Update() {
      foreach (var button in listenActions) {
        if (IsActionHeld(button)) {
          if (actions[button] != InputStates.Held) {
            if (onActionDown != null) {
              onActionDown(button);
            }
          }

          actions[button] = InputStates.Held;
        } else {
          if (actions[button] != InputStates.Off) {
            if (onActionUp != null) {
              onActionUp(button);
            }
          }

          actions[button] = InputStates.Off;
        }
      }
    }

    /// <summary>
    /// Returns true while the Action(~Button) is held down.
    /// </summary>
    public abstract bool IsActionHeld(string action);
    /// <summary>
    /// Returns true the frame the Action(~Button) is down.
    /// </summary>
    public abstract bool IsActionDown(string action);
    public abstract bool IsActionUp(string action);
    public abstract bool IsLeftDown();
    public abstract bool IsRightDown();
    public abstract bool IsUpDown();
    public abstract bool IsDownDown();
    public abstract float GetAxisRawX();
    public abstract float GetAxisRawY();
    public abstract Vector2 GetAxisRaw();
  }
}
