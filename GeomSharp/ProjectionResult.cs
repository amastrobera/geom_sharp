using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp {

  /// <summary>
  /// Union of projection to 2D result. Typically used invoking the extensions .ToXY()/.ToYZ()/.ToZY()
  /// The Value can be null. If not null, it will anything. What ? Check the value type.
  /// For example.
  /// A line projected into XY plane
  ///   var projection = line1.ToXY(); // ProjectionResult
  ///   if ( projection.ValueType == typeof(NullValue) ) {
  ///       nothing to do
  ///   } else if ( projection.ValueType == typeof(Point2D) ) {
  ///       var proj = (Point2D) projection.Value;
  ///       Console.WriteLine(proj.ToWkt());
  ///   } else if ( projection.ValueType == typeof(Line2D) ) {
  ///       var proj = (Line2D) projection.Value;
  ///       Console.WriteLine(proj.ToWkt());
  ///   }
  /// </summary>
  public class ProjectionResult : IEquatable<ProjectionResult> {
    public object Value { get; }
    public Type ValueType { get; }

    public ProjectionResult() => (Value, ValueType) = (null, typeof(NullValue));
    public ProjectionResult(Point2D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(Point3D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(Line2D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(Line3D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(Ray2D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(Ray3D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(LineSegment2D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(LineSegment3D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(PointSet2D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(PointSet3D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(Polyline2D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(Polyline3D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(Polygon2D v) => (Value, ValueType) = (v, v.GetType());
    public ProjectionResult(Polygon3D v) => (Value, ValueType) = (v, v.GetType());

    public bool Equals(ProjectionResult other) => ValueType.Equals(other.ValueType) && Value.Equals(other.Value);

    public override bool Equals(object other) => other != null && other is Angle && this.Equals((Angle)other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(ProjectionResult a, ProjectionResult b) {
      return a.Equals(b);
    }

    public static bool operator !=(ProjectionResult a, ProjectionResult b) {
      return !a.Equals(b);
    }

    public override string ToString() {
      return ValueType.ToString() + ": " + Value.ToString();
    }
  }
}
