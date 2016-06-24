using System;
using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  public class Rope : MonoBehaviour, IUpdateEntity {
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
    /// Angluar drag per section.
    /// </summary>
    public float angularDrag = 1.0f;

    public float faceDir {
      get {
        return Mathf.Sign(rotationSpeed);
      }
    }

    /// <summary>
    /// Rotation speed in degrees per second.
    /// </summary>
    [Tooltip ("Rotation speed in degrees per second.")]
    public float rotationSpeed = 33.0f;

    /// <summary>
    /// If true we move at a constant speed, false we slow down at the apex.
    /// </summary>
    [Tooltip ("A value between 0 for no slow down at apex and 1 for lots of slow down at apex")]
    [Range(0,1)]
    public float slowDownModifier;

    [HideInInspector]
    public GameObject[] sections;

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

      Rigidbody2D rb = Utils.GetOrAddComponent<Rigidbody2D>(section);
      rb.mass = ropeMass;
      // NOTE this is mandatory atm.
      // the rope movement it's a bit basic, because i cannot have a more stable
      // version
      rb.isKinematic = true;

      BoxCollider2D bc2d = Utils.GetOrAddComponent<BoxCollider2D>(section);
      // Default to a 0.5f wide box collider
      bc2d.size = new Vector2(0.5f, segmentLength);
      bc2d.isTrigger = true;

      // Check Hinge Joint
      HingeJoint2D hingeJoint = Utils.GetOrAddComponent<HingeJoint2D>(section);
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

      // Special case, for last section
      if (i == segments - 1) {
        rb.mass = ropeMass * 5;
      }

      sections[i] = section;
      return section;
    }

    void Start() {
      // create the rope from bottom to top
      // and chain them
      Utils.DestroyImmediateChildren(gameObject.transform);

      sections = new GameObject[segments];

      GameObject anchor = CreateAnchor();

      // Create segments
      Rigidbody2D nextConnectedBody = anchor.GetComponent<Rigidbody2D>();

      for (int i = 0; i < segments; i++) {
        GameObject section = CreateSection(i, nextConnectedBody);

        // Update for next loop
        nextConnectedBody = section.GetComponent<Rigidbody2D>();
      }
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
      UpdateManager.instance.others.Add(this);
    }

    public virtual void OnDisable() {
      UpdateManager.instance.others.Remove(this);
    }

    /// <summary>
    /// Rope motion!
    /// </summary>
    public virtual void ManagedUpdate(float delta) {
      // rotate only top section
      // rope isKinematic atm, not realistic but works perfectly
      // movement need more work, but rotate the hinge it's a nightmare!
      GameObject obj = sections[0];

      float deg;
      float actualSpeed = rotationSpeed;
      if (slowDownModifier > 0) {
        deg = Utils.NormalizeDegree(transform.eulerAngles.z);
        float modifier = (90.0f / angleLimits) * slowDownModifier;
        float factor = Mathf.Abs(Mathf.Cos(deg * Mathf.Deg2Rad) * modifier);
        actualSpeed *= factor;
      }


      transform.RotateAround(
        obj.GetComponent<HingeJoint2D>().connectedBody.transform.position,
        new Vector3(0, 0, -1),
        delta * actualSpeed
      );
      deg = Utils.NormalizeDegree(transform.eulerAngles.z);
      //Utils.DrawZAngle(transform.position, deg);

      // go-back
      if (deg >= angleLimits) {
        rotationSpeed = Mathf.Sign(rotationSpeed) == 1 ? rotationSpeed : -rotationSpeed;
      } else if (deg <= -angleLimits) {
        rotationSpeed = Mathf.Sign(rotationSpeed) == -1 ? rotationSpeed : -rotationSpeed;
      }
    }
  }
}
