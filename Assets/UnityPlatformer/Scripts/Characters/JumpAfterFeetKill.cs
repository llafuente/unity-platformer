using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// When hurt another Character jump a little!
  /// </summary>
  [RequireComponent (typeof (CharacterHealth))]
  [RequireComponent (typeof (Character))]
  class JumpAfterFeetKill : MonoBehaviour {
    /// <summary>
    /// Jump properties
    /// </summary>
    public JumpConstantProperties jumpProperties = new JumpConstantProperties(new Vector2(0, 20));

    CharacterHealth cHealth;
    Character character;
    CharacterActionJump actionJump;

    void OnEnable() {
      cHealth = GetComponent<CharacterHealth>();
      character = GetComponent<Character>();
      cHealth.onHurt += OnHurtCharacter;
    }

    void Start() {
      actionJump = character.GetAction<CharacterActionJump>();
    }

    void OnHurtCharacter(Damage dt, CharacterHealth to) {
      actionJump.ForceJump(new JumpConstant(character,
        jumpProperties.Clone((int) character.faceDir)
      ));
    }
  }
}
