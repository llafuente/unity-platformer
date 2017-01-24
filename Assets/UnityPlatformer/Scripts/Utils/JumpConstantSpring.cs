using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Serializable class to configure JumpConstantSpring in the Editor
  /// </summary>
  [Serializable]
  public class JumpConstantSpringProperties {
    /// <summary>
    /// Initial velocity
    /// </summary>
    public Vector2 initialVelocity = new Vector2(0, 20);
    /// <summary>
    /// Penetration in units
    /// </summary>
    public float penetration = 0.5f;
    /// <summary>
    /// Penetrate if speed is greater than
    /// </summary>
    public float minPenetrationSpeed = 3;
    /// <summary>
    /// Constructor
    /// </summary>
    public JumpConstantSpringProperties(Vector2 ivel, float pen, float mpenvel) {
      initialVelocity = ivel;
      penetration = pen;
      minPenetrationSpeed = mpenvel;
    }
    /// <summary>
    /// Clone
    /// </summary>
    public JumpConstantSpringProperties Clone(int faceDir) {
      return new JumpConstantSpringProperties(initialVelocity, penetration, minPenetrationSpeed);
    }
  };

  /// <summary>
  /// Math behind the Jump
  /// </summary>
  public class JumpConstantSpring : Jump {
    /// <summary>
    /// Initial velocity
    /// </summary>
    public Vector2 initialVelocity;
    /// <summary>
    /// Penetration in units
    /// </summary>
    public float penetration = 0.5f;
    /// <summary>
    /// Penetrate if speed is greater than
    /// </summary>
    public float minPenetrationSpeed = 3;

    float currentDeceleration = 0;
    float initialYPos = 0;
    /// <summary>
    /// Constructor
    /// </summary>
    public JumpConstantSpring(Character _character, Vector2 _initialVelocity, float _penetration, float _minPenetrationSpeed) {
      character = _character;
      initialVelocity = _initialVelocity;
      penetration = _penetration;
      minPenetrationSpeed = _minPenetrationSpeed;
    }
    /// <summary>
    /// Constructor
    /// </summary>
    public JumpConstantSpring(Character _character, JumpConstantSpringProperties jp) {
      character = _character;
      initialVelocity = jp.initialVelocity;
      penetration = jp.penetration;
      minPenetrationSpeed = jp.minPenetrationSpeed;
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
          velocity = initialVelocity;
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
