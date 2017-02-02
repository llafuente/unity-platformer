using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityPlatformer {
  /// <summary>
  /// Utils staff
  /// </summary>
  public static class Utils {
    static public void DynamicTrigger(GameObject gameObject) {
      // force trigger, dinamic and never sleep, so stay will be called every frame
      Collider2D col2d = gameObject.GetComponent<Collider2D>();
      Assert.IsNotNull(col2d, "(Utils) Missing Monobehaviour Collider2D at " + gameObject.GetFullName());
      col2d.isTrigger = true;
      Rigidbody2D rb2d = gameObject.GetOrAddComponent<Rigidbody2D>();
      Assert.IsNotNull(col2d, "(Utils) Missing Monobehaviour Rigidbody2D at " + gameObject.GetFullName());
      rb2d.bodyType = RigidbodyType2D.Dynamic;
      rb2d.sleepMode = RigidbodySleepMode2D.NeverSleep;
      rb2d.gravityScale = 0.0f;
    }

    static public void KinematicTrigger(GameObject gameObject) {
      // force trigger, dinamic and never sleep, so stay will be called every frame
      Collider2D col2d = gameObject.GetComponent<Collider2D>();
      Assert.IsNotNull(col2d, "(Utils) Missing Monobehaviour Collider2D at " + gameObject.GetFullName());
      col2d.isTrigger = true;
      Rigidbody2D rb2d = gameObject.GetOrAddComponent<Rigidbody2D>();
      Assert.IsNotNull(col2d, "(Utils) Missing Monobehaviour Rigidbody2D at " + gameObject.GetFullName());
      rb2d.bodyType = RigidbodyType2D.Kinematic;
      rb2d.sleepMode = RigidbodySleepMode2D.NeverSleep;
      rb2d.gravityScale = 0.0f;
    }
    static public void SetDebugCollider2D(GameObject obj, string color) {
      Collider2DRenderer ren = obj.GetOrAddComponent<Collider2DRenderer>();
      ren.material = Resources.Load("Materials/Green", typeof(Material)) as Material;
    }
    #if UNITY_EDITOR
    /// <summary>
    /// Draw on editor
    /// </summary>
    static public void DrawCollider2D(GameObject obj) {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(
          obj.transform.position,
          obj.transform.rotation,
           obj.transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        BoxCollider2D box2d = obj.GetComponent<BoxCollider2D>();
        if (box2d != null) {
          Gizmos.DrawWireCube((Vector3)box2d.offset, box2d.size);
          return;
        }

        CircleCollider2D circle2d = obj.GetComponent<CircleCollider2D>();
        if (circle2d != null) {
          Gizmos.DrawWireSphere((Vector3)circle2d.offset, circle2d.radius);
          return;
        }

        PolygonCollider2D poly2d = obj.GetComponent<PolygonCollider2D>();
        if (poly2d != null) {
          //Gizmos.DrawWireSphere((Vector3)circle2d.offset, circle2d.radius);
          Vector2[] vertices = poly2d.points;
          int len = vertices.Length;
          for (int i = 0; i < len; i++) {
            Gizmos.DrawLine(
              vertices[i],
              i == len - 1 ? vertices[0] : vertices[i + 1]
            );
          }
          return;
        }

        // TODO handle the rest

        Gizmos.matrix = Matrix4x4.identity;
    }
    #endif

    static public Character SmartGetCharacter(GameObject obj) {
      Character p = obj.GetComponent<Character>();
      if (p != null) {
        return p;
      }

      HitBox hb = obj.GetComponent<HitBox>();
      if (hb != null) {
        return hb.owner.character;
      }

      return null;
    }
  }
}
