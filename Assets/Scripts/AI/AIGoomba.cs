using System;
using UnityEngine;

namespace UnityPlatformer {
  ///<summary>
  /// Patrol Artificial inteligence.
  /// NOTE does not require Actions but it's recommended:
  /// CharacterActionAirMovement and/or CharacterActionGroundMovement
  ///</summary>
  [RequireComponent (typeof (Enemy))]
  public class AIGoomba: Enemy {
    #region public

    public enum Facing {
      Left = -1,
      Right = 1
    };

    public Facing initialFacing;
    [Comment("Distance to test if ground is on left/right side. Helps when Enemy standing on platform moving down.")]
    public float rayLengthFactor = 1.0f;
    [Comment("Do not fall on platform edge. Go back.")]
    public bool doNotFall = true;

    #endregion

    #region private

    Facing facing;

    #endregion

    public override void Start() {
      base.Start();

      controller.collisions.onLeftWall += OnLeftWall;
      controller.collisions.onRightWall += OnRightWall;

      facing = initialFacing;
    }

    void OnLeftWall() {
      facing = Facing.Right;
      input.SetX((float) facing);
    }

    void OnRightWall() {
      facing = Facing.Left;
      input.SetX((float) facing);
    }

    void Toogle() {
      facing = facing == Facing.Left ? Facing.Right : Facing.Left;
      input.SetX((float) facing);
    }

    public override void ManagedUpdate(float delta) {
      if (doNotFall) {
        if (!controller.IsGroundOnLeft (rayLengthFactor)) {
          OnLeftWall ();
        } else if (!controller.IsGroundOnRight (rayLengthFactor)) {
          OnRightWall ();
        }
      }

      base.ManagedUpdate(delta);
    }
  }
}
