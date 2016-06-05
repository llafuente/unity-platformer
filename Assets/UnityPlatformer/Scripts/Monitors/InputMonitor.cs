using UnityEngine;
using UnityEngine.UI;

namespace UnityPlatformer {
  public class InputMonitor : Monitor {
    public int length = 2;
    public PlatformerInput input;
    public Character character;

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
