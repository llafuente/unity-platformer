using UnityEngine;

namespace UnityPlatformer {
  public class Configuration : MBSingleton<Configuration> {
    public string playerTag = "Player";
    public string movingPlatformThroughTag = "MovingPlatformThrough";
    public string movingPlatformTag = "MovingPlatform";
    public string enemyTag = "Enemy";
    public string projectileTag = "Projectile";
    public float minDistanceToEnv = 0.1f;
    public LayerMask laddersMask;
  }
}
