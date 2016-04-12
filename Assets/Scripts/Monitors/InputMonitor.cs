using UnityEngine;
using UnityEngine.UI;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Monitors {
  [RequireComponent (typeof (PlatformerInput))]
  [RequireComponent (typeof (Character))]
  public class InputMonitor : MonoBehaviour {
    public int length = 2;
    PlatformerInput input;
    Character character;

    public void Start() {
		    input = GetComponent<PlatformerInput> ();
		    character = GetComponent<Character> ();
    }

  	public void Update() {
      Debug.DrawRay(
        character.transform.position,
        (Vector3)(input.GetAxisRaw() * length),
        Color.yellow);
    }
  }
}
