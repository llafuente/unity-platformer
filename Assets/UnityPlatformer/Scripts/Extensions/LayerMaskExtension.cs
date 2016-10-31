using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// UnityEngine.LayerMask
  /// </summary>
  public static class LayerMaskExtension {
    /// <summary>
    /// LayerMask contains given layer?
    /// </summary>
    static public bool Contains(this LayerMask lm, int l) {
      return ((0x1 << l) & lm) != 0;
    }
  }
}
