using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  public class Polygon3D : IEquatable<Polygon3D>, IEnumerable<Point3D> {
    public List<Point3D> Vertices { get; }
    public readonly int Size;
    public UnitVector3D Normal { get; }

    public Polygon3D(params Point3D[] points) {
      if (points.Length < 3) {
        throw new ArgumentException("tried to initialize a polygon with less than 3 points");
      }
      Vertices = (new List<Point3D>(points)).RemoveCollinearPoints();
      // input adjustment: correcting mistake of passing collinear points to a polygon
      if (Vertices.Count < 3) {
        throw new ArgumentException("tried to initialize a polygon with less than 3 non-collinear points");
      }
      // adding the size
      Size = Vertices.Count;

      // adding the normal
      var centroid = Vertices.Average();
      Normal = (Vertices[0] - centroid).CrossProduct((Vertices[1] - centroid)).Normalize();

      // verify that all points are on the same plane!
      for (int i1 = 2; i1 < Size; i1++) {
        int i2 = (i1 + 1) % Size;
        var local_normal = (Vertices[0] - centroid).CrossProduct((Vertices[1] - centroid));
        if (!local_normal.CrossProduct(Normal).AlmostEquals(Vector3D.Zero)) {
          throw new ArgumentException(
              "tried to initialize a Polygon3D with a set of points that do not belong to the same plane");
        }
      }
    }

    public Polygon3D(IEnumerable<Point3D> points) : this(points.ToArray()) {}

    public Point3D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Vertices[i];
      }
    }

    public bool Equals(Polygon3D other) {
      if (other.Size != Size) {
        return false;
      }
      if (!Normal.AlmostEquals(other.Normal)) {
        return false;
      }
      var points_hashset = Vertices.ToHashSet();
      foreach (var p in other) {
        if (!points_hashset.Contains(p)) {
          return false;
        }
      }

      return true;
    }

    public override bool Equals(object other) => other != null && other is Point3D && this.Equals((Point3D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Polygon3D a, Polygon3D b) {
      return a.Equals(b);
    }

    public static bool operator !=(Polygon3D a, Polygon3D b) {
      return !a.Equals(b);
    }

    public IEnumerator<Point3D> GetEnumerator() {
      return Vertices.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    // special formatting
    public override string ToString() => (Vertices.Count == 0)
                                             ? "{}"
                                             : "{" + string.Join(",", Vertices.Select(v => v.ToString())) +
                                                   "," + Vertices[0].ToString() + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Vertices.Count == 0)
            ? "POLYGON EMPTY"
            : "POLYGON ((" +
                  string.Join(
                      ",",
                      Vertices.Select(v => string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}",
                                                                       "{",
                                                                       precision,
                                                                       "}"),
                                                         v.X,
                                                         v.Y,
                                                         v.Z))) +
                  ", " +
                  string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                                Vertices[0].X,
                                Vertices[0].Y,
                                Vertices[0].Z) +
                  "))";
  }

}
