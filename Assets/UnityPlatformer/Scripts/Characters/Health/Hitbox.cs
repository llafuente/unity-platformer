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

    // this allow to implement diferent HitBoxes depending on the
    // Character State
    [Comment("Disable HitBox when character is any given state")]
    public States disableWhileOnState = States.None;

    #endregion

    internal BoxCollider2D body;

    Damage dt;

    void Start() {
      dt = GetComponent<Damage> ();
      if (type == HitBoxType.DealDamage && dt == null) {
        Debug.LogError("DealDamage require Damage Behaviour", this);
      }

      body = GetComponent<BoxCollider2D>();

      if (type == HitBoxType.EnterAreas) {
        if (owner.character.enterAreas != null && owner.character.enterAreas != this ) {
          Debug.LogWarning("(HitBox) Only one EnterAreas HitBox is allowed!");
        }
        owner.character.enterAreas = this;
      }
    }

    public bool IsDisabled() {
      return owner.character.IsOnAnyState(disableWhileOnState);
    }

#if UNITY_EDITOR
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    void OnDrawGizmos() {
        body = GetComponent<BoxCollider2D>();

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

        Gizmos.DrawWireCube(transform.position + (Vector3)body.offset, body.size);
        //Handles.Label(transform.position + new Vector3(-box.size.x * 0.5f, box.size.y * 0.5f, 0), "HitBox: " + type);
    }
#endif

    void OnTriggerEnter2D(Collider2D o) {

      if (type == HitBoxType.DealDamage) {
        // source disabled?
        if (IsDisabled()) {
          Log.Debug("(HitBox) {0} cannot deal damage it's disabled", this.gameObject.name);
          return;
        }

        //Debug.LogFormat("me {0} of {1} collide with {2}@{3}", name, owner, o.gameObject, o.gameObject.layer);

        var hitbox = o.gameObject.GetComponent<HitBox> ();
        if (hitbox != null && hitbox.type == HitBoxType.RecieveDamage) {
          Log.Debug("(HitBox) Collide {0} with {1}", gameObject.name, hitbox.gameObject.name);
          // target disabled?
          if (hitbox.IsDisabled()) {
            Log.Debug("(HitBox) {0} cannot recieve damage it's disabled", o.gameObject.name);
            return;
          }

          // can I deal damage to this HitBox?
          // check layer
          if (hitbox.collideWith.Contains(gameObject.layer)) {
            // check we not the same, or i can damage to myself at lest
            if (dealDamageToSelf || (!dealDamageToSelf && hitbox.owner != dt.causer)) {
              Log.Debug("(HitBox) Damage to {0}", hitbox.owner.gameObject.name);
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
