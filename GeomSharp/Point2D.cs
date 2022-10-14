using System;
using System.Linq;

namespace GeomSharp {
  /// <summary>
  /// A Point of two coordinates (U,V) on an arbitrary 2D plane
  /// </summary>
  public class Point2D : IEquatable<Point2D> {
    public double U { get; }
    public double V { get; }

    public static Point2D Zero => new Point2D(0, 0);

    public Point2D(double u = 0, double v = 0) => (U, V) = (u, v);

    public Point2D(Point2D copy) => (U, V) = (copy.U, copy.V);

    public Point2D(Vector copy_raw) {
      if (copy_raw.Size != 2) {
        throw new ArgumentException(
            String.Format("tried to initialize a Point2D with an {0:D}-dimention vector", copy_raw.Size));
      }
      U = Math.Round(copy_raw[0], Constants.NINE_DECIMALS);
      V = Math.Round(copy_raw[1], Constants.NINE_DECIMALS);
    }

    // unary operations
    public static Point2D FromVector(Vector v) {
      return new Point2D(v);
    }

    public double[] ToArray() => new double[] { U, V };

    public Vector ToVector() => Vector.FromArray(new double[] { U, V });

    public Vector2D ToVector2D() {
      return new Vector2D(U, V);
    }

    /// <summary>
    /// Equality check with custom tolerance adjustment
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool AlmostEquals(Point2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Math.Round(this.U - other.U, decimal_precision) == 0 && Math.Round(this.V - other.V, decimal_precision) == 0;

    public bool Equals(Point2D other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Point2D && this.Equals((Point2D)other);

    public override int GetHashCode() => ToWkt().GetHashCode();

    public static bool operator ==(Point2D a, Point2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Point2D a, Point2D b) {
      return !a.AlmostEquals(b);
    }

    // arithmetics with Points

    public static Point2D operator +(Point2D a, Vector2D vec) => FromVector(a.ToVector() + vec.ToVector());

    // Point + Point is invalid
    // public static Point2D operator +(Point2D a, Point2D b)

    public static Vector2D operator -(Point2D a, Point2D b) => Vector2D.FromVector(a.ToVector() - b.ToVector());

    // Point * Point is invalid
    // public static Point2D operator*(Point2D a, Point2D b)

    public static Point2D operator -(Point2D a, Vector2D vec) => FromVector(a.ToVector() - vec.ToVector());

    public static Point2D operator -(Point2D a) => FromVector(-(a.ToVector()));

    public static Point2D operator*(Point2D b, double k) => FromVector(b.ToVector() * k);

    public static Point2D operator*(double k, Point2D b) => FromVector(k * b.ToVector());

    public static Point2D operator /(Point2D b, double k) => FromVector(b.ToVector() / k);

    public double DistanceTo(Point2D p) => (p - this).Length();

    // special formatting functions

    public override string ToString() => "{" + String.Format("{0:F9} {1:F9}", U, V) + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        string.Format("POINT (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ")", U, V);
  }

}
