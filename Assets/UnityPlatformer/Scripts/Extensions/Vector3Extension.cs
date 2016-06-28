using UnityEngine;
using System.Collections.Generic;

namespace UnityPlatformer {
  public static class Vector3Extension {
    static public void Draw(this Vector3 point, float extend = 0.5f, Color? color = null) {
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
    static public void DrawZAngle(this Vector3 point, float degreeAngle = 0.5f, Color? color = null) {
      Debug.DrawRay(point, new Vector2(
        Mathf.Cos(degreeAngle * Mathf.Deg2Rad) * 3,
        Mathf.Sin(degreeAngle * Mathf.Deg2Rad) * 3
      ));
    }
  }
}
