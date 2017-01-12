using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

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
    public virtual void StartJump(Character character) {
      CharacterActionJump actionJump = character.GetAction<CharacterActionJump>();

      Assert.IsNotNull(actionJump, "(Jumper) Missing Monobehaviour CharacterActionJump at " + character.gameObject.GetFullName());

      actionJump.ForceJump(
        new JumpConstantSpring(character, jumpProperties.Clone(1))
      );
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
