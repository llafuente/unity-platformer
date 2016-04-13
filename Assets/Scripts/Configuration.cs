using UnityEngine;

namespace UnityPlatformer {
  public class Configuration : MonoBehaviour {
    public static Configuration instance;

    public string playerTag = "Player";
    public string movingPlatformThroughTag = "MovingPlatformThrough";
    public string movingPlatformTag = "MovingPlatform";
    public string enemyTag = "Enemy";
    public string projectileTag = "Projectile";
    public float minDistanceToEnv = 0.1f;


    void Awake() {
      if (Configuration.instance) {
        Debug.LogError("Configuration must be instanced only once, this instance will be ignored.");
        return;
      }

      Configuration.instance = this;
    }
  }
}
