using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp {

  /// <summary>
  /// Union of intersection results. The Value can be null. If not null, it will anything. What ? Check the value type.
  /// For example.
  /// A line intersects another line in one point
  ///   var intersection = line1.Intersection(line2); // IntersectionResult
  ///   if ( intersection.ValueType == typeof(NullValue) ) {
  ///       nothing to do
  ///   } else if ( intersection.ValueType == typeof(Point3D) ) {
  ///       var inter = (Point3D) intersection.Value;
  ///       Console.WriteLine(inter.ToWkt());
  ///   }
  /// </summary>
  public class IntersectionResult : IEquatable<IntersectionResult> {
    public object Value { get; }
    public Type ValueType { get; }

    public IntersectionResult() => (Value, ValueType) = (null, typeof(NullValue));
    public IntersectionResult(Point2D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Point3D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Line2D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Line3D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(LineSegment2D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(LineSegment3D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Ray2D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Ray3D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Triangle2D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Triangle3D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Polyline2D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Polyline3D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Polygon2D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(Polygon3D v) => (Value, ValueType) = (v, v.GetType());

    public IntersectionResult(PointSet2D v) => (Value, ValueType) = (v, v.GetType());
    public IntersectionResult(PointSet3D v) => (Value, ValueType) = (v, v.GetType());

    public bool AlmostEquals(IntersectionResult other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!ValueType.Equals(other.ValueType)) {
        return false;
      }

      // 2D
      if (ValueType == typeof(Point2D)) {
        return ((Point2D)Value).AlmostEquals((Point2D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(Line2D)) {
        return ((Line2D)Value).AlmostEquals((Line2D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(Ray2D)) {
        return ((Ray2D)Value).AlmostEquals((Ray2D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(LineSegment2D)) {
        return ((LineSegment2D)Value).AlmostEquals((LineSegment2D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(Polyline2D)) {
        return ((Polyline2D)Value).AlmostEquals((Polyline2D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(PointSet2D)) {
        return ((PointSet2D)Value).AlmostEquals((PointSet2D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(Triangle2D)) {
        return ((Triangle2D)Value).AlmostEquals((Triangle2D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(Polygon2D)) {
        return ((Polygon2D)Value).AlmostEquals((Polygon2D)other.Value, decimal_precision);
      }

      // 3D
      if (ValueType == typeof(Point3D)) {
        return ((Point3D)Value).AlmostEquals((Point3D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(Line3D)) {
        return ((Line3D)Value).AlmostEquals((Line3D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(Ray3D)) {
        return ((Ray3D)Value).AlmostEquals((Ray3D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(LineSegment3D)) {
        return ((LineSegment3D)Value).AlmostEquals((LineSegment3D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(Polyline3D)) {
        return ((Polyline3D)Value).AlmostEquals((Polyline3D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(PointSet3D)) {
        return ((PointSet3D)Value).AlmostEquals((PointSet3D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(Triangle3D)) {
        return ((Triangle3D)Value).AlmostEquals((Triangle3D)other.Value, decimal_precision);
      }
      if (ValueType == typeof(Polygon3D)) {
        return ((Polygon3D)Value).AlmostEquals((Polygon3D)other.Value, decimal_precision);
      }

      // other (exception)
      throw new Exception("unhandled value type = " + ValueType);
    }

    public bool Equals(IntersectionResult other) => AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Angle && this.Equals((Angle)other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(IntersectionResult a, IntersectionResult b) {
      return a.Equals(b);
    }

    public static bool operator !=(IntersectionResult a, IntersectionResult b) {
      return !a.Equals(b);
    }

    public override string ToString() {
      return ValueType.ToString() + ((Value is null) ? "" : ": " + Value.ToString());
    }

    public string ToWkt(int decimal_precision = Constants.THREE_DECIMALS) {
      if (Value is null) {
        return "";
      }

      // 2D
      if (ValueType == typeof(Point2D)) {
        return ((Point2D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(Line2D)) {
        return ((Line2D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(Ray2D)) {
        return ((Ray2D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(LineSegment2D)) {
        return ((LineSegment2D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(Polyline2D)) {
        return ((Polyline2D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(PointSet2D)) {
        return ((PointSet2D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(Triangle2D)) {
        return ((Triangle2D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(Polygon2D)) {
        return ((Polygon2D)Value).ToWkt(decimal_precision);
      }

      // 3D
      if (ValueType == typeof(Point3D)) {
        return ((Point3D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(Line3D)) {
        return ((Line3D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(Ray3D)) {
        return ((Ray3D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(LineSegment3D)) {
        return ((LineSegment3D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(Polyline3D)) {
        return ((Polyline3D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(PointSet3D)) {
        return ((PointSet3D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(Triangle3D)) {
        return ((Triangle3D)Value).ToWkt(decimal_precision);
      }
      if (ValueType == typeof(Polygon3D)) {
        return ((Polygon3D)Value).ToWkt(decimal_precision);
      }

      throw new Exception("ToWkt unmanaged value type " + ValueType.ToString());
    }
  }
}
