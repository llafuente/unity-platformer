using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// Base class for: Players, NPCs, enemies.
  /// Handle all movement logic and transform collider information
  /// into 'readable' information for animations.
  /// </summary>
  [RequireComponent (typeof (PlatformerCollider2D))]
  [RequireComponent (typeof (CharacterHealth))]
  public class Character: MonoBehaviour, IUpdateEntity {
    #region public

    [Comment("Time to wait before change state to falling.")]
    public float fallingTime = 0.1f;
    [Comment("Time before disable OnGround State.")]
    public float groundGraceTime = 0.05f;

    public float minVelocity = 0.05f;

    ///
    /// Callbacks
    ///

    public delegate void AreaChange(Areas before, Areas after);
    public AreaChange onAreaChange;
    public delegate void HurtCharacter(Damage dt, CharacterHealth to, Character character);
    public HurtCharacter onHurtCharacter;
    public delegate void StateChange(States before, States after);
    public StateChange onStateChange;
    public delegate void CharacterDelegate(Character character, float delta);
    public CharacterDelegate onBeforeMove;
    public CharacterDelegate onAfterMove;

    #endregion

    #region ~private

    /// <summary>
    /// List of enabled actions
    /// </summary>
    internal List<CharacterAction> actions = new List<CharacterAction>();
    internal Facing faceDir;
    internal States state = States.None;
    internal Areas area = Areas.None;
    internal BoxCollider2D body;
    internal Ladder ladder;
    internal Ladder ladderBottom;
    internal Liquid liquid;
    internal Item item;
    internal Grab grab;
    internal Rope rope;
    internal Track track;
    internal int ropeIndex = -1;
    internal Vector2 lastJumpDistance {
      get {
        return jumpEnd - jumpStart;
      }
    }
    internal Vector2 lastFallDistance {
      get {
        return fallEnd - fallStart;
      }
    }

    public float height {
      get {
        return pc2d.GetComponent<BoxCollider2D>().bounds.size.y;
      }
    }

    internal Vector2 fallDistance;
    internal MovingPlatform platform;
    // character velocity by itself. Movement
    internal Vector3 velocity = Vector3.zero;
    // World velocity, wind, tracks etc.
    internal Vector3 worldVelocity = Vector3.zero;
    internal PlatformerCollider2D pc2d;
    internal CharacterHealth health;

    /// <summary>
    /// Force to play this animation
    /// </summary>
    internal string forceAnimation;
    /// <summary>
    /// Do not execute any Action, Character still moves, so set velocity to
    /// Vector3.zero if necesarry
    /// </summary>
    internal float frozen = -1f;
    /// <summary>
    /// back reference to CharacterAnimator
    /// </summary>
    internal CharacterAnimator animator;

    public Vector2 head {
      get {
        return pc2d.raycastOrigins.topCenter;
      }
    }
    public Vector2 feet {
      get {
        return pc2d.raycastOrigins.bottomCenter;
      }
    }

    #endregion

    #region private

    CharacterAction lastAction;

    Cooldown fallingCD;
    Cooldown groundCD;

    Vector2 jumpStart;
    Vector2 jumpEnd;

    Vector2 fallStart;
    Vector2 fallEnd;


    #endregion

    /// <summary>
    /// This method precalculate some vars, but those value could change. This need to be refactored.
    /// Maybe setters are the appropiate method to refactor this.
    /// </summary>
    virtual public void Awake() {
      forceAnimation = null;
      //Debug.Log("Start new Character: " + gameObject.name);
      pc2d = GetComponent<PlatformerCollider2D> ();
      health = GetComponent<CharacterHealth>();
      health.onHurt += OnHurt;
      body = GetComponent<BoxCollider2D>();

      if (fallingCD == null) {
        fallingCD = new Cooldown(fallingTime);
      }

      if (groundCD == null) {
        groundCD = new Cooldown(groundGraceTime);
      }
      //TODO review how hotswapping behave in this case ?!
      health.onDeath += OnDeath;
    }

    void OnHurt(Damage dt, CharacterHealth to) {
      if (dt != null && onHurtCharacter != null) {
        onHurtCharacter(dt, to, this);
      }
    }

    public T GetAction<T>() {
      foreach (var i in actions) {
        if (i.GetType() == typeof(T)) {
          return (T) Convert.ChangeType(i, typeof(T));
        }
      }

      return default(T);
    }

    /// <summary>
    /// Managed update called by UpdateManager
    /// Transform Input into platformer magic :)
    /// </summary>
    public virtual void PlatformerUpdate(float delta) {

      // before anything try to find if there is a ladder below
      // it's neccesary for ActionLadder&ActionCrounch
      RaycastHit2D hit = pc2d.DoFeetRay(
        pc2d.skinWidth * 2,
        Configuration.instance.laddersMask
      );

      if (hit) {
        ladderBottom = hit.collider.gameObject.GetComponent<Ladder>();
        if (ladderBottom == null) {
          Debug.LogWarning("Object with ladder mask but no Ladder Behaviour found", hit.collider.gameObject);
        }
      } else {
        ladderBottom = null;
      }

      //
      frozen -= delta;
      int prio = 0;
      int tmp;
      CharacterAction action = null;

      if (frozen < 0) {
        foreach (var i in actions) {
          tmp = i.WantsToUpdate(delta);
          if (tmp < 0) {
            i.PerformAction(Time.fixedDeltaTime);
          } else if (prio < tmp) {
            prio = tmp;
            action = i;
          }
        }
      }

      // reset / defaults
      pc2d.disableWorldCollisions = false;
      PostUpdateActions a = PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;

      if (action != null) {
        if (lastAction != action) {
          action.GainControl(delta);
        }

        action.PerformAction(Time.fixedDeltaTime);
        a = action.GetPostUpdateActions();
      }

      if (Utils.biton((int)a, (int)PostUpdateActions.APPLY_GRAVITY)) {
        // TODO REVIEW x/y gravity...
        velocity.y += pc2d.gravity.y * delta;
      }

      if (!Utils.biton((int)a, (int)PostUpdateActions.WORLD_COLLISIONS)) {
        pc2d.disableWorldCollisions = true;
      }

      if (Mathf.Abs(velocity.x) < minVelocity) {
        velocity.x = 0.0f;
      }

      if (Mathf.Abs(velocity.y) < minVelocity) {
        velocity.y = 0.0f;
      }

      if (onBeforeMove != null) {
        onBeforeMove(this, delta);
      }

      pc2d.Move((velocity + worldVelocity) * delta, delta);

      if (onAfterMove != null) {
        onAfterMove(this, delta);
      }


      // this is meant to fix jump and falling hit something unexpected
      if (pc2d.collisions.above || pc2d.collisions.below) {
        velocity.y = 0;
      }

      if (pc2d.collisions.below) {
        fallingCD.Reset();
        groundCD.Reset();
        SolfEnterState(States.OnGround);
      } else {
        groundCD.Increment();
        // give some margin
        if (groundCD.Ready()) {
          SolfExitState(States.OnGround);
        }

        // falling but not wallsliding
        if (velocity.y < 0 &&
          !IsOnState(States.WallSliding) &&
          !IsOnState(States.Liquid) &&
          !IsOnState(States.Rope)) {
          fallingCD.Increment();
          if (fallingCD.Ready()) {
            SolfEnterState(States.Falling);
          }
        }
      }

      if (lastAction != null && lastAction != action) {
        lastAction.LoseControl(delta);
      }

      lastAction = action;
    }

    public virtual void LatePlatformerUpdate(float delta) {
    }

    public bool IsOnState(States _state) {
      return (state & _state) == _state;
    }

    public bool IsOnAnyState(States _state) {
      return (state & _state) > 0;
    }

    public bool IsOnArea(Areas _area) {
      return (area & _area) == _area;
    }

    /// <summary>
    /// EnterState if it's not already in it
    /// It a safe mechanism to not trigger the change
    /// </summary>
    public void SolfEnterState(States a) {
      if (!IsOnState(a)) {
        EnterState(a);
      }
    }
    /// <summary>
    /// EnterState if it's not already in it
    /// It a safe mechanism to not trigger the change
    /// </summary>
    public void SolfExitState(States a) {
      if (IsOnState(a)) {
        ExitState(a);
      }
    }
    /// <summary>
    /// Notify that Character enter a new state.
    /// There are incompatible states, like jumping and falling,
    /// You don't need to handle those manually, we will exit
    /// any state needed here.
    /// </summary>
    /// <param name="a">State to enter.</param>
    /// <param name="privcall">Do call callbacks, changes will be batch into one call.</param>
    public void EnterState(States a, bool privcall = false) {
      if (a == States.Falling && IsOnState(States.Falling)) {
        fallStart = transform.position;
      }
      if (a == States.Jumping && !IsOnState(States.Jumping)) {
        jumpStart = transform.position;
        // temporary disable slopes because jump initial velocity.y
        // was modified by ClimbSlope & DescendSlope
        pc2d.DisableSlopes(0.1f);
      }

      if (a == States.Falling) {
        ExitState(States.Hanging, true);
      }
      if (a == States.Jumping) {
        ExitState(States.Falling | States.OnGround | States.WallSliding, true);
      }
      if (a == States.OnGround) {
        ExitState(States.Falling | States.Hanging | States.WallSliding, true);
      }
      if (a == States.WallSliding) {
        ExitState(States.Falling | States.Hanging | States.Jumping, true);
      }
      // while grabbing or ladder, just do that!
      if (a == States.Grabbing || a == States.Ladder) {
        state = 0;
      }
      if (state == States.Grabbing) {
        // while grabbing cannot enter in another state
        // we need to leave first
        return;
      }

      States before = state;
      state |= a;

      if (!privcall && onStateChange != null && before != state) {
        onStateChange(before, state);
      }
    }

    /// <summary>
    /// Notify that Character enter a new state.
    /// There are incompatible states, like jumping and falling,
    /// You don't need to handle those manually, we will exit
    /// any state needed here.
    /// </summary>
    /// <param name="a">State to enter.</param>
    /// <param name="privcall">Do call callbacks, changes will be batch into one call.</param>
    public void ExitState(States a, bool privcall = false) {
      // TODO REVIEW if Hanging include Jumping this fail...
      if (a == States.Falling && IsOnState(States.Falling)) {
        fallEnd = transform.position;
      }
      if (a == States.Jumping && IsOnState(States.Jumping)) {
        jumpEnd = transform.position;
      }

      States before = state;
      state &= ~a;

      if (!privcall && onStateChange != null && before != state) {
        onStateChange(before, state);
      }
    }

    public void EnterArea(Areas a) {
      Areas before = area;
      area |= a;

      if (before != area && onAreaChange != null) {
        onAreaChange(before, area);
      }
    }

    public void ExitArea(Areas a) {
      Areas before = area;
      area &= ~a;

      if (before != area && onAreaChange != null) {
        onAreaChange(before, area);
      }
    }

    virtual public void OnDeath() {
      Debug.Log("Player die! play some fancy animation!");
    }

    public virtual Vector3 GetCenter() {
      return pc2d.GetComponent<BoxCollider2D>().bounds.center;
    }

    public virtual void OnEnable() {
      Awake();
      UpdateManager.instance.Push(this, Configuration.instance.charactersPriority);
    }

    public virtual void OnDisable() {
      fallingCD = null;
      groundCD = null;
      UpdateManager.instance.Remove(this);
    }

    public void SetOverrideAnimation(string animation, bool freeze) {
      if (animator == null) {
        Debug.LogWarning("Cannot OverrideAnimation. There is no PlatformerAnimator linked to this Character", this);
        return;
      }

      float delay = animator.GetAnimationLength(animation);

      forceAnimation = animation;
      if (freeze) {
        frozen = delay;
      }

      UpdateManager.instance.SetTimeout(ClearOverrideAnimation, delay);
    }

    public void ClearOverrideAnimation() {
      forceAnimation = null;
    }

    /// <summary>
    /// Tell you if there is something on the left side
    /// NOTE ray origin is raycastOrigins.bottomLeft
    /// </summary>
    public bool IsGroundOnLeft(float rayLengthFactor, float delta) {
      RaycastHit2D hit = pc2d.LeftFeetRay(pc2d.skinWidth * rayLengthFactor, velocity * delta);

      return hit.collider != null;
    }

    /// <summary>
    /// Tell you if there is something on the right side
    /// NOTE ray origin is raycastOrigins.bottomRight
    /// </summary>
    public bool IsGroundOnRight(float rayLengthFactor, float delta) {
      RaycastHit2D hit = pc2d.RightFeetRay(pc2d.skinWidth * rayLengthFactor, velocity * delta);

      return hit.collider != null;
    }

    // TODO REVIEW necessary?
    public void SetFacing(Facing f) {
      faceDir = f;
    }

    public void SetFacing(float x) {
      if (x == 0) {
        faceDir = Facing.None;
      } else {
        x = Mathf.Sign(x);
        faceDir = x == 1 ? Facing.Right : Facing.Left;
      }
    }
  }
}
