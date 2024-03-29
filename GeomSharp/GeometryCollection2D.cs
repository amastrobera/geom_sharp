﻿using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;

namespace GeomSharp {
  /// <summary>
  /// A Line on an arbitrary 2D plane
  /// It's either defined as an infinite straight line passing by two points
  /// or an infinite straight line passing by a point, with a given direction
  /// </summary>
  [Serializable]
  public class GeometryCollection2D : Geometry2D,
                                      IEnumerable<Geometry2D>,
                                      IEquatable<GeometryCollection2D>,
                                      ISerializable {
    private List<Geometry2D> Geometries;
    public readonly int Size;

    // constructors
    public GeometryCollection2D(IEnumerable<Geometry2D> geoms) {
      Geometries = geoms as List<Geometry2D>;
      Size = Geometries.Count;
    }

    // enumeration interface implementation
    public Geometry2D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Geometries[i];
      }
    }

    public IEnumerator<Geometry2D> GetEnumerator() {
      return Geometries.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    // generic overrides from object class
    public override string ToString() => base.ToString();
    public override int GetHashCode() => base.GetHashCode();

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is GeometryCollection2D &&
                                                 this.Equals((GeometryCollection2D)other);
    public override bool Equals(Geometry2D other) => other.GetType() == typeof(GeometryCollection2D) &&
                                                     this.Equals(other as GeometryCollection2D);

    public bool Equals(GeometryCollection2D other) => this.AlmostEquals(other);

    public override bool AlmostEquals(Geometry2D other,
                                      int decimal_precision = 3) => other.GetType() == typeof(GeometryCollection2D) &&
                                                                    this.AlmostEquals(other as GeometryCollection2D,
                                                                                      decimal_precision);

    public bool AlmostEquals(GeometryCollection2D other, int decimal_precision = Constants.THREE_DECIMALS) {
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
    public static bool operator ==(GeometryCollection2D a, GeometryCollection2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(GeometryCollection2D a, GeometryCollection2D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Geometries", Geometries, typeof(List<Geometry2D>));
    }

    public GeometryCollection2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Geometries = (List<Geometry2D>)info.GetValue("Geometries", typeof(List<Geometry2D>));
    }

    public static GeometryCollection2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (GeometryCollection2D)(new BinaryFormatter().Deserialize(fs));
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

    public override Geometry2D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

    // relationship to all the other geometries
    // point
    public override bool Contains(Point2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Geometries.Any(g => g.Contains(other, decimal_precision));

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
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line segment
    public override bool Intersects(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
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
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
    public override bool Overlaps(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polyline
    public override bool Intersects(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
    public override bool Overlaps(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  ray
    public override bool Intersects(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
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
  }

}
