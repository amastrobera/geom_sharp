using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {
  /// <summary>
  /// A Point of two coordinates (X,Y,Z) on an arbitrary 3D plane
  /// </summary>
  public class Point3D : IEquatable<Point3D> {
    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public static Point3D Zero => new Point3D(0, 0, 0);

    public Point3D(double x = 0, double y = 0, double z = 0) => (X, Y, Z) = (x, y, z);

    public Point3D(Point3D copy) => (X, Y, Z) = (copy.X, copy.Y, copy.Z);

    public Point3D(Vector copy_raw) {
      if (copy_raw.Size != 3) {
        throw new ArgumentException(
            String.Format("tried to initialize a Point3D with an {0:D}-dimention vector", copy_raw.Size));
      }
      X = Math.Round(copy_raw[0], Constants.NINE_DECIMALS);
      Y = Math.Round(copy_raw[1], Constants.NINE_DECIMALS);
      Z = Math.Round(copy_raw[2], Constants.NINE_DECIMALS);
    }

    // unary operations
    public static Point3D FromVector(Vector v) {
      return new Point3D(v);
    }

    public double[] ToArray() => ToVector().ToArray();

    public Vector ToVector() => Vector.FromArray(new double[] { X, Y, Z });
    public Vector3D ToVector3D() {
      return new Vector3D(X, Y, Z);
    }

    /// <summary>
    /// Equality check with custom tolerance adjustment
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool AlmostEquals(Point3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Math.Round(this.X - other.X, decimal_precision) == 0 && Math.Round(this.Y - other.Y, decimal_precision) == 0 &&
        Math.Round(this.Z - other.Z, decimal_precision) == 0;

    public bool Equals(Point3D other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Point3D && this.Equals((Point3D)other);

    public override int GetHashCode() => ToWkt().GetHashCode();

    public static bool operator ==(Point3D a, Point3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Point3D a, Point3D b) {
      return !a.AlmostEquals(b);
    }

    // arithmetics with Points
    public static Point3D operator +(Point3D a, Vector3D vec) => FromVector(a.ToVector() + vec.ToVector());

    // Point + Point is invalid
    // public static Point3D operator +(Point3D a, Point3D b)

    public static Vector3D operator -(Point3D a, Point3D b) => Vector3D.FromVector(a.ToVector() - b.ToVector());

    // Point * Point is invalid
    // public static Point3D operator*(Point3D a, Point3D b)

    public static Point3D operator -(Point3D a, Vector3D vec) => FromVector(a.ToVector() - vec.ToVector());

    public static Point3D operator -(Point3D a) => FromVector(-(a.ToVector()));

    public static Point3D operator*(Point3D b, double k) => FromVector(b.ToVector() * k);

    public double DotProduct(Vector3D vec) => ToVector().DotProduct(vec.ToVector());

    public static double operator*(Point3D a, Vector3D b) => a.DotProduct(b);

    public static Point3D operator*(double k, Point3D b) => FromVector(k * b.ToVector());

    public static Point3D operator /(Point3D b, double k) => FromVector(b.ToVector() / k);

    public double DistanceTo(Point3D p) => (p - this).Length();

    // special formatting functions
    public override string ToString() => "{" + String.Format("{0:F9} {1:F9} {2:F9}", X, Y, Z) + "}";
    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return string.Format(
          "POINT (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}") + ")",
          X,
          Y,
          Z);
    }
  }

}
