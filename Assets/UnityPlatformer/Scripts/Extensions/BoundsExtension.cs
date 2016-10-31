using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// UnityEngine.Bounds
  /// </summary>
  public static class BoundsExtension {
    /// <summary>
    /// Draw the bounds using Debug.DrawRay
    /// </summary>
    static public void Draw(this Bounds bounds, Transform transform, Color? color = null) {
      Color c = color ?? Color.white;
      Vector3 bc = bounds.center;//transform.position + transform.rotation * (bounds.center - transform.position);
      Vector3 bextends = bounds.extents;

      //TODO quat is necesary ?
      // Quaternion quat = transform.rotation;

      Vector3 topFrontRight = bc + Vector3.Scale(bextends, new Vector3(1, 1, 1));
      Vector3 topFrontLeft = bc + Vector3.Scale(bextends, new Vector3(-1, 1, 1));
      Vector3 topBackLeft = bc + Vector3.Scale(bextends, new Vector3(-1, 1, -1));
      Vector3 topBackRight = bc + Vector3.Scale(bextends, new Vector3(1, 1, -1));
      Vector3 bottomFrontRight = bc + Vector3.Scale(bextends, new Vector3(1, -1, 1));
      Vector3 bottomFrontLeft = bc + Vector3.Scale(bextends, new Vector3(-1, -1, 1));
      Vector3 bottomBackLeft = bc + Vector3.Scale(bextends, new Vector3(-1, -1, -1));
      Vector3 bottomBackRight = bc + Vector3.Scale(bextends, new Vector3(1, -1, -1));

      Vector3[] corners = new Vector3[] {
        topFrontRight,
        topFrontLeft,
        topBackLeft,
        topBackRight,
        bottomFrontRight,
        bottomFrontLeft,
        bottomBackLeft,
        bottomBackRight
      };

      int i1;
      int i2;

      for (int i = 0; i < 4; i++) {
        //top rectangle
        i1 = (i+1)%4;
        Debug.DrawRay(corners[i], corners[i1] - corners[i], c);

        //vertical lines
        i1 = i + 4;
        Debug.DrawRay(corners[i], corners[i1] - corners[i], c);

        //bottom rectangle
        i2 = 4 + (i+1)%4;
        Debug.DrawRay(corners[i1], corners[i2] - corners[i1], c);
      }
    }
  }
}
