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
      if (points.Length < 4) {
        throw new ArgumentException("tried to initialize a polygon with less than 4 points");
      }
      Vertices = (new List<Point3D>(points)).RemoveCollinearPoints(decimal_precision);
      // input adjustment: correcting mistake of passing collinear points to a polygon
      if (Vertices.Count < 4) {
        throw new ArgumentException("tried to initialize a polygon with less than 4 non-collinear points");
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

    public Plane RefPlane() => Plane.FromPointAndNormal(Vertices[0], Normal);

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
    public override bool Equals(object other) => other != null && other is Polygon3D && this.Equals((Polygon3D)other);

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

    public bool Intersects(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public IntersectionResult Intersection(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var plane = RefPlane();
      var other_plane = other.RefPlane();

      // check if planes intersect into a line
      var plane_intersection = plane.Intersection(other_plane, decimal_precision);
      if (plane_intersection.ValueType != typeof(Line3D)) {
        return new IntersectionResult();
      }
      var line_inter = (Line3D)plane_intersection.Value;

      // check if the line intersection of the two planes passes through both polygons
      //    intersection with polygon 1
      var line_inter_2D_on_poly = Line2D.FromPoints(plane.ProjectInto(line_inter.Origin),
                                                    plane.ProjectInto(line_inter.Origin + line_inter.Direction),
                                                    decimal_precision);
      var poly_2D = new Polygon2D(Vertices.Select(v => plane.ProjectInto(v)));
      var line_inter_on_poly_2D = poly_2D.Intersection(line_inter_2D_on_poly, decimal_precision);
      if (line_inter_on_poly_2D.ValueType != typeof(LineSegment2D)) {
        return new IntersectionResult();
      }
      var seg_inter_on_poly_2D = (LineSegment2D)line_inter_on_poly_2D.Value;
      var seg_inter = LineSegment3D.FromPoints(plane.Evaluate(seg_inter_on_poly_2D.P0),
                                               plane.Evaluate(seg_inter_on_poly_2D.P1),
                                               decimal_precision);
      //    intersection with polygon 2
      var seg_inter_on_other_2D = LineSegment2D.FromPoints(other_plane.ProjectInto(seg_inter.P0),
                                                           other_plane.ProjectInto(seg_inter.P1),
                                                           decimal_precision);
      var other_2D = new Polygon2D(other.Vertices.Select(v => other_plane.ProjectInto(v)));
      //      both vertices contained ? this is the smallest intersection LineSegment
      if (other_2D.Contains(seg_inter_on_other_2D.P0, decimal_precision) &&
          other_2D.Contains(seg_inter_on_other_2D.P1, decimal_precision)) {
        return new IntersectionResult(seg_inter);
      }
      var other_seg_inter_2D = other_2D.Intersection(seg_inter_on_other_2D, decimal_precision);
      //      no intersection ? polygon crosses the plane but not the other polygon
      if (other_seg_inter_2D.ValueType != typeof(LineSegment2D)) {
        return new IntersectionResult();
      }
      //      smaller than that segment intersection ? save it and return it
      var smaller_inter_on_other_2D = (LineSegment2D)other_seg_inter_2D.Value;
      var smaller_inter_on_other = LineSegment3D.FromPoints(other_plane.Evaluate(smaller_inter_on_other_2D.P0),
                                                            other_plane.Evaluate(smaller_inter_on_other_2D.P1),
                                                            decimal_precision);
      return new IntersectionResult(smaller_inter_on_other);
    }

    /// <summary>
    /// If the list of points is not all lying on a planar surface, this planar surface will be approximated by the
    /// average plane crossing all points
    /// </summary>
    /// <param name="points">any enumeration of 3D Points</param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public static Plane ApproxPlane(IEnumerable<Point3D> points, int decimal_precision = Constants.THREE_DECIMALS) {
      Func<Point3D> CenterOfMass = () => {
        var _v = new Vector(new double[] { 0, 0, 0 });
        int _n = points.Count();
        if (_n < 3) {
          throw new ArgumentException("ApproxPlane called with less than 3 points");
        }
        foreach (var p in points) {
          _v += p.ToVector() / _n;
        }

        return Point3D.FromVector(_v);
      };

      // TODO: pre-sort CCW on a given plane, so that the average normal computed below will go in the same direction!

      var cm = CenterOfMass();
      int n = points.Count();
      var avg_norm_vec = new Vector(new double[] { 0, 0, 0 });
      var point_list = new List<Point3D>(points);
      int divisor = n;
      for (int i = 0; i <= n; i++) {
        (int i1, int i2) = (i % n, (i + 1) % n);
        if (point_list[i1].AlmostEquals(point_list[i2], decimal_precision)) {
          ++i;
          --divisor;
        } else {
          avg_norm_vec += Plane.FromPoints(cm, point_list[i1], point_list[i2]).Normal.ToVector();
        }
      }

      if (divisor < 3) {
        throw new ArgumentException(
            "ApproxPlane: too many duplicate points (less than 3 unique points with decimal_precision=" +
            decimal_precision.ToString());
      }
      avg_norm_vec /= divisor;
      return Plane.FromPointAndNormal(cm, Vector3D.FromVector(avg_norm_vec).Normalize(), decimal_precision);
    }

    /// <summary>
    /// Sorts a list of points in CCW order and creates a polygon out of it
    /// If the list of points is not all lying on a planar surface, this planar surface will be approximated by the
    /// average plane crossing all points
    /// </summary>
    /// <param name="points">any enumeration of 3D Points</param>
    /// <returns></returns>
    public static Polygon3D ConcaveHull(IEnumerable<Point3D> points) {
      if (points.Count() < 3) {
        throw new ArgumentException("tried to create a Concave Hull with less than 3 points");
      }

      var plane = ApproxPlane(points);

      var poly_2d = Polygon2D.ConcaveHull(points.Select(p => plane.ProjectInto(p)));

      return (poly_2d is null) ? null : new Polygon3D(poly_2d.Select(p => plane.Evaluate(p)));
    }

    /// <summary>
    /// Computes the convex hull of any point enumeration
    /// If the list of points is not all lying on a planar surface, this planar surface will be approximated by the
    /// average plane crossing all points
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static Polygon3D ConvexHull(List<Point3D> points, int decimal_precision = Constants.THREE_DECIMALS) {
      if (points.Count < 3) {
        throw new ArgumentException("tried to create a Convex Hull with less than 3 points");
      }

      var plane = ApproxPlane(points, decimal_precision);

      var poly_2d = Polygon2D.ConvexHull(points.Select(p => plane.ProjectInto(p)).ToList(), decimal_precision);

      return (poly_2d is null) ? null : new Polygon3D(poly_2d.Select(p => plane.Evaluate(p)));
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
