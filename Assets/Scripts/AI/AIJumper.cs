using System;
using UnityEngine;

namespace UnityPlatformer {
  ///<summary>
  /// Jump until obstacle.
  /// TODO follow player (turn always)
  /// TODO jump delay
  /// TODO can turn during jump
  /// NOTE if you want the Jumper to move on ground add: CharacterActionGroundMovement
  ///</summary>
  public class AIJumper: Enemy {
    #region public

    public enum Facing {
      Left = -1,
      Right = 1
    };

    public Facing initialFacing;
    [Comment("Distance to test if ground is on left/right side. Helps when Enemy standing on platform moving down.")]
    public float rayLengthFactor = 1.0f;

    #endregion

    #region private

    Facing facing;

    #endregion

    public void Start() {
      input.EnableAction("Jump");
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
      base.ManagedUpdate(delta);
    }
  }
}
