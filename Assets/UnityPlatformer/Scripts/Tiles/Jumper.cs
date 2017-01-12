using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Tile that force Character to Jump!
  /// </summary>
  [RequireComponent (typeof (Collider2D))]
  public class Jumper : MonoBehaviour {
    /// <summary>
    /// Jump properties
    /// </summary>
    public JumpConstantSpringProperties jumpProperties;

    public virtual void Start() {
      // force trigger
      Collider2D col2d = GetComponent<Collider2D>();
      Assert.IsNotNull(col2d, "(Projectile) Missing Monobehaviour Collider2D at " + gameObject.GetFullName());
      col2d.isTrigger = true;
    }

    /// <summary>
    /// Force a player to jump!
    /// </summary>
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
    // REVIEW could the Character double-enter?
    /// <summary>
    /// If Character enter, force him to jump!
    /// </summary>
    public virtual void OnTriggerEnter2D(Collider2D o) {
      Character p = o.GetComponent<Character>();
      if (p) {
        StartJump(p);
      }
    }
  }
}
