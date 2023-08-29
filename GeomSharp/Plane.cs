
using GeomSharp.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  public class Plane : IEquatable<Plane> {
    public Point3D Origin { get; }
    public UnitVector3D Normal { get; }

    public UnitVector3D AxisU { get; }

    public UnitVector3D AxisV { get; }

    public static Plane XY => FromPointAndNormal(Point3D.Zero, Vector3D.AxisZ);
    public static Plane YZ => FromPointAndNormal(Point3D.Zero, Vector3D.AxisX);
    public static Plane ZX => FromPointAndNormal(Point3D.Zero, Vector3D.AxisY);

    // constructors
    private Plane(Point3D origin,
                  UnitVector3D normal,
                  UnitVector3D u_axis,
                  UnitVector3D v_axis) => (Origin, Normal, AxisU, AxisV) = (origin, normal, u_axis, v_axis);

    public static Plane TryFromPoints(Point3D p0,
                                      Point3D p1,
                                      Point3D p2,
                                      int decimal_precision = Constants.THREE_DECIMALS) {
      Plane p = null;
      try {
        p = FromPoints(p0, p1, p2, decimal_precision);
      } catch (Exception) {
        p = null;
      }
      return p;
    }

    public static Plane FromPoints(Point3D p0,
                                   Point3D p1,
                                   Point3D p2,
                                   int decimal_precision = Constants.THREE_DECIMALS) {
      if (p0.AreCollinear(p1, p2, decimal_precision)) {
        throw new ArithmeticException("tried to create a plane from collinear points");
      }

      var U = p1 - p0;
      var V = p2 - p0;

      // make U and V perpendicular by using the normal
      var n = U.CrossProduct(V).Normalize();
      var u = U.Normalize();
      var v = n.CrossProduct(u).Normalize();
      return new Plane(p0, n, u, v);
    }

    public static Plane FromPointAndLine(Point3D p, Line3D line, int decimal_precision = Constants.THREE_DECIMALS) {
      return FromPoints(p, line.P0, line.P1, decimal_precision);  // will throw if collinear
    }

    public static Plane FromTwoLines(Line3D line1, Line3D line2, int decimal_precision = Constants.THREE_DECIMALS) {
      var result = line1.Intersection(line2, decimal_precision);
      if (result.ValueType != typeof(Point3D)) {
        throw new ArithmeticException("cannot build a plane from two lines non-intersecting");
      }
      var origin = (Point3D)result.Value;
      // make U and V perpendicular by using the normal
      var n = line1.Direction.CrossProduct(line2.Direction).Normalize();
      var u = line1.Direction;
      var v = n.CrossProduct(u).Normalize();

      return new Plane(origin, n, u, v);
    }

    public static Plane FromPointAndNormal(Point3D origin,
                                           UnitVector3D normal,
                                           int decimal_precision = Constants.THREE_DECIMALS) {
      // pick a point slightly off the origin, project it on a temporary plane
      var other = Point3D.FromVector(origin.ToVector() + 1);   // add a unit to any dimension
      var temp_plane = new Plane(origin, normal, null, null);  // initialize without U/V axis
      Point3D proj_point;
      proj_point = temp_plane.ProjectOnto(other);
      if (proj_point.AlmostEquals(origin, decimal_precision)) {
        // try adding 1
        var one_vec = new Vector3D(1, 1, 1).Normalize();
        var one_point = origin + (normal.IsParallel(one_vec, decimal_precision) ? (one_vec + Vector3D.AxisY) : one_vec);
        proj_point = temp_plane.ProjectOnto(one_point);
        if (proj_point.AlmostEquals(origin, decimal_precision)) {
          throw new Exception("could not generate Axis U / Asis V with proj_point");
        }
      }

      // compute AxisU
      var u_axis = (proj_point - origin).Normalize();
      //      test that the axis belongs to the plane (perpendicular to the normal)
      if (Math.Round(u_axis.DotProduct(temp_plane.Normal), decimal_precision) != 0) {
        throw new Exception("FromPointAndNormal failed (u_axis perp to normal)");
      }

      // compute AxisV (cross prod of ZX axis: Normal x AxisU)
      var v_axis = temp_plane.Normal.CrossProduct(u_axis).Normalize();
      //      test that the axis belongs to the plane (perpendicular to the normal)
      if (Math.Round(v_axis.DotProduct(temp_plane.Normal), decimal_precision) != 0) {
        throw new Exception("FromPointAndNormal failed (v_axis perp to normal)");
      }

      if (Math.Round(v_axis.DotProduct(u_axis), decimal_precision) != 0) {
        throw new Exception("FromPointAndNormal failed (v_axis perp u_axis)");
      }

      return new Plane(origin, normal, u_axis, v_axis);
    }

    // unary operations

    public bool AlmostEquals(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) && Normal.AlmostEquals(other.Normal, decimal_precision) &&
        (Origin.AlmostEquals(other.Origin, decimal_precision) ||
         (Origin - other.Origin).IsPerpendicular(Normal, decimal_precision));

    public bool Equals(Plane other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Plane && this.Equals((Plane)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Plane a, Plane b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Plane a, Plane b) {
      return !a.AlmostEquals(b);
    }

    /// <summary>
    /// Returns the perpendicular vector to the input vector (on this plane)
    /// w = n x v
    /// Also called the "generalized perp operator"
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Vector3D Perp(Vector3D v) => Normal.CrossProduct(v);

    /// <summary>
    /// (Signed) distance of a point from a plane.
    /// Negative = the point is below the plane
    /// Positive = the point is above
    /// Can be used for a test of intersection with a line-segment (one point below, one point above, means
    /// intersection)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public double SignedDistance(Point3D p) => Normal.DotProduct(p - Origin);

    public double Distance(Point3D p) => Math.Abs(SignedDistance(p));

    // containment
    public bool Contains(Point3D p,
                         int decimal_precision = Constants.THREE_DECIMALS) => Math.Round(SignedDistance(p),
                                                                                         decimal_precision) == 0;

    public bool Contains(Line3D line, int decimal_precision = Constants.THREE_DECIMALS) =>
        Normal.IsPerpendicular(line.Direction, decimal_precision) && Contains(line.Origin, decimal_precision);

    public bool Contains(Ray3D r, int decimal_precision = Constants.THREE_DECIMALS) =>
        Normal.IsPerpendicular(r.Direction, decimal_precision) && Contains(r.Origin, decimal_precision);

    public bool Contains(LineSegment3D s, int decimal_precision = Constants.THREE_DECIMALS) =>
        Normal.IsPerpendicular(s.P1 - s.P0, decimal_precision) && Contains(s.P0, decimal_precision);

    // projection (3D to 2D)

    /// <summary>
    /// Projects orthogonally the point on the plane
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Point3D ProjectOnto(Point3D p) {
      // funny edge case
      if (Contains(p)) {
        return p;
      }

      // in theory this is sufficient, but in practice this result is far from zero-distance to the plane
      // var q = p - SignedDistance(p) * Normal;

      // so, let's use Daniel Sunday's formula to achieve greatness
      var U = -SignedDistance(p) * Normal;
      var W = p - Origin;
      var n = Normal;
      double sI = -n.DotProduct(W) / n.DotProduct(U);
      var q = p + sI * U;

      if (!Contains(q)) {
        throw new Exception("ProjectOnto failed");
      }
      return q;
    }

    /// <summary>
    /// Projects on a plane along a given axis
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Point3D ProjectOnto(Point3D p, UnitVector3D proj_axis) {
      // funny edge case
      if (Contains(p)) {
        return p;
      }

      // Daniel Sunday's magic
      var U = proj_axis;
      var W = p - Origin;
      var n = Normal;

      double sI = -n.DotProduct(W) / n.DotProduct(U);

      Point3D q = p + sI * U;

      if (!Contains(q)) {
        throw new Exception("VerticalProjectOnto failed");
      }

      return q;
    }

    /// <summary>
    /// Projects on a plane along a given axis (throws if the plane is vertical!)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Point3D VerticalProjectOnto(Point3D p) => ProjectOnto(p, Vector3D.AxisZ);

    /// <summary>
    /// Project given 3D XYZ point into plane,
    /// returning the UV coordinates of the result
    /// in the local 2D plane coordinate system.
    /// Author: Jeremy Tammik
    /// It's based on the formula of cosine. Given vectors A, B and theta = angle(A,B)
    /// cos(theta) = A*B / |A|*|B|
    /// The projection of a vector B onto another A is then
    /// |B|*cos(theta) = A*B / |A|
    /// ProjectInto projects the Point3D p onto the same 3D plane. Then projects the point on the AxisU and AxisV of
    /// the plane, and returns the 2D coordinates of the point along the basis AxisU,AxisV.
    /// </summary>
    public Point2D ProjectInto(Point3D p) {
      var q = ProjectOnto(p);
      var B = q - Origin;
      double B_len = B.Length();
      double B_cos = B.DotProduct(AxisU);
      double B_sin = B.DotProduct(AxisV);
      return new Point2D(B_cos, B_sin);

      // // this below yields the same result as the above
      // var U_int = Line3D.FromDirection(q, AxisV).Intersection(Line3D.FromDirection(Origin, AxisU));
      // if (U_int.ValueType == typeof(NullValue)) {
      //   throw new Exception("failed to project the 3D point on the AxisU");
      // }
      // double u = ((Point3D)U_int.Value - Origin).Length();

      // var V_int = Line3D.FromDirection(q, AxisU).Intersection(Line3D.FromDirection(Origin, AxisV));
      // if (V_int.ValueType == typeof(NullValue)) {
      //   throw new Exception("failed to project the 3D point on the AxisV");
      // }
      // double v = ((Point3D)V_int.Value - Origin).Length();

      // return new Point2D(u, v);
    }

    /// <summary>
    /// Project (vertically) given 3D XYZ point into plane,
    /// returning the UV coordinates of the result
    /// in the local 2D plane coordinate system.
    /// It's based on the formula of cosine. Given vectors A, B and theta = angle(A,B)
    /// cos(theta) = A*B / |A|*|B|
    /// The projection of a vector B onto another A is then
    /// |B|*cos(theta) = A*B / |A|
    /// ProjectInto projects the Point3D p onto the same 3D plane. Then projects the point on the AxisU and AxisV of
    /// the plane, and returns the 2D coordinates of the point along the basis AxisU,AxisV.
    /// </summary>
    public Point2D VerticalProjectInto(Point3D p) {
      var q = VerticalProjectOnto(p);
      var B = q - Origin;
      double B_len = B.Length();
      double B_cos = B.DotProduct(AxisU);
      double B_sin = B.DotProduct(AxisV);
      return new Point2D(B_cos, B_sin);
    }

    // evaluation (2D to 3D)

    /// <summary>
    /// We transform a plane's 2D point to the 3D version of it.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Point3D Evaluate(Point2D p) => Origin + AxisU * p.U + AxisV * p.V;

    public List<Point3D> Evaluate(List<Point2D> point_list_2d) =>
        new List<Point3D>(point_list_2d.Select(p => Evaluate(p)));

    public LineSegment3D Evaluate(LineSegment2D shape_2d) => LineSegment3D.FromPoints(Evaluate(shape_2d.P0),
                                                                                      Evaluate(shape_2d.P1));

    public Triangle3D Evaluate(Triangle2D shape_2d) => Triangle3D.FromPoints(Evaluate(shape_2d.P0),
                                                                             Evaluate(shape_2d.P1),
                                                                             Evaluate(shape_2d.P2));

    public Polygon3D Evaluate(Polygon2D shape_2d) => new Polygon3D(shape_2d.Select(p => Evaluate(p)));

    // special formatting
    public override string ToString() {
      return "PLANE {orig=" + Origin.ToString() + ", normal=" + Normal.ToString() + ", AxisU=" + AxisU.ToString() +
             ", AxisV=" + AxisV.ToString() + "}";
    }

    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return string.Format(
          "GEOMETRYCOLLECTION (" +

              "POINT (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}") + ")" +
              "," +

              "LINESTRING (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}") +
              ", " + String.Format("{0}3:F{1:D}{2} {0}4:F{1:D}{2} {0}5:F{1:D}{2}", "{", precision, "}") + ")",

          "LINESTRING (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}") + ", " +
              String.Format("{0}6:F{1:D}{2} {0}7:F{1:D}{2} {0}8:F{1:D}{2}", "{", precision, "}") + ")" + ")",

          "LINESTRING (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}") + ", " +
              String.Format("{0}9:F{1:D}{2} {0}10:F{1:D}{2} {0}11:F{1:D}{2}", "{", precision, "}") + ")" + ")"

              + ")"

          ,
          Origin.X,
          Origin.Y,
          Origin.Z,
          Normal.X,
          Normal.Y,
          Normal.Z,
          AxisU.X,
          AxisU.Y,
          AxisU.Z,
          AxisV.X,
          AxisV.Y,
          AxisV.Z);
    }

    // relationship to other geometries

    //  plane
    public bool Intersects(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);

    /// <summary>
    /// Tells if a plane intersects another and computes the Line intesection (or null if they don't intersect).
    /// If first checks whther the plane is parallel to the other.
    /// If not, it finds the direction line first, U = n1 x n2, and the origin point P = intersection of three planes.
    /// The three planes involved are this plane, the other plane, and the plane with normal n3 = U and passing
    /// through the origin O. 11 adds and 23 multiplies and always works (three planes linearly independent as they
    /// are all perpendicular)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Intersection(Plane other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (Normal.IsParallel(other.Normal, decimal_precision)) {
        return new IntersectionResult();
      }

      // var plane3 = Plane.FromPointAndNormal(Point3D.Zero, Normal.CrossProduct(other.Normal).Normalize());
      var U = Normal.CrossProduct(other.Normal);

      (double d1, double d2) = (-Normal.DotProduct(Origin), -other.Normal.DotProduct(other.Origin));

      var pI = Point3D.Zero + (d2 * Normal - d1 * other.Normal).CrossProduct(U) / U.DotProduct(U);

      if (!(Contains(pI, decimal_precision) && other.Contains(pI, decimal_precision))) {
        throw new Exception("plane.Intersection(plane) failed computation: pI does not belong to planes");
      }

      return new IntersectionResult(Line3D.FromDirection(pI, U.Normalize()));
    }

    //  geometry collection
    public bool Intersects(GeometryCollection3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => other.Intersects(this,
                                                                                                 decimal_precision);
    public IntersectionResult Intersection(GeometryCollection3D other,
                                           int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public bool Overlaps(GeometryCollection3D other,
                         int decimal_precision = Constants.THREE_DECIMALS) => other.Overlaps(this, decimal_precision);
    public IntersectionResult Overlap(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  line
    public bool Intersects(Line3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => other.Intersects(this,
                                                                                                 decimal_precision);
    public IntersectionResult Intersection(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public bool Overlaps(Line3D other,
                         int decimal_precision = Constants.THREE_DECIMALS) => other.Overlaps(this, decimal_precision);
    public IntersectionResult Overlap(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  line segment
    public bool Intersects(LineSegment3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => other.Intersects(this,
                                                                                                 decimal_precision);
    public IntersectionResult Intersection(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public bool Overlaps(LineSegment3D other,
                         int decimal_precision = Constants.THREE_DECIMALS) => other.Overlaps(this, decimal_precision);
    public IntersectionResult Overlap(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  line segment set
    public bool Intersects(LineSegmentSet3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => other.Intersects(this,
                                                                                                 decimal_precision);
    public IntersectionResult Intersection(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public bool Overlaps(LineSegmentSet3D other,
                         int decimal_precision = Constants.THREE_DECIMALS) => other.Overlaps(this, decimal_precision);
    public IntersectionResult Overlap(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  point set
    public bool Intersects(PointSet3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => other.Intersects(this,
                                                                                                 decimal_precision);
    public IntersectionResult Intersection(PointSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public bool Overlaps(PointSet3D other,
                         int decimal_precision = Constants.THREE_DECIMALS) => other.Overlaps(this, decimal_precision);
    public IntersectionResult Overlap(PointSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  polygon
    public bool Intersects(Polygon3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => other.Intersects(this,
                                                                                                 decimal_precision);
    public IntersectionResult Intersection(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public bool Overlaps(Polygon3D other,
                         int decimal_precision = Constants.THREE_DECIMALS) => other.Overlaps(this, decimal_precision);
    public IntersectionResult Overlap(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  polyline
    public bool Intersects(Polyline3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => other.Intersects(this,
                                                                                                 decimal_precision);
    public IntersectionResult Intersection(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public bool Overlaps(Polyline3D other,
                         int decimal_precision = Constants.THREE_DECIMALS) => other.Overlaps(this, decimal_precision);
    public IntersectionResult Overlap(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  ray
    public bool Intersects(Ray3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => other.Intersects(this,
                                                                                                 decimal_precision);
    public IntersectionResult Intersection(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public bool Overlaps(Ray3D other,
                         int decimal_precision = Constants.THREE_DECIMALS) => other.Overlaps(this, decimal_precision);
    public IntersectionResult Overlap(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  triangle
    public bool Intersects(Triangle3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => other.Intersects(this,
                                                                                                 decimal_precision);
    public IntersectionResult Intersection(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public bool Overlaps(Triangle3D other,
                         int decimal_precision = Constants.THREE_DECIMALS) => other.Overlaps(this, decimal_precision);
    public IntersectionResult Overlap(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    // perpendicular to other geometries
    public bool IsPerpendicular(Vector3D vec, int decimal_precision = Constants.THREE_DECIMALS) =>
        vec.IsPerpendicular(this, decimal_precision);
    public bool IsPerpendicular(Line3D line, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.IsPerpendicular(this, decimal_precision);
    public bool IsPerpendicular(Ray3D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        ray.IsPerpendicular(this, decimal_precision);
    public bool IsPerpendicular(LineSegment3D segment, int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.IsPerpendicular(this, decimal_precision);
  }
}
