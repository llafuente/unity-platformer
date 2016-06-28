using UnityEngine;

namespace UnityPlatformer {
  public static class LayerMaskExtension {
    static public bool Contains(this LayerMask lm, int l) {
      return ((0x1 << l) & lm) != 0;
    }
  }
}
