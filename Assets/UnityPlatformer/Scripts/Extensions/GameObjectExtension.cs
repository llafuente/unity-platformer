using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// UnityEngine.GameObject
  /// </summary>
  public static class GameObjectExtension {
    /// <summary>
    /// GetComponent, if not exists AddComponent and return it.
    /// </summary>
    static public T GetOrAddComponent<T>(this GameObject obj) where T : UnityEngine.Component {
      T t = obj.GetComponent<T>();

      if (t == null) {
        return (T) obj.AddComponent<T>();
      }

      return (T) t;
    }
    /// <summary>
    /// Get mine and all children Rendereres and return the Bounds that Encapsulate them.
    /// </summary>
    static public Bounds GetRenderersBounds(this GameObject obj) {
      Bounds bounds = GetChildRenderersBounds(obj);
      Renderer renderer = obj.GetComponent<Renderer>();
      if (renderer != null) {
        bounds.Encapsulate(renderer.bounds);
      }

      return bounds;
    }
    /// <summary>
    /// Get all children Rendereres and return the Bounds that Encapsulate them.
    /// </summary>
    static public Bounds GetChildRenderersBounds(this GameObject obj) {
      Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
      Bounds bounds = new Bounds();
      bounds = renderers[0].bounds;
      for (int i = 1; i < renderers.Length; i++) {
        bounds.Encapsulate(renderers[i].bounds);
      }

      return bounds;
    }
    /// <summary>
    /// Shortcut
    /// </summary>
    static public GameObject CreateChild(this GameObject obj, string name) {
      GameObject robj = new GameObject ();
      robj.name = name;
      robj.transform.parent = obj.transform;

      return robj;
    }
    /// <summary>
    /// Get full path to object
    /// </summary>
    static public string GetFullName(this GameObject theObj) {
      string path = "/" + theObj.name;
      GameObject obj = theObj;
      while (obj.transform.parent != null) {
        obj = obj.transform.parent.gameObject;
        path = "/" + obj.name + path;
      }

      return path;
    }
  }
}
