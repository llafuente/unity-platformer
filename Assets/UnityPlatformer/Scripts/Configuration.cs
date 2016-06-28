using UnityEngine;

namespace UnityPlatformer {
  public class Configuration : MBSingleton<Configuration> {
    public Vector2 gravity = Vector2.zero;
    public float minDistanceToEnv = 0.1f;
    public LayerMask laddersMask;

    [Space(20)]
    [Header("Tags")]
    public string playerTag = "Player";
    public string oneWayPlatformUpTag = "OneWayPlatformUp";
    public string oneWayPlatformDownTag = "OneWayPlatformDown";
    public string oneWayWallLeftTag = "OneWayWallLeft";
    public string onwWayWallRightTag = "OneWayWallRight";
    public string movingPlatformThroughTag = "MovingPlatformThrough";
    public string movingPlatformTag = "MovingPlatform";
    public string enemyTag = "Enemy";
    public string projectileTag = "Projectile";
    public string boxTag = "Box";

    [Space(20)]
    [Header("UpdateManager priorities")]
    public int movingPlatformsPriority = 50;
    public int charactersPriority = 30;
    public int enemiesPriority = 30;
    public int projectilesPriority = 20;
    public int defaultPriority = 10;


    static public bool IsOneWayPlatformUp(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayPlatformUpTag) != -1;
    }

    static public bool IsOneWayPlatformUp(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayPlatformUpTag) != -1;
    }

    static public bool IsOneWayPlatformDown(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayPlatformDownTag) != -1;
    }

    static public bool IsOneWayPlatformDown(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayPlatformDownTag) != -1;
    }

    static public bool IsOneWayWallLeft(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayWallLeftTag) != -1;
    }

    static public bool IsOneWayWallLeft(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayWallLeftTag) != -1;
    }

    static public bool IsOneWayWallRight(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.onwWayWallRightTag) != -1;
    }

    static public bool IsOneWayWallRight(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.onwWayWallRightTag) != -1;
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
