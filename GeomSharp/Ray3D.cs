using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp {
  /// <summary>
  /// A Ray on an arbitrary 3D plane
  /// </summary>
  public class Ray3D : IEquatable<Ray3D> {
    public Point3D Origin { get; }
    public UnitVector3D Direction { get; }

    public Ray3D(Point3D origin, UnitVector3D direction) {
      Origin = origin;
      Direction = direction;
    }

    public bool AlmostEquals(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Origin.AlmostEquals(other.Origin, decimal_precision) && Direction.AlmostEquals(other.Direction,
                                                                                       decimal_precision);

    public bool Equals(Ray3D other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Point3D && this.Equals((Point3D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Ray3D a, Ray3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Ray3D a, Ray3D b) {
      return !a.AlmostEquals(b);
    }

    public Line3D ToLine() => Line3D.FromDirection(Origin, Direction);

    /// <summary>
    /// Tells if a point is a ahead of the ray. It uses a property of the DotProduct (being positive in the angle beween
    /// two vectors is acute, zero if perpendicular, negative otherwise)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool IsAhead(Point3D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        (Math.Round((p - Origin).DotProduct(Direction), decimal_precision) >= 0) ? true : false;

    /// <summary>
    /// Tells if a point is a behind of the ray. It uses a property of the DotProduct (being positive in the angle
    /// beween two vectors is acute, zero if perpendicular, negative otherwise)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool IsBehind(Point3D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        (Math.Round((p - Origin).DotProduct(Direction), decimal_precision) < 0) ? true : false;

    public bool IsParallel(Ray3D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => Direction.IsParallel(other.Direction,
                                                                                                     decimal_precision);

    public bool IsPerpendicular(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Direction.IsPerpendicular(other.Direction, decimal_precision);

    public bool Contains(Point3D p, int decimal_precision = Constants.THREE_DECIMALS) {
      // check if the point is on the same line
      if (!Direction.CrossProduct(p - Origin).AlmostEquals(Vector3D.Zero, decimal_precision)) {
        return false;
      }

      // check if the point is within the boundaries of the segment
      return IsAhead(p, decimal_precision);
    }

    public double DistanceTo(Point3D p) {
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
    public bool Intersects(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);

    /// <summary>
    /// If two rays intersect, this return the point in which one of the is stroke through
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Intersection(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) {
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

    /// <summary>
    /// Tells if two lines overlap (they share a common section)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);

    /// <summary>
    /// If two lines overlap, this function returns the shared section between them
    /// The returned line segment follows the same direction as this segment
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Overlap(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) {
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

    public override string ToString() =>
        "{" + String.Format("{0:F9} {1:F9} {2:F9}", Origin.X, Origin.Y, Origin.Z) +
        " -> " + String.Format("{0:F9} {1:F9} {2:F9}", Direction.X, Direction.Y, Direction.Z) + "}";
    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
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
  }

}
