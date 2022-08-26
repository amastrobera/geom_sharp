using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  public static class OverlapExtensions2D {
    // intersection functions among different objects

    // LineSegment and Line 2D
    public static bool Overlaps(this LineSegment2D segment, Line2D line) => segment.Overlap(line).ValueType !=
                                                                            typeof(NullValue);

    public static IntersectionResult Overlap(this LineSegment2D segment, Line2D line) =>
        line.Direction.IsParallel(segment.P1 - segment.P0) && line.Contains(segment.P0)
            ? new IntersectionResult(segment)
            : new IntersectionResult();

    public static bool Overlaps(this Line2D line, LineSegment2D segment) => segment.Overlaps(line);

    public static IntersectionResult Overlap(this Line2D line, LineSegment2D segment) => segment.Overlap(line);

    // LineSegment and Ray 2D
    public static bool Overlaps(this LineSegment2D segment, Ray2D ray) => segment.Overlap(ray).ValueType !=
                                                                          typeof(NullValue);

    public static IntersectionResult Overlap(this LineSegment2D segment, Ray2D ray) {
      if (!ray.Direction.IsParallel(segment.P1 - segment.P0)) {
        return new IntersectionResult();
      }
      (bool p0_in, bool p1_in, bool origin_in) =
          (ray.Contains(segment.P0), ray.Contains(segment.P1), segment.Contains(ray.Origin));

      if (p0_in && p1_in) {
        return new IntersectionResult();
      }

      if (origin_in && p0_in) {
        return new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, segment.P0));
      }

      if (origin_in && p1_in) {
        return new IntersectionResult(LineSegment2D.FromPoints(ray.Origin, segment.P1));
      }

      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray2D ray, LineSegment2D segment) => segment.Overlaps(ray);

    public static IntersectionResult Overlap(this Ray2D ray, LineSegment2D segment) => segment.Overlap(ray);

    // Line and Ray 2D
    public static bool Overlaps(this Line2D line, Ray2D ray) => line.Overlap(ray).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Line2D line, Ray2D ray) {
      if (!ray.Direction.IsParallel(line.Direction)) {
        return new IntersectionResult();
      }

      if (line.Contains(ray.Origin)) {
        return new IntersectionResult(ray);
      }

      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray2D ray, Line2D line) => line.Overlaps(ray);

    public static IntersectionResult Overlap(this Ray2D ray, Line2D line) => line.Overlap(ray);

    // Triangle and Line 2D
    public static bool Overlaps(this Triangle2D triangle, Line2D line) => triangle.Overlap(line).ValueType !=
                                                                          typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle2D triangle, Line2D line) {
      return new IntersectionResult();
    }

    public static bool Overlaps(this Line2D line, Triangle2D triangle) => triangle.Overlaps(line);

    public static IntersectionResult Overlap(this Line2D line, Triangle2D triangle) => triangle.Overlap(line);

    // Triangle and ray 2D
    public static bool Overlaps(this Triangle2D triangle, Ray2D ray) => triangle.Overlap(ray).ValueType !=
                                                                        typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle2D triangle, Ray2D ray) {
      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray2D ray, Triangle2D triangle) => triangle.Overlaps(ray);

    public static IntersectionResult Overlap(this Ray2D ray, Triangle2D triangle) => triangle.Overlap(ray);

    // Triangle and LineSegment 2D

    public static bool Overlaps(this Triangle2D triangle, LineSegment2D segment) => triangle.Overlap(segment).ValueType
                                                                                    != typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle2D triangle, LineSegment2D segment) {
      return new IntersectionResult();
    }

    public static bool Overlaps(this LineSegment2D segment, Triangle2D triangle) => triangle.Overlaps(segment);

    public static IntersectionResult Overlap(this LineSegment2D segment,
                                             Triangle2D triangle) => triangle.Overlap(segment);
  }

}
