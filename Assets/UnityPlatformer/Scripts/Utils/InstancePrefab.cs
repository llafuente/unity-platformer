using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UnityPlatformer {
  // TODO alignament for image/text
  public class InstancePrefab : MonoBehaviour {
    public Texture placeholder;
    public GameObject prefab;
    public bool attachToRoot = true; // false child

    internal GameObject instance = null;

    // no virtual on purpose. override OnAwake
    public void Awake() {
      OnAwake();
    }

    public virtual void OnAwake(bool notify = true) {
      if (instance == null) {
        instance = Instantiate(prefab, transform.position, transform.rotation) as GameObject;

        if (!attachToRoot) {
           instance.transform.parent = gameObject.transform;
        }
      }

      if (notify) {
        SendMessage("OnInstancePrefab", this, SendMessageOptions.DontRequireReceiver);
      }
    }


    #if UNITY_EDITOR
      public virtual void OnDrawGizmos() {
        if (Application.isPlaying) return;

        if (placeholder) {
          Handles.Label(transform.position + new Vector3(0, 0.1f, 0), transform.gameObject.name);
          Handles.Label(transform.position, placeholder);
        } else {
          Handles.Label(transform.position, transform.gameObject.name);
        }
      }
    #endif

  }
}
