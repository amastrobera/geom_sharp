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
  public class PointSet2D : IEquatable<PointSet2D>, IEnumerable<Point2D>, ISerializable {
    private List<Point2D> Vertices;
    public readonly int Size;

    public PointSet2D(Point2D[] points, int decimal_precision = Constants.THREE_DECIMALS) {
      Vertices = (new List<Point2D>(points)).RemoveDuplicates(decimal_precision);
      Size = Vertices.Count;
    }

    public PointSet2D(IEnumerable<Point2D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    public Point2D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Vertices[i];
      }
    }

    public bool AlmostEquals(PointSet2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other.Size != Size) {
        return false;
      }

      // dictionary of strings to verify point inclusion
      HashSet<string> point_hashset = Vertices.Select(p => p.ToWkt(decimal_precision)).ToHashSet();

      foreach (var p in other.Vertices) {
        if (!point_hashset.Contains(p.ToWkt(decimal_precision))) {
          return false;
        }
      }

      return true;
    }

    public bool Equals(PointSet2D other) => this.AlmostEquals(other);
    public override bool Equals(object other) => other != null && other is PointSet2D && this.Equals((PointSet2D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(PointSet2D a, PointSet2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(PointSet2D a, PointSet2D b) {
      return !a.AlmostEquals(b);
    }

    public IEnumerator<Point2D> GetEnumerator() {
      return Vertices.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    public Point2D CenterOfMass() =>
        Point2D.FromVector(Vertices.Select(v => v.ToVector()).Aggregate((v1, v2) => v1 + v2) / Size);

    public (Point2D Min, Point2D Max) BoundingBox() => (new Point2D(Vertices.Min(v => v.U), Vertices.Min(v => v.V)),
                                                        new Point2D(Vertices.Max(v => v.U), Vertices.Max(v => v.V)));

    // formatting functions

    public List<Point2D> ToList() => Vertices.ToList();

    public Polygon2D ConcaveHull() => Polygon2D.ConcaveHull(Vertices);

    public Polygon2D ConvexHull(int decimal_precision = Constants.THREE_DECIMALS) =>
        Polygon2D.ConvexHull(Vertices, decimal_precision);

    public override string ToString() => (Vertices.Count == 0)
                                             ? "{}"
                                             : "{" + string.Join(",", Vertices.Select(v => v.ToString())) +
                                                   "," + Vertices[0].ToString() + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Vertices.Count == 0)
            ? "MULTIPOINT EMPTY"
            : "MULTIPOINT (" +
                  string.Join(",",
                              Vertices.Select(v => string.Format(
                                                  String.Format("({0}0:F{1:D}{2} {0}1:F{1:D}{2})", "{", precision, "}"),
                                                  v.U,
                                                  v.V))) +
                  ")";

    // serialization functions
    // Implement this method to serialize data. The method is called on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Vertices", Vertices, typeof(List<Point2D>));
    }
    // The special constructor is used to deserialize values.
    public PointSet2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Vertices = (List<Point2D>)info.GetValue("Vertices", typeof(List<Point2D>));
    }

    public static PointSet2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (PointSet2D)(new BinaryFormatter().Deserialize(fs));
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
