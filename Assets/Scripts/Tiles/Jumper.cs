using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  [RequireComponent (typeof (BoxTrigger2D))]
  public class Jumper : MonoBehaviour {
    public JumpConstantSpringProperties jumpProperties;

    public virtual void StartJump(Character c) {
      // search CharacterActionJump
      CharacterActionJump actionJump = null;

      foreach (var x in c.actions) {
        if (x is CharacterActionJump) {
          actionJump = x as CharacterActionJump;
        }
      }

      if (actionJump != null) {
        actionJump.ForceJump(
          new JumpConstantSpring(c, jumpProperties.Clone(1))
        );
      } else {
        Debug.LogWarning("character found without CharacterActionJump so ignore.");
      }
    }

    public virtual void OnTriggerEnter2D(Collider2D o) {
      Character p = o.GetComponent<Character>();
      if (p) {
        StartJump(p);
      }
    }

    public virtual void OnTriggerStay2D(Collider2D o) {
    }

    public virtual void OnTriggerExit2D(Collider2D o) {
    }
  }
}
