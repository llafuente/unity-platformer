using UnityEngine;

namespace UnityPlatformer {
  public class MBSingleton<T> : MonoBehaviour where T : MBSingleton<T> {
    public static T instance;

    public virtual void Awake() {
      if (Configuration.instance) {
        Debug.LogError(gameObject + " must be instanced only once, this instance will be ignored.");
        return;
      }

      instance = this as T;
    }
  }
}
