using System;
using UnityEngine;

namespace UnityPlatformer {
  [Serializable]
  public class JumpConstantSpringProperties {
    public float initialVelocity = 20;
    public float penetration = 0.5f;
    public float minPenetrationSpeed = 3;

    public JumpConstantSpringProperties(float ivel, float pen, float mpenvel) {
      initialVelocity = ivel;
      penetration = pen;
      minPenetrationSpeed = mpenvel;
    }

    public JumpConstantSpringProperties Clone(int faceDir) {
      return new JumpConstantSpringProperties(initialVelocity, penetration, minPenetrationSpeed);
    }
  };

  /// <summary>
  /// Math behind the Jump
  /// </summary>
  public class JumpConstantSpring : Jump {
    public float initialVelocity;
    public float penetration = 0.5f;
    public float minPenetrationSpeed = 3;

    float currentDeceleration = 0;
    float initialYPos = 0;

    public JumpConstantSpring(Character _character, float _initialVelocity, float _penetration) {
      character = _character;
      initialVelocity = _initialVelocity;
      penetration = _penetration;
    }

    public JumpConstantSpring(Character _character, JumpConstantSpringProperties jp) {
      character = _character;
      initialVelocity = jp.initialVelocity;
      penetration = jp.penetration;
    }

    public override void StartJump(ref Vector3 velocity) {
      Reset();
      // TODO if you want 100% accuracy calc gravity here...
      velocity.y = Mathf.Min(-minPenetrationSpeed, velocity.y);
      currentDeceleration = (velocity.y * velocity.y) / (penetration * 2);
      initialYPos = character.transform.position.y;
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

    public override bool Jumping(ref Vector3 velocity, float delta) {
      base.Jumping(ref velocity, delta);

      // spring-penetration
      if (velocity.y < 0 && Math.Abs(initialYPos - character.transform.position.y) < penetration) {
        velocity.y += delta * currentDeceleration;
      } else {
        // jump
        if (currentDeceleration != 0) {
          currentDeceleration = 0;
          velocity.y = initialVelocity;
          // need to enter again, because before we were falling!
          character.EnterState(States.Jumping);
        } else if (velocity.y <= 0) {
          // jump ended
          return false;
        }
      }

      return true;
    }
  }
}
