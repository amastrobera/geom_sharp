using System;
using System.Collections.Generic;

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
    public bool Contains(Point2D point, int decimal_precision = Constants.THREE_DECIMALS) {
      var loc_01 = LineSegment2D.FromPoints(P0, P1).Location(point, decimal_precision);
      var loc_12 = LineSegment2D.FromPoints(P1, P2).Location(point, decimal_precision);
      var loc_20 = LineSegment2D.FromPoints(P2, P0).Location(point, decimal_precision);

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

    public bool AlmostEquals(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (Orientation != other.Orientation) {
        return false;
      }

      Func<Triangle2D, Point2D, bool> TriangleContainsPoint = (Triangle2D t, Point2D p) => {
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
    public bool Equals(Triangle2D other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Triangle2D && this.Equals((Triangle2D)other);

    public override int GetHashCode() => new { P0, P1, P2, Orientation }.GetHashCode();

    public static bool operator ==(Triangle2D a, Triangle2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Triangle2D a, Triangle2D b) {
      return !a.AlmostEquals(b);
    }

    // TODO: remove, once Intersection is implemented
    public bool Intersects(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      (var edge01, var edge12, var edge20) = (LineSegment2D.FromPoints(P0, P1, decimal_precision),
                                              LineSegment2D.FromPoints(P1, P2, decimal_precision),
                                              LineSegment2D.FromPoints(P2, P0, decimal_precision));

      (var other01, var other12, var other20) = (LineSegment2D.FromPoints(other.P0, other.P1, decimal_precision),
                                                 LineSegment2D.FromPoints(other.P1, other.P2, decimal_precision),
                                                 LineSegment2D.FromPoints(other.P2, other.P0, decimal_precision));

      return edge01.Intersects(other01) || edge01.Intersects(other12) || edge01.Intersects(other20) ||
             edge12.Intersects(other01) || edge12.Intersects(other12) || edge12.Intersects(other20) ||
             edge20.Intersects(other01) || edge20.Intersects(other12) || edge20.Intersects(other20);
    }

    // TODO: add, once Intersection is implemented
    // public bool Intersects(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
    //    Intersection(other, decimal_precision).ValueType != typeof(NullValue);

    public IntersectionResult Intersection(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      (var edge01, var edge12, var edge20) = (LineSegment2D.FromPoints(P0, P1, decimal_precision),
                                              LineSegment2D.FromPoints(P1, P2, decimal_precision),
                                              LineSegment2D.FromPoints(P2, P0, decimal_precision));

      (var other01, var other12, var other20) = (LineSegment2D.FromPoints(other.P0, other.P1, decimal_precision),
                                                 LineSegment2D.FromPoints(other.P1, other.P2, decimal_precision),
                                                 LineSegment2D.FromPoints(other.P2, other.P0, decimal_precision));

      // TODO: implement and return the resulting Polygon2D

      return new IntersectionResult();
    }

    /// <summary>
    /// Two triangles 2D overlap if they share
    /// - a point
    /// - an edge
    /// - the whole surface
    /// In all other cases of shared surface, the triangles (2D) intersect
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);

    public IntersectionResult Overlap(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) {
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

      // does any lines overlap with any other ?
      AddResultIfAny(s1.Overlap(other_s1, decimal_precision));
      AddResultIfAny(s1.Overlap(other_s2, decimal_precision));
      AddResultIfAny(s1.Overlap(other_s3, decimal_precision));

      AddResultIfAny(s2.Overlap(other_s1, decimal_precision));
      AddResultIfAny(s2.Overlap(other_s2, decimal_precision));
      AddResultIfAny(s2.Overlap(other_s3, decimal_precision));

      AddResultIfAny(s3.Overlap(other_s1, decimal_precision));
      AddResultIfAny(s3.Overlap(other_s2, decimal_precision));
      AddResultIfAny(s3.Overlap(other_s3, decimal_precision));

      // is any point contained in any triangle ?
      if (Contains(other.P0, decimal_precision)) {
        output_points.Add(other.P0);
      }

      if (Contains(other.P1, decimal_precision)) {
        output_points.Add(other.P1);
      }

      if (Contains(other.P2, decimal_precision)) {
        output_points.Add(other.P2);
      }

      if (other.Contains(P0, decimal_precision)) {
        output_points.Add(P0);
      }

      if (other.Contains(P1, decimal_precision)) {
        output_points.Add(P1);
      }

      if (other.Contains(P2, decimal_precision)) {
        output_points.Add(P2);
      }

      // does any line intersects with any other ?
      AddResultIfAny(s1.Intersection(other_s1, decimal_precision));
      AddResultIfAny(s1.Intersection(other_s2, decimal_precision));
      AddResultIfAny(s1.Intersection(other_s3, decimal_precision));

      AddResultIfAny(s2.Intersection(other_s1, decimal_precision));
      AddResultIfAny(s2.Intersection(other_s2, decimal_precision));
      AddResultIfAny(s2.Intersection(other_s3, decimal_precision));

      AddResultIfAny(s3.Intersection(other_s1, decimal_precision));
      AddResultIfAny(s3.Intersection(other_s2, decimal_precision));
      AddResultIfAny(s3.Intersection(other_s3, decimal_precision));

      // final output point count
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

    public bool IsAdjacent(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        AdjacentSide(other, decimal_precision).ValueType != typeof(NullValue);

    public IntersectionResult AdjacentSide(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) {
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
