// #define UP_USE_WII_INPUT_MANAGER
// #define UP_USE_CN_INPUT_MANAGER

using UnityEngine;
using System.Collections.Generic;
using System;

#if UP_USE_CN_INPUT_MANAGER
  using CnControls;
#endif

#if UP_USE_WII_INPUT_MANAGER
  using WiimoteApi;
#endif

// TODO player selection?!

namespace UnityPlatformer {
  /// <summary>
  /// internal map of Wiimote library buttons
  /// TODO nunchuck, and other pads?
  /// </summary>
  public enum WiiButtons {
    WII_BUTTON_1,
    WII_BUTTON_2,
    WII_BUTTON_A,
    WII_BUTTON_B,
    WII_BUTTON_PLUS,
    WII_BUTTON_MINUS,
    WII_BUTTON_HOME
  }
  /// <summary>
  /// Keyboard, Touch and Wii
  ///
  /// Touch use CnControls [https://www.assetstore.unity3d.com/en/#!/content/15233]
  /// to enable touch you need to uncomment '\#define UP_USE_CN_INPUT_MANAGER'
  /// at the top of this file\n
  /// Wii controls use WiimoteApi [https://github.com/Flafla2/Unity-Wiimote] to enable wiimote support you need to
  /// uncomment '\#define UP_USE_WII_INPUT_MANAGER' at the top of this file
  /// </summary>
  public class DefaultInput : PlatformerInput {
    [Serializable]
    public class InputMap {
      /// <summary>
      /// constructor
      /// </summary>
      public InputMap(String _action, String _handheld, String _keyboard, WiiButtons _wii) {
        action = _action;
        handheld = _handheld;
        keyboard = _keyboard;
        wii = _wii;
      }
      /// <summary>
      /// action identifier
      /// </summary>
      public String action;
      /// <summary>
      /// CNInput name
      /// </summary>
      public String handheld;
      /// <summary>
      /// Keyboard name
      /// </summary>
      public String keyboard;
      /// <summary>
      /// Button in a wiimote
      /// </summary>
      public WiiButtons wii;
    };
    /// <summary>
    /// List of action - button/key mapping
    /// </summary>
    public List<InputMap> inputsMap = new List<InputMap> {
      // default map
      new InputMap (
        "Jump",
        "Jump",
        "Jump",
        WiiButtons.WII_BUTTON_1
      ), new InputMap (
        "Attack",
        "Attack",
        "Fire2",
        WiiButtons.WII_BUTTON_2
      ), new InputMap (
        "Use",
        "Use",
        "Fire1",
        WiiButtons.WII_BUTTON_A
      ), new InputMap (
        "Run",
        "Use",
        "Run",
        WiiButtons.WII_BUTTON_B
      ), new InputMap (
        "Pull",
        "Pull",
        "Pull",
        WiiButtons.WII_BUTTON_B // REVIEW
      )
    };

    #if UP_USE_WII_INPUT_MANAGER
    Wiimote remote;
    #endif

    public override void Start() {
      base.Start();
      #if UP_USE_WII_INPUT_MANAGER
      WiimoteManager.FindWiimotes(); // Poll native bluetooth drivers to find Wiimotes

      foreach(Wiimote r in WiimoteManager.Wiimotes) {
        remote = r;
        remote.SendPlayerLED(true, false, false, true);
        remote.SendDataReportMode(InputDataType.REPORT_BUTTONS);
      }
      #endif
    }

    public override bool IsActionHeld(string action) {
      foreach (var i in inputsMap) {
        if (i.action == action) {
          #if UP_USE_CN_INPUT_MANAGER
          if (SystemInfo.deviceType == DeviceType.Handheld) {
            return CnInputManager.GetButton(i.handheld);
          }
          #endif

          #if UP_USE_WII_INPUT_MANAGER
          if (remote != null) {
            return GetWiiButton(i.wii);
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

          #if UP_USE_WII_INPUT_MANAGER
          if (remote != null) {
            return GetWiiButton(i.wii);
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

          #if UP_USE_WII_INPUT_MANAGER
          if (remote != null) {
            return !GetWiiButton(i.wii);
          }
          #endif

          return Input.GetButtonUp(i.keyboard);
        }
      }

      Debug.LogWarning ("Cannot find action: " + action);
      return false;
    }

    #if UP_USE_WII_INPUT_MANAGER
    bool GetWiiButton(WiiButtons btn) {
      if (remote != null) {
        switch(btn) {
        case WiiButtons.WII_BUTTON_1:
          return remote.Button.one;
        case WiiButtons.WII_BUTTON_2:
          return remote.Button.two;
        case WiiButtons.WII_BUTTON_A:
          return remote.Button.a;
        case WiiButtons.WII_BUTTON_B:
          return remote.Button.b;
        case WiiButtons.WII_BUTTON_PLUS:
          return remote.Button.plus;
        case WiiButtons.WII_BUTTON_MINUS:
          return remote.Button.minus;
        case WiiButtons.WII_BUTTON_HOME:
          return remote.Button.home;
        }
      }

      return false;
    }
    #endif

    public override bool IsLeftDown() {
      #if UP_USE_CN_INPUT_MANAGER
      if (SystemInfo.deviceType == DeviceType.Handheld) {
        return CnInputManager.GetAxis("Horizontal") > 0;
      }
      #endif

      #if UP_USE_WII_INPUT_MANAGER
      if (remote != null) {
        //return remote.Button.d_left;
        return remote.Button.d_up;
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

      #if UP_USE_WII_INPUT_MANAGER
      if (remote != null) {
        //return remote.Button.d_right;
        return remote.Button.d_down;
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

      #if UP_USE_WII_INPUT_MANAGER
      if (remote != null) {
        //return remote.Button.d_up;
        return remote.Button.d_right;
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

      #if UP_USE_WII_INPUT_MANAGER
      if (remote != null) {
        //return remote.Button.d_down;
        return remote.Button.d_left;
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

      #if UP_USE_WII_INPUT_MANAGER
      if (remote != null) {
        //return remote.Button.d_left ? -1 : (remote.Button.d_right ? 1 : 0);
        return remote.Button.d_up ? -1 : (remote.Button.d_down ? 1 : 0);
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

      #if UP_USE_WII_INPUT_MANAGER
      if (remote != null) {
        return remote.Button.d_left ? -1 : (remote.Button.d_down ? 1 : 0);
      }
      #endif

      return Input.GetAxisRaw ("Vertical");
    }

    public override Vector2 GetAxisRaw() {
      return new Vector2(GetAxisRawX(), GetAxisRawY());
    }

    public override void Update() {
      #if UP_USE_WII_INPUT_MANAGER
      int ret;
      do
      {
          ret = remote.ReadWiimoteData();
      } while (ret > 0); // ReadWiimoteData() returns 0 when nothing is left to read.  So by doing this we continue to
                         // update the Wiimote until it is "up to date."
      #endif

      base.Update();
    }
  }
}
