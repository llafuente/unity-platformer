using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPlatformer;

namespace UnityPlatformer {
  /// <summary>
  /// Utils class to check in wich state is a character
  /// </summary>
  [Serializable]
  public class CharacterStatesCheck {
    /// <summary>
    /// Character must be in at least one of this states
    /// </summary>
    [Help("First check character is on at least one of the 'Required states'\nThen check character is not in any 'Forbidden states'\n'Required states' = 'Forbidden states' = Nothing, means always valid.")]
    [EnumFlagsAttribute()]
    public StatesMask requiredStates = 0;
    /// <summary>
    /// Character cannot be in any of this states
    /// </summary>
    [EnumFlagsAttribute()]
    public StatesMask forbiddenStates = 0;

    public bool ValidStates(Character character) {
      if (requiredStates != 0 && !character.IsOnAnyState((States) requiredStates)) {
        return false;
      }
      // TODO REVIEW this maybe a problem if Crounch has higher priority
      // but that's exacly why priority exists to rob the state to another action
      if (forbiddenStates != 0 && character.IsOnAnyState((States) forbiddenStates)) {
        return false;
      }

      return true;
    }
  }
}
