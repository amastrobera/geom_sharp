using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  public class Triangle2D : IEquatable<Triangle2D> {
    public Point2D P0 { get; }
    public Point2D P1 { get; }
    public Point2D P2 { get; }

    public Constants.Orientation Orientation { get; }
    public UnitVector2D U { get; }
    public UnitVector2D V { get; }

    private Triangle2D(Point2D p0, Point2D p1, Point2D p2) {
      P0 = p0;
      P1 = p1;
      P2 = p2;
      // unit vectors (normalized)
      U = (P1 - P0).Normalize();
      V = (P2 - P0).Normalize();

      Orientation = (U.PerpProduct(V) >= 0) ? Constants.Orientation.COUNTER_CLOCKWISE : Constants.Orientation.CLOCKWISE;
    }

    public static Triangle2D FromPoints(Point2D p0, Point2D p1, Point2D p2) {
      if ((p1 - p0).IsParallel(p2 - p0)) {
        throw new ArithmeticException("tried to construct a Triangle with collinear points");
      }

      var t = new Triangle2D(p0, p1, p2);

      if (t.Area() == 0) {
        throw new ArithmeticException("tried to construct a Triangle of nearly zero-area");
      }

      return t;
    }

    public double Area() => Math.Round((P1 - P0).CrossProduct(P2 - P0) / 2, Constants.NINE_DECIMALS);

    public Point2D CenterOfMass() => Point2D.FromVector((P0.ToVector() + P1.ToVector() + P2.ToVector()) / 3);

    /// <summary>
    /// Tells whether a point is inside a triangle T. This includes points on the borders, dT.
    /// Uses baricentric coordinate test
    /// Computes the solution with Ruffini's rule
    /// P = M * x
    /// | xp | = | x_p0 x_p1 x_p2 |   | a |
    /// | yp | = | y_p0 y_p1 y_p2 | * | b |
    /// | zp | = | z_p0 z_p1 z_p2 |   | c |
    /// Find (a,b,c)
    /// If all of (a,b,c) is in the range [0,1] the point belongs to the triangle
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool Contains(Point2D point) {
      var mtx = Matrix<double>.Build.DenseOfColumnArrays(new double[][] { new double[] { P0.U, P0.V, 1 },
                                                                          new double[] { P1.U, P1.V, 1 },
                                                                          new double[] { P2.U, P2.V, 1 } });

      double d = mtx.Determinant();

      if (Math.Round(d, Constants.NINE_DECIMALS) == 0) {
        return false;
      }

      var abc = mtx.Solve(Vector<double>.Build.Dense(
          new double[] { point.U, point.V, 1 }));  // Factorization based solution from Math.Net
      (double a, double b, double c) =
          (Math.Round(abc[0], Constants.NINE_DECIMALS),
           Math.Round(abc[1], Constants.NINE_DECIMALS),
           Math.Round(abc[2], Constants.NINE_DECIMALS));  // rounding issue will kill you if not checked

      return a >= 0 && a <= 1 && b >= 0 && b <= 1 && c >= 0 && c <= 1;
    }

    public bool Equals(Triangle2D other) {
      if (!Orientation.Equals(other.Orientation)) {
        return false;
      }
      var point_list = new List<Point2D>() { P0, P1, P2 };
      if (!point_list.Contains(other.P0)) {
        return false;
      }
      if (!point_list.Contains(other.P1)) {
        return false;
      }
      if (!point_list.Contains(other.P2)) {
        return false;
      }
      // no check on point order (CCW or CW) is needed, since the constructor guarantees the Facing to be contructed
      // by points, and therefore incorporates this information
      return true;
    }

    public override bool Equals(object other) => other != null && other is Triangle2D && this.Equals((Triangle2D)other);

    public override int GetHashCode() => new { P0, P1, P2, Orientation }.GetHashCode();

    public static bool operator ==(Triangle2D a, Triangle2D b) {
      return a.Equals(b);
    }

    public static bool operator !=(Triangle2D a, Triangle2D b) {
      return !a.Equals(b);
    }

    public bool Intersects(Triangle2D other) => false;

    public IntersectionResult Intersection(Triangle2D other) {
      return new IntersectionResult();
    }

    public bool Overlaps(Triangle2D other) => Overlap(other).ValueType != typeof(NullValue);

    public IntersectionResult Overlap(Triangle2D other) {
      return new IntersectionResult();
    }

    public bool IsAdjacent(Triangle2D other) => AdjacentSide(other).ValueType != typeof(NullValue);

    public IntersectionResult AdjacentSide(Triangle2D other) {
      return new IntersectionResult();
    }

    // special formatting
    public override string ToString() {
      return "{" + P0.ToString() + ", " + P1.ToString() + ", " + P2.ToString() + "}";
    }

    public string ToWkt() {
      return string.Format("POLYGON (({0:F2} {1:F2}, {2:F2} {3:F2},{4:F2} {5:F2},{0:F2} {1:F2}))",
                           P0.U,
                           P0.V,
                           P1.U,
                           P1.V,
                           P2.U,
                           P2.V);
    }
  }
}
