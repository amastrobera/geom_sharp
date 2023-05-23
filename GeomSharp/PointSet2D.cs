using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using GeomSharp.Extensions;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  [Serializable]
  public class PointSet2D : Geometry2D, IEquatable<PointSet2D>, IEnumerable<Point2D>, ISerializable {
    private List<Point2D> Vertices;
    public readonly int Size;

    // constructors
    public PointSet2D(Point2D[] points, int decimal_precision = Constants.THREE_DECIMALS) {
      Vertices = (new List<Point2D>(points)).RemoveDuplicates(decimal_precision);
      Size = Vertices.Count;
    }

    public PointSet2D(IEnumerable<Point2D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    // enumeration interface implementation
    public Point2D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Vertices[i];
      }
    }
    public IEnumerator<Point2D> GetEnumerator() {
      return Vertices.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    // generic overrides from object class
    public override int GetHashCode() => ToWkt().GetHashCode();
    public override string ToString() => (Vertices.Count == 0)
                                             ? "{}"
                                             : "{" + string.Join(",", Vertices.Select(v => v.ToString())) +
                                                   "," + Vertices[0].ToString() + "}";

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is PointSet2D && this.Equals((PointSet2D)other);
    public override bool Equals(Geometry2D other) => other.GetType() == typeof(PointSet2D) &&
                                                     this.Equals(other as PointSet2D);

    public bool Equals(PointSet2D other) => this.AlmostEquals(other);

    public override bool AlmostEquals(Geometry2D other, int decimal_precision = 3) =>
        other.GetType() == typeof(PointSet2D) && this.AlmostEquals(other as PointSet2D, decimal_precision);
    public bool AlmostEquals(PointSet2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other is null) {
        return false;
      }
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

    // comparison operators
    public static bool operator ==(PointSet2D a, PointSet2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(PointSet2D a, PointSet2D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Vertices", Vertices, typeof(List<Point2D>));
    }
    
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

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Vertices.Count == 0)
            ? "MULTIPOINT EMPTY"
            : "MULTIPOINT (" +
                  string.Join(",",
                              Vertices.Select(v => string.Format(
                                                  String.Format("({0}0:F{1:D}{2} {0}1:F{1:D}{2})", "{", precision, "}"),
                                                  v.U,
                                                  v.V))) +
                  ")";
    public override Geometry2D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

    // own functions
    public Point2D CenterOfMass() =>
        Point2D.FromVector(Vertices.Select(v => v.ToVector()).Aggregate((v1, v2) => v1 + v2) / Size);

    public (Point2D Min, Point2D Max) BoundingBox() => (new Point2D(Vertices.Min(v => v.U), Vertices.Min(v => v.V)),
                                                        new Point2D(Vertices.Max(v => v.U), Vertices.Max(v => v.V)));

    // formatting functions

    public List<Point2D> ToList() => Vertices.ToList();

    public Polygon2D ConcaveHull() => Polygon2D.ConcaveHull(Vertices);

    public Polygon2D ConvexHull(int decimal_precision = Constants.THREE_DECIMALS) =>
        Polygon2D.ConvexHull(Vertices, decimal_precision);
  }
}
