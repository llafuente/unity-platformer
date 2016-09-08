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

    internal Facing facing;

    override public void Start() {
      base.Start();

      pc2d.onLeftWall += OnLeftWall;
      pc2d.onRightWall += OnRightWall;

      facing = initialFacing;
      input.SetX((float) facing);
    }

    virtual public void OnLeftWall() {
      facing = Facing.Right;
      input.SetX((float) facing);
    }

    virtual public void OnRightWall() {
      facing = Facing.Left;
      input.SetX((float) facing);
    }

    virtual public void Toogle() {
      facing = facing == Facing.Left ? Facing.Right : Facing.Left;
      input.SetX((float) facing);
    }

    public override void PlatformerUpdate(float delta) {
      if (doNotFall && pc2d.collisions.below) {
        if (!IsGroundOnLeft (rayLengthFactor, delta)) {
          OnLeftWall ();
        } else if (!IsGroundOnRight (rayLengthFactor, delta)) {
          OnRightWall ();
        }
      }

      base.PlatformerUpdate(delta);
    }
  }
}
