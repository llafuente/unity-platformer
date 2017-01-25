using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Assertions;

namespace UnityPlatformer {
  /// <summary>
  /// A projectile is an Object that move and collide with the environment.
  ///
  /// If a projectile collide with a HitBox then deal damage\n
  /// If a projectile collide witht the world is destroyed
  /// </summary>
  [RequireComponent (typeof (HitBox))]
  [RequireComponent (typeof (Damage))]
  public class Projectile : MonoBehaviour, IUpdateEntity {
    /// <summary>
    /// Layers wich destroy the projectile: StaticGeometry + HitBox + Projectiles
    /// </summary>
    [Comment("Destroy when collide with")]
    public LayerMask collisionMask;
    /// <summary>
    /// Initial velocity
    /// </summary>
    [Comment("Initial velocity, will be modified over time.")]
    public Vector2 velocity;
    /// <summary>
    /// Gravity, Projectiles ignore global gravity, usually everyone want
    /// straight projectiles
    /// </summary>
    [Comment("Projectile are not affected by global gravity use it's own")]
    public Vector2 gravity = Vector2.zero;
    /// <summary>
    /// Delay after impact before destroying the projectile
    /// </summary>
    [Comment("Destroy after impact")]
    public float destroyDelay;
    /// <summary>
    /// Callback when impact something
    /// </summary>
    public Action onImpact;
    /// <summary>
    /// Callback when is destroyed
    /// </summary>
    public Action onDestroy;
    /// <summary>
    /// impact against something "damageable" ?
    /// </summary>
    internal bool impact = false;
    /// <summary>
    /// When Object is Awake, it will be automatically disabled
    /// When Fired the projectile, will be cloned and enabled
    /// </summary>
    void Awake() {
      // disable at start (so people dont need to remember)
      gameObject.SetActive(false);

      // force trigger
      Collider2D col2d = GetComponent<Collider2D>();
      Assert.IsNotNull(col2d, "(Projectile) Missing Monobehaviour Collider2D at " + gameObject.GetFullName());
      col2d.isTrigger = true;
    }
#if UNITY_EDITOR
    /// <summary>
    /// Set layer to Configuration.ropesMask
    /// </summary>
    void Reset() {
      gameObject.layer = Configuration.instance.projectilesMask;
    }
#endif
    /// <summary>
    /// Clone the projectile, activate, add it to UpdateManager
    /// and return :)
    /// </summary>
    // TODO REVIEW onImpact & onDestroy are not cloned with Instantiate
    public virtual Projectile Fire(Vector3 position) {
      // TODO REVIEW WTF! all 4 lines are necessary?!
      gameObject.SetActive(true);
      GameObject obj = (GameObject) Instantiate(gameObject, position, Quaternion.identity);
      gameObject.SetActive(false);
      obj.SetActive(true);

      Projectile prj = obj.GetComponent<Projectile>();
      Assert.IsNotNull(prj, "(Projectile) Cloned object does not have Projectile monobehaviour: " + gameObject.GetFullName());

      UpdateManager.Push(prj, Configuration.instance.projectilesPriority);

      return prj;
    }
    /// <summary>
    /// Move projectile accordingly
    /// </summary>
    public virtual void PlatformerUpdate(float delta) {
      //Debug.Log("update projectile" + gameObject.GetFullName() + ": " + velocity);

      velocity += gravity * delta;
      transform.position += (Vector3)velocity * delta;
    }
    /// <summary>
    /// Do nothing use PlatformerUpdate instead
    /// </summary>
    public virtual void LatePlatformerUpdate(float delta) {}
    /// <summary>
    /// When collide with something, check the mask, then check if
    /// it's damageable, and Destroy
    /// </summary>
    public virtual void OnTriggerEnter2D(Collider2D o) {
      if (collisionMask.Contains(o.gameObject.layer)) {
        var dst = o.gameObject.GetComponent<HitBox> ();
        if (dst != null) {
          impact = true;
          // ignore DealDamage & EnterAreas
          if (dst.type != HitBoxType.RecieveDamage) {
            return;
          }
          // if HitBoxType.RecieveDamage
          if (onImpact != null) {
            onImpact();
          }
          //Debug.Log("Projectile impact something, deal damage!");
          dst.owner.Damage(GetComponent<Damage>());
        }

        Destroy();
      }
    }
    /// <summary>
    /// Destroy Projectile waiting destroyDelay if needed
    /// </summary>
    public virtual void Destroy() {
      UpdateManager.SetTimeout(DestroyNow, destroyDelay);
    }
    /// <summary>
    /// Real destroy method
    /// </summary>
    public void DestroyNow() {
      UpdateManager.Remove(this);
      Destroy (gameObject);
    }
  }
}
