using UnityEngine;
using System.Collections.Generic;

namespace UnityPlatformer {
  public static class TransformExtension {
    static public void DestroyImmediateChildren(this Transform transform) {
      var children = new List<GameObject>();
      foreach (Transform child in transform) children.Add(child.gameObject);
      children.ForEach(child => GameObject.DestroyImmediate(child));
      //foreach (var child in children) {
      //  transform.DestroyImmediate(child);
      //}
    }

    static public void DestroyChildren(this Transform transform) {
      var children = new List<GameObject>();
      foreach (Transform child in transform) children.Add(child.gameObject);
      children.ForEach(child => GameObject.Destroy(child));
    }
  }
}
