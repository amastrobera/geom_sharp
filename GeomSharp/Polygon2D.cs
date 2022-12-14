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
  public class Polygon2D : IEquatable<Polygon2D>, IEnumerable<Point2D>, ISerializable {
    private List<Point2D> Vertices;
    public readonly int Size;

    public Polygon2D(Point2D[] points, int decimal_precision = Constants.THREE_DECIMALS) {
      if (points.Length < 4) {
        throw new ArgumentException("tried to initialize a polygon with less than 4 points");
      }

      Vertices = (new List<Point2D>(points)).RemoveCollinearPoints(decimal_precision);
      // input adjustment: correcting mistake of passing collinear points to a polygon
      if (Vertices.Count < 4) {
        throw new ArgumentException("tried to initialize a polygon with less than 4 non-collinear points");
      }

      Size = Vertices.Count;
    }

    public Polygon2D(IEnumerable<Point2D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    public Point2D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Vertices[i];
      }
    }

    public bool AlmostEquals(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other.Size != Size) {
        return false;
      }

      // different area, different polygon
      if (Math.Round(Area() - other.Area(), decimal_precision) != 0) {
        return false;
      }

      // different set of points, different polygons
      //    function to return the index of the first point of the list equal to the given point
      Func<List<Point2D>, Point2D, int> GetFirstEqualPoint = (List<Point2D> _vertices, Point2D _point) => {
        for (int _i = 0; _i < _vertices.Count; _i++) {
          if (_vertices[_i].AlmostEquals(_point, decimal_precision)) {
            return _i;
          }
        }
        return -1;
      };
      //    no equal point found
      int first_equal_idx = GetFirstEqualPoint(other.Vertices, other[0]);
      if (first_equal_idx < 0) {
        return false;
      }
      //    test point by point
      for (int i = 0; i < Size; ++i) {
        int j = (first_equal_idx + i) % Size;
        if (!Vertices[i].AlmostEquals(other.Vertices[j], decimal_precision)) {
          return false;
        }
      }

      return true;
    }

    public bool Equals(Polygon2D other) => this.AlmostEquals(other);
    public override bool Equals(object other) => other != null && other is Polygon2D && this.Equals((Polygon2D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Polygon2D a, Polygon2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Polygon2D a, Polygon2D b) {
      return !a.AlmostEquals(b);
    }

    public IEnumerator<Point2D> GetEnumerator() {
      return Vertices.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    public double Area() {
      double a = 0;  // risk of overflow ...

      for (int i = 0; i < Vertices.Count - 1; i++) {
        a += Vertices[i].ToVector2D().PerpProduct(Vertices[i + 1].ToVector2D());
      }

      return a / 2;
    }

    public Point2D CenterOfMass() =>
        Point2D.FromVector(Vertices.Select(v => v.ToVector()).Aggregate((v1, v2) => v1 + v2) / Size);

    public (Point2D Min, Point2D Max) BoundingBox() => (new Point2D(Vertices.Min(v => v.U), Vertices.Min(v => v.V)),
                                                        new Point2D(Vertices.Max(v => v.U), Vertices.Max(v => v.V)));

    /// <summary>
    /// The winding number algorithms calculates the number of time the polygon winds around a point. The count is upped
    /// when a the ray from a point to the right crosses an upward-going edge, and downed when that ray crosses a
    /// downward-going edge. If the counter reaches zero, the point is outside.
    /// This algorithm works for simple and non-simple closed polygons, convex or non-convex.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool Contains(Point2D point, int decimal_precision = Constants.THREE_DECIMALS) {
      // first check to save time, if the point is outside the bounding box, then it's outside the polygon too
      // improvement to the algorithm's big-O
      var bbox = BoundingBox();
      if (Math.Round(point.U - bbox.Min.U, decimal_precision) < 0 ||
          Math.Round(point.V - bbox.Min.V, decimal_precision) < 0 ||
          Math.Round(point.U - bbox.Max.U, decimal_precision) > 0 ||
          Math.Round(point.V - bbox.Max.V, decimal_precision) > 0) {
        return false;
      }

      // test for border containment (the point is on the perimeter of the polygon)
      for (int i = 0; i < Size; i++) {
        if (LineSegment2D.FromPoints(Vertices[i], Vertices[(i + 1) % Size]).Contains(point, decimal_precision)) {
          return true;
        }
      }

      // winding number method (without using the ray.Intersects(segment) function ...
      int wn = 0;
      for (int i = 0; i < Size; i++) {
        var edge = LineSegment2D.FromPoints(Vertices[i], Vertices[(i + 1) % Size]);

        // upward crossing, up the wn
        if (Math.Round(edge.P0.V - point.V, decimal_precision) <= 0) {
          if (Math.Round(edge.P1.V - point.V, decimal_precision) > 0) {
            var relative_location = edge.Location(point, decimal_precision);
            if (relative_location == Constants.Location.LEFT) {
              ++wn;
            }
          }
        } else {
          // downward crossing, down the wn
          if (Math.Round(edge.P1.V - point.V, decimal_precision) <= 0) {
            var relative_location = edge.Location(point, decimal_precision);
            if (relative_location == Constants.Location.RIGHT) {
              --wn;
            }
          }
        }
      }

      return !(wn == 0);
    }

    public bool Intersects(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public IntersectionResult Intersection(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      // TODO: apply the notorious N*Log(N) algorithm of multi-segment intersections

      var mpoint = new List<Point2D>();

      // test all edges intersections
      for (int i = 0; i < Size - 1; ++i) {
        var seg = LineSegment2D.FromPoints(Vertices[i], Vertices[i + 1], decimal_precision);
        for (int j = 0; j < other.Size - 1; ++j) {
          var other_seg = LineSegment2D.FromPoints(other.Vertices[j], other.Vertices[j + 1], decimal_precision);
          try {
            var inter = seg.Intersection(other_seg, decimal_precision);
            if (inter.ValueType == typeof(Point2D)) {
              mpoint.Add((Point2D)inter.Value);
            }
          } catch (Exception ex) {
            // warning of Intersection throw
          }
        }
      }

      // add all contained points
      for (int i = 0; i < Size; ++i) {
        if (other.Contains(Vertices[i], decimal_precision)) {
          mpoint.Add(Vertices[i]);
        }
      }
      for (int j = 0; j < Size; ++j) {
        if (Contains(other.Vertices[j], decimal_precision)) {
          mpoint.Add(other.Vertices[j]);
        }
      }

      // convex hull of these points
      if (mpoint.Count == 0) {
        return new IntersectionResult();
      }

      if (mpoint.Count == 3) {
        return new IntersectionResult(Triangle2D.FromPoints(mpoint[0], mpoint[1], mpoint[2]));
      }

      var cvhull = ConvexHull(mpoint, decimal_precision);
      return (cvhull is null) ? new IntersectionResult() : new IntersectionResult(cvhull);
    }

    /// <summary>
    /// Sorts a list of points in CCW order and creates a polygon out of it
    /// </summary>
    /// <param name="points">any enumeration of 2D Points</param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public static Polygon2D ConcaveHull(IEnumerable<Point2D> points, int decimal_precision = Constants.THREE_DECIMALS) {
      var sorted_points = new List<Point2D>(points).SortCCW(decimal_precision).RemoveDuplicates(decimal_precision);

      return (sorted_points.Count < 3) ? null : new Polygon2D(sorted_points);
    }

    /// <summary>
    /// Computes the convex hull of any point enumeration
    /// </summary>
    /// <param name="points"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public static Polygon2D ConvexHull(List<Point2D> points, int decimal_precision = Constants.THREE_DECIMALS) {
      var concave_hull = ConcaveHull(points, decimal_precision);

      if (concave_hull is null) {
        return null;
      }
      var sorted_points = concave_hull.Vertices;

      // pick the lowest point
      int n = sorted_points.Count;
      int i0 = 0;
      double v_max = sorted_points[i0].V;
      for (int i = 1; i < n; ++i) {
        if (Math.Round(sorted_points[i].V - v_max, decimal_precision) > 0) {
          v_max = sorted_points[i].V;
          i0 = i;
        }
      }
      // initialize with the smallest point and the point after
      var cvpoints = new List<Point2D>();
      cvpoints.Add(sorted_points[i0 % n]);
      cvpoints.Add(sorted_points[(i0 + 1) % n]);

      for (int i = 2; i <= n; ++i) {
        cvpoints.Add(sorted_points[(i0 + i) % n]);

        int m = cvpoints.Count;
        int i3 = m - 1;
        int i2 = m - 2;
        int i1 = m - 3;

        // TODO: Works better when (precision - 1 if precision > 0 else precision) when used by point_to_line_location
        while (i1 >= 0 && Line2D.FromPoints(cvpoints[i1], cvpoints[i2], decimal_precision)
                                  .Location(cvpoints[i3], decimal_precision) != Constants.Location.LEFT) {
          cvpoints[i2] = cvpoints[i3];
          cvpoints.RemoveAt(cvpoints.Count - 1);
          i1 -= 1;
          i2 -= 1;
          i3 -= 1;
        }
      }
      // end condition
      if (cvpoints[0].AlmostEquals(cvpoints[cvpoints.Count - 1], decimal_precision)) {
        cvpoints.RemoveAt(cvpoints.Count - 1);
      }

      return (cvpoints.Count < 3) ? null : new Polygon2D(cvpoints);
    }

    // formatting functions
    public override string ToString() => (Vertices.Count == 0)
                                             ? "{}"
                                             : "{" + string.Join(",", Vertices.Select(v => v.ToString())) +
                                                   "," + Vertices[0].ToString() + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Vertices.Count == 0)
            ? "POLYGON EMPTY"
            : "POLYGON ((" +
                  string.Join(",",
                              Vertices.Select(
                                  v => string.Format(
                                      String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"), v.U, v.V))) +
                  ", " +
                  string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"),
                                Vertices[0].U,
                                Vertices[0].V) +
                  "))";

    // serialization functions
    // Implement this method to serialize data. The method is called on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Vertices", Vertices, typeof(List<Point2D>));
    }
    // The special constructor is used to deserialize values.
    public Polygon2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Vertices = (List<Point2D>)info.GetValue("Vertices", typeof(List<Point2D>));
    }

    public static Polygon2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Polygon2D)(new BinaryFormatter().Deserialize(fs));
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
