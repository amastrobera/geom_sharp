using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp {
  /// <summary>
  /// A LineSegment on an arbitrary 3D plane
  /// </summary>
  public class LineSegment3D : IEquatable<LineSegment3D> {
    public Point3D P0 { get; }
    public Point3D P1 { get; }

    private LineSegment3D(Point3D p0, Point3D p1) => (P0, P1) = (p0, p1);

    public static LineSegment3D FromPoints(Point3D p0, Point3D p1) {
      if (p0 == p1) {
        throw new ArgumentException("trying to initialize a line-segment with two identical points");
      }
      return new LineSegment3D(p0, p1);
    }

    public bool Equals(LineSegment3D other) => P0.Equals(other.P0) && P1.Equals(other.P1);

    public override bool Equals(object other) => other != null && other is LineSegment3D &&
                                                 this.Equals((LineSegment3D)other);

    public override int GetHashCode() => ToWkt().GetHashCode();

    public static bool operator ==(LineSegment3D a, LineSegment3D b) {
      return a.Equals(b);
    }

    public static bool operator !=(LineSegment3D a, LineSegment3D b) {
      return !a.Equals(b);
    }

    public Line3D ToLine() => Line3D.FromPoints(P0, P1);
    public double Length() => (P1 - P0).Length();

    public double DistanceTo(Point3D p) {
      double line_d = ToLine().DistanceTo(p);
      double p0_d = P0.DistanceTo(p);
      double p1_d = P1.DistanceTo(p);
      return Math.Round(Math.Min(line_d, Math.Min(p0_d, p1_d)), Constants.NINE_DECIMALS);
    }

    public bool IsParallel(LineSegment3D other) => (P1 - P0).IsParallel(other.P1 - other.P0);

    public bool IsPerpendicular(Line3D other) => (P1 - P0).IsPerpendicular(other.P1 - other.P0);

    public bool Contains(Point3D p) {
      // check if the point is on the same line
      if (!(P1 - P0).CrossProduct(p - P0).AlmostEquals(Vector3D.Zero)) {
        return false;
      }

      // check if the point is within the boundaries of the segment
      if (p.AlmostEquals(P0)) {  // this check avoids throwing on .SameDirection() call.
        return true;
      }
      if ((p - P0).SameDirectionAs(P1 - P0)) {
        double t = Math.Round(P0.DistanceTo(p) / Length(), Constants.THREE_DECIMALS);
        if (t >= 0 && t <= 1) {
          return true;
        }
      }
      return false;
    }

    public Point3D ProjectOnto(Point3D p) {
      var pp = ToLine().ProjectOnto(p);
      if (!Contains(pp)) {
        if (Math.Round(P0.DistanceTo(pp) - P1.DistanceTo(pp), Constants.NINE_DECIMALS) < 0) {
          return P0;
        }
        return P1;
      }
      return pp;
    }

    /// <summary>
    /// Percentage location of the point on the line segment.
    /// It throws if the point does not belong to the line
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public double LocationPct(Point3D point) =>
        !Contains(point) ? throw new Exception("LocationPct called with Point3D that does not belong to the line")
                         : Math.Round(P0.DistanceTo(point) / Length(), Constants.NINE_DECIMALS);

    public bool Intersects(LineSegment3D other) => Intersection(other).ValueType != typeof(NullValue);

    public IntersectionResult Intersection(LineSegment3D other) {
      var int_res = ToLine().Intersection(other.ToLine());

      if (int_res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)int_res.Value;
      if (Contains(pI) && other.Contains(pI)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    /// <summary>
    /// Tells if two lines overlap (they share a common section)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(LineSegment3D other) => Overlap(other).ValueType != typeof(NullValue);

    /// <summary>
    /// If two lines overlap, this function returns the shared section between them
    /// The returned line segment follows the same direction as this segment
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Overlap(LineSegment3D other) {
      if (!IsParallel(other)) {
        return new IntersectionResult();
      }
      (bool other_p0_in, bool other_p1_in) = (Contains(other.P0), Contains(other.P1));
      (bool p0_in, bool p1_in) = (other.Contains(P0), other.Contains(P1));

      var direction = (P1 - P0).Normalize();

      if (!p0_in && !p1_in && other_p0_in && other_p1_in) {
        // the other is contained in the segment
        return new IntersectionResult(
            (direction == (other.P1 - other.P0).Normalize()) ? other : new LineSegment3D(other.P1, other.P0));
      } else if (p0_in && !p1_in && !other_p0_in && other_p1_in) {
        // segment's last point in, other's first point in
        return new IntersectionResult((direction == (other.P1 - P0).Normalize()) ? new LineSegment3D(P0, other.P1)
                                                                                 : new LineSegment3D(other.P1, P0));
      } else if (!p0_in && p1_in && other_p0_in && !other_p1_in) {
        // segment's last point in, other's first point in
        return new IntersectionResult((direction == (P1 - other.P0).Normalize()) ? new LineSegment3D(other.P0, P1)
                                                                                 : new LineSegment3D(P1, other.P0));
      }
      // if (p0_in && p1_in && !other_p0_in && !other_p1_in)
      //  the segment is contained in the other
      return new IntersectionResult(this);
    }

    // special formatting
    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return "LINESTRING (" +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                           P0.X,
                           P0.Y,
                           P0.Z) +
             "," +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                           P1.X,
                           P1.Y,
                           P1.Z) +
             ")";
    }
  }

}
