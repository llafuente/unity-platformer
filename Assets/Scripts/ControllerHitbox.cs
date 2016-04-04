using UnityEngine;
using UnityEngine.UI;

public class ControllerHitbox : MonoBehaviour {
  public LayerMask collideWith;
  public AliveEntity notifyTo;
  public int damageDealt = 1;

  void OnTriggerEnter2D(Collider2D o) {
    Debug.Log(this.name + " collide with: " + o.gameObject);
		if (utils.layermask_contains(collideWith, o.gameObject.layer)) {
			var dst = o.gameObject.GetComponent<ControllerHitbox> ();
			notifyTo.Damage (dst.damageDealt, dst);
		}
  }

	void OnTriggerStay2D(Collider2D o) {
		// TODO handle something
	}

	public void OnTriggerExit2D(Collider2D o) {
		// TODO handle something
	}
}
