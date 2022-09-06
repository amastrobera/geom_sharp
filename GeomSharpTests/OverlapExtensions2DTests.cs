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

    [RepeatedTestMethod(1)]
    public void TriangleToLine() {
      Assert.IsTrue(false);
    }

    [RepeatedTestMethod(1)]
    public void TriangleToRay() {
      Assert.IsTrue(false);
    }

    [RepeatedTestMethod(1)]
    public void TriangleToLineSegment() {
      Assert.IsTrue(false);
    }
  }
}
