using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UnityPlatformer {
  public class PlayerStart : InstancePrefab {
    /// <summary>
    /// If you have your own camera, this should be false.
    /// </summary>
    public bool setupCameraFollow = true;

    public override void Awake() {
      base.Awake();

      if (setupCameraFollow) {
        Debug.Log("SETUP CAMRA!!!");
        Character[] chars = instance.gameObject.GetComponentsInChildren<Character>();
        PlatformerInput[] inputs = instance.gameObject.GetComponentsInChildren<PlatformerInput>();


        if (chars.Length != 1) {
          Debug.LogErrorFormat("Cannot setup camera: Found {0} Character(s)", chars.Length);
          return;
        }

        if (inputs.Length != 1) {
          Debug.LogErrorFormat("Cannot setup camera: Found {0} PlatformerInput(s)", inputs.Length);
          return;
        }

        var cams = Camera.allCameras;

        foreach (var c in cams) {

          CameraFollow cf = c.GetComponent<CameraFollow>();
          if (cf) {
            cf.target = chars[0];
            cf.targetInput = inputs[0];
          }
        }
      }
    }
  }
}
