using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  public class Polygon2D : IEquatable<Polygon2D>, IEnumerable<Point2D> {
    private List<Point2D> Vertices;
    public readonly int Size;

    public Polygon2D(params Point2D[] points) {
      if (points.Length < 3) {
        throw new ArgumentException("tried to initialize a polygon with less than 3 points");
      }

      Vertices = (new List<Point2D>(points)).RemoveCollinearPoints();
      // input adjustment: correcting mistake of passing collinear points to a polygon
      if (Vertices.Count < 3) {
        throw new ArgumentException("tried to initialize a polygon with less than 3 non-collinear points");
      }

      Size = Vertices.Count;
    }

    public Polygon2D(IEnumerable<Point2D> points) : this(points.ToArray()) {}

    public Point2D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Vertices[i];
      }
    }

    public bool AlmostEquals(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other.Size != Size) {
        return false;
      }
      var points_hashset = Vertices.ToHashSet();  // TODO: better function using decimal_precision
      foreach (var p in other) {
        if (!points_hashset.Contains(p)) {
          return false;
        }
      }

      return true;
    }

    public bool Equals(Polygon2D other) => this.AlmostEquals(other);
    public override bool Equals(object other) => other != null && other is Point2D && this.Equals((Point2D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Polygon2D a, Polygon2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Polygon2D a, Polygon2D b) {
      return !a.AlmostEquals(b);
    }

    public IEnumerator<Point2D> GetEnumerator() {
      return Vertices.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    public double Area() {
      double a = 0;  // risk of overflow ...

      for (int i = 0; i < Vertices.Count - 1; i++) {
        a += Vertices[i].ToVector2D().PerpProduct(Vertices[i + 1].ToVector2D());
      }

      return a / 2;
    }

    public Point2D CenterOfMass() =>
        Point2D.FromVector(Vertices.Select(v => v.ToVector()).Aggregate((v1, v2) => v1 + v2) / Size);

    /// <summary>
    /// The crossing number algorithm as described here: https://en.wikipedia.org/wiki/Point_in_polygon
    /// </summary>
    /// <param name="point"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool Contains(Point2D point, int decimal_precision = Constants.THREE_DECIMALS) {
      int xings = 0;
      var ray = new Ray2D(point, Vector2D.AxisU);  // horizontal ray going to the right

      for (int i = 0; i < Size; i++) {
        var edge = LineSegment2D.FromPoints(Vertices[i], Vertices[(i + 1) % Size], decimal_precision);

        // first edge case: the point might be on one of the edges
        if (edge.Contains(point, decimal_precision)) {
          return true;
        }

        if (edge.ToLine().Direction.AlmostEquals(ray.Direction, decimal_precision)) {
          // rule 3: exclude horizontal edges
          continue;
        }
        // rule 4: edge / ray intersection must happen strictly on the (right) side of the ray
        if (edge.Intersects(ray, decimal_precision)) {
          // rule 1,2: consider the half-edge only (I will use the upward-edge, and exclude intersections
          // occurring on the EndPoint). Because we have used an intersects-function that uses the full-edge, we now
          // have to exclude such edge if the intersection point is the last point (upward-edge)
          if (ray.Contains(edge.P1, decimal_precision)) {
            continue;
          }
          xings++;
        }
      }

      return (xings % 2 == 1);  // true (or false) on number of crossings being odd (or even)
    }

    /// <summary>
    /// Sorts a list of points in CCW order and creates a polygon out of it
    /// </summary>
    /// <param name="points">list of DB.XYZ</param>
    /// <returns></returns>
    public static Polygon2D ConcaveHull(List<Point2D> points) {
      var sorted_points = new List<Point2D>(points);
      sorted_points.SortCCW();

      return new Polygon2D(sorted_points);
    }

    public static Polygon2D ConvexHull(List<Point2D> points) {
      var sorted_points = new List<Point2D>(points);
      sorted_points.SortCCW();

      // pick the lowest point
      int n = points.Count;
      int i0 = 0;
      double v_min = points[i0].V;
      for (int i = 1; i < n; ++i) {
        if (points[i].V < v_min) {
          v_min = points[i].V;
          i0 = i;
        }
      }
      // initialize with the smallest point and the point after
      var cvpoints = new List<Point2D>();
      cvpoints.Add(points[i0 % n]);
      cvpoints.Add(points[(i0 + 1) % n]);

      for (int i = 2; i < n + 1; ++i) {
        cvpoints.Add(points[(i0 + i) % n]);

        int m = cvpoints.Count;
        int i3 = m - 1;
        int i2 = i3 - 1;
        int i1 = i3 - 2;

        // TODO: Works better when (precision - 1 if precision > 0 else precision) when used by point_to_line_location
        while (i1 >= 0 &&
               Line2D.FromTwoPoints(cvpoints[i1], cvpoints[i2]).Location(cvpoints[i3]) != Constants.Location.LEFT) {
          cvpoints[i2] = cvpoints[i3];
          cvpoints.RemoveAt(cvpoints.Count - 1);
          i1 -= 1;
          i2 -= 1;
          i3 -= 1;
        }
      }
      // end condition
      if (cvpoints[0].AlmostEquals(cvpoints[cvpoints.Count - 1])) {
        cvpoints.RemoveAt(cvpoints.Count - 1);
      }

      return new Polygon2D(cvpoints);
    }

    // formatting functions
    public override string ToString() => (Vertices.Count == 0)
                                             ? "{}"
                                             : "{" + string.Join(",", Vertices.Select(v => v.ToString())) +
                                                   "," + Vertices[0].ToString() + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Vertices.Count == 0)
            ? "POLYGON EMPTY"
            : "POLYGON ((" +
                  string.Join(",",
                              Vertices.Select(
                                  v => string.Format(
                                      String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"), v.U, v.V))) +
                  ", " +
                  string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"),
                                Vertices[0].U,
                                Vertices[0].V) +
                  "))";
  }
}
