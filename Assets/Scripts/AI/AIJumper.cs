using System;
using UnityEngine;
using UnityPlatformer.Characters;
using UnityPlatformer.Actions;

namespace UnityPlatformer.AI {
  ///<summary>
  /// Jump until obstacle.
  /// TODO follow player (turn always)
  /// TODO jump delay
  /// TODO can turn during jump
  /// NOTE if you want the Jumper to move on ground add: CharacterActionGroundMovement
  ///</summary>
  [RequireComponent (typeof (CharacterActionAirMovement))]
  [RequireComponent (typeof (CharacterActionJump))]
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

    public override void Start() {
      base.Start();

      input.EnableAction("Jump");
      controller.collisions.onLeftWall += OnLeftWall;
      controller.collisions.onRightWall += OnRightWall;

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
