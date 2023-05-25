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
  public class LineSegmentSet2D : Geometry2D, IEquatable<LineSegmentSet2D>, IEnumerable<LineSegment2D>, ISerializable {
    private List<LineSegment2D> Items;
    public readonly int Size;

    // constructors
    public LineSegmentSet2D(LineSegment2D[] segments, int decimal_precision = Constants.THREE_DECIMALS) {
      {
        // dictionary of strings to verify unique inclusion
        var seg_hashmap = new Dictionary<string, int>();
        foreach (var s in (new List<LineSegment2D>(segments).Select((p, i) => (p.ToWkt(decimal_precision), i)))) {
          (string key, int i) = (s.Item1, s.Item2);
          if (!seg_hashmap.ContainsKey(key)) {
            seg_hashmap.Add(key, i);
          }
        }
        Items = new List<LineSegment2D>(seg_hashmap.Select(pair => segments[pair.Value]));
      }

      Size = Items.Count;
    }

    public LineSegmentSet2D(IEnumerable<LineSegment2D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    // enumeration interface implementation
    public LineSegment2D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Items[i];
      }
    }

    public IEnumerator<LineSegment2D> GetEnumerator() {
      return Items.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    // generic overrides from object class
    public override int GetHashCode() => ToWkt().GetHashCode();
    public override string ToString() => (Items.Count == 0) ? "{}"
                                                            : "{" + string.Join(",", Items.Select(v => v.ToString())) +
                                                                  "," + Items[0].ToString() + "}";

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is LineSegmentSet2D &&
                                                 this.Equals((LineSegmentSet2D)other);
    public override bool Equals(Geometry2D other) => other.GetType() == typeof(LineSegmentSet2D) &&
                                                     this.Equals(other as LineSegmentSet2D);

    public bool Equals(LineSegmentSet2D other) => this.AlmostEquals(other);

    public override bool AlmostEquals(Geometry2D other, int decimal_precision = 3) =>
        other.GetType() == typeof(LineSegmentSet2D) && this.AlmostEquals(other as LineSegmentSet2D, decimal_precision);

    public bool AlmostEquals(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS) {
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
    public static bool operator ==(LineSegmentSet2D a, LineSegmentSet2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(LineSegmentSet2D a, LineSegmentSet2D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Items", Items, typeof(List<LineSegment2D>));
    }

    public LineSegmentSet2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Items = (List<LineSegment2D>)info.GetValue("Items", typeof(List<LineSegment2D>));
    }

    public static LineSegmentSet2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (LineSegmentSet2D)(new BinaryFormatter().Deserialize(fs));
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
                               string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"),
                                             s.P0.U,
                                             s.P0.V) +
                               "," +
                               string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"),
                                             s.P1.U,
                                             s.P1.V) +
                               ")")) +
                  ")";

    public override Geometry2D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

    // relationship to all the other geometries

    //  point
    public override bool Contains(Point2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Items.Any(g => g.Contains(other, decimal_precision));

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

    // own functions
    public Point2D CenterOfMass() => Point2D.FromVector(
        Items.Select(s => (s.P0.ToVector() + s.P1.ToVector()) / 2.0).Aggregate((v1, v2) => v1 + v2) / Size);

    public (Point2D Min, Point2D Max) BoundingBox() =>
        (new Point2D(Items.Min(s => Math.Min(s.P0.U, s.P1.U)), Items.Min(s => Math.Min(s.P0.V, s.P1.V))),
         new Point2D(Items.Max(s => Math.Max(s.P0.U, s.P1.U)), Items.Max(s => Math.Max(s.P0.V, s.P1.V))));

    public List<LineSegment2D> ToList() => Items.ToList();
  }
}
