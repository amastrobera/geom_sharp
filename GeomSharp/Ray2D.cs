using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp {
  /// <summary>
  /// A Ray on an arbitrary 2D plane
  /// </summary>
  public class Ray2D : IEquatable<Ray2D> {
    public Point2D Origin { get; }
    public UnitVector2D Direction { get; }

    public Ray2D(Point2D origin, UnitVector2D direction) {
      Origin = origin;
      Direction = direction;
    }

    public bool Equals(Ray2D other) => Origin.Equals(other.Origin) && Direction.Equals(other.Direction);

    public override bool Equals(object other) => other != null && other is Point2D && this.Equals((Point2D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Ray2D a, Ray2D b) {
      return a.Equals(b);
    }

    public static bool operator !=(Ray2D a, Ray2D b) {
      return !a.Equals(b);
    }

    public Line2D ToLine() => Line2D.FromDirection(Origin, Direction);

    /// <summary>
    /// Tells if a point is a ahead of the ray. It uses a property of the DotProduct (being positive in the angle beween
    /// two vectors is acute, zero if perpendicular, negative otherwise)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool IsAhead(Point2D p) => (Math.Round((p - Origin).DotProduct(Direction), Constants.THREE_DECIMALS) >= 0)
                                          ? true
                                          : false;

    /// <summary>
    /// Tells if a point is a behind of the ray. It uses a property of the DotProduct (being positive in the angle
    /// beween two vectors is acute, zero if perpendicular, negative otherwise)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool IsBehind(Point2D p) => (Math.Round((p - Origin).DotProduct(Direction), Constants.THREE_DECIMALS) < 0)
                                           ? true
                                           : false;

    public bool IsParallel(Ray2D other) => Direction.IsParallel(other.Direction);

    public bool IsPerpendicular(Ray2D other) => Direction.IsPerpendicular(other.Direction);

    public bool Contains(Point2D p) {
      // check if the point is on the same line
      if (Math.Round(Direction.PerpProduct(p - Origin), Constants.THREE_DECIMALS) != 0) {
        return false;
      }

      // check if the point is within the boundaries of the segment
      return IsAhead(p);
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
    public bool Intersects(Ray2D other) => Intersection(other).ValueType != typeof(NullValue);

    /// <summary>
    /// If two rays intersect, this return the point in which one of the is stroke through
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Intersection(Ray2D other) {
      var line_int = ToLine().Intersection(other.ToLine());
      if (line_int.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var Ps = (Point2D)line_int.Value;
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
    public bool Overlaps(Ray2D other) => Overlap(other).ValueType != typeof(NullValue);

    /// <summary>
    /// If two lines overlap, this function returns the shared section between them
    /// The returned line segment follows the same direction as this segment
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Overlap(Ray2D other) {
      if (!IsParallel(other)) {
        return new IntersectionResult();
      }

      bool origin_in = other.Contains(Origin);
      bool other_origin_in = Contains(other.Origin);

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
    public string ToWkt(int precision = Constants.NINE_DECIMALS, bool make_unit_line = false) {
      if (make_unit_line) {
        return string.Format("GEOMETRYCOLLECTION (" + "LINESTRING (" +
                                 String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ", " +
                                 String.Format("{0}2:F{1:D}{2} {0}3:F{1:D}{2}", "{", precision, "}") + ")" +
                                 ", POINT (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") +
                                 ")" + ")",
                             Origin.U,
                             Origin.V,
                             Origin.U + Direction.U,
                             Origin.V + Direction.V);
      }
      return string.Format("RAY (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ", " +
                               String.Format("{0}2:F{1:D}{2} {0}3:F{1:D}{2}", "{", precision, "}") + ")",
                           Origin.U,
                           Origin.V,
                           Direction.U,
                           Direction.V);
    }
  }

}
