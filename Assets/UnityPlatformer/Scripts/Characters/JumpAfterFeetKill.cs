using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  class JumpAfterFeetKill : MonoBehaviour {
    Character character;

    //[Comment("Jump with no direction pressed.")]
    public JumpConstantProperties jumpProperties = new JumpConstantProperties(new Vector2(0, 20));

    CharacterActionJump actionJump;

    void OnEnable() {
      character = GetComponent<Character>();
      character.onHurtCharacter += OnHurtCharacter;
    }

    void Start() {
      actionJump = character.GetAction<CharacterActionJump>();
    }

    void OnHurtCharacter(DamageType dt, Health h, Character to) {
      actionJump.ForceJump(new JumpConstant(character,
        jumpProperties.Clone((int) character.faceDir)
      ));
    }
  }
}
