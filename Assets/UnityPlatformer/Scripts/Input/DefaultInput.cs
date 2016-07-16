// #def UP_USE_CN_INPUT_MANAGER

using UnityEngine;
using System.Collections.Generic;
using System;

#if UP_USE_CN_INPUT_MANAGER
  using CnControls;
#endif


namespace UnityPlatformer {
  /// <summary>
  /// Keyboard and Touch (https://www.assetstore.unity3d.com/en/#!/content/15233)
  /// All CnControls is enconsed in #ifdef.
  /// If your project use CnControls define UP_USE_CN_INPUT_MANAGER to support it.
  /// </summary>
  public class DefaultInput : PlatformerInput {
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
      ), new InputMap (
        "Use",
        "Use",
        "Fire1"
      ), new InputMap (
        "Run",
        "Use",
        "Run"
      )
    };

    public override bool IsActionHeld(string action) {
      foreach (var i in inputsMap) {
        if (i.action == action) {
          #if UP_USE_CN_INPUT_MANAGER
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

    public override bool IsActionDown(string action) {
      foreach (var i in inputsMap) {
        if (i.action == action) {
          #if UP_USE_CN_INPUT_MANAGER
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

    public override bool IsActionUp(string action) {
      foreach (var i in inputsMap) {
        if (i.action == action) {
          #if UP_USE_CN_INPUT_MANAGER
          if (SystemInfo.deviceType == DeviceType.Handheld) {
            return CnInputManager.GetButtonUp(i.handheld);
          }
          #endif

          return Input.GetButtonUp(i.keyboard);
        }
      }

      Debug.LogWarning ("Cannot find action: " + action);
      return false;
    }

    public override bool IsLeftDown() {
      #if UP_USE_CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Horizontal") > 0;
      }
      #endif

      return Input.GetAxisRaw("Horizontal") > 0;
    }

    public override bool IsRightDown() {
      #if UP_USE_CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Horizontal") < 0;
      }
      #endif

      return Input.GetAxisRaw("Horizontal") < 0;
    }

    public override bool IsUpDown() {
      #if UP_USE_CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Vertical") > 0;
      }
      #endif

      return Input.GetAxisRaw("Vertical") > 0;
    }

    public override bool IsDownDown() {
      #if UP_USE_CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Vertical") < 0;
      }
      #endif

      return Input.GetAxisRaw("Vertical") < 0;
    }

    public override float GetAxisRawX() {
      #if UP_USE_CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Horizontal") < 0;
      }
      #endif

      return Input.GetAxisRaw ("Horizontal");
    }

    public override float GetAxisRawY() {
      #if UP_USE_CN_INPUT_MANAGER
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
