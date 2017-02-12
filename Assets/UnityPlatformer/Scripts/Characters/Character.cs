using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPlatformer;
using UnityEngine.Assertions;

namespace UnityPlatformer {
  /// <summary>
  /// Base class for: Players, NPCs, enemies.
  ///
  /// Handle all movement logic and transform collider information
  /// into 'readable' information for animations.<para />
  /// NOTE executionOrder should be -25
  /// </summary>
  [RequireComponent (typeof (CharacterHealth))]
  public class Character: PlatformerCollider2D, IUpdateEntity {
    [Header("Character")]
    /// <summary>
    /// Delay before enter States.Falling.
    /// </summary>
    [Comment("Time to wait before change state to falling.")]
    public float fallingTime = 0.1f;
    /// <summary>
    /// Delay before exit States.OnGround.
    /// </summary>
    [Comment("Time before disable OnGround State.")]
    public float groundGraceTime = 0.05f;
    /// <summary>
    /// Minimum velocity. This is not a magnitude.
    /// </summary>
    public float minVelocity = 0.05f;
    /// <summary>
    /// Area change callback type
    /// </summary>
    public delegate void AreaChange(Areas before, Areas after);
    /// <summary>
    /// Area change event (enter or leave)
    /// </summary>
    public AreaChange onAreaChange;
    /// <summary>
    /// Character is Injured callback type
    /// </summary>
    public delegate void InjuredCharacter(Damage dt, CharacterHealth to, Character character);
    /// <summary>
    /// Character is Injured
    /// </summary>
    public InjuredCharacter onInjuredCharacter;
    /// <summary>
    /// State change event callback type
    /// </summary>
    public delegate void StateChange(States before, States after);
    /// <summary>
    /// State change event (enter or leave)
    /// </summary>
    public StateChange onStateChange;
    /// <summary>
    /// Character move callback type
    /// </summary>
    public delegate void CharacterMove(Character character, float delta);
    /// <summary>
    /// Before character move event
    /// </summary>
    public CharacterMove onBeforeMove;
    /// <summary>
    /// After character move event
    /// </summary>
    public CharacterMove onAfterMove;
    /// <summary>
    /// List of enabled actions
    ///
    /// NOTE Each CharacterAction must sync with it's character
    /// </summary>
    [HideInInspector]
    public List<CharacterAction> actions = new List<CharacterAction>();
    /// <summary>
    /// Where is facing
    /// </summary>
    [HideInInspector]
    public Facing faceDir;
    /// <summary>
    /// faceDir change is allowed?\n
    /// This is used at melee attack so when you start an attack character don't
    /// turn and attack both sides at the same time.
    /// </summary>
    [HideInInspector]
    public bool turnAllowed = true;
    /// <summary>
    /// Current states
    /// </summary>
    [HideInInspector]
    public States state = States.None;
    /// <summary>
    /// Current areas
    /// </summary>
    [HideInInspector]
    public Areas area = Areas.None;
    /// <summary>
    /// Current fence
    /// </summary>
    [HideInInspector]
    public Fence fence;
    /// <summary>
    /// Current ladder
    /// </summary>
    [HideInInspector]
    public Ladder ladder;
    /// <summary>
    /// Ladder at Character feet
    /// </summary>
    [HideInInspector]
    public Ladder ladderBottom;
    /// <summary>
    /// Current liquid
    /// </summary>
    [HideInInspector]
    public Liquid liquid;
    /// <summary>
    /// Current reachable item
    /// </summary>
    [HideInInspector]
    public Item item;
    /// <summary>
    /// Current Grab area
    /// </summary>
    [HideInInspector]
    public Grab grab;
    /// <summary>
    /// Current Rope
    /// </summary>
    [HideInInspector]
    public Rope rope;
    /// <summary>
    /// Current RopeSection index
    /// </summary>
    [HideInInspector]
    public int ropeIndex = -1;
    /// <summary>
    /// Current Track
    /// </summary>
    [HideInInspector]
    public TrackData track;
    /// <summary>
    /// Current MovingPlatform
    /// </summary>
    [HideInInspector]
    public MovingPlatform platform;
    /// <summary>
    /// Last jump distance
    /// </summary>
    [HideInInspector]
    public Vector2 lastJumpDistance {
      get {
        return jumpEnd - jumpStart;
      }
    }
    /// <summary>
    /// Last fall distance
    /// </summary>
    [HideInInspector]
    public Vector2 lastFallDistance {
      get {
        return fallEnd - fallStart;
      }
    }
    /// <summary>
    /// Character head position
    /// </summary>
    public Vector2 head {
      get {
        return raycastOrigins.topCenter;
      }
    }
    /// <summary>
    /// Character feet position
    /// </summary>
    public Vector2 feet {
      get {
        return raycastOrigins.bottomCenter;
      }
    }
    /// <summary>
    /// character velocity
    ///
    /// NOTE Character real velocity is velocity + worldVelocity
    /// </summary>
    [HideInInspector]
    public Vector3 velocity = Vector3.zero;
    /// <summary>
    /// World velocity: wind, Track...
    ///
    /// NOTE Character real velocity is velocity + worldVelocity
    /// </summary>
    [HideInInspector]
    public Vector3 worldVelocity = Vector3.zero;
    /// <summary>
    /// Amount moved in the last PlatformerUpdate
    /// </summary>
    [HideInInspector]
    public Vector3 movedLastFrame = Vector3.zero;
    /// <summary>
    /// Cache CharacterHealth
    /// </summary>
    [HideInInspector]
    public CharacterHealth health;
    /// <summary>
    /// Cache HitBox
    /// </summary>
    [HideInInspector]
    public HitBox enterAreas;
    /// <summary>
    /// Cache CharacterAnimator
    /// </summary>
    [HideInInspector]
    public CharacterAnimator animator;
    /// <summary>
    /// Force to play this animation
    /// </summary>
    [HideInInspector]
    public string forceAnimation;
    /// <summary>
    /// Do not execute any Action, Character still moves, so set velocity to
    /// Vector3.zero if necesarry
    ///
    /// NOTE Even frozen, forceAnimation works...
    /// </summary>
    [HideInInspector]
    public float frozen = -1f;




