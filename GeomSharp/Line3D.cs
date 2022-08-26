using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Numerics.LinearAlgebra;

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

    public bool Contains(Point3D p) => (p - P0).IsParallel(Direction);

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
    /// If intersection, we will find (s,t) such that:
    ///     Origin(1) + s*dir(1) = P(s|t) = Origin(2) + t*dir(2)
    /// I formalize it in the form Ax = b, then use MathNet.Numerics to solve it
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Intersection(Line3D other) {
      if (IsParallel(other)) {
        return new IntersectionResult();
      }

      (var O1, var O2) = (Origin, other.Origin);
      (var dir1, var dir2) = (Direction, other.Direction);

      var A = Matrix<double>.Build.DenseOfRowArrays(new double[][] { new double[] { dir1.X, -dir2.X },
                                                                     new double[] { dir1.Y, -dir2.Y },
                                                                     new double[] { dir1.Z, -dir2.Z } });
      var b = (O2 - O1).ToVector();

      try {
        var x = A.Solve(b);
        if (x is null || x.Count == 0) {
          throw new Exception("empty solution to Intersection");
        }
        (double s, double t) = (x[0], x[1]);

        (var p1, var p2) = (O1 + s * dir1, O2 + t * dir2);
        if (!p1.AlmostEquals(p2)) {
          throw new Exception(String.Format("intersection does not match {0} to {1}", p1.ToWkt(), p2.ToWkt()));
        }

        return new IntersectionResult(p1);

      } catch (Exception ex) {
        Console.WriteLine("Intersection: " + ex.Message);
      }

      return new IntersectionResult();
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
    public string ToWkt(int precision = Constants.NINE_DECIMALS) {
      return "LINESTRING (" +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                           P0.X,
                           P0.Y,
                           P0.Z) +
             "," +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                           P1.X,
                           P1.Y,
                           P1.Z);
    }
  }

}
