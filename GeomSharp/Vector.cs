using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {
  /// <summary>
  /// A Mathematical Vector containing any number of double
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

    // public Vector(Vector copy) => (U, V) = (copy.U, copy.V);

    // protected Vector(Vector<double> copy_raw) {
    //   if (copy_raw.Count() != 2) {
    //     throw new ArgumentException(
    //         String.Format("tried to initialize a Vector with an {0:D}-dimention vector", copy_raw.Count()));
    //   }
    //   U = copy_raw[0];  // U = Math.Round(copy_raw[0], Constants.NINE_DECIMALS);
    //   V = copy_raw[1];  // V = Math.Round(copy_raw[1], Constants.NINE_DECIMALS);
    // }

    //// unary operations
    // public static Vector FromVector(Vector<double> v) {
    //   return new Vector(v);
    // }
    // public double[] ToArray() => new double[] { U, V };

    // public Vector<double> ToVector() => Vector<double>.Build.Dense(new double[] { U, V });

    // unary methods

    public double Length() {
      double ssq = 0;
      for (int i = 0; i < Size; i++) {
        ssq += Math.Pow(_values[i], 2);
      }
      return Math.Sqrt(ssq);
    }

    /// <summary>
    /// Equality check with custom tolerance adjustment
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool AlmostEquals(Vector other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (Size != other.Size) {
        return false;
      }

      for (int i = 0; i < Size; ++i) {
        if (Math.Round(_values[i] - other._values[i], decimal_precision) != 0) {
          return false;
        }
      }
      return true;
    }

    public bool Equals(Vector other) => AlmostEquals(other);

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

    //// special formatting
    // public override string ToString() => "{" + String.Format("{0:F9} {1:F9}", U, V) + "}";
    // public string ToWkt(int precision = Constants.THREE_DECIMALS) {
    //   return string.Format("VECTOR (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ")",
    //                        U,
    //                        V);
    // }
  }

}