    /// <summary>
    /// Collision bound
    /// </summary>
    [HideInInspector]
    public Bounds colBounds;

    [HideInInspector]
    public CharacterAction lastAction;
    [HideInInspector]
    public CharacterAction meleeInProgress;
    /// <summary>
    /// Cooldown for fallingTime
    /// </summary>
    Cooldown fallingCD;
    /// <summary>
    /// Cooldown for groundGraceTime
    /// </summary>
    Cooldown groundCD;
    /// <summary>
    /// Position when last jump started
    /// </summary>
    Vector2 jumpStart;
    /// <summary>
    /// Position when last jump ended
    /// </summary>
    Vector2 jumpEnd;
    /// <summary>
    /// Position when last fall started
    /// </summary>
    Vector2 fallStart;
    /// <summary>
    /// Position when last fall ended
    /// </summary>
    Vector2 fallEnd;
    /// <summary>
    /// This method precalculate some vars, but those value could change. This need to be refactored.
    /// Maybe setters are the appropiate method to refactor this.
    /// </summary>
    virtual public void Awake() {
      forceAnimation = null;

      if (health == null) {
        health = GetComponent<CharacterHealth>();
        // TODO review how hotswapping behave in this case ?!
        health.onHurt += onInjured;
        health.onDeath += OnDeath;
      }

      if (fallingCD == null) {
        fallingCD = new Cooldown(fallingTime);
      }

      if (groundCD == null) {
        groundCD = new Cooldown(groundGraceTime);
      }

    }

