﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp {
  /// <summary>
  /// A LineSegment on an arbitrary 2D plane
  /// </summary>
  public class LineSegment2D : IEquatable<LineSegment2D> {
    public Point2D P0 { get; }
    public Point2D P1 { get; }

    private LineSegment2D(Point2D p0, Point2D p1) => (P0, P1) = (p0, p1);

    public static LineSegment2D FromPoints(Point2D p0, Point2D p1, int decimal_precision = Constants.THREE_DECIMALS) {
      if (p0.AlmostEquals(p1, decimal_precision)) {
        throw new ArgumentException("trying to initialize a line-segment with two identical points");
      }
      return new LineSegment2D(p0, p1);
    }

    public bool AlmostEquals(LineSegment2D other) => P0.AlmostEquals(other.P0) && P1.AlmostEquals(other.P1);

    public bool Equals(LineSegment2D other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is LineSegment2D &&
                                                 this.Equals((LineSegment2D)other);

    public override int GetHashCode() => ToWkt().GetHashCode();

    public static bool operator ==(LineSegment2D a, LineSegment2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(LineSegment2D a, LineSegment2D b) {
      return !a.AlmostEquals(b);
    }

    public Line2D ToLine() => Line2D.FromTwoPoints(P0, P1);

    public double Length() => (P1 - P0).Length();

    public double DistanceTo(Point2D p) => Math.Abs(SignedDistanceTo(p));

    public double SignedDistanceTo(Point2D p) {
      double line_d = ToLine().SignedDistanceTo(p);
      double p0_d = P0.DistanceTo(p);
      double p1_d = P1.DistanceTo(p);
      return Math.Min(Math.Abs(line_d), Math.Min(p0_d, p1_d)) * Math.Sign(line_d);
    }

    /// <summary>
    /// Tells what the location is for a Point relative to the Line, on the 2D plane
    /// Can be done in 3D too, by creating a plane of the line + point, and projecting all into it, then using this
    /// method
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Constants.Location Location(Point2D p, int decimal_precision = Constants.THREE_DECIMALS) {
      var line_location = ToLine().Location(p, decimal_precision);

      if (line_location == Constants.Location.ON_LINE) {
        if (p.AlmostEquals(P0, decimal_precision)) {  // this check avoids throwing on .SameDirection() call.
          line_location = Constants.Location.ON_SEGMENT;
        } else {
          if ((p - P0).SameDirectionAs(P1 - P0, decimal_precision)) {
            double t = Math.Round(P0.DistanceTo(p) / Length(), decimal_precision);
            if (t >= 0 && t <= 1) {
              line_location = Constants.Location.ON_SEGMENT;
            }
          }
        }
      }
      return line_location;
    }

    public bool IsParallel(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        (P1 - P0).IsParallel(other.P1 - other.P0, decimal_precision);

    public bool IsPerpendicular(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        (P1 - P0).IsPerpendicular(other.P1 - other.P0, decimal_precision);

    public bool Contains(Point2D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        Location(p, decimal_precision) == Constants.Location.ON_SEGMENT;

    public Point2D ProjectOnto(Point2D p) {
      var pp = ToLine().ProjectOnto(p);
      int decimal_precision = Constants.THREE_DECIMALS;
      if (!Contains(pp, decimal_precision)) {
        if (Math.Round(P0.DistanceTo(pp) - P1.DistanceTo(pp), decimal_precision) < 0) {
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
    public double LocationPct(Point2D point, int decimal_precision = Constants.THREE_DECIMALS) =>
        !Contains(point, decimal_precision)
            ? throw new Exception("LocationPct called with Point2D that does not belong to the line")
            : Math.Round(P0.DistanceTo(point) / Length(), decimal_precision);

    /// <summary>
    /// Tells whether two line segments intersect.
    /// First the parallelism is tested. Then the crossing. Then, the intersection point being contained inside both
    /// segments
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Intersects(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      // overlaps or parallel
      if (IsParallel(other, decimal_precision)) {
        return false;
      }

      // overlaps (second case)
      if (Contains(other.P0, decimal_precision) && Contains(other.P1, decimal_precision)) {
        return false;
      }

      // I examine all cases to avoid the overflow problem caused by the (more simple) DistanceTo(P0)*DistanceTo(P1) < 0
      (double d_this_other_p0, double d_this_other_p1, double d_other_this_p0, double d_other_this_p1) =
          (Math.Round(SignedDistanceTo(other.P0), decimal_precision),
           Math.Round(SignedDistanceTo(other.P1), decimal_precision),
           Math.Round(other.SignedDistanceTo(P0), decimal_precision),
           Math.Round(other.SignedDistanceTo(P1),
                      decimal_precision));  // rounding issues will kill you if unmanaged

      return ((d_this_other_p0 >= 0 && d_this_other_p1 <= 0) || (d_this_other_p0 <= 0 && d_this_other_p1 >= 0)) &&
             ((d_other_this_p0 >= 0 && d_other_this_p1 <= 0) || (d_other_this_p0 <= 0 && d_other_this_p1 >= 0));
    }

    public IntersectionResult Intersection(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (IsParallel(other, decimal_precision)) {
        return new IntersectionResult();
      }
      // TODO: can be put this code in a common place, and avoid duplicating it over and over ?
      var V = other.P1 - other.P0;
      var U = P1 - P0;
      var W = P0 - other.P0;

      double sI = Math.Round(-V.PerpProduct(W) / V.PerpProduct(U),  // guaranteed non-zero if non-parallel
                             decimal_precision);

      double tI = Math.Round(U.PerpProduct(W) / U.PerpProduct(V),  // guaranteed non-zero if non-parallel
                             decimal_precision);

      if (sI >= 0 && sI <= 1 && tI >= 0 && tI <= 1) {
        // intersection: the point Ps cannot possibly be outside of the segment!
        var Ps = P0 + sI * (P1 - P0);
        if (!Contains(Ps, decimal_precision)) {
          throw new Exception("Intersects() miscalculated Ps");
        }
        // intersection: the point Qs cannot possibly be outside the other segment!
        var Qs = other.P0 + tI * (other.P1 - other.P0);
        if (!other.Contains(Qs, decimal_precision)) {
          throw new Exception("Intersects() miscalculated Qs");
        }
        return new IntersectionResult(Ps);
      }

      // no intersection: the point Ps cannot possibly belong to the segment!
      if (Contains(P0 + sI * (P1 - P0), decimal_precision)) {
        throw new Exception("Intersects() miscalculated Ps");
      }
      // no intersection: the point Qs cannot possibly belong to the other segment!
      if (!other.Contains(other.P0 + tI * (other.P1 - other.P0), decimal_precision)) {
        throw new Exception("Intersects() miscalculated Qs");
      }

      return new IntersectionResult();
    }

    /// <summary>
    /// Tells if two lines overlap (they share a common section)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!IsParallel(other, decimal_precision)) {
        return false;
      }
      if (Contains(other.P0, decimal_precision) || Contains(other.P1, decimal_precision) ||
          other.Contains(P0, decimal_precision) || other.Contains(P1, decimal_precision)) {
        return true;
      }
      return false;
    }

    /// <summary>
    /// If two lines overlap, this function returns the shared section between them
    /// The returned line segment follows the same direction as this segment
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Overlap(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!Overlaps(other, decimal_precision)) {
        return new IntersectionResult();
      }
      (bool other_p0_in, bool other_p1_in) =
          (Contains(other.P0, decimal_precision), Contains(other.P1, decimal_precision));
      (bool p0_in, bool p1_in) = (other.Contains(P0, decimal_precision), other.Contains(P1, decimal_precision));

      var direction = (P1 - P0).Normalize();

      if (!p0_in && !p1_in && other_p0_in && other_p1_in) {
        // the other is contained in the segment
        return direction.AlmostEquals((other.P1 - other.P0).Normalize(), decimal_precision)
                   ? new IntersectionResult(other)
                   : new IntersectionResult(new LineSegment2D(other.P1, other.P0));
      } else if (p0_in && !p1_in && !other_p0_in && other_p1_in) {
        // segment's last point in, other's first point in
        return direction.AlmostEquals((other.P1 - P0).Normalize(), decimal_precision)
                   ? new IntersectionResult(new LineSegment2D(P0, other.P1))
                   : new IntersectionResult(new LineSegment2D(other.P1, P0));
      } else if (!p0_in && p1_in && other_p0_in && !other_p1_in) {
        // segment's last point in, other's first point in
        return direction.AlmostEquals((P1 - other.P0).Normalize(), decimal_precision)
                   ? new IntersectionResult(new LineSegment2D(other.P0, P1))
                   : new IntersectionResult(new LineSegment2D(P1, other.P0));
      }
      // if (p0_in && p1_in && !other_p0_in && !other_p1_in)
      //  the segment is contained in the other
      return new IntersectionResult(this);
    }

    // text / deugging
    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return "LINESTRING (" +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"), P0.U, P0.V) + "," +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"), P1.U, P1.V) + ")";
    }
  }
}
