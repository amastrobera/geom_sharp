﻿using System;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using GeomSharp.Algebra;

namespace GeomSharp {
  /// <summary>
  /// A Geometrical Vector of two coordinates (U,V) on an arbitrary 2D plane
  /// </summary>
  [Serializable]
  public class Vector2D : IEquatable<Vector2D>, ISerializable {
    public double U { get; }
    public double V { get; }

    public static Vector2D Zero => new Vector2D(0, 0);
    public static UnitVector2D AxisU => UnitVector2D.FromDoubles(1, 0);
    public static UnitVector2D AxisV => UnitVector2D.FromDoubles(0, 1);

    public Vector2D(double u = 0, double v = 0) => (U, V) = (u, v);

    public Vector2D(Vector2D copy) => (U, V) = (copy.U, copy.V);

    protected Vector2D(Vector copy_raw) {
      if (copy_raw.Size != 2) {
        throw new ArgumentException(
            String.Format("tried to initialize a Vector2D with an {0:D}-dimention vector", copy_raw.Size));
      }
      U = copy_raw[0];  // U = Math.Round(copy_raw[0], Constants.NINE_DECIMALS);
      V = copy_raw[1];  // V = Math.Round(copy_raw[1], Constants.NINE_DECIMALS);
    }

    // unary operations
    public static Vector2D FromVector(Vector v) {
      return new Vector2D(v);
    }
    public double[] ToArray() => ToVector().ToArray();

    public Vector ToVector() => new Vector(new double[] { U, V });

    // unary methods

    public double Length() => ToVector().Length();

    public UnitVector2D Normalize() {
      double norm = Length();

      if (norm == 0) {
        throw new Exception("cannot normalize a zero length vector");
      }

      // Console.WriteLine(String.Format("this={0}, norm={1:F4}", this.ToWkt(), norm));

      // return UnitVector2D.FromVector(this / norm);
      return UnitVector2D.FromDoubles(this.U / norm, this.V / norm);
    }

    public bool SameDirectionAs(Vector2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        IsParallel(other, decimal_precision) &&
        Math.Sign(Math.Round(U, decimal_precision)) == Math.Sign(Math.Round(other.U, decimal_precision)) &&
        Math.Sign(Math.Round(V, decimal_precision)) ==
            Math.Sign(Math.Round(other.V,
                                 decimal_precision));  // comparing the signs instead of the product avoids overflow

    public bool OppositeDirectionAs(Vector2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        IsParallel(other, decimal_precision) &&
        Math.Sign(Math.Round(U, decimal_precision)) != Math.Sign(Math.Round(other.U, decimal_precision)) &&
        Math.Sign(Math.Round(V, decimal_precision)) !=
            Math.Sign(Math.Round(other.V,
                                 decimal_precision));  // comparing the signs instead of the product avoids overflow

    /// <summary>
    /// Equality check with custom tolerance adjustment
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool AlmostEquals(Vector2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) && Math.Round(this.U - other.U, decimal_precision) == 0 &&
        Math.Round(this.V - other.V, decimal_precision) == 0;

    public bool Equals(Vector2D other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Vector2D && this.Equals((Vector2D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Vector2D a, Vector2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Vector2D a, Vector2D b) {
      return !a.AlmostEquals(b);
    }

    // binary operations

    // arithmetics with Vectors
    public static Point2D operator +(Vector2D vec, Point2D a) =>
        Point2D.FromVector(a.ToVector() + vec.ToVector());  // same operation, commutative to the other I wrote

    public static Vector2D operator +(Vector2D a, Vector2D b) => FromVector(a.ToVector() + b.ToVector());

    public static Vector2D operator -(Vector2D a, Vector2D b) => FromVector(a.ToVector() - b.ToVector());

    public static Vector2D operator -(Vector2D a) => FromVector(-(a.ToVector()));

    public static double operator*(Vector2D a, Vector2D b) => a.DotProduct(b);

    public static Vector2D operator*(Vector2D b, double k) => FromVector(b.ToVector() * k);

    public static Vector2D operator*(double k, Vector2D b) => FromVector(k * b.ToVector());

    public static Vector2D operator /(Vector2D b, double k) => FromVector(b.ToVector() / k);

    public double DotProduct(Vector2D other) => ToVector().DotProduct(other.ToVector());

    /// <summary>
    /// Returns the perpendicular vector 2D to this one
    /// </summary>
    /// <returns></returns>
    public Vector2D Perp() {
      return new Vector2D(-V, U);
    }

    /// <summary>
    /// The perpendicular product 2D (aka 2D outer product)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public double PerpProduct(Vector2D other) => Perp().DotProduct(other);

    public double CrossProduct(Vector2D other) => PerpProduct(other);

    public bool IsPerpendicular(Vector2D b,
                                int decimal_precision = Constants.THREE_DECIMALS) => Math.Round(DotProduct(b),
                                                                                                decimal_precision) == 0;

    public bool IsParallel(Vector2D b,
                           int decimal_precision = Constants.THREE_DECIMALS) => Math.Round(PerpProduct(b),
                                                                                           decimal_precision) == 0;

    /// <summary>
    /// Angle spanned between two vectors
    /// cos(theta) = V * W / (|V|*|W|) = v*w (unit vectors)
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public Angle AngleTo(Vector2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      double acos = Math.Acos(DotProduct(other) / (Length() * other.Length()));  // returns 0 <= acos < Math.PI
      // handle all cases
      double dy = other.V - V;
      return Angle.FromRadians(Math.Round(dy, decimal_precision) >= 0 ? acos : 2 * Math.PI - acos);
    }

    // special formatting
    public override string ToString() => "{" + String.Format("{0:F9} {1:F9}", U, V) + "}";
    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return string.Format("VECTOR (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ")",
                           U,
                           V);
    }

    // serialization functions
    // Implement this method to serialize data. The method is called on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("U", U, typeof(double));
      info.AddValue("V", V, typeof(double));
    }

    public Vector2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      U = (double)info.GetValue("U", typeof(double));
      V = (double)info.GetValue("V", typeof(double));
    }

    public static Vector2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Vector2D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    public void ToBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Create);
        (new BinaryFormatter()).Serialize(fs, this);
        fs.Close();
      } catch (Exception e) {
        // warning failed to deserialize
      }
    }
  }

  /// <summary>
  /// A Vector of two coordinates (U,V) on an arbitrary 2D plane, which norm is 1
  /// </summary>
  public class UnitVector2D : Vector2D {
    private UnitVector2D(double u, double v) : base(u, v) {}

    private UnitVector2D(Vector2D v) : base(v) {}

    public static UnitVector2D FromDoubles(double u, double v) {
      if (Math.Round(new Vector2D(u, v).Length(), Constants.NINE_DECIMALS) != 1) {
        throw new ArgumentException(
            String.Format("initialized UnitVector2D with non unit coordinates ({0:F9}, {1:F9}", u, v));
      }
      return new UnitVector2D(u, v);
    }

    public static UnitVector2D FromVector(Vector2D v) =>
        (Math.Round(v.Length(), Constants.NINE_DECIMALS) != 1)
            ? throw new ArgumentException(String.Format("initialized UnitVector3D with non unit coordinates ({0})",
                                                        v.ToWkt()))
            : new UnitVector2D(v);

    public static UnitVector2D operator -(UnitVector2D a) => FromDoubles(-a.U, -a.V);

    public UnitVector2D(SerializationInfo info, StreamingContext context) => FromVector(new Vector2D(info, context));

    // clang-format off
    public static new UnitVector2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (UnitVector2D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }
// clang-format on
}
}
