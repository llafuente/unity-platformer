using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPlatformer {
  public class Utils {

    // this need more work to handle degrees > 360
    static public float NormalizeDegree(float deg) {
      if (deg > 180) deg -= 360;
      if (deg < -180) deg += 360;

      return deg;
    }

    static public bool biton(int a, int b) {
      return (a & b) == b;
    }
  }
}
