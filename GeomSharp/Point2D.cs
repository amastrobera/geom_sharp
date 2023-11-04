using System;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using GeomSharp.Algebra;

namespace GeomSharp {
  /// <summary>
  /// A Point of two coordinates (U,V) on an arbitrary 2D plane
  /// </summary>
  [Serializable]
  public class Point2D : Geometry2D, IEquatable<Point2D>, ISerializable {
    public double U { get; }
    public double V { get; }

    public static Point2D Zero => new Point2D(0, 0);

    // constructors
    public Point2D(double u = 0, double v = 0) => (U, V) = (u, v);

    public Point2D(Point2D copy) => (U, V) = (copy.U, copy.V);

    public Point2D(Vector copy_raw) {
      if (copy_raw.Size != 2) {
        throw new ArgumentException(
            String.Format("tried to initialize a Point2D with an {0:D}-dimention vector", copy_raw.Size));
      }
      (U, V) = (copy_raw[0], copy_raw[1]);
    }

    // generic overrides from object class
    public override int GetHashCode() => ToWkt().GetHashCode();
    public override string ToString() => "{" + String.Format("{0:F9} {1:F9}", U, V) + "}";

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is Point2D && this.Equals((Point2D)other);
    public override bool Equals(Geometry2D other) => other.GetType() == typeof(Point2D) &&
                                                     this.Equals(other as Point2D);

    public bool Equals(Point2D other) => this.AlmostEquals(other);

    public override bool AlmostEquals(Geometry2D other, int decimal_precision = 3) =>
        other.GetType() == typeof(Point2D) && this.AlmostEquals(other as Point2D, decimal_precision);

    public bool AlmostEquals(Point2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) &&
        ToVector().AlmostEquals(other.ToVector(), decimal_precision, Extensions.EqualityMethod.SUM_OF_SQUARES);

    // comparison operators
    public static bool operator ==(Point2D a, Point2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Point2D a, Point2D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("U", U, typeof(double));
      info.AddValue("V", V, typeof(double));
    }

    public Point2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      U = (double)info.GetValue("U", typeof(double));
      V = (double)info.GetValue("V", typeof(double));
    }

    public static Point2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Point2D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        string.Format("POINT (" + String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}") + ")", U, V);

    // relationship to all the other geometries

    //  point
    public override bool Contains(Point2D other,
                                  int decimal_precision = Constants.THREE_DECIMALS) => AlmostEquals(other,
                                                                                                    decimal_precision);

    //  geometry collection
    public override bool Intersects(GeometryCollection2D other,
                                    int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult Intersection(
        GeometryCollection2D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(GeometryCollection2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(GeometryCollection2D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  line
    public override bool Intersects(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult Intersection(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        new IntersectionResult();
    public override bool Overlaps(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  line segment
    public override bool Intersects(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult
    Intersection(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  line segment set
    public override bool Intersects(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult
    Intersection(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(LineSegmentSet2D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  polygon
    public override bool Intersects(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult
    Intersection(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  polyline
    public override bool Intersects(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult
    Intersection(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  ray
    public override bool Intersects(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult Intersection(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        new IntersectionResult();
    public override bool Overlaps(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    //  triangle
    public override bool Intersects(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) => false;
    public override IntersectionResult
    Intersection(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) => new IntersectionResult();
    public override bool Overlaps(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Contains(this, decimal_precision);
    public override IntersectionResult Overlap(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    // own functions
    public static Point2D FromVector(Vector v) {
      return new Point2D(v);
    }

    public double[] ToArray() => new double[] { U, V };

    public Vector ToVector() => new Vector(new double[] { U, V });

    public Vector2D ToVector2D() {
      return new Vector2D(U, V);
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
  }

}
