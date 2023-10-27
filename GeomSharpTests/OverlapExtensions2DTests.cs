// internal
using GeomSharp;

// external
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
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "intersect forrreal (mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going down
      ray = new Ray2D(mid, -u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "intersect forrreal (to mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going up
      ray = new Ray2D(mid, u_perp);
      Assert.IsFalse(
          line.Overlaps(ray, precision),
          "intersect forrreal (from mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going down
      ray = new Ray2D(p0, -u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "intersect forrreal (to p0)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going up
      ray = new Ray2D(p0, u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "intersect forrreal (from p0)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      ray = new Ray2D(p0 + 2 * u_perp, u);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + line.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      //      downwards
      ray = new Ray2D(p0 - 2 * u_perp, -u);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + line.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray2D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "not intersect (mid-)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      in the middle, crossing
      ray = new Ray2D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "not intersect (mid+)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 4: overlap
      //      ray starts from the middle of the line
      ray = new Ray2D(mid, u);
      Assert.IsTrue(line.Overlaps(ray, precision),
                    "overlap (mid+)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      ray = new Ray2D(mid, -u);
      Assert.IsTrue(line.Overlaps(ray, precision),
                    "overlap (mid-)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));
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
      Assert.IsFalse(segment.Overlaps(line, precision),
                     "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the middle, going down
      line = Line2D.FromDirection(mid, -u_perp);
      Assert.IsFalse(
          segment.Overlaps(line, precision),
          "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the first extremity, going down
      line = Line2D.FromDirection(p0, -u_perp);
      Assert.IsFalse(
          segment.Overlaps(line, precision),
          "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the first extremity, going up
      line = Line2D.FromDirection(p1, u_perp);
      Assert.IsFalse(
          segment.Overlaps(line, precision),
          "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      line = Line2D.FromDirection(p0 + 2 * u_perp, u);
      Assert.IsFalse(segment.Overlaps(line, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + line.ToWkt(precision));

      //      downwards
      line = Line2D.FromDirection(p0 - 2 * u_perp, -u);
      Assert.IsFalse(segment.Overlaps(line, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + line.ToWkt(precision));

      // case 3: overlap
      //      just one point in the the first extremity, going down
      line = Line2D.FromDirection(p0, u);
      Assert.IsTrue(segment.Overlaps(line, precision),
                    "overlap (from p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the first extremity, going up
      line = Line2D.FromDirection(p1, -u);
      Assert.IsTrue(segment.Overlaps(line, precision),
                    "overlap (to p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));
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
      Assert.IsFalse(segment.Overlaps(ray, precision),
                     "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going down
      ray = new Ray2D(mid, -u_perp);
      Assert.IsFalse(
          segment.Overlaps(ray, precision),
          "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going down
      ray = new Ray2D(p0, -u_perp);
      Assert.IsFalse(
          segment.Overlaps(ray, precision),
          "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the second extremity, going up
      ray = new Ray2D(p1, -u_perp);
      Assert.IsFalse(
          segment.Overlaps(ray, precision),
          "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      ray = new Ray2D(p0 + 2 * u_perp, u);
      Assert.IsFalse(segment.Overlaps(ray, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      //      downwards
      ray = new Ray2D(p0 - 2 * u_perp, -u);
      Assert.IsFalse(segment.Overlaps(ray, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray2D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Overlaps(ray, precision),
                     "not intersect (mid-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      in the middle, crossing
      ray = new Ray2D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Overlaps(ray, precision),
                     "not intersect (mid+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 4: overlap

      //      in the middle
      ray = new Ray2D(mid, u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (mid+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      in the middle
      ray = new Ray2D(mid, -u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (mid-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      at the beginning
      ray = new Ray2D(p1, -u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (p1-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      at the end
      ray = new Ray2D(p0, u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (p0+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      at the beginning
      ray = new Ray2D(p1, u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (p1+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      at the end
      ray = new Ray2D(p0, -u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (p0-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));
    }

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
      Point2D lp0;
      UnitVector2D u;
      UnitVector2D u_perp;

      // case 1: overlap across the edges
      // overlap and perpendicular to P0-P1
      u = (t.P1 - t.P0).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, u);
      Assert.IsTrue(line.Overlaps(t, precision),
                    "line overlapping p0-p1 edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsFalse(line.Overlaps(t, precision),
                     "line perpendicular to p0-p1 edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      // overlap and perpendicular to P1-P2
      u = (t.P2 - t.P1).Normalize();
      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P1.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, u);
      Assert.IsTrue(line.Overlaps(t, precision),
                    "line overlapping p1-p2 edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsFalse(line.Overlaps(t, precision),
                     "line perpendicular to p1-p2 edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      // overlap and perpendicular to P2-P0
      u = (t.P0 - t.P2).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P2.ToVector()) / 2.0);
      line = Line2D.FromDirection(lp0, u);
      Assert.IsTrue(line.Overlaps(t, precision),
                    "line overlapping p2-p0 edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));

      u_perp = u.Perp().Normalize();
      line = Line2D.FromDirection(lp0, u_perp);
      Assert.IsFalse(line.Overlaps(t, precision),
                     "line perpendicular to p2-p0 edge t=" + t.ToWkt(precision) + ", line=" + line.ToWkt(precision));
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
      Point2D lp0;
      UnitVector2D u;
      UnitVector2D u_perp;

      // case 1: overlap across the edges
      // overlap and perpendicular to P0-P1
      u = (t.P1 - t.P0).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      ray = new Ray2D(lp0, u);
      Assert.IsTrue(ray.Overlaps(t, precision),
                    "line overlapping p0-p1 edge t=" + t.ToWkt(precision) + ", line=" + ray.ToWkt(precision));

      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsFalse(ray.Overlaps(t, precision),
                     "line perpendicular to p0-p1 edge t=" + t.ToWkt(precision) + ", line=" + ray.ToWkt(precision));

      // overlap and perpendicular to P1-P2
      u = (t.P2 - t.P1).Normalize();
      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P1.ToVector()) / 2.0);
      ray = new Ray2D(lp0, u);
      Assert.IsTrue(ray.Overlaps(t, precision),
                    "line overlapping p1-p2 edge t=" + t.ToWkt(precision) + ", line=" + ray.ToWkt(precision));

      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsFalse(ray.Overlaps(t, precision),
                     "line perpendicular to p1-p2 edge t=" + t.ToWkt(precision) + ", line=" + ray.ToWkt(precision));

      // overlap and perpendicular to P2-P0
      u = (t.P0 - t.P2).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P2.ToVector()) / 2.0);
      ray = new Ray2D(lp0, u);
      Assert.IsTrue(ray.Overlaps(t, precision),
                    "line overlapping p2-p0 edge t=" + t.ToWkt(precision) + ", line=" + ray.ToWkt(precision));

      u_perp = u.Perp().Normalize();
      ray = new Ray2D(lp0, u_perp);
      Assert.IsFalse(ray.Overlaps(t, precision),
                     "line perpendicular to p2-p0 edge t=" + t.ToWkt(precision) + ", line=" + ray.ToWkt(precision));
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
      UnitVector2D u;
      UnitVector2D u_perp;

      // case 1: overlap across the edges
      // overlap and perpendicular to P0-P1, parallel but detached
      u = (t.P1 - t.P0).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsTrue(segment.Overlaps(t, precision),
                    "segment overlapping p0-p1 edge t=" + t.ToWkt(precision) + ", line=" + segment.ToWkt(precision));

      u_perp = u.Perp().Normalize();
      lp1 = lp0 + 2 * u_perp;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Overlaps(t, precision),
          "segment perpendicular to p0-p1 edge t=" + t.ToWkt(precision) + ", line=" + segment.ToWkt(precision));

      lp0 = lp0 - 2 * u_perp;
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Overlaps(t, precision),
          "segment parallel (detatched) to p0-p1 edge t=" + t.ToWkt(precision) + ", line=" + segment.ToWkt(precision));

      // overlap and perpendicular to P1-P2, parallel but detached
      u = (t.P2 - t.P1).Normalize();
      lp0 = Point2D.FromVector((t.P2.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsTrue(segment.Overlaps(t, precision),
                    "segment overlapping p1-p2 edge t=" + t.ToWkt(precision) + ", line=" + segment.ToWkt(precision));

      u_perp = u.Perp().Normalize();
      lp1 = lp0 + 2 * u_perp;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Overlaps(t, precision),
          "segment perpendicular to p1-p2 edge t=" + t.ToWkt(precision) + ", line=" + segment.ToWkt(precision));

      lp0 = lp0 - 2 * u_perp;
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Overlaps(t, precision),
          "segment parallel (detatched) to p1-p2 edge t=" + t.ToWkt(precision) + ", line=" + segment.ToWkt(precision));

      // overlap and perpendicular to P2-P0, parallel but detached
      u = (t.P0 - t.P2).Normalize();
      lp0 = Point2D.FromVector((t.P0.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsTrue(segment.Overlaps(t, precision),
                    "segment overlapping p2-p0 edge t=" + t.ToWkt(precision) + ", line=" + segment.ToWkt(precision));

      u_perp = u.Perp().Normalize();
      lp1 = lp0 + 2 * u_perp;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Overlaps(t, precision),
          "segment perpendicular to p2-p0 edge t=" + t.ToWkt(precision) + ", line=" + segment.ToWkt(precision));

      lp0 = lp0 - 2 * u_perp;
      lp1 = lp0 + 2 * u;
      segment = LineSegment2D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Overlaps(t, precision),
          "segment parallel (detatched) to p2-p0 edge t=" + t.ToWkt(precision) + ", line=" + segment.ToWkt(precision));
    }
  }
}
