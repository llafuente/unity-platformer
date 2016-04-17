using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils {
  public class Rotate : MonoBehaviour {
    [Comment("In Degrees")]
    public Vector3 rotationPerSecond;

    void FixedUpdate() {
      transform.Rotate(rotationPerSecond * Time.fixedDeltaTime, Space.Self);
    }
  }
}
