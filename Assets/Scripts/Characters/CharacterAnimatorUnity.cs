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

  public class CharacterAnimatorUnity: CharacterAnimator {
    public Animator animator;

    string playing;

    public override void Start() {
      base.Start();
    }

    public override void Play(string animation) {
      if (playing == animation) {
        return;
      }

      //Debug.LogFormat("play: {0}", animation);
      playing = animation;
      animator.Play(animation);
    }
  }
  #endif
}
