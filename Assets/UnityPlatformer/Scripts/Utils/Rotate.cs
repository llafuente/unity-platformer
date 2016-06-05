using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityPlatformer {
  /// <summary>
  /// Rotate current object
  /// </summary>
  public class Rotate : MonoBehaviour {
    [Comment("In Degrees")]
    public Vector3 rotationPerSecond;

    void FixedUpdate() {
      transform.Rotate(rotationPerSecond * Time.fixedDeltaTime, Space.Self);
    }
  }
}
