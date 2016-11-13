using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

// TODO alignament for image/text

namespace UnityPlatformer {
  /// <summary>
  /// Instance a prefab on start
  /// </summary>
  public class InstancePrefab : MonoBehaviour {
    /// <summary>
    /// Counter to make names unique
    /// </summary>
    static int count = 1;
    /// <summary>
    /// Placeholder sprite for editor, to see something
    /// </summary>
    public Sprite placeholder;
    /// <summary>
    /// Prefab to instance
    /// </summary>
    public GameObject prefab;
    /// <summary>
    /// Attach the prefab as a child or at root node.
    /// </summary>
    public bool attachToRoot = true; // false child
    /// <summary>
    /// reference to instanced prefab
    /// </summary>
    [HideInInspector]
    public GameObject instance = null;

    /// <summary>
    /// NOTE no virtual on purpose. override OnAwake
    /// </summary>
    public void Awake() {
      OnAwake();
    }
    /// <summary>
    /// Rename logic: append counter to the name
    /// </summary>
    public virtual void Rename(Transform transform) {
      foreach (Transform child in transform) {
        child.gameObject.name = child.gameObject.name + count;
      }
    }
    /// <summary>
    /// Instance the prefab, rename and attach it
    /// </summary>
    /// <param name="notify">true -> SendMessage: OnInstancePrefab</param>
    public virtual void OnAwake(bool notify = true) {
      if (instance == null) {
        instance = Instantiate(prefab, transform.position, transform.rotation) as GameObject;

        instance.name = gameObject.name;
        Rename(instance.transform);
        ++count;

        if (!attachToRoot) {
           instance.transform.parent = gameObject.transform;
        }
      }

      if (notify) {
        SendMessage("OnInstancePrefab", this, SendMessageOptions.DontRequireReceiver);
      }
    }


    #if UNITY_EDITOR
      /// <summary>
      /// Draw placeholder + name in editor mode.
      /// </summary>
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
