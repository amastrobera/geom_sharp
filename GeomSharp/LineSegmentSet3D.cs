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
  public class LineSegmentSet3D : IEquatable<LineSegmentSet3D>, IEnumerable<LineSegment3D>, ISerializable {
    private List<LineSegment3D> Items;
    public readonly int Size;

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
        Items = new List<LineSegment3D>(seg_hashmap.Select(pair => segments[pair.Value]));
      }

      Size = Items.Count;
    }

    public LineSegmentSet3D(IEnumerable<LineSegment3D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    public LineSegment3D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Items[i];
      }
    }

    public bool AlmostEquals(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) {
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

    public bool Equals(LineSegmentSet3D other) => this.AlmostEquals(other);
    public override bool Equals(object other) => other != null && other is LineSegmentSet3D &&
                                                 this.Equals((LineSegmentSet3D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(LineSegmentSet3D a, LineSegmentSet3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(LineSegmentSet3D a, LineSegmentSet3D b) {
      return !a.AlmostEquals(b);
    }

    public IEnumerator<LineSegment3D> GetEnumerator() {
      return Items.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    public Point3D CenterOfMass() => Point3D.FromVector(
        Items.Select(s => (s.P0.ToVector() + s.P1.ToVector()) / 2.0).Aggregate((v1, v2) => v1 + v2) / Size);

    public (Point3D Min, Point3D Max) BoundingBox() => (new Point3D(Items.Min(s => Math.Min(s.P0.X, s.P1.X)),
                                                                    Items.Min(s => Math.Min(s.P0.Y, s.P1.Y)),
                                                                    Items.Min(s => Math.Min(s.P0.Z, s.P1.Z))),
                                                        new Point3D(Items.Max(s => Math.Max(s.P0.X, s.P1.X)),
                                                                    Items.Max(s => Math.Max(s.P0.Y, s.P1.Y)),
                                                                    Items.Max(s => Math.Max(s.P0.Z, s.P1.Z))));

    // formatting functions

    public List<LineSegment3D> ToList() => Items.ToList();

    public override string ToString() => (Items.Count == 0) ? "{}"
                                                            : "{" + string.Join(",", Items.Select(v => v.ToString())) +
                                                                  "," + Items[0].ToString() + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) =>
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

    // serialization functions
    // Implement this method to serialize data. The method is called on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Items", Items, typeof(List<LineSegment3D>));
    }
    // The special constructor is used to deserialize values.
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

    public void ToBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Create);
        (new BinaryFormatter()).Serialize(fs, this);
        fs.Close();
      } catch (Exception e) {
        // warning failed to deserialize
      }
    }
  }
}
