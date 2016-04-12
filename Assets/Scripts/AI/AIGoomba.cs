using System;
using UnityEngine;
using UnityPlatformer.Characters;

namespace UnityPlatformer.AI {
	/// TODO use this must use: CharacterActionGroundMovement
	public class AIGoomba: Enemy {
		public enum Facing {
			Left = -1,
			Right = 1
		};

		public Facing initialFacing;
		public float velocityX;
		// helps when Enemy standing on platform moving down.
		// increase to allow further test
		public float rayLengthFactor = 1.0f;

		Facing facing;

		public override void Start() {
			base.Start();
			controller.collisions.OnLeftWall += OnLeftWall;
			controller.collisions.OnRightWall += OnRightWall;

			facing = initialFacing;
		}

		void OnLeftWall() {
			facing = Facing.Right;
		}

		void OnRightWall() {
			facing = Facing.Left;
		}

		void Toogle() {
			facing = facing == Facing.Left ? Facing.Right : Facing.Left;
		}

		public override void ManagedUpdate(float delta) {
			if (!controller.IsGroundOnLeft (rayLengthFactor)) {
				OnLeftWall ();
			} else if (!controller.IsGroundOnRight (rayLengthFactor)) {
				OnRightWall ();
			}

			var v = new Vector3 (
				velocityX * delta * (int)facing,
				-5 * delta,
				0);
			controller.Move(v, false);
		}
	}
}
