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
  }
}
