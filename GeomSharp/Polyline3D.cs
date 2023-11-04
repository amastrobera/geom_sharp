using GeomSharp.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GeomSharp {
  /// <summary>
  /// A Curve made of straight lines in 3D, each line bound by a pair of vertices
  /// </summary>
  [Serializable]
  public class Polyline3D : Geometry3D, IEquatable<Polyline3D>, IEnumerable<Point3D>, ISerializable {
    public List<Point3D> Nodes { get; }
    public readonly int Size;

    // constructors
    public Polyline3D(Point3D[] points, int decimal_precision = Constants.THREE_DECIMALS) {
      if (points.Length < 2) {
        throw new ArgumentException("tried to initialize a polyline with less than 2 points");
      }

      Nodes = (new List<Point3D>(points)).RemoveCollinearPoints(decimal_precision);
      // input adjustment: correcting mistake of passing collinear points to a polygon
      if (Nodes.Count < 2) {
        throw new ArgumentException("tried to initialize a polyline with less than 2 non-collinear points");
      }
      // set the size
      Size = Nodes.Count;
    }

    public Polyline3D(IEnumerable<Point3D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    // enumeration interface implementation
    public Point3D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Nodes[i];
      }
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }
    public IEnumerator<Point3D> GetEnumerator() {
      return Nodes.GetEnumerator();
    }

    // generic overrides from object class
    public override string ToString() => "{" + string.Join(",", Nodes.Select(v => v.ToString())) + "}";
    public override int GetHashCode() => ToWkt().GetHashCode();

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is Polyline3D && this.Equals((Polyline3D)other);
    public override bool Equals(Geometry3D other) => other.GetType() == typeof(Polyline3D) &&
                                                     this.Equals(other as Polyline3D);
    public bool Equals(Polyline3D other) => this.AlmostEquals(other);
    public override bool AlmostEquals(Geometry3D other, int decimal_precision = 3) =>
        other.GetType() == typeof(Polyline3D) && this.AlmostEquals(other as Polyline3D, decimal_precision);

    public bool AlmostEquals(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other is null) {
        return false;
      }
      if (other.Size != Size) {
        return false;
      }

      if (Math.Round(Length() - other.Length(), decimal_precision) != 0) {
        return false;
      }

      for (int i = 0; i < Size; ++i) {
        if (!Nodes[i].AlmostEquals(other.Nodes[i], decimal_precision)) {
          return false;
        }
      }

      return true;
    }

    // comparison operators
    public static bool operator !=(Polyline3D a, Polyline3D b) {
      return !a.AlmostEquals(b);
    }

    public static bool operator ==(Polyline3D a, Polyline3D b) {
      return a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Nodes", Nodes, typeof(List<Point3D>));
    }

    public Polyline3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Nodes = (List<Point3D>)info.GetValue("Nodes", typeof(List<Point3D>));
    }

    public static Polyline3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Polyline3D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Nodes.Count == 0)
            ? "LINESTRING EMPTY"
            : "LINESTRING (" +
                  string.Join(",",
                              Nodes.Select(v => string.Format(
                                               String.Format(
                                                   "{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                                               v.X,
                                               v.Y,
                                               v.Z))) +

                  ")";

    // relationship to all the other geometries

    //  plane
    public override bool Intersects(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Plane other, int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point3D>();
      int n = Size;
      for (int i = 0; i < n - 1; ++i) {
        var pline_seg = LineSegment3D.FromPoints(Nodes[i], Nodes[i + 1], decimal_precision);
        var inter = other.Intersection(pline_seg, decimal_precision);
        if (inter.ValueType == typeof(Point3D)) {
          mpoint.Add((Point3D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet3D(mpoint)) : new IntersectionResult();
    }
    public override bool Overlaps(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    // point
    public override bool Contains(Point3D point, int decimal_precision = Constants.THREE_DECIMALS) {
      foreach (var piece_line in ToSegments(decimal_precision)) {
        if (piece_line.Contains(point, decimal_precision)) {
          return true;
        }
      }
      return false;
    }

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
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point3D>();
      int n = Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg = LineSegment3D.FromPoints(Nodes[i1], Nodes[i2], decimal_precision);
        var inter = seg.Intersection(other, decimal_precision);
        if (inter.ValueType == typeof(Point3D)) {
          mpoint.Add((Point3D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet3D(mpoint)) : new IntersectionResult();
    }
    public override bool Overlaps(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line segment
    public override bool Intersects(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(LineSegment3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point3D>();
      int n = Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg2 = LineSegment3D.FromPoints(Nodes[i1], Nodes[i2], decimal_precision);
        var inter = seg2.Intersection(other, decimal_precision);
        if (inter.ValueType == typeof(Point3D)) {
          mpoint.Add((Point3D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet3D(mpoint)) : new IntersectionResult();
    }
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
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Polygon3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polyline
    public override bool Intersects(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);

    public override IntersectionResult Intersection(Polyline3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
      // TODO: bounding box test improvement

      var mpoints = new List<Point3D>();

      foreach (var seg in ToSegments(decimal_precision)) {
        var inter = seg.Intersection(other, decimal_precision);
        if (inter.ValueType == typeof(Point3D)) {
          mpoints.Add((Point3D)inter.Value);
        } else if (inter.ValueType == typeof(PointSet3D)) {
          foreach (var mp in (PointSet3D)inter.Value) {
            mpoints.Add(mp);
          }
        }
      }

      return (mpoints.Count > 0) ? new IntersectionResult(new PointSet3D(mpoints)) : new IntersectionResult();
    }

    public override bool Overlaps(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  ray
    public override bool Intersects(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point3D>();
      int n = Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg = LineSegment3D.FromPoints(Nodes[i1], Nodes[i2], decimal_precision);
        var inter = seg.Intersection(other, decimal_precision);
        if (inter.ValueType == typeof(Point3D)) {
          mpoint.Add((Point3D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet3D(mpoint)) : new IntersectionResult();
    }
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

    // own functions

    public (Point3D Min, Point3D Max)
        BoundingBox() => (new Point3D(Nodes.Min(v => v.X), Nodes.Min(v => v.Y), Nodes.Min(v => v.Z)),
                          new Point3D(Nodes.Max(v => v.X), Nodes.Max(v => v.Y), Nodes.Max(v => v.Z)));

    public double Length() {
      double d = 0;
      foreach (var line_piece in ToLines()) {
        d += line_piece.Length();
      }
      return d;
    }

    public LineSegmentSet3D ToSegments(int decimal_precision = Constants.THREE_DECIMALS) {
      var lineset = new List<LineSegment3D>();

      for (int i = 0; i < Nodes.Count - 1; i++) {
        lineset.Add(LineSegment3D.FromPoints(Nodes[i], Nodes[i + 1], decimal_precision));
      }

      return new LineSegmentSet3D(lineset);
    }

    public LineSegmentSet3D ToLines() {
      var lineset = new List<LineSegment3D>();

      for (int i = 0; i < Nodes.Count - 1; i++) {
        lineset.Add(LineSegment3D.FromPoints(Nodes[i], Nodes[i + 1]));
      }

      return new LineSegmentSet3D(lineset);
    }

    private int IndexOfNearestSegmentToPoint(Point3D point, int decimal_precision = Constants.THREE_DECIMALS) {
      (int i_min, double d_min) = (-1, double.MaxValue);
      foreach ((int idx, var seg) in ToSegments(decimal_precision).Select((v, i) => (i, v))) {
        double d = seg.DistanceTo(point);
        if (d < d_min) {
          d_min = d;
          i_min = idx;
        }
      }
      if (i_min < 0) {
        throw new Exception("IndexOfNearestSegmentToPoint failed to find i");
      }

      return i_min;
    }

    public Point3D ProjectOnto(Point3D p, int decimal_precision = Constants.THREE_DECIMALS) {
      int i_nearest = IndexOfNearestSegmentToPoint(p, decimal_precision);
      return Line3D.FromPoints(Nodes[i_nearest], Nodes[i_nearest + 1], decimal_precision).ProjectOnto(p);
    }

    /// <summary>
    /// Returns the relative position of a point onto a curve in % [0,1]
    /// Returns double.MaxValue and warns an error if the point does not belong to the curve at all
    /// It's in 3D, and works on a linearized version of the curve
    /// It's the twin function of GetPointOnPolyline
    /// </summary>
    /// <param name="point"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public double LocationPct(Point3D point, int decimal_precision = Constants.THREE_DECIMALS) {
      double curve_len = Length();
      double len_done = 0;

      foreach (var piece_line in ToSegments(decimal_precision)) {
        double piece_len = piece_line.Length();

        if (piece_line.Contains(point, decimal_precision)) {
          double t = piece_line.P0.DistanceTo(point) / curve_len;

          if (Math.Round(t, decimal_precision) >= 0.0 && Math.Round(t, decimal_precision) <= 1.0) {
            double pct = (len_done + t * piece_len) / curve_len;
            return pct;
          }
        }
        len_done += piece_len;
      }

      // no segment of the polyline contains the point, throw
      // throw new Exception("LocationAlongTheLine no polyline segment contains the point");
      return double.MaxValue;
    }

    /// <summary>
    /// Returns the point corresponding to the %len of the curve
    /// Gives a warning and returns null in case of errors
    /// It's the twin function of LocationAlongTheLine
    /// </summary>
    /// <param name="pct">must be in the range [0, 1] of throws</param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public Point3D GetPointOnPolyline(double pct, int decimal_precision = Constants.THREE_DECIMALS) {
      if (Math.Round(pct, decimal_precision) < 0 || Math.Round(pct, decimal_precision) > 1) {
        throw new ArgumentException("pct should be in the range [0,1]");
      }

      if (Math.Round(pct, decimal_precision) == 0) {
        return Nodes.First();
      }

      if (Math.Round(pct, decimal_precision) == 1) {
        return Nodes.Last();
      }

      double curve_len = Length();
      double curve_todo = pct * curve_len;
      double len_done = 0;
      double pct_done = 0;

      foreach (var piece_line in ToSegments(decimal_precision)) {
        double piece_len = piece_line.Length();

        if (Math.Round((len_done + piece_len) - curve_todo, decimal_precision) >= 0) {
          // found segment containing the point
          double tI = ((pct - pct_done) * curve_len) / piece_len;

          var PI = piece_line.P0 + tI * (piece_line.P1 - piece_line.P0);
          return PI;
        }

        len_done += piece_len;
        pct_done = len_done / curve_len;
      }
      // string.Format("GetPointOnPolyline did not find the point on the curve for {0:F4}%", pct)
      return null;
    }
  }
}
