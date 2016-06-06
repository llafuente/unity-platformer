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
  }
}
