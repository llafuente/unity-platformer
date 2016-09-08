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
    public bool dealDamageToSelf = false;

    #endregion

    Damage dt;

    void Start() {
      dt = GetComponent<Damage> ();
      if (type == HitBoxType.DealDamage && dt == null) {
        Debug.LogWarning("DealDamage require Damage Behaviour", this);
      }
    }

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
        //Handles.Label(transform.position + new Vector3(-box.size.x * 0.5f, box.size.y * 0.5f, 0), "HitBox: " + type);
    }
#endif

    void OnTriggerEnter2D(Collider2D o) {
      if (type == HitBoxType.DealDamage) {
        //Debug.LogFormat("me {0} of {1} collide with {2}@{3}", name, owner, o.gameObject, o.gameObject.layer);

        var hitbox = o.gameObject.GetComponent<HitBox> ();
        if (hitbox != null && hitbox.type == HitBoxType.RecieveDamage && dt != null) {

          // can I deal damage to this HitBox?
          // check layer
          if (hitbox.collideWith.Contains(gameObject.layer)) {
            // check we not the same, or i can damage to myself at lest
            if (dealDamageToSelf || (!dealDamageToSelf && hitbox.owner != dt.causer)) {
              hitbox.owner.Damage(dt);
            }
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
