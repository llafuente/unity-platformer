using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

/// <summary>
/// Create a mesh for given Collider2D and apply material.
/// NOTE: MeshFilter and MeshRenderer are hidden
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Collider2D))]
public class Collider2DRenderer : MonoBehaviour
{
  public Material material
  {
    get { return _material; }
    set
    {
      _material = value;
      _mr.sharedMaterial = _material;
    }
  }

  static protected Material _default_material;
  protected Material _material;
  protected MeshFilter _mf;
  protected MeshRenderer _mr;

  // clang-format on
  void Start()
  {

    var castedTarget = (target as TestHide);
    castedTarget.GetComponent<BoxCollider>()

    if (_default_material == null)
    {
      // TODO fixit!
      //_default_material = Resources.Load("Transparent", typeof(Material)) as
      // Material;
      _default_material = AssetDatabase.LoadAssetAtPath<Material>(
          "Noboxout/Utils/Materials/Collider2D.mat");
      //_default_material = new Material(Shader.Find("Transparent"));
    }

    if (_default_material == null)
    {
      Debug.LogWarning("cannot load Transparent material!");
    }

    _mf = GetComponent<MeshFilter>();
    if (_mf == null)
    {
      _mf = gameObject.AddComponent<MeshFilter>();
      _mf.hideFlags = HideFlags.HideInInspector;
    }

    _mr = GetComponent<MeshRenderer>();
    if (_mr == null)
    {
      _mr = gameObject.AddComponent<MeshRenderer>();

      _mr.sharedMaterial = material != null ? material : _default_material;
      _mr.receiveShadows = false;
      _mr.shadowCastingMode = ShadowCastingMode.Off;
      _mr.reflectionProbeUsage = ReflectionProbeUsage.Off;

      _mr.hideFlags = HideFlags.HideInInspector;
    }

    Update();
  }

  void Update()
  {
    Mesh mesh = new Mesh();

    PolygonCollider2D _polygon2d = gameObject.GetComponent<PolygonCollider2D>();
    BoxCollider2D _box2d = gameObject.GetComponent<BoxCollider2D>();
    CircleCollider2D _circle2d = gameObject.GetComponent<CircleCollider2D>();
    EdgeCollider2D _edge2d = gameObject.GetComponent<EdgeCollider2D>();

    if (_polygon2d)
    {
      // points are alredy rotated :)
      int pointCount = _polygon2d.GetTotalPointCount();
      Vector2[] points = _polygon2d.points;

      Vector3[] vertices = new Vector3[pointCount];
      for (int j = 0; j < pointCount; j++)
      {
        Vector2 actual = points[j];
        vertices[j] = new Vector3(actual.x, actual.y, 0);
      }
      Triangulator tr = new Triangulator(points);
      int[] triangles = tr.Triangulate();
      mesh.vertices = vertices;
      mesh.triangles = triangles;
    }

    if (_box2d)
    {
      mesh.vertices = GetBoxCorners(_box2d);
      int[] triangles = {0, 1, 2, 1, 3, 2};
      mesh.triangles = triangles;
    }

    if (_circle2d)
    {
      float scale = 1f / 16f;

      Vector3[] vertices = new Vector3[16];
      Vector2[] points = new Vector2[16];
      for (int j = 0; j < 16; j++)
      {
        float x = (_circle2d.offset.x +
                   Mathf.Cos(scale * j * 2 * Mathf.PI) * _circle2d.radius) *
                  _circle2d.transform.localScale.x;
        float y = (_circle2d.offset.y +
                   Mathf.Sin(scale * j * 2 * Mathf.PI) * _circle2d.radius) *
                  _circle2d.transform.localScale.y;
        points[j] = new Vector2(x, y);
        vertices[j] = new Vector3(x, y, 0);
      }
      Triangulator tr = new Triangulator(points);
      int[] triangles = tr.Triangulate();
      mesh.vertices = vertices;
      mesh.triangles = triangles;
    }

    if (_edge2d)
    {
      Debug.LogWarning("EdgeCollider2D is not supported");
    }

    _mf.mesh = mesh;
  }

  // Assign the collider in the inspector or elsewhere in your code
  Vector3[] GetBoxCorners(BoxCollider2D box)
  {

    Transform bcTransform = box.transform;

    // The collider's local width and height, accounting for scale, divided by 2
    Vector2 size = new Vector2(box.size.x * bcTransform.localScale.x * 0.5f,
                               box.size.y * bcTransform.localScale.y * 0.5f);

    // Find the 4 corners of the BoxCollider2D in LOCAL space, if the
    // BoxCollider2D had never been rotated
    Quaternion rotationInverse = Quaternion.Inverse(transform.rotation);
    Vector3 corner1 = rotationInverse * new Vector2(-size.x + box.offset.x,
                                                    -size.y + box.offset.y);
    Vector3 corner2 = rotationInverse * new Vector2(-size.x + box.offset.x,
                                                    size.y + box.offset.y);
    Vector3 corner3 = rotationInverse * new Vector2(size.x + box.offset.x,
                                                    -size.y + box.offset.y);
    Vector3 corner4 = rotationInverse *
                      new Vector2(size.x + box.offset.x, size.y + box.offset.y);

    // Rotate those 4 corners around the centre of the collider to match its
    // transform.rotation
    corner1 =
        RotatePointAroundPivot(corner1, Vector3.zero, bcTransform.eulerAngles);
    corner2 =
        RotatePointAroundPivot(corner2, Vector3.zero, bcTransform.eulerAngles);
    corner3 =
        RotatePointAroundPivot(corner3, Vector3.zero, bcTransform.eulerAngles);
    corner4 =
        RotatePointAroundPivot(corner4, Vector3.zero, bcTransform.eulerAngles);

    Vector3[] ret = new Vector3[4];
    ret[0] = corner1;
    ret[1] = corner2;
    ret[2] = corner3;
    ret[3] = corner4;

    return ret;
  }

  // Helper method courtesy of @aldonaletto
  // http://answers.unity3d.com/questions/532297/rotate-a-vector-around-a-certain-point.html
  Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
  {
    Vector3 dir = point - pivot; // get point direction relative to pivot
    dir = Quaternion.Euler(angles) * dir; // rotate it
    point = dir + pivot;                  // calculate rotated point
    return point;                         // return it
  }
}
