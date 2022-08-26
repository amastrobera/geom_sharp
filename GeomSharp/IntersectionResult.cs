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

    public bool Equals(IntersectionResult other) => ValueType.Equals(other.ValueType) && Value.Equals(other.Value);

    public override bool Equals(object other) => other != null && other is Angle && this.Equals((Angle)other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(IntersectionResult a, IntersectionResult b) {
      return a.Equals(b);
    }

    public static bool operator !=(IntersectionResult a, IntersectionResult b) {
      return !a.Equals(b);
    }

    public override string ToString() {
      return ValueType.ToString() + ": " + Value.ToString();
    }
  }
}
