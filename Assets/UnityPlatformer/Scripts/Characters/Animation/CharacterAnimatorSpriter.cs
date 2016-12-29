//#define UP_USE_SPRITER

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPlatformer;

#if UP_USE_SPRITER
using SpriterDotNetUnity;
#endif

namespace UnityPlatformer {
  #if UP_USE_SPRITER

  /// <summary>
  /// Animator using SpriterDotNet
  /// TODO queue animation, support start-loop animation
  /// </summary>
  public class CharacterAnimatorSpriter: CharacterAnimator {
    public SpriterDotNetBehaviour behaviour;

    public override void Start() {
      base.Start();

      /* display all animations
      foreach (var i in behaviour.Animator.GetAnimations()) {
        Debug.Log("animation " + i);
      }
      */

    }

    public override void Play(string animation) {
      if (behaviour.Animator.Name != animation) {
        behaviour.Animator.Play(animation);
      }
    }

    public override void PlaybackSpeed(float speed) {
      previousSpeed = behaviour.Animator.Speed;
      behaviour.Animator.Speed = speed;
    }

    public override void StopPlayback() {
      previousSpeed = behaviour.Animator.Speed;
      behaviour.Animator.Speed = 0.0f;
    }

    public override void StartPlayback() {
      behaviour.Animator.Speed = previousSpeed != 0.0f ? previousSpeed : 1.0f;
      previousSpeed = 0.0f;
    }

    public override float GetAnimationLength(string animation) {
      // TODO

      return 0.0f;
    }
  }
  #endif
}
