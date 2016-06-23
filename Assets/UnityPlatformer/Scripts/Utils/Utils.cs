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

    static public T GetOrAddComponent<T>(GameObject obj) where T : UnityEngine.Component {
      T t = obj.GetComponent<T>();

      if (t == null) {
        return (T) obj.AddComponent<T>();
      }

      return (T) t;
    }

    static public void DestroyImmediateChildren(Transform transform) {
      var children = new List<GameObject>();
      foreach (Transform child in transform) children.Add(child.gameObject);
      children.ForEach(child => GameObject.DestroyImmediate(child));
      //foreach (var child in children) {
      //  transform.DestroyImmediate(child);
      //}
    }

    static public bool layermask_contains(LayerMask lm, int l) {
      return ((0x1 << l) & lm) != 0;
    }

    static public bool biton(int a, int b) {
      return (a & b) == b;
    }

    static public void DrawZAngle(Vector3 point, float degreeAngle = 0.5f, Color? color = null) {
      Debug.DrawRay(point, new Vector2(
        Mathf.Cos(degreeAngle * Mathf.Deg2Rad) * 3,
        Mathf.Sin(degreeAngle * Mathf.Deg2Rad) * 3
      ));
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
