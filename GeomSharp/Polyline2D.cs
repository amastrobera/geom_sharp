﻿using GeomSharp.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GeomSharp {
  /// <summary>
  /// A Curve made of straight lines in 2D, each line bound by a pair of vertices
  /// </summary>
  [Serializable]
  public class Polyline2D : Geometry2D, IEquatable<Polyline2D>, IEnumerable<Point2D>, ISerializable {
    public List<Point2D> Nodes { get; }
    public readonly int Size;

    // constructors
    public Polyline2D(Point2D[] points, int decimal_precision = Constants.THREE_DECIMALS) {
      if (points.Length < 2) {
        throw new ArgumentException("tried to initialize a polyline with less than 2 points");
      }

      Nodes = (new List<Point2D>(points)).RemoveCollinearPoints(decimal_precision);
      // input adjustment: correcting mistake of passing collinear points to a polygon
      if (Nodes.Count < 2) {
        throw new ArgumentException("tried to initialize a polyline with less than 2 non-collinear points");
      }
      // set the size
      Size = Nodes.Count;
    }

    public Polyline2D(IEnumerable<Point2D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    // enumeration interface implementation
    public Point2D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Nodes[i];
      }
    }

    public IEnumerator<Point2D> GetEnumerator() {
      return Nodes.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    // generic overrides from object class
    public override int GetHashCode() => ToWkt().GetHashCode();
    public override string ToString() => "{" + string.Join(",", Nodes.Select(v => v.ToString())) + "}";

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is Polyline2D && this.Equals((Polyline2D)other);
    public override bool Equals(Geometry2D other) => other.GetType() == typeof(Polyline2D) &&
                                                     this.Equals(other as Polyline2D);

    public bool Equals(Polyline2D other) => this.AlmostEquals(other);

    public override bool AlmostEquals(Geometry2D other, int decimal_precision = 3) =>
        other.GetType() == typeof(Polyline2D) && this.AlmostEquals(other as Polyline2D, decimal_precision);

    public bool AlmostEquals(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) {
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
    public static bool operator ==(Polyline2D a, Polyline2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Polyline2D a, Polyline2D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Nodes", Nodes, typeof(List<Point2D>));
    }
    public Polyline2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Nodes = (List<Point2D>)info.GetValue("Nodes", typeof(List<Point2D>));
    }
    public static Polyline2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Polyline2D)(new BinaryFormatter().Deserialize(fs));
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
                              Nodes.Select(
                                  v => string.Format(
                                      String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"), v.U, v.V))) +

                  ")";
    public override Geometry2D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

    // relationship to all the other geometries

    //  point
    public override bool Contains(Point2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      foreach (var piece_line in ToSegments(decimal_precision)) {
        if (piece_line.Contains(other, decimal_precision)) {
          return true;
        }
      }
      return false;
    }

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
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point2D>();
      int n = Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg = LineSegment2D.FromPoints(Nodes[i1], Nodes[i2], decimal_precision);
        var inter = seg.Intersection(other, decimal_precision);
        if (inter.ValueType == typeof(Point2D)) {
          mpoint.Add((Point2D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet2D(mpoint)) : new IntersectionResult();
    }

    public override bool Overlaps(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line segment
    public override bool Intersects(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);

    public override IntersectionResult Intersection(LineSegment2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point2D>();
      int n = Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg2 = LineSegment2D.FromPoints(Nodes[i1], Nodes[i2], decimal_precision);
        var inter = seg2.Intersection(other, decimal_precision);
        if (inter.ValueType == typeof(Point2D)) {
          mpoint.Add((Point2D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet2D(mpoint)) : new IntersectionResult();
    }

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
        other.Intersects(this, decimal_precision);
    public override IntersectionResult Intersection(Polygon2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        other.Intersection(this, decimal_precision);
    public override bool Overlaps(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polyline
    public override bool Intersects(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);

    public override IntersectionResult Intersection(Polyline2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
      // TODO: bounding box test improvement

      var mpoints = new List<Point2D>();

      foreach (var seg in ToSegments(decimal_precision)) {
        var inter = seg.Intersection(other, decimal_precision);
        if (inter.ValueType == typeof(Point2D)) {
          mpoints.Add((Point2D)inter.Value);
        } else if (inter.ValueType == typeof(PointSet2D)) {
          foreach (var mp in (PointSet2D)inter.Value) {
            mpoints.Add(mp);
          }
        }
      }

      return (mpoints.Count > 0) ? new IntersectionResult(new PointSet2D(mpoints)) : new IntersectionResult();
    }
    public override bool Overlaps(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  ray
    public override bool Intersects(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point2D>();
      int n = Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg = LineSegment2D.FromPoints(Nodes[i1], Nodes[i2], decimal_precision);
        var inter = seg.Intersection(other, decimal_precision);
        if (inter.ValueType == typeof(Point2D)) {
          mpoint.Add((Point2D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet2D(mpoint)) : new IntersectionResult();
    }

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
    public double Length() {
      double d = 0;
      foreach (var line_piece in ToSegments()) {
        d += line_piece.Length();
      }
      return d;
    }
    public LineSegmentSet2D ToSegments(int decimal_precision = Constants.THREE_DECIMALS) {
      var lineset = new List<LineSegment2D>();

      for (int i = 0; i < Nodes.Count - 1; i++) {
        lineset.Add(LineSegment2D.FromPoints(Nodes[i], Nodes[i + 1], decimal_precision));
      }

      return new LineSegmentSet2D(lineset);
    }

    private int IndexOfNearestSegmentToPoint(Point2D point, int decimal_precision = Constants.THREE_DECIMALS) {
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

    public Point2D ProjectOnto(Point2D p, int decimal_precision = Constants.THREE_DECIMALS) {
      int i_nearest = IndexOfNearestSegmentToPoint(p, decimal_precision);
      return Line2D.FromPoints(Nodes[i_nearest], Nodes[i_nearest + 1], decimal_precision).ProjectOnto(p);
    }

    /// <summary>
    /// Returns the relative position of a point onto a curve in % [0,1]
    /// Returns double.MaxValue and warns an error if the point does not belong to the curve at all
    /// It's in 3D, and works on a linearized version of the curve
    /// It's the twin function of GetPoint
    /// </summary>
    /// <param name="point"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public double GetPct(Point2D point, int decimal_precision = Constants.THREE_DECIMALS) {
      double curve_len = Length();
      double len_done = 0;

      foreach (var piece_line in ToSegments(decimal_precision)) {
        double piece_len = piece_line.Length();
        if (piece_line.Contains(point, decimal_precision)) {
          double t = Math.Round(piece_line.P0.DistanceTo(point) / piece_line.Length(), decimal_precision);
          if (t >= 0 && t <= 1) {
            return (len_done + t * piece_len) / curve_len;
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
    /// <param name="pct_location">must be in the range [0, 1] of throws</param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public Point2D GetPoint(double pct_location, int decimal_precision = Constants.THREE_DECIMALS) {
      if (Math.Round(pct_location, decimal_precision) < 0 || Math.Round(pct_location, decimal_precision) > 1) {
        throw new ArgumentException("pct should be in the range [0,1]");
      }

      double curve_len = Math.Round(Length(), decimal_precision);
      double curve_todo = Math.Round(pct_location * curve_len, decimal_precision);
      double len_done = 0;
      double pct_done = 0;
      foreach (var piece_line in ToSegments(decimal_precision)) {
        double piece_len = piece_line.Length();
        if (Math.Round(len_done + piece_len, decimal_precision) >= curve_todo) {
          // found segment containing the point
          double tI = ((pct_location - pct_done) * curve_len) / piece_len;
          var PI = piece_line.P0 + tI * (piece_line.P1 - piece_line.P0);
          return PI;
        }
        len_done += piece_len;
        pct_done = len_done / curve_len;
      }

      // string.Format("GetPoint did not find the point on the curve for {0:F4}%", pct)
      return null;
    }
  }

}
