using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

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

    public Vector Clone() => new Vector(this);

    public static Vector FromArray(double[] arr) => FromEnumerable(arr);

    public static Vector FromList(List<double> ldoubles) => FromEnumerable(ldoubles);

    public double[] ToArray() => _values;

    public List<double> ToList() => _values.ToList();

    private Vector(int size) {
      if (size <= 0) {
        throw new ArgumentException("init vector with size <= 0");
      }
      _values = new double[size];
      Size = size;
      for (int i = 0; i < Size; i++) {
        _values[i] = 0;
      }
    }

    private Vector(Vector cp) {
      Size = cp.Size;
      for (int i = 0; i < Size; i++) {
        _values[i] = cp._values[i];
      }
    }

    private static Vector FromEnumerable(IEnumerable<double> enumd) {
      var v = new Vector(enumd.Count());
      int i = 0;
      foreach (var val in enumd) {
        v._values[i] = val;
        ++i;
      }
      return v;
    }

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
    public static Vector operator +(Vector a, Vector b) {
      if (a.Size != b.Size) {
        throw new ArgumentException("operator+ on vectors of different size");
      }

      var v = new Vector(a.Size);
      for (int i = 0; i < v.Size; ++i) {
        v._values[i] = a._values[i] + b._values[i];
      }
      return v;
    }

    public static Vector operator +(Vector a, double k) {
      var v = new Vector(a.Size);
      for (int i = 0; i < v.Size; ++i) {
        v._values[i] = a._values[i] + k;
      }
      return v;
    }

    public static Vector operator +(double k, Vector b) => b + k;

    public static Vector operator -(Vector a, double k) {
      var v = new Vector(a.Size);
      for (int i = 0; i < v.Size; ++i) {
        v._values[i] = a._values[i] - k;
      }
      return v;
    }

    public static Vector operator -(double k, Vector b) => b - k;

    public static Vector operator -(Vector a, Vector b) {
      if (a.Size != b.Size) {
        throw new ArgumentException("operator- on vectors of different size");
      }

      var v = new Vector(a.Size);
      for (int i = 0; i < v.Size; ++i) {
        v._values[i] = a._values[i] - b._values[i];
      }
      return v;
    }

    public static Vector operator -(Vector a) {
      var v = new Vector(a.Size);
      for (int i = 0; i < v.Size; ++i) {
        v._values[i] = -a._values[i];
      }
      return v;
    }

    public static Vector operator*(Vector a, Vector b) {
      if (a.Size != b.Size) {
        throw new ArgumentException("operator* on vectors of different size");
      }

      var v = new Vector(a.Size);
      for (int i = 0; i < v.Size; ++i) {
        v._values[i] = a._values[i] * b._values[i];
      }
      return v;
    }

    public static Vector operator*(Vector a, double k) {
      var v = new Vector(a.Size);
      for (int i = 0; i < v.Size; ++i) {
        v._values[i] = a._values[i] * k;
      }
      return v;
    }

    public static Vector operator*(double k, Vector b) => b * k;

    public static Vector operator /(Vector a, double k) {
      var v = new Vector(a.Size);
      int i = 0;
      try {
        for (; i < v.Size; ++i) {
          v._values[i] = a._values[i] / k;
        }
      } catch (DivideByZeroException) {
        throw new ArithmeticException("operator/ division by zero on index " + i.ToString());
      } catch (OverflowException) {
        throw new ArithmeticException("operator/ overflow on index " + i.ToString());
      }
      return v;
    }

    public double DotProduct(Vector other) {
      if (Size != other.Size) {
        throw new ArgumentException("DotProduct on vectors of different size");
      }

      double d = 0;
      for (int i = 0; i < other.Size; ++i) {
        d += _values[i] * other._values[i];
      }
      return d;
    }

    // special formatting
    public override string ToString() => ToWkt();
    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return "VECTOR (" +
             string.Join(",",
                         _values.Select(v => string.Format(String.Format("{0}0:F{1:D}{2}", "{", precision, "}"), v))) +
             ")";
    }
  }

}
