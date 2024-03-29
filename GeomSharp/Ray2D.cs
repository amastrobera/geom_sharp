﻿using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GeomSharp {
  /// <summary>
  /// A Ray on an arbitrary 2D plane
  /// </summary>
  [Serializable]
  public class Ray2D : Geometry2D, IEquatable<Ray2D>, ISerializable {
    public Point2D Origin { get; }
    public UnitVector2D Direction { get; }

    // constructors
    public Ray2D(Point2D origin, UnitVector2D direction) {
      Origin = origin;
      Direction = direction;
    }

    // generic overrides from object class
    public override int GetHashCode() => ToWkt().GetHashCode();
    public override string ToString() => "{" + String.Format("{0:F9} {1:F9}", Origin.U, Origin.V) +
                                         " -> " + String.Format("{0:F9} {1:F9}", Direction.U, Direction.V) + "}";

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is Ray2D && this.Equals((Ray2D)other);
    public override bool Equals(Geometry2D other) => other.GetType() == typeof(Ray2D) && this.Equals(other as Ray2D);

    public bool Equals(Ray2D other) => this.AlmostEquals(other);

    public override bool AlmostEquals(Geometry2D other, int decimal_precision = 3) =>
        other.GetType() == typeof(Ray2D) && this.AlmostEquals(other as Ray2D, decimal_precision);

    public bool AlmostEquals(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) && Origin.AlmostEquals(other.Origin, decimal_precision) &&
        Direction.AlmostEquals(other.Direction, decimal_precision);

    // comparison operators
    public static bool operator ==(Ray2D a, Ray2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Ray2D a, Ray2D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Origin", Origin, typeof(Point2D));
      info.AddValue("Direction", Direction, typeof(UnitVector2D));
    }
    public Ray2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Origin = (Point2D)info.GetValue("Origin", typeof(Point2D));
      Direction = (UnitVector2D)info.GetValue("Direction", typeof(UnitVector2D));
    }

    public static Ray2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Ray2D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return string.Format("GEOMETRYCOLLECTION (" + "LINESTRING (" +
                               String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ", " +
                               String.Format("{0}2:F{1:D}{2} {0}3:F{1:D}{2}", "{", precision, "}") + ")" + ", POINT (" +
                               String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ")" + ")",
                           Origin.U,
                           Origin.V,
                           Origin.U + Direction.U,
                           Origin.V + Direction.V);
    }
    public override Geometry2D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

    // relationship to all the other geometries

    //  point
    public override bool Contains(Point2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      // if (p.AlmostEquals(Origin, decimal_precision)) {
      //   return true;
      // }

      // check if the point is on the same line
      if (Math.Round(Direction.PerpProduct(other - Origin), decimal_precision) != 0) {
        return false;
      }

      // check if the point is within the boundaries of the segment
      return IsAhead(other, decimal_precision);
    }

    //  geometry collection
    public override bool Intersects(GeometryCollection2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(GeometryCollection2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(GeometryCollection2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(GeometryCollection2D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line
    public override bool Intersects(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlaps(this, decimal_precision);
    public override IntersectionResult Overlap(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  line segment
    public override bool Intersects(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(LineSegment2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlaps(this, decimal_precision);
    public override IntersectionResult Overlap(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    //  line segment set
    public override bool Intersects(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(LineSegmentSet2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(LineSegmentSet2D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polygon
    public override bool Intersects(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Polygon2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polyline
    public override bool Intersects(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Polyline2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  ray
    public override bool Intersects(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var line_int = ToLine().Intersection(other.ToLine(), decimal_precision);
      if (line_int.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var Ps = (Point2D)line_int.Value;
      if (!(Contains(Ps, decimal_precision) && other.Contains(Ps, decimal_precision))) {
        return new IntersectionResult();
      }

      return new IntersectionResult(Ps);
    }
    public override bool Overlaps(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Overlap(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) {
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

      return new IntersectionResult(LineSegment2D.FromPoints(Origin, other.Origin));
    }

    //  triangle
    public override bool Intersects(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Triangle2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlaps(this, decimal_precision);
    public override IntersectionResult Overlap(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Overlap(this, decimal_precision);

    // own functions
    public Line2D ToLine() => Line2D.FromDirection(Origin, Direction);

    /// <summary>
    /// Tells if a point is a ahead of the ray. It uses a property of the DotProduct (being positive in the angle beween
    /// two vectors is acute, zero if perpendicular, negative otherwise)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool IsAhead(Point2D p, int decimal_precision = Constants.THREE_DECIMALS) => !IsBehind(p, decimal_precision);

    /// <summary>
    /// Tells if a point is a behind of the ray. It uses a property of the DotProduct (being positive in the angle
    /// beween two vectors is acute, zero if perpendicular, negative otherwise)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool IsBehind(Point2D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        Math.Round(Direction.DotProduct(p - Origin), decimal_precision) < 0 ? true : false;

    public bool IsParallel(Ray2D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => Direction.IsParallel(other.Direction,
                                                                                                     decimal_precision);

    public bool IsPerpendicular(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Direction.IsPerpendicular(other.Direction, decimal_precision);

    public double DistanceTo(Point2D p) {
      if (IsBehind(p)) {
        return Origin.DistanceTo(p);
      }

      return ToLine().DistanceTo(p);
    }
  }

}
