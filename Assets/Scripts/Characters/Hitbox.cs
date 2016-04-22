using UnityEngine;
using UnityEngine.UI;
namespace UnityPlatformer {
  public class Hitbox : MonoBehaviour {
    #region public

    [Comment("Who can deal damage to me?")]
    public LayerMask collideWith;
    [Comment("Who am I?")]
    public CharacterHealth owner;

    #endregion

    void OnTriggerEnter2D(Collider2D o) {
      Debug.Log(this.name + " collide with: " + o.gameObject);
  		if (Utils.layermask_contains(collideWith, o.gameObject.layer)) {
  			var dst = o.gameObject.GetComponent<DamageType> ();
        if (dst == null) {
          Debug.LogWarning("Try to damage something that is not a: DamageType. Adjust collideWith");
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
