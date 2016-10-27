using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;

namespace UnityPlatformer {
  /// <summary>
  /// List of part a character can have, maybe needed to handle
  /// how to reach to certain Damages
  /// </summary>
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
  /// <summary>
  /// Type of HitBoxe, what it do
  /// </summary>
  public enum HitBoxType {
    DealDamage,
    RecieveDamage,
    EnterAreas
  }

  /// <summary>
  /// HitBoxes are triggers that Deal/Revieve Damage or enter areas(tile)
  /// </summary>
  [RequireComponent (typeof (BoxCollider2D))]
  public class HitBox : MonoBehaviour {
    /// <summary>
    /// Character part this HitBox belong
    /// </summary>
    public CharacterPart part;
    /// <summary>
    /// Who can deal damage to me?
    ///
    /// Only used when type=HitBoxType.RecieveDamage.
    /// </summary>
    [Comment("Who can deal damage to me?")]
    public LayerMask collideWith;
    /// <summary>
    /// HitBox owner
    /// </summary>
    [Comment("Who am I?")]
    public CharacterHealth owner;
    /// <summary>
    /// HitBoxType
    /// </summary>
    public HitBoxType type = HitBoxType.DealDamage;
    /// <summary>
    /// Can i deal damage to myself?
    /// </summary>
    public bool dealDamageToSelf = false;

    /// <summary>
    /// this allow to implement diferent HitBoxes depending on the
    /// Character State
    ///
    /// For example have a RecieveDamage while standing and other while crouching
    /// </summary>
    [Comment("Disable HitBox when character is any given state")]
    public States disableWhileOnState = States.None;
    /// <summary>
    /// BoxCollider2D
    /// </summary>
    internal BoxCollider2D body;
    /// <summary>
    /// Damage info, only used type=HitBoxType.DealDamage.
    /// </summary>
    internal Damage dt;
    /// <summary>
    /// Report missconfigurations and initialize
    /// </summary>
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
    /// <summary>
    /// Return if the HitBox is disabled base on disableWhileOnState
    /// </summary>
    public bool IsDisabled() {
      return owner.character.IsOnAnyState(disableWhileOnState);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draw in the Editor mode
    /// </summary>
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
    /// <summary>
    /// I'm a DealDamage, o is RecieveDamage, then Deal Damage to it's owner!
    /// </summary>
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
  }
}
