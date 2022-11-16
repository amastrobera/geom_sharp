using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  public static class OverlapExtensions3D {
    // intersection functions among different objects

    // LineSegment and Line 3D
    public static bool Overlaps(this LineSegment3D segment,
                                Line3D line,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlap(line, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this LineSegment3D segment,
                                             Line3D line,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Direction.IsParallel(segment.P1 - segment.P0, decimal_precision) && line.Contains(segment.P0,
                                                                                               decimal_precision)
            ? new IntersectionResult(segment)
            : new IntersectionResult();

    public static bool Overlaps(this Line3D line,
                                LineSegment3D segment,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlaps(line, decimal_precision);

    public static IntersectionResult Overlap(this Line3D line,
                                             LineSegment3D segment,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlap(line, decimal_precision);

    // LineSegment and Ray 3D
    public static bool Overlaps(this LineSegment3D segment,
                                Ray3D ray,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlap(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this LineSegment3D segment,
                                             Ray3D ray,
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
                     : new IntersectionResult(LineSegment3D.FromPoints(ray.Origin, segment.P0));
        }

        if (p1_in) {
          return ray.Origin.AlmostEquals(segment.P1, decimal_precision)
                     ? new IntersectionResult(segment.P1)
                     : new IntersectionResult(LineSegment3D.FromPoints(ray.Origin, segment.P1));
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

    public static bool Overlaps(this Ray3D ray,
                                LineSegment3D segment,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlaps(ray, decimal_precision);

    public static IntersectionResult Overlap(this Ray3D ray,
                                             LineSegment3D segment,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Overlap(ray, decimal_precision);

    // Line and Ray 3D
    public static bool Overlaps(this Line3D line, Ray3D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Overlap(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Line3D line,
                                             Ray3D ray,
                                             int decimal_precision = Constants.THREE_DECIMALS) {
      if (!ray.Direction.IsParallel(line.Direction, decimal_precision)) {
        return new IntersectionResult();
      }

      if (line.Contains(ray.Origin, decimal_precision)) {
        return new IntersectionResult(ray);
      }

      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray3D ray, Line3D line, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Overlaps(ray, decimal_precision);

    public static IntersectionResult Overlap(this Ray3D ray,
                                             Line3D line,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Overlap(ray, decimal_precision);

    // Plane and Line 3D

    public static bool Overlaps(this Plane plane, Line3D line, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Overlap(line, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Plane plane,
                                             Line3D line,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Normal.IsPerpendicular(line.Direction, decimal_precision) && plane.Contains(line.Origin,
                                                                                          decimal_precision)
            ? new IntersectionResult(line)
            : new IntersectionResult();

    public static bool Overlaps(this Line3D line, Plane plane, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Overlaps(line, decimal_precision);

    public static IntersectionResult Overlap(this Line3D line,
                                             Plane plane,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Overlap(line, decimal_precision);

    // Plane and Ray 3D
    public static bool Overlaps(this Plane plane, Ray3D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Overlap(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Plane plane,
                                             Ray3D ray,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Normal.IsPerpendicular(ray.Direction, decimal_precision) && plane.Contains(ray.Origin, decimal_precision)
            ? new IntersectionResult(ray)
            : new IntersectionResult();

    public static bool Overlaps(this Ray3D ray, Plane plane, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Overlaps(ray, decimal_precision);

    public static IntersectionResult Overlap(this Ray3D ray,
                                             Plane plane,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Overlap(ray, decimal_precision);

    // Plane and LineSegment 3D
    public static bool Overlaps(this Plane plane,
                                LineSegment3D segment,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Overlap(segment, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Plane plane,
                                             LineSegment3D segment,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Normal.IsPerpendicular(segment.P1 - segment.P0, decimal_precision) && plane.Contains(segment.P0,
                                                                                                   decimal_precision)
            ? new IntersectionResult(segment)
            : new IntersectionResult();

    public static bool Overlaps(this LineSegment3D segment,
                                Plane plane,
                                int decimal_precision = Constants.THREE_DECIMALS) => plane.Overlaps(segment,
                                                                                                    decimal_precision);

    public static IntersectionResult Overlap(this LineSegment3D segment,
                                             Plane plane,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Overlap(segment, decimal_precision);

    // Plane and Triangle 3D
    public static bool Overlaps(this Plane plane,
                                Triangle3D triangle,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Overlap(triangle, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Plane plane,
                                             Triangle3D triangle,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.AlmostEquals(triangle.RefPlane(), decimal_precision) ? new IntersectionResult(triangle)
                                                                   : new IntersectionResult();
    public static bool Overlaps(this Triangle3D triangle,
                                Plane plane,
                                int decimal_precision = Constants.THREE_DECIMALS) => plane.Overlaps(triangle,
                                                                                                    decimal_precision);

    public static IntersectionResult Overlap(this Triangle3D triangle,
                                             Plane plane,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Overlap(triangle, decimal_precision);

    // Triangle and Line 3D
    public static bool Overlaps(this Triangle3D triangle,
                                Line3D line,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(line, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle3D triangle,
                                             Line3D line,
                                             int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = triangle.RefPlane();
      if (!ref_plane.Contains(line, decimal_precision)) {
        return new IntersectionResult();
      }
      // from 3D to 2D to compute the intersection points
      var triangle_2D = Triangle2D.FromPoints(ref_plane.ProjectInto(triangle.P0),
                                              ref_plane.ProjectInto(triangle.P1),
                                              ref_plane.ProjectInto(triangle.P2));

      var line_2D = Line2D.FromPoints(ref_plane.ProjectInto(line.Origin),
                                         ref_plane.ProjectInto(line.Origin + 2 * line.Direction));

      // from 2D back to 3D
      // if intersection in 2D, return it
      var inter_2D = triangle_2D.Intersection(line_2D, decimal_precision);
      if (inter_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)inter_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (inter_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)inter_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // also if there overlap in 2D, return it
      var ovrl_2D = triangle_2D.Overlap(line_2D, decimal_precision);
      if (ovrl_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)ovrl_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (ovrl_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)ovrl_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // no valid intersection
      return new IntersectionResult();
    }

    public static bool Overlaps(this Line3D line,
                                Triangle3D triangle,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlaps(line, decimal_precision);

    public static IntersectionResult Overlap(this Line3D line,
                                             Triangle3D triangle,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(line, decimal_precision);

    // Triangle and ray 3D
    public static bool Overlaps(this Triangle3D triangle,
                                Ray3D ray,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle3D triangle,
                                             Ray3D ray,
                                             int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = triangle.RefPlane();
      if (!ref_plane.Contains(ray, decimal_precision)) {
        return new IntersectionResult();
      }
      // from 3D to 2D to compute the intersection points
      var triangle_2D = Triangle2D.FromPoints(ref_plane.ProjectInto(triangle.P0),
                                              ref_plane.ProjectInto(triangle.P1),
                                              ref_plane.ProjectInto(triangle.P2));

      var orig_2D = ref_plane.ProjectInto(ray.Origin);
      var ray_2D = new Ray2D(orig_2D, (ref_plane.ProjectInto(ray.Origin + 2 * ray.Direction) - orig_2D).Normalize());

      // from 2D back to 3D
      // if 2D intersection, return it
      var inter_2D = triangle_2D.Intersection(ray_2D, decimal_precision);
      if (inter_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)inter_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (inter_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)inter_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // also if 2D overlap, return it
      var ovrl_2D = triangle_2D.Overlap(ray_2D, decimal_precision);
      if (ovrl_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)ovrl_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (ovrl_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)ovrl_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      //  no valid intersection
      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray3D ray,
                                Triangle3D triangle,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlaps(ray, decimal_precision);

    public static IntersectionResult Overlap(this Ray3D ray,
                                             Triangle3D triangle,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(ray, decimal_precision);

    // Triangle and LineSegment 3D

    public static bool Overlaps(this Triangle3D triangle,
                                LineSegment3D segment,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(segment, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle3D triangle,
                                             LineSegment3D segment,
                                             int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = triangle.RefPlane();
      if (!ref_plane.Contains(segment, decimal_precision)) {
        return new IntersectionResult();
      }
      // from 3D to 2D to compute the intersection points
      var triangle_2D = Triangle2D.FromPoints(ref_plane.ProjectInto(triangle.P0),
                                              ref_plane.ProjectInto(triangle.P1),
                                              ref_plane.ProjectInto(triangle.P2));

      var segment_2D = LineSegment2D.FromPoints(ref_plane.ProjectInto(segment.P0), ref_plane.ProjectInto(segment.P1));

      // from 2D back to 3D
      // if intersection 2D, return it
      var inter_2D = triangle_2D.Intersection(segment_2D, decimal_precision);
      if (inter_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)inter_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (inter_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)inter_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // also if overlap, return it
      var ovlp_2D = triangle_2D.Overlap(segment_2D, decimal_precision);
      if (ovlp_2D.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)ovlp_2D.Value);
        return new IntersectionResult(p_3d);
      } else if (ovlp_2D.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)ovlp_2D.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // no valid intersection
      return new IntersectionResult();
    }

    public static bool Overlaps(this LineSegment3D segment,
                                Triangle3D triangle,
                                int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlaps(segment, decimal_precision);

    public static IntersectionResult Overlap(this LineSegment3D segment,
                                             Triangle3D triangle,
                                             int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Overlap(segment, decimal_precision);
  }

}
