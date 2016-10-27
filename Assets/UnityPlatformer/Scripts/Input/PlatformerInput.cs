using UnityEngine;
using System.Collections.Generic;
using System;

namespace UnityPlatformer {
  /// <summary>
  /// Serializable button/key states
  /// </summary>
  [Serializable]
  public enum InputStates {
    // this state is a fake, just to turn the button off and fire the callback
    Off2 = -1,

    Off = 0,
    On = 1,
    Held = 2,
  }

  /// <summary>
  /// Handle input for: Players and IA
  ///
  /// The Action concept is the same as keys for keyboards
  /// but it's abstracted to handle the rest with just a single name.\n
  /// example: action Jump. On keyboard: Space key. PS4: X button. WII: A button.\n
  /// You map everything to the 'Jump' action, extend the PlatformerInput to
  /// read input properly :)\n
  /// NOTE onActionUp, onActionDown it's the recommended way to handle
  /// input rather than calling IsActionUp/Down, because you can miss a frame and
  /// don't catch IsActionDown. Or you can use a more reliable version: IsActionHeld
  /// </summary>
  public abstract class PlatformerInput : MonoBehaviour {
    /// <summary>
    /// List of all available actions to listen
    /// </summary>
    [Comment("List of actions that will fire events")]
    public List<string> listenActions = new List<string> {"Jump", "Attack", "Use", "Run", "Pull"};
    /// <summary>
    /// callback type for keyup/keydown
    /// </summary>
    public delegate void InputActionDelegate(string button);
    /// <summary>
    /// callback when action up (key/button associated is up)
    /// </summary>
    public InputActionDelegate onActionUp;
    /// <summary>
    /// callback when action down (key/button associated is down)
    /// </summary>
    public InputActionDelegate onActionDown;
    /// <summary>
    /// Serializable type for input actions/states
    /// </summary>
    [Serializable]
    public class InputDictionary : SerializableDictionary<string, InputStates> {};
    /// <summary>
    /// Dictionary of actions/states
    /// </summary>
    protected InputDictionary actions = new InputDictionary();
    /// <summary>
    /// Init actions Dictionary
    /// </summary>
    public virtual void Start() {
      foreach (var button in listenActions) {
        Log.Info("(PlatformerInput) Initialize button " + button);
        actions[button] = InputStates.Off;
      }
    }
    /// <summary>
    /// Listen to changs in Unity Input
    /// </summary>
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
    /// Returns true this frame the Action(~Button) is down.
    /// NOTE you could miss down event, use Held instead
    /// </summary>
    public abstract bool IsActionDown(string action);
    /// <summary>
    /// Returns true this frame the Action(~Button) is up.
    /// NOTE you could miss up event, use Held instead
    /// </summary>
    public abstract bool IsActionUp(string action);
    /// <summary>
    /// Returns true while Left is down.
    /// </summary>
    public abstract bool IsLeftDown();
    /// <summary>
    /// Returns true while Right is down.
    /// </summary>
    public abstract bool IsRightDown();
    /// <summary>
    /// Returns true while Up is down.
    /// </summary>
    public abstract bool IsUpDown();
    /// <summary>
    /// Returns true while Down is down.
    /// </summary>
    public abstract bool IsDownDown();
    /// <summary>
    /// Returns Input.GetAxisRaw ("Horizontal") or equivalent
    /// </summary>
    public abstract float GetAxisRawX();
    /// <summary>
    /// Returns Input.GetAxisRaw ("Vertical") or equivalent
    /// </summary>
    public abstract float GetAxisRawY();
    /// <summary>
    /// Returns a Vector2 with GetAxisRawX and GetAxisRawY
    /// </summary>
    public abstract Vector2 GetAxisRaw();
  }
}
