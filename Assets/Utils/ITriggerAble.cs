using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  public interface ITriggerAble {
    void OnTriggerEnter2D(Collider2D o);
    void OnTriggerExit2D(Collider2D o);
    void OnTriggerStay2D(Collider2D o);
  }
}
