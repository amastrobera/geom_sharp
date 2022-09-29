using System;
using System.Linq;

namespace GeomSharp {
  /// <summary>
  /// A Geometrical Vector of two coordinates (X,Y,Z) on an arbitrary 3D plane
  /// </summary>
  public class Vector3D : IEquatable<Vector3D> {
    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public static Vector3D Zero => new Vector3D(0, 0, 0);
    public static UnitVector3D AxisX => UnitVector3D.FromDoubles(1, 0, 0);
    public static UnitVector3D AxisY => UnitVector3D.FromDoubles(0, 1, 0);
    public static UnitVector3D AxisZ => UnitVector3D.FromDoubles(0, 0, 1);

    public Vector3D(double x = 0, double y = 0, double z = 0) => (X, Y, Z) = (x, y, z);

    public Vector3D(Vector3D copy) => (X, Y, Z) = (copy.X, copy.Y, copy.Z);

    private Vector3D(Vector copy_raw) {
      if (copy_raw.Size != 3) {
        throw new ArgumentException(
            String.Format("tried to initialize a Point3D with an {0:D}-dimention vector", copy_raw.Size));
      }
      X = copy_raw[0];  // X = Math.Round(copy_raw[0], Constants.NINE_DECIMALS);
      Y = copy_raw[1];  // Y = Math.Round(copy_raw[1], Constants.NINE_DECIMALS);
      Z = copy_raw[2];  // Z = Math.Round(copy_raw[2], Constants.NINE_DECIMALS);
    }

    // unary operations

    public static Vector3D FromVector(Vector v) {
      return new Vector3D(v);
    }

    public double[] ToArray() => ToVector().ToArray();

    public Vector ToVector() => Vector.FromArray(new double[] { X, Y, Z });

    public double Length() => ToVector().Length();

    public UnitVector3D Normalize() {
      double norm = Length();

      if (norm == 0) {
        throw new Exception("cannot normalize a zero length vector");
      }
      // Console.WriteLine(String.Format("this={0}, norm={1:F4}", this.ToWkt(), norm));

      // return UnitVector3D.FromVector(this / norm);
      return UnitVector3D.FromDoubles(this.X / norm, this.Y / norm, this.Z / norm);
    }

    public bool SameDirectionAs(Vector3D other) =>
        IsParallel(other) &&
        Math.Sign(Math.Round(X, Constants.THREE_DECIMALS)) == Math.Sign(Math.Round(other.X,
                                                                                   Constants.THREE_DECIMALS)) &&
        Math.Sign(Math.Round(Y, Constants.THREE_DECIMALS)) == Math.Sign(Math.Round(other.Y,
                                                                                   Constants.THREE_DECIMALS)) &&
        Math.Sign(Math.Round(Z, Constants.THREE_DECIMALS)) == Math.Sign(Math.Round(other.Z, Constants.THREE_DECIMALS));

    public bool OppositeDirectionAs(Vector3D other) =>
        IsParallel(other) &&
        Math.Sign(Math.Round(X, Constants.THREE_DECIMALS)) != Math.Sign(Math.Round(other.X,
                                                                                   Constants.THREE_DECIMALS)) &&
        Math.Sign(Math.Round(Y, Constants.THREE_DECIMALS)) != Math.Sign(Math.Round(other.Y,
                                                                                   Constants.THREE_DECIMALS)) &&
        Math.Sign(Math.Round(Z, Constants.THREE_DECIMALS)) != Math.Sign(Math.Round(other.Z, Constants.THREE_DECIMALS));

