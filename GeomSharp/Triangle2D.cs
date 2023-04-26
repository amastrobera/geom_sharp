using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using GeomSharp.Intersection;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  [Serializable]
  public class Triangle2D : IEquatable<Triangle2D>, ISerializable {
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

    public (Point2D Min, Point2D Max)
        BoundingBox() => (new Point2D(Math.Min(P0.U, Math.Min(P1.U, P2.U)), Math.Min(P0.V, Math.Min(P1.V, P2.V))),
                          new Point2D(Math.Max(P0.U, Math.Max(P1.U, P2.U)), Math.Max(P0.V, Math.Max(P1.V, P2.V))));

    /// <summary>
    /// Tells whether a point is inside a triangle T. It uses a geometry-only (no linear-algebra) method.
    /// A series of three calls to the notorious IsLeft function to verify that the point is on the left of each edge.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool Contains(Point2D point, int decimal_precision = Constants.THREE_DECIMALS) {
      // first check to save time, if the point is outside the bounding box, then it's outside the polygon too
      // improvement to the algorithm's big-O
      var bbox = BoundingBox();
      if (Math.Round(point.U - bbox.Min.U, decimal_precision) < 0 ||
          Math.Round(point.V - bbox.Min.V, decimal_precision) < 0 ||
          Math.Round(point.U - bbox.Max.U, decimal_precision) > 0 ||
          Math.Round(point.V - bbox.Max.V, decimal_precision) > 0) {
        return false;
      }

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
      if (other is null) {
        return false;
      }
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

    public bool Intersects(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);

    public IntersectionResult Intersection(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      // check against vertex containment
      (bool p0_in, bool p1_in, bool p2_in) = (other.Contains(this.P0, decimal_precision),
                                              other.Contains(this.P1, decimal_precision),
                                              other.Contains(this.P2, decimal_precision));
      int ps_in = (new List<bool> { p0_in, p1_in, p2_in }).Select(b => b ? 1 : 0).Sum();
      //    if all points are contained, it's overlap
      if (ps_in == 3) {
        return new IntersectionResult();
      }
      (bool q0_in, bool q1_in, bool q2_in) = (this.Contains(other.P0, decimal_precision),
                                              this.Contains(other.P1, decimal_precision),
                                              this.Contains(other.P2, decimal_precision));
      int qs_in = (new List<bool> { q0_in, q1_in, q2_in }).Select(b => b ? 1 : 0).Sum();
      //    if all points are contained, it's overlap
      if (qs_in == 3) {
        return new IntersectionResult();
      }

      //    if only 2 points or less are contained, this is an intersection
      var inter_points = new List<Point2D>();
      if (ps_in > 0) {
        if (p0_in) {
          inter_points.Add(this.P0);
        }
        if (p1_in) {
          inter_points.Add(this.P1);
        }
        if (p2_in) {
          inter_points.Add(this.P2);
        }
      }
      if (qs_in > 0) {
        if (q0_in) {
          inter_points.Add(other.P0);
        }
        if (q1_in) {
          inter_points.Add(other.P1);
        }
        if (q2_in) {
          inter_points.Add(other.P2);
        }
      }

      // check the intersections with edges
      Func<LineSegment2D, Triangle2D, bool> AddEdgeToTriangleIntersections =
          (LineSegment2D _test_edge, Triangle2D _triangle) => {
            IntersectionResult _res = _test_edge.Intersection(_triangle, decimal_precision);

            if (_res.ValueType == typeof(Point2D)) {
              inter_points.Add((Point2D)_res.Value);
              return true;
            } else if (_res.ValueType == typeof(LineSegment2D)) {
              var _seg = (LineSegment2D)_res.Value;
              inter_points.Add(_seg.P0);
              inter_points.Add(_seg.P1);
              return true;
            }

            return false;
          };

      AddEdgeToTriangleIntersections(LineSegment2D.FromPoints(this.P0, this.P1, decimal_precision), other);
      AddEdgeToTriangleIntersections(LineSegment2D.FromPoints(this.P1, this.P2, decimal_precision), other);
      AddEdgeToTriangleIntersections(LineSegment2D.FromPoints(this.P2, this.P0, decimal_precision), other);

      AddEdgeToTriangleIntersections(LineSegment2D.FromPoints(other.P0, other.P1, decimal_precision), this);
      AddEdgeToTriangleIntersections(LineSegment2D.FromPoints(other.P1, other.P2, decimal_precision), this);
      AddEdgeToTriangleIntersections(LineSegment2D.FromPoints(other.P2, other.P0, decimal_precision), this);

      if (inter_points.Count > 0) {
        inter_points = inter_points.SortCCW(decimal_precision)
                           .RemoveCollinearPoints(decimal_precision);  // removes duplicates, and collinear points (not
                                                                       // necessary), up to a line of two points

        if (inter_points.Count == 0) {
          return new IntersectionResult();  // this is Touch, not intersection, return NullValue
        }

        if (inter_points.Count == 1) {
          return new IntersectionResult();  // this is Touch, not intersection, return NullValue
        }

        if (inter_points.Count == 2) {
          throw new Exception("intersection of two triangles is a line (impossible)");
        }

        if (inter_points.Count == 3) {
          return new IntersectionResult(FromPoints(inter_points[0], inter_points[1], inter_points[2]));
        }

        return new IntersectionResult(new Polygon2D(inter_points));
      }

      return new IntersectionResult();
    }

    /// <summary>
    /// Two triangles 2D overlap if they share the surface area (all of it)
    /// In all other cases of shared surface, the triangles (2D) intersect
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);

    public IntersectionResult Overlap(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      // case 1: vertex containment
      (bool p0_in, bool p1_in, bool p2_in) = (other.Contains(this.P0, decimal_precision),
                                              other.Contains(this.P1, decimal_precision),
                                              other.Contains(this.P2, decimal_precision));
      int ps_in = (new List<bool> { p0_in, p1_in, p2_in }).Select(b => b ? 1 : 0).Sum();

      (bool q0_in, bool q1_in, bool q2_in) = (this.Contains(other.P0, decimal_precision),
                                              this.Contains(other.P1, decimal_precision),
                                              this.Contains(other.P2, decimal_precision));
      int qs_in = (new List<bool> { q0_in, q1_in, q2_in }).Select(b => b ? 1 : 0).Sum();
      //    if all points are contained, it's overlap
      if (qs_in == 3) {
        return new IntersectionResult(other);
      }
      if (ps_in == 3) {
        return new IntersectionResult(this);
      }

      //    if only 2 points or less are contained, this is an intersection, not an overlap
      if (ps_in > 0) {
        return new IntersectionResult();
      }
      if (qs_in > 0) {
        return new IntersectionResult();
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
    public bool IsAdjacent(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        AdjacentSide(other, decimal_precision).ValueType != typeof(NullValue);

    /// <summary>
    /// Returns the adjacent side of two triangles
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public IntersectionResult AdjacentSide(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      // vertex containment
      var p_in = new List<(Point2D P, bool IsIn)> { (this.P0, other.Contains(this.P0, decimal_precision)),
                                                    (this.P1, other.Contains(this.P1, decimal_precision)),
                                                    (this.P2, other.Contains(this.P2, decimal_precision)) };
      int n_p_in = p_in.Count(a => a.IsIn == true);

      var q_in = new List<(Point2D P, bool IsIn)> { (other.P0, this.Contains(other.P0, decimal_precision)),
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
        return new IntersectionResult(LineSegment2D.FromPoints(all_points_in[0], all_points_in[1], decimal_precision));
      }

      return new IntersectionResult();
    }

    /// <summary>
    /// Two triangles touch if one of them has a vertex contained in the edge of the other.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool Touches(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        TouchPoint(other, decimal_precision).ValueType == typeof(Point2D);

    /// <summary>
    /// Returns common point between two triangles
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public IntersectionResult TouchPoint(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var segment_touch_points =
          (new List<(Point2D Point, bool Touches)> { (other.P0, IsOnPerimeter(other.P0, decimal_precision)),
                                                     (other.P1, IsOnPerimeter(other.P1, decimal_precision)),
                                                     (other.P2, IsOnPerimeter(other.P2, decimal_precision)),
                                                     (this.P0, other.IsOnPerimeter(this.P0, decimal_precision)),
                                                     (this.P1, other.IsOnPerimeter(this.P1, decimal_precision)),
                                                     (this.P2, other.IsOnPerimeter(this.P2, decimal_precision)) })
              .Where(b => b.Touches);
      if (segment_touch_points.Count() == 0) {
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

    public bool IsOnPerimeter(Point2D point, int decimal_precision = Constants.THREE_DECIMALS) =>
        LineSegment2D.FromPoints(P0, P1, decimal_precision).Contains(point) ||
        LineSegment2D.FromPoints(P1, P2, decimal_precision).Contains(point) ||
        LineSegment2D.FromPoints(P2, P0, decimal_precision).Contains(point);

    // special formatting
    public override string ToString() {
      return "{" + P0.ToString() + ", " + P1.ToString() + ", " + P2.ToString() + "}";
    }

    public string ToWkt(int decimal_precision = Constants.THREE_DECIMALS) {
      return string.Format("POLYGON (({0:F2} {1:F2}, {2:F2} {3:F2},{4:F2} {5:F2},{0:F2} {1:F2}))",
                           P0.U,
                           P0.V,
                           P1.U,
                           P1.V,
                           P2.U,
                           P2.V);
    }

    // serialization functions
    // Implement this method to serialize data. The method is called on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("P0", P0, typeof(Point2D));
      info.AddValue("P1", P1, typeof(Point2D));
      info.AddValue("P2", P2, typeof(Point2D));

      info.AddValue("Orientation", Orientation, typeof(Constants.Orientation));
      info.AddValue("U", U, typeof(UnitVector2D));
      info.AddValue("V", V, typeof(UnitVector2D));
    }
    // The special constructor is used to deserialize values.
    public Triangle2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      P0 = (Point2D)info.GetValue("P0", typeof(Point2D));
      P1 = (Point2D)info.GetValue("P1", typeof(Point2D));
      P2 = (Point2D)info.GetValue("P2", typeof(Point2D));

      Orientation = (Constants.Orientation)info.GetValue("Orientation", typeof(Constants.Orientation));
      U = (UnitVector2D)info.GetValue("U", typeof(UnitVector2D));
      V = (UnitVector2D)info.GetValue("V", typeof(UnitVector2D));
    }

    public static Triangle2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Triangle2D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    public void ToBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Create);
        (new BinaryFormatter()).Serialize(fs, this);
        fs.Close();
      } catch (Exception e) {
        // warning failed to deserialize
      }
    }
  }
}
