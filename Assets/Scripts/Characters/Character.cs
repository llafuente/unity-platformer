using System;
using UnityEngine;
using UnityPlatformer.Actions;

namespace UnityPlatformer.Characters {
  /// <summary>
  /// Base class for: Players, NPCs, enemies.
  /// </summary>
  [RequireComponent (typeof (PlatformerCollider2D))]
  [RequireComponent (typeof (CharacterHealth))]
  public class Character: MonoBehaviour, IUpdateEntity {
    CharacterAction[] actions;
    CharacterAction lastAction;

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

    // Actions
    public Action onEnterArea;
    public Action onExitArea;

    //
    // ~private
    //
    [HideInInspector]
    public float ladderCenter;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public PlatformerCollider2D controller;
    [HideInInspector]
    public CharacterHealth health;

    /// <summary>
    /// This method precalculate some vars, but those value could change. This need to be refactored.
    /// Maybe setters are the appropiate method to refactor this.
    /// </summary>
    virtual public void Start() {
      controller = GetComponent<PlatformerCollider2D> ();
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
        if (lastAction != null && lastAction != action) {
          lastAction.GainControl();
        }

        action.PerformAction(Time.fixedDeltaTime);
        a = action.GetPostUpdateActions();
      }

      if (Utils.biton((int)a, (int)PostUpdateActions.APPLY_GRAVITY)) {
        velocity.y += gravity * delta;
      }

      if (!Utils.biton((int)a, (int)PostUpdateActions.WORLD_COLLISIONS)) {
        controller.disableWorldCollisions = true;
      }

      controller.Move(velocity * delta, GetComponent<PlatformerInput>().GetAxisRaw());

      // this is meant to fix jump and falling hit something unexpected
      if (controller.collisions.above || controller.collisions.below) {
        velocity.y = 0;
      }

      if (lastAction != null && lastAction != action) {
        lastAction.LoseControl();
      }

      lastAction = action;
    }

    public bool IsOnState(States _state) {
      return (state & _state) == _state;
    }

    public bool IsOnArea(Areas area) {
      return (area & area) == area;
    }

    public void EnterArea(Bounds b, Areas a) {
      area |= a;
      // TODO this should be a map
      if (a == Areas.Ladder) {
        ladderCenter = b.center.x;
      }

      if (onEnterArea != null) {
        onEnterArea(); // TODO send params?
      }
    }

    public void ExitArea(Bounds b, Areas a) {
      area &= ~a;
      if (a == Areas.Ladder && IsOnState (States.Ladder)) {
        state &= ~States.Ladder;
      }

      if (onExitArea != null) {
        onExitArea(); // TODO send params?
      }
    }

    virtual public void OnDeath() {
      Debug.Log("Player die! play some fancy animation!");
    }
  }
}
