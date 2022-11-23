using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

  public static class IntersectionExtensions3D {
    // intersection functions among different objects

    // LineSegment and Line 3D
    public static bool Intersects(this LineSegment3D segment,
                                  Line3D line,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersection(line, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this LineSegment3D segment,
                                                  Line3D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var line_inter = segment.ToLine().Intersection(line, decimal_precision);
      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)line_inter.Value;
      if (segment.Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Line3D line,
                                  LineSegment3D segment,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersects(line, decimal_precision);

    public static IntersectionResult Intersection(this Line3D line,
                                                  LineSegment3D segment,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersection(line, decimal_precision);

    // LineSegment and Ray 3D
    public static bool Intersects(this LineSegment3D segment,
                                  Ray3D ray,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this LineSegment3D segment,
                                                  Ray3D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var line_inter = segment.ToLine().Intersection(ray.ToLine(), decimal_precision);
      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)line_inter.Value;
      if (segment.Contains(pI, decimal_precision) && ray.Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Ray3D ray,
                                  LineSegment3D segment,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersects(ray, decimal_precision);

    public static IntersectionResult Intersection(this Ray3D ray,
                                                  LineSegment3D segment,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.Intersection(ray, decimal_precision);

    // Line and Ray 3D
    public static bool Intersects(this Line3D line, Ray3D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Line3D line,
                                                  Ray3D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var line_inter = line.Intersection(ray.ToLine(), decimal_precision);
      if (line_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)line_inter.Value;
      if (ray.Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Ray3D ray, Line3D line, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Intersects(line, decimal_precision);

    public static IntersectionResult Intersection(this Ray3D ray,
                                                  Line3D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Intersection(line, decimal_precision);

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
    public static bool Intersects(this Plane plane,
                                  LineSegment3D segment,
                                  int decimal_precision = Constants.THREE_DECIMALS) {
      if (plane.Contains(segment, decimal_precision)) {  // calling 4 times the same distance function instead of 2:
                                                         // only for code readability
        return false;
      }

      // I examine all cases to avoid the overflow problem caused by the (more simple) DistanceTo(P0)*DistanceTo(P1) < 0
      (double d_this_other_p0, double d_this_other_p1) =
          (Math.Round(plane.SignedDistance(segment.P0), decimal_precision),
           Math.Round(plane.SignedDistance(segment.P1), decimal_precision));

      return (d_this_other_p0 >= 0 && d_this_other_p1 <= 0) || (d_this_other_p0 <= 0 && d_this_other_p1 >= 0);
    }

    public static IntersectionResult Intersection(this Plane plane,
                                                  LineSegment3D segment,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        !plane.Intersects(segment, decimal_precision)
            ? new IntersectionResult()
            : new IntersectionResult(plane.ProjectOnto(segment.P0, (segment.P1 - segment.P0).Normalize()));

    public static bool Intersects(this LineSegment3D segment,
                                  Plane plane,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersects(segment, decimal_precision);

    public static IntersectionResult Intersection(this LineSegment3D segment,
                                                  Plane plane,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(segment, decimal_precision);

    // Plane and Line 3D

    public static bool Intersects(this Plane plane, Line3D line, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(line, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Plane plane,
                                                  Line3D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      if (plane.Contains(line, decimal_precision)) {
        return new IntersectionResult();
      }
      // Daniel Sunday's magic
      var U = line.Direction;
      var W = line.Origin - plane.Origin;
      var n = plane.Normal;

      double sI = -n.DotProduct(W) / n.DotProduct(U);

      Point3D q = line.Origin + sI * U;

      if (!plane.Contains(q, decimal_precision)) {
        throw new Exception("plane.Intersection(Line3D) failed");
      }

      return new IntersectionResult(q);
    }

    public static bool Intersects(this Line3D line, Plane plane, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersects(line, decimal_precision);

    public static IntersectionResult Intersection(this Line3D line,
                                                  Plane plane,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(line, decimal_precision);

    // Plane and Triangle 3D
    public static bool Intersects(this Plane plane,
                                  Triangle3D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(triangle, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Plane plane,
                                                  Triangle3D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var plane_inter = plane.Intersection(triangle.RefPlane());
      if (plane_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var line_inter = (Line3D)plane_inter.Value;

      return line_inter.Overlap(triangle);
    }

    public static bool Intersects(this Triangle3D triangle,
                                  Plane plane,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersects(triangle, decimal_precision);

    public static IntersectionResult Intersection(this Triangle3D triangle,
                                                  Plane plane,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(triangle, decimal_precision);

    // Plane and Ray 3D
    public static bool Intersects(this Plane plane, Ray3D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Plane plane,
                                                  Ray3D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      if (plane.Contains(ray, decimal_precision)) {
        return new IntersectionResult();
      }
      // Daniel Sunday's magic
      var U = ray.Direction;
      var W = ray.Origin - plane.Origin;
      var n = plane.Normal;

      double sI = -n.DotProduct(W) / n.DotProduct(U);

      if (Math.Round(sI, decimal_precision) < 0) {
        return new IntersectionResult();
      }

      Point3D q = ray.Origin + sI * U;

      if (!plane.Contains(q, decimal_precision)) {
        throw new Exception("plane.Intersection(Line3D) failed");
      }

      return new IntersectionResult(q);
    }

    public static bool Intersects(this Ray3D ray, Plane plane, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersects(ray, decimal_precision);

    public static IntersectionResult Intersection(this Ray3D ray,
                                                  Plane plane,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(ray, decimal_precision);

    // Plane and Polyline

    public static bool Intersects(this Plane plane,
                                  Polyline3D pline,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Plane plane,
                                                  Polyline3D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point3D>();
      int n = pline.Size;
      for (int i = 0; i < n - 1; ++i) {
        var pline_seg = LineSegment3D.FromPoints(pline[i], pline[i + 1], decimal_precision);
        var inter = plane.Intersection(pline_seg, decimal_precision);
        if (inter.ValueType == typeof(Point3D)) {
          mpoint.Add((Point3D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet3D(mpoint)) : new IntersectionResult();
    }

    public static bool Intersects(this Polyline3D segment,
                                  Plane plane,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersects(segment, decimal_precision);

    public static IntersectionResult Intersection(this Polyline3D segment,
                                                  Plane plane,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(segment, decimal_precision);

    // Plane and Polygon

    public static bool Intersects(this Plane plane, Polygon3D poly, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(poly, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Plane plane,
                                                  Polygon3D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var poly_plane = poly.RefPlane();
      var plane_inter = poly_plane.Intersection(plane, decimal_precision);
      if (plane_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }
      var plane_line = (Line3D)plane_inter.Value;

      // change the problem into a 2D one
      //      intersect in 2D one polygon with the line, the result is either empty or a multi line set
      var poly_2D = new Polygon2D(poly.Select(p => poly_plane.ProjectInto(p)), decimal_precision);
      var plane_line_2D = Line2D.FromPoints(poly_plane.ProjectInto(plane_line.P0),
                                            poly_plane.ProjectInto(plane_line.P1),
                                            decimal_precision);
      var poly_inter_set_2D = poly_2D.Intersection(plane_line_2D, decimal_precision);
      if (poly_inter_set_2D.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      if (poly_inter_set_2D.ValueType == typeof(LineSegment2D)) {
        return new IntersectionResult(poly_plane.Evaluate((LineSegment2D)poly_inter_set_2D.Value));
      }

      if (poly_inter_set_2D.ValueType == typeof(LineSegmentSet2D)) {
        var mline = new List<LineSegment3D>();
        foreach (var seg in (LineSegmentSet2D)poly_inter_set_2D.Value) {
          mline.Add(poly_plane.Evaluate(seg));
        }
        return new IntersectionResult(new LineSegmentSet3D(mline));
      }

      throw new Exception("Plane to Polygon intersection, unkown return type " +
                          poly_inter_set_2D.ValueType.ToString());
    }

    public static bool Intersects(this Polygon3D poly, Plane plane, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersects(poly, decimal_precision);

    public static IntersectionResult Intersection(this Polygon3D poly,
                                                  Plane plane,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Intersection(poly, decimal_precision);

    // Triangle and Line 3D
    public static bool Intersects(this Triangle3D triangle,
                                  Line3D line,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(line, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle3D triangle,
                                                  Line3D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var res = triangle.RefPlane().Intersection(line, decimal_precision);
      if (res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)res.Value;

      if (triangle.Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Line3D line,
                                  Triangle3D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersects(line, decimal_precision);

    public static IntersectionResult Intersection(this Line3D line,
                                                  Triangle3D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(line, decimal_precision);

    // Triangle and ray 3D
    public static bool Intersects(this Triangle3D triangle,
                                  Ray3D ray,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle3D triangle,
                                                  Ray3D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var res = triangle.RefPlane().Intersection(ray, decimal_precision);
      if (res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)res.Value;

      if (triangle.Contains(pI, decimal_precision) && ray.Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Ray3D ray,
                                  Triangle3D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersects(ray, decimal_precision);

    public static IntersectionResult Intersection(this Ray3D ray,
                                                  Triangle3D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(ray, decimal_precision);

    // Triangle and LineSegment 3D

    public static bool Intersects(this Triangle3D triangle,
                                  LineSegment3D segment,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(segment, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle3D triangle,
                                                  LineSegment3D segment,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var res = triangle.RefPlane().Intersection(segment, decimal_precision);
      if (res.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var pI = (Point3D)res.Value;

      if (triangle.Contains(pI, decimal_precision) && segment.Contains(pI, decimal_precision)) {
        return new IntersectionResult(pI);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this LineSegment3D segment,
                                  Triangle3D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersects(segment, decimal_precision);

    public static IntersectionResult Intersection(this LineSegment3D segment,
                                                  Triangle3D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(segment, decimal_precision);

    // Polyline to Line
    public static bool Intersects(this Polyline3D pline,
                                  Line3D line,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(line, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polyline3D pline,
                                                  Line3D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point3D>();
      int n = pline.Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg = LineSegment3D.FromPoints(pline[i1], pline[i2], decimal_precision);
        var inter = seg.Intersection(line, decimal_precision);
        if (inter.ValueType == typeof(Point3D)) {
          mpoint.Add((Point3D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet3D(mpoint)) : new IntersectionResult();
    }

    public static bool Intersects(this Line3D line,
                                  Polyline3D pline,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Line3D line,
                                                  Polyline3D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(line, decimal_precision);

    // Polyline to Ray
    public static bool Intersects(this Polyline3D pline, Ray3D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polyline3D pline,
                                                  Ray3D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point3D>();
      int n = pline.Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg = LineSegment3D.FromPoints(pline[i1], pline[i2], decimal_precision);
        var inter = seg.Intersection(ray, decimal_precision);
        if (inter.ValueType == typeof(Point3D)) {
          mpoint.Add((Point3D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet3D(mpoint)) : new IntersectionResult();
    }

    public static bool Intersects(this Ray3D ray, Polyline3D pline, int decimal_precision = Constants.THREE_DECIMALS) =>
        ray.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Ray3D ray,
                                                  Polyline3D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(ray, decimal_precision);

    // Polyline to LineSegment
    public static bool Intersects(this Polyline3D pline,
                                  LineSegment3D seg,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(seg, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polyline3D pline,
                                                  LineSegment3D seg,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var mpoint = new List<Point3D>();
      int n = pline.Size;
      for (int i = 0; i < n - 1; ++i) {
        int i1 = i;
        int i2 = i + 1;
        var seg2 = LineSegment3D.FromPoints(pline[i1], pline[i2], decimal_precision);
        var inter = seg2.Intersection(seg, decimal_precision);
        if (inter.ValueType == typeof(Point3D)) {
          mpoint.Add((Point3D)inter.Value);
        }
      }

      return (mpoint.Count > 0) ? new IntersectionResult(new PointSet3D(mpoint)) : new IntersectionResult();
    }

    public static bool Intersects(this LineSegment3D seg,
                                  Polyline3D pline,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        seg.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this LineSegment3D seg,
                                                  Polyline3D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(seg, decimal_precision);

    // Triangle and Polyline
    public static bool Intersects(this Polyline3D pline,
                                  Triangle3D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(triangle, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polyline3D pline,
                                                  Triangle3D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      throw new NotImplementedException();
    }

    public static bool Intersects(this Triangle3D triangle,
                                  Polyline3D pline,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle3D triangle,
                                                  Polyline3D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(triangle, decimal_precision);

    // Polygon to Line
    public static bool Intersects(this Polygon3D poly, Line3D line, int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(line, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polygon3D poly,
                                                  Line3D line,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = poly.RefPlane();
      var plane_inter = ref_plane.Intersection(line, decimal_precision);
      if (plane_inter.ValueType != typeof(Point3D)) {
        return new IntersectionResult();
      }

      var plane_point_inter = (Point3D)plane_inter.Value;

      if (poly.Contains(plane_point_inter, decimal_precision)) {
        return new IntersectionResult(plane_point_inter);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Line3D line, Polygon3D poly, int decimal_precision = Constants.THREE_DECIMALS) =>
        line.Intersection(poly, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Line3D line,
                                                  Polygon3D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(line, decimal_precision);

    // Polygon to Ray
    public static bool Intersects(this Polygon3D poly, Ray3D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(ray, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polygon3D poly,
                                                  Ray3D ray,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = poly.RefPlane();
      var plane_inter = ref_plane.Intersection(ray, decimal_precision);
      if (plane_inter.ValueType != typeof(Point3D)) {
        return new IntersectionResult();
      }

      var plane_point_inter = (Point3D)plane_inter.Value;

      if (poly.Contains(plane_point_inter, decimal_precision)) {
        return new IntersectionResult(plane_point_inter);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this Ray3D ray, Polygon3D poly, int decimal_precision = Constants.THREE_DECIMALS) =>
        ray.Intersection(poly, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Ray3D ray,
                                                  Polygon3D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(ray, decimal_precision);

    // Polygon to LineSegment
    public static bool Intersects(this Polygon3D poly,
                                  LineSegment3D seg,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(seg, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polygon3D poly,
                                                  LineSegment3D seg,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = poly.RefPlane();
      var plane_inter = ref_plane.Intersection(seg, decimal_precision);
      if (plane_inter.ValueType != typeof(Point3D)) {
        return new IntersectionResult();
      }

      var plane_point_inter = (Point3D)plane_inter.Value;

      if (poly.Contains(plane_point_inter, decimal_precision)) {
        return new IntersectionResult(plane_point_inter);
      }

      return new IntersectionResult();
    }

    public static bool Intersects(this LineSegment3D seg,
                                  Polygon3D poly,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        seg.Intersection(poly, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this LineSegment3D seg,
                                                  Polygon3D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(seg, decimal_precision);

    // Polygon to Polyline
    public static bool Intersects(this Polygon3D poly,
                                  Polyline3D pline,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(pline, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polygon3D poly,
                                                  Polyline3D pline,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = poly.RefPlane();
      var plane_inter = ref_plane.Intersection(pline, decimal_precision);

      if (plane_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      if (plane_inter.ValueType == typeof(Point3D)) {
        var plane_point_inter = (Point3D)plane_inter.Value;

        if (poly.Contains(plane_point_inter, decimal_precision)) {
          return new IntersectionResult(plane_point_inter);
        }
        return new IntersectionResult();
      }

      if (plane_inter.ValueType == typeof(PointSet3D)) {
        var inter_set = (PointSet3D)plane_inter.Value;

        var mpoint = inter_set.Where(p => poly.Contains(p, decimal_precision));

        if (mpoint.Count() == 0) {
          return new IntersectionResult();
        }

        return new IntersectionResult(new PointSet3D(mpoint));
      }

      throw new ArithmeticException("unknown intsection type of polygon to polyline: " +
                                    plane_inter.ValueType.ToString());
    }

    public static bool Intersects(this Polyline3D pline,
                                  Polygon3D poly,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        pline.Intersection(poly, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polyline3D pline,
                                                  Polygon3D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(pline, decimal_precision);

    // Triangle and Polygon
    public static bool Intersects(this Polygon3D poly,
                                  Triangle3D triangle,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(triangle, decimal_precision).ValueType != typeof(NullValue);
    public static IntersectionResult Intersection(this Polygon3D poly,
                                                  Triangle3D triangle,
                                                  int decimal_precision = Constants.THREE_DECIMALS) {
      throw new NotImplementedException();
    }

    public static bool Intersects(this Triangle3D triangle,
                                  Polygon3D poly,
                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        triangle.Intersection(poly, decimal_precision).ValueType != typeof(NullValue);

    public static IntersectionResult Intersection(this Triangle3D triangle,
                                                  Polygon3D poly,
                                                  int decimal_precision = Constants.THREE_DECIMALS) =>
        poly.Intersection(triangle, decimal_precision);
  }

}
