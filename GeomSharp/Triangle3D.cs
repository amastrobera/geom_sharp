﻿
using GeomSharp.Collections;

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  [Serializable]
  public class Triangle3D : Geometry3D, IEquatable<Triangle3D>, ISerializable {
    public Point3D P0 { get; }
    public Point3D P1 { get; }
    public Point3D P2 { get; }

    public UnitVector3D Normal { get; }
    public UnitVector3D U { get; }
    public UnitVector3D V { get; }

    // constructors
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

    // generic overrides from object class
    public override string ToString() {
      return "{" + P0.ToString() + ", " + P1.ToString() + ", " + P2.ToString() + "}";
    }
    public override int GetHashCode() => new { P0, P1, P2, Normal }.GetHashCode();

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is Triangle3D && this.Equals((Triangle3D)other);
    public override bool Equals(Geometry3D other) => other.GetType() == typeof(Triangle3D) &&
                                                     this.Equals(other as Triangle3D);
    public bool Equals(Triangle3D other) => this.AlmostEquals(other);
    public override bool AlmostEquals(Geometry3D other, int decimal_precision = 3) =>
        other.GetType() == typeof(Triangle3D) && this.AlmostEquals(other as Triangle3D, decimal_precision);

    public bool AlmostEquals(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other is null) {
        return false;
      }
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

    // comparison operators
    public static bool operator ==(Triangle3D a, Triangle3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Triangle3D a, Triangle3D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("P0", P0, typeof(Point3D));
      info.AddValue("P1", P1, typeof(Point3D));
      info.AddValue("P2", P2, typeof(Point3D));

      info.AddValue("Normal", Normal, typeof(Constants.Orientation));
      info.AddValue("U", U, typeof(UnitVector3D));
      info.AddValue("V", V, typeof(UnitVector3D));
    }

    public Triangle3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      P0 = (Point3D)info.GetValue("P0", typeof(Point3D));
      P1 = (Point3D)info.GetValue("P1", typeof(Point3D));
      P2 = (Point3D)info.GetValue("P2", typeof(Point3D));

      Normal = (UnitVector3D)info.GetValue("Normal", typeof(UnitVector3D));
      U = (UnitVector3D)info.GetValue("U", typeof(UnitVector3D));
      V = (UnitVector3D)info.GetValue("V", typeof(UnitVector3D));
    }

    public static Triangle3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Triangle3D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int decimal_precision = Constants.THREE_DECIMALS) {
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

    public override Geometry3D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

    // relationship to all the other geometries

    //  plane
    public override bool Intersects(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Plane other, int decimal_precision = Constants.THREE_DECIMALS) {
      var plane_inter = other.Intersection(RefPlane());
      if (plane_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var line_inter = (Line3D)plane_inter.Value;

      return line_inter.Overlap(this);
    }
    public override bool Overlaps(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Overlap(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.AlmostEquals(RefPlane(), decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    // point
    public override bool Contains(Point3D point, int decimal_precision = Constants.THREE_DECIMALS) {
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

    //  geometry collection
    public override bool Intersects(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(GeometryCollection3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(GeometryCollection3D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line
    public override bool Intersects(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var res = RefPlane().Intersection(other, decimal_precision);
      if (res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)res.Value;

      if (Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }
    public override bool Overlaps(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Overlap(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = RefPlane();
      if (!ref_plane.Contains(other, decimal_precision)) {
        return new IntersectionResult();
      }
      // from 3D to 2D to compute the intersection points
      var triangle_2D =
          Triangle2D.FromPoints(ref_plane.ProjectInto(P0), ref_plane.ProjectInto(P1), ref_plane.ProjectInto(P2));

      var line_2D = Line2D.FromPoints(ref_plane.ProjectInto(other.Origin),
                                      ref_plane.ProjectInto(other.Origin + 2 * other.Direction));

      // from 2D back to 3D
      // if intersection in 2D, return it
      var inter_2D = triangle_2D.Intersection(line_2D, decimal_precision);
      if (inter_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)inter_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (inter_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)inter_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // also if there overlap in 2D, return it
      var ovrl_2D = triangle_2D.Overlap(line_2D, decimal_precision);
      if (ovrl_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)ovrl_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (ovrl_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)ovrl_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // no valid intersection
      return new IntersectionResult();
    }

    //  line segment
    public override bool Intersects(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(LineSegment3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
      var res = RefPlane().Intersection(other, decimal_precision);
      if (res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)res.Value;

      if (Contains(pI, decimal_precision) && other.Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }
    public override bool Overlaps(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Overlap(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = RefPlane();
      if (!ref_plane.Contains(other, decimal_precision)) {
        return new IntersectionResult();
      }
      // from 3D to 2D to compute the intersection points
      var triangle_2D =
          Triangle2D.FromPoints(ref_plane.ProjectInto(P0), ref_plane.ProjectInto(P1), ref_plane.ProjectInto(P2));

      var segment_2D = LineSegment2D.FromPoints(ref_plane.ProjectInto(other.P0), ref_plane.ProjectInto(other.P1));

      // from 2D back to 3D
      // if intersection 2D, return it
      var inter_2D = triangle_2D.Intersection(segment_2D, decimal_precision);
      if (inter_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)inter_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (inter_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)inter_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // also if overlap, return it
      var ovlp_2D = triangle_2D.Overlap(segment_2D, decimal_precision);
      if (ovlp_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)ovlp_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (ovlp_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)ovlp_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // no valid intersection
      return new IntersectionResult();
    }

    //  line segment set
    public override bool Intersects(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(LineSegmentSet3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(LineSegmentSet3D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polygon
    public override bool Intersects(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
    public override bool Overlaps(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polyline
    public override bool Intersects(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
    public override bool Overlaps(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  ray
    public override bool Intersects(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var res = RefPlane().Intersection(other, decimal_precision);
      if (res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)res.Value;

      if (Contains(pI, decimal_precision) && other.Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public override bool Overlaps(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Overlap(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = RefPlane();
      if (!ref_plane.Contains(other, decimal_precision)) {
        return new IntersectionResult();
      }
      // from 3D to 2D to compute the intersection points
      var triangle_2D =
          Triangle2D.FromPoints(ref_plane.ProjectInto(P0), ref_plane.ProjectInto(P1), ref_plane.ProjectInto(P2));

      var orig_2D = ref_plane.ProjectInto(other.Origin);
      var ray_2D =
          new Ray2D(orig_2D, (ref_plane.ProjectInto(other.Origin + 2 * other.Direction) - orig_2D).Normalize());

      // from 2D back to 3D
      // if 2D intersection, return it
      var inter_2D = triangle_2D.Intersection(ray_2D, decimal_precision);
      if (inter_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)inter_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (inter_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)inter_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // also if 2D overlap, return it
      var ovrl_2D = triangle_2D.Overlap(ray_2D, decimal_precision);
      if (ovrl_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)ovrl_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (ovrl_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)ovrl_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      //  no valid intersection
      return new IntersectionResult();
    }

    //  triangle
    public override bool Intersects(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);

    public override IntersectionResult Intersection(Triangle3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
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

      // the segment overlap in 2D must belong to both triangles
      var inter_segment = (LineSegment3D)ovlp_this.Value;
      var ovlp_other = inter_segment.Overlap(other);
      if (ovlp_other.ValueType != typeof(LineSegment3D)) {
        return new IntersectionResult();
      }

      // the segment overlap must not be an adjacent side
      if (ovlp_other.AlmostEquals(AdjacentSide(other), decimal_precision)) {
        return new IntersectionResult();
      }

      return ovlp_other;
    }

    public override bool Overlaps(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);

    public override IntersectionResult Overlap(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) {
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
        if (ovlp.ValueType == typeof(Triangle2D)) {
          return new IntersectionResult(plane.Evaluate((Triangle2D)ovlp.Value));
        }
        throw new Exception("unhandled case of triangle overlap: " + ovlp.ValueType);
      }

      var inter = this_2d.Intersection(other_2d);
      if (inter.ValueType != typeof(NullValue)) {
        if (inter.ValueType == typeof(Triangle2D)) {
          return new IntersectionResult(plane.Evaluate((Triangle2D)inter.Value));
        }
        if (inter.ValueType == typeof(Polygon2D)) {
          return new IntersectionResult(plane.Evaluate((Polygon2D)inter.Value));
        }
        throw new Exception("unhandled case of triangle intersection: " + inter.ValueType);
      }

      return new IntersectionResult();
    }

    // own functions
    public double Area() => (P1 - P0).CrossProduct(P2 - P0).Length() / 2;

    public Point3D CenterOfMass() => Point3D.FromVector((P0.ToVector() + P1.ToVector() + P2.ToVector()) / 3);

    public Plane RefPlane() => Plane.FromPoints(P0, P1, P2);

    public (Point3D Min, Point3D Max) BoundingBox() => (new Point3D(Math.Min(P0.X, Math.Min(P1.X, P2.X)),
                                                                    Math.Min(P0.Y, Math.Min(P1.Y, P2.Y)),
                                                                    Math.Min(P0.Z, Math.Min(P1.Z, P2.Z))),
                                                        new Point3D(Math.Max(P0.X, Math.Max(P1.X, P2.X)),
                                                                    Math.Max(P0.Y, Math.Max(P1.Y, P2.Y)),
                                                                    Math.Min(P0.Z, Math.Min(P1.Z, P2.Z))));

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
  }
}
