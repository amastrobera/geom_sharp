using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  public static class IntersectionExtensions2D {
    // intersection functions among different objects

    // LineSegment and Line 2D
    public static bool Intersects(this LineSegment2D segment,
                                  Line2D line,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersection(line, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this LineSegment2D segment,
                                                  Line2D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var line_inter = segment.ToLine().Intersection(line, decimal_precision);
      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point2D)line_inter.Value;
      if (!segment.Contains(pI, decimal_precision)) {
        return new IntersectionResult();
      }
      return new IntersectionResult(pI);
    }

    public static bool Intersects(this Line2D line,
                                  LineSegment2D segment,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersects(line, decimal_precision);

    public static IntersectionResult Intersection(this Line2D line,
                                                  LineSegment2D segment,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersection(line, decimal_precision);

    // LineSegment and Ray 2D
    public static bool Intersects(this LineSegment2D segment,
                                  Ray2D ray,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this LineSegment2D segment,
                                                  Ray2D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var line_inter = segment.ToLine().Intersection(ray.ToLine(), decimal_precision);
      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point2D)line_inter.Value;
      if (!(segment.Contains(pI, decimal_precision) && ray.Contains(pI, decimal_precision))) {
        return new IntersectionResult();
      }
      return new IntersectionResult(pI);
    }

    public static bool Intersects(this Ray2D ray,
                                  LineSegment2D segment,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersects(ray, decimal_precision);

    public static IntersectionResult Intersection(this Ray2D ray,
                                                  LineSegment2D segment,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersection(ray, decimal_precision);

    // Line and Ray 2D
    public static bool Intersects(this Line2D line, Ray2D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Line2D line,
                                                  Ray2D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var line_inter = line.Intersection(ray.ToLine(), decimal_precision);
      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point2D)line_inter.Value;
      if (!ray.Contains(pI, decimal_precision)) {
        return new IntersectionResult();
      }
      return new IntersectionResult(pI);
    }

    public static bool Intersects(this Ray2D ray, Line2D line, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Intersects(line, decimal_precision);

    public static IntersectionResult Intersection(this Ray2D ray,
                                                  Line2D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Intersection(line, decimal_precision);

    // Triangle and Line 2D
    public static bool Intersects(this Triangle2D triangle,
                                  Line2D line,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(line, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle2D triangle,
                                                  Line2D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      // case 1: two points connect, but it's merely an overlap (the line contains one of the edges)
      if (line.Overlaps(LineSegment2D.FromPoints(triangle.P0, triangle.P1), decimal_precision) ||
          line.Overlaps(LineSegment2D.FromPoints(triangle.P1, triangle.P2), decimal_precision) ||
          line.Overlaps(LineSegment2D.FromPoints(triangle.P2, triangle.P0), decimal_precision)) {
        return new IntersectionResult();
      }

      (Point2D pi1, Point2D pi2) = (null, null);
      IntersectionResult edge_inter = null;

      // edge 1 intersection
      edge_inter = LineSegment2D.FromPoints(triangle.P0, triangle.P1).Intersection(line, decimal_precision);
      if (edge_inter.ValueType != typeof(NullValue)) {
        pi1 = (Point2D)edge_inter.Value;
      }

      // edge 2 intersection
      edge_inter = LineSegment2D.FromPoints(triangle.P1, triangle.P2).Intersection(line, decimal_precision);
      if (edge_inter.ValueType != typeof(NullValue)) {
        var p = (Point2D)edge_inter.Value;
        if (pi1 is null) {
          pi1 = p;
        } else {
          if (!pi1.AlmostEquals(p, decimal_precision)) {  // avoid duplicates
            pi2 = p;
          }
        }
      }

      // edge 3 intersection
      if (pi2 is null) {
        edge_inter = LineSegment2D.FromPoints(triangle.P2, triangle.P0).Intersection(line, decimal_precision);
        if (edge_inter.ValueType != typeof(NullValue)) {
          var p = (Point2D)edge_inter.Value;
          if (pi1 is null) {
            pi1 = p;
          } else {
            if (!pi1.AlmostEquals(p, decimal_precision)) {  // avoid duplicates
              pi2 = p;
            }
          }
        }
      }

      // case 2: no intersection
      if ((pi1 is null) && (pi2 is null)) {
        return new IntersectionResult();
      }

      // case 3: one point intersection
      if (!(pi1 is null) && (pi2 is null)) {
        return new IntersectionResult(pi1);
      }
      if ((pi1 is null) && !(pi2 is null)) {
        return new IntersectionResult(pi2);
      }

      // case 4: line segment intersection (return it in the same direction as the line)
      return new IntersectionResult((pi2 - pi1).SameDirectionAs(line.Direction, decimal_precision)
                                        ? LineSegment2D.FromPoints(pi1, pi2)
                                        : LineSegment2D.FromPoints(pi2, pi1));
    }

    public static bool Intersects(this Line2D line,
                                  Triangle2D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersects(line, decimal_precision);

    public static IntersectionResult Intersection(this Line2D line,
                                                  Triangle2D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(line, decimal_precision);

    // Triangle and ray 2D
    public static bool Intersects(this Triangle2D triangle,
                                  Ray2D ray,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle2D triangle,
                                                  Ray2D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var inter_res = ray.ToLine().Intersection(triangle, decimal_precision);

      if (inter_res.ValueType == typeof(NullValue)) {
        // case 1: no intersection
        return new IntersectionResult();

      } else if (inter_res.ValueType == typeof(Point2D)) {
        // case 2: intersection is a point. return if inside the ray
        if (ray.Contains((Point2D)inter_res.Value, decimal_precision)) {
          return inter_res;
        }

      } else if (inter_res.ValueType == typeof(LineSegment2D)) {
        // case 3: intersection is a line segment. return what is inside the ray, if anything
        var inter_seg = (LineSegment2D)inter_res.Value;

        (bool p0_in, bool p1_in) =
            (ray.Contains(inter_seg.P0, decimal_precision), ray.Contains(inter_seg.P1, decimal_precision));

        // the segment is all inside the ray, or not at all
        if (p0_in && p1_in) {
          return inter_res;
        } else if (p0_in && !p1_in) {
          return new IntersectionResult(inter_seg.P0);
        } else if (!p0_in && p1_in) {
          return new IntersectionResult(inter_seg.P1);
        }
      }

      // case 4: none of the earlier ifs captured a valid intersection, return nothing
      return new IntersectionResult();
    }

    public static bool Intersects(this Ray2D ray,
                                  Triangle2D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersects(ray, decimal_precision);

    public static IntersectionResult Intersection(this Ray2D ray,
                                                  Triangle2D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(ray, decimal_precision);

    // Triangle and LineSegment 2D

    public static bool Intersects(this Triangle2D triangle,
                                  LineSegment2D segment,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(segment, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle2D triangle,
                                                  LineSegment2D segment,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var inter_res = segment.ToLine().Intersection(triangle, decimal_precision);
      if (inter_res.ValueType == typeof(NullValue)) {
        // case 1: no intersection
        return new IntersectionResult();

      } else if (inter_res.ValueType == typeof(Point2D)) {
        // case 2: intersection is a point. return if inside the ray
        if (segment.Contains((Point2D)inter_res.Value, decimal_precision)) {
          return inter_res;
        }

      } else if (inter_res.ValueType == typeof(LineSegment2D)) {
        // case 3: intersection is a line segment. return what is inside the ray, if anything
        var inter_seg = (LineSegment2D)inter_res.Value;

        (bool p0_in, bool p1_in) =
            (segment.Contains(inter_seg.P0, decimal_precision), segment.Contains(inter_seg.P1, decimal_precision));

        // the segment is all inside the segment, or not at all
        if (p0_in && p1_in) {
          return inter_res;
        } else if (p0_in && !p1_in) {
          return new IntersectionResult(inter_seg.P0);
        } else if (p0_in && !p1_in) {
          return new IntersectionResult(inter_seg.P1);
        }
      }

      // case 4: none of the earlier ifs captured a valid intersection, return nothing
      return new IntersectionResult();
    }

    public static bool Intersects(this LineSegment2D segment,
                                  Triangle2D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersects(segment, decimal_precision);

    public static IntersectionResult Intersection(this LineSegment2D segment,
                                                  Triangle2D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(segment, decimal_precision);

    // Polyline to Line
    public static bool Intersects(this Polyline2D pline,
                                  Line2D line,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(line, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polyline2D pline,
                                                  Line2D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point2D>();
      int n = pline.Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg = LineSegment2D.FromPoints(pline[i1], pline[i2], decimal_precision);
        var inter = seg.Intersection(line, decimal_precision);
        if (inter.ValueType == typeof(Point2D)) {
          mpoint.Add((Point2D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet2D(mpoint)) : new IntersectionResult();
    }

    public static bool Intersects(this Line2D line,
                                  Polyline2D pline,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Line2D line,
                                                  Polyline2D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(line, decimal_precision);

    // Polyline to Ray
    public static bool Intersects(this Polyline2D pline, Ray2D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polyline2D pline,
                                                  Ray2D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point2D>();
      int n = pline.Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg = LineSegment2D.FromPoints(pline[i1], pline[i2], decimal_precision);
        var inter = seg.Intersection(ray, decimal_precision);
        if (inter.ValueType == typeof(Point2D)) {
          mpoint.Add((Point2D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet2D(mpoint)) : new IntersectionResult();
    }

    public static bool Intersects(this Ray2D ray, Polyline2D pline, int decimal_precision = Constants.THREE_DECIMALS) =>
        ray.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Ray2D ray,
                                                  Polyline2D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(ray, decimal_precision);

    // Polyline to LineSegment
    public static bool Intersects(this Polyline2D pline,
                                  LineSegment2D seg,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(seg, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polyline2D pline,
                                                  LineSegment2D seg,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point2D>();
      int n = pline.Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg2 = LineSegment2D.FromPoints(pline[i1], pline[i2], decimal_precision);
        var inter = seg2.Intersection(seg, decimal_precision);
        if (inter.ValueType == typeof(Point2D)) {
          mpoint.Add((Point2D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet2D(mpoint)) : new IntersectionResult();
    }

    public static bool Intersects(this LineSegment2D seg,
                                  Polyline2D pline,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        seg.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this LineSegment2D seg,
                                                  Polyline2D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(seg, decimal_precision);

    // Triangle and Polyline
    public static bool Intersects(this Polyline2D pline,
                                  Triangle2D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(triangle, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polyline2D pline,
                                                  Triangle2D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      throw new NotImplementedException();
    }

    public static bool Intersects(this Triangle2D triangle,
                                  Polyline2D pline,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle2D triangle,
                                                  Polyline2D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(triangle, decimal_precision);

    // Polygon to Line
    public static bool Intersects(this Polygon2D poly, Line2D line, int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(line, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polygon2D poly,
                                                  Line2D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint_pair =
          new List<(bool EdgeState,
                    Point2D Intersection)>();  // open-close + intersection point; the OPEN (or CLOSED) states cannot be
                                               // represented by enum inside a method, therefore they are a boolean

      // Console.WriteLine("\n\tline=" + line.ToWkt(decimal_precision) + "\n\tpoly=" + poly.ToWkt(decimal_precision));

      // test all edges intersections
      for (int i = 0; i < poly.Size; ++i) {
        try {
          (int i1, int i2) = (i, (i + 1) % poly.Size);
          var poly_seg = LineSegment2D.FromPoints(poly[i1], poly[i2], decimal_precision);
          var inter = line.Intersection(poly_seg, decimal_precision);
          if (inter.ValueType == typeof(Point2D)) {
            // do not add the point if it is only a "touch", that is if it the line strikes through a vertex of the
            // polygon
            var p = (Point2D)inter.Value;
            var p0_loc = line.Location(poly_seg.P0, decimal_precision);
            var p1_loc = line.Location(poly_seg.P1, decimal_precision);

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
          (s1, s2) => line.ProjectInto(s1.P0, decimal_precision).CompareTo(line.ProjectInto(s2.P0, decimal_precision)));

      return new IntersectionResult(new LineSegmentSet2D(multi_line_set));
    }

    public static bool Intersects(this Line2D line, Polygon2D poly, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Intersection(poly, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Line2D line,
                                                  Polygon2D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(line, decimal_precision);

    // Polygon to Ray
    public static bool Intersects(this Polygon2D poly, Ray2D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polygon2D poly,
                                                  Ray2D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var line_inter = ray.ToLine().Intersection(poly, decimal_precision);

      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      Func<LineSegment2D, IntersectionResult> GetResultFromSegment = (LineSegment2D seg_inter) => {
        (bool p0_in, bool p1_in) =
            (ray.IsAhead(seg_inter.P0, decimal_precision), ray.IsAhead(seg_inter.P1, decimal_precision));

        if (p0_in && p1_in) {
          return new IntersectionResult(seg_inter);
        }
        if (!p0_in && p1_in) {
          return new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, seg_inter.P1, decimal_precision));
        }
        if (p0_in && !p1_in) {
          return new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, seg_inter.P0, decimal_precision));
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

    public static bool Intersects(this Ray2D ray, Polygon2D poly, int decimal_precision = Constants.THREE_DECIMALS) =>
        ray.Intersection(poly, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Ray2D ray,
                                                  Polygon2D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(ray, decimal_precision);

    // Polygon to LineSegment
    public static bool Intersects(this Polygon2D poly,
                                  LineSegment2D seg,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(seg, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polygon2D poly,
                                                  LineSegment2D seg,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var line_inter = seg.ToLine().Intersection(poly, decimal_precision);

      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      Func<LineSegment2D, IntersectionResult> GetResultFromSegment = (LineSegment2D seg_inter) => {
        (bool p0_in, bool p1_in) =
            (seg.Contains(seg_inter.P0, decimal_precision), seg.Contains(seg_inter.P1, decimal_precision));

        (bool this_p0_in, bool this_p1_in) =
            (seg_inter.Contains(seg.P0, decimal_precision), seg_inter.Contains(seg.P1, decimal_precision));

        // case 1: intersection segment is all contained in the segment
        if (p0_in && p1_in) {
          return new IntersectionResult(seg_inter);
        }
        // case 2: the segment is all contained in the intersection segment
        if (this_p0_in && this_p1_in) {
          return new IntersectionResult(seg);
        }
        // case 3-4: only a portion of the intersection segment is contained in the segment
        if (!p0_in && p1_in) {
          return new IntersectionResult(LineSegment2D.FromPoints(seg.P0, seg_inter.P1, decimal_precision));
        }
        if (p0_in && !p1_in) {
          return new IntersectionResult(LineSegment2D.FromPoints(seg_inter.P0, seg.P1, decimal_precision));
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

    public static bool Intersects(this LineSegment2D seg,
                                  Polygon2D poly,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        seg.Intersection(poly, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this LineSegment2D seg,
                                                  Polygon2D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(seg, decimal_precision);

    // Polygon to Polyline
    public static bool Intersects(this Polygon2D poly,
                                  Polyline2D pline,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polygon2D poly,
                                                  Polyline2D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point2D>();

      // test all edges intersections
      for (int i = 0; i < poly.Size; ++i) {
        try {
          var poly_seg = LineSegment2D.FromPoints(poly[i], poly[(i + 1) % poly.Size], decimal_precision);
          for (int j = 0; j <= pline.Size - 1; ++j) {
            var pline_seg = LineSegment2D.FromPoints(pline[j], pline[(j + 1) % poly.Size], decimal_precision);
            var inter = pline_seg.Intersection(poly_seg, decimal_precision);
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

    public static bool Intersects(this Polyline2D pline,
                                  Polygon2D poly,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polyline2D pline,
                                                  Polygon2D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(pline, decimal_precision);

    // Triangle and Polygon
    public static bool Intersects(this Polygon2D poly,
                                  Triangle2D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(triangle, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polygon2D poly,
                                                  Triangle2D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      throw new NotImplementedException();
    }

    public static bool Intersects(this Triangle2D triangle,
                                  Polygon2D poly,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(poly, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle2D triangle,
                                                  Polygon2D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(triangle, decimal_precision);
  }
}
