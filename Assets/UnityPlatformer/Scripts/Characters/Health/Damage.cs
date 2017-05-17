using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace UnityPlatformer {
  /// <summary>
  /// Damage info
  /// </summary>
  public class Damage : MonoBehaviour {
    /// <summary>
    /// Damage amount
    /// </summary>
    [Comment("Damage amount")]
    public int amount = 1;
    /// <summary>
    /// Damage type
    /// </summary>
    public DamageType type = DamageType.Default;
    /// <summary>
    /// who cause this damage
    /// </summary>
    public CharacterHealth causer;
    /// <summary>
    /// TODO direction will be calc, someday :)
    /// </summary>
    [HideInInspector]
    public Vector3 direction;
    /// <summary>
    /// Can deal Damage to friends (same alignment)\n
    /// NOTE: that require CharacterHealth.friendlyFire to be on.
    /// </summary>
    public bool friendlyFire = false;
    /// <summary>
    /// check missconfiguration
    /// </summary>
    public void Start() {
      Assert.IsNotNull(causer, "(Damage) causer cannot be null at " + gameObject.GetFullName());

      // REVIEW this may not be necessary...
      HitBox hitbox = GetComponent<HitBox> ();
      Assert.IsNotNull(hitbox, "(Damage) Missing MonoBehaviour HitBox at " + gameObject.GetFullName());
      if (hitbox.type != HitBoxType.DealDamage) {
        Assert.IsNotNull(null, "(Damage) Damage found but hitbox type is not DealDamage at " + gameObject.GetFullName());
      }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Set layer to Configuration.ropesMask
    /// </summary>
    void Reset() {
      if (causer == null) {
        causer = GetComponentInParent<CharacterHealth>();
      }
    }
#endif
  }
}
