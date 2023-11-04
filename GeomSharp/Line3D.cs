using System;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Microsoft.SqlServer.Server;
using GeomSharp.Utils;

namespace GeomSharp {
  /// <summary>
  /// A Line on an arbitrary 3D plane
  /// It's either defined as an infinite straight line passing by two points
  /// or an infinite straight line passing by a point, with a given direction
  /// </summary>
  [Serializable]
  public class Line3D : Geometry3D, IEquatable<Line3D>, ISerializable {
    public Point3D P0 { get; }
    public Point3D P1 { get; }
    public Point3D Origin { get; }
    public UnitVector3D Direction { get; }

    // constructors
    public static Line3D FromPoints(Point3D p0, Point3D p1, int decimal_precision = Constants.THREE_DECIMALS) =>
        (p0.AlmostEquals(p1, decimal_precision))
            ? throw new NullLengthException("trying to initialize a line with two identical points")
            : new Line3D(p0, p1);

    public static Line3D FromDirection(Point3D orig,
                                       UnitVector3D dir,
                                       int decimal_precision = Constants.THREE_DECIMALS) =>
        new Line3D(orig.ToDecimals(decimal_precision), dir.ToDecimals(decimal_precision).Normalize());

    private Line3D(Point3D p0, Point3D p1) {
      P0 = p0;
      P1 = p1;
      Origin = P0;
      Direction = (p1 - p0).Normalize();
    }

    private Line3D(Point3D origin, UnitVector3D direction) {
      Origin = origin;
      Direction = direction;
      P0 = Origin;
      P1 = Origin + 1 * Direction;  // simbolic
    }

    // generic overrides from object class
    public override string ToString() => base.ToString();
    public override int GetHashCode() => Direction.ToWkt().GetHashCode();

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is Line3D && this.Equals((Line3D)other);
    public override bool Equals(Geometry3D other) => other.GetType() == typeof(Line3D) && this.Equals(other as Line3D);
    public bool Equals(Line3D other) => this.AlmostEquals(other);
    public override bool AlmostEquals(Geometry3D other, int decimal_precision = 3) =>
        other.GetType() == typeof(Line3D) && this.AlmostEquals(other as Line3D, decimal_precision);
    public bool AlmostEquals(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) && Direction.AlmostEquals(other.Direction, decimal_precision);

    // comparison operators
    public static bool operator ==(Line3D a, Line3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Line3D a, Line3D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("P0", P0, typeof(Point3D));
      info.AddValue("P1", P1, typeof(Point3D));
      info.AddValue("Origin", Origin, typeof(Point3D));
      info.AddValue("Direction", Direction, typeof(UnitVector3D));
    }

    public Line3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      P0 = (Point3D)info.GetValue("P0", typeof(Point3D));
      P1 = (Point3D)info.GetValue("P1", typeof(Point3D));
      Origin = (Point3D)info.GetValue("Origin", typeof(Point3D));
      Direction = (UnitVector3D)info.GetValue("Direction", typeof(UnitVector3D));
    }

