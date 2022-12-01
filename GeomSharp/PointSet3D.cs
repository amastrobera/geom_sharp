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
  public class PointSet3D : IEquatable<PointSet3D>, IEnumerable<Point3D>, ISerializable {
    private List<Point3D> Vertices;
    public readonly int Size;

    public PointSet3D(Point3D[] points, int decimal_precision = Constants.THREE_DECIMALS) {
      Vertices = (new List<Point3D>(points)).RemoveDuplicates(decimal_precision);
      Size = Vertices.Count;
    }

    public PointSet3D(IEnumerable<Point3D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    public Point3D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Vertices[i];
      }
    }

    public bool AlmostEquals(PointSet3D other, int decimal_precision = Constants.THREE_DECIMALS) {
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

    public bool Equals(PointSet3D other) => this.AlmostEquals(other);
    public override bool Equals(object other) => other != null && other is PointSet3D && this.Equals((PointSet3D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(PointSet3D a, PointSet3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(PointSet3D a, PointSet3D b) {
      return !a.AlmostEquals(b);
    }

    public IEnumerator<Point3D> GetEnumerator() {
      return Vertices.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    public Point3D CenterOfMass() =>
        Point3D.FromVector(Vertices.Select(v => v.ToVector()).Aggregate((v1, v2) => v1 + v2) / Size);

    public (Point3D Min, Point3D Max)
        BoundingBox() => (new Point3D(Vertices.Min(v => v.X), Vertices.Min(v => v.Y), Vertices.Min(v => v.Z)),
                          new Point3D(Vertices.Max(v => v.X), Vertices.Max(v => v.Y), Vertices.Max(v => v.Z)));

    // formatting functions

    public List<Point3D> ToList() => Vertices.ToList();

    public Polygon3D ConcaveHull() => Polygon3D.ConcaveHull(Vertices);

    public Polygon3D ConvexHull() => Polygon3D.ConvexHull(Vertices);

    public override string ToString() => (Vertices.Count == 0)
                                             ? "{}"
                                             : "{" + string.Join(",", Vertices.Select(v => v.ToString())) +
                                                   "," + Vertices[0].ToString() + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Vertices.Count == 0)
            ? "MULTIPOINT EMPTY"
            : "MULTIPOINT (" +
                  string.Join(
                      ",",
                      Vertices.Select(v => string.Format(String.Format("({0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2})",
                                                                       "{",
                                                                       precision,
                                                                       "}"),
                                                         v.X,
                                                         v.Y,
                                                         v.Z))) +
                  ")";

    // serialization functions
    // Implement this method to serialize data. The method is called on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Vertices", Vertices, typeof(List<Point3D>));
    }
    // The special constructor is used to deserialize values.
    public PointSet3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Vertices = (List<Point3D>)info.GetValue("Vertices", typeof(List<Point3D>));
    }

    public static PointSet3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (PointSet3D)(new BinaryFormatter().Deserialize(fs));
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
