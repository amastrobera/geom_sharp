// internal
using GeomSharp;
using GeomSharp.Extensions;

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
  public class OverlapExtensions2DTests {
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
      Assert.IsFalse(line.Overlaps(ray), "intersect forrreal (mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going down
      ray = new Ray2D(mid, -u_perp);
      Assert.IsFalse(line.Overlaps(ray),
                     "intersect forrreal (to mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going up
      ray = new Ray2D(mid, u_perp);
      Assert.IsFalse(line.Overlaps(ray),
                     "intersect forrreal (from mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going down
      ray = new Ray2D(p0, -u_perp);
      Assert.IsFalse(line.Overlaps(ray), "intersect forrreal (to p0)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going up
      ray = new Ray2D(p0, u_perp);
      Assert.IsFalse(line.Overlaps(ray),
                     "intersect forrreal (from p0)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      ray = new Ray2D(p0 + 2 * u_perp, u);
      Assert.IsFalse(
          line.Overlaps(ray),
          "no intersection (parallel, shift upwards random vector)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      downwards
      ray = new Ray2D(p0 - 2 * u_perp, -u);
      Assert.IsFalse(
          line.Overlaps(ray),
          "no intersection (parallel, shift downwards random vector)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray2D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Overlaps(ray), "not intersect (mid-)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      in the middle, crossing
      ray = new Ray2D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Overlaps(ray), "not intersect (mid+)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      // case 4: overlap
      //      ray starts from the middle of the line
      ray = new Ray2D(mid, u);
      Assert.IsTrue(line.Overlaps(ray), "overlap (mid+)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      ray = new Ray2D(mid, -u);
      Assert.IsTrue(line.Overlaps(ray), "overlap (mid-)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());
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
      Assert.IsFalse(segment.Overlaps(line),
                     "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the middle, going down
      line = Line2D.FromDirection(mid, -u_perp);
      Assert.IsFalse(segment.Overlaps(line),
                     "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the first extremity, going down
      line = Line2D.FromDirection(p0, -u_perp);
      Assert.IsFalse(segment.Overlaps(line),
                     "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the first extremity, going up
      line = Line2D.FromDirection(p1, u_perp);
      Assert.IsFalse(segment.Overlaps(line),
                     "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      line = Line2D.FromDirection(p0 + 2 * u_perp, u);
      Assert.IsFalse(segment.Overlaps(line),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + line.ToWkt());

      //      downwards
      line = Line2D.FromDirection(p0 - 2 * u_perp, -u);
      Assert.IsFalse(segment.Overlaps(line),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + line.ToWkt());

      // case 3: overlap
      //      just one point in the the first extremity, going down
      line = Line2D.FromDirection(p0, u);
      Assert.IsTrue(segment.Overlaps(line), "overlap (from p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the first extremity, going up
      line = Line2D.FromDirection(p1, -u);
      Assert.IsTrue(segment.Overlaps(line), "overlap (to p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());
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
      Assert.IsFalse(segment.Overlaps(ray),
                     "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going down
      ray = new Ray2D(mid, -u_perp);
      Assert.IsFalse(segment.Overlaps(ray),
                     "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going down
      ray = new Ray2D(p0, -u_perp);
      Assert.IsFalse(segment.Overlaps(ray),
                     "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the second extremity, going up
      ray = new Ray2D(p1, -u_perp);
      Assert.IsFalse(segment.Overlaps(ray),
                     "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      ray = new Ray2D(p0 + 2 * u_perp, u);
      Assert.IsFalse(segment.Overlaps(ray),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + ray.ToWkt());

      //      downwards
      ray = new Ray2D(p0 - 2 * u_perp, -u);
      Assert.IsFalse(segment.Overlaps(ray),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + ray.ToWkt());

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray2D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Overlaps(ray), "not intersect (mid-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      in the middle, crossing
      ray = new Ray2D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Overlaps(ray), "not intersect (mid+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      // case 4: overlap

      //      in the middle
      ray = new Ray2D(mid, u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (mid+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      in the middle
      ray = new Ray2D(mid, -u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (mid-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      at the beginning
      ray = new Ray2D(p1, -u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (p1-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      at the end
      ray = new Ray2D(p0, u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (p0+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      at the beginning
      ray = new Ray2D(p1, u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (p1+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      at the end
      ray = new Ray2D(p0, -u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (p0-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());
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
      Point2D lp0;
      UnitVector2D u;
      UnitVector2D u_perp;

      // case 1: overlap across the edges
      // overlap and perpendicular to P0-P1
      u = (t.P1 - t.P0).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, u);
      Assert.IsTrue(line.Overlaps(t), "line overlapping p0-p1 edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsFalse(line.Overlaps(t), "line perpendicular to p0-p1 edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      // overlap and perpendicular to P1-P2
      u = (t.P2 - t.P1).Normalize();
      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P1.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, u);
      Assert.IsTrue(line.Overlaps(t), "line overlapping p1-p2 edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsFalse(line.Overlaps(t), "line perpendicular to p1-p2 edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      // overlap and perpendicular to P2-P0
      u = (t.P0 - t.P2).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P2.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, u);
      Assert.IsTrue(line.Overlaps(t), "line overlapping p2-p0 edge t=" + t.ToWkt() + ", line=" + line.ToWkt());

      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsFalse(line.Overlaps(t), "line perpendicular to p2-p0 edge t=" + t.ToWkt() + ", line=" + line.ToWkt());
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
      Point2D lp0;
      UnitVector2D u;
      UnitVector2D u_perp;

      // case 1: overlap across the edges
      // overlap and perpendicular to P0-P1
      u = (t.P1 - t.P0).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      ray = new Ray2D(lp0, u);
      Assert.IsTrue(ray.Overlaps(t), "line overlapping p0-p1 edge t=" + t.ToWkt() + ", line=" + ray.ToWkt());

      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsFalse(ray.Overlaps(t), "line perpendicular to p0-p1 edge t=" + t.ToWkt() + ", line=" + ray.ToWkt());

      // overlap and perpendicular to P1-P2
      u = (t.P2 - t.P1).Normalize();
      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P1.ToVector()) / 2.0);
      ray = new Ray2D(lp0, u);
      Assert.IsTrue(ray.Overlaps(t), "line overlapping p1-p2 edge t=" + t.ToWkt() + ", line=" + ray.ToWkt());

      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsFalse(ray.Overlaps(t), "line perpendicular to p1-p2 edge t=" + t.ToWkt() + ", line=" + ray.ToWkt());

      // overlap and perpendicular to P2-P0
      u = (t.P0 - t.P2).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P2.ToVector()) / 2.0);
      ray = new Ray2D(lp0, u);
      Assert.IsTrue(ray.Overlaps(t), "line overlapping p2-p0 edge t=" + t.ToWkt() + ", line=" + ray.ToWkt());

      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsFalse(ray.Overlaps(t), "line perpendicular to p2-p0 edge t=" + t.ToWkt() + ", line=" + ray.ToWkt());
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
      UnitVector2D u;
      UnitVector2D u_perp;

      // case 1: overlap across the edges
      // overlap and perpendicular to P0-P1, parallel but detached
      u = (t.P1 - t.P0).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Overlaps(t), "segment overlapping p0-p1 edge t=" + t.ToWkt() + ", line=" + segment.ToWkt());

      u_perp = u.Perp().Normalize();
      lp1 = lp0 + 2 * u_perp;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Overlaps(t),
                     "segment perpendicular to p0-p1 edge t=" + t.ToWkt() + ", line=" + segment.ToWkt());

      lp0 = lp0 - 2 * u_perp;
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Overlaps(t),
                     "segment parallel (detatched) to p0-p1 edge t=" + t.ToWkt() + ", line=" + segment.ToWkt());

      // overlap and perpendicular to P1-P2, parallel but detached
      u = (t.P2 - t.P1).Normalize();
      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Overlaps(t), "segment overlapping p1-p2 edge t=" + t.ToWkt() + ", line=" + segment.ToWkt());

      u_perp = u.Perp().Normalize();
      lp1 = lp0 + 2 * u_perp;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Overlaps(t),
                     "segment perpendicular to p1-p2 edge t=" + t.ToWkt() + ", line=" + segment.ToWkt());

      lp0 = lp0 - 2 * u_perp;
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Overlaps(t),
                     "segment parallel (detatched) to p1-p2 edge t=" + t.ToWkt() + ", line=" + segment.ToWkt());

      // overlap and perpendicular to P2-P0, parallel but detached
      u = (t.P0 - t.P2).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Overlaps(t), "segment overlapping p2-p0 edge t=" + t.ToWkt() + ", line=" + segment.ToWkt());

      u_perp = u.Perp().Normalize();
      lp1 = lp0 + 2 * u_perp;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Overlaps(t),
                     "segment perpendicular to p2-p0 edge t=" + t.ToWkt() + ", line=" + segment.ToWkt());

      lp0 = lp0 - 2 * u_perp;
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Overlaps(t),
                     "segment parallel (detatched) to p2-p0 edge t=" + t.ToWkt() + ", line=" + segment.ToWkt());
    }
  }
}
