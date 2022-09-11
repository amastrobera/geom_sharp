using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  public class Triangle2D : IEquatable<Triangle2D> {
    public Point2D P0 { get; }
    public Point2D P1 { get; }
    public Point2D P2 { get; }

    public Constants.Orientation Orientation { get; }
    public UnitVector2D U { get; }
    public UnitVector2D V { get; }

    private Triangle2D(Point2D p0, Point2D p1, Point2D p2) {
      P0 = p0;
      P1 = p1;
      P2 = p2;
      // unit vectors (normalized)
      U = (P1 - P0).Normalize();
      V = (P2 - P0).Normalize();

      Orientation = (U.PerpProduct(V) >= 0) ? Constants.Orientation.COUNTER_CLOCKWISE : Constants.Orientation.CLOCKWISE;
    }

    public static Triangle2D FromPoints(Point2D p0, Point2D p1, Point2D p2) {
      if (p1.AlmostEquals(p0) || p1.AlmostEquals(p2) || p2.AlmostEquals(p0)) {
        throw new ArithmeticException("tried to construct a Triangle with equal points");
      }

      if ((p1 - p0).IsParallel(p2 - p0)) {
        throw new ArithmeticException("tried to construct a Triangle with collinear points");
      }

      var t = new Triangle2D(p0, p1, p2);

      if (t.Area() == 0) {
        throw new ArithmeticException("tried to construct a Triangle of nearly zero-area");
      }

      return t;
    }

    public double Area() => Math.Round((P1 - P0).CrossProduct(P2 - P0) / 2, Constants.NINE_DECIMALS);

    public Point2D CenterOfMass() => Point2D.FromVector((P0.ToVector() + P1.ToVector() + P2.ToVector()) / 3);

    /// <summary>
    /// Tells whether a point is inside a triangle T. It uses a geometry-only (no linear-algebra) method.
    /// A series of three calls to the notorious IsLeft function to verify that the point is on the left of each edge.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool Contains(Point2D point) {
      var loc_01 = LineSegment2D.FromPoints(P0, P1).Location(point);
      var loc_12 = LineSegment2D.FromPoints(P1, P2).Location(point);
      var loc_20 = LineSegment2D.FromPoints(P2, P0).Location(point);

      // this is really impossible
      if (Orientation == Constants.Orientation.UNKNOWN) {
        throw new Exception(
            "cannot use the Contains (geometrical solution) function for a triangle which point orientation is unknown");
      }

      if (Orientation == Constants.Orientation.COUNTER_CLOCKWISE) {
        return (loc_01 == Constants.Location.LEFT || loc_01 == Constants.Location.ON_SEGMENT) &&
               (loc_12 == Constants.Location.LEFT || loc_12 == Constants.Location.ON_SEGMENT) &&
               (loc_20 == Constants.Location.LEFT || loc_20 == Constants.Location.ON_SEGMENT);
      }

      return (loc_01 == Constants.Location.RIGHT || loc_01 == Constants.Location.ON_SEGMENT) &&
             (loc_12 == Constants.Location.RIGHT || loc_12 == Constants.Location.ON_SEGMENT) &&
             (loc_20 == Constants.Location.RIGHT || loc_20 == Constants.Location.ON_SEGMENT);
    }

    public bool Equals(Triangle2D other) {
      if (!Orientation.Equals(other.Orientation)) {
        return false;
      }
      var point_list = new List<Point2D>() { P0, P1, P2 };
      if (!point_list.Contains(other.P0)) {
        return false;
      }
      if (!point_list.Contains(other.P1)) {
        return false;
      }
      if (!point_list.Contains(other.P2)) {
        return false;
      }
      // no check on point order (CCW or CW) is needed, since the constructor guarantees the Facing to be contructed
      // by points, and therefore incorporates this information
      return true;
    }

    public override bool Equals(object other) => other != null && other is Triangle2D && this.Equals((Triangle2D)other);

    public override int GetHashCode() => new { P0, P1, P2, Orientation }.GetHashCode();

    public static bool operator ==(Triangle2D a, Triangle2D b) {
      return a.Equals(b);
    }

    public static bool operator !=(Triangle2D a, Triangle2D b) {
      return !a.Equals(b);
    }

    /// <summary>
    /// Two triangles overlap if they are on the same plane, and share the same area (or a portion of the same area)
    /// This includes: triangles adjacent by one edge, triangles with one point in common, triangles intersecting
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Triangle2D other) => Overlap(other).ValueType != typeof(NullValue);

    public IntersectionResult Overlap(Triangle2D other) {
      var output_points = new List<Point2D>();

      var s1 = LineSegment2D.FromPoints(P0, P1);
      var s2 = LineSegment2D.FromPoints(P1, P2);
      var s3 = LineSegment2D.FromPoints(P2, P0);
      var other_s1 = LineSegment2D.FromPoints(other.P0, other.P1);
      var other_s2 = LineSegment2D.FromPoints(other.P1, other.P2);
      var other_s3 = LineSegment2D.FromPoints(other.P2, other.P0);

      Func<IntersectionResult, bool> AddResultIfAny = (IntersectionResult _res) => {
        if (_res.ValueType == typeof(Point2D)) {
          output_points.Add((Point2D)_res.Value);
          return true;
        }
        if (_res.ValueType == typeof(LineSegment2D)) {
          var seg = (LineSegment2D)_res.Value;
          output_points.Add(seg.P0);
          output_points.Add(seg.P1);
          return true;
        }

        return false;
      };

      // any lines overlap
      AddResultIfAny(s1.Overlap(other_s1));
      AddResultIfAny(s1.Overlap(other_s2));
      AddResultIfAny(s1.Overlap(other_s3));
      AddResultIfAny(s2.Overlap(other_s1));
      AddResultIfAny(s2.Overlap(other_s2));
      AddResultIfAny(s2.Overlap(other_s3));
      AddResultIfAny(s3.Overlap(other_s1));
      AddResultIfAny(s3.Overlap(other_s2));
      AddResultIfAny(s3.Overlap(other_s3));
      AddResultIfAny(other_s1.Overlap(s1));
      AddResultIfAny(other_s1.Overlap(s2));
      AddResultIfAny(other_s1.Overlap(s3));
      AddResultIfAny(other_s2.Overlap(s1));
      AddResultIfAny(other_s2.Overlap(s2));
      AddResultIfAny(other_s2.Overlap(s3));
      AddResultIfAny(other_s3.Overlap(s1));
      AddResultIfAny(other_s3.Overlap(s2));
      AddResultIfAny(other_s3.Overlap(s3));

      // any point contained in
      if (Contains(other.P0)) {
        output_points.Add(other.P0);
      }

      if (Contains(other.P1)) {
        output_points.Add(other.P1);
      }

      if (Contains(other.P2)) {
        output_points.Add(other.P2);
      }

      if (other.Contains(P0)) {
        output_points.Add(P0);
      }

      if (other.Contains(P1)) {
        output_points.Add(P1);
      }

      if (other.Contains(P2)) {
        output_points.Add(P2);
      }

      // any intersection
      AddResultIfAny(s1.Intersection(other_s1));
      AddResultIfAny(s1.Intersection(other_s2));
      AddResultIfAny(s1.Intersection(other_s3));
      AddResultIfAny(s2.Intersection(other_s1));
      AddResultIfAny(s2.Intersection(other_s2));
      AddResultIfAny(s2.Intersection(other_s3));
      AddResultIfAny(s3.Intersection(other_s1));
      AddResultIfAny(s3.Intersection(other_s2));
      AddResultIfAny(s3.Intersection(other_s3));
      AddResultIfAny(other_s1.Intersection(s1));
      AddResultIfAny(other_s1.Intersection(s2));
      AddResultIfAny(other_s1.Intersection(s3));
      AddResultIfAny(other_s2.Intersection(s1));
      AddResultIfAny(other_s2.Intersection(s2));
      AddResultIfAny(other_s2.Intersection(s3));
      AddResultIfAny(other_s3.Intersection(s1));
      AddResultIfAny(other_s3.Intersection(s2));
      AddResultIfAny(other_s3.Intersection(s3));

      if (output_points.Count == 0) {
        return new IntersectionResult();
      }

      if (output_points.Count == 1) {
        return new IntersectionResult(output_points[0]);
      }

      if (output_points.Count == 2) {
        return new IntersectionResult(LineSegment2D.FromPoints(output_points[0], output_points[1]));
      }

      // triangle or polygon
      output_points.SortCCW();

      if (output_points.Count == 3) {
        return new IntersectionResult(Triangle2D.FromPoints(output_points[0], output_points[1], output_points[2]));
      }

      return new IntersectionResult(new Polygon2D(output_points));
    }

    public bool IsAdjacent(Triangle2D other) => AdjacentSide(other).ValueType != typeof(NullValue);

    public IntersectionResult AdjacentSide(Triangle2D other) {
      return new IntersectionResult();
    }

    // special formatting
    public override string ToString() {
      return "{" + P0.ToString() + ", " + P1.ToString() + ", " + P2.ToString() + "}";
    }

    public string ToWkt() {
      return string.Format("POLYGON (({0:F2} {1:F2}, {2:F2} {3:F2},{4:F2} {5:F2},{0:F2} {1:F2}))",
                           P0.U,
                           P0.V,
                           P1.U,
                           P1.V,
                           P2.U,
                           P2.V);
    }
  }
}
