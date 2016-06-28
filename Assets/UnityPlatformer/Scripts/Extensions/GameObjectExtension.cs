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
  }
}
