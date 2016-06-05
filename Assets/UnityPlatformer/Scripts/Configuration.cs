using UnityEngine;

namespace UnityPlatformer {
  public class Configuration : MBSingleton<Configuration> {
    public Vector2 gravity = Vector2.zero;
    public string playerTag = "Player";
    public string oneWayPlatformsTag = "OneWayPlatforms";
    public string movingPlatformThroughTag = "MovingPlatformThrough";
    public string movingPlatformTag = "MovingPlatform";
    public string enemyTag = "Enemy";
    public string projectileTag = "Projectile";
    public string boxTag = "Box";
    public float minDistanceToEnv = 0.1f;
    public LayerMask laddersMask;

    static public bool IsOneWayPlatform(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayPlatformsTag) != -1;
    }

    static public bool IsOneWayPlatform(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayPlatformsTag) != -1;
    }

    static public bool IsMovingPlatformThrough(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.movingPlatformThroughTag) != -1;
    }

    static public bool IsMovingPlatformThrough(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.movingPlatformThroughTag) != -1;
    }

    static public bool IsMovingPlatform(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.movingPlatformTag) != -1;
    }

    static public bool IsMovingPlatform(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.movingPlatformTag) != -1;
    }

    static public bool IsBox(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.boxTag) != -1;
    }

    static public bool IsBox(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.boxTag) != -1;
    }
  }
}
