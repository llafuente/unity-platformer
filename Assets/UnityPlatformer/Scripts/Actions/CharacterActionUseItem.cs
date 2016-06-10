using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// TODO We need some kind of debounce / Use delay
  /// </summary>
  public class CharacterActionUseItem: CharacterAction {
    #region public

    public Vector3 characterOffset = new Vector3(0, 0, 0);
    public string actionUse;

    [Space(10)]
    [Comment("Remember: Higher priority wins. Modify with caution")]
    public int priority = 30;

    #endregion

    public override int WantsToUpdate(float delta) {
      if (
        character.IsOnArea(Areas.Item) &&
        pc2d.collisions.below &&
        character.item.IsUsableBy(character) &&
        input.IsActionHeld(actionUse)
      ) {
        return priority;
      }
      return 0;
    }

    public override void PerformAction(float delta) {
      character.item.Use(character);
    }

    public override PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
