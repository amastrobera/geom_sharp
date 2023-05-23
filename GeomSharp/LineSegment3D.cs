using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GeomSharp {
  /// <summary>
  /// A LineSegment on an arbitrary 3D plane
  /// </summary>
  [Serializable]
  public class LineSegment3D : Geometry3D, IEquatable<LineSegment3D>, ISerializable {
    public Point3D P0 { get; }
    public Point3D P1 { get; }

    // constructors
    private LineSegment3D(Point3D p0, Point3D p1) => (P0, P1) = (p0, p1);

    public static LineSegment3D FromPoints(Point3D p0, Point3D p1, int decimal_precision = Constants.THREE_DECIMALS) {
      if (p0.AlmostEquals(p1, decimal_precision)) {
        throw new ArgumentException("trying to initialize a line-segment with two identical points");
      }
      return new LineSegment3D(p0, p1);
    }

    // generic overrides from object class
    public override string ToString() => base.ToString();
    public override int GetHashCode() => ToWkt().GetHashCode();

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is LineSegment3D &&
                                                 this.Equals((LineSegment3D)other);
    public override bool Equals(Geometry3D other) => other.GetType() == typeof(LineSegment3D) &&
                                                     this.Equals(other as LineSegment3D);
    public bool Equals(LineSegment3D other) => this.AlmostEquals(other);
    public override bool AlmostEquals(Geometry3D other, int decimal_precision = 3) =>
        other.GetType() == typeof(LineSegment3D) && this.AlmostEquals(other as LineSegment3D, decimal_precision);
    public bool AlmostEquals(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) && P0.AlmostEquals(other.P0, decimal_precision) &&
        P1.AlmostEquals(other.P1, decimal_precision);

    // comparison operators
    public static bool operator ==(LineSegment3D a, LineSegment3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(LineSegment3D a, LineSegment3D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("P0", P0, typeof(Point3D));
      info.AddValue("P1", P1, typeof(Point3D));
    }

    public LineSegment3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      P0 = (Point3D)info.GetValue("P0", typeof(Point3D));
      P1 = (Point3D)info.GetValue("P1", typeof(Point3D));
    }

    public static LineSegment3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (LineSegment3D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) {
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

    public override Geometry3D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

    // own functions
    public Line3D ToLine() => Line3D.FromPoints(P0, P1);
    public double Length() => (P1 - P0).Length();

    /// <summary>
    /// Almost equals from a to b, or from b to a
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool IsSameSegment(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        P0.AlmostEquals(other.P0, decimal_precision) && P1.AlmostEquals(other.P1, decimal_precision) ||
        P1.AlmostEquals(other.P0, decimal_precision) && P0.AlmostEquals(other.P1, decimal_precision);

    public double DistanceTo(Point3D p) {
      double line_d = ToLine().DistanceTo(p);
      double p0_d = P0.DistanceTo(p);
      double p1_d = P1.DistanceTo(p);
      return Math.Min(line_d, Math.Min(p0_d, p1_d));
    }

    public bool IsParallel(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        (P1 - P0).IsParallel(other.P1 - other.P0, decimal_precision);

    public bool IsPerpendicular(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        (P1 - P0).IsPerpendicular(other.P1 - other.P0, decimal_precision);

    public bool Contains(Point3D p, int decimal_precision = Constants.THREE_DECIMALS) {
      // check if the point is on the same line
      if (!(P1 - P0).CrossProduct(p - P0).AlmostEquals(Vector3D.Zero, decimal_precision)) {
        return false;
      }

      // check if the point is within the boundaries of the segment
      if (p.AlmostEquals(P0, decimal_precision)) {  // this check avoids throwing on .SameDirection() call.
        return true;
      }
      if ((p - P0).SameDirectionAs(P1 - P0, decimal_precision)) {
        double t = Math.Round(P0.DistanceTo(p) / Length(), decimal_precision);
        if (t >= 0 && t <= 1) {
          return true;
        }
      }
      return false;
    }

    public Point3D ProjectOnto(Point3D p) {
      var pp = ToLine().ProjectOnto(p);
      if (!Contains(pp)) {
        if (Math.Round(P0.DistanceTo(pp) - P1.DistanceTo(pp), Constants.THREE_DECIMALS) < 0) {
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

    public IntersectionResult Intersection(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var int_res = ToLine().Intersection(other.ToLine(), decimal_precision);

      if (int_res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)int_res.Value;
      if (Contains(pI, decimal_precision) && other.Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }
      return new IntersectionResult();
    }

    /// <summary>
    /// Tells if two lines overlap (they share a common section)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlap(other, decimal_precision).ValueType != typeof(NullValue);

    /// <summary>
    /// If two lines overlap, this function returns the shared section between them
    /// The returned line segment follows the same direction as this segment
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Overlap(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!IsParallel(other, decimal_precision)) {
        return new IntersectionResult();
      }
      (bool other_p0_in, bool other_p1_in) =
          (Contains(other.P0, decimal_precision), Contains(other.P1, decimal_precision));
      (bool p0_in, bool p1_in) = (other.Contains(P0, decimal_precision), other.Contains(P1, decimal_precision));

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
  }
}
