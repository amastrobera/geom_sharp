// internal
using GeomSharp;

// external
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {
  /// <summary>
  /// tests for all the MathNet.Spatial.Euclidean functions I created
  /// </summary>
  [TestClass]
  public class IntersectionExtensions2DTests {
    // Line, LineSegment, Ray cross test
    [RepeatedTestMethod(100)]
    public void LineToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine2D(decimal_precision: precision);

      if (line is null) {
        return;
      }

      var mid = Point2D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = line.Direction;

      // temp data
      Ray2D ray;
      UnitVector2D u_perp = u.Perp().Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      ray = new Ray2D(mid - 2 * u_perp, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray2D(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (p0)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray2D(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (p1)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going down
      ray = new Ray2D(mid, -u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (to mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going up
      ray = new Ray2D(mid, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (from mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going down
      ray = new Ray2D(p0, -u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (to p0)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going up
      ray = new Ray2D(p0, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (from p0)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the second extremity, going up
      ray = new Ray2D(p1, -u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (from p1)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the second extremity, going down
      ray = new Ray2D(p1, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (to p1)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector2D shift = RandomGenerator.MakeVector2D(decimal_precision: precision);
      ray = new Ray2D(p0 + 2 * shift, u);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + line.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      //      downwards
      ray = new Ray2D(p0 - 2 * shift, -u);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + line.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray2D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (mid-)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray2D(p0 - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (p0-)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray2D(p1 - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (p1-)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      in the middle, crossing
      ray = new Ray2D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (mid+)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray2D(p0 + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (p0+)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray2D(p1 + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (p1+)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void LineToLineSegment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var segment, var p0, var p1) = RandomGenerator.MakeLineSegment2D(decimal_precision: precision);

      if (segment is null) {
        return;
      }

      var mid = Point2D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = segment.ToLine().Direction;

      // temp data
      Line2D line;
      UnitVector2D u_perp = u.Perp().Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      line = Line2D.FromDirection(mid - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(line, precision),
                    "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      on the first extremity, crossing
      line = Line2D.FromDirection(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(line, precision),
                    "intersect forrreal (p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      on the second extremity, crossing
      line = Line2D.FromDirection(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(line, precision),
                    "intersect forrreal (p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the middle, going down
      line = Line2D.FromDirection(mid, -u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the middle, going up
      line = Line2D.FromDirection(mid, u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (from mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the first extremity, going down
      line = Line2D.FromDirection(p0, -u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the first extremity, going up
      line = Line2D.FromDirection(p0, u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the second extremity, going up
      line = Line2D.FromDirection(p1, -u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the second extremity, going down
      line = Line2D.FromDirection(p1, u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (to p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector2D shift = RandomGenerator.MakeVector2D();
      line = Line2D.FromDirection(p0 + 2 * shift, u);
      Assert.IsFalse(segment.Intersects(line, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + line.ToWkt(precision));

      //      downwards
      line = Line2D.FromDirection(p0 - 2 * shift, -u);
      Assert.IsFalse(segment.Intersects(line, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + line.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void LineSegmentToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var segment, var p0, var p1) = RandomGenerator.MakeLineSegment2D(decimal_precision: precision);

      if (segment is null) {
        return;
      }

      var mid = Point2D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = segment.ToLine().Direction;

      // temp data
      Ray2D ray;
      UnitVector2D u_perp = u.Perp().Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      ray = new Ray2D(mid - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(ray, precision),
                    "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray2D(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(ray, precision),
                    "intersect forrreal (p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray2D(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(ray, precision),
                    "intersect forrreal (p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going down
      ray = new Ray2D(mid, -u_perp);
      Assert.IsTrue(
          segment.Intersects(ray, precision),
          "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going up
      ray = new Ray2D(mid, u_perp);
      Assert.IsTrue(
          segment.Intersects(ray, precision),
          "intersect forrreal (from mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going down
      ray = new Ray2D(p0, -u_perp);
      Assert.IsTrue(segment.Intersects(ray, precision),
                    "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going up
      ray = new Ray2D(p0, u_perp);
      Assert.IsTrue(
          segment.Intersects(ray, precision),
          "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the second extremity, going up
      ray = new Ray2D(p1, -u_perp);
      Assert.IsTrue(
          segment.Intersects(ray, precision),
          "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the second extremity, going down
      ray = new Ray2D(p1, u_perp);
      Assert.IsTrue(segment.Intersects(ray, precision),
                    "intersect forrreal (to p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector2D shift = RandomGenerator.MakeVector2D(decimal_precision: precision);
      ray = new Ray2D(p0 + 2 * shift, u);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      //      downwards
      ray = new Ray2D(p0 - 2 * shift, -u);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray2D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (mid-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray2D(p0 - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (p0-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray2D(p1 - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (p1-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      in the middle, crossing
      ray = new Ray2D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (mid+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray2D(p0 + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (p0+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray2D(p1 + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (p1+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));
    }

    [Ignore]
    [RepeatedTestMethod(1)]
    public void LineToPolyline() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void RayToPolyline() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void LineSegmentToPolyline() {}

    // Triangle with other basic primitives

    [RepeatedTestMethod(100)]
    public void TriangleToLine() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_triangle = RandomGenerator.MakeTriangle2D(decimal_precision: precision);
      var t = random_triangle.Triangle;
      (var p0, var p1, var p2) = (random_triangle.p0, random_triangle.p1, random_triangle.p2);

      if (t is null) {
        return;
      }

      // temp data
      Point2D cm = t.CenterOfMass();
      Line2D line;
      Point2D lp0, lp1;
      UnitVector2D u;
      UnitVector2D u_perp;

      // case 1: intersect
      //    perpendicular to an edge
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      u = (t.P1 - t.P0).Normalize();
      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsTrue(line.Intersects(t, precision),
                    "line through perp on p0-p1 edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      u = (t.P2 - t.P1).Normalize();
      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsTrue(line.Intersects(t, precision),
                    "line through perp on p1-p2 edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      u = (t.P0 - t.P2).Normalize();
      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsTrue(line.Intersects(t, precision),
                    "line through perp on p2-p0 edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      //    strike through two edges
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(line.Intersects(t, precision),
                    "line through mid points of edges t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(line.Intersects(t, precision),
                    "line through mid points of edges t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(line.Intersects(t, precision),
                    "line through mid points of edges t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      //    strike through one point
      line = Line2D.FromDirection(t.P0, (t.P1 - t.P2).Normalize());
      Assert.IsTrue(line.Intersects(t, precision),
                    "line through a vertex t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      line = Line2D.FromDirection(t.P1, (t.P2 - t.P0).Normalize());
      Assert.IsTrue(line.Intersects(t, precision),
                    "line through a vertex t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      line = Line2D.FromDirection(t.P2, (t.P1 - t.P0).Normalize());
      Assert.IsTrue(line.Intersects(t, precision),
                    "line through a vertex t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      // case 2: parallel (no intersect)
      line = Line2D.FromDirection(t.P0 + (t.P0 - cm), (t.P1 - t.P2).Normalize());
      Assert.IsFalse(line.Intersects(t, precision),
                     "external line parallel to an edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      line = Line2D.FromDirection(t.P1 + (t.P1 - cm), (t.P2 - t.P0).Normalize());
      Assert.IsFalse(line.Intersects(t, precision),
                     "external line parallel to an edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      line = Line2D.FromDirection(t.P2 + (t.P2 - cm), (t.P0 - t.P1).Normalize());
      Assert.IsFalse(line.Intersects(t, precision),
                     "external line parallel to an edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      // case 3: overlap (no intersect)
      line = Line2D.FromDirection(t.P0, (t.P1 - t.P0).Normalize());
      Assert.IsFalse(line.Intersects(t, precision),
                     "line overlapping an edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      line = Line2D.FromDirection(t.P1, (t.P2 - t.P1).Normalize());
      Assert.IsFalse(line.Intersects(t, precision),
                     "line overlapping an edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      line = Line2D.FromDirection(t.P2, (t.P0 - t.P2).Normalize());
      Assert.IsFalse(line.Intersects(t, precision),
                     "line overlapping an edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void TriangleToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_triangle = RandomGenerator.MakeTriangle2D(decimal_precision: precision);
      var t = random_triangle.Triangle;
      (var p0, var p1, var p2) = (random_triangle.p0, random_triangle.p1, random_triangle.p2);

      if (t is null) {
        return;
      }

      // temp data
      Point2D cm = t.CenterOfMass();
      Ray2D ray;
      Point2D lp0, lp1;
      UnitVector2D u;
      UnitVector2D u_perp;

      // case 1: intersect
      //    perpendicular to an edge
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      u = (t.P1 - t.P0).Normalize();
      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through perp on p0-p1 edge t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      u = (t.P2 - t.P1).Normalize();
      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through perp on p1-p2 edge t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      u = (t.P0 - t.P2).Normalize();
      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through perp on p2-p0 edge t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      //    strike through two edges
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through mid points of edges t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through mid points of edges t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through mid points of edges t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      //    inner point strike through two edges
      lp0 = cm;
      lp1 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through mid points of edges t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      lp0 = cm;
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through mid points of edges t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      lp0 = cm;
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through mid points of edges t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      //    strike through one point
      ray = new Ray2D(t.P0, (t.P1 - t.P2).Normalize());
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through a vertex t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      ray = new Ray2D(t.P1, (t.P2 - t.P0).Normalize());
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through a vertex t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      ray = new Ray2D(t.P2, (t.P1 - t.P0).Normalize());
      Assert.IsTrue(ray.Intersects(t, precision),
                    "ray through a vertex t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      // case 2: parallel (no intersect)
      ray = new Ray2D(t.P0 + (t.P0 - cm), (t.P1 - t.P2).Normalize());
      Assert.IsFalse(ray.Intersects(t, precision),
                     "external ray parallel to an edge t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      ray = new Ray2D(t.P1 + (t.P1 - cm), (t.P2 - t.P0).Normalize());
      Assert.IsFalse(ray.Intersects(t, precision),
                     "external ray parallel to an edge t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      ray = new Ray2D(t.P2 + (t.P2 - cm), (t.P0 - t.P1).Normalize());
      Assert.IsFalse(ray.Intersects(t, precision),
                     "external ray parallel to an edge t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      // case 3: overlap (no intersect)
      ray = new Ray2D(t.P0, (t.P1 - t.P0).Normalize());
      Assert.IsFalse(ray.Intersects(t, precision),
                     "ray overlapping an edge t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      ray = new Ray2D(t.P1, (t.P2 - t.P1).Normalize());
      Assert.IsFalse(ray.Intersects(t, precision),
                     "ray overlapping an edge t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));

      ray = new Ray2D(t.P2, (t.P0 - t.P2).Normalize());
      Assert.IsFalse(ray.Intersects(t, precision),
                     "ray overlapping an edge t=" + t.ToWkt(precision) + ", ray=" + ray.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void TriangleToLineSegment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_triangle = RandomGenerator.MakeTriangle2D(decimal_precision: precision);
      var t = random_triangle.Triangle;
      (var p0, var p1, var p2) = (random_triangle.p0, random_triangle.p1, random_triangle.p2);

      if (t is null) {
        return;
      }

      // temp data
      Point2D cm = t.CenterOfMass();
      LineSegment2D segment;
      Point2D lp0, lp1;
      Vector2D U, V;

      // case 1: intersect
      //    perpendicular to an edge
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0 + (lp0 - cm), cm, precision);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through perp on p0-p1 edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0 + (lp0 - cm), cm);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through perp on p1-p2 edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0 + (lp0 - cm), cm);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through perp on p2-p0 edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      //    strike through two edges
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through mid points of edges t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through mid points of edges t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through mid points of edges t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      //    strike through one point
      U = (t.P2 - t.P1);
      lp0 = t.P0 + U * 2;
      lp1 = t.P0 - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsTrue(segment.Intersects(t, precision),
                    "segment through a vertex t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      U = (t.P0 - t.P2);
      lp0 = t.P1 + U * 2;
      lp1 = t.P1 - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsTrue(segment.Intersects(t, precision),
                    "segment through a vertex t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      U = (t.P1 - t.P0);
      lp0 = t.P2 + U * 2;
      lp1 = t.P2 - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsTrue(segment.Intersects(t, precision),
                    "segment through a vertex t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      // case 2: parallel (no intersect)
      // a segment parallel to P0-P1 and lying above P2
      U = (t.P1 - t.P0);
      V = t.P2 - cm;
      lp0 = t.P2 + V + U * 2;
      lp1 = t.P2 + V - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "external segment parallel to an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      // a segment parallel to P1-P2 and lying above P0
      U = (t.P2 - t.P1);
      V = t.P0 - cm;
      lp0 = t.P0 + V + U * 2;
      lp1 = t.P0 + V - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "external segment parallel to an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      // a segment parallel to P2-P0 and lying above P1
      U = (t.P0 - t.P2);
      V = t.P1 - cm;
      lp0 = t.P1 + V + U * 2;
      lp1 = t.P1 + V - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "external segment parallel to an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      // case 3: overlap (no intersect)
      lp0 = t.P0;
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(segment.Intersects(t, precision),
                     "segment overlapping an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = t.P1;
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(segment.Intersects(t, precision),
                     "segment overlapping an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = t.P2;
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(segment.Intersects(t, precision),
                     "segment overlapping an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      //    contained inside the triangle
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment2D.FromPoints(lp0 + U / 4, lp1 - U / 4, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "segment contained in the triangle t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment2D.FromPoints(lp0 + U / 4, lp1 - U / 4, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "segment contained in the triangle t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment2D.FromPoints(lp0 + U / 4, lp1 - U / 4, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "segment contained in the triangle t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));
    }

    [Ignore]
    [RepeatedTestMethod(1)]
    public void LineToPolygon() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void RayToPolygon() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void LineSegmentToPolygon() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void PolylineToPolygon() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void TriangleToPolygon() {}
  }
}
