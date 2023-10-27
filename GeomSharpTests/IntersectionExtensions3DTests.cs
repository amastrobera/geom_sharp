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
  public class IntersectionExtensions3DTests {
    // Line, LineSegment, Ray cross test
    [RepeatedTestMethod(100)]
    public void LineToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine3D(decimal_precision: precision);

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
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray3D(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (p0)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray3D(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (p1)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going down
      ray = new Ray3D(mid, -u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (to mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going up
      ray = new Ray3D(mid, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (from mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going down
      ray = new Ray3D(p0, -u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (to p0)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going up
      ray = new Ray3D(p0, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (from p0)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the second extremity, going up
      ray = new Ray3D(p1, -u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (from p1)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the second extremity, going down
      ray = new Ray3D(p1, u_perp);
      Assert.IsTrue(line.Intersects(ray, precision),
                    "intersect forrreal (to p1)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector3D shift = RandomGenerator.MakeVector3D();
      ray = new Ray3D(p0 + 2 * shift, u);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + line.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      //      downwards
      ray = new Ray3D(p0 - 2 * shift, -u);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + line.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (mid-)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray3D(p0 - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (p0-)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray3D(p1 - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (p1-)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      in the middle, crossing
      ray = new Ray3D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (mid+)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray3D(p0 + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (p0+)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray3D(p1 + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray, precision),
                     "not intersect (p1+)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void LineToLineSegment() {
      int precision = RandomGenerator.MakeInt(0, 9);
      // input
      (var segment, var p0, var p1) = RandomGenerator.MakeLineSegment3D(decimal_precision: precision);

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
      Assert.IsTrue(segment.Intersects(line, precision),
                    "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      on the first extremity, crossing
      line = Line3D.FromDirection(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(line, precision),
                    "intersect forrreal (p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      on the second extremity, crossing
      line = Line3D.FromDirection(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(line, precision),
                    "intersect forrreal (p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the middle, going down
      line = Line3D.FromDirection(mid, -u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the middle, going up
      line = Line3D.FromDirection(mid, u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (from mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the first extremity, going down
      line = Line3D.FromDirection(p0, -u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the first extremity, going up
      line = Line3D.FromDirection(p0, u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the second extremity, going up
      line = Line3D.FromDirection(p1, -u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the second extremity, going down
      line = Line3D.FromDirection(p1, u_perp);
      Assert.IsTrue(
          segment.Intersects(line, precision),
          "intersect forrreal (to p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector3D shift = RandomGenerator.MakeVector3D();
      line = Line3D.FromDirection(p0 + 2 * shift, u);
      Assert.IsFalse(segment.Intersects(line, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + line.ToWkt(precision));

      //      downwards
      line = Line3D.FromDirection(p0 - 2 * shift, -u);
      Assert.IsFalse(segment.Intersects(line, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + line.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void LineSegmentToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var segment, var p0, var p1) = RandomGenerator.MakeLineSegment3D(decimal_precision: precision);

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
      Assert.IsTrue(segment.Intersects(ray, precision),
                    "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray3D(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(ray, precision),
                    "intersect forrreal (p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray3D(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(ray, precision),
                    "intersect forrreal (p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going down
      ray = new Ray3D(mid, -u_perp);
      Assert.IsTrue(
          segment.Intersects(ray, precision),
          "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going up
      ray = new Ray3D(mid, u_perp);
      Assert.IsTrue(
          segment.Intersects(ray, precision),
          "intersect forrreal (from mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going down
      ray = new Ray3D(p0, -u_perp);
      Assert.IsTrue(segment.Intersects(ray, precision),
                    "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going up
      ray = new Ray3D(p0, u_perp);
      Assert.IsTrue(
          segment.Intersects(ray, precision),
          "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the second extremity, going up
      ray = new Ray3D(p1, -u_perp);
      Assert.IsTrue(
          segment.Intersects(ray, precision),
          "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the second extremity, going down
      ray = new Ray3D(p1, u_perp);
      Assert.IsTrue(segment.Intersects(ray, precision),
                    "intersect forrreal (to p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector3D shift = RandomGenerator.MakeVector3D();
      ray = new Ray3D(p0 + 2 * shift, u);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      //      downwards
      ray = new Ray3D(p0 - 2 * shift, -u);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (mid-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray3D(p0 - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (p0-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray3D(p1 - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (p1-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      in the middle, crossing
      ray = new Ray3D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (mid+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the first extremity, crossing
      ray = new Ray3D(p0 + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (p0+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      on the second extremity, crossing
      ray = new Ray3D(p1 + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray, precision),
                     "not intersect (p1+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));
    }

    // Polyline with other primitives
    [Ignore]
    [RepeatedTestMethod(1)]
    public void LineToPolyline() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void RayToPolyline() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void LineSegmentToPolyline() {}

    // Plane with other basic primitives
    [RepeatedTestMethod(100)]
    public void PlaneToLine() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      // cache varibles
      var shift_vector =
          (Math.Round(plane.Normal.DotProduct(Vector3D.AxisZ), precision) == 0) ? plane.Normal : Vector3D.AxisZ;
      var parallel_vector1 = plane.AxisU;
      var parallel_vector2 = plane.AxisV;
      var shift = 2 * shift_vector;
      (double a, double b, double c) = (0, 0, 0);
      (Point3D pp1, Point3D pp2) = (null, null);
      Line3D line = null;

      // linear combination
      (a, b, c) = RandomGenerator.MakeLinearCombo3SumTo1();
      pp1 = new Point3D(a * p0.X + b * p1.X + c * p2.X,
                        a * p0.Y + b * p1.Y + c * p2.Y,
                        a * p0.Z + b * p1.Z + c * p2.Z);  // belongs to plane
      (int i, int max_inter) = (0, 100);
      while ((pp2 is null || pp2.AlmostEquals(pp1, precision)) && i < max_inter) {
        (a, b, c) = RandomGenerator.MakeLinearCombo3SumTo1();
        pp2 = new Point3D(a * p0.X + b * p1.X + c * p2.X,
                          a * p0.Y + b * p1.Y + c * p2.Y,
                          a * p0.Z + b * p1.Z + c * p2.Z);  // belongs to plane
        ++i;
      }
      if (i == max_inter) {
        throw new Exception("failed to generate pp2 != pp1, test abandoned");
      }

      // crosses
      line = Line3D.FromPoints(pp1 + shift, pp1 - shift, precision);
      Assert.IsTrue(plane.Intersects(line, precision), "failed p1+shift->p1-shift intersects");

      line = Line3D.FromPoints(p1, p1 + shift, precision);
      Assert.IsTrue(plane.Intersects(line, precision), "failed p1+shift intersects");

      line = Line3D.FromPoints(p1, p1 - shift, precision);
      Assert.IsTrue(plane.Intersects(line, precision), "failed p1-shift intersects");

      line = Line3D.FromPoints(pp1 + shift, pp2 - shift, precision);
      Assert.IsTrue(plane.Intersects(line, precision));

      // parallel
      line = Line3D.FromDirection(pp1, parallel_vector1);
      Assert.IsFalse(plane.Intersects(line, precision), "failed p1 parallel1");

      line = Line3D.FromDirection(pp1, parallel_vector2);
      Assert.IsFalse(plane.Intersects(line, precision), "failed p1 parallel2");

      line = Line3D.FromDirection(pp2, parallel_vector1);
      Assert.IsFalse(plane.Intersects(line, precision), "failed p2 parallel1");

      line = Line3D.FromDirection(pp2, parallel_vector2);
      Assert.IsFalse(plane.Intersects(line, precision), "failed p2 parallel2");

      // non-parallel two points and crossing
      line = Line3D.FromPoints(pp1 + shift, pp2 + shift / 2, precision);
      Assert.IsTrue(plane.Intersects(line, precision));

      line = Line3D.FromPoints(pp1 - shift, pp2 - shift / 2, precision);
      Assert.IsTrue(plane.Intersects(line, precision));
    }

    [RepeatedTestMethod(100)]
    public void PlaneToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      // cache varibles
      var shift_vector = plane.Normal.IsPerpendicular(Vector3D.AxisZ, precision) ? plane.Normal : Vector3D.AxisZ;

      var parallel_vector1 = plane.AxisU;
      var parallel_vector2 = plane.AxisV;
      var shift = 2 * shift_vector;
      (double a, double b, double c) = (0, 0, 0);
      (Point3D pp1, Point3D pp2) = (null, null);
      Ray3D ray = null;

      // linear combination
      (a, b, c) = RandomGenerator.MakeLinearCombo3SumTo1();
      pp1 = new Point3D(a * p0.X + b * p1.X + c * p2.X,
                        a * p0.Y + b * p1.Y + c * p2.Y,
                        a * p0.Z + b * p1.Z + c * p2.Z);  // belongs to plane
      (int i, int max_inter) = (0, 100);
      while ((pp2 is null || pp2.AlmostEquals(pp1, precision)) && i < max_inter) {
        (a, b, c) = RandomGenerator.MakeLinearCombo3SumTo1();
        pp2 = new Point3D(a * p0.X + b * p1.X + c * p2.X,
                          a * p0.Y + b * p1.Y + c * p2.Y,
                          a * p0.Z + b * p1.Z + c * p2.Z);  // belongs to plane
        ++i;
      }
      if (i == max_inter) {
        throw new Exception("failed to generate pp2 != pp1, test abandoned");
      }

      // crosses
      ray = new Ray3D(pp1 + shift, -shift_vector);
      Assert.IsTrue(plane.Intersects(ray, precision), "failed p1+shift down intersects");

      ray = new Ray3D(pp1 - shift, shift_vector);
      Assert.IsTrue(plane.Intersects(ray, precision), "failed p1-shift up intersects");

      ray = new Ray3D(pp2 + shift, -shift_vector);
      Assert.IsTrue(plane.Intersects(ray, precision), "failed p2+shift down intersects");

      ray = new Ray3D(pp2 - shift, shift_vector);
      Assert.IsTrue(plane.Intersects(ray, precision), "failed p2-shift up intersects");

      // parallel
      ray = new Ray3D(pp1, parallel_vector1);
      Assert.IsFalse(plane.Intersects(ray, precision), "failed p1 parallel1");

      ray = new Ray3D(pp1, parallel_vector2);
      Assert.IsFalse(plane.Intersects(ray, precision), "failed p1 parallel2");

      ray = new Ray3D(pp2, parallel_vector1);
      Assert.IsFalse(plane.Intersects(ray, precision), "failed p2 parallel1");

      ray = new Ray3D(pp2, parallel_vector2);
      Assert.IsFalse(plane.Intersects(ray, precision), "failed p2 parallel2");

      // non-parallel non crossing
      ray = new Ray3D(pp1 + shift, shift_vector);
      Assert.IsFalse(plane.Intersects(ray, precision), "failed p1+shift up no intersects");

      ray = new Ray3D(pp1 - shift, -shift_vector);
      Assert.IsFalse(plane.Intersects(ray, precision), "failed p1-shift down no intersects");

      ray = new Ray3D(pp2 + shift, shift_vector);
      Assert.IsFalse(plane.Intersects(ray, precision), "failed p2+shift up no intersects");

      ray = new Ray3D(pp2 - shift, -shift_vector);
      Assert.IsFalse(plane.Intersects(ray, precision), "failed p2-shift down no intersects");
    }

    [RepeatedTestMethod(100)]
    public void PlaneToLineSegment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      // cache varibles
      var shift_vector =
          (Math.Round(plane.Normal.DotProduct(Vector3D.AxisZ), precision) == 0) ? plane.Normal : Vector3D.AxisZ;
      var shift = 2 * shift_vector;
      (double a, double b, double c) = (0, 0, 0);
      (Point3D pp1, Point3D pp2) = (null, null);
      LineSegment3D segment = null;

      // linear combination
      (a, b, c) = RandomGenerator.MakeLinearCombo3SumTo1();
      pp1 = new Point3D(a * p0.X + b * p1.X + c * p2.X,
                        a * p0.Y + b * p1.Y + c * p2.Y,
                        a * p0.Z + b * p1.Z + c * p2.Z);  // belongs to plane
      (int i, int max_inter) = (0, 100);
      while ((pp2 is null || pp2.AlmostEquals(pp1)) && i < max_inter) {
        (a, b, c) = RandomGenerator.MakeLinearCombo3SumTo1();
        pp2 = new Point3D(a * p0.X + b * p1.X + c * p2.X,
                          a * p0.Y + b * p1.Y + c * p2.Y,
                          a * p0.Z + b * p1.Z + c * p2.Z);  // belongs to plane
        ++i;
      }
      if (i == max_inter) {
        throw new Exception("failed to generate pp2 != pp1, test abandoned");
      }

      // crosses
      segment = LineSegment3D.FromPoints(pp1 + shift, pp1 - shift, precision);
      Assert.IsTrue(plane.Intersects(segment, precision), "failed p1+shift->p1-shift intersects");

      segment = LineSegment3D.FromPoints(p1, p1 + shift, precision);
      Assert.IsTrue(plane.Intersects(segment, precision), "failed p1+shift intersects");

      segment = LineSegment3D.FromPoints(p1, p1 - shift, precision);
      Assert.IsTrue(plane.Intersects(segment, precision), "failed p1-shift intersects");

      segment = LineSegment3D.FromPoints(pp1 + shift, pp2 - shift, precision);
      Assert.IsTrue(plane.Intersects(segment, precision));

      // parallel
      segment = LineSegment3D.FromPoints(pp1 + shift, pp2 + shift, precision);
      Assert.IsFalse(plane.Intersects(segment, precision), "failed p1-p2 parallel shift(+)");

      segment = LineSegment3D.FromPoints(pp1 - shift, pp2 - shift, precision);
      Assert.IsFalse(plane.Intersects(segment, precision));

      // non-parallel and non crossing
      segment = LineSegment3D.FromPoints(pp1 + shift, pp2 + shift / 2, precision);
      Assert.IsFalse(plane.Intersects(segment, precision));

      segment = LineSegment3D.FromPoints(pp1 - shift, pp2 - shift / 2, precision);
      Assert.IsFalse(plane.Intersects(segment, precision));
    }

    [Ignore]
    [RepeatedTestMethod(1)]
    public void PlaneToPolyline() {}

    [RepeatedTestMethod(100)]
    public void PlaneToPolygon() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // 3D
      (var poly, var cm, double radius, int n) = RandomGenerator.MakeConvexPolygon3D(decimal_precision: precision);

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (poly is null) {
        return;
      }

      // cache varibles
      Plane other_plane;
      var poly_plane = poly.RefPlane();
      var poly_plane_norm = poly_plane.Normal;
      var poly_plane_axis_u = poly_plane.AxisU;
      var poly_plane_axis_v = poly_plane.AxisV;

      cm = poly.CenterOfMass();
      n = poly.Size;

      // test 1: a plane parallel to the polygon does not intersect
      other_plane = Plane.FromPointAndNormal(cm + 2 * poly_plane_norm, poly_plane_norm, precision);
      Assert.IsFalse(other_plane.Intersects(poly, precision),
                     "a plane parallel to the polygon does not intersect, \n\tplane = " + other_plane.ToWkt(precision) +
                         "\n\tpoly=" + poly.ToWkt(precision));

      // test 2: a plane perpendicular to the polygon intersects
      for (int i = 0; i < n; ++i) {
        var p = poly[i];
        var p_axis = (p - cm).Normalize();

        other_plane = Plane.FromPointAndNormal(cm, p_axis, precision);
        Assert.IsTrue(other_plane.Intersects(poly, precision),
                      "a plane perpendicular to the polygon intersects, iter " + i.ToString() + "/" + n.ToString() +
                          "\n\tplane = " + other_plane.ToWkt(precision) + "\n\tpoly=" + poly.ToWkt(precision));
      }

      // test 3: a plane perpendicular to the polygon, but outside the polygon, does not intersect
      for (int i = 0; i < n; ++i) {
        var p = poly[i];
        var p_axis = (p - cm).Normalize();

        other_plane = Plane.FromPointAndNormal(p + 2 * p_axis, p_axis, precision);
        Assert.IsFalse(other_plane.Intersects(poly, precision),
                       "a plane perpendicular to the polygon, but outside the polygon, does not intersect, iter " +
                           i.ToString() + "/" + n.ToString() + "\n\tplane = " + other_plane.ToWkt(precision) +
                           "\n\tpoly=" + poly.ToWkt(precision));
      }
    }

    // Triangle with other basic primitives
    [RepeatedTestMethod(100)]
    public void TriangleToPlane() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_triangle = RandomGenerator.MakeTriangle3D(decimal_precision: precision);
      var t = random_triangle.Triangle;

      if (t is null) {
        return;
      }
      // cache varibles
      Point3D cm = t.CenterOfMass();
      UnitVector3D n = t.RefPlane().Normal;
      UnitVector3D n_perp =
          n.CrossProduct((n.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();
      Plane plane;

      // no intersection (same plane)
      plane = t.RefPlane();
      Assert.IsFalse(t.Intersects(plane, precision), "no intersection (same plane)");

      // intersection (crosses)
      plane = Plane.FromPointAndNormal(cm, n_perp, precision);
      Assert.IsTrue(t.Intersects(plane, precision), "intersection (crosses)");
    }

    [RepeatedTestMethod(100)]
    public void TriangleToLine() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_triangle = RandomGenerator.MakeTriangle3D(decimal_precision: precision);
      var t = random_triangle.Triangle;

      if (t is null) {
        return;
      }
      // cache varibles
      Point3D cm = t.CenterOfMass();
      Point3D p1, p2;  //  points to use for further calculation
      var plane = t.RefPlane();
      var normal = plane.Normal;
      var vertical_shift_vector = normal.IsPerpendicular(Vector3D.AxisZ, precision) ? normal : Vector3D.AxisZ;
      var parallel_vector_u = plane.AxisU;
      var parallel_vector_v = plane.AxisV;
      Line3D line = null;

      // crosses
      line = Line3D.FromDirection(cm, vertical_shift_vector);
      Assert.IsTrue(t.Intersects(line, precision), "failed cm+shift intersects");

      // crosses with one point in one of the vertices
      line = Line3D.FromDirection(t.P0, vertical_shift_vector);
      Assert.IsTrue(t.Intersects(line, precision), "failed p0+shift down intersects");

      line = Line3D.FromDirection(t.P1, vertical_shift_vector);
      Assert.IsTrue(t.Intersects(line, precision), "failed p1+shift down intersects");

      line = Line3D.FromDirection(t.P2, vertical_shift_vector);
      Assert.IsTrue(t.Intersects(line, precision), "failed p2+shift down intersects");

      // crosses with one point on the triangle edges
      p1 = cm + 2 * vertical_shift_vector;
      p2 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      line = Line3D.FromDirection(p1, (p2 - p1).Normalize());
      Assert.IsTrue(t.Intersects(line, precision), "failed cm -> p0-p1 intersects");

      p1 = cm + 2 * vertical_shift_vector;
      p2 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      line = Line3D.FromDirection(p1, (p2 - p1).Normalize());
      Assert.IsTrue(t.Intersects(line, precision), "failed cm -> p1-p2 intersects");

      p1 = cm + 2 * vertical_shift_vector;
      p2 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      line = Line3D.FromDirection(p1, (p2 - p1).Normalize());
      Assert.IsTrue(t.Intersects(line, precision), "failed cm -> p2-p0 intersects");

      // overlap (crosses in 2D, no intersection in 3D)
      line = Line3D.FromDirection(cm, parallel_vector_u);
      Assert.IsFalse(t.Intersects(line, precision),
                     "failed cm to parallel U overlaps (intersection 2D, no intersect in 3D)");

      line = Line3D.FromDirection(cm, parallel_vector_v);
      Assert.IsFalse(t.Intersects(line, precision),
                     "failed cm to parallel V overlaps (intersection 2D, no intersect in 3D)");

      // crosses the plane but not the triangle
      p1 = cm + 2 * vertical_shift_vector;
      p2 = t.P1 + parallel_vector_u * 2;
      line = Line3D.FromDirection(p1, (p2 - p1).Normalize());
      Assert.IsFalse(t.Intersects(line, precision), "failed cm -> p1 intersects plane but not triangle");

      p1 = cm + 2 * vertical_shift_vector;
      p2 = t.P2 + parallel_vector_v * 2;
      line = Line3D.FromDirection(p1, (p2 - p1).Normalize());
      Assert.IsFalse(t.Intersects(line, precision), "failed cm -> p2 intersects plane but not triangle");

      p1 = cm + 2 * vertical_shift_vector;
      p2 = t.P0 - parallel_vector_v * 2;
      line = Line3D.FromDirection(p1, (p2 - p1).Normalize());
      Assert.IsFalse(t.Intersects(line, precision), "failed cm -> p0 intersects plane but not triangle");
    }

    [RepeatedTestMethod(100)]
    public void TriangleToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_triangle = RandomGenerator.MakeTriangle3D(decimal_precision: precision);
      var t = random_triangle.Triangle;

      if (t is null) {
        return;
      }
      // cache varibles
      Point3D cm = t.CenterOfMass();
      Point3D p1, p2;  //  points to use for further calculation
      var plane = t.RefPlane();
      var normal = plane.Normal;
      var vertical_shift_vector = normal.IsPerpendicular(Vector3D.AxisZ, precision) ? normal : Vector3D.AxisZ;
      var parallel_vector_u = plane.AxisU;
      var parallel_vector_v = plane.AxisV;
      Ray3D ray = null;

      // crosses
      ray = new Ray3D(cm + 2 * vertical_shift_vector, -vertical_shift_vector);
      Assert.IsTrue(t.Intersects(ray, precision), "failed cm+shift down intersects");

      ray = new Ray3D(cm - 2 * vertical_shift_vector, vertical_shift_vector);
      Assert.IsTrue(t.Intersects(ray, precision), "failed cm-shift up intersects");

      // crosses with one point in one of the vertices
      ray = new Ray3D(t.P0 + 2 * vertical_shift_vector, -vertical_shift_vector);
      Assert.IsTrue(t.Intersects(ray, precision), "failed p0+shift down intersects");

      ray = new Ray3D(t.P0 - 2 * vertical_shift_vector, vertical_shift_vector);
      Assert.IsTrue(t.Intersects(ray, precision), "failed p0-shift up intersects");

      ray = new Ray3D(t.P1 + 2 * vertical_shift_vector, -vertical_shift_vector);
      Assert.IsTrue(t.Intersects(ray, precision), "failed p1+shift down intersects");

      ray = new Ray3D(t.P1 - 2 * vertical_shift_vector, vertical_shift_vector);
      Assert.IsTrue(t.Intersects(ray, precision), "failed p1-shift up intersects");

      ray = new Ray3D(t.P2 + 2 * vertical_shift_vector, -vertical_shift_vector);
      Assert.IsTrue(t.Intersects(ray, precision), "failed p2+shift down intersects");

      ray = new Ray3D(t.P2 - 2 * vertical_shift_vector, vertical_shift_vector);
      Assert.IsTrue(t.Intersects(ray, precision), "failed p2-shift up intersects");

      // crosses with one point on the triangle edges
      p1 = cm + 2 * vertical_shift_vector;
      p2 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      ray = new Ray3D(p1, (p2 - p1).Normalize());
      Assert.IsTrue(t.Intersects(ray, precision), "failed cm -> p0-p1 intersects");

      p1 = cm + 2 * vertical_shift_vector;
      p2 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      ray = new Ray3D(p1, (p2 - p1).Normalize());
      Assert.IsTrue(t.Intersects(ray, precision), "failed cm -> p1-p2 intersects");

      p1 = cm + 2 * vertical_shift_vector;
      p2 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      ray = new Ray3D(p1, (p2 - p1).Normalize());
      Assert.IsTrue(t.Intersects(ray, precision), "failed cm -> p2-p0 intersects");

      // overlap (crosses in 2D, no intersection in 3D)
      ray = new Ray3D(cm, parallel_vector_u);
      Assert.IsFalse(t.Intersects(ray, precision),
                     "failed cm to parallel U overlaps (intersection 2D, no intersect in 3D)");

      ray = new Ray3D(cm, parallel_vector_v);
      Assert.IsFalse(t.Intersects(ray, precision),
                     "failed cm to parallel V overlaps (intersection 2D, no intersect in 3D)");

      // crosses the plane but not the triangle
      p1 = cm + 2 * vertical_shift_vector;
      p2 = t.P1 + parallel_vector_u * 2;
      ray = new Ray3D(p1, (p2 - p1).Normalize());
      Assert.IsFalse(t.Intersects(ray, precision), "failed cm -> p1 intersects plane but not triangle");

      p1 = cm + 2 * vertical_shift_vector;
      p2 = t.P2 + parallel_vector_v * 2;
      ray = new Ray3D(p1, (p2 - p1).Normalize());
      Assert.IsFalse(t.Intersects(ray, precision), "failed cm -> p2 intersects plane but not triangle");

      p1 = cm + 2 * vertical_shift_vector;
      p2 = t.P0 - parallel_vector_v * 2;
      ray = new Ray3D(p1, (p2 - p1).Normalize());
      Assert.IsFalse(t.Intersects(ray, precision), "failed cm -> p0 intersects plane but not triangle");
    }

    [RepeatedTestMethod(100)]
    public void TriangleToLineSegment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_triangle = RandomGenerator.MakeTriangle3D(decimal_precision: precision);
      var t = random_triangle.Triangle;
      (var p0, var p1, var p2) = (random_triangle.p0, random_triangle.p1, random_triangle.p2);

      if (t is null) {
        return;
      }

      // temp data
      Point3D cm = t.CenterOfMass();
      LineSegment3D segment;
      Point3D lp0, lp1;
      Vector3D U;
      UnitVector3D tnorm = t.RefPlane().Normal;

      // case 1: intersect

      //      perpendicular to the CM
      segment = LineSegment3D.FromPoints(cm - tnorm * 2, cm + tnorm * 2, precision);
      Assert.IsTrue(segment.Intersects(t, precision));

      //    perpendicular to an edge
      lp0 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0 - tnorm * 2, lp0 + tnorm * 2, precision);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through perp on p0-p1 edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0 - tnorm * 2, lp0 + tnorm * 2, precision);
      Assert.IsTrue(segment.Intersects(t, precision),
                    "segment through perp on p1-p2 edge" + "\n\t t=" + t.ToWkt(precision) +
                        ", segment=" + segment.ToWkt(precision));

      lp0 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0 - tnorm * 2, lp0 + tnorm * 2, precision);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through perp on p2-p0 edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      // perpendicular to a vertex
      lp0 = t.P0;
      segment = LineSegment3D.FromPoints(lp0 - tnorm * 2, lp0 + tnorm * 2, precision);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through perp on p0 vert t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = t.P1;
      segment = LineSegment3D.FromPoints(lp0 - tnorm * 2, lp0 + tnorm * 2, precision);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through perp on p1 vert t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = t.P2;
      segment = LineSegment3D.FromPoints(lp0 - tnorm * 2, lp0 + tnorm * 2, precision);
      Assert.IsTrue(
          segment.Intersects(t, precision),
          "segment through perp on p2 vert t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      // case 2: line intersects but segment outside the scope of the triangle (no intersect)
      //        perpendicular
      lp0 = cm;
      segment = LineSegment3D.FromPoints(lp0 + tnorm * 2, lp0 + tnorm * 4, precision);
      Assert.IsFalse(segment.Intersects(t, precision));

      lp0 = t.P0;
      segment = LineSegment3D.FromPoints(lp0 - tnorm * 2, lp0 - tnorm * 4, precision);
      Assert.IsFalse(segment.Intersects(t, precision));

      //      skewed
      lp0 = cm + tnorm * 4;
      lp1 = t.P0 + tnorm * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(segment.Intersects(t, precision));

      lp0 = cm - tnorm * 2;
      lp1 = t.P0 - tnorm * 4;
      segment = LineSegment3D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(segment.Intersects(t, precision));

      // case 3: parallel (no intersect)
      lp0 = cm - tnorm * 2;
      lp1 = t.P0 - tnorm * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(segment.Intersects(t, precision));

      lp0 = cm + tnorm * 2;
      lp1 = t.P0 + tnorm * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(segment.Intersects(t, precision));

      U = (t.P1 - t.P0);
      lp0 = t.P2 + (cm - t.P2) + U * 2;
      lp1 = t.P2 + (cm - t.P2) - U * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "external segment parallel to an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      U = (t.P2 - t.P1);
      lp0 = t.P0 + (cm - t.P0) + U * 2;
      lp1 = t.P0 + (cm - t.P0) - U * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "external segment parallel to an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      U = (t.P0 - t.P2);
      lp0 = t.P1 + (cm - t.P1) + U * 2;
      lp1 = t.P1 + (cm - t.P1) - U * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "external segment parallel to an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      // case 4: overlap (no intersect)
      lp0 = t.P0;
      lp1 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(segment.Intersects(t, precision),
                     "segment overlapping an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = t.P1;
      lp1 = Point3D.FromVector((t.P2.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(segment.Intersects(t, precision),
                     "segment overlapping an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = t.P2;
      lp1 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0, lp1, precision);
      Assert.IsFalse(segment.Intersects(t, precision),
                     "segment overlapping an edge t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      //    contained inside the triangle
      lp0 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment3D.FromPoints(lp0 + U / 4, lp1 - U / 4, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "segment contained in the triangle t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment3D.FromPoints(lp0 + U / 4, lp1 - U / 4, precision);
      Assert.IsFalse(
          segment.Intersects(t, precision),
          "segment contained in the triangle t=" + t.ToWkt(precision) + ", segment=" + segment.ToWkt(precision));

      lp0 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment3D.FromPoints(lp0 + U / 4, lp1 - U / 4, precision);
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
