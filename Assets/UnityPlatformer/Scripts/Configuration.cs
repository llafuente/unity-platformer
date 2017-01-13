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
    [Header("Layers")]
    /// <summary>
    /// Mask name for Ladders
    /// </summary>
    public string laddersLayerName = "Ladders";
    /// <summary>
    /// Mask for Ladders
    /// </summary>
    [HideInInspector]
    public LayerMask laddersMask;

    /// <summary>
    /// Mask name for Fences
    /// </summary>
    public string fencesLayerName = "Fences";
    /// <summary>
    /// Mask for Fences
    /// </summary>
    [HideInInspector]
    public LayerMask fencesMask;

    /// <summary>
    /// Mask name for Ropes
    /// </summary>
    public string ropesLayerName = "Ropes";
    /// <summary>
    /// Mask for Ropes
    /// </summary>
    [HideInInspector]
    public LayerMask ropesMask;

    /// <summary>
    /// Mask name for HitBoxes
    /// </summary>
    public string hitBoxesLayerName = "HitBoxes";
    /// <summary>
    /// Mask for HitBoxes
    /// </summary>
    [HideInInspector]
    public LayerMask hitBoxesMask;

    /// <summary>
    /// Mask name for Players
    /// </summary>
    public string playersLayerName = "Players";
    /// <summary>
    /// Mask for Players
    /// </summary>
    [HideInInspector]
    public LayerMask playersMask;

    /// <summary>
    /// Mask name for Enemies
    /// </summary>
    public string enemiesLayerName = "Enemies";
    /// <summary>
    /// Mask for Enemies
    /// </summary>
    [HideInInspector]
    public LayerMask enemiesMask;

    /// <summary>
    /// Mask name for Grabables
    /// </summary>
    public string grabablesLayerName = "Grabables";
    /// <summary>
    /// Mask for Grabables
    /// </summary>
    [HideInInspector]
    public LayerMask grabablesMask;

    /// <summary>
    /// Mask name for Liquids
    /// </summary>
    public string liquidsLayerName = "Liquids";
    /// <summary>
    /// Mask for Liquids
    /// </summary>
    [HideInInspector]
    public LayerMask liquidsMask;

    /// <summary>
    /// Mask name for Boxes
    /// </summary>
    public string boxesLayerName = "Boxes";
    /// <summary>
    /// Mask for Boxes
    /// </summary>
    [HideInInspector]
    public LayerMask boxesMask;

    /// <summary>
    /// Mask name for Tracks
    /// </summary>
    public string tracksLayerName = "Tracks";
    /// <summary>
    /// Mask for Tracks
    /// </summary>
    [HideInInspector]
    public LayerMask tracksMask;

    /// <summary>
    /// Mask name for Items
    /// </summary>
    public string itemsLayerName = "Items";
    /// <summary>
    /// Mask for Items
    /// </summary>
    [HideInInspector]
    public LayerMask itemsMask;

    /// <summary>
    /// Mask name for Items
    /// </summary>
    public string projectilesLayerName = "Projectiles";
    /// <summary>
    /// Mask for Items
    /// </summary>
    [HideInInspector]
    public LayerMask projectilesMask;

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

#if UNITY_EDITOR
    /// <summary>
    /// transform layer names to LayerMask
    /// </summary>
    void OnValidate() {
      laddersMask = LayerMask.NameToLayer(laddersLayerName);
      fencesMask = LayerMask.NameToLayer(fencesLayerName);
      ropesMask = LayerMask.NameToLayer(ropesLayerName);
      hitBoxesMask = LayerMask.NameToLayer(hitBoxesLayerName);
      enemiesMask = LayerMask.NameToLayer(enemiesLayerName);
      grabablesMask = LayerMask.NameToLayer(grabablesLayerName);
      liquidsMask = LayerMask.NameToLayer(liquidsLayerName);
      boxesMask = LayerMask.NameToLayer(boxesLayerName);
      tracksMask = LayerMask.NameToLayer(tracksLayerName);
      itemsMask = LayerMask.NameToLayer(itemsLayerName);
      projectilesMask = LayerMask.NameToLayer(projectilesLayerName);
    }
#endif

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

    static public Character GetNearestPlayer(Vector3 position) {
      GameObject[] gos = GameObject.FindGameObjectsWithTag(instance.playerTag);
      GameObject closest = null;

      float distance = Mathf.Infinity;
      foreach (GameObject go in gos) {
        Vector3 diff = go.transform.position - position;
        float curDistance = diff.sqrMagnitude;
        if (curDistance < distance) {
          closest = go;
          distance = curDistance;
        }
      }

      return closest ? closest.GetComponent<Character>() : null;
    }
  }
}
