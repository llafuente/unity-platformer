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

    public override float GetAnimationLength(string animation) {
      // TODO

      return 0.0f;
    }
  }
  #endif
}
