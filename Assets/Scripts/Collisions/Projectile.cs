using UnityEngine;
using System;
using System.Collections;
using UnityPlatformer.Characters;

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
    [Comment("Destroy after hit")]
    public float destroyDelay;

    //
    // Actions
    //
    public Action onHit;
    public Action onDestroy;

    #endregion

    [HideInInspector]
    public bool startActive = false;

    // hit something "Damage-able"
    bool hit = false;

    void Awake() {
      Debug.Log("AWAKE projectile!");
      gameObject.SetActive(startActive); // disable
    }

    // factory
    public GameObject Fire(Vector3 position) {
			// TODO REVIEW WTF! all 4 lines are necessary?!
			gameObject.SetActive(true);
      GameObject obj = (GameObject) Instantiate(gameObject, position, Quaternion.identity);
			gameObject.SetActive(false);
			obj.SetActive(true);

			Projectile prj = obj.GetComponent<Projectile>();
			if (prj == null) {
				Debug.LogWarning("Cloned object does not have Projectile?!");
				return null;
			}
			//Debug.Log("cloned" + obj);

      UpdateManager.projectiles.Add(prj);
      return obj;
    }

    public void ManagedUpdate(float delta) {
      velocity.y += gravity * delta;
      transform.position += (Vector3)velocity * delta;
    }

    void OnTriggerEnter2D(Collider2D o) {
      //Debug.Log(this.name + " collide with: " + o.gameObject + "@" + o.gameObject.layer);
  		if (Utils.layermask_contains(collisionMask, o.gameObject.layer)) {
  			var dst = o.gameObject.GetComponent<Hitbox> ();
        if (dst == null) {
          //Debug.LogWarning("Destroy projectile");
        } else {
          hit = true;
          //Debug.Log("Projectile hit something, deal damage!");
  	      dst.owner.Damage (GetComponent<DamageType>());
        }
				//Debug.Log("Dispose !!!!!!!!");
				StartCoroutine(Dispose());

  		}
    }

  	void OnTriggerStay2D(Collider2D o) {
  		// TODO handle something
  	}

  	public void OnTriggerExit2D(Collider2D o) {
  		// TODO handle something
  	}

    virtual protected IEnumerator Dispose() {
			Debug.Log("Dispose" + gameObject);
      yield return new WaitForSeconds(destroyDelay);
			UpdateManager.projectiles.Remove(this);
      Destroy (gameObject);
    }
  }
}
