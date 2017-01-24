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
      // force trigger, dinamic and never sleep
      Collider2D col2d = gameObject.GetComponent<Collider2D>();
      Assert.IsNotNull(col2d, "(Utils) Missing Monobehaviour Collider2D at " + gameObject.GetFullName());
      col2d.isTrigger = true;
      Rigidbody2D rb2d = gameObject.GetComponent<Rigidbody2D>();
      Assert.IsNotNull(col2d, "(Utils) Missing Monobehaviour Rigidbody2D at " + gameObject.GetFullName());
      rb2d.bodyType = RigidbodyType2D.Dynamic;
      rb2d.sleepMode = RigidbodySleepMode2D.NeverSleep;
      rb2d.gravityScale = 0.0f;
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
        } else {
          CircleCollider2D circle2d = obj.GetComponent<CircleCollider2D>();
          if (circle2d != null) {
            Gizmos.DrawWireSphere((Vector3)circle2d.offset, circle2d.radius);
          } // TODO handle the rest
        }
    }
    #endif
  }
}
