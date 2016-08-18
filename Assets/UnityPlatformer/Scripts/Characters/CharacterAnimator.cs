using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// Animator class, read all information available at character to play
  /// animations
  /// </summary>
  public abstract class CharacterAnimator: MonoBehaviour, IUpdateEntity {

    public Character character;
    public bool rotateOnSlopes = true;
    public float maxSlope = 70f;

    public virtual void Start() {
      character.animator = this;
      character.onAreaChange += OnAreaChange;
      character.onHurtCharacter += OnHurtCharacter;
      character.onStateChange += OnStateChange;
    }

    public virtual void OnEnable() {
      UpdateManager.instance.Push(this, Configuration.instance.defaultPriority);
    }

    public virtual void OnDisable() {
      UpdateManager.instance.Remove(this);
    }

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
        Play("rope");
        // override rotation
        float z = character.rope.sections[character.ropeIndex].transform.eulerAngles.z;
        transform.rotation = Quaternion.AngleAxis(z, Vector3.forward);
      } else if (character.IsOnState(States.Pushing)) {
        Play("pushing");
      } else if (character.IsOnState(States.Liquid)) {
        Play("swimming");
      } else if (character.IsOnState(States.WallSliding)) {
        Play("wallsliding");
      } else if (character.IsOnState(States.Slipping)) {
        Play("slipping");
      } else if (character.IsOnState(States.Grabbing)) {
        Play("grabbing");
      } else if (character.IsOnState(States.MeleeAttack)) {
        Play("attack_melee");
      } else if (character.IsOnState(States.Ladder)) {
        Play("ladder");
      } else if (character.IsOnState(States.Jumping)) {
        //Transition("jump_start", 0.1f);
        Play("jump");
      } else if (character.IsOnState(States.Falling)) {
        Play("falling");
      } else {
        if (character.velocity.x != 0) {
          Play("walk");
        } else {
          Play("idle");
        }
      }
    }

    public virtual void LatePlatformerUpdate(float delta) {
    }

    public abstract void Play(string animation);

    public abstract float GetAnimationLength(string animation);

    public virtual void OnAreaChange(Areas before, Areas after) {
      //Debug.LogFormat("area change {1} vs {2}", before, after);
    }
    public virtual void OnHurtCharacter(DamageType dt, Health h, Character to) {
      //Debug.LogFormat("hurt {1} with {2} damage", to.gameObject, dt.amount);
    }
    public virtual void OnStateChange(States before, States after) {
      //Debug.LogFormat("OnStateChange {1} vs {2}", before, after);
    }
  }
}
