using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UnityPlatformer {
  // TODO alignament for image/text
  public class InstancePrefab : MonoBehaviour {
    public Sprite placeholder;
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

        var eCam = UnityEditor.SceneView.currentDrawingSceneView.camera;
        var cameraDistance =  Vector3.Distance(eCam.transform.position, transform.position);
        GUIStyle style = GUI.skin.label;
        style.fontSize = (int)(512 / cameraDistance);

        if (placeholder) {

          Vector3 size = placeholder.bounds.size * 0.5f;
          size.y *= -1;
          Vector3 top_left = transform.position - size;
          Vector2 start = HandleUtility.WorldToGUIPoint(top_left);
          Vector2 end = HandleUtility.WorldToGUIPoint(transform.position + size);


          Handles.BeginGUI();
          GUI.DrawTexture(
            new Rect(start.x, start.y, Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y)),
            placeholder.texture);

          Handles.EndGUI();

          Handles.Label(top_left + new Vector3(0, 0.2f, 1), transform.gameObject.name);
        } else {
          Handles.Label(transform.position, transform.gameObject.name);
        }
      }
    #endif

  }
}
