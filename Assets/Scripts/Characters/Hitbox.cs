using UnityEngine;
using UnityEngine.UI;
namespace UnityPlatformer.Characters {
  public class Hitbox : MonoBehaviour {
    public LayerMask collideWith;
    public CharacterHealth owner;

    void OnTriggerEnter2D(Collider2D o) {
      Debug.Log(this.name + " collide with: " + o.gameObject);
  		if (Utils.layermask_contains(collideWith, o.gameObject.layer)) {
  			var dst = o.gameObject.GetComponent<DamageType> ();
        if (dst == null) {
          Debug.LogWarning("Try to damage something that is not a: DamageType");
        } else {
  	       owner.Damage (dst);
        }
  		}
    }

  	void OnTriggerStay2D(Collider2D o) {
  		// TODO handle something
  	}

  	public void OnTriggerExit2D(Collider2D o) {
  		// TODO handle something
  	}
  }
}
