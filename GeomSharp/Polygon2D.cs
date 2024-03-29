﻿using GeomSharp.Collections;

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
  public class Polygon2D : Geometry2D, IEquatable<Polygon2D>, IEnumerable<Point2D>, ISerializable {
    private List<Point2D> Vertices;
    public readonly int Size;

    // constructors
    public Polygon2D(Point2D[] points, int decimal_precision = Constants.THREE_DECIMALS) {
      if (points.Length < 3) {
        throw new ArgumentException("tried to initialize a polygon with less than 3 points");
      }

      Vertices = (new List<Point2D>(points)).RemoveCollinearPoints(decimal_precision);
      // input adjustment: correcting mistake of passing collinear points to a polygon
      if (Vertices.Count < 3) {
        throw new ArgumentException("tried to initialize a polygon with less than 3 non-collinear points");
      }

      Size = Vertices.Count;
    }

    public Polygon2D(Triangle2D triangle, int decimal_precision = Constants.THREE_DECIMALS)
        : this(new Point2D[3] { triangle.P0, triangle.P1, triangle.P2 }, decimal_precision) {}

    public Polygon2D(IEnumerable<Point2D> points, int decimal_precision = Constants.THREE_DECIMALS)
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
    public override bool Equals(object other) => other != null && other is Polygon2D && this.Equals((Polygon2D)other);
    public override bool Equals(Geometry2D other) => other.GetType() == typeof(Polygon2D) &&
                                                     this.Equals(other as Polygon2D);

    public bool Equals(Polygon2D other) => this.AlmostEquals(other);

    public override bool AlmostEquals(Geometry2D other, int decimal_precision = 3) =>
        other.GetType() == typeof(Polygon2D) && this.AlmostEquals(other as Polygon2D, decimal_precision);
    public bool AlmostEquals(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other is null) {
        return false;
      }

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

    // comparison operators
    public static bool operator ==(Polygon2D a, Polygon2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Polygon2D a, Polygon2D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Vertices", Vertices, typeof(List<Point2D>));
    }

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

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) =>
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
    public override Geometry2D FromWkt(string wkt) {
      throw new NotImplementedException();
    }

    // relationship to all the other geometries

    //  point
    public override bool Contains(Point2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      // first check to save time, if the point is outside the bounding box, then it's outside the polygon too
      // improvement to the algorithm's big-O
      var bbox = BoundingBox();
      if (Math.Round(other.U - bbox.Min.U, decimal_precision) < 0 ||
          Math.Round(other.V - bbox.Min.V, decimal_precision) < 0 ||
          Math.Round(other.U - bbox.Max.U, decimal_precision) > 0 ||
          Math.Round(other.V - bbox.Max.V, decimal_precision) > 0) {
        return false;
      }

      // test for border containment (the other is on the perimeter of the polygon)
      for (int i = 0; i < Size; i++) {
        if (LineSegment2D.FromPoints(Vertices[i], Vertices[(i + 1) % Size]).Contains(other, decimal_precision)) {
          return true;
        }
      }

      // winding number method (without using the ray.Intersects(segment) function ...
      int wn = 0;
      for (int i = 0; i < Size; i++) {
        var edge = LineSegment2D.FromPoints(Vertices[i], Vertices[(i + 1) % Size]);

        // upward crossing, up the wn
        if (Math.Round(edge.P0.V - other.V, decimal_precision) <= 0) {
          if (Math.Round(edge.P1.V - other.V, decimal_precision) > 0) {
            var relative_location = edge.Location(other, decimal_precision);
            if (relative_location == Constants.Location.LEFT) {
              ++wn;
            }
          }
        } else {
          // downward crossing, down the wn
          if (Math.Round(edge.P1.V - other.V, decimal_precision) <= 0) {
            var relative_location = edge.Location(other, decimal_precision);
            if (relative_location == Constants.Location.RIGHT) {
              --wn;
            }
          }
        }
      }

      return !(wn == 0);
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
      var mpoint_pair =
          new List<(bool EdgeState,
                    Point2D Intersection)>();  // open-close + intersection point; the OPEN (or CLOSED) states cannot be
                                               // represented by enum inside a method, therefore they are a boolean

      // Console.WriteLine("\n\tline=" + line.ToWkt(decimal_precision) + "\n\tpoly=" + poly.ToWkt(decimal_precision));

      // test all edges intersections
      for (int i = 0; i < Size; ++i) {
        try {
          (int i1, int i2) = (i, (i + 1) % Size);
          var poly_seg = LineSegment2D.FromPoints(Vertices[i1], Vertices[i2], decimal_precision);
          var inter = other.Intersection(poly_seg, decimal_precision);
          if (inter.ValueType == typeof(Point2D)) {
            // do not add the point if it is only a "touch", that is if it the line strikes through a vertex of the
            // polygon
            var p = (Point2D)inter.Value;
            var p0_loc = other.Location(poly_seg.P0, decimal_precision);
            var p1_loc = other.Location(poly_seg.P1, decimal_precision);

            // Console.WriteLine("\n\t(" + i1.ToString() + "," + i2.ToString() + ") point intersection on " +
            //                   p.ToWkt(decimal_precision) + "\n\t\tp0_loc=" + p0_loc + ", p1_loc=" + p1_loc);

            // WARNING: the code below assumes the Polygon2D to be counter-clockwise sorted; if this is not the case,
            // the checks below should be the opposite (false/true)
            if (p1_loc == Constants.Location.ON_SEGMENT || p1_loc == Constants.Location.ON_LINE) {
              // Console.WriteLine("\n\t\t x rejected");
              continue;  // discard intersections with the first or last point, consider only pure intersection (segment
                         // split in two)
            }
            if ((p0_loc == Constants.Location.LEFT || p0_loc == Constants.Location.ON_SEGMENT ||
                 p0_loc == Constants.Location.ON_LINE) &&
                p1_loc == Constants.Location.RIGHT) {
              // Console.WriteLine("\n\t\t - accepted, open");
              mpoint_pair.Add((true, p));  // open intersection segment
            }
            if ((p0_loc == Constants.Location.RIGHT || p0_loc == Constants.Location.ON_SEGMENT ||
                 p0_loc == Constants.Location.ON_LINE) &&
                p1_loc == Constants.Location.LEFT) {
              // Console.WriteLine("\n\t\t - accepted, close");
              mpoint_pair.Add((false, p));  // close intersection segment
            }
          }
        } catch (Exception ex) {
          // warning of Intersection throw
        }
      }

      if (mpoint_pair.Count == 0) {
        return new IntersectionResult();
      }

      var multi_line_set = new List<LineSegment2D>();

      Func<bool, int, int> GetFirstIndex = (bool open_closed_state, int i_from) => {
        for (int _i = i_from; _i < i_from + mpoint_pair.Count; ++_i) {
          int _idx = (_i % mpoint_pair.Count);
          if (mpoint_pair[_idx].EdgeState == open_closed_state) {
            return _idx;
          }
        }
        return int.MinValue;
      };

      while (mpoint_pair.Count > 0) {
        int i_open = GetFirstIndex(true, 0);
        if (i_open == int.MinValue) {
          // this is just a touch point, not an intersection
          // Console.WriteLine("Polygon2D to Line intersection failed to find i_open" +
          //                  "\n\tpoly=" + poly.ToWkt(decimal_precision) + "\n\tline=" +
          //                  line.ToWkt(decimal_precision));
          return new IntersectionResult();
        }
        int i_closed = GetFirstIndex(false, i_open);
        if (i_closed == int.MinValue) {
          // this is just a touch point, not an intersection
          // Console.WriteLine("Polygon2D to Line intersection failed to find i_closed" +
          //                  "\n\tpoly=" + poly.ToWkt(decimal_precision) + "\n\tline=" +
          //                  line.ToWkt(decimal_precision));
          return new IntersectionResult();
        }
        // use indices from the list
        multi_line_set.Add(LineSegment2D.FromPoints(mpoint_pair[i_open].Intersection,
                                                    mpoint_pair[i_closed].Intersection,
                                                    decimal_precision));
        // pop indices out of the list
        if (i_open > i_closed) {
          mpoint_pair.RemoveAt(i_open);
          mpoint_pair.RemoveAt(i_closed);
        } else {
          mpoint_pair.RemoveAt(i_closed);
          mpoint_pair.RemoveAt(i_open);
        }
      }

      if (multi_line_set.Count == 1) {
        return new IntersectionResult(multi_line_set[0]);
      }

      // sort the segments by appearence (of their first point, P0) on the line's direction
      multi_line_set.Sort(
          (s1, s2) =>
              other.ProjectInto(s1.P0, decimal_precision).CompareTo(other.ProjectInto(s2.P0, decimal_precision)));

      return new IntersectionResult(new LineSegmentSet2D(multi_line_set));
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
      var line_inter = Intersection(other.ToLine(), decimal_precision);

      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      Func<LineSegment2D, IntersectionResult> GetResultFromSegment = (LineSegment2D seg_inter) => {
        (bool p0_in, bool p1_in) =
            (other.Contains(seg_inter.P0, decimal_precision), other.Contains(seg_inter.P1, decimal_precision));

        (bool this_p0_in, bool this_p1_in) =
            (seg_inter.Contains(other.P0, decimal_precision), seg_inter.Contains(other.P1, decimal_precision));

        // case 1: intersection segment is all contained in the segment
        if (p0_in && p1_in) {
          return new IntersectionResult(seg_inter);
        }
        // case 2: the segment is all contained in the intersection segment
        if (this_p0_in && this_p1_in) {
          return new IntersectionResult(other);
        }
        // case 3-4: only a portion of the intersection segment is contained in the segment
        if (!p0_in && p1_in && !other.P0.AlmostEquals(seg_inter.P1, decimal_precision)) {
          return new IntersectionResult(LineSegment2D.FromPoints(other.P0, seg_inter.P1, decimal_precision));
        }
        if (p0_in && !p1_in && !seg_inter.P0.AlmostEquals(other.P1, decimal_precision)) {
          return new IntersectionResult(LineSegment2D.FromPoints(seg_inter.P0, other.P1, decimal_precision));
        }
        return new IntersectionResult();
      };

      if (line_inter.ValueType == typeof(LineSegment2D)) {
        return GetResultFromSegment((LineSegment2D)line_inter.Value);
      }

      if (line_inter.ValueType == typeof(LineSegmentSet2D)) {
        var mline = new List<LineSegment2D>();
        foreach (var lseg in (LineSegmentSet2D)line_inter.Value) {
          var seg_res = GetResultFromSegment(lseg);
          if (seg_res.ValueType == typeof(LineSegment2D)) {
            mline.Add((LineSegment2D)seg_res.Value);
          }
        }
        if (mline.Count > 0) {
          return new IntersectionResult(new LineSegmentSet2D(mline));
        }
      }

      return new IntersectionResult();
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
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) {
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
    public override bool Overlaps(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polyline
    public override bool Intersects(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Polyline2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point2D>();

      // test all edges intersections
      for (int i = 0; i < Size; ++i) {
        try {
          var poly_seg = LineSegment2D.FromPoints(Vertices[i], Vertices[(i + 1) % Size], decimal_precision);
          for (int j = 0; j <= other.Size - 1; ++j) {
            var other_seg = LineSegment2D.FromPoints(other[j], other[(j + 1) % Size], decimal_precision);
            var inter = other_seg.Intersection(poly_seg, decimal_precision);
            if (inter.ValueType == typeof(Point2D)) {
              mpoint.Add((Point2D)inter.Value);
            }
          }
        } catch (Exception ex) {
          // warning of Intersection throw
        }
      }

      if (mpoint.Count == 0) {
        return new IntersectionResult();
      }

      return new IntersectionResult(new PointSet2D(mpoint));
    }

    public override bool Overlaps(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  ray
    public override bool Intersects(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var line_inter = Intersection(other.ToLine(), decimal_precision);

      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      Func<LineSegment2D, IntersectionResult> GetResultFromSegment = (LineSegment2D seg_inter) => {
        (bool p0_in, bool p1_in) =
            (other.IsAhead(seg_inter.P0, decimal_precision), other.IsAhead(seg_inter.P1, decimal_precision));

        if (p0_in && p1_in) {
          return new IntersectionResult(seg_inter);
        }
        if (!p0_in && p1_in) {
          return new IntersectionResult(LineSegment2D.FromPoints(other.Origin, seg_inter.P1, decimal_precision));
        }
        if (p0_in && !p1_in) {
          return new IntersectionResult(LineSegment2D.FromPoints(other.Origin, seg_inter.P0, decimal_precision));
        }
        return new IntersectionResult();
      };

      if (line_inter.ValueType == typeof(LineSegment2D)) {
        return GetResultFromSegment((LineSegment2D)line_inter.Value);
      }

      if (line_inter.ValueType == typeof(LineSegmentSet2D)) {
        var mline = new List<LineSegment2D>();
        foreach (var seg in (LineSegmentSet2D)line_inter.Value) {
          var seg_res = GetResultFromSegment(seg);
          if (seg_res.ValueType == typeof(LineSegment2D)) {
            mline.Add((LineSegment2D)seg_res.Value);
          }
        }
        if (mline.Count > 0) {
          return new IntersectionResult(new LineSegmentSet2D(mline));
        }
      }

      return new IntersectionResult();
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
    public double Area() {
      double a = 0;  // risk of overflow ...

      for (int i = 0; i < Vertices.Count; i++) {
        int j = (i + 1) % Vertices.Count;
        a += Vertices[i].ToVector2D().PerpProduct(Vertices[j].ToVector2D());
      }

      return a / 2;
    }

    public LineSegmentSet2D ToSegments(int decimal_precision = Constants.THREE_DECIMALS) {
      var lineset = new List<LineSegment2D>();

      for (int i = 0; i < Vertices.Count; i++) {
        int j = (1 + i) % Vertices.Count;
        lineset.Add(LineSegment2D.FromPoints(Vertices[i], Vertices[j], decimal_precision));
      }

      return new LineSegmentSet2D(lineset);
    }

    /// <summary>
    /// Center of mass of a non self-intersecting polygon
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArithmeticException"></exception>
    public Point2D CenterOfMass() {
      // TODO: add function of polygon triangularization to handle the case of self-intersecting polygon
      (double cx, double cy) = (0, 0);

      double signed_area = 0;
      for (int i = 0; i < Vertices.Count; i++) {
        int j = (i + 1) % Vertices.Count;

        double a_piece = Vertices[i].ToVector2D().PerpProduct(Vertices[j].ToVector2D());  // same maths for Area()
        signed_area += a_piece;

        cx += (Vertices[i].U + Vertices[j].U) * a_piece;
        cy += (Vertices[i].V + Vertices[j].V) * a_piece;
      }

      if (Math.Round(signed_area, Constants.THREE_DECIMALS) == 0) {
        throw new ArithmeticException("CenterOfMass failed, area = 0 (3 decimal precisions)");
      }

      signed_area /= 2;
      cx /= (6 * signed_area);
      cy /= (6 * signed_area);

      return new Point2D(cx, cy);
    }

    public (Point2D Min, Point2D Max) BoundingBox() => (new Point2D(Vertices.Min(v => v.U), Vertices.Min(v => v.V)),
                                                        new Point2D(Vertices.Max(v => v.U), Vertices.Max(v => v.V)));

    public List<Triangle2D> Triangulate() {
      // TODO: add this function

      if (Vertices.Count == 3) {
        return new List<Triangle2D> { Triangle2D.FromPoints(Vertices[0], Vertices[1], Vertices[2]) };
      }

      throw new NotImplementedException("triangulation not implemented yet");
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
  }
}
