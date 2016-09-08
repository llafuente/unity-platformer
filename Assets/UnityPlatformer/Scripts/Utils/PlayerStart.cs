using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace UnityPlatformer {
  public class PlayerStart : InstancePrefab {
    /// <summary>
    /// If you have your own camera, this should be false.
    /// </summary>
    public bool setupCameraFollow = true;
    public bool monitor = false;

    internal CharacterMonitor mon;

    public override void OnAwake(bool notify = true) {
      base.OnAwake(false); // notify below, maybe someone need mon

      Character[] chars = instance.gameObject.GetComponentsInChildren<Character>();
      if (chars.Length != 1) {
        Debug.LogErrorFormat("Found {0} Character(s) expected 1", chars.Length);
        return;
      }

      PlatformerInput[] inputs = instance.gameObject.GetComponentsInChildren<PlatformerInput>();

      if (inputs.Length != 1) {
        Debug.LogErrorFormat("Found {0} PlatformerInput(s) expected 1", inputs.Length);
        return;
      }

      mon = chars[0].gameObject.GetOrAddComponent<CharacterMonitor>();
      mon.enabled = monitor;

      if (setupCameraFollow) {
        var cams = Camera.allCameras;

        foreach (var c in cams) {
          CameraFollow cf = c.GetComponent<CameraFollow>();

          if (cf) {
            cf.target = chars[0];
            cf.targetInput = inputs[0];
          }
        }
      }

      // notify
      SendMessage("OnInstancePrefab", this, SendMessageOptions.DontRequireReceiver);
    }
  }
}
