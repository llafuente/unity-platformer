using System;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityPlatformer {
  public class Rope : MonoBehaviour, IUpdateEntity {
    public LayerMask passengers;
    /// <summary>
    /// The number of segments..
    /// </summary>
    public int segments;

    /// <summary>
    /// The length of the rope..
    /// </summary>
    public float segmentLength;

    /// <summary>
    /// The prefab to use for each rope section (i.e. put the mesh or sprite here)..
    /// </summary>
    public GameObject sectionPrefab;

    /// <summary>
    /// If true only the bottom section of the rope will be grabable and will have a fixed position..
    /// </summary>
    public bool usedFixedPosition;

    /// <summary>
    /// The mass of each rope segment (except the last which has mass = ropeMass * 5).
    /// </summary>
    public float ropeMass = 1.0f;

    /// <summary>
    /// Min rope angle.
    /// </summary>
    public float angleLimits = 0;
    /// <summary>
    /// rope rotation easing
    /// </summary>
    public EasingType easing = EasingType.Sine;

    public bool stop = false;
    [Range(0, 1)]
    public float initialTime = 0;

    /*
    /// <summary>
    /// Angluar drag per section.
    /// </summary>
    public float angularDrag = 1.0f;
    */
    public float faceDir {
      get {
        return Mathf.Sign(lastAngle);
      }
    }

    /// <summary>
    /// Rotation speed in degrees per second.
    /// </summary>
    [Tooltip ("Time to complete a full rotation from -angleLimits to +angleLimits (seconds)")]
    public float rotationTime = 3.0f;

    /// <summary>
    /// If true we move at a constant speed, false we slow down at the apex.
    /// </summary>
    [Tooltip ("A value between 0 for no slow down at apex and 1 for lots of slow down at apex")]
    [Range(0,1)]
    public float slowDownModifier;

    public delegate void RopeCallback(Rope rope);
    public RopeCallback onSideReached;
    public RopeCallback onBreakRope;

    internal GameObject[] sections;

    int timeSign = 1;
    float time = 0;
    float lastAngle = 0;
    Health health;

    void Start() {
      // create the rope from bottom to top
      // and chain them
      gameObject.transform.DestroyImmediateChildren();

      health = GetComponent<Health>();
      if (health != null) {
        health.onDeath += BreakRope;
      }

      sections = new GameObject[segments];

      GameObject anchor = CreateAnchor();

      // Create segments
      Rigidbody2D nextConnectedBody = anchor.GetComponent<Rigidbody2D>();

      for (int i = 0; i < segments; i++) {
        GameObject section = CreateSection(i, nextConnectedBody);

        // Update for next loop
        nextConnectedBody = section.GetComponent<Rigidbody2D>();
      }

      time = initialTime;
      if (time == 1) {
        timeSign = -1;
      }
      UpdateRotation();
    }


#if UNITY_EDITOR
    /// update the rope on each change...
    void OnValidate() {
      if (transform.parent != null) {
        Debug.LogWarning ("Rope creation may not work correctly if the Rope is a child of another object.");
      }

      if (transform.localScale != Vector3.one) {
        Debug.LogWarning ("Scale cannot be changed");
        transform.localScale = Vector3.one;
      }
    }
#endif

    public float SpeedToSectionOffset(float speed) {
      return speed / segmentLength;
    }

    GameObject CreateAnchor() {
      GameObject anchor = new GameObject ();
      anchor.transform.parent = transform;
      anchor.transform.localPosition = Vector3.zero;
      anchor.name = "RopeAnchor";
      Rigidbody2D anchorRigidbody = anchor.AddComponent<Rigidbody2D> ();
      anchorRigidbody.isKinematic = true;

      return anchor;
    }

    GameObject CreateSection(int i, Rigidbody2D connectedBody) {
      GameObject section;
      float currentLocalYPos = -segmentLength / 2.0f - segmentLength * i;

      if (sectionPrefab != null) {
        section = (GameObject) GameObject.Instantiate(sectionPrefab);
      } else {
        section = new GameObject();
      }
      section.name = "RopeSection_" + i;
      section.layer = gameObject.layer;

      // Set length and position
      section.transform.parent = transform;
      section.transform.localPosition = new Vector3(0, currentLocalYPos, 0);

      Rigidbody2D rb = section.GetOrAddComponent<Rigidbody2D>();
      rb.mass = ropeMass;
      // NOTE this is mandatory atm.
      // the rope movement it's a bit basic, because i cannot have a more stable
      // version
      rb.isKinematic = true;

      BoxCollider2D bc2d = section.GetOrAddComponent<BoxCollider2D>();
      // Default to a 0.5f wide box collider
      bc2d.size = new Vector2(0.5f, segmentLength);
      bc2d.isTrigger = true;

      // Check Hinge Joint
      HingeJoint2D hingeJoint = section.GetOrAddComponent<HingeJoint2D>();
      hingeJoint.anchor = new Vector2(0, 0.5f);
      hingeJoint.connectedAnchor = new Vector2(0, i == 0 ? 0.0f : -0.5f);
      hingeJoint.connectedBody = connectedBody;
      if (angleLimits > 0) {
        hingeJoint.useLimits = true;
        JointAngleLimits2D limits = new JointAngleLimits2D();
        limits.min = angleLimits;
        limits.max = -angleLimits;
        hingeJoint.limits = limits;
      }

      // this will handle the player entering the rope
      RopeSection rs = section.AddComponent<RopeSection>();
      rs.rope = this;
      rs.index = i;

      if (health != null) {
        // NOTE add/get the HitBox so it can be destroyed
        // if we add a hitbox, it's just useless we need a proper collideWith
        // configured
        HitBox hitbox = section.GetOrAddComponent<HitBox>();
        hitbox.owner = health;
        //hitbox.type = HitBoxType.RecieveDamage;
        //hitbox.collideWith = recieveDamage;
      }

      // Special case, for last section
      if (i == segments - 1) {
        rb.mass = ropeMass * 5;
      }

      sections[i] = section;
      return section;
    }

    /// <summary>
    /// Gets the rope section above the provided one or null if none.
    /// </summary>
    /// <returns>The section below.</returns>
    virtual public GameObject GetSectionAbove(GameObject section) {
      int index = Array.IndexOf(sections, section);
      if (index > 0) return sections[index - 1];
      return null;
    }

    /// <summary>
    /// Gets the rope section below the provided one or null if none.
    /// </summary>
    /// <returns>The section below.</returns>
    virtual public GameObject GetSectionBelow(GameObject section) {
      int index = Array.IndexOf(sections, section);
      if (index < segments - 1) return sections[index + 1];
      return null;
    }

    public virtual void OnEnable() {
      UpdateManager.instance.Push(this, Configuration.instance.movingPlatformsPriority);
    }

    public virtual void OnDisable() {
      UpdateManager.instance.Remove(this);
    }

    public void UpdateRotation() {
      // rotate only top section
      // rope isKinematic atm, not realistic but works perfectly
      // movement need more work, but rotate the hinge it's a nightmare!
      GameObject obj = sections[0];

      if (time > 1) {
        time = 1;
        timeSign = -1;
        if (onSideReached != null) {
          onSideReached(this);
        }
      } else if (time < 0) {
        time = 0;
        timeSign = 1;
        if (onSideReached != null) {
          onSideReached(this);
        }
      }

      float factor = Easing.EaseInOut(time, easing);
      lastAngle = factor * 2 * angleLimits - angleLimits;

      // reset first
      transform.rotation = Quaternion.identity;
      transform.RotateAround(
        obj.GetComponent<HingeJoint2D>().connectedBody.transform.position,
        new Vector3(0, 0, -1),
        lastAngle
      );
    }

    /// <summary>
    /// Rope motion!
    /// </summary>
    public virtual void ManagedUpdate(float delta) {
      if (stop) return;

      time += delta * timeSign / rotationTime;
      UpdateRotation();
    }

    public void BreakRope() {
      if (onBreakRope != null) {
        onBreakRope(this);
      }

      Debug.Log("BreakRope");

      stop = true;

      gameObject.transform.DestroyChildren();
    }


    #if UNITY_EDITOR
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    void OnDrawGizmos() {
      Gizmos.color = Color.green;
      float height = segments * segmentLength;

      var c =  Mathf.Cos(angleLimits * Mathf.Deg2Rad);
      var s =  Mathf.Sin(angleLimits * Mathf.Deg2Rad);

      Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, height));
      Gizmos.DrawLine(transform.position,
        transform.position + new Vector3(s * height, -c * height));
      Gizmos.DrawLine(transform.position,
        transform.position + new Vector3(-s * height, -c * height));
    }
    #endif
  }
}
