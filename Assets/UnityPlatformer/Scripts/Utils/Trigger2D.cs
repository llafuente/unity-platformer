using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Abstract class that contains a ITriggerAble
  /// </summary>
  public abstract class Trigger2D : MonoBehaviour {
    /// <summary>
    /// Allowed mask, if something in the mask collide, trigger will fire!
    /// </summary>
    public LayerMask collisionMask;
    /// <summary>
    /// callback type
    /// </summary>
    public delegate void OnTrigger(Collider2D o);
    /// <summary>
    /// callback when OnTriggerEnter2D is called with a object that match collisionMask
    /// </summary>
    public OnTrigger onTriggerEnter2D;
    /// <summary>
    /// callback when OnTriggerExit2D is called with a object that match collisionMask
    /// </summary>
    public OnTrigger onTriggerExit2D;
    /// <summary>
    /// callback when OnTriggerStay2D is called with a object that match collisionMask
    /// </summary>
    public OnTrigger onTriggerStay2D;
    /// <summary>
    /// Object
    /// </summary>
    public ITriggerAble obj;

    void OnTriggerEnter2D(Collider2D o) {
      //Debug.Log("OnTriggerEnter2D " + o.gameObject.name);
      if (onTriggerEnter2D != null && collisionMask.Contains(o.gameObject.layer)) {
        onTriggerEnter2D(o);
      }
    }

    void OnTriggerExit2D(Collider2D o) {
      //Debug.Log("OnTriggerExit2D " + o.gameObject.name);

      if (onTriggerExit2D != null && collisionMask.Contains(o.gameObject.layer)) {
        onTriggerExit2D(o);
      }
    }

    void OnTriggerStay2D(Collider2D o) {
      //Debug.Log("OnTriggerStay2D " + o.gameObject.name);

      if (onTriggerStay2D != null && collisionMask.Contains(o.gameObject.layer)) {
        onTriggerStay2D(o);
      }
    }
  }
}
