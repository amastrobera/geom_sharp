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
      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine2D();

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
      Assert.IsTrue(line.Intersects(ray), "intersect forrreal (mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray2D(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(line.Intersects(ray), "intersect forrreal (p0)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray2D(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(line.Intersects(ray), "intersect forrreal (p1)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going down
      ray = new Ray2D(mid, -u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (to mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going up
      ray = new Ray2D(mid, u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (from mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going down
      ray = new Ray2D(p0, -u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (to p0)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going up
      ray = new Ray2D(p0, u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (from p0)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the second extremity, going up
      ray = new Ray2D(p1, -u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (from p1)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the second extremity, going down
      ray = new Ray2D(p1, u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (to p1)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector2D shift = RandomGenerator.MakeVector2D();
      ray = new Ray2D(p0 + 2 * shift, u);
      Assert.IsFalse(
          line.Intersects(ray),
          "no intersection (parallel, shift upwards random vector)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      downwards
      ray = new Ray2D(p0 - 2 * shift, -u);
      Assert.IsFalse(
          line.Intersects(ray),
          "no intersection (parallel, shift downwards random vector)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray2D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (mid-)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray2D(p0 - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (p0-)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray2D(p1 - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (p1-)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      in the middle, crossing
      ray = new Ray2D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (mid+)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray2D(p0 + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (p0+)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray2D(p1 + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (p1+)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void LineToLineSegment() {
      // input
      (var segment, var p0, var p1) = RandomGenerator.MakeLineSegment2D();

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
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      on the first extremity, crossing
      line = Line2D.FromDirection(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      on the second extremity, crossing
      line = Line2D.FromDirection(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the middle, going down
      line = Line2D.FromDirection(mid, -u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the middle, going up
      line = Line2D.FromDirection(mid, u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (from mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the first extremity, going down
      line = Line2D.FromDirection(p0, -u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the first extremity, going up
      line = Line2D.FromDirection(p0, u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the second extremity, going up
      line = Line2D.FromDirection(p1, -u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the second extremity, going down
      line = Line2D.FromDirection(p1, u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (to p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector2D shift = RandomGenerator.MakeVector2D();
      line = Line2D.FromDirection(p0 + 2 * shift, u);
      Assert.IsFalse(segment.Intersects(line),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + line.ToWkt());

      //      downwards
      line = Line2D.FromDirection(p0 - 2 * shift, -u);
      Assert.IsFalse(segment.Intersects(line),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + line.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void LineSegmentToRay() {
      // input
      (var segment, var p0, var p1) = RandomGenerator.MakeLineSegment2D();

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
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray2D(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray2D(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going down
      ray = new Ray2D(mid, -u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going up
      ray = new Ray2D(mid, u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (from mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going down
      ray = new Ray2D(p0, -u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going up
      ray = new Ray2D(p0, u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the second extremity, going up
      ray = new Ray2D(p1, -u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the second extremity, going down
      ray = new Ray2D(p1, u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (to p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector2D shift = RandomGenerator.MakeVector2D();
      ray = new Ray2D(p0 + 2 * shift, u);
      Assert.IsFalse(segment.Intersects(ray),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + ray.ToWkt());

      //      downwards
      ray = new Ray2D(p0 - 2 * shift, -u);
      Assert.IsFalse(segment.Intersects(ray),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + ray.ToWkt());

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray2D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (mid-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray2D(p0 - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (p0-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray2D(p1 - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (p1-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      in the middle, crossing
      ray = new Ray2D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (mid+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray2D(p0 + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (p0+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray2D(p1 + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (p1+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());
    }

    // Triangle with other basic primitives

    [RepeatedTestMethod(100)]
    public void TriangleToLine() {
      var random_triangle = RandomGenerator.MakeTriangle2D();
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
      Assert.IsTrue(line.Intersects(t), "line through perp on p0-p1 edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      u = (t.P2 - t.P1).Normalize();
      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsTrue(line.Intersects(t), "line through perp on p1-p2 edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      u = (t.P0 - t.P2).Normalize();
      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsTrue(line.Intersects(t), "line through perp on p2-p0 edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      //    strike through two edges
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(line.Intersects(t), "line through mid points of edges t=" + t.ToWkt() + ", line=" + line.ToWkt());

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(line.Intersects(t), "line through mid points of edges t=" + t.ToWkt() + ", line=" + line.ToWkt());

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(line.Intersects(t), "line through mid points of edges t=" + t.ToWkt() + ", line=" + line.ToWkt());

      //    strike through one point
      line = Line2D.FromDirection(t.P0, (t.P1 - t.P2).Normalize());
      Assert.IsTrue(line.Intersects(t), "line through a vertex t=" + t.ToWkt() + ", line=" + line.ToWkt());

      line = Line2D.FromDirection(t.P1, (t.P2 - t.P0).Normalize());
      Assert.IsTrue(line.Intersects(t), "line through a vertex t=" + t.ToWkt() + ", line=" + line.ToWkt());

      line = Line2D.FromDirection(t.P2, (t.P1 - t.P0).Normalize());
      Assert.IsTrue(line.Intersects(t), "line through a vertex t=" + t.ToWkt() + ", line=" + line.ToWkt());

      // case 2: parallel (no intersect)
      line = Line2D.FromDirection(t.P0 + (t.P0 - cm), (t.P1 - t.P2).Normalize());
      Assert.IsFalse(line.Intersects(t), "external line parallel to an edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      line = Line2D.FromDirection(t.P1 + (t.P1 - cm), (t.P2 - t.P0).Normalize());
      Assert.IsFalse(line.Intersects(t), "external line parallel to an edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      line = Line2D.FromDirection(t.P2 + (t.P2 - cm), (t.P0 - t.P1).Normalize());
      Assert.IsFalse(line.Intersects(t), "external line parallel to an edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      // case 3: overlap (no intersect)
      line = Line2D.FromDirection(t.P0, (t.P1 - t.P0).Normalize());
      Assert.IsFalse(line.Intersects(t), "line overlapping an edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      line = Line2D.FromDirection(t.P1, (t.P2 - t.P1).Normalize());
      Assert.IsFalse(line.Intersects(t), "line overlapping an edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      line = Line2D.FromDirection(t.P2, (t.P0 - t.P2).Normalize());
      Assert.IsFalse(line.Intersects(t), "line overlapping an edge t=" + t.ToWkt() + ", line=" + line.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void TriangleToRay() {
      var random_triangle = RandomGenerator.MakeTriangle2D();
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
      Assert.IsTrue(ray.Intersects(t), "ray through perp on p0-p1 edge t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      u = (t.P2 - t.P1).Normalize();
      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsTrue(ray.Intersects(t), "ray through perp on p1-p2 edge t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      u = (t.P0 - t.P2).Normalize();
      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsTrue(ray.Intersects(t), "ray through perp on p2-p0 edge t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      //    strike through two edges
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t), "ray through mid points of edges t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t), "ray through mid points of edges t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t), "ray through mid points of edges t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      //    inner point strike through two edges
      lp0 = cm;
      lp1 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t), "ray through mid points of edges t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      lp0 = cm;
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t), "ray through mid points of edges t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      lp0 = cm;
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      ray = new Ray2D(lp0, (lp1 - lp0).Normalize());
      Assert.IsTrue(ray.Intersects(t), "ray through mid points of edges t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      //    strike through one point
      ray = new Ray2D(t.P0, (t.P1 - t.P2).Normalize());
      Assert.IsTrue(ray.Intersects(t), "ray through a vertex t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      ray = new Ray2D(t.P1, (t.P2 - t.P0).Normalize());
      Assert.IsTrue(ray.Intersects(t), "ray through a vertex t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      ray = new Ray2D(t.P2, (t.P1 - t.P0).Normalize());
      Assert.IsTrue(ray.Intersects(t), "ray through a vertex t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      // case 2: parallel (no intersect)
      ray = new Ray2D(t.P0 + (t.P0 - cm), (t.P1 - t.P2).Normalize());
      Assert.IsFalse(ray.Intersects(t), "external ray parallel to an edge t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      ray = new Ray2D(t.P1 + (t.P1 - cm), (t.P2 - t.P0).Normalize());
      Assert.IsFalse(ray.Intersects(t), "external ray parallel to an edge t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      ray = new Ray2D(t.P2 + (t.P2 - cm), (t.P0 - t.P1).Normalize());
      Assert.IsFalse(ray.Intersects(t), "external ray parallel to an edge t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      // case 3: overlap (no intersect)
      ray = new Ray2D(t.P0, (t.P1 - t.P0).Normalize());
      Assert.IsFalse(ray.Intersects(t), "ray overlapping an edge t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      ray = new Ray2D(t.P1, (t.P2 - t.P1).Normalize());
      Assert.IsFalse(ray.Intersects(t), "ray overlapping an edge t=" + t.ToWkt() + ", ray=" + ray.ToWkt());

      ray = new Ray2D(t.P2, (t.P0 - t.P2).Normalize());
      Assert.IsFalse(ray.Intersects(t), "ray overlapping an edge t=" + t.ToWkt() + ", ray=" + ray.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void TriangleToLineSegment() {
      var random_triangle = RandomGenerator.MakeTriangle2D();
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
      segment = LineSegment2D.FromPoints(lp0 + (lp0 - cm), cm);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through perp on p0-p1 edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0 + (lp0 - cm), cm);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through perp on p1-p2 edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0 + (lp0 - cm), cm);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through perp on p2-p0 edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      //    strike through two edges
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through mid points of edges t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through mid points of edges t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through mid points of edges t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      //    strike through one point
      U = (t.P2 - t.P1);
      lp0 = t.P0 + U * 2;
      lp1 = t.P0 - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t), "segment through a vertex t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      U = (t.P0 - t.P2);
      lp0 = t.P1 + U * 2;
      lp1 = t.P1 - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t), "segment through a vertex t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      U = (t.P1 - t.P0);
      lp0 = t.P2 + U * 2;
      lp1 = t.P2 - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t), "segment through a vertex t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      // case 2: parallel (no intersect)
      // a segment parallel to P0-P1 and lying above P2
      U = (t.P1 - t.P0);
      V = t.P2 - cm;
      lp0 = t.P2 + V + U * 2;
      lp1 = t.P2 + V - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "external segment parallel to an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      // a segment parallel to P1-P2 and lying above P0
      U = (t.P2 - t.P1);
      V = t.P0 - cm;
      lp0 = t.P0 + V + U * 2;
      lp1 = t.P0 + V - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "external segment parallel to an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      // a segment parallel to P2-P0 and lying above P1
      U = (t.P0 - t.P2);
      V = t.P1 - cm;
      lp0 = t.P1 + V + U * 2;
      lp1 = t.P1 + V - U * 2;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "external segment parallel to an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      // case 3: overlap (no intersect)
      lp0 = t.P0;
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "segment overlapping an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = t.P1;
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "segment overlapping an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = t.P2;
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "segment overlapping an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      //    contained inside the triangle
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment2D.FromPoints(lp0 + U / 4, lp1 - U / 4);
      Assert.IsFalse(segment.Intersects(t),
                     "segment contained in the triangle t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment2D.FromPoints(lp0 + U / 4, lp1 - U / 4);
      Assert.IsFalse(segment.Intersects(t),
                     "segment contained in the triangle t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment2D.FromPoints(lp0 + U / 4, lp1 - U / 4);
      Assert.IsFalse(segment.Intersects(t),
                     "segment contained in the triangle t=" + t.ToWkt() + ", segment=" + segment.ToWkt());
    }
  }
}
