using System;
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
  public class Ray2D : IEquatable<Ray2D>, ISerializable {
    public Point2D Origin { get; }
    public UnitVector2D Direction { get; }

    public Ray2D(Point2D origin, UnitVector2D direction) {
      Origin = origin;
      Direction = direction;
    }

    public bool AlmostEquals(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Origin.AlmostEquals(other.Origin, decimal_precision) && Direction.AlmostEquals(other.Direction,
                                                                                       decimal_precision);

    public bool Equals(Ray2D other) => this.AlmostEquals(other);
    public override bool Equals(object other) => other != null && other is Point2D && this.Equals((Point2D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Ray2D a, Ray2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Ray2D a, Ray2D b) {
      return !a.AlmostEquals(b);
    }

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

    public bool Contains(Point2D p, int decimal_precision = Constants.THREE_DECIMALS) {
      // if (p.AlmostEquals(Origin, decimal_precision)) {
      //   return true;
      // }

      // check if the point is on the same line
      if (Math.Round(Direction.PerpProduct(p - Origin), decimal_precision) != 0) {
        return false;
      }

      // check if the point is within the boundaries of the segment
      return IsAhead(p, decimal_precision);
    }

    public double DistanceTo(Point2D p) {
      if (IsBehind(p)) {
        return Origin.DistanceTo(p);
      }

      return ToLine().DistanceTo(p);
    }

    /// <summary>
    /// Tells if two rays intersect (one of them cuts throw the other, splitting it in two)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Intersects(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);

    /// <summary>
    /// If two rays intersect, this return the point in which one of the is stroke through
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Intersection(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) {
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

    /// <summary>
    /// Tells if two lines overlap (they share a common section)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);

    /// <summary>
    /// If two lines overlap, this function returns the shared section between them
    /// The returned line segment follows the same direction as this segment
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Overlap(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) {
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

    // special formatting
    public override string ToString() => "{" + String.Format("{0:F9} {1:F9}", Origin.U, Origin.V) +
                                         " -> " + String.Format("{0:F9} {1:F9}", Direction.U, Direction.V) + "}";
    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return string.Format("GEOMETRYCOLLECTION (" + "LINESTRING (" +
                               String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ", " +
                               String.Format("{0}2:F{1:D}{2} {0}3:F{1:D}{2}", "{", precision, "}") + ")" + ", POINT (" +
                               String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ")" + ")",
                           Origin.U,
                           Origin.V,
                           Origin.U + Direction.U,
                           Origin.V + Direction.V);
    }

    // serialization functions
    // Implement this method to serialize data. The method is called on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Origin", Origin, typeof(Point2D));
      info.AddValue("Direction", Direction, typeof(UnitVector2D));
    }
    // The special constructor is used to deserialize values.
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
