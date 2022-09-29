using System;

namespace GeomSharp {
  /// <summary>
  /// A Line on an arbitrary 3D plane
  /// It's either defined as an infinite straight line passing by two points
  /// or an infinite straight line passing by a point, with a given direction
  /// </summary>
  public class Line3D : IEquatable<Line3D> {
    public Point3D P0 { get; }
    public Point3D P1 { get; }
    public Point3D Origin { get; }
    public UnitVector3D Direction { get; }

    public static Line3D FromPoints(Point3D p0, Point3D p1) =>
        (p0 == p1) ? throw new NullLengthException("trying to initialize a line with two identical points")
                   : new Line3D(p0, p1);

    public static Line3D FromDirection(Point3D orig, UnitVector3D dir) => new Line3D(orig, dir);

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

    public bool Equals(Line3D other) => Direction.Equals(other.Direction);

    public override bool Equals(object other) => other != null && other is Line3D && this.Equals((Line3D)other);

    public override int GetHashCode() => Direction.ToWkt().GetHashCode();

    public static bool operator ==(Line3D a, Line3D b) {
      return a.Equals(b);
    }

    public static bool operator !=(Line3D a, Line3D b) {
      return !a.Equals(b);
    }

    public bool IsParallel(Line3D other) => Direction.IsParallel(other.Direction);

    public bool IsPerpendicular(Line3D other) => Direction.IsPerpendicular(other.Direction);

    public bool Contains(Point3D p) => p.AlmostEquals(Origin) || (p - Origin).IsParallel(Direction);

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
    public double DistanceTo(Point3D p) => Math.Round(Direction.CrossProduct(p - Origin).Length(),
                                                      Constants.NINE_DECIMALS);

    public bool Intersects(Line3D other) => Intersection(other).ValueType != typeof(NullValue);

    /// <summary>
    /// Finds the intersection between two 3D Lines. It solves a linear system of 3 equations with 2 unknowns.
    /// L1: Origin(1) + dir(1)   // this line
    /// L2: Origin(2) + dir(2)   // other line
    /// I project the two lines on the first plane onto which they aren't perpendicular. Compute the intersection over
    /// there, in 2D, then I test whether this intersection holds in 3D, and if so, return it. I formalize it in the
    /// Sufficient condition for 3D intersection is 2D intersection. If something does not intersect on (any of the
    /// three) 2D planes, it doesn't intersect in 3D either. If something intersects in any 2D plane, then we must
    /// verify the intersection point belongs to both lines involved.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Intersection(Line3D other) {
      if (IsParallel(other)) {
        return new IntersectionResult();
      }

      // pick the first plane 2D where both lines can be projected as lines and not as dots
      // (verify that neither line is perpendicular to the projecting plane)
      Plane plane_2d =
          !(this.IsPerpendicular(Plane.XY) || other.IsPerpendicular(Plane.XY))
              ? Plane.XY
              : (!(this.IsPerpendicular(Plane.YZ) || other.IsPerpendicular(Plane.YZ)) ? Plane.YZ : Plane.ZX);

      (var p1, var other_p1) = (plane_2d.ProjectInto(Origin), plane_2d.ProjectInto(other.Origin));
      (var p2, var other_p2) =
          (plane_2d.ProjectInto(Origin + 2 * Direction), plane_2d.ProjectInto(other.Origin + 2 * other.Direction));
      (var line_2d, var other_line_2d) = (Line2D.FromTwoPoints(p1, p2), Line2D.FromTwoPoints(other_p1, other_p2));

      var inter_res = line_2d.Intersection(other_line_2d);
      if (inter_res.ValueType == typeof(NullValue)) {
        // no 2D intersection, no 3D intersection either
        return new IntersectionResult();
      }
      // there is a 2D intersection, let's get the respective 3D point
      var pI_2d = (Point2D)inter_res.Value;

      // given the linear relationship between the 3D line and the projected 2D line, we can find the point on the 3D
      // line with a length ratio
      var A = Origin;
      var B = Origin + 2 * Direction;
      var a = plane_2d.ProjectInto(A);
      var b = plane_2d.ProjectInto(B);
      var len_ratio = (pI_2d - a).Length() / (b - a).Length();
      Point3D pI = A + len_ratio * (B - A);

      // and verify it belongs to both lines
      if (!(Contains(pI) && other.Contains(pI))) {
        // this is merely a 2D intersection, 3D lines do not intersect
        return new IntersectionResult();
      }

      // all is well, yea yea
      return new IntersectionResult(pI);
    }

    /// <summary>
    /// Tells if two lines overlap (they share a common section)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Line3D other) {
      if (!IsParallel(other)) {
        return false;
      }
      if (Contains(other.P0) || Contains(other.P1)) {
        return true;
      }
      return false;
    }

    /// <summary>
    /// If two lines overlap, this function returns the shared section between them
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Overlap(Line3D other) => Overlaps(other) ? new IntersectionResult(this)
                                                                       : new IntersectionResult();

    // text / deugging
    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      (var p1, var p2) = (Origin - 2 * Direction, Origin + 2 * Direction);
      return "GEOMETRYCOLLECTION (" +

             "POINT (" +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                           Origin.X,
                           Origin.Y,
                           Origin.Z) +
             ")"

             + "," +

             "LINESTRING (" +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                           p1.X,
                           p1.Y,
                           p1.Z) +
             "," +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                           p2.X,
                           p2.Y,
                           p2.Z) +
             ")" +

             ")";
    }
  }
}
