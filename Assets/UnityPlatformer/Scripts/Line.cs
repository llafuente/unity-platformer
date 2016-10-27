using UnityEngine;
using System.Collections.Generic;

namespace UnityPlatformer {
  /// <summary>
  /// It's a 3D Line.
  /// Include a WYSIWYG Editor: LineInspector.
  /// Line is used by MovingPlatform
  /// </summary>
  public class Line : MonoBehaviour {
    public Vector3[] points = new Vector3[2] {
      new Vector3(0, 0, 0),
      new Vector3(1, 0, 0)
    };
  }
}
