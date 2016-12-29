using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// Animator class
  ///
  /// Use all data available at Character &amp; cia to know what to play
  /// </summary>
  public abstract class CharacterAnimator: MonoBehaviour, IUpdateEntity {
    /// <summary>
    /// Character
    /// </summary>
    public Character character;
    /// <summary>
    /// When a player is on a slope rotate the player the same angle?
    /// </summary>
    public bool rotateOnSlopes = true;
    /// <summary>
    /// Max rotation slope, above this value it will be 0º
    /// </summary>
    public float maxSlope = 70f;

    [Space(10)]
    [Header("Animation names")]

    public string fallLoop = "falling";
    public string fenceIdle = "fence_idle";
    public string fenceRight = "fence_right";
    public string fenceLeft = "fence_left";
    public string fenceUp = "fence_up";
    public string fenceDown = "fence_down";
    public string ladder = "ladder";
    public string rope = "rope";
    public string pushing = "pushing";
    public string pulling = "pulling";
    public string swimming = "swimming";
    public string wallsliding = "wallsliding";
    public string slipping = "slipping";
    public string grabbing = "grabbing";
    public string attackMelee = "attack_melee";
    public string jump = "jump";
    public string walk = "walk";
    public string idle = "idle";
    public string crounchWalk = "crounch_walk";
    public string crounchIdle = "crounch_idle";

    /// <summary>
    /// Previous speed changed when calling PlaybackSpeed, StopPlayback or StartPlayback
    /// </summary>
    public float previousSpeed;
    /// <summary>
    /// Start listening
    /// </summary>
    public virtual void Start() {
      character.animator = this;
      character.onAreaChange += OnAreaChange;
      character.onInjuredCharacter += OnInjuredCharacter;
      character.onStateChange += OnStateChange;
    }
    /// <summary>
    /// sync UpdateManager
    /// </summary>
    public virtual void OnEnable() {
      UpdateManager.Push(this, Configuration.instance.defaultPriority);
    }
    /// <summary>
    /// sync UpdateManager
    /// </summary>
    public virtual void OnDisable() {
      UpdateManager.Remove(this);
    }
    /// <summary>
    /// Calculate what animation to play and do it!
    /// </summary>
    public virtual void PlatformerUpdate(float delta) {
      if (character.faceDir == Facing.Right) {
        transform.localScale  = new Vector3(1, 1, 1);
      } else {
        transform.localScale  = new Vector3(-1, 1, 1);
      }

      if (rotateOnSlopes) {
        if (
          character.pc2d.collisions.slopeAngle != 0 &&
          character.pc2d.collisions.below &&
          !character.pc2d.collisions.left &&
          !character.pc2d.collisions.right &&
          maxSlope > character.pc2d.collisions.slopeAngle
        ) {
          float angle = 90 - Mathf.Atan2(character.pc2d.collisions.slopeNormal.y,
          -character.pc2d.collisions.slopeNormal.x) * Mathf.Rad2Deg;

          transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        } else {
          transform.rotation = Quaternion.identity;
        }
      }

      if (character.forceAnimation != null) {
        Play(character.forceAnimation);
      } else if (character.IsOnState(States.Rope)) {
        Play(rope);
        // override rotation
        float z = character.rope.sections[character.ropeIndex].transform.eulerAngles.z;
        transform.rotation = Quaternion.AngleAxis(z, Vector3.forward);
      } else if (character.IsOnState(States.Pushing)) {
        Play(pushing);
      } else if (character.IsOnState(States.Pulling)) {
        Play(pulling);
      } else if (character.IsOnState(States.Liquid)) {
        Play(swimming);
      } else if (character.IsOnState(States.WallSliding)) {
        Play(wallsliding);
      } else if (character.IsOnState(States.Slipping)) {
        Play(slipping);
      } else if (character.IsOnState(States.Grabbing)) {
        Play(grabbing);
      } else if (character.IsOnState(States.MeleeAttack)) {
        Play(attackMelee);
      } else if (character.IsOnState(States.Ladder)) {
        Play(ladder);
      } else if (character.IsOnState(States.Fence)) {
        // left/right has priority
        if (character.velocity.x != 0) {
          Play(Mathf.Sign(character.velocity.x) == 1 ? fenceRight : fenceLeft);
        } else if (character.velocity.y != 0) {
          Play(Mathf.Sign(character.velocity.y) == 1 ? fenceUp : fenceDown);
        } else {
          Play(fenceIdle);
        }
      } else if (character.IsOnState(States.Jumping)) {
        //Transition("jump_start", 0.1f);
        Play(jump);
      } else if (character.IsOnState(States.Falling)) {
        Play(fallLoop);
      } else {
        if (character.IsOnState(States.Crounch)) {
          if (character.velocity.x != 0) {
            Play(crounchWalk);
          } else {
            Play(crounchIdle);
          }
        } else {
          if (character.velocity.x != 0) {
            Play(walk);
          } else {
            Play(idle);
          }
        }
      }
    }
    /// <summary>
    /// do nothing
    /// </summary>
    public virtual void LatePlatformerUpdate(float delta) {}
    /// <summary>
    /// Play animation
    ///
    /// NOTE this method should check that it's not currently playing
    /// the given animation, because each PlatformerUpdate you will recieve
    /// the same animation
    /// </summary>
    public abstract void Play(string animation);
    /// <summary>
    /// Set playback speed.
    ///
    /// 1: normal\n
    /// 0-1: slow-motion\n
    /// >1: fast-motion
    /// </summary>
    public abstract void PlaybackSpeed(float speed);
    /// <summary>
    /// Stop current animation
    /// </summary>
    public abstract void StopPlayback();
    /// <summary>
    /// Continue playing current animation
    /// </summary>
    public abstract void StartPlayback();
    /// <summary>
    /// Get animation length in seconds
    /// </summary>
    public abstract float GetAnimationLength(string animation);
    /// <summary>
    /// Not used atm
    /// </summary>
    public virtual void OnAreaChange(Areas before, Areas after) {
      //Debug.LogFormat("area change {1} vs {2}", before, after);
    }
    /// <summary>
    /// Not used atm
    /// </summary>
    public virtual void OnInjuredCharacter(Damage dt, CharacterHealth h, Character to) {
      //Debug.LogFormat("hurt {1} with {2} damage", to.gameObject, dt.amount);
    }
    /// <summary>
    /// Not used atm
    /// </summary>
    public virtual void OnStateChange(States before, States after) {
      //Debug.LogFormat("OnStateChange {1} vs {2}", before, after);
    }
  }
}
