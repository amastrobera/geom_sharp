using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  public class LineSegmentSet2D : IEquatable<LineSegmentSet2D>, IEnumerable<LineSegment2D> {
    private List<LineSegment2D> Items;
    public readonly int Size;

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

    public LineSegment2D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Items[i];
      }
    }

    public bool AlmostEquals(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS) {
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

    public bool Equals(LineSegmentSet2D other) => this.AlmostEquals(other);
    public override bool Equals(object other) => other != null && other is LineSegmentSet2D &&
                                                 this.Equals((LineSegmentSet2D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(LineSegmentSet2D a, LineSegmentSet2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(LineSegmentSet2D a, LineSegmentSet2D b) {
      return !a.AlmostEquals(b);
    }

    public IEnumerator<LineSegment2D> GetEnumerator() {
      return Items.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    public Point2D CenterOfMass() => Point2D.FromVector(
        Items.Select(s => (s.P0.ToVector() + s.P1.ToVector()) / 2.0).Aggregate((v1, v2) => v1 + v2) / Size);

    public (Point2D Min, Point2D Max) BoundingBox() =>
        (new Point2D(Items.Min(s => Math.Min(s.P0.U, s.P1.U)), Items.Min(s => Math.Min(s.P0.V, s.P1.V))),
         new Point2D(Items.Max(s => Math.Max(s.P0.U, s.P1.U)), Items.Max(s => Math.Max(s.P0.V, s.P1.V))));

    // formatting functions

    public List<LineSegment2D> ToList() => Items.ToList();

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
                               string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"),
                                             s.P0.U,
                                             s.P0.V) +
                               "," +
                               string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"),
                                             s.P1.U,
                                             s.P1.V) +
                               ")")) +
                  ")";
  }
}
