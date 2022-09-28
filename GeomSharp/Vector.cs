using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace GeomSharp {
  /// <summary>
  /// A Geometrical Vector of two coordinates (U,V) on an arbitrary 2D plane
  /// </summary>
  public class Vector : IEquatable<Vector> {
    private double[] _values;
    public readonly int Size;

    public double this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return _values[i];
      }
      set {
        _values[i] = value;
      }
    }

    private Vector(uint size) {
      _values = new double[size];
      Size = (int)size;
      for (int i = 0; i < Size; i++) {
        _values[i] = 0;
      }
    }

    public Vector(Vector copy) => (U, V) = (copy.U, copy.V);

    protected Vector(Vector<double> copy_raw) {
      if (copy_raw.Count() != 2) {
        throw new ArgumentException(
            String.Format("tried to initialize a Vector with an {0:D}-dimention vector", copy_raw.Count()));
      }
      U = copy_raw[0];  // U = Math.Round(copy_raw[0], Constants.NINE_DECIMALS);
      V = copy_raw[1];  // V = Math.Round(copy_raw[1], Constants.NINE_DECIMALS);
    }

    // unary operations
    public static Vector FromVector(Vector<double> v) {
      return new Vector(v);
    }
    public double[] ToArray() => new double[] { U, V };

    public Vector<double> ToVector() => Vector<double>.Build.Dense(new double[] { U, V });

    // unary methods

    public double Length() => ToVector().L1Norm();  // weird rounding errors
    // public double Length() => Math.Sqrt(U * U + V * V);

    public UnitVector Normalize() {
      double norm = Length();

      if (norm == 0) {
        throw new Exception("cannot normalize a zero length vector");
      }

      // Console.WriteLine(String.Format("this={0}, norm={1:F4}", this.ToWkt(), norm));

      // return UnitVector.FromVector(this / norm);
      return UnitVector.FromDoubles(this.U / norm, this.V / norm);
    }

    public bool SameDirectionAs(Vector other) =>
        IsParallel(other) && Math.Sign(U) == Math.Sign(other.U) && Math.Sign(V) == Math.Sign(other.V);

    public bool OppositeDirectionAs(Vector other) =>
        IsParallel(other) && Math.Sign(U) != Math.Sign(other.U) && Math.Sign(V) != Math.Sign(other.V);

    /// <summary>
    /// Equality check with custom tolerance adjustment
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool AlmostEquals(Vector other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Math.Round(this.U - other.U, decimal_precision) == 0 && Math.Round(this.V - other.V, decimal_precision) == 0;

    public bool Equals(Vector other) =>
        Math.Round(this.U - other.U, Constants.NINE_DECIMALS) == 0 && Math.Round(this.V - other.V,
                                                                                 Constants.NINE_DECIMALS) == 0;

    public override bool Equals(object other) => other != null && other is Vector && this.Equals((Vector)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Vector a, Vector b) {
      return a.Equals(b);
    }

    public static bool operator !=(Vector a, Vector b) {
      return !a.Equals(b);
    }

    // binary operations

    // arithmetics with Vectors
    public static Point2D operator +(Vector vec, Point2D a) =>
        Point2D.FromVector(a.ToVector() + vec.ToVector());  // same operation, commutative to the other I wrote

    public static Vector operator +(Vector a, Vector b) => FromVector(a.ToVector() + b.ToVector());

    public static Vector operator -(Vector a, Vector b) => FromVector(a.ToVector() - b.ToVector());

    public static Vector operator -(Vector a) => FromVector(-(a.ToVector()));

    public static double operator*(Vector a, Vector b) => a.DotProduct(b);

    public static Vector operator*(Vector b, double k) => FromVector(b.ToVector() * k);

    public static Vector operator*(double k, Vector b) => FromVector(k * b.ToVector());

    public static Vector operator /(Vector b, double k) => FromVector(b.ToVector() / k);

    public double DotProduct(Vector other) => ToVector().DotProduct(other.ToVector());

    /// <summary>
    /// Returns the perpendicular vector 2D to this one
    /// </summary>
    /// <returns></returns>
    public Vector Perp() {
      return new Vector(-V, U);
    }

    /// <summary>
    /// The perpendicular product 2D (aka 2D outer product)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public double PerpProduct(Vector other) => Perp().DotProduct(other);

    public double CrossProduct(Vector other) => PerpProduct(other);

    public bool IsPerpendicular(Vector b) => Math.Round(DotProduct(b), Constants.THREE_DECIMALS) == 0;

    public bool IsParallel(Vector b) => Math.Round(PerpProduct(b), Constants.THREE_DECIMALS) == 0;

    /// <summary>
    /// Angle spanned between two vectors
    /// cos(theta) = V * W / (|V|*|W|) = v*w (unit vectors)
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public Angle AngleTo(Vector b) => Angle.FromRadians(Math.Acos(DotProduct(b) / (Length() * b.Length())));

    // special formatting
    public override string ToString() => "{" + String.Format("{0:F9} {1:F9}", U, V) + "}";
    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return string.Format("VECTOR (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ")",
                           U,
                           V);
    }
  }

  /// <summary>
  /// A Vector of two coordinates (U,V) on an arbitrary 2D plane, which norm is 1
  /// </summary>
  public class UnitVector : Vector {
    private UnitVector(double u, double v) : base(u, v) {}

    private UnitVector(Vector v) : base(v) {}

    public static UnitVector FromDoubles(double u, double v) {
      if (Math.Round(new Vector(u, v).Length(), Constants.NINE_DECIMALS) != 1) {
        throw new ArgumentException(
            String.Format("initialized UnitVector with non unit coordinates ({0:F9}, {1:F9}", u, v));
      }
      return new UnitVector(u, v);
    }

    public static UnitVector FromVector(Vector v) =>
        (Math.Round(v.Length(), Constants.NINE_DECIMALS) != 1)
            ? throw new ArgumentException(String.Format("initialized UnitVector3D with non unit coordinates ({0})",
                                                        v.ToWkt()))
            : new UnitVector(v);

    public static UnitVector operator -(UnitVector a) => FromDoubles(-a.U, -a.V);
  }
}
