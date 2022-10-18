using System;

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
  }
}
