using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
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

      //Debug.Log("play: " + animation + " // " + character.state, gameObject);
      playing = animation;
      animator.Play(animation);
    }

    public override float GetAnimationLength(string animation) {
      RuntimeAnimatorController ac = animator.runtimeAnimatorController;
      for(int i = 0; i<ac.animationClips.Length; i++) {
        if(ac.animationClips[i].name == animation) {
          return ac.animationClips[i].length;
        }
      }

      return 0.0f;
    }

  }
}
