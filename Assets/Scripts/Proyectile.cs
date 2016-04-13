using System;
using UnityEngine;

namespace UnityPlatformer {
  public class Proyectile: MonoBehaviour {
    /*
    public LayerMask DamageTo;
    public LayerMask DestroyWhenCollideWith;
    public int damageAmount = 1;

    void OnTriggerEnter2D(Collider2D o) {
      Debug.Log(this.name + " collide with: " + o.gameObject);

      if (Utils.layermask_contains(DamageTo, o.gameObject.layer)) {
        var dst = o.gameObject.GetComponent<CharacterHealth> ();
        if (dst == null) {
          Debug.LogWarning("Try to damage something that is not a: CharacterHealth");
        } else {
  	       dst.Damage (damageAmount, this);
        }
  		}

  		if (Utils.layermask_contains(DestroyWhenCollideWith, o.gameObject.layer)) {
  			Destroy(this);
  		}
    }

  	void OnTriggerStay2D(Collider2D o) {
  		// TODO handle something
  	}

  	public void OnTriggerExit2D(Collider2D o) {
  		// TODO handle something
  	}
    */
  }
}
