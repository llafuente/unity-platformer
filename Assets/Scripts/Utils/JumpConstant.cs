using System;
using UnityEngine;

namespace UnityPlatformer {
  [Serializable]
  public class JumpConstantProperties {
    public Vector2 initialVelocity = new Vector2(10, 0);

    public JumpConstantProperties(Vector2 iv) {
      initialVelocity = iv;
    }

    public JumpConstantProperties Clone(int faceDir) {
      Vector2 v = initialVelocity;
      v.x *= faceDir;

      return new JumpConstantProperties(v);
    }
  };

  /// <summary>
  /// Math behind the Jump
  /// </summary>
  public class JumpConstant : Jump {
    public Vector2 initialVelocity;

    Character character;
    // TODO FIXME maxJumpHeight is not used!!!
    public JumpConstant(Character _character, Vector2 _initialVelocity) {
      character = _character;
      initialVelocity = _initialVelocity;
    }

    public JumpConstant(Character _character, JumpConstantProperties jp) {
      character = _character;
      initialVelocity = jp.initialVelocity;
    }

    public override void StartJump(ref Vector3 velocity) {
      Reset();
      velocity.x = initialVelocity.x;
      velocity.y = initialVelocity.y;
    }

    public override void EndJump(ref Vector3 velocity) {
      // do nothing
    }

    // TODO good question :D
    public override bool IsBeforeApex() {
      return true;
    }

    public override bool IsHanging() {
      return false;
    }

    public override bool Jumping(ref Vector3 velocity) {
      base.Jumping(ref velocity);
      // jumps frames

      if (velocity.y > 0) { // TODO REVIEW is faked atm! should be IsBeforeApex()
        // jumping
        return true;
      }

      return false;
    }
  }
}
