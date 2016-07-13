using System;
using UnityEngine;

namespace UnityPlatformer {
  ///<summary>
  /// Patrol Artificial inteligence.
  /// NOTE does not require Actions but it's recommended:
  /// CharacterActionAirMovement and/or CharacterActionGroundMovement
  ///</summary>
  public class AIGoomba: Enemy {
    #region public

    public Facing initialFacing = Facing.Left;
    [Comment("Distance to test if ground is on left/right side. Helps when Enemy standing on platform moving down.")]
    public float rayLengthFactor = 1.0f;
    [Comment("Do not fall on platform edge. Go back.")]
    public bool doNotFall = true;

    #endregion

    #region private

    Facing facing;

    #endregion

    public void Start() {
      pc2d.onLeftWall += OnLeftWall;
      pc2d.onRightWall += OnRightWall;

      facing = initialFacing;
      input.SetX((float) facing);
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
        if (!IsGroundOnLeft (rayLengthFactor, delta)) {
          OnLeftWall ();
        } else if (!IsGroundOnRight (rayLengthFactor, delta)) {
          OnRightWall ();
        }
      }

      base.ManagedUpdate(delta);
    }
  }
}
