using UnityEngine;
using System.Collections.Generic;
using System;

#if CN_INPUT_MANAGER
  using CnControls;
#endif


namespace UnityPlatformer {
  /// <summary>
  /// </summary>
  public class DefaultInput : PlatformerInput
  {
    [Serializable]
    public class InputMap {
      public InputMap(String _action, String _handheld, String _keyboard) {
        action = _action;
        handheld = _handheld;
        keyboard = _keyboard;
      }
      public String action;
      public String handheld;
      public String keyboard;
    };

    public List<InputMap> inputsMap = new List<InputMap> {
      // default map
      new InputMap (
        "Jump",
        "Jump",
        "Jump"
      ), new InputMap (
        "Attack",
        "Attack",
        "Fire2"
      )
    };

    void Update() {
      // TODO foreach input check if down, stored and allow "pressed"
    }

    public override bool IsActionDown(string action) {
      foreach (var i in inputsMap) {
        if (i.action == action) {
          #if CN_INPUT_MANAGER
          if (SystemInfo.deviceType == DeviceType.Handheld) {
            return CnInputManager.GetButtonDown(i.handheld);
          }
          #endif

          return Input.GetButtonDown(i.keyboard);
        }
      }

      Debug.LogWarning ("Cannot find action: " + action);
      return false;
    }

    public override bool IsActionButtonDown(string action) {
      foreach (var i in inputsMap) {
        if (i.action == action) {
          #if CN_INPUT_MANAGER
          if (SystemInfo.deviceType == DeviceType.Handheld) {
            return CnInputManager.GetButton(i.handheld);
          }
          #endif

          return Input.GetButton(i.keyboard);
        }
      }

      Debug.LogWarning ("Cannot find action: " + action);
      return false;
    }

    public override bool IsLeftDown() {
      #if CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Horizontal") > 0;
      }
      #endif

      return Input.GetAxisRaw("Horizontal") > 0;
    }

    public override bool IsRightDown() {
      #if CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Horizontal") < 0;
      }
      #endif

      return Input.GetAxisRaw("Horizontal") < 0;
    }

    public override bool IsUpDown() {
      #if CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Vertical") > 0;
      }
      #endif

      return Input.GetAxisRaw("Vertical") > 0;
    }

    public override bool IsDownDown() {
      #if CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Vertical") < 0;
      }
      #endif

      return Input.GetAxisRaw("Vertical") < 0;
    }

    public override float GetAxisRawX() {
      #if CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Horizontal") < 0;
      }
      #endif

      return Input.GetAxisRaw ("Horizontal");
    }

    public override float GetAxisRawY() {
      #if CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Vertical") < 0;
      }
      #endif

      return Input.GetAxisRaw ("Vertical");
    }

    public override Vector2 GetAxisRaw() {
      return new Vector2(GetAxisRawX(), GetAxisRawY());
    }
  }
}
