using System;
using System.Linq;
using System.Collections.Generic;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  public class Triangle3D : IEquatable<Triangle3D> {
    public Point3D P0 { get; }
    public Point3D P1 { get; }
    public Point3D P2 { get; }

    public UnitVector3D Normal { get; }
    public UnitVector3D U { get; }
    public UnitVector3D V { get; }

    private Triangle3D(Point3D p0, Point3D p1, Point3D p2) {
      P0 = p0;
      P1 = p1;
      P2 = p2;
      // unit vectors (normalized)
      U = (P1 - P0).Normalize();
      V = (P2 - P0).Normalize();

      Normal = U.CrossProduct(V).Normalize();
    }

    public static Triangle3D FromPoints(Point3D p0, Point3D p1, Point3D p2) {
      if (p1.AlmostEquals(p0) || p1.AlmostEquals(p2) || p2.AlmostEquals(p0)) {
        throw new ArithmeticException("tried to construct a Triangle with equal points");
      }

      if ((p1 - p0).IsParallel(p2 - p0)) {
        throw new ArithmeticException("tried to construct a Triangle with collinear points");
      }

      var t = new Triangle3D(p0, p1, p2);

      if (t.Area() == 0) {
        throw new ArithmeticException("tried to construct a Triangle of nearly zero-area");
      }

      return t;
    }

    public double Area() => (P1 - P0).CrossProduct(P2 - P0).Length() / 2;

    public Point3D CenterOfMass() => Point3D.FromVector((P0.ToVector() + P1.ToVector() + P2.ToVector()) / 3);

    public Plane RefPlane() => Plane.FromPoints(P0, P1, P2);

    /// <summary>
    /// Tells whether a point is inside a triangle T.
    /// First tests if a point is on the same plane of the triangle. The projects everything in 2D and tests for
    /// containment over there.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool Contains(Point3D point, int decimal_precision = Constants.THREE_DECIMALS) {
      // first test: is it on the same plane ?
      var plane = RefPlane();
      if (!Plane.FromPoints(P0, P1, P2).Contains(point, decimal_precision)) {
        return false;
      }

      // project into 2D plane and fint out whether there is containment
      var triangle_2d = Triangle2D.FromPoints(plane.ProjectInto(P0), plane.ProjectInto(P1), plane.ProjectInto(P2));
      var point_2d = plane.ProjectInto(point);

      return triangle_2d.Contains(point_2d, decimal_precision);
    }

    public bool AlmostEquals(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!Normal.AlmostEquals(other.Normal, decimal_precision)) {
        return false;
      }

      Func<Triangle3D, Point3D, bool> TriangleContainsPoint = (Triangle3D t, Point3D p) => {
        return t.P0.AlmostEquals(p, decimal_precision) || t.P1.AlmostEquals(p, decimal_precision) ||
               t.P2.AlmostEquals(p, decimal_precision);
      };

      if (!TriangleContainsPoint(this, other.P0)) {
        return false;
      }

      if (!TriangleContainsPoint(this, other.P1)) {
        return false;
      }

      if (!TriangleContainsPoint(this, other.P2)) {
        return false;
      }

      // no check on point order (CCW or CW) is needed, since the constructor guarantees the Normal to be contructed
      // by points, and therefore incorporates this information
      return true;
    }

    public bool Equals(Triangle3D other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Triangle3D && this.Equals((Triangle3D)other);

    public override int GetHashCode() => new { P0, P1, P2, Normal }.GetHashCode();

    public static bool operator ==(Triangle3D a, Triangle3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Triangle3D a, Triangle3D b) {
      return !a.AlmostEquals(b);
    }

    public bool Intersects(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);

    public IntersectionResult Intersection(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var plane_inter = RefPlane().Intersection(other.RefPlane(), decimal_precision);
      if (plane_inter.ValueType != typeof(Line3D)) {
        return new IntersectionResult();
      }

      var inter_line = (Line3D)plane_inter.Value;

      // a line on the same plane passing through a triangle was defined as an overlap
      var ovlp_this = inter_line.Overlap(this);
      if (ovlp_this.ValueType != typeof(LineSegment3D)) {
        return new IntersectionResult();
      }

      var inter_segment = (LineSegment3D)ovlp_this.Value;
      var ovlp_other = inter_segment.Overlap(other);
      if (ovlp_other.ValueType != typeof(LineSegment3D)) {
        return new IntersectionResult();
      }
      return ovlp_other;
    }

    /// <summary>
    /// Tells whether two triangles overlap.
    /// To overlap the triangles must lie on the same plane, to begin with. Then it is sufficient for one point of a
    /// triangle to be contained in the other triangle.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);

    /// <summary>
    /// Finds the overlap of two triangles. The overlap can be a Point3D, or a LineSegment3D, a Triangle3D or a
    /// Polygon3D.
    /// To overlap the triangles must lie on the same plane, to begin with. Then it is sufficient for one
    /// point of a triangle to be contained in the other triangle.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Overlap(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!RefPlane().AlmostEquals(other.RefPlane(), decimal_precision)) {
        return new IntersectionResult();
      }

      var plane = RefPlane();
      var this_2d =
          Triangle2D.FromPoints(plane.ProjectInto(this.P0), plane.ProjectInto(this.P1), plane.ProjectInto(this.P2));
      var other_2d =
          Triangle2D.FromPoints(plane.ProjectInto(other.P0), plane.ProjectInto(other.P1), plane.ProjectInto(other.P2));

      var ovlp = this_2d.Overlap(other_2d);
      if (ovlp.ValueType != typeof(NullValue)) {
        return ovlp;
      }

      var inter = this_2d.Intersection(other_2d);
      if (inter.ValueType != typeof(NullValue)) {
        return inter;
      }

      return new IntersectionResult();
    }

    /// <summary>
    /// Two triangles are adjancent if
    /// - they have at most one edge in common, or part of that edge (otherwise it's a surface overlap)
    /// - no point of a triangle is inside another (containment)
    /// - no intersection exists between the two triangles (intersection)
    /// This function tells whether the triangles are adjacent
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool IsAdjacent(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        AdjacentSide(other, decimal_precision).ValueType != typeof(NullValue);

    /// <summary>
    /// Returns the adjacent side of two triangles
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public IntersectionResult AdjacentSide(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      // vertex containment
      var p_in = new List<(Point3D P, bool IsIn)> { (this.P0, other.Contains(this.P0, decimal_precision)),
                                                    (this.P1, other.Contains(this.P1, decimal_precision)),
                                                    (this.P2, other.Contains(this.P2, decimal_precision)) };
      int n_p_in = p_in.Count(a => a.IsIn == true);

      var q_in = new List<(Point3D P, bool IsIn)> { (other.P0, this.Contains(other.P0, decimal_precision)),
                                                    (other.P1, this.Contains(other.P1, decimal_precision)),
                                                    (other.P2, this.Contains(other.P2, decimal_precision)) };

      int n_q_in = q_in.Count(a => a.IsIn == true);

      //    if all points are contained, it's overlap
      if (n_q_in == 3 || n_p_in == 3) {
        return new IntersectionResult();
      }
      // if no points are contained, it's intersection or unrelated
      if (n_q_in == 0 || n_p_in == 0) {
        return new IntersectionResult();
      }

      var all_points_in = p_in.Where(a => a.IsIn).Select(a => a.P).ToList();
      all_points_in.AddRange(q_in.Where(a => a.IsIn).Select(b => b.P).ToList());
      all_points_in = all_points_in.RemoveDuplicates(decimal_precision);

      if (all_points_in.Count == 2) {
        return new IntersectionResult(LineSegment3D.FromPoints(all_points_in[0], all_points_in[1], decimal_precision));
      }

      return new IntersectionResult();
    }

    /// <summary>
    /// Two triangles touch if one of them has a vertex contained in the edge of the other.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool Touches(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        TouchPoint(other, decimal_precision).ValueType == typeof(Point3D);

    /// <summary>
    /// Returns common point between two triangles
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public IntersectionResult TouchPoint(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var segment_touch_points =
          (new List<(Point3D Point, bool Touches)> { (other.P0, IsOnPerimeter(other.P0, decimal_precision)),
                                                     (other.P1, IsOnPerimeter(other.P1, decimal_precision)),
                                                     (other.P2, IsOnPerimeter(other.P2, decimal_precision)),
                                                     (this.P0, other.IsOnPerimeter(this.P0, decimal_precision)),
                                                     (this.P1, other.IsOnPerimeter(this.P1, decimal_precision)),
                                                     (this.P2, other.IsOnPerimeter(this.P2, decimal_precision)) })
              .Where(b => b.Touches);

      if (segment_touch_points.Count() == 0) {
        // a touch can still happen if two triangles are intersecting in exacly one point
        if (!RefPlane().AlmostEquals(other.RefPlane(), decimal_precision)) {
          // intersection might be one point only, therefore, a mere touch

          var plane_inter = RefPlane().Intersection(other.RefPlane(), decimal_precision);
          if (plane_inter.ValueType != typeof(Line3D)) {
            return new IntersectionResult();
          }

          var inter_line = (Line3D)plane_inter.Value;

          // a line on the same plane passing through a triangle was defined as an overlap
          var ovlp_this = inter_line.Overlap(this);
          if (ovlp_this.ValueType == typeof(NullValue)) {
            return new IntersectionResult();
          }

          var inter_segment = (LineSegment3D)ovlp_this.Value;
          var ovlp_other = inter_segment.Overlap(other);
          if (ovlp_other.ValueType == typeof(Point3D)) {
            return ovlp_other;
          }

          return new IntersectionResult();
        }
        return new IntersectionResult();
      }

      var point_list = segment_touch_points.Select(pr => pr.Point).ToList().RemoveDuplicates(decimal_precision);

      if (point_list.Count > 1) {
        // TODO: warning, this could be an Overlap or Intersection
        return new IntersectionResult();
      }

      if (point_list.Count == 1) {
        return new IntersectionResult(point_list.First());
      }

      return new IntersectionResult();
    }

    public bool IsOnPerimeter(Point3D point, int decimal_precision = Constants.THREE_DECIMALS) =>
        LineSegment3D.FromPoints(P0, P1, decimal_precision).Contains(point) ||
        LineSegment3D.FromPoints(P1, P2, decimal_precision).Contains(point) ||
        LineSegment3D.FromPoints(P2, P0, decimal_precision).Contains(point);

    public override string ToString() {
      return "{" + P0.ToString() + ", " + P1.ToString() + ", " + P2.ToString() + "}";
    }

    public string ToWkt() {
      return string.Format(
          "POLYGON (({0:F2} {1:F2} {2:F2}, {3:F2} {4:F2} {5:F2}, {6:F2} {7:F2} {8:F2}, {0:F2} {1:F2} {2:F2}))",
          P0.X,
          P0.Y,
          P0.Z,
          P1.X,
          P1.Y,
          P1.Z,
          P2.X,
          P2.Y,
          P2.Z);
    }
  }

}
