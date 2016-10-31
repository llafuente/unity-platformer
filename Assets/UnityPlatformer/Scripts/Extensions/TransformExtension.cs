using UnityEngine;
using System.Collections.Generic;

namespace UnityPlatformer {
  /// <summary>
  /// UnityEngine.Transform
  /// </summary>
  public static class TransformExtension {
    /// <summary>
    /// DestroyImmediate all childrens
    /// </summary>
    static public void DestroyImmediateChildren(this Transform transform) {
      var children = new List<GameObject>();
      foreach (Transform child in transform) children.Add(child.gameObject);
      children.ForEach(child => GameObject.DestroyImmediate(child));
    }
    /// <summary>
    /// Destroy all childrens
    /// </summary>
    static public void DestroyChildren(this Transform transform) {
      var children = new List<GameObject>();
      foreach (Transform child in transform) children.Add(child.gameObject);
      children.ForEach(child => GameObject.Destroy(child));
    }
  }
}
