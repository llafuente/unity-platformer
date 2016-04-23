using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPlatformer;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// Base class for: Players, NPCs, enemies.
  /// </summary>
  [RequireComponent (typeof (PlatformerCollider2D))]
  [RequireComponent (typeof (CharacterHealth))]
  public class Character: MonoBehaviour, IUpdateEntity {

    #region public

    [Comment("Override Configuration.gravity, (0,0) means use global.")]
    public Vector2 gravityOverride = Vector2.zero;

    public Vector2 gravity {
      get {
        return gravityOverride == Vector2.zero ? Configuration.instance.gravity : gravityOverride;
      }
    }

    /// <summary>
    /// States in wich the Character can be.
    /// Can be combine
    /// </summary>
    public enum States {
      None =                0,
      OnGround =            1,
      OnMovingPlatform =    3,
      OnSlope =             1 << 2 | OnGround,
      Jumping =             1 << 3,
      Hanging =             1 << 4 | Jumping,
      Falling =             1 << 5,
      FallingFast =         1 << 6 | Falling,
      Ladder =              1 << 7,
      WallSliding =         1 << 8,
      WallSticking =        1 << 9,
      //Dashing,
      //Frozen,
      //Slipping,
      //FreedomState
    }

    /// <summary>
    /// Areas in wich the Character can be.
    /// REVIEW can this be used to handle hazardous areas?
    /// </summary>
    public enum Areas {
      None = 0x0,
      Ladder = 0x01
    }

    ///
    /// Actions
    ///

    public Action onEnterArea;
    public Action onExitArea;

    #endregion

    #region ~private

    [HideInInspector]
    public List<CharacterAction> actions = new List<CharacterAction>();
    [HideInInspector]
    public States state = States.None;
    [HideInInspector]
    public Areas area = Areas.None;
    [HideInInspector]
    public BoxCollider2D body;
    [HideInInspector]
    public Ladder ladder;
    [HideInInspector]
    public Vector2 lastJumpDistance {
      get {
        return jumpEnd - jumpStart;
      }
    }
    [HideInInspector]
    public Vector2 fallDistance;
    [HideInInspector]
    public MovingPlatform platform;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public PlatformerCollider2D controller;
    [HideInInspector]
    public CharacterHealth health;

    #endregion

    #region private

    CharacterAction lastAction;

    Vector2 jumpStart;
    Vector2 jumpEnd;

    #endregion

    /// <summary>
    /// This method precalculate some vars, but those value could change. This need to be refactored.
    /// Maybe setters are the appropiate method to refactor this.
    /// </summary>
    virtual public void Awake() {
      //Debug.Log("Start new Character: " + gameObject.name);
      controller = GetComponent<PlatformerCollider2D> ();
      health = GetComponent<CharacterHealth>();
      body = GetComponent<BoxCollider2D>();

      health.onDeath += OnDeath;
    }

    public void Attach(UpdateManager um) {
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
        tmp = i.WantsToUpdate(delta);
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
        // TODO REVIEW x/y gravity...
        velocity.y += gravity.y * delta;
      }

      if (!Utils.biton((int)a, (int)PostUpdateActions.WORLD_COLLISIONS)) {
        controller.disableWorldCollisions = true;
      }

      controller.Move(velocity * delta);

      // this is meant to fix jump and falling hit something unexpected
      if (controller.collisions.above || controller.collisions.below) {
        velocity.y = 0;
      }

      if (controller.collisions.below) {
        EnterState(States.OnGround);
      } else {
        ExitState(States.OnGround);
        if (velocity.y < 0) {
          EnterState(States.Falling);
        }
      }

      if (lastAction != null && lastAction != action) {
        lastAction.LoseControl();
      }

      lastAction = action;
    }

    public bool IsOnState(States _state) {
      return (state & _state) == _state;
    }

    public bool IsOnArea(Areas _area) {
      return (area & _area) == _area;
    }

    public void EnterState(States a) {
      if (a == States.Falling && IsOnState(States.Jumping)) {
        ExitState(States.Jumping);
      }
      if (a == States.Jumping && !IsOnState(States.Jumping)) {
        jumpStart = transform.position;
      }
      if (a == States.OnGround && IsOnState(States.Falling)) {
        ExitState(States.Falling);
      }

      state |= a;
    }

    public void ExitState(States a) {
      // TODO REVIEW if Hanging include Jumping this fail...
      if (a == States.Jumping && IsOnState(States.Jumping)) {
        jumpEnd = transform.position;
      }

      state &= ~a;
    }

    public void EnterArea(Areas a) {
      area |= a;

      if (onEnterArea != null) {
        onEnterArea(); // TODO send params?
      }
    }

    public void ExitArea(Areas a) {
      area &= ~a;

      if (onExitArea != null) {
        onExitArea(); // TODO send params?
      }
    }

    virtual public void OnDeath() {
      Debug.Log("Player die! play some fancy animation!");
    }

    public virtual Vector3 GetFeetPosition() {
      return body.bounds.center - new Vector3(
        0,
        body.bounds.size.y * 0.5f,
        0);
    }
  }
}
