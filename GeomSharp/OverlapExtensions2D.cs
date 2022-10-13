using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  public static class OverlapExtensions2D {
    // intersection functions among different objects

    // LineSegment and Line 2D
    public static bool Overlaps(this LineSegment2D segment,
                                Line2D line,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlap(line, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this LineSegment2D segment,
                                             Line2D line,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Direction.IsParallel(segment.P1 - segment.P0, decimal_precision) && line.Contains(segment.P0,
                                                                                               decimal_precision)
            ? new IntersectionResult(segment)
            : new IntersectionResult();

    public static bool Overlaps(this Line2D line,
                                LineSegment2D segment,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlaps(line, decimal_precision);

    public static IntersectionResult Overlap(this Line2D line,
                                             LineSegment2D segment,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlap(line, decimal_precision);

    // LineSegment and Ray 2D
    public static bool Overlaps(this LineSegment2D segment,
                                Ray2D ray,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlap(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this LineSegment2D segment,
                                             Ray2D ray,
                                             int decimal_precision = Constants.THREE_DECIMALS) {
      if (!ray.Direction.IsParallel(segment.P1 - segment.P0, decimal_precision)) {
        return new IntersectionResult();
      }
      (bool p0_in, bool p1_in, bool origin_in) = (ray.Contains(segment.P0, decimal_precision),
                                                  ray.Contains(segment.P1, decimal_precision),
                                                  segment.Contains(ray.Origin, decimal_precision));

      if (origin_in) {
        if (p0_in) {
          return ray.Origin.AlmostEquals(segment.P0, decimal_precision)
                     ? new IntersectionResult(segment.P0)
                     : new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, segment.P0));
        }

        if (p1_in) {
          return ray.Origin.AlmostEquals(segment.P1, decimal_precision)
                     ? new IntersectionResult(segment.P1)
                     : new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, segment.P1));
        }
      }

      if (p0_in && p1_in) {
        return new IntersectionResult(segment);
      }

      if (p0_in && !p1_in) {
        return new IntersectionResult(segment.P0);
      }

      if (!p0_in && p1_in) {
        return new IntersectionResult(segment.P1);
      }

      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray2D ray,
                                LineSegment2D segment,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlaps(ray, decimal_precision);

    public static IntersectionResult Overlap(this Ray2D ray,
                                             LineSegment2D segment,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlap(ray, decimal_precision);

    // Line and Ray 2D
    public static bool Overlaps(this Line2D line, Ray2D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Overlap(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Line2D line,
                                             Ray2D ray,
                                             int decimal_precision = Constants.THREE_DECIMALS) {
      if (!ray.Direction.IsParallel(line.Direction, decimal_precision)) {
        return new IntersectionResult();
      }

      if (line.Contains(ray.Origin, decimal_precision)) {
        return new IntersectionResult(ray);
      }

      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray2D ray, Line2D line, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Overlaps(ray, decimal_precision);

    public static IntersectionResult Overlap(this Ray2D ray,
                                             Line2D line,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Overlap(ray, decimal_precision);

    // Triangle and Line 2D
    public static bool Overlaps(this Triangle2D triangle,
                                Line2D line,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(line, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle2D triangle,
                                             Line2D line,
                                             int decimal_precision = Constants.THREE_DECIMALS) {
      (bool p0_in, bool p1_in, bool p2_in) = (line.Contains(triangle.P0, decimal_precision),
                                              line.Contains(triangle.P1, decimal_precision),
                                              line.Contains(triangle.P2, decimal_precision));

      if (p0_in && p1_in) {
        return new IntersectionResult((triangle.P1 - triangle.P0).SameDirectionAs(line.Direction, decimal_precision)
                                          ? LineSegment2D.FromPoints(triangle.P0, triangle.P1)
                                          : LineSegment2D.FromPoints(triangle.P1, triangle.P0));
      }

      if (p1_in && p2_in) {
        return new IntersectionResult((triangle.P2 - triangle.P1).SameDirectionAs(line.Direction, decimal_precision)
                                          ? LineSegment2D.FromPoints(triangle.P1, triangle.P2)
                                          : LineSegment2D.FromPoints(triangle.P2, triangle.P1));
      }

      if (p2_in && p0_in) {
        return new IntersectionResult((triangle.P0 - triangle.P2).SameDirectionAs(line.Direction, decimal_precision)
                                          ? LineSegment2D.FromPoints(triangle.P2, triangle.P0)
                                          : LineSegment2D.FromPoints(triangle.P0, triangle.P2));
      }

      return new IntersectionResult();
    }

    public static bool Overlaps(this Line2D line,
                                Triangle2D triangle,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlaps(line, decimal_precision);

    public static IntersectionResult Overlap(this Line2D line,
                                             Triangle2D triangle,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(line, decimal_precision);

    // Triangle and ray 2D
    public static bool Overlaps(this Triangle2D triangle,
                                Ray2D ray,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle2D triangle,
                                             Ray2D ray,
                                             int decimal_precision = Constants.THREE_DECIMALS) {
      (bool p0_in, bool p1_in, bool p2_in) = (ray.Contains(triangle.P0, decimal_precision),
                                              ray.Contains(triangle.P1, decimal_precision),
                                              ray.Contains(triangle.P2, decimal_precision));

      // no intersection at all of any vertex
      if (!p0_in && !p1_in && !p2_in) {
        return new IntersectionResult();
      }

      // test if the ray completely contains one of the triangle edges
      if (p0_in && p1_in) {
        return new IntersectionResult((triangle.P1 - triangle.P0).SameDirectionAs(ray.Direction, decimal_precision)
                                          ? LineSegment2D.FromPoints(triangle.P0, triangle.P1)
                                          : LineSegment2D.FromPoints(triangle.P1, triangle.P0));
      }

      if (p1_in && p2_in) {
        return new IntersectionResult((triangle.P2 - triangle.P1).SameDirectionAs(ray.Direction, decimal_precision)
                                          ? LineSegment2D.FromPoints(triangle.P1, triangle.P2)
                                          : LineSegment2D.FromPoints(triangle.P2, triangle.P1));
      }

      if (p2_in && p0_in) {
        return new IntersectionResult((triangle.P0 - triangle.P2).SameDirectionAs(ray.Direction, decimal_precision)
                                          ? LineSegment2D.FromPoints(triangle.P2, triangle.P0)
                                          : LineSegment2D.FromPoints(triangle.P0, triangle.P2));
      }

      // test if the ray contains a part of the edges
      (var edge01, var edge12, var edge20) = (LineSegment2D.FromPoints(triangle.P0, triangle.P1),
                                              LineSegment2D.FromPoints(triangle.P1, triangle.P2),
                                              LineSegment2D.FromPoints(triangle.P2, triangle.P0));

      (bool orig_in_edge01, bool orig_in_edge12, bool orig_in_edge20) =
          (edge01.Contains(ray.Origin, decimal_precision),
           edge12.Contains(ray.Origin, decimal_precision),
           edge20.Contains(ray.Origin, decimal_precision));

      if (orig_in_edge01 && p0_in) {
        return new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, triangle.P0));
      }

      if (orig_in_edge01 && p1_in) {
        return new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, triangle.P1));
      }

      if (orig_in_edge12 && p1_in) {
        return new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, triangle.P1));
      }

      if (orig_in_edge12 && p2_in) {
        return new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, triangle.P2));
      }

      if (orig_in_edge20 && p2_in) {
        return new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, triangle.P2));
      }

      if (orig_in_edge20 && p0_in) {
        return new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, triangle.P0));
      }

      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray2D ray,
                                Triangle2D triangle,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlaps(ray, decimal_precision);

    public static IntersectionResult Overlap(this Ray2D ray,
                                             Triangle2D triangle,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(ray, decimal_precision);

    // Triangle and LineSegment 2D

    public static bool Overlaps(this Triangle2D triangle,
                                LineSegment2D segment,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(segment, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle2D triangle,
                                             LineSegment2D segment,
                                             int decimal_precision = Constants.THREE_DECIMALS) {
      var s_edge01_ovrl = LineSegment2D.FromPoints(triangle.P0, triangle.P1).Overlap(segment, decimal_precision);
      if (s_edge01_ovrl.ValueType != typeof(NullValue)) {
        return s_edge01_ovrl;
      }

      var s_edge12_ovrl = LineSegment2D.FromPoints(triangle.P1, triangle.P2).Overlap(segment, decimal_precision);
      if (s_edge12_ovrl.ValueType != typeof(NullValue)) {
        return s_edge12_ovrl;
      }

      var s_edge20_ovrl = LineSegment2D.FromPoints(triangle.P2, triangle.P0).Overlap(segment, decimal_precision);
      if (s_edge20_ovrl.ValueType != typeof(NullValue)) {
        return s_edge20_ovrl;
      }

      return new IntersectionResult();
    }

    public static bool Overlaps(this LineSegment2D segment,
                                Triangle2D triangle,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlaps(segment, decimal_precision);

    public static IntersectionResult Overlap(this LineSegment2D segment,
                                             Triangle2D triangle,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(segment, decimal_precision);
  }
}
