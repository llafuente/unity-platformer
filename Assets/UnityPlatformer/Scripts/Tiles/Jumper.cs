using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

namespace UnityPlatformer {
  /// <summary>
  /// Tile that force Character to Jump!
  /// </summary>
  [RequireComponent (typeof (Rigidbody2D))]
  public class Jumper : Physhic2DMonoBehaviour {
    /// <summary>
    /// Jump properties
    /// </summary>
    public JumpConstantSpringProperties jumpProperties;
    /// <summary>
    /// force Collider2D to be trigger using: Utils.KinematicTrigger
    /// </summary>
    public override void Start() {
      base.Start();
      Utils.DynamicTrigger(gameObject);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Set Collider2D to be trigger using: Utils.KinematicTrigger
    /// </summary>
    public virtual void Reset() {
      Utils.DynamicTrigger(gameObject);
    }
#endif

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
      Character p = Utils.SmartGetCharacter(o.gameObject);
      if (p != null) {
        StartJump(p);
      }
    }
  }
}
