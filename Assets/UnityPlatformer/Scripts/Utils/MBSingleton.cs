using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Generic Singleton template for MonoBehaviours
  /// </summary>
  public class MBSingleton<T> : MonoBehaviour where T : MBSingleton<T> {
    /// <summary>
    /// Singleton instance
    /// </summary>
    private static T _instance;
    /// <summary>
    /// Singleton instance retrieval
    /// </summary>
    public static T instance {
      get {
        if (_instance == null) {
            _instance = (T) FindObjectOfType(typeof(T));
        }
        return _instance;
      }
    }
  }
}