    public static Line3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Line3D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return "LINE (" + Origin.ToWkt(precision) + "," + Direction.ToWkt(precision) + ")";
    }

    // own functions
    public bool IsParallel(Line3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => Direction.IsParallel(other.Direction,
                                                                                                     decimal_precision);

    public bool IsPerpendicular(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Direction.IsPerpendicular(other.Direction, decimal_precision);

    /// <summary>
    /// Projects a Point onto a line
    /// The fomula is
    /// P(b) = P0 + b*(P1-P0) = P0 + W*vl *vl
    /// where
    /// b = d(P0, Pb) / d(P0,P1) = W*Vl / Vl*Vl = W*vl / Vl
    /// W=p-P0, Vl=P1-P0, vl=Vl/|Vl| unit vector
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Point3D ProjectOnto(Point3D p) => Origin + (p - Origin).DotProduct(Direction) * Direction;

    /// <summary>
    /// Distance from a point
    /// In 3D the formula is d(p,L) = | Vl x W | / |Vl| = |vl x W|
    /// where Vl=P1-P0, vl = Vl/|Vl|, W=p-P0, and (P0,P1) are the points of the line
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public double DistanceTo(Point3D p) => Direction.CrossProduct(p - Origin).Length();

    // relationship to all the other geometries
    //  plane
    public override bool Intersects(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Plane other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other.Contains(this, decimal_precision)) {
        return new IntersectionResult();
      }
      // Daniel Sunday's magic
      var U = Direction;
      var W = Origin - other.Origin;
      var n = other.Normal;

      double sI = -n.DotProduct(W) / n.DotProduct(U);

      Point3D q = Origin + sI * U;

      if (!other.Contains(q, decimal_precision)) {
        throw new Exception("plane.Intersection(Line3D) failed");
      }

      return new IntersectionResult(q);
    }

    public override bool Overlaps(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Overlap(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Normal.IsPerpendicular(Direction, decimal_precision) && other.Contains(Origin, decimal_precision)
            ? new IntersectionResult(this)
            : new IntersectionResult();

    // point
    public override bool Contains(Point3D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        p.AlmostEquals(Origin, decimal_precision) || (p - Origin).IsParallel(Direction, decimal_precision);

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
      if (IsParallel(other, decimal_precision)) {
        return new IntersectionResult();
      }

      // pick the first plane 2D where both lines can be projected as lines and not as dots
      // (verify that neither line is perpendicular to the projecting plane)
      Plane plane_2d =
          !(this.IsPerpendicular(Plane.XY, decimal_precision) || other.IsPerpendicular(Plane.XY, decimal_precision))
              ? Plane.XY
              : (!(this.IsPerpendicular(Plane.YZ, decimal_precision) ||
                   other.IsPerpendicular(Plane.YZ, decimal_precision))
                     ? Plane.YZ
                     : Plane.ZX);

      (var p1, var other_p1) = (plane_2d.ProjectInto(Origin), plane_2d.ProjectInto(other.Origin));
      (var p2, var other_p2) =
          (plane_2d.ProjectInto(Origin + 2 * Direction), plane_2d.ProjectInto(other.Origin + 2 * other.Direction));
      (var line_2d, var other_line_2d) =
          (Line2D.FromPoints(p1, p2, decimal_precision), Line2D.FromPoints(other_p1, other_p2, decimal_precision));

      var inter_res = line_2d.Intersection(other_line_2d, decimal_precision);
      if (inter_res.ValueType == typeof(NullValue)) {
        // no 2D intersection, no 3D intersection either
        return new IntersectionResult();
      }
      // there is a 2D intersection, let's get the respective 3D point
      var pI_2d = (Point2D)inter_res.Value;

      // given the linear relationship between the 3D line and the projected 2D line, we can find the point on
      // the 3D line with a length ratio
      var A = Origin;
      var B = Origin + 2 * Direction;
      var a = plane_2d.ProjectInto(A);
      var b = plane_2d.ProjectInto(B);
      var len_ratio = (pI_2d - a).Length() / (b - a).Length();
      Point3D pI = A + len_ratio * (B - A);

      // and verify it belongs to both lines
      if (!(Contains(pI, decimal_precision) && other.Contains(pI, decimal_precision))) {
        // this is merely a 2D intersection, 3D lines do not intersect
        return new IntersectionResult();
      }

      // all is well, yea yea
      return new IntersectionResult(pI);
    }

    public override bool Overlaps(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!IsParallel(other, decimal_precision)) {
        return false;
      }
      if (Contains(other.P0, decimal_precision) || Contains(other.P1, decimal_precision)) {
        return true;
      }
      return false;
    }

    public override IntersectionResult Overlap(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  line segment
    public override bool Intersects(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(LineSegment3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlaps(this, decimal_precision);
    public override IntersectionResult Overlap(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

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
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Polygon3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polyline
    public override bool Intersects(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Polyline3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  ray
    public override bool Intersects(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var line_inter = Intersection(other.ToLine(), decimal_precision);
      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)line_inter.Value;
      if (other.Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }
    public override bool Overlaps(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Overlap(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!other.Direction.IsParallel(Direction, decimal_precision)) {
        return new IntersectionResult();
      }

      if (Contains(other.Origin, decimal_precision)) {
        return new IntersectionResult(other);
      }

      return new IntersectionResult();
    }

    //  triangle
    public override bool Intersects(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Triangle3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlaps(this, decimal_precision);
    public override IntersectionResult Overlap(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    // operations with plane
    public bool IsPerpendicular(Plane plane, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Normal.IsParallel(Direction, decimal_precision) && plane.AxisU.IsPerpendicular(Direction,
                                                                                             decimal_precision);
  }
}
