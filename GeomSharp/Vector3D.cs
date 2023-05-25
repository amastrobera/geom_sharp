using System;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using GeomSharp.Algebra;
using Microsoft.SqlServer.Server;

namespace GeomSharp {
  /// <summary>
  /// A Geometrical Vector of two coordinates (X,Y,Z) on an arbitrary 3D plane
  /// </summary>
  [Serializable]
  public class Vector3D : IEquatable<Vector3D>, ISerializable {
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

    public Vector ToVector() => new Vector(new double[] { X, Y, Z });

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

    public bool SameDirectionAs(Vector3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        IsParallel(other, decimal_precision) &&
        Math.Sign(Math.Round(X, decimal_precision)) == Math.Sign(Math.Round(other.X, decimal_precision)) &&
        Math.Sign(Math.Round(Y, decimal_precision)) == Math.Sign(Math.Round(other.Y, decimal_precision)) &&
        Math.Sign(Math.Round(Z, decimal_precision)) ==
            Math.Sign(Math.Round(
                other.Z, decimal_precision));  // comparing the sign of each instead of their product avoids overflow

    public bool OppositeDirectionAs(Vector3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        IsParallel(other, decimal_precision) &&
        Math.Sign(Math.Round(X, decimal_precision)) != Math.Sign(Math.Round(other.X, decimal_precision)) &&
        Math.Sign(Math.Round(Y, decimal_precision)) != Math.Sign(Math.Round(other.Y, decimal_precision)) &&
        Math.Sign(Math.Round(Z, decimal_precision)) !=
            Math.Sign(Math.Round(
                other.Z, decimal_precision));  // comparing the sign of each instead of their product avoids overflow

    /// <summary>
    /// Equality check with custom tolerance adjustment
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool AlmostEquals(Vector3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) && Math.Round(this.X - other.X, decimal_precision) == 0 &&
        Math.Round(this.Y - other.Y, decimal_precision) == 0 && Math.Round(this.Z - other.Z, decimal_precision) == 0;

    public bool Equals(Vector3D other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Vector3D && this.Equals((Vector3D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Vector3D a, Vector3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Vector3D a, Vector3D b) {
      return !a.AlmostEquals(b);
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

    public bool IsPerpendicular(Vector3D b,
                                int decimal_precision = Constants.THREE_DECIMALS) => Math.Round(DotProduct(b),
                                                                                                decimal_precision) == 0;

    public bool IsParallel(Vector3D b, int decimal_precision = Constants.THREE_DECIMALS) {
      var u = Normalize();
      var v = b.Normalize();
      if (u.AlmostEquals(v, decimal_precision) || u.AlmostEquals(-v, decimal_precision)) {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Angle spanned between two vectors
    /// cos(theta) = V * W / (|V|*|W|) = v*w (unit vectors)
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public Angle AngleTo(Vector3D other, int decimal_precision = Constants.THREE_DECIMALS) => Angle.FromRadians(
        Math.Acos(DotProduct(other) /
                  (Length() * other.Length())));  // TODO: adjust the sign of the acos according to the quadrant dy
                                                  // falls into (what quadrant ? this is 3D)

    // special formatting functions

    public override string ToString() => "{" + String.Format("{0:F3} {1:F3} {2:F3}", X, Y, Z) + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return string.Format(
          "VECTOR (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}") + ")",
          X,
          Y,
          Z);
    }

    // serialization functions
    // Implement this method to serialize data. The method is called on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("X", X, typeof(double));
      info.AddValue("Y", Y, typeof(double));
      info.AddValue("Z", Z, typeof(double));
    }

    public Vector3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      X = (double)info.GetValue("X", typeof(double));
      Y = (double)info.GetValue("Y", typeof(double));
      Z = (double)info.GetValue("Z", typeof(double));
    }

    public static Vector3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Vector3D)(new BinaryFormatter().Deserialize(fs));
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

    // operations with the plane
    public bool IsPerpendicular(Plane plane, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Normal.IsParallel(this, decimal_precision) && plane.AxisU.IsPerpendicular(this, decimal_precision);
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

    public UnitVector3D(SerializationInfo info, StreamingContext context) => FromVector(new Vector3D(info, context));

    // clang-format off
    public  static new UnitVector3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (UnitVector3D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }
// clang-format on
}
}
