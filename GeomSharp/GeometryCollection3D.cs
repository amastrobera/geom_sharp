using System;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GeomSharp {
  /// <summary>
  /// A Line on an arbitrary 3D plane
  /// It's either defined as an infinite straight line passing by two points
  /// or an infinite straight line passing by a point, with a given direction
  /// </summary>
  [Serializable]
  public class GeometryCollection3D : Geometry3D,
                                      IEnumerable<Geometry3D>,
                                      IEquatable<GeometryCollection3D>,
                                      ISerializable {
    private List<Geometry3D> Geometries;
    public readonly int Size;

    // constructors
    public GeometryCollection3D(IEnumerable<Geometry3D> geoms) {
      Geometries = geoms as List<Geometry3D>;
      Size = Geometries.Count;
    }

    // enumeration interface implementation
    public Geometry3D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Geometries[i];
      }
    }

    public IEnumerator<Geometry3D> GetEnumerator() {
      return Geometries.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    // generic overrides from object class
    public override string ToString() => base.ToString();
    public override int GetHashCode() => base.GetHashCode();

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is GeometryCollection3D &&
                                                 this.Equals((GeometryCollection3D)other);
    public override bool Equals(Geometry3D other) => other.GetType() == typeof(GeometryCollection3D) &&
                                                     this.Equals(other as GeometryCollection3D);

    public bool Equals(GeometryCollection3D other) => this.AlmostEquals(other);

    public override bool AlmostEquals(Geometry3D other,
                                      int decimal_precision = 3) => other.GetType() == typeof(GeometryCollection3D) &&
                                                                    this.AlmostEquals(other as GeometryCollection3D,
                                                                                      decimal_precision);

    public bool AlmostEquals(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (Size != other.Size) {
        return false;
      }

      // naive approach, this only checks that the geometries are in the same order!
      for (int i = 0; i < Size; ++i) {
        if (!this[i].AlmostEquals(other[i], decimal_precision)) {
          return false;
        }
      }

      return true;
    }

    // comparison operators
    public static bool operator ==(GeometryCollection3D a, GeometryCollection3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(GeometryCollection3D a, GeometryCollection3D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Geometries", Geometries, typeof(List<Geometry3D>));
    }

    public GeometryCollection3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Geometries = (List<Geometry3D>)info.GetValue("Geometries", typeof(List<Geometry3D>));
    }

    public static GeometryCollection3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (GeometryCollection3D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) {
      StringBuilder s = new StringBuilder();

      s.Append("GEOMETRYCOLLECTION");
      if (Size == 0) {
        s.Append(" EMPTY");

      } else {
        s.Append(" (");
        foreach (var geom in Geometries) {
          s.Append(geom.ToWkt(precision));
        }
        s.Append(")");
      }

      return s.ToString();
    }

    public override Geometry3D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

    // relationship to all the other geometries

    //  plane
    public override bool Intersects(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Geometries.Any(g => g.Intersects(other, decimal_precision));
    public override IntersectionResult Intersection(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Geometries.Any(g => g.Overlaps(other, decimal_precision));
    public override IntersectionResult Overlap(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    // point
    public override bool Contains(Point3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Geometries.Any(g => g.Contains(other, decimal_precision));

    //  geometry collection
    public override bool Intersects(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(GeometryCollection3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(GeometryCollection3D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line
    public override bool Intersects(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line segment
    public override bool Intersects(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
    public override bool Overlaps(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line segment set
    public override bool Intersects(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(LineSegmentSet3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(LineSegmentSet3D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polygon
    public override bool Intersects(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
    public override bool Overlaps(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polyline
    public override bool Intersects(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
    public override bool Overlaps(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  ray
    public override bool Intersects(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  triangle
    public override bool Intersects(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
    public override bool Overlaps(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
  }

}
