using UnityEngine;
using UnityEngine.UI;

namespace UnityPlatformer {
  /// <summary>
  /// Display a line in the character position with the input (Axis)
  /// </summary>
  public class InputMonitor : Monitor {
    /// <summary>
    /// Line length
    /// </summary>
    public int length = 2;
    /// <summary>
    /// Input to listen
    /// </summary>
    public PlatformerInput input;
    /// <summary>
    /// Character where display the line
    /// </summary>
    public Character character;
    /// <summary>
    /// On every update draw the line
    /// </summary>
    public void Update() {
      text = string.Format(
        "Axis: {0}",
        input.GetAxisRaw()
      );

      Debug.DrawRay(
        character.transform.position,
        (Vector3)(input.GetAxisRaw() * length),
        Color.yellow);
    }
  }
}
