using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  public class Polygon3D : IEquatable<Polygon3D>, IEnumerable<Point3D> {
    public List<Point3D> Vertices { get; }
    public readonly int Size;
    public UnitVector3D Normal { get; }

    private Polygon3D(Point3D[] points, int decimal_precision = Constants.THREE_DECIMALS) {
      if (points.Length < 3) {
        throw new ArgumentException("tried to initialize a polygon with less than 3 points");
      }
      Vertices = (new List<Point3D>(points)).RemoveCollinearPoints(decimal_precision);
      // input adjustment: correcting mistake of passing collinear points to a polygon
      if (Vertices.Count < 3) {
        throw new ArgumentException("tried to initialize a polygon with less than 3 non-collinear points");
      }
      // adding the size
      Size = Vertices.Count;

      // adding the normal
      var centroid = Vertices.Average();
      Normal = (Vertices[0] - centroid).CrossProduct((Vertices[1] - centroid)).Normalize();

      // verify that all points are on the same plane!
      for (int i1 = 2; i1 < Size; i1++) {
        int i2 = (i1 + 1) % Size;
        var local_normal = (Vertices[0] - centroid).CrossProduct((Vertices[1] - centroid));
        if (!local_normal.CrossProduct(Normal).AlmostEquals(Vector3D.Zero, decimal_precision)) {
          throw new ArgumentException(
              "tried to initialize a Polygon3D with a set of points that do not belong to the same plane");
        }
      }
    }

    public Polygon3D(IEnumerable<Point3D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    public Point3D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Vertices[i];
      }
    }

    public double Area() {
      var plane = Plane.FromPointAndNormal(Vertices[0], Normal);

      // transform the problem into a 2D one (a polygon is a planar geometry after all)
      return new Polygon2D(Vertices.Select(v => plane.ProjectInto(v))).Area();
    }

    public Point3D CenterOfMass() =>
        Point3D.FromVector(Vertices.Select(v => v.ToVector()).Aggregate((v1, v2) => v1 + v2) / Size);

    public bool AlmostEquals(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      // different number of points, different polygon (we assume they have been built removing collinear points and
      // duplicates (constructor guarantees that)
      if (other.Size != Size) {
        return false;
      }
      // different normal, different plane, different polygon
      if (!Normal.AlmostEquals(other.Normal, decimal_precision)) {
        return false;
      }

      // different area, different polygon
      if (Math.Round(Area() - other.Area(), decimal_precision) != 0) {
        return false;
      }

      // different set of points, different polygons
      //    function to return the index of the first point of the list equal to the given point
      Func<List<Point3D>, Point3D, int> GetFirstEqualPoint = (List<Point3D> _vertices, Point3D _point) => {
        for (int _i = 0; _i < _vertices.Count; _i++) {
          if (_vertices[_i].AlmostEquals(_point, decimal_precision)) {
            return _i;
          }
        }
        return -1;
      };
      //    no equal point found
      int first_equal_idx = GetFirstEqualPoint(other.Vertices, other[0]);
      if (first_equal_idx < 0) {
        return false;
      }
      //    test point by point
      for (int i = 0; i < Size; ++i) {
        int j = (first_equal_idx + i) % Size;
        if (!Vertices[i].AlmostEquals(other.Vertices[j], decimal_precision)) {
          return false;
        }
      }

      return true;
    }
    public bool Equals(Polygon3D other) => this.AlmostEquals(other);
    public override bool Equals(object other) => other != null && other is Point3D && this.Equals((Point3D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Polygon3D a, Polygon3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Polygon3D a, Polygon3D b) {
      return !a.AlmostEquals(b);
    }

    public IEnumerator<Point3D> GetEnumerator() {
      return Vertices.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    public (Point3D Min, Point3D Max)
        BoundingBox() => (new Point3D(Vertices.Min(v => v.X), Vertices.Min(v => v.Y), Vertices.Min(v => v.Z)),
                          new Point3D(Vertices.Max(v => v.X), Vertices.Max(v => v.Y), Vertices.Max(v => v.Z)));

    /// <summary>
    /// The crossing number algorithm as described here: https://en.wikipedia.org/wiki/Point_in_polygon
    /// </summary>
    /// <param name="point"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool Contains(Point3D point, int decimal_precision = Constants.THREE_DECIMALS) {
      // first check to save time, if the point is outside the plane, then it's outside the polygon too
      // improvement to the algorithm's big-O
      var plane = Plane.FromPointAndNormal(Vertices[0], Normal, decimal_precision);
      if (!plane.Contains(point, decimal_precision)) {
        return false;
      }

      // transform the problem into a 2D one (a polygon is a planar geometry after all)
      return new Polygon2D(Vertices.Select(v => plane.ProjectInto(v)))
          .Contains(plane.ProjectInto(point), decimal_precision);
    }

    // special formatting
    public override string ToString() => (Vertices.Count == 0)
                                             ? "{}"
                                             : "{" + string.Join(",", Vertices.Select(v => v.ToString())) +
                                                   "," + Vertices[0].ToString() + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Vertices.Count == 0)
            ? "POLYGON EMPTY"
            : "POLYGON ((" +
                  string.Join(
                      ",",
                      Vertices.Select(v => string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}",
                                                                       "{",
                                                                       precision,
                                                                       "}"),
                                                         v.X,
                                                         v.Y,
                                                         v.Z))) +
                  ", " +
                  string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                                Vertices[0].X,
                                Vertices[0].Y,
                                Vertices[0].Z) +
                  "))";
  }

}
