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
  }
}
