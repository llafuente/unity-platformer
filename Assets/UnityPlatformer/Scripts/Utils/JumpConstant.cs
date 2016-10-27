using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Serializable class to configure JumpConstant in the Editor
  /// </summary>
  [Serializable]
  public class JumpConstantProperties {
    /// <summary>
    /// Initial velocity
    /// </summary>
    public Vector2 initialVelocity = new Vector2(10, 0);

    /// <summary>
    /// Constructor
    /// </summary>
    public JumpConstantProperties(Vector2 iv) {
      initialVelocity = iv;
    }
    /// <summary>
    /// Clone
    /// </summary>
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
    /// <summary>
    /// Initial velocity
    /// </summary>
    public Vector2 initialVelocity;

    /// <summary>
    /// Constructor
    /// </summary>
    public JumpConstant(Character _character, Vector2 _initialVelocity) {
      character = _character;
      initialVelocity = _initialVelocity;
    }
    /// <summary>
    /// Constructor
    /// </summary>
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

    public override bool Jumping(ref Vector3 velocity, float delta) {
      base.Jumping(ref velocity, delta);
      // jumps frames

      if (velocity.y > 0) { // TODO REVIEW is faked atm! should be IsBeforeApex()
        // jumping
        return true;
      }

      return false;
    }
  }
}
