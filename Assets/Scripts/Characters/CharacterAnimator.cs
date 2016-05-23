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

    public virtual void Start() {
      character.onAreaChange += OnAreaChange;
      character.onHurtCharacter += OnHurtCharacter;
      character.onStateChange += OnStateChange;
    }

    public virtual void OnEnable() {
      UpdateManager.instance.others.Add(this);
    }

    public virtual void OnDisable() {
      UpdateManager.instance.others.Remove(this);
    }

    public virtual void ManagedUpdate(float delta) {
      if (character.pc2d.collisions.faceDir == 1) {
        transform.localScale  = new Vector3(1, 1, 1);
      } else {
        transform.localScale  = new Vector3(-1, 1, 1);
      }

      if (rotateOnSlopes) {
        if (character.pc2d.collisions.slopeAngle != 0) {
          float angle = 90 - Mathf.Atan2(character.pc2d.collisions.slopeNormal.y,
          -character.pc2d.collisions.slopeNormal.x) * Mathf.Rad2Deg;

          transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        } else {
          transform.rotation = Quaternion.identity;
        }
      }

      if (character.IsOnState(States.WallSliding)) {
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

    public abstract void Play(string animation);

    public virtual void OnAreaChange(Areas before, Areas after) {
      //Debug.LogFormat("area change {1} vs {2}", before, after);
    }
    public virtual void OnHurtCharacter(DamageType dt, Character to) {
      //Debug.LogFormat("hurt {1} with {2} damage", to.gameObject, dt.amount);
    }
    public virtual void OnStateChange(States before, States after) {
      //Debug.LogFormat("OnStateChange {1} vs {2}", before, after);
    }
  }
}
