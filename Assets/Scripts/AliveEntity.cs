using System;
using UnityEngine;

public class AliveEntity: MonoBehaviour
{
  public int heath = 1;

  virtual public void ManagedUpdate(float delta) {
  }

  virtual public void Damage(int amount = 1, ControllerHitbox hitb = null) {
    Debug.Log(this.name + " recieve damage " + amount);
    heath -= amount;
    if (heath <= 0) {
      Die();
    }
  }

  virtual public void Heal(int amount = 1) {
    heath += amount;
  }

  // TODO
  virtual public void Die() {
	var l = GetComponentsInChildren<ControllerHitbox> ();
    foreach (var x in l) {
	  x.gameObject.SetActive(false);
    }
    //Destroy(gameObject);
  }

}
