using GeomSharp.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  [Serializable]
  public class MultiPolygon2D : Geometry2D, IEquatable<MultiPolygon2D>, IEnumerable<Polygon2D>, ISerializable {
    private List<Polygon2D> Polygons;
    public readonly int Size;

    // constructors
    public MultiPolygon2D(Polygon2D[] polygons, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!polygons.Any()) {
        throw new ArgumentException("tried to initialize a multi polygon no polygon");
      }

      Polygons = new List<Polygon2D>(polygons);

      Size = Polygons.Count;
    }

    public MultiPolygon2D(Triangle2D[] triangles, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!triangles.Any()) {
        throw new ArgumentException("tried to initialize a multi polygon no triangles");
      }

      Polygons = triangles.Select(t => new Polygon2D(t, decimal_precision)).ToList();

      Size = Polygons.Count;
    }

    public MultiPolygon2D(IEnumerable<Triangle2D> triangles, int decimal_precision = Constants.THREE_DECIMALS)
        : this(triangles.ToArray(), decimal_precision) {}

    public MultiPolygon2D(IEnumerable<Polygon2D> polygons, int decimal_precision = Constants.THREE_DECIMALS)
        : this(polygons.ToArray(), decimal_precision) {}

    // enumeration interface implementation
    public Polygon2D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Polygons[i];
      }
    }
    public IEnumerator<Polygon2D> GetEnumerator() {
      return Polygons.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    // generic overrides from object class
    public override int GetHashCode() => ToWkt().GetHashCode();
    public override string ToString() => (Polygons.Count == 0)
                                             ? "{}"
                                             : "{" + string.Join(",", Polygons.Select(v => v.ToString())) +
                                                   "," + Polygons[0].ToString() + "}";

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is MultiPolygon2D &&
                                                 this.Equals((MultiPolygon2D)other);
    public override bool Equals(Geometry2D other) => other.GetType() == typeof(MultiPolygon2D) &&
                                                     this.Equals(other as MultiPolygon2D);

    public bool Equals(MultiPolygon2D other) => this.AlmostEquals(other);

    public override bool AlmostEquals(Geometry2D other, int decimal_precision = 3) =>
        other.GetType() == typeof(MultiPolygon2D) && this.AlmostEquals(other as MultiPolygon2D, decimal_precision);

    [Experimental("not implemented")]
    public bool AlmostEquals(MultiPolygon2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other is null) {
        return false;
      }

      // different number of points, different polygon
      if (other.Size != Size) {
        return false;
      }

      throw new NotImplementedException();
    }

    // comparison operators
    public static bool operator ==(MultiPolygon2D a, MultiPolygon2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(MultiPolygon2D a, MultiPolygon2D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Polygons", Polygons, typeof(List<Polygon2D>));
    }

    public MultiPolygon2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Polygons = (List<Polygon2D>)info.GetValue("Polygons", typeof(List<Polygon2D>));
    }

    public static MultiPolygon2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (MultiPolygon2D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Polygons.Count == 0) ? "MULTIPOLYGON EMPTY"
                              : "MULTIPOLYGON (" +
                                    string.Join(",",
                                                Polygons.Select(poly => poly.ToWkt(precision))
                                                    .Where(wkt => wkt != "POLYGON EMPTY")
                                                    .Select(s => s.Substring("POLYGON ".Length).Trim())) +
                                    ")";

    // relationship to all the other geometries

    //  point
    public override bool Contains(Point2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Polygons.Any(p => p.Contains(other, decimal_precision));

    //  geometry collection
    public override bool Intersects(GeometryCollection2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(GeometryCollection2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(GeometryCollection2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(GeometryCollection2D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line
    public override bool Intersects(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      throw new NotImplementedException("");
    }

    public override bool Overlaps(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line segment
    public override bool Intersects(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(LineSegment2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
      throw new NotImplementedException("");
    }
    public override bool Overlaps(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line segment set
    public override bool Intersects(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(LineSegmentSet2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(LineSegmentSet2D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polygon
    public override bool Intersects(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      throw new NotImplementedException("");
    }
    public override bool Overlaps(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polyline
    public override bool Intersects(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Polyline2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
      throw new NotImplementedException("");
    }

    public override bool Overlaps(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  ray
    public override bool Intersects(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      throw new NotImplementedException("");
    }
    public override bool Overlaps(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  triangle
    public override bool Intersects(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
    public override bool Overlaps(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    // own functions

    public double Area() => Polygons.Sum(p => p.Area());

    public LineSegmentSet2D ToSegments(int decimal_precision = Constants.THREE_DECIMALS) {
      var lineset = new List<LineSegment2D>();

      for (int i = 0; i < Polygons.Count; i++) {
        lineset.AddRange(Polygons[i].ToSegments(decimal_precision));
      }

      return new LineSegmentSet2D(lineset);
    }

    public (Point2D Min, Point2D Max)
        BoundingBox() => (new Point2D(Polygons.Min(p => p.Min(v => v.U)), Polygons.Min(p => p.Min(v => v.V))),
                          new Point2D(Polygons.Max(p => p.Max(v => v.U)), Polygons.Max(p => p.Max(v => v.V))));
  }
}
