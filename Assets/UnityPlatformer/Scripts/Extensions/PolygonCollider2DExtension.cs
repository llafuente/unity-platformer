using UnityEngine;

namespace UnityPlatformer {
  public static class PolygonCollider2DExtension {
    static public Vector2[] GetWorldPoints(this PolygonCollider2D poly) {
      Vector2[] polyPoints = new Vector2[poly.points.Length];

      for (int i = 0; i < polyPoints.Length; ++i) {
        polyPoints[i] = poly.transform.TransformPoint(poly.points[i]);
      }

      return polyPoints;
    }

    static public Vector3[] GetWorldPoints3(this PolygonCollider2D poly, bool close = false) {
      Vector3[] polyPoints = new Vector3[poly.points.Length + (close ? 2 : 0)];

      int i = 0;
      for (; i < poly.points.Length; ++i) {
        polyPoints[i] = (Vector3)poly.transform.TransformPoint(poly.points[i]);
      }

      if (close) {
        polyPoints[i++] = (Vector3)poly.transform.TransformPoint(poly.points[0]);
        polyPoints[i++] = (Vector3)poly.transform.TransformPoint(poly.points[1]);
      }

      return polyPoints;
    }

    // check if the bounds is completely inside, check all four points
    static public bool Contains(this PolygonCollider2D poly, Vector2 pmin, Vector2 pmax) {
      return poly.Contains(pmin) &&
             poly.Contains(pmax) &&
             poly.Contains(new Vector2(pmin.x, pmax.y)) &&
             poly.Contains(new Vector2(pmax.x, pmin.y));
    }

    static public bool Contains(this PolygonCollider2D poly, Bounds b) {
      Vector2 pmin = b.min;
      Vector2 pmax = b.max;

      // check if the bounds is completely inside, check all four points
      return poly.Contains(pmin) &&
             poly.Contains(pmax) &&
             poly.Contains(new Vector2(pmin.x, pmax.y)) &&
             poly.Contains(new Vector2(pmax.x, pmin.y));
    }

    static public bool Contains(this PolygonCollider2D poly, Vector2 p) {
      Vector2[] polyPoints = poly.GetWorldPoints();

      int j = polyPoints.Length - 1;
      bool inside = false;
      for(int i = 0; i < polyPoints.Length ; j = i++) {
        if ((
          (polyPoints[i].y <= p.y && p.y < polyPoints[j].y)  ||
          (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)
          ) && (
            p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x
          )) {
          inside = !inside;
        }
      }

      return inside;
    }

    static bool Intersection(Vector2 p1, Vector2 q1, Vector2  p2, Vector2  q2, out Vector2 res) {
        float eps = 1e-9f;
        float dx1 = p1.x - q1.x;
        float dy1 = p1.y - q1.y;
        float dx2 = p2.x - q2.x;
        float dy2 = p2.y - q2.y;
        float denom = dx1 * dy2 - dy1 * dx2;

        if (Mathf.Abs(denom) < eps) {
          res = Vector2.zero;
          return false;
        }

        float cross1 = p1.x * q1.y - p1.y * q1.x;
        float cross2 = p2.x * q2.y - p2.y * q2.x;
        float px = (cross1 * dx2 - cross2 * dx1) / denom;
        float py = (cross1 * dy2 - cross2 * dy1) / denom;

        res = new Vector2(px, py);
        return true;
    }

    static public bool Intersects(this PolygonCollider2D poly, Vector2 a, Vector2 b, out Vector2 intersection) {
      Vector2[] polyPoints = poly.GetWorldPoints();

      Vector2 itsc;
      for(int i = 0; i < polyPoints.Length - 1; ++i) {
        if (Intersection(polyPoints[i], polyPoints[i + 1], a, b, out itsc)) {
          intersection = itsc;
          return true;
        }
      }

      intersection = Vector2.zero;
      return false;
    }

    static public bool IntersectsTop(this PolygonCollider2D poly, Bounds bounds, out Vector2 intersection) {
      Vector2 topleft = new Vector2(bounds.center.x - bounds.size.x * 0.5f, bounds.min.y + bounds.size.y * 0.5f);
      Vector2 topright = new Vector2(bounds.center.x + bounds.size.x * 0.5f, bounds.min.y + bounds.size.y * 0.5f);

      return poly.Intersects(topleft, topright, out intersection);
    }
  }
}
