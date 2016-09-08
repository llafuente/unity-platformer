using UnityEngine;
using System;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// If the projectile collide with a HitBox, deal damage
  /// Is destroyed otherwise
  /// </summary>
  [RequireComponent (typeof (Collider2D))]
  [RequireComponent (typeof (Rigidbody2D))]
  [RequireComponent (typeof (DamageType))]
  public class Projectile : MonoBehaviour, IUpdateEntity {
    #region public

    [Comment("Destroy when collide with")]
    public LayerMask collisionMask;
    [Comment("Initial velocity, will be modified over time.")]
    public Vector2 velocity;
    [Comment("Y gravity")]
    public float gravity;
    [Comment("Destroy after impact")]
    public float destroyDelay;

    //
    // Actions
    //
    public Action onImpact;
    public Action onDestroy;

    #endregion

    // impact against something "damageable"
    [HideInInspector]
    public bool impact = false;

    ///<summary>
    /// When Object is Awake, it will be automatically disabled
    /// When Fired the projectile, will be cloned and enabled
    ///</summary>
    void Awake() {
      // disable at start (so people dont need to remember)
      gameObject.SetActive(false);
    }

    ///<summary>
    /// Clone the projectile, activate, add it to UpdateManager
    /// and return :)
    ///</summary>
    // TODO REVIEW onImpact & onDestroy are not cloned with Instantiate
    public virtual Projectile Fire(Vector3 position) {
      // TODO REVIEW WTF! all 4 lines are necessary?!
      gameObject.SetActive(true);
      GameObject obj = (GameObject) Instantiate(gameObject, position, Quaternion.identity);
      gameObject.SetActive(false);
      obj.SetActive(true);

      // defensive programming
      Projectile prj = obj.GetComponent<Projectile>();
      if (prj == null) {
        Debug.LogWarning("Cloned object does not have Projectile?!");
        return null;
      }

      UpdateManager.instance.Push(prj, Configuration.instance.projectilesPriority);
      return prj;
    }

    ///<summary>
    /// Move projectile accordingly
    ///</summary>
    public virtual void PlatformerUpdate(float delta) {
      velocity.y += gravity * delta;
      transform.position += (Vector3)velocity * delta;
    }

    public virtual void LatePlatformerUpdate(float delta) {
    }

    ///<summary>
    /// When collide with something, check the mask, then check if
    /// it's damageable, and Destroy
    ///</summary>
    public virtual void OnTriggerEnter2D(Collider2D o) {
      //Debug.Log(this.name + " collide with: " + o.gameObject + "@" + o.gameObject.layer);
      if (collisionMask.Contains(o.gameObject.layer)) {
        var dst = o.gameObject.GetComponent<HitBox> ();
        if (dst == null) {
          //Debug.LogWarning("Destroy projectile");
        } else {
          impact = true;
          if (onImpact != null) {
            onImpact();
          }
          //Debug.Log("Projectile impact something, deal damage!");
          dst.owner.Damage(GetComponent<Damage>());
        }
        //Debug.Log("Destroy !!!!!!!!");
        Destroy();
      }
    }

    public virtual void OnTriggerStay2D(Collider2D o) {
      // TODO handle something
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
      // TODO handle something
    }

    ///<summary>
    /// Destroy gameObject waiting destroyDelay
    ///</summary>
    public virtual void Destroy() {
      if (destroyDelay != 0f) {
        UpdateManager.instance.SetTimeout(_Destroy, destroyDelay);
        return;
      }
      _Destroy();
    }

    protected void _Destroy() {
      UpdateManager.instance.Remove(this);
      Destroy (gameObject);
    }
  }
}