    void onInjured(Damage dt, CharacterHealth to) {
      if (dt != null && onInjuredCharacter != null) {
        onInjuredCharacter(dt, to, this);
      }
    }
    /// <summary>
    /// Get CharacterAction by type
    /// </summary>
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
    ///
    /// Call all actions ask them who 'WantsToUpdate'\n
    /// The highest priority action what want GainControl and PerformAction\n
    /// Given action give a list of things to do after using GetPostUpdateActions\n
    /// When all is done, fire events
    /// </summary>
    public virtual void PlatformerUpdate(float delta) {
      colBounds = bounds;

      // before anything try to find if there is a ladder below
      // it's neccesary for ActionLadder&ActionCrounch
      // this is not the right place... but where?! to be unique
      RaycastHit2D hit = FeetRay(
        skinWidth * 2,
        1 << Configuration.instance.laddersMask
      );

      if (hit) {
        ladderBottom = hit.collider.gameObject.GetComponent<Ladder>();
        Assert.IsNotNull(ladderBottom, "GameObject at Ladders layer without Ladder MonoBehaviour at " + hit.collider.gameObject.GetFullName());
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
      disableWorldCollisions = false;
      PostUpdateActions a = PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;

      if (action != null) {
        if (lastAction != action) {
          action.GainControl(delta);
        }

        action.PerformAction(Time.fixedDeltaTime);
        a = action.GetPostUpdateActions();
      }

      if (BitOn((int)a, (int)PostUpdateActions.APPLY_GRAVITY)) {
        // TODO REVIEW x/y gravity...
        velocity.y += gravity.y * delta;
      }

      if (!BitOn((int)a, (int)PostUpdateActions.WORLD_COLLISIONS)) {
        disableWorldCollisions = true;
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

      // check velocity don't exceed terminalVelocity
      // Limit X
      //velocity.x = Mathf.Min(velocity.x, terminalVelocity.x);
      //velocity.x = Mathf.Max(velocity.x, -terminalVelocity.x);
      // Limit Y but only freefall
      velocity.y = Mathf.Max(velocity.y, -terminalVelocity.y);

      movedLastFrame = Move((velocity + worldVelocity) * delta, delta);

      if (onAfterMove != null) {
        onAfterMove(this, delta);
      }


      // this is meant to fix jump and falling hit something unexpected
      if (collisions.above || collisions.below) {
        velocity.y = 0;
      }

      if (collisions.below) {
        fallingCD.Reset();
        groundCD.Reset();
        EnterStateGraceful(States.OnGround);
      } else {
        // give some margin
        if (groundCD.Ready()) {
          ExitStateGraceful(States.OnGround);
        }

        // falling but not wallsliding
        if (velocity.y < 0 &&
          !IsOnState(States.WallSliding) &&
          !IsOnState(States.Liquid) &&
          !IsOnState(States.Rope)) {
          if (fallingCD.Ready()) {
            EnterStateGraceful(States.Falling);
          }
        }
      }

      if (lastAction != null && lastAction != action) {
        lastAction.LoseControl(delta);
      }

      lastAction = action;
    }
    /// <summary>
    /// Do nothing
    /// </summary>
    public virtual void LatePlatformerUpdate(float delta) {}
    /// <summary>
    /// Return if the Character is on given state
    /// </summary>
    public bool IsOnState(States _state) {
      return BitOn((int)state, (int)_state);
    }
    /// <summary>
    /// Return if the Character is on any of the given states
    /// </summary>
    public bool IsOnAnyState(States _state) {
      return ((int)state & (int)_state) != 0;
    }
    /// <summary>
    /// Return if the Character is on given area
    /// </summary>
    public bool IsOnArea(Areas _area) {
      return BitOn((int)area, (int)_area);
    }
    /// <summary>
    /// EnterState if it's not already in it
    /// It a safe mechanism to not trigger the change
    /// </summary>
    public void EnterStateGraceful(States a) {
      if (!IsOnState(a)) {
        EnterState(a);
      }
    }
    /// <summary>
    /// EnterState if it's not already in it
    /// It a safe mechanism to not trigger the change
    /// </summary>
    public void ExitStateGraceful(States a) {
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
        DisableSlopes(0.1f);
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

      // if is OnGround and doing nothing, is idling
      if (
          (state & States.OnGround) != 0 &&
          velocity.x == 0 &&
          (state & (
            States.MeleeAttack | States.Slipping |
            States.Pushing | States.Pulling
          )) == 0
        ) {
        state |= States.Idle;
      } else {
        state &= ~States.Idle;
      }

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
    /// <summary>
    /// Character enter given area
    /// </summary>
    public void EnterArea(Areas a) {
      Areas before = area;
      area |= a;

      if (before != area && onAreaChange != null) {
        onAreaChange(before, area);
      }
    }
    /// <summary>
    /// Character exit given area
    /// </summary>
    public void ExitArea(Areas a) {
      Areas before = area;
      area &= ~a;

      if (before != area && onAreaChange != null) {
        onAreaChange(before, area);
      }
    }
    /// <summary>
    /// Event when player is dead
    /// </summary>
    virtual public void OnDeath() {
      Debug.Log("Player die! play some fancy animation!");
      UpdateManager.Remove (this);
      gameObject.SetActive(false);
      // disable hitboxes
    }
    /// <summary>
    /// sync UpdateManager
    /// </summary>
    public override void OnEnable() {
      base.OnEnable();
      Awake();
      UpdateManager.Push(this, Configuration.instance.charactersPriority);
    }
    /// <summary>
    /// sync UpdateManager
    /// </summary>
    public virtual void OnDisable() {
      fallingCD = null;
      groundCD = null;
      UpdateManager.Remove(this);
    }
    /// <summary>
    /// Override current animation with the one given. The animation will
    /// be entirely played, then it will ClearOverrideAnimation
    /// </summary>
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

      UpdateManager.SetTimeout(ClearOverrideAnimation, delay);
    }
    /// <summary>
    /// nullify forceAnimation. to be used with UpdateManager.SetTimeout
    /// </summary>
    public void ClearOverrideAnimation() {
      forceAnimation = null;
    }
    /// <summary>
    /// Tell you if there is something on the left side
    /// NOTE ray origin is raycastOrigins.bottomLeft
    /// </summary>
    public bool IsGroundOnLeft(float rayLengthFactor, float delta, Vector3? vel = null) {
      RaycastHit2D hit = LeftFeetRay(skinWidth * rayLengthFactor, (vel ?? velocity) * delta);

      return hit.collider != null;
    }
    /// <summary>
    /// Tell you if there is something on the right side
    /// NOTE ray origin is raycastOrigins.bottomRight
    /// </summary>
    public bool IsGroundOnRight(float rayLengthFactor, float delta, Vector3? vel = null) {
      RaycastHit2D hit = RightFeetRay(skinWidth * rayLengthFactor, (vel ?? velocity) * delta);

      return hit.collider != null;
    }
    /// <summary>
    /// Set faceDir manualy
    /// </summary>
    public void SetFacing(Facing f) {
      if (turnAllowed) {
        faceDir = f;
      }
    }
    /// <summary>
    /// Based on 'horizontal movement'
    /// </summary>
    public void SetFacing(float x) {
      if (turnAllowed) {
        if (x == 0) {
          faceDir = Facing.None;
        } else {
          x = Mathf.Sign(x);
          faceDir = x == 1 ? Facing.Right : Facing.Left;
        }
      }
    }
    /// <summary>
    /// Is there a box on given direction?
    /// </summary>
    public bool IsBox(Directions dir) {
      PlatformerCollider2D.Contacts[] contacts = collisions.contacts;

      bool valid_box = false;
      for (int i = 0; i < collisions.contactsCount; ++i) {
        if (contacts[i].dir != dir) continue;

        Box b = contacts[i].hit.collider.gameObject.GetComponent<Box>();
        if (b != null) {
          // guard against dark arts
          if (Configuration.IsBox(b.boxCharacter.gameObject)) {
            if (
              // the box cannot be falling!
              (b.boxCharacter.IsOnState(States.Falling)) ||
              // box cannot be colliding against anything in the movement direction
              (dir == Directions.Right && b.boxCharacter.collisions.right) ||
              (dir == Directions.Left && b.boxCharacter.collisions.left)
              ) {
              continue;
            }

            valid_box = true;
          }
        }
      }

      return valid_box;
    }
    /// <summary>
    /// Get the lowest box (the one Character Pull/Push)
    /// </summary>
    public Box GetLowestBox(Directions dir) {
      PlatformerCollider2D.Contacts[] contacts = collisions.contacts;
      // sarch the lowest box and push it
      float minY = Mathf.Infinity;
      int index = -1;
      Log.Silly("(Push) PushBox.count {0}", collisions.contactsCount);
      for (int i = 0; i < collisions.contactsCount; ++i) {
        if (contacts[i].dir != dir) continue;

        Box b = contacts[i].hit.collider.gameObject.GetComponent<Box>();
        if (b != null) {
          if (!Configuration.IsBox(b.boxCharacter.gameObject)) {
            Debug.LogWarning("Found a Character that should be a box", b.boxCharacter.gameObject);
            continue;
          }

          float y = b.transform.position.y;
          Log.Silly("(Push) Found a box at index {0} {1} {2} {3}", i, b, minY, y);
          if (y < minY) {
            minY = y;
            index = i;
          }
        }
      }

      Log.Silly("(Push) will push {0} {1}", index, velocity.ToString("F4"));

      if (index != -1) {
        return contacts[index].hit.collider.gameObject.GetComponent<Box>();
      }

      return null;
    }
    /// <summary>
    /// shortcut: Is a bit on
    /// </summary>
    static public bool BitOn(int a, int b) {
      return (a & b) == b;
    }
    /// <summary>
    /// This is a smash/splash/squash test to kill character if needed
    /// </summary>
    void OnCollisionStay2D(Collision2D collisionInfo) {
      //if (collisionInfo.collider.gameObject.layer == Configuration.instance.staticGeometryMask) {
      if (collisionMask.Contains(collisionInfo.collider.gameObject.layer)) {
        Vector3 max = colBounds.max;
        Vector3 min = colBounds.min;



        Debug.Log("OnCollisionStay!!" + collisionInfo.collider.gameObject.GetFullName());
        foreach (ContactPoint2D contact in collisionInfo.contacts) {
          //Debug.DrawRay(contact.point, contact.normal, Color.blue);
          ((Vector3)contact.point).Draw();

          if (contact.point.x - min.x < max.x - contact.point.x) {
            min.x = contact.point.x;
          } else {
            max.x = contact.point.x;
          }

          if (contact.point.y - min.y < max.y - contact.point.y) {
            min.y = contact.point.y;
          } else {
            max.y = contact.point.y;
          }
        }

        colBounds.SetMinMax(min, max);

        colBounds.Draw(transform, Color.blue);
        if (bounds.size.x * bounds.size.y * 0.8 > colBounds.size.x * colBounds.size.y) {
          Debug.Log("Character crushed!!!");
          health.Kill();
        }

      }
    }
  }
}
