using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// A Character: Player, npc, enemy...
	/// TODO double jump
	/// TODO Dash
	/// TODO Attack range
	/// TODO Attack melee
	/// TODO states and area are must be in Controller2D
	/// TODO create a platformer input, to handle all input, and store information here, LateUpdate is messing events
	/// </summary>
	[RequireComponent (typeof (Controller2D))]
	[RequireComponent (typeof (CharacterHealth))]
  public class Character: MonoBehaviour, UpdateEntity {
    CharacterAction[] actions;

		float accelerationTimeAirborne = .2f;

	  float ladderMoveSpeed = 4;

	  public Vector2 wallJumpClimb;
	  public Vector2 wallJumpOff;
	  public Vector2 wallLeap;

	  public float wallSlideSpeedMax = 3;
	  public float wallStickTime = .25f;
	  float timeToWallUnstick;

	  float gravity = -50;

		//
		// ~private
		//
		[HideInInspector]
		public float ladderCenter;
		[HideInInspector]
		public Vector3 velocity;

		Controller2D controller;


    /// <summary>
		/// This method precalculate some vars, but those value could change. This need to be refactored.
		/// Maybe setters are the appropiate method to refactor this.
		/// </summary>
		void Start() {
			controller = GetComponent<Controller2D> ();
      actions = GetComponents<CharacterAction>();

			CharacterHealth ch = GetComponent<CharacterHealth>();
			ch.onDeath += OnDeath;
    }

    // TODO REVIEW FixedUpdate?
    void Update() {

    }

	  public void Attach(UpdateManager um) {
	    // TODO HACK WIP
	    GetComponent<CharacterActionJump>().Attach(um);
	  }

	  /// <summary>
	  /// Managed update called by UpdateManager
	  /// Transform Input into platformer magic :)
	  /// </summary>
	  public void ManagedUpdate(float delta) {
	    int wallDirX = (controller.collisions.left) ? -1 : 1;


	    bool wallSliding = false;
			/*
	    if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
	      wallSliding = true;

	      if (velocity.y < -wallSlideSpeedMax) {
	        velocity.y = -wallSlideSpeedMax;
	      }

	      if (timeToWallUnstick > 0) {
					// TODO ResetXSmoothing ??
	        //velocityXSmoothing = 0;
	        velocity.x = 0;

	        if (input.x != wallDirX && input.x != 0) {
	          timeToWallUnstick -= delta;
	        }
	        else {
	          timeToWallUnstick = wallStickTime;
	        }
	      }
	      else {
	        timeToWallUnstick = wallStickTime;
	      }
	    }

	    // jump
	    if (Input.GetKeyDown (KeyCode.Space)) {
	      if (wallSliding) {
	        if (wallDirX == input.x) {
	          velocity.x = -wallDirX * wallJumpClimb.x;
	          velocity.y = wallJumpClimb.y;
	        }
	        else if (input.x == 0) {
	          velocity.x = -wallDirX * wallJumpOff.x;
	          velocity.y = wallJumpOff.y;
	        }
	        else {
	          velocity.x = -wallDirX * wallLeap.x;
	          velocity.y = wallLeap.y;
	        }
	      }

	    }
			*/



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

	  public bool IsOnState(Controller2D.States _state) {
	    return (controller.state & _state) == _state;
	  }

	  public bool IsOnLadder() {
	    return (controller.area & Controller2D.Areas.Ladder) == Controller2D.Areas.Ladder;
	  }

	  public void EnterLadderArea(Bounds b) {
	    controller.area |= Controller2D.Areas.Ladder;
	    ladderCenter = b.center.x;
	  }

	  public void ExitLadderArea(Bounds b) {
	    controller.area &= ~Controller2D.Areas.Ladder;
	    if (IsOnState (Controller2D.States.Ladder)) {
	      controller.state &= ~Controller2D.States.Ladder;
	    }
	  }

	  public void OnDeath() {
	    Debug.Log("Player die! play some fancy animation!");
	  }
	}
}
