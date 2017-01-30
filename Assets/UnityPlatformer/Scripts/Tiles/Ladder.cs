using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

// TODO use BoxTileTrigger?

namespace UnityPlatformer {
  /// <summary>
  /// Ladder tile. Climb ladder
  /// </summary>
  [RequireComponent (typeof (BoxCollider2D))]
  public class Ladder : MonoBehaviour {
    /// <summary>
    /// true: Top is reachable (Ladder)
    /// false: Top is not reachable (Vine)
    /// </summary>
    [Comment("Top is reachable (true) blocked (false).")]
    public bool topDismount = true;
    /// <summary>
    /// true: Bottom is reachable (Ladder, Fall)
    /// false: Bottom is not reachable (Vine)
    /// </summary>
    [Comment("Bottom is reachable (true) blocked (false).")]
    public bool bottomDismount = true;
    /// <summary>
    /// BoxCollider2D
    /// </summary>
    internal BoxCollider2D body;
    /// <summary>
    /// get BoxCollider2D
    /// </summary>
    virtual public void Start() {
      body = GetComponent<BoxCollider2D>();
    }
#if UNITY_EDITOR
    /// <summary>
    /// Set layer to Configuration.laddersMask
    /// </summary>
    void Reset() {
      gameObject.layer = Configuration.instance.laddersMask;
    }
#endif
    /// <summary>
    /// get real-world-coordinates top of the ladder
    /// </summary>
    virtual public Vector3 GetTop() {
      return body.bounds.center + new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }
    /// <summary>
    /// get real-world-coordinates bottom of the ladder
    /// </summary>
    virtual public Vector3 GetBottom() {
      return body.bounds.center - new Vector3(0, body.bounds.size.y * 0.5f, 0);
    }
    /// <summary>
    /// Return if the character at given position will be above the top
    /// this is used to know before moving the character is will reach the top in
    /// the next frame
    /// </summary>
    virtual public bool IsAboveTop(Character c, Vector2 pos) {
      return pos.y > (GetTop().y - c.minDistanceToEnv) + 0.001f;
    }
    /// <summary>
    /// Return if the Character is very close to the top
    /// </summary>
    virtual public bool IsAtTop(Character c, Vector2 pos) {
      return Mathf.Abs(pos.y - GetTop().y) - c.minDistanceToEnv < 0.001f;
    }
    /// <summary>
    /// Return if the character at given position will be above the botom
    /// this is used to know before moving the character is will reach the
    /// botom in the next frame
    /// </summary>
    virtual public bool IsBelowBottom(Character c, Vector2 pos) {
      return pos.y < (GetBottom().y + c.minDistanceToEnv) - 0.001f;
    }
    /// <summary>
    /// Return if the Character is very close to the bottom
    /// </summary>
    virtual public bool IsAtBottom(Character c, Vector2 pos) {
      float bottomY = GetBottom().y;

      return Mathf.Abs(pos.y - bottomY) < c.skinWidth;
    }
    /// <summary>
    /// Notify Character is on ladder area
    /// </summary>
    virtual public void EnableLadder(Character p) {
      p.ladder = this;
      p.EnterArea(Areas.Ladder);
    }
    /// <summary>
    /// Notify Character to exit ladder state
    /// For example if character jumps.
    /// </summary>
    virtual public void Dismount(Character p) {
      p.ExitState(States.Ladder);
    }
    /// <summary>
    /// Notify Character to exit ladder area
    /// </summary>
    virtual public void DisableLadder(Character p) {
      Dismount(p);
      p.ExitArea(Areas.Ladder);
      p.ladder = null;
    }
    /// <summary>
    /// if a hitbox (EnterAreas) enters -> EnableLadder
    /// </summary>
    public virtual void OnTriggerEnter2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        EnableLadder(h.owner.GetComponent<Character>());
      }
    }
    /// <summary>
    /// if a hitbox (EnterAreas) lave -> DisableLadder
    /// </summary>
    public virtual void OnTriggerExit2D(Collider2D o) {
      HitBox h = o.GetComponent<HitBox>();
      if (h && h.type == HitBoxType.EnterAreas) {
        DisableLadder(h.owner.GetComponent<Character>());
      }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draw in the editor
    /// </summary>
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    void OnDrawGizmos() {
      if (Application.isPlaying) return;

      body = GetComponent<BoxCollider2D>();

      Vector3 size = body.size;
      Vector3 pos = transform.position + (Vector3)body.offset;

      Handles.Label(pos + new Vector3(-size.x * 0.5f, size.y * 0.5f, 0), "Ladder");

      Vector3[] verts = new Vector3[] {
        new Vector3(pos.x - size.x * 0.5f, pos.y - size.y * 0.5f, pos.z),
        new Vector3(pos.x - size.x * 0.5f, pos.y + size.y * 0.5f, pos.z),
        new Vector3(pos.x + size.x * 0.5f, pos.y + size.y * 0.5f, pos.z),
        new Vector3(pos.x + size.x * 0.5f, pos.y - size.y * 0.5f, pos.z)
      };
      Handles.DrawSolidRectangleWithOutline(verts, new Color(1, 1, 0, 0.05f), new Color(0, 0, 0, 0.5f));
    }
#endif
  }
}
