using UnityEngine;
using UnityEngine.UI;

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
    internal Vector3 direction;
    /// <summary>
    /// check missconfiguration
    /// </summary>
    void Start() {
      if (causer == null) {
        Debug.LogWarning("causer cannot be null", this);
      }
    }
  }
}
