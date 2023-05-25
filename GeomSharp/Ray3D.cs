using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using GeomSharp;

namespace GeomSharp {
  /// <summary>
  /// A Ray on an arbitrary 3D plane
  /// </summary>
  [Serializable]
  public class Ray3D : Geometry3D, IEquatable<Ray3D>, ISerializable {
    public Point3D Origin { get; }
    public UnitVector3D Direction { get; }

    // constructors
    public Ray3D(Point3D origin, UnitVector3D direction) {
      Origin = origin;
      Direction = direction;
    }

    // generic overrides from object class
    public override string ToString() =>
        "{" + String.Format("{0:F9} {1:F9} {2:F9}", Origin.X, Origin.Y, Origin.Z) +
        " -> " + String.Format("{0:F9} {1:F9} {2:F9}", Direction.X, Direction.Y, Direction.Z) + "}";
    public override int GetHashCode() => ToWkt().GetHashCode();

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is Ray3D && this.Equals((Ray3D)other);
    public override bool Equals(Geometry3D other) => other.GetType() == typeof(Ray3D) && this.Equals(other as Ray3D);
    public bool Equals(Ray3D other) => this.AlmostEquals(other);
    public override bool AlmostEquals(Geometry3D other, int decimal_precision = 3) =>
        other.GetType() == typeof(Ray3D) && this.AlmostEquals(other as Ray3D, decimal_precision);

    public bool AlmostEquals(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) && Origin.AlmostEquals(other.Origin, decimal_precision) &&
        Direction.AlmostEquals(other.Direction, decimal_precision);

    // comparison operators
    public static bool operator ==(Ray3D a, Ray3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Ray3D a, Ray3D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Origin", Origin, typeof(Point3D));
      info.AddValue("Direction", Direction, typeof(UnitVector3D));
    }

    public Ray3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Origin = (Point3D)info.GetValue("Origin", typeof(Point3D));
      Direction = (UnitVector3D)info.GetValue("Direction", typeof(UnitVector3D));
    }

    public static Ray3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Ray3D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return string.Format(
          "GEOMETRYCOLLECTION (" + "LINESTRING (" +
              String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}") + ", " +
              String.Format("{0}3:F{1:D}{2} {0}4:F{1:D}{2} {0}5:F{1:D}{2}", "{", precision, "}") + ")" + ", POINT (" +
              String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}") + ")" + ")",
          Origin.X,
          Origin.Y,
          Origin.Z,
          Origin.X + Direction.X,
          Origin.Y + Direction.Y,
          Origin.Z + Direction.Z);
    }

    public override Geometry3D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

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

      if (Math.Round(sI, decimal_precision) < 0) {
        return new IntersectionResult();
      }

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
    public override bool Contains(Point3D p, int decimal_precision = Constants.THREE_DECIMALS) {
      // if (p.AlmostEquals(Origin, decimal_precision)) {
      //   return true;
      // }

      // check if the point is on the same line
      if (!Direction.CrossProduct(p - Origin).AlmostEquals(Vector3D.Zero, decimal_precision)) {
        return false;
      }

      // check if the point is within the boundaries of the segment
      return IsAhead(p, decimal_precision);
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
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlaps(this, decimal_precision);
    public override IntersectionResult Overlap(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

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
        other.Overlaps(this, decimal_precision);
    public override IntersectionResult Overlap(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  polyline
    public override bool Intersects(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Polyline3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlaps(this, decimal_precision);
    public override IntersectionResult Overlap(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  ray
    public override bool Intersects(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var line_int = ToLine().Intersection(other.ToLine(), decimal_precision);
      if (line_int.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var Ps = (Point3D)line_int.Value;
      if (!(Contains(Ps) && other.Contains(Ps))) {
        return new IntersectionResult();
      }

      return new IntersectionResult(Ps);
    }
    public override bool Overlaps(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);

    public override IntersectionResult Overlap(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!IsParallel(other, decimal_precision)) {
        return new IntersectionResult();
      }

      bool origin_in = other.Contains(Origin, decimal_precision);
      bool other_origin_in = Contains(other.Origin, decimal_precision);

      if (!other_origin_in && !origin_in) {
        return new IntersectionResult();
      }

      // the other is contained in the ray
      if (!other_origin_in && origin_in) {
        return new IntersectionResult(this);
      }

      // ray's last point in, other's first point in
      if (other_origin_in && !origin_in) {
        return new IntersectionResult(other);
      }
      //  if (other_origin_in && origin_in)
      //  the rays shoot towards each other

      return new IntersectionResult(LineSegment3D.FromPoints(Origin, other.Origin));
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

    // own functions
    public Line3D ToLine() => Line3D.FromDirection(Origin, Direction);

    /// <summary>
    /// Tells if a point is a ahead of the ray. It uses a property of the DotProduct (being positive in the angle beween
    /// two vectors is acute, zero if perpendicular, negative otherwise)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool IsAhead(Point3D p, int decimal_precision = Constants.THREE_DECIMALS) => !IsBehind(p, decimal_precision);

    /// <summary>
    /// Tells if a point is a behind of the ray. It uses a property of the DotProduct (being positive in the angle
    /// beween two vectors is acute, zero if perpendicular, negative otherwise)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool IsBehind(Point3D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        Math.Round(Direction.DotProduct(p - Origin), decimal_precision) < 0 ? true : false;

    public bool IsParallel(Ray3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => Direction.IsParallel(other.Direction,
                                                                                                     decimal_precision);

    public bool IsPerpendicular(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Direction.IsPerpendicular(other.Direction, decimal_precision);

    public double DistanceTo(Point3D p) {
      if (IsBehind(p)) {
        return Origin.DistanceTo(p);
      }

      return ToLine().DistanceTo(p);
    }

    // operations with plane
    public bool IsPerpendicular(Plane plane, int decimal_precision = Constants.THREE_DECIMALS) =>
        ToLine().IsPerpendicular(plane, decimal_precision);  // && ray.IsAhead(plane.Origin);
  }

}
