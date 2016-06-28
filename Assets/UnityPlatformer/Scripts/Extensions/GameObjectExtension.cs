using UnityEngine;

namespace UnityPlatformer {
  public static class GameObjectExtension {
    static public T GetOrAddComponent<T>(this GameObject obj) where T : UnityEngine.Component {
      T t = obj.GetComponent<T>();

      if (t == null) {
        return (T) obj.AddComponent<T>();
      }

      return (T) t;
    }

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
