#define UP_USE_SPRITER

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

    UnitySpriterAnimator animator;

    public override void Start() {
      base.Start();

      animator = behaviour.Animator;
      /* display all animations
      foreach (var i in animator.GetAnimations()) {
        Debug.Log(i);
      }
      */
    }

    public override void Play(string animation) {
        if (animator.Name != animation) {
        animator.Play(animation);
      }
    }
  }
  #endif
}
