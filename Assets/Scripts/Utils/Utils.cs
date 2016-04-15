using System;
using UnityEngine;

namespace UnityPlatformer {
  public class Utils {

    static public bool layermask_contains(LayerMask lm, int l) {
      return ((0x1 << l) & lm) != 0;
    }

    static public bool biton(int a, int b) {
      return (a & b) == b;
    }
    static public void DrawPoint(Vector3 point, float extend = 0.5f, Color? color = null) {
      Color c = color ?? Color.white;

      Debug.DrawRay(
        point,
        new Vector3(extend, extend, 0),
        c
      );
      Debug.DrawRay(
        point,
        new Vector3(-extend, extend, 0),
        c
      );
      Debug.DrawRay(
        point,
        new Vector3(-extend, -extend, 0),
        c
      );
      Debug.DrawRay(
        point,
        new Vector3(extend, -extend, 0),
        c
      );
    }
  }
}
