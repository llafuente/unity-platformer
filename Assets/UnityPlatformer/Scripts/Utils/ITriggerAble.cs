using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// interface to force to implement trigger collisions
  /// </summary>
  public interface ITriggerAble {
    /// <summary>
    /// OnTriggerEnter2D
    /// </summary>
    void OnTriggerEnter2D(Collider2D o);
    /// <summary>
    /// OnTriggerExit2D
    /// </summary>
    void OnTriggerExit2D(Collider2D o);
    /// <summary>
    /// OnTriggerStay2D
    /// </summary>
    void OnTriggerStay2D(Collider2D o);
  }
}
