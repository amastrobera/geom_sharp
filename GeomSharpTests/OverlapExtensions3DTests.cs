// internal
using GeomSharp;

// external
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {
  /// <summary>
  /// tests for all the MathNet.Spatial.Euclidean functions I created
  /// </summary>
  [TestClass]
  public class OverlapExtensions3DTests {
    // Line, LineSegment, Ray cross test
    [RepeatedTestMethod(100)]
    public void LineToRay() {
      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine3D();

      if (line is null) {
        return;
      }

      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = line.Direction;

      // temp data
      Ray3D ray;
      UnitVector3D u_perp =
          u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, u_perp);
      Assert.IsFalse(line.Overlaps(ray), "intersect forrreal (mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going down
      ray = new Ray3D(mid, -u_perp);
      Assert.IsFalse(line.Overlaps(ray),
                     "intersect forrreal (to mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going up
      ray = new Ray3D(mid, u_perp);
      Assert.IsFalse(line.Overlaps(ray),
                     "intersect forrreal (from mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going down
      ray = new Ray3D(p0, -u_perp);
      Assert.IsFalse(line.Overlaps(ray), "intersect forrreal (to p0)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going up
      ray = new Ray3D(p0, u_perp);
      Assert.IsFalse(line.Overlaps(ray),
                     "intersect forrreal (from p0)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      ray = new Ray3D(p0 + 2 * u_perp, u);
      Assert.IsFalse(
          line.Overlaps(ray),
          "no intersection (parallel, shift upwards random vector)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      downwards
      ray = new Ray3D(p0 - 2 * u_perp, -u);
      Assert.IsFalse(
          line.Overlaps(ray),
          "no intersection (parallel, shift downwards random vector)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Overlaps(ray), "not intersect (mid-)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      in the middle, crossing
      ray = new Ray3D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Overlaps(ray), "not intersect (mid+)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      // case 4: overlap
      //      ray starts from the middle of the line
      ray = new Ray3D(mid, u);
      Assert.IsTrue(line.Overlaps(ray), "overlap (mid+)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      ray = new Ray3D(mid, -u);
      Assert.IsTrue(line.Overlaps(ray), "overlap (mid-)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void LineToLineSegment() {
      // input
      (var segment, var p0, var p1) = RandomGenerator.MakeLineSegment3D();

      if (segment is null) {
        return;
      }

      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = segment.ToLine().Direction;

      // temp data
      Line3D line;
      UnitVector3D u_perp =
          u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      line = Line3D.FromDirection(mid - 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Overlaps(line),
                     "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the middle, going down
      line = Line3D.FromDirection(mid, -u_perp);
      Assert.IsFalse(segment.Overlaps(line),
                     "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the first extremity, going down
      line = Line3D.FromDirection(p0, -u_perp);
      Assert.IsFalse(segment.Overlaps(line),
                     "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the first extremity, going up
      line = Line3D.FromDirection(p1, u_perp);
      Assert.IsFalse(segment.Overlaps(line),
                     "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      line = Line3D.FromDirection(p0 + 2 * u_perp, u);
      Assert.IsFalse(segment.Overlaps(line),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + line.ToWkt());

      //      downwards
      line = Line3D.FromDirection(p0 - 2 * u_perp, -u);
      Assert.IsFalse(segment.Overlaps(line),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + line.ToWkt());

      // case 3: overlap
      //      just one point in the the first extremity, going down
      line = Line3D.FromDirection(p0, u);
      Assert.IsTrue(segment.Overlaps(line), "overlap (from p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the first extremity, going up
      line = Line3D.FromDirection(p1, -u);
      Assert.IsTrue(segment.Overlaps(line), "overlap (to p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void LineSegmentToRay() {
      // input
      (var segment, var p0, var p1) = RandomGenerator.MakeLineSegment3D();

      if (segment is null) {
        return;
      }

      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = segment.ToLine().Direction;

      // temp data
      Ray3D ray;
      UnitVector3D u_perp =
          u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Overlaps(ray),
                     "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going down
      ray = new Ray3D(mid, -u_perp);
      Assert.IsFalse(segment.Overlaps(ray),
                     "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going down
      ray = new Ray3D(p0, -u_perp);
      Assert.IsFalse(segment.Overlaps(ray),
                     "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the second extremity, going up
      ray = new Ray3D(p1, -u_perp);
      Assert.IsFalse(segment.Overlaps(ray),
                     "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      ray = new Ray3D(p0 + 2 * u_perp, u);
      Assert.IsFalse(segment.Overlaps(ray),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + ray.ToWkt());

      //      downwards
      ray = new Ray3D(p0 - 2 * u_perp, -u);
      Assert.IsFalse(segment.Overlaps(ray),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + ray.ToWkt());

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Overlaps(ray), "not intersect (mid-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      in the middle, crossing
      ray = new Ray3D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Overlaps(ray), "not intersect (mid+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      // case 4: overlap

      //      in the middle
      ray = new Ray3D(mid, u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (mid+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      in the middle
      ray = new Ray3D(mid, -u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (mid-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      at the beginning
      ray = new Ray3D(p1, -u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (p1-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      at the end
      ray = new Ray3D(p0, u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (p0+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      at the beginning
      ray = new Ray3D(p1, u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (p1+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      at the end
      ray = new Ray3D(p0, -u);
      Assert.IsTrue(segment.Overlaps(ray), "overlap (p0-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());
    }

    // Plane with other basic primitives
    [RepeatedTestMethod(100)]
    public void PlaneToLine() {
      (var plane, var p0, var p1, var p2) = RandomGenerator.MakePlane();
      if (plane is null) {
        return;
      }

      // cache
      Line3D line;

      line = Line3D.FromDirection(plane.Origin, plane.AxisU);
      Assert.IsTrue(plane.Overlaps(line));

      line = Line3D.FromDirection(plane.Origin, plane.AxisV);
      Assert.IsTrue(plane.Overlaps(line));

      line = Line3D.FromPoints(p0, p1);
      Assert.IsTrue(plane.Overlaps(line));

      line = Line3D.FromPoints(p0, p2);
      Assert.IsTrue(plane.Overlaps(line));

      line = Line3D.FromPoints(p1, p2);
      Assert.IsTrue(plane.Overlaps(line));

      (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();

      line = Line3D.FromPoints(
          new Point3D(a * p0.X + b * p1.X + c * p2.X, a * p0.Y + b * p1.Y + c * p2.Y, a * p0.Z + b * p1.Z + c * p2.Z),
          p2);
      Assert.IsTrue(plane.Overlaps(line));
    }

    [RepeatedTestMethod(100)]
    public void PlaneToRay() {
      (var plane, var p0, var p1, var p2) = RandomGenerator.MakePlane();
      if (plane is null) {
        return;
      }

      // cache
      Ray3D ray;

      ray = new Ray3D(plane.Origin, plane.AxisU);
      Assert.IsTrue(plane.Overlaps(ray));

      ray = new Ray3D(plane.Origin, plane.AxisV);
      Assert.IsTrue(plane.Overlaps(ray));

      ray = new Ray3D(p0, (p1 - p0).Normalize());
      Assert.IsTrue(plane.Overlaps(ray));

      ray = new Ray3D(p0, (p2 - p0).Normalize());
      Assert.IsTrue(plane.Overlaps(ray));

      ray = new Ray3D(p1, (p2 - p1).Normalize());
      Assert.IsTrue(plane.Overlaps(ray));

      (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();

      ray = new Ray3D(
          p2,
          (new Point3D(a * p0.X + b * p1.X + c * p2.X, a * p0.Y + b * p1.Y + c * p2.Y, a * p0.Z + b * p1.Z + c * p2.Z) -
           p2)
              .Normalize());
      Assert.IsTrue(plane.Overlaps(ray));
    }

    [RepeatedTestMethod(100)]
    public void PlaneToLineSegment() {
      (var plane, var p0, var p1, var p2) = RandomGenerator.MakePlane();
      if (plane is null) {
        return;
      }

      // cache
      LineSegment3D seg;

      seg = LineSegment3D.FromPoints(plane.Origin, plane.Origin + plane.AxisU);
      Assert.IsTrue(plane.Overlaps(seg));

      seg = LineSegment3D.FromPoints(plane.Origin, plane.Origin + plane.AxisV);
      Assert.IsTrue(plane.Overlaps(seg));

      seg = LineSegment3D.FromPoints(p0, p1);
      Assert.IsTrue(plane.Overlaps(seg));

      seg = LineSegment3D.FromPoints(p0, p2);
      Assert.IsTrue(plane.Overlaps(seg));

      seg = LineSegment3D.FromPoints(p1, p2);
      Assert.IsTrue(plane.Overlaps(seg));

      (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();

      seg = LineSegment3D.FromPoints(
          new Point3D(a * p0.X + b * p1.X + c * p2.X, a * p0.Y + b * p1.Y + c * p2.Y, a * p0.Z + b * p1.Z + c * p2.Z),
          p2);
      Assert.IsTrue(plane.Overlaps(seg));
    }

    // Triangle with other basic primitives

    [Ignore]
    [RepeatedTestMethod(1)]
    public void TriangleToPlane() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void TriangleToLine() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void TriangleToRay() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void TriangleToLineSegment() {}
  }
}
