using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using GeomSharp.Algebra;

namespace GeomSharp {
  /// <summary>
  /// A Point of two coordinates (X,Y,Z) on an arbitrary 3D plane
  /// </summary>

  [Serializable]
  public class Point3D : Geometry3D, IEquatable<Point3D>, ISerializable {
    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public static Point3D Zero => new Point3D(0, 0, 0);

    // constructors
    public Point3D(double x = 0, double y = 0, double z = 0) => (X, Y, Z) = (x, y, z);

    public Point3D(Point3D copy) => (X, Y, Z) = (copy.X, copy.Y, copy.Z);

    public Point3D(Vector copy_raw) {
      if (copy_raw.Size != 3) {
        throw new ArgumentException(
            String.Format("tried to initialize a Point3D with an {0:D}-dimention vector", copy_raw.Size));
      }
      X = copy_raw[0];
      Y = copy_raw[1];
      Z = copy_raw[2];
    }

    public static Point3D FromVector(Vector v) {
      return new Point3D(v);
    }

    // generic overrides from object class
    public override string ToString() => "{" + String.Format("{0:F9} {1:F9} {2:F9}", X, Y, Z) + "}";
    public override int GetHashCode() => ToWkt().GetHashCode();

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is Point3D && this.Equals((Point3D)other);
    public override bool Equals(Geometry3D other) => other.GetType() == typeof(Point3D) &&
                                                     this.Equals(other as Point3D);
    public bool Equals(Point3D other) => this.AlmostEquals(other);
    public override bool AlmostEquals(Geometry3D other, int decimal_precision = 3) =>
        other.GetType() == typeof(Point3D) && this.AlmostEquals(other as Point3D, decimal_precision);

    public bool AlmostEquals(Point3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) && Math.Round(this.X - other.X, decimal_precision) == 0 &&
        Math.Round(this.Y - other.Y, decimal_precision) == 0 && Math.Round(this.Z - other.Z, decimal_precision) == 0;

    // comparison operators
    public static bool operator ==(Point3D a, Point3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Point3D a, Point3D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("X", X, typeof(double));
      info.AddValue("Y", Y, typeof(double));
      info.AddValue("Z", Z, typeof(double));
    }

    public Point3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      X = (double)info.GetValue("X", typeof(double));
      Y = (double)info.GetValue("Y", typeof(double));
      Z = (double)info.GetValue("Z", typeof(double));
    }

    public static Point3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Point3D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) {
      return string.Format(
          "POINT (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}") + ")",
          X,
          Y,
          Z);
    }

    // relationship to all the other geometries

    //  plane
    public override bool Intersects(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  point
    public override bool Contains(Point3D other,
                                  int decimal_precision = Constants.THREE_DECIMALS) => AlmostEquals(other,
                                                                                                    decimal_precision);

    //  geometry collection
    public override bool Intersects(GeometryCollection3D other,
                                    int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult Intersection(
        GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(GeometryCollection3D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  line
    public override bool Intersects(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult Intersection(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        new IntersectionResult();
    public override bool Overlaps(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  line segment
    public override bool Intersects(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult
    Intersection(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  line segment set
    public override bool Intersects(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult
    Intersection(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(LineSegmentSet3D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  polygon
    public override bool Intersects(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult
    Intersection(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  polyline
    public override bool Intersects(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult
    Intersection(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  ray
    public override bool Intersects(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult Intersection(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        new IntersectionResult();
    public override bool Overlaps(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  triangle
    public override bool Intersects(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult
    Intersection(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    // own functions
    public double[] ToArray() => ToVector().ToArray();

    public Vector ToVector() => new Vector(new double[] { X, Y, Z });
    public Vector3D ToVector3D() {
      return new Vector3D(X, Y, Z);
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
  }

}
