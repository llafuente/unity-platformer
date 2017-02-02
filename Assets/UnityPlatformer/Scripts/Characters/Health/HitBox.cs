using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using UnityEngine.Assertions;

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
    Weapon =        1 << 8,
    Projectile =    1 << 9,
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
  /// HitBoxes are triggers that Deal/Revieve Damage or enter areas(tile)\n
  /// DealDamage require Damage MonoBehaviour\n
  /// EnterAreas require Collider2D to be BoxCollider2D
  /// </summary>
  //[RequireComponent (typeof (Rigidbody2D))] // only for DealDamage...
  [RequireComponent (typeof (Collider2D))]
  public class HitBox : MonoBehaviour {
    /// <summary>
    /// Character part this HitBox belong
    /// </summary>
    public CharacterPart part;
    /// <summary>
    /// Flag
    /// </summary>
    public bool useGlobalMask = true;
    /// <summary>
    /// Who can deal damage to me?
    ///
    /// Only used when type=HitBoxType.RecieveDamage.
    /// </summary>
    [DisableIf("useGlobalMask")]
    public LayerMask collisionMask;
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
    /// Combo to check character state
    /// </summary>
    public CharacterStatesCheck characterState;
    /// <summary>
    /// Damage info, only used type=HitBoxType.DealDamage.
    /// </summary>
    internal Damage damage;
    /// <summary>
    /// check missconfigurations and initialize
    /// </summary>
    public void Start() {
      Assert.IsNotNull(owner, "CharacterHealth owner is required at " + gameObject.GetFullName());

      damage = GetComponent<Damage> ();
      if (type == HitBoxType.DealDamage) {
        Assert.IsNotNull(damage, "Missing MonoBehaviour Damage at " + gameObject.GetFullName() + " because typem is DealDamage");
      }

      if (type == HitBoxType.EnterAreas) {
        BoxCollider2D box2d = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(box2d, "Missing MonoBehaviour BoxCollider2D at " + gameObject.GetFullName() + " because typem is EnterAreas");
      }

      if (type == HitBoxType.EnterAreas) {
        if (owner.character.enterAreas != null && owner.character.enterAreas != this ) {
          Debug.LogWarning("Only one EnterAreas HitBox is allowed!");
        }
        owner.character.enterAreas = this;
      }

      Utils.KinematicTrigger(gameObject);
    }
#if UNITY_EDITOR
    /// <summary>
    /// Set layer to Configuration.ropesMask
    /// </summary>
    void Reset() {
      gameObject.layer = Configuration.instance.hitBoxesMask;
      owner = GetComponentInParent<CharacterHealth>();

      Utils.KinematicTrigger(gameObject);
    }
#endif
    /// <summary>
    /// Return if the HitBox is disabled base on enabledOnStates
    /// </summary>
    public bool IsDisabled() {
      return characterState.ValidStates(owner.character);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draw in the Editor mode
    /// </summary>
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

        Utils.DrawCollider2D(gameObject);
        //Handles.Label(transform.position + new Vector3(-box.size.x * 0.5f, box.size.y * 0.5f, 0), "HitBox: " + type);
    }
#endif
    public LayerMask GetCollisionMask() {
      if (useGlobalMask) {
        switch(type) {
        case HitBoxType.DealDamage:
          return Configuration.instance.dealDamageMask;
        case HitBoxType.RecieveDamage:
          return Configuration.instance.recieveDamageMask;
        case HitBoxType.EnterAreas:
          return Configuration.instance.enterAreasMask;
        }
      }

      return collisionMask;
    }
    /// <summary>
    /// I'm a DealDamage, o is RecieveDamage, then Deal Damage to it's owner!
    /// </summary>
    public void OnTriggerEnter2D(Collider2D o) {
Log.Debug("" + o);
      if (type == HitBoxType.DealDamage) {
        // source disabled?
        if (IsDisabled()) {
          Log.Debug("{0} cannot deal damage it's disabled", this.gameObject.GetFullName());
          return;
        }

        //Debug.LogFormat("me {0} of {1} collide with {2}@{3}", name, owner, o.gameObject, o.gameObject.layer);

        var hitbox = o.gameObject.GetComponent<HitBox> ();
        Log.Silly("o is a HitBox? {0} at {0}", hitbox, o.gameObject.GetFullName());
        if (hitbox != null && hitbox.type == HitBoxType.RecieveDamage) {
          Log.Debug("Collide {0} with {1}", gameObject.GetFullName(), hitbox.gameObject.GetFullName());
          // target disabled?
          if (hitbox.IsDisabled()) {
            Log.Debug("{0} cannot recieve damage it's disabled", o.gameObject.GetFullName());
            return;
          }

          // can I deal damage to this HitBox?
          // check layer
          if (hitbox.GetCollisionMask().Contains(gameObject.layer)) {
            Log.Silly("compatible layers");
            // check we not the same, or i can damage to myself at lest
            if (dealDamageToSelf || (!dealDamageToSelf && hitbox.owner != damage.causer)) {
              Log.Debug("Damage to {0} with {1}", hitbox.owner.gameObject.GetFullName(), damage);
              hitbox.owner.Damage(damage);
            }
          } else {
            Log.Silly("incompatible layers {0} vs {1}",
              string.Join(",", hitbox.GetCollisionMask().MaskToNames()),
              LayerMask.LayerToName(gameObject.layer)
            );
          }
        }
      }
    }
  }
}
