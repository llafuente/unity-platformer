using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  public class Box : MonoBehaviour/*, IUpdateEntity*/ {
    /// <summary>
    /// Real Body, what is going to be moved
    /// </summary>
    public PlatformerCollider2D collider;
    /*
    public virtual void OnEnable() {
      UpdateManager.instance.others.Add(this);
    }

    public virtual void OnDisable() {
      UpdateManager.instance.others.Remove(this);
    }

    Vector3 velocity = Vector3.zero;
    public virtual void ManagedUpdate(float delta) {
      velocity.y += collider.gravity.y * delta;
      Debug.Log(velocity.ToString("F4"));
      collider.Move(velocity);
      if (collider.collisions.above || collider.collisions.below) {
        velocity.y = 0;
      }
    }
    */
  }
}
