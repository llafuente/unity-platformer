using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
  [RequireComponent (typeof (BoxCollider2D))]
  public class HitBox : MonoBehaviour {
    #region public

    public CharacterPart part;
    [Comment("Who can deal damage to me?")]
    public LayerMask collideWith;
    [Comment("Who am I?")]
    public CharacterHealth owner;
    public HitBoxType type = HitBoxType.DealDamage;

    #endregion

#if UNITY_EDITOR
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    void OnDrawGizmos() {

        switch(type) {
        case HitBoxType.DealDamage:
          Gizmos.color = Color.red;
          break;
        case HitBoxType.RecieveDamage:
          Gizmos.color = Color.yellow;
          break;
        case HitBoxType.EnterAreas:
          Gizmos.color = Color.green;
          break;
        }

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, box.size);
    }
#endif

    void OnTriggerEnter2D(Collider2D o) {
      //Debug.Log(this.name + " collide with: " + o.gameObject);
      if (collideWith.Contains(o.gameObject.layer)) {
        var dst_hb = o.gameObject.GetComponent<HitBox> ();

        // do not deal damage to 'myself' and hitbox deal damage
        //if (dst_hb && dst_hb.owner != owner && dst_hb.type == HitBoxType.DealDamage) {
        if (dst_hb && dst_hb.type == HitBoxType.DealDamage) {
          var dst = o.gameObject.GetComponent<DamageType> ();
          if (dst == null) {
            Debug.LogWarning("Try to damage something that is not a: DamageType. Adjust collideWith or type (HitBoxType.DealDamage)");
          } else if (owner != dst.causer) {
            owner.Damage(dst);
          }
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
