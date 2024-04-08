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
  public class LineSegmentSet3D : Geometry3D, IEquatable<LineSegmentSet3D>, IEnumerable<LineSegment3D>, ISerializable {
    private List<LineSegment3D> Items;
    public readonly int Size;

    // constructors
    public LineSegmentSet3D(LineSegment3D[] segments, int decimal_precision = Constants.THREE_DECIMALS) {
      {
        // dictionary of strings to verify unique inclusion
        var seg_hashmap = new Dictionary<string, int>();
        foreach (var s in (new List<LineSegment3D>(segments).Select((p, i) => (p.ToWkt(decimal_precision), i)))) {
          (string key, int i) = (s.Item1, s.Item2);
          if (!seg_hashmap.ContainsKey(key)) {
            seg_hashmap.Add(key, i);
          }
        }
        Items = new List<LineSegment3D>(seg_hashmap.OrderBy(p => p.Value).Select(pair => segments[pair.Value]));
      }

      Size = Items.Count;
    }

    public LineSegmentSet3D(IEnumerable<LineSegment3D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    // enumeration interface implementation
    public LineSegment3D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Items[i];
      }
    }
    public IEnumerator<LineSegment3D> GetEnumerator() {
      return Items.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    // generic overrides from object class
    public override string ToString() => (Items.Count == 0) ? "{}"
                                                            : "{" + string.Join(",", Items.Select(v => v.ToString())) +
                                                                  "," + Items[0].ToString() + "}";
    public override int GetHashCode() => ToWkt().GetHashCode();

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is LineSegmentSet3D &&
                                                 this.Equals((LineSegmentSet3D)other);
    public override bool Equals(Geometry3D other) => other.GetType() == typeof(LineSegmentSet3D) &&
                                                     this.Equals(other as LineSegmentSet3D);
    public bool Equals(LineSegmentSet3D other) => this.AlmostEquals(other);
    public override bool AlmostEquals(Geometry3D other, int decimal_precision = 3) =>
        other.GetType() == typeof(LineSegmentSet3D) && this.AlmostEquals(other as LineSegmentSet3D, decimal_precision);

    public bool AlmostEquals(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other is null) {
        return false;
      }
      if (other.Size != Size) {
        return false;
      }

      // dictionary of strings to verify point inclusion
      HashSet<string> seg_hashset = Items.Select(p => p.ToWkt(decimal_precision)).ToHashSet();

      foreach (var s in other.Items) {
        if (!seg_hashset.Contains(s.ToWkt(decimal_precision))) {
          return false;
        }
      }

      return true;
    }

    // comparison operators
    public static bool operator ==(LineSegmentSet3D a, LineSegmentSet3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(LineSegmentSet3D a, LineSegmentSet3D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Items", Items, typeof(List<LineSegment3D>));
    }

    public LineSegmentSet3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Items = (List<LineSegment3D>)info.GetValue("Items", typeof(List<LineSegment3D>));
    }

    public static LineSegmentSet3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (LineSegmentSet3D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Items.Count == 0)
            ? "MULTILINESTRING EMPTY"
            : "MULTILINESTRING (" +
                  string.Join(
                      ",",
                      Items.Select(
                          s => "(" +
                               string.Format(
                                   String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                                   s.P0.X,
                                   s.P0.Y,
                                   s.P0.Z) +
                               "," +
                               string.Format(
                                   String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                                   s.P1.X,
                                   s.P1.Y,
                                   s.P1.Z) +
                               ")")) +
                  ")";

    public override Geometry3D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

    // own functions
    public Point3D CenterOfMass() => Point3D.FromVector(
        Items.Select(s => (s.P0.ToVector() + s.P1.ToVector()) / 2.0).Aggregate((v1, v2) => v1 + v2) / Size);

    public (Point3D Min, Point3D Max) BoundingBox() => (new Point3D(Items.Min(s => Math.Min(s.P0.X, s.P1.X)),
                                                                    Items.Min(s => Math.Min(s.P0.Y, s.P1.Y)),
                                                                    Items.Min(s => Math.Min(s.P0.Z, s.P1.Z))),
                                                        new Point3D(Items.Max(s => Math.Max(s.P0.X, s.P1.X)),
                                                                    Items.Max(s => Math.Max(s.P0.Y, s.P1.Y)),
                                                                    Items.Max(s => Math.Max(s.P0.Z, s.P1.Z))));

    public List<LineSegment3D> ToList() => Items.ToList();

    // relationship to all the other geometries

    //  plane
    public override bool Intersects(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Items.Any(g => g.Intersects(other, decimal_precision));
    public override IntersectionResult Intersection(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Items.Any(g => g.Overlaps(other, decimal_precision));
    public override IntersectionResult Overlap(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    // point
    public override bool Contains(Point3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Items.Any(g => g.Contains(other, decimal_precision));

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
