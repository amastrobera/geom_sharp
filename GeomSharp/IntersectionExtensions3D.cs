using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  public static class IntersectionExtensions3D {
    // intersection functions among different objects

    // LineSegment and Line 3D
    public static bool Intersects(this LineSegment3D segment, Line3D line) => segment.Intersection(line).ValueType !=
                                                                              typeof(NullValue);

    public static IntersectionResult Intersection(this LineSegment3D segment, Line3D line) {
      var line_inter = segment.ToLine().Intersection(line);
      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)line_inter.Value;
      if (segment.Contains(pI)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Line3D line, LineSegment3D segment) => segment.Intersects(line);

    public static IntersectionResult Intersection(this Line3D line,
                                                  LineSegment3D segment) => segment.Intersection(line);

    // LineSegment and Ray 3D
    public static bool Intersects(this LineSegment3D segment, Ray3D ray) => segment.Intersection(ray).ValueType !=
                                                                            typeof(NullValue);

    public static IntersectionResult Intersection(this LineSegment3D segment, Ray3D ray) {
      var line_inter = segment.ToLine().Intersection(ray.ToLine());
      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)line_inter.Value;
      if (segment.Contains(pI) && ray.Contains(pI)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Ray3D ray, LineSegment3D segment) => segment.Intersects(ray);

    public static IntersectionResult Intersection(this Ray3D ray, LineSegment3D segment) => segment.Intersection(ray);

    // Line and Ray 3D
    public static bool Intersects(this Line3D line, Ray3D ray) => line.Intersection(ray).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Line3D line, Ray3D ray) {
      var line_inter = line.Intersection(ray.ToLine());
      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)line_inter.Value;
      if (ray.Contains(pI)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Ray3D ray, Line3D line) => line.Intersects(line);

    public static IntersectionResult Intersection(this Ray3D ray, Line3D line) => line.Intersection(line);

    // Plane and LineSegment 3D

    /// <summary>
    /// Tells whether a line intersects the plane.
    /// It counts line segment that crosses the plane (it is split in two by the plane).
    /// It also counts a line whose extremity P0 (or P1) is contained in the plane.
    /// It does not count a line whose both extremities (P0 and P1) are contained in the plane (the segment belongs to
    /// the plane) segments
    /// </summary>
    /// <param name="segment"></param>
    /// <returns></returns>
    public static bool Intersects(this Plane plane, LineSegment3D segment) {
      if (plane.Contains(
              segment)) {  // calling 4 times the same distance function instead of 2: only for code readability
        return false;
      }

      // I examine all cases to avoid the overflow problem caused by the (more simple) DistanceTo(P0)*DistanceTo(P1) < 0
      (double d_this_other_p0, double d_this_other_p1) =
          (Math.Round(plane.SignedDistance(segment.P0), Constants.THREE_DECIMALS),
           Math.Round(plane.SignedDistance(segment.P1), Constants.THREE_DECIMALS));

      return (d_this_other_p0 >= 0 && d_this_other_p1 <= 0) || (d_this_other_p0 <= 0 && d_this_other_p1 >= 0);
    }

    public static IntersectionResult Intersection(this Plane plane, LineSegment3D segment) =>
        !plane.Intersects(segment)
            ? new IntersectionResult()
            : new IntersectionResult(plane.ProjectOnto(segment.P0, (segment.P1 - segment.P0).Normalize()));

    public static bool Intersects(this LineSegment3D segment, Plane plane) => plane.Intersects(segment);

    public static IntersectionResult Intersection(this LineSegment3D segment,
                                                  Plane plane) => plane.Intersection(segment);

    // Plane and Line 3D

    public static bool Intersects(this Plane plane, Line3D line) => plane.Intersection(line).ValueType !=
                                                                    typeof(NullValue);

    public static IntersectionResult Intersection(this Plane plane, Line3D line) {
      if (plane.Contains(line)) {
        return new IntersectionResult();
      }
      // Daniel Sunday's magic
      var U = line.Direction;
      var W = line.Origin - plane.Origin;
      var n = plane.Normal;

      double sI = -n.DotProduct(W) / n.DotProduct(U);

      Point3D q = line.Origin + sI * U;

      if (!plane.Contains(q)) {
        throw new Exception("plane.Intersection(Line3D) failed");
      }

      return new IntersectionResult(q);
    }

    public static bool Intersects(this Line3D line, Plane plane) => plane.Intersects(line);

    public static IntersectionResult Intersection(this Line3D line, Plane plane) => plane.Intersection(line);

    // Plane and Ray 3D
    public static bool Intersects(this Plane plane, Ray3D ray) => plane.Intersection(ray).ValueType !=
                                                                  typeof(NullValue);

    public static IntersectionResult Intersection(this Plane plane, Ray3D ray) {
      if (plane.Contains(ray)) {
        return new IntersectionResult();
      }
      // Daniel Sunday's magic
      var U = ray.Direction;
      var W = ray.Origin - plane.Origin;
      var n = plane.Normal;

      double sI = -n.DotProduct(W) / n.DotProduct(U);

      if (Math.Round(sI, Constants.NINE_DECIMALS) < 0) {
        return new IntersectionResult();
      }

      Point3D q = ray.Origin + sI * U;

      if (!plane.Contains(q)) {
        throw new Exception("plane.Intersection(Line3D) failed");
      }

      return new IntersectionResult(q);
    }

    public static bool Intersects(this Ray3D ray, Plane plane) => plane.Intersects(ray);

    public static IntersectionResult Intersection(this Ray3D ray, Plane plane) => plane.Intersection(ray);

    // Triangle and Line 3D
    public static bool Intersects(this Triangle3D triangle, Line3D line) => triangle.Intersection(line).ValueType !=
                                                                            typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle3D triangle, Line3D line) {
      var res = triangle.RefPlane().Intersection(line);
      if (res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)res.Value;

      if (triangle.Contains(pI)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Line3D line, Triangle3D triangle) => triangle.Intersects(line);

    public static IntersectionResult Intersection(this Line3D line, Triangle3D triangle) => triangle.Intersection(line);

    // Triangle and ray 3D
    public static bool Intersects(this Triangle3D triangle, Ray3D ray) => triangle.Intersection(ray).ValueType !=
                                                                          typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle3D triangle, Ray3D ray) {
      var res = triangle.RefPlane().Intersection(ray);
      if (res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)res.Value;

      if (triangle.Contains(pI) && ray.Contains(pI)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Ray3D ray, Triangle3D triangle) => triangle.Intersects(ray);

    public static IntersectionResult Intersection(this Ray3D ray, Triangle3D triangle) => triangle.Intersection(ray);

    // Triangle and LineSegment 3D

    public static bool Intersects(this Triangle3D triangle,
                                  LineSegment3D segment) => triangle.Intersection(segment).ValueType !=
                                                            typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle3D triangle, LineSegment3D segment) {
      var res = triangle.RefPlane().Intersection(segment);
      if (res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)res.Value;

      if (triangle.Contains(pI) && segment.Contains(pI)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this LineSegment3D segment, Triangle3D triangle) => triangle.Intersects(segment);

    public static IntersectionResult Intersection(this LineSegment3D segment,
                                                  Triangle3D triangle) => triangle.Intersection(segment);
  }

}
