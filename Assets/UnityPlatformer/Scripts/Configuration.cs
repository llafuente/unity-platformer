using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// UnityPlatformer Configuration behaviour.
  ///
  /// It's a singleton and must be included in every scene.\n
  /// We recommended you to prefab this.\n
  /// NOTE executionOrder should be -100
  /// </summary>
  public class Configuration : MBSingleton<Configuration> {
    /// <summary>
    /// Global gravity. Can be overriden by Character.gravity
    /// </summary>
    public Vector2 gravity = Vector2.zero;
    /// <summary>
    /// Global min distance to Environment
    /// </summary>
    public float minDistanceToEnv = 0.1f;
    /// <summary>
    /// Mask for ladders
    /// </summary>
    public LayerMask laddersMask;

    [Space(20)]
    [Header("Tags")]
    /// <summary>
    /// Player tag
    /// </summary>
    public string playerTag = "Player";
    /// <summary>
    /// one-way-platform up tag
    /// </summary>
    public string oneWayPlatformUpTag = "OneWayPlatformUp";
    /// <summary>
    /// one-way-platform down tag
    /// </summary>
    public string oneWayPlatformDownTag = "OneWayPlatformDown";
    /// <summary>
    /// one-way-platform left tag
    /// </summary>
    public string oneWayWallLeftTag = "OneWayWallLeft";
    /// <summary>
    /// one-way-platform right tag
    /// </summary>
    public string onwWayWallRightTag = "OneWayWallRight";
    /// <summary>
    /// moving platform through tag
    /// </summary>
    public string movingPlatformThroughTag = "MovingPlatformThrough";
    /// <summary>
    /// moving platform tag
    /// </summary>
    public string movingPlatformTag = "MovingPlatform";
    /// <summary>
    /// enemy tag
    /// </summary>
    public string enemyTag = "Enemy";
    /// <summary>
    /// projectile tag
    /// </summary>
    public string projectileTag = "Projectile";
    /// <summary>
    /// box tag
    /// </summary>
    public string boxTag = "Box";

    [Space(20)]
    [Header("UpdateManager priorities")]
    /// <summary>
    /// Cooldown priority, should be the highest so when updating anything else
    /// cooldowns are updated first
    /// </summary>
    public int cooldownsPriority = 100;
    /// <summary>
    /// Moving platform priority
    /// </summary>
    public int movingPlatformsPriority = 50;
    /// <summary>
    /// Characters priority
    /// </summary>
    public int charactersPriority = 30;
    /// <summary>
    /// Enemies priority
    /// </summary>
    public int enemiesPriority = 30;
    /// <summary>
    /// Projectile priority
    /// </summary>
    public int projectilesPriority = 20;
    /// <summary>
    /// Default priority
    /// </summary>
    public int defaultPriority = 10;

    [Space(20)]
    [Header("Masks")]
    /// <summary>
    /// Mask for HitBoxes with type RecieveDamage
    /// </summary>
    public LayerMask recieveDamageMask;
    /// <summary>
    /// Mask for HitBoxes with type DealDamage
    /// </summary>
    public LayerMask dealDamageMask;
    /// <summary>
    /// Mask for HitBoxes with type EnterAreas
    /// </summary>
    public LayerMask enterAreasMask;

    /// <summary>
    /// check if given GameObject has a the tag oneWayPlatformUpTag
    /// </summary>
    static public bool IsOneWayPlatformUp(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayPlatformUpTag) != -1;
    }
    /// <summary>
    /// check if given Collider2D has a the tag oneWayPlatformUpTag
    /// </summary>
    static public bool IsOneWayPlatformUp(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayPlatformUpTag) != -1;
    }
    /// <summary>
    /// check if given GameObject has a the tag oneWayPlatformDownTag
    /// </summary>
    static public bool IsOneWayPlatformDown(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayPlatformDownTag) != -1;
    }
    /// <summary>
    /// check if given Collider2D has a the tag oneWayPlatformDownTag
    /// </summary>
    static public bool IsOneWayPlatformDown(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayPlatformDownTag) != -1;
    }
    /// <summary>
    /// check if given GameObject has a the tag oneWayWallLeftTag
    /// </summary>
    static public bool IsOneWayWallLeft(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayWallLeftTag) != -1;
    }
    /// <summary>
    /// check if given Collider2D has a the tag oneWayWallLeftTag
    /// </summary>
    static public bool IsOneWayWallLeft(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.oneWayWallLeftTag) != -1;
    }
    /// <summary>
    /// check if given GameObject has a the tag onwWayWallRightTag
    /// </summary>
    static public bool IsOneWayWallRight(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.onwWayWallRightTag) != -1;
    }
    /// <summary>
    /// check if given Collider2D has a the tag onwWayWallRightTag
    /// </summary>
    static public bool IsOneWayWallRight(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.onwWayWallRightTag) != -1;
    }
    /// <summary>
    /// check if given GameObject has a the tag movingPlatformThroughTag
    /// </summary>
    static public bool IsMovingPlatformThrough(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.movingPlatformThroughTag) != -1;
    }
    /// <summary>
    /// check if given Collider2D has a the tag movingPlatformThroughTag
    /// </summary>
    static public bool IsMovingPlatformThrough(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.movingPlatformThroughTag) != -1;
    }
    /// <summary>
    /// check if given GameObject has a the tag movingPlatformTag
    /// </summary>
    static public bool IsMovingPlatform(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.movingPlatformTag) != -1;
    }
    /// <summary>
    /// check if given Collider2D has a the tag movingPlatformTag
    /// </summary>
    static public bool IsMovingPlatform(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.movingPlatformTag) != -1;
    }
    /// <summary>
    /// check if given GameObject has a the tag boxTag
    /// </summary>
    static public bool IsBox(GameObject obj) {
      return obj.tag.IndexOf(Configuration.instance.boxTag) != -1;
    }
    /// <summary>
    /// check if given Collider2D has a the tag boxTag
    /// </summary>
    static public bool IsBox(Collider2D obj) {
      return obj.tag.IndexOf(Configuration.instance.boxTag) != -1;
    }
  }
}
