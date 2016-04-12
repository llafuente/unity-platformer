using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Base class for: Players, NPCs, enemies.
  /// </summary>
  [RequireComponent (typeof (Controller2D))]
  [RequireComponent (typeof (CharacterHealth))]
  public class Character: MonoBehaviour, UpdateEntity {
    CharacterAction[] actions;

    // TODO REVIEW make a decision about it, calc from jump, make it public
    float gravity = -50;

    public enum States
		{
			None = 0,             // 0000000
			OnGround = 1,         // 0000001
			OnMovingPlatform = 3, // 0000011
			OnSlope = 5,          // 0000100
			Jumping = 8,          // 0001000
			Falling = 16,         // 0010000
			FallingFast = 48,     // 0110000
			Ladder = 64,          // 1000000
			//WallSliding,
			//WallSticking,
			//Dashing,
			//Frozen,
			//Slipping,
			//FreedomState
		}
		public States state = States.None;

		public enum Areas
		{
			None = 0x0,
			Ladder = 0x01
		}
		public Areas area = Areas.None;

    //
    // ~private
    //
    [HideInInspector]
    public float ladderCenter;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Controller2D controller;
    [HideInInspector]
    public CharacterHealth health;

    /// <summary>
    /// This method precalculate some vars, but those value could change. This need to be refactored.
    /// Maybe setters are the appropiate method to refactor this.
    /// </summary>
    virtual public void Start() {
      controller = GetComponent<Controller2D> ();
      actions = GetComponents<CharacterAction>();
      health = GetComponent<CharacterHealth>();

      health.onDeath += OnDeath;
    }

    public void Attach(UpdateManager um) {
      // TODO HACK WIP
      GetComponent<CharacterActionJump>().Attach(um);
    }

    /// <summary>
    /// Managed update called by UpdateManager
    /// Transform Input into platformer magic :)
    /// </summary>
    virtual public void ManagedUpdate(float delta) {
      int prio = 0;
      int tmp;
      CharacterAction action = null;

      foreach (var i in actions) {
        tmp = i.WantsToUpdate();
        if (tmp < 0) {
          i.PerformAction(Time.fixedDeltaTime);
        } else if (prio < tmp) {
          prio = tmp;
          action = i;
        }
      }

      // reset / defaults
      controller.disableWorldCollisions = false;
      PostUpdateActions a = PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;

      if (action != null) {
        action.PerformAction(Time.fixedDeltaTime);
        a = action.GetPostUpdateActions();
      }

      if (utils.biton((int)a, (int)PostUpdateActions.APPLY_GRAVITY)) {
        velocity.y += gravity * delta;
      }

      if (!utils.biton((int)a, (int)PostUpdateActions.WORLD_COLLISIONS)) {
        controller.disableWorldCollisions = true;
      }

      controller.Move(velocity * delta, GetComponent<PlatformerController>().GetAxisRaw());

      // this is meant to fix jump and falling hit something unexpected
      if (controller.collisions.above || controller.collisions.below) {
        velocity.y = 0;
      }
    }

    public bool IsOnState(States _state) {
      return (state & _state) == _state;
    }

    public bool IsOnLadder() {
      return (area & Areas.Ladder) == Areas.Ladder;
    }

    public void EnterLadderArea(Bounds b) {
      area |= Areas.Ladder;
      ladderCenter = b.center.x;
    }

    public void ExitLadderArea(Bounds b) {
      area &= ~Areas.Ladder;
      if (IsOnState (States.Ladder)) {
        state &= ~States.Ladder;
      }
    }

    virtual public void OnDeath() {
      Debug.Log("Player die! play some fancy animation!");
    }
  }
}