    /// <summary>
    /// Equality check with custom tolerance adjustment
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool AlmostEquals(Vector3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Math.Round(this.X - other.X, decimal_precision) == 0 && Math.Round(this.Y - other.Y, decimal_precision) == 0 &&
        Math.Round(this.Z - other.Z, decimal_precision) == 0;

    public bool Equals(Vector3D other) => Math.Round(this.X - other.X, Constants.NINE_DECIMALS) == 0 &&
                                          Math.Round(this.Y - other.Y, Constants.NINE_DECIMALS) == 0 &&
                                          Math.Round(this.Z - other.Z, Constants.NINE_DECIMALS) == 0;

    public override bool Equals(object other) => other != null && other is Vector3D && this.Equals((Vector3D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Vector3D a, Vector3D b) {
      return a.Equals(b);
    }

    public static bool operator !=(Vector3D a, Vector3D b) {
      return !a.Equals(b);
    }

    // arithmetics with Vectors
    public static Point3D operator +(Vector3D vec, Point3D a) =>
        Point3D.FromVector(a.ToVector() + vec.ToVector());  // same operation, commutative to the other I wrote

    public static Vector3D operator +(Vector3D a, Vector3D b) => FromVector(a.ToVector() + b.ToVector());

    public static Vector3D operator -(Vector3D a, Vector3D b) => FromVector(a.ToVector() - b.ToVector());

    public static Vector3D operator -(Vector3D a) => FromVector(-(a.ToVector()));

    public static double operator*(Vector3D a, Vector3D b) => a.DotProduct(b);

    public static double operator*(Vector3D a, Point3D b) => a.DotProduct(b);

    public static Vector3D operator*(Vector3D b, double k) => FromVector(b.ToVector() * k);

    public static Vector3D operator*(double k, Vector3D b) => FromVector(k * b.ToVector());

    public static Vector3D operator /(Vector3D b, double k) => FromVector(b.ToVector() / k);

    public double DotProduct(Vector3D other) => ToVector().DotProduct(other.ToVector());

    public double DotProduct(Point3D point) => ToVector().DotProduct(point.ToVector());

    /// <summary>
    /// The cross product (aka 3D outer product)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public Vector3D CrossProduct(Vector3D other) {
      return new Vector3D(this.Y * other.Z - this.Z * other.Y,
                          this.Z * other.X - this.X * other.Z,
                          this.X * other.Y - this.Y * other.X);
    }

    public UnitVector3D PerpOnPlane(UnitVector3D plane_normal) => (!plane_normal.IsParallel(this))
                                                                      ? plane_normal.CrossProduct(this).Normalize()
                                                                      : null;

    public bool IsPerpendicular(Vector3D b) => Math.Round(DotProduct(b), Constants.NINE_DECIMALS) == 0;

    public bool IsParallel(Vector3D b) {
      var u = Normalize();
      var v = b.Normalize();
      if (u.AlmostEquals(v) || u.AlmostEquals(-v)) {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Angle spanned between two vectors
    /// cos(theta) = V * W / (|V|*|W|) = v*w (unit vectors)
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public Angle AngleTo(Vector3D b) => Angle.FromRadians(Math.Acos(DotProduct(b) / (Length() * b.Length())));

    // special formatting functions

    public override string ToString() => "{" + String.Format("{0:F3} {1:F3} {2:F3}", X, Y, Z) + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return string.Format(
          "VECTOR (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}") + ")",
          X,
          Y,
          Z);
    }
  }

  /// <summary>
  /// A Vector of two coordinates (U,V) on an arbitrary 3D plane, which norm is 1
  /// </summary>
  public class UnitVector3D : Vector3D {
    private UnitVector3D(double x, double y, double z) : base(x, y, z) {}

    private UnitVector3D(Vector3D v) : base(v) {}

    public static UnitVector3D FromDoubles(double x, double y, double z) {
      if (Math.Round(new Vector3D(x, y, z).Length(), Constants.NINE_DECIMALS) != 1) {
        throw new ArgumentException(
            String.Format("initialized UnitVector3D with non unit coordinates ({0:F9}, {1:F9}, {2:F9})", x, y, z));
      }
      return new UnitVector3D(x, y, z);
    }

    public static UnitVector3D FromVector(Vector3D v) =>
        (Math.Round(v.Length(), Constants.NINE_DECIMALS) != 1)
            ? throw new ArgumentException(String.Format("initialized UnitVector3D with non unit coordinates ({0})",
                                                        v.ToWkt()))
            : new UnitVector3D(v);

    public static UnitVector3D operator -(UnitVector3D a) => FromDoubles(-a.X, -a.Y, -a.Z);
  }
}
