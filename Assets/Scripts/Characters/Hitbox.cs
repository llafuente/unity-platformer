using UnityEngine;
using UnityEngine.UI;
namespace UnityPlatformer {
  public enum CharacterPart {
    None =          0,
    FakeBody =      1,
    Body =          1 << 2,
    RightHand =     1 << 3,
    LeftHand =      1 << 4,
    RightFoot =     1 << 5,
    LeftFoot =      1 << 6,
    Head =          1 << 7,
  }

  public enum HitBoxType {
    DealDamage,
    RecieveDamage,
    EnterAreas
  }

  /// <sumary>
  /// HitBoxes
  /// </sumary>
  [RequireComponent (typeof (BoxTrigger2D))]
  public class HitBox : MonoBehaviour {
    #region public

    public CharacterPart part;
    [Comment("Who can deal damage to me?")]
    public LayerMask collideWith;
    [Comment("Who am I?")]
    public CharacterHealth owner;
    public HitBoxType type = HitBoxType.DealDamage;

    #endregion

    void OnTriggerEnter2D(Collider2D o) {
      //Debug.Log(this.name + " collide with: " + o.gameObject);
      if (Utils.layermask_contains(collideWith, o.gameObject.layer)) {
        var dst = o.gameObject.GetComponent<DamageType> ();
        if (dst == null) {
          Debug.LogWarning("Try to damage something that is not a: DamageType. Adjust collideWith");
        } else {
          owner.Damage(dst);
        }
      }
    }

    void OnTriggerStay2D(Collider2D o) {
      // TODO handle something
    }

    public void OnTriggerExit2D(Collider2D o) {
      // TODO handle something
    }
  }
}
