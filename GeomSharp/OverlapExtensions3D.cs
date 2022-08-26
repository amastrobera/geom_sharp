using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  public static class OverlapExtensions3D {
    // intersection functions among different objects

    // LineSegment and Line 3D
    public static bool Overlaps(this LineSegment3D segment, Line3D line) => segment.Overlap(line).ValueType !=
                                                                            typeof(NullValue);

    public static IntersectionResult Overlap(this LineSegment3D segment, Line3D line) =>
        line.Direction.IsParallel(segment.P1 - segment.P0) && line.Contains(segment.P0)
            ? new IntersectionResult(segment)
            : new IntersectionResult();

    public static bool Overlaps(this Line3D line, LineSegment3D segment) => segment.Overlaps(line);

    public static IntersectionResult Overlap(this Line3D line, LineSegment3D segment) => segment.Overlap(line);

    // LineSegment and Ray 3D
    public static bool Overlaps(this LineSegment3D segment, Ray3D ray) => segment.Overlap(ray).ValueType !=
                                                                          typeof(NullValue);

    public static IntersectionResult Overlap(this LineSegment3D segment, Ray3D ray) {
      if (!ray.Direction.IsParallel(segment.P1 - segment.P0)) {
        return new IntersectionResult();
      }
      (bool p0_in, bool p1_in, bool origin_in) =
          (ray.Contains(segment.P0), ray.Contains(segment.P1), segment.Contains(ray.Origin));

      if (p0_in && p1_in) {
        return new IntersectionResult();
      }

      if (origin_in && p0_in) {
        return new IntersectionResult(LineSegment3D.FromPoints(ray.Origin, segment.P0));
      }

      if (origin_in && p1_in) {
        return new IntersectionResult(LineSegment3D.FromPoints(ray.Origin, segment.P1));
      }

      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray3D ray, LineSegment3D segment) => segment.Overlaps(ray);

    public static IntersectionResult Overlap(this Ray3D ray, LineSegment3D segment) => segment.Overlap(ray);

    // Line and Ray 3D
    public static bool Overlaps(this Line3D line, Ray3D ray) => line.Overlap(ray).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Line3D line, Ray3D ray) {
      if (!ray.Direction.IsParallel(line.Direction)) {
        return new IntersectionResult();
      }

      if (line.Contains(ray.Origin)) {
        return new IntersectionResult(ray);
      }

      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray3D ray, Line3D line) => line.Overlaps(ray);

    public static IntersectionResult Overlap(this Ray3D ray, Line3D line) => line.Overlap(ray);

    // Plane and Line 3D

    public static bool Overlaps(this Plane plane, Line3D line) => plane.Overlap(line).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Plane plane, Line3D line) =>
        plane.Normal.IsPerpendicular(line.Direction) && plane.Contains(line.Origin) ? new IntersectionResult(line)
                                                                                    : new IntersectionResult();

    public static bool Overlaps(this Line3D line, Plane plane) => plane.Overlaps(line);

    public static IntersectionResult Overlap(this Line3D line, Plane plane) => plane.Overlap(line);

    // Plane and Ray 3D
    public static bool Overlaps(this Plane plane, Ray3D ray) => plane.Overlap(ray).ValueType != typeof(NullValue);

    public static IntersectionResult Overlap(this Plane plane, Ray3D ray) =>
        plane.Normal.IsPerpendicular(ray.Direction) && plane.Contains(ray.Origin) ? new IntersectionResult(ray)
                                                                                  : new IntersectionResult();

    public static bool Overlaps(this Ray3D ray, Plane plane) => plane.Overlaps(ray);

    public static IntersectionResult Overlap(this Ray3D ray, Plane plane) => plane.Overlap(ray);

    // Plane and LineSegment 3D

    public static bool Overlaps(this Plane plane, LineSegment3D segment) => plane.Overlap(segment).ValueType !=
                                                                            typeof(NullValue);

    public static IntersectionResult Overlap(this Plane plane, LineSegment3D segment) =>
        plane.Normal.IsPerpendicular(segment.P1 - segment.P0) && plane.Contains(segment.P0)
            ? new IntersectionResult(segment)
            : new IntersectionResult();

    public static bool Overlaps(this LineSegment3D segment, Plane plane) => plane.Overlaps(segment);

    public static IntersectionResult Overlap(this LineSegment3D segment, Plane plane) => plane.Overlap(segment);

    // Triangle and Line 3D
    public static bool Overlaps(this Triangle3D triangle, Line3D line) => triangle.Overlap(line).ValueType !=
                                                                          typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle3D triangle, Line3D line) {
      var ref_plane = triangle.RefPlane();
      if (!ref_plane.Contains(line)) {
        return new IntersectionResult();
      }
      // from 3D to 2D to compute the intersection points
      var triangle_2d = Triangle2D.FromPoints(ref_plane.ProjectInto(triangle.P0),
                                              ref_plane.ProjectInto(triangle.P1),
                                              ref_plane.ProjectInto(triangle.P2));

      var line_2d = Line2D.FromTwoPoints(ref_plane.ProjectInto(line.Origin),
                                         ref_plane.ProjectInto(line.Origin + 2 * line.Direction));

      // from 2D back to 3D
      var inter_2d = triangle_2d.Intersection(line_2d);
      if (inter_2d.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)inter_2d.Value);
        return new IntersectionResult(p_3d);
      } else if (inter_2d.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)inter_2d.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // no valid intersection
      return new IntersectionResult();
    }

    public static bool Overlaps(this Line3D line, Triangle3D triangle) => triangle.Overlaps(line);

    public static IntersectionResult Overlap(this Line3D line, Triangle3D triangle) => triangle.Overlap(line);

    // Triangle and ray 3D
    public static bool Overlaps(this Triangle3D triangle, Ray3D ray) => triangle.Overlap(ray).ValueType !=
                                                                        typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle3D triangle, Ray3D ray) {
      var ref_plane = triangle.RefPlane();
      if (!ref_plane.Contains(ray)) {
        return new IntersectionResult();
      }
      // from 3D to 2D to compute the intersection points
      var triangle_2d = Triangle2D.FromPoints(ref_plane.ProjectInto(triangle.P0),
                                              ref_plane.ProjectInto(triangle.P1),
                                              ref_plane.ProjectInto(triangle.P2));

      var orig_2d = ref_plane.ProjectInto(ray.Origin);
      var ray_2d = new Ray2D(orig_2d, (ref_plane.ProjectInto(ray.Origin + 2 * ray.Direction) - orig_2d).Normalize());

      // from 2D back to 3D
      var inter_2d = triangle_2d.Intersection(ray_2d);
      if (inter_2d.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)inter_2d.Value);
        return new IntersectionResult(p_3d);
      } else if (inter_2d.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)inter_2d.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // no valid intersection
      return new IntersectionResult();
    }

    public static bool Overlaps(this Ray3D ray, Triangle3D triangle) => triangle.Overlaps(ray);

    public static IntersectionResult Overlap(this Ray3D ray, Triangle3D triangle) => triangle.Overlap(ray);

    // Triangle and LineSegment 3D

    public static bool Overlaps(this Triangle3D triangle, LineSegment3D segment) => triangle.Overlap(segment).ValueType
                                                                                    != typeof(NullValue);

    public static IntersectionResult Overlap(this Triangle3D triangle, LineSegment3D segment) {
      var ref_plane = triangle.RefPlane();
      if (!ref_plane.Contains(segment)) {
        return new IntersectionResult();
      }
      // from 3D to 2D to compute the intersection points
      var triangle_2d = Triangle2D.FromPoints(ref_plane.ProjectInto(triangle.P0),
                                              ref_plane.ProjectInto(triangle.P1),
                                              ref_plane.ProjectInto(triangle.P2));

      var segment_2d = LineSegment2D.FromPoints(ref_plane.ProjectInto(segment.P0), ref_plane.ProjectInto(segment.P1));

      // from 2D back to 3D
      var inter_2d = triangle_2d.Intersection(segment_2d);
      if (inter_2d.ValueType == typeof(Point2D)) {
        var p_3d = ref_plane.Evaluate((Point2D)inter_2d.Value);
        return new IntersectionResult(p_3d);
      } else if (inter_2d.ValueType == typeof(LineSegment2D)) {
        var line_inter = (LineSegment2D)inter_2d.Value;
        (var p0_3d, var p1_3d) = (ref_plane.Evaluate(line_inter.P0), ref_plane.Evaluate(line_inter.P1));
        return new IntersectionResult(LineSegment3D.FromPoints(p0_3d, p1_3d));
      }

      // no valid intersection
      return new IntersectionResult();
    }

    public static bool Overlaps(this LineSegment3D segment, Triangle3D triangle) => triangle.Overlaps(segment);

    public static IntersectionResult Overlap(this LineSegment3D segment,
                                             Triangle3D triangle) => triangle.Overlap(segment);
  }

}
