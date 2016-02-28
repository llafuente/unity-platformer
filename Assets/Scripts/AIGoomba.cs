using System;
using UnityEngine;

public class AIGoomba: Enemy
{
	public enum Facing {
		Left = -1,
		Right = 1
	};

	public Facing initialFacing;
	public float velocityX;

	Facing facing;
	Controller2D controller;

	void Start() {
		controller = GetComponent<Controller2D> ();
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

	override public void ManagedUpdate(float delta) {
		if (!controller.IsGroundOnLeft ()) {
			OnLeftWall ();
		} else if (!controller.IsGroundOnRight ()) {
			OnRightWall ();
		}

		var v = new Vector3 (
			velocityX * delta * (int)facing,
			-5 * delta,
			0);
		controller.Move(v, false);
	}
}

