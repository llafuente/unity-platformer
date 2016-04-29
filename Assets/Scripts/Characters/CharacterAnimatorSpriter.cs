using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPlatformer;
using SpriterDotNetUnity;

namespace UnityPlatformer {
  #if UP_USE_SPRITER

  /// <summary>
  /// Animator using SpriterDotNet
  /// </summary>
  public class CharacterAnimatorSpriter: CharacterAnimator {
    public SpriterDotNetBehaviour behaviour;

    UnitySpriterAnimator animator;

    public override void Start() {
      base.Start();

      animator = behaviour.Animator;

      foreach (var i in animator.GetAnimations()) {
        Debug.Log(i);
      }
    }

    public override void Play(string animation) {
        if (animator.Name != animation) {
        animator.Play(animation);
      }
    }
  }
  #endif
}
