﻿// internal
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
  public class IntersectionExtensions3DTests {
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
      Assert.IsTrue(line.Intersects(ray), "intersect forrreal (mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray3D(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(line.Intersects(ray), "intersect forrreal (p0)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray3D(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(line.Intersects(ray), "intersect forrreal (p1)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going down
      ray = new Ray3D(mid, -u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (to mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going up
      ray = new Ray3D(mid, u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (from mid)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going down
      ray = new Ray3D(p0, -u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (to p0)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going up
      ray = new Ray3D(p0, u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (from p0)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the second extremity, going up
      ray = new Ray3D(p1, -u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (from p1)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the second extremity, going down
      ray = new Ray3D(p1, u_perp);
      Assert.IsTrue(line.Intersects(ray),
                    "intersect forrreal (to p1)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector3D shift = RandomGenerator.MakeVector3D();
      ray = new Ray3D(p0 + 2 * shift, u);
      Assert.IsFalse(
          line.Intersects(ray),
          "no intersection (parallel, shift upwards random vector)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      downwards
      ray = new Ray3D(p0 - 2 * shift, -u);
      Assert.IsFalse(
          line.Intersects(ray),
          "no intersection (parallel, shift downwards random vector)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (mid-)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray3D(p0 - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (p0-)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray3D(p1 - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (p1-)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      in the middle, crossing
      ray = new Ray3D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (mid+)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray3D(p0 + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (p0+)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray3D(p1 + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Intersects(ray), "not intersect (p1+)" + "\ns1=" + line.ToWkt() + ", s2=" + ray.ToWkt());
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
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      on the first extremity, crossing
      line = Line3D.FromDirection(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      on the second extremity, crossing
      line = Line3D.FromDirection(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the middle, going down
      line = Line3D.FromDirection(mid, -u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the middle, going up
      line = Line3D.FromDirection(mid, u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (from mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the first extremity, going down
      line = Line3D.FromDirection(p0, -u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the first extremity, going up
      line = Line3D.FromDirection(p0, u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the second extremity, going up
      line = Line3D.FromDirection(p1, -u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      //      just one point in the the second extremity, going down
      line = Line3D.FromDirection(p1, u_perp);
      Assert.IsTrue(segment.Intersects(line),
                    "intersect forrreal (to p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + line.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector3D shift = RandomGenerator.MakeVector3D();
      line = Line3D.FromDirection(p0 + 2 * shift, u);
      Assert.IsFalse(segment.Intersects(line),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + line.ToWkt());

      //      downwards
      line = Line3D.FromDirection(p0 - 2 * shift, -u);
      Assert.IsFalse(segment.Intersects(line),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + line.ToWkt());
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
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray3D(p0 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray3D(p1 - 2 * u_perp, u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going down
      ray = new Ray3D(mid, -u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the middle, going up
      ray = new Ray3D(mid, u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (from mid)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going down
      ray = new Ray3D(p0, -u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the first extremity, going up
      ray = new Ray3D(p0, u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the second extremity, going up
      ray = new Ray3D(p1, -u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      just one point in the the second extremity, going down
      ray = new Ray3D(p1, u_perp);
      Assert.IsTrue(segment.Intersects(ray),
                    "intersect forrreal (to p1)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector3D shift = RandomGenerator.MakeVector3D();
      ray = new Ray3D(p0 + 2 * shift, u);
      Assert.IsFalse(segment.Intersects(ray),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + ray.ToWkt());

      //      downwards
      ray = new Ray3D(p0 - 2 * shift, -u);
      Assert.IsFalse(segment.Intersects(ray),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt() +
                         ", s2=" + ray.ToWkt());

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (mid-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray3D(p0 - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (p0-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray3D(p1 - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (p1-)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      in the middle, crossing
      ray = new Ray3D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (mid+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the first extremity, crossing
      ray = new Ray3D(p0 + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (p0+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());

      //      on the second extremity, crossing
      ray = new Ray3D(p1 + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Intersects(ray),
                     "not intersect (p1+)" + "\ns1=" + segment.ToWkt() + ", s2=" + ray.ToWkt());
    }

    // Plane with other basic primitives
    [RepeatedTestMethod(100)]
    public void PlaneToLine() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      // cache varibles
      var shift_vector = (Math.Round(plane.Normal.DotProduct(Vector3D.AxisZ), Constants.NINE_DECIMALS) == 0)
                             ? plane.Normal
                             : Vector3D.AxisZ;
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
      line = Line3D.FromPoints(pp1 + shift, pp1 - shift);
      Assert.IsTrue(plane.Intersects(line), "failed p1+shift->p1-shift intersects");

      line = Line3D.FromPoints(p1, p1 + shift);
      Assert.IsTrue(plane.Intersects(line), "failed p1+shift intersects");

      line = Line3D.FromPoints(p1, p1 - shift);
      Assert.IsTrue(plane.Intersects(line), "failed p1-shift intersects");

      line = Line3D.FromPoints(pp1 + shift, pp2 - shift);
      Assert.IsTrue(plane.Intersects(line));

      // parallel
      line = Line3D.FromDirection(pp1, parallel_vector1);
      Assert.IsFalse(plane.Intersects(line), "failed p1 parallel1");

      line = Line3D.FromDirection(pp1, parallel_vector2);
      Assert.IsFalse(plane.Intersects(line), "failed p1 parallel2");

      line = Line3D.FromDirection(pp2, parallel_vector1);
      Assert.IsFalse(plane.Intersects(line), "failed p2 parallel1");

      line = Line3D.FromDirection(pp2, parallel_vector2);
      Assert.IsFalse(plane.Intersects(line), "failed p2 parallel2");

      // non-parallel two points and crossing
      line = Line3D.FromPoints(pp1 + shift, pp2 + shift / 2);
      Assert.IsTrue(plane.Intersects(line));

      line = Line3D.FromPoints(pp1 - shift, pp2 - shift / 2);
      Assert.IsTrue(plane.Intersects(line));
    }

    [RepeatedTestMethod(100)]
    public void PlaneToRay() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      // cache varibles
      var shift_vector = plane.Normal.IsPerpendicular(Vector3D.AxisZ) ? plane.Normal : Vector3D.AxisZ;

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
      ray = new Ray3D(pp1 + shift, -shift_vector);
      Assert.IsTrue(plane.Intersects(ray), "failed p1+shift down intersects");

      ray = new Ray3D(pp1 - shift, shift_vector);
      Assert.IsTrue(plane.Intersects(ray), "failed p1-shift up intersects");

      ray = new Ray3D(pp2 + shift, -shift_vector);
      Assert.IsTrue(plane.Intersects(ray), "failed p2+shift down intersects");

      ray = new Ray3D(pp2 - shift, shift_vector);
      Assert.IsTrue(plane.Intersects(ray), "failed p2-shift up intersects");

      // parallel
      ray = new Ray3D(pp1, parallel_vector1);
      Assert.IsFalse(plane.Intersects(ray), "failed p1 parallel1");

      ray = new Ray3D(pp1, parallel_vector2);
      Assert.IsFalse(plane.Intersects(ray), "failed p1 parallel2");

      ray = new Ray3D(pp2, parallel_vector1);
      Assert.IsFalse(plane.Intersects(ray), "failed p2 parallel1");

      ray = new Ray3D(pp2, parallel_vector2);
      Assert.IsFalse(plane.Intersects(ray), "failed p2 parallel2");

      // non-parallel non crossing
      ray = new Ray3D(pp1 + shift, shift_vector);
      Assert.IsFalse(plane.Intersects(ray), "failed p1+shift up no intersects");

      ray = new Ray3D(pp1 - shift, -shift_vector);
      Assert.IsFalse(plane.Intersects(ray), "failed p1-shift down no intersects");

      ray = new Ray3D(pp2 + shift, shift_vector);
      Assert.IsFalse(plane.Intersects(ray), "failed p2+shift up no intersects");

      ray = new Ray3D(pp2 - shift, -shift_vector);
      Assert.IsFalse(plane.Intersects(ray), "failed p2-shift down no intersects");
    }

    [RepeatedTestMethod(100)]
    public void PlaneToLineSegment() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      // cache varibles
      var shift_vector = (Math.Round(plane.Normal.DotProduct(Vector3D.AxisZ), Constants.NINE_DECIMALS) == 0)
                             ? plane.Normal
                             : Vector3D.AxisZ;
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
      segment = LineSegment3D.FromPoints(pp1 + shift, pp1 - shift);
      Assert.IsTrue(plane.Intersects(segment), "failed p1+shift->p1-shift intersects");

      segment = LineSegment3D.FromPoints(p1, p1 + shift);
      Assert.IsTrue(plane.Intersects(segment), "failed p1+shift intersects");

      segment = LineSegment3D.FromPoints(p1, p1 - shift);
      Assert.IsTrue(plane.Intersects(segment), "failed p1-shift intersects");

      segment = LineSegment3D.FromPoints(pp1 + shift, pp2 - shift);
      Assert.IsTrue(plane.Intersects(segment));

      // parallel
      segment = LineSegment3D.FromPoints(pp1 + shift, pp2 + shift);
      Assert.IsFalse(plane.Intersects(segment), "failed p1-p2 parallel shift(+)");

      segment = LineSegment3D.FromPoints(pp1 - shift, pp2 - shift);
      Assert.IsFalse(plane.Intersects(segment));

      // non-parallel and non crossing
      segment = LineSegment3D.FromPoints(pp1 + shift, pp2 + shift / 2);
      Assert.IsFalse(plane.Intersects(segment));

      segment = LineSegment3D.FromPoints(pp1 - shift, pp2 - shift / 2);
      Assert.IsFalse(plane.Intersects(segment));
    }

    // Triangle with other basic primitives

    [RepeatedTestMethod(1)]
    public void TriangleToPlane() {
      Assert.IsTrue(false);
    }

    [RepeatedTestMethod(1)]
    public void TriangleToLine() {
      Assert.IsTrue(false);
    }

    [RepeatedTestMethod(100)]
    public void TriangleToRay() {
      var random_triangle = RandomGenerator.MakeTriangle3D();
      var t = random_triangle.Triangle;
      (var p0, var p1, var p2) = (random_triangle.p0, random_triangle.p1, random_triangle.p2);

      if (t is null) {
        return;
      }
      // cache varibles
      var plane = t.RefPlane();
      var normal = plane.Normal;
      var shift_vector = normal.IsPerpendicular(Vector3D.AxisZ) ? normal : Vector3D.AxisZ;
      var parallel_vector1 = plane.AxisU;
      var parallel_vector2 = plane.AxisV;
      var shift = 2 * shift_vector;
      (double a, double b, double c) = (0, 0, 0);
      (Point3D pp1, Point3D pp2, Point3D pp3, Point3D pp4) = (null, null, null, null);
      Ray3D ray = null;

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
      ray = new Ray3D(pp1 + shift, -shift_vector);
      Assert.IsTrue(t.Intersects(ray), "failed p1+shift down intersects");

      ray = new Ray3D(pp1 - shift, shift_vector);
      Assert.IsTrue(t.Intersects(ray), "failed p1-shift up intersects");

      ray = new Ray3D(pp2 + shift, -shift_vector);
      Assert.IsTrue(t.Intersects(ray), "failed p2+shift down intersects");

      ray = new Ray3D(pp2 - shift, shift_vector);
      Assert.IsTrue(t.Intersects(ray), "failed p2-shift up intersects");

      // crosses with one point in
      ray = new Ray3D(pp1, -shift_vector);
      Assert.IsTrue(t.Intersects(ray), "failed p1-shift intersects one point in");

      ray = new Ray3D(pp1, shift_vector);
      Assert.IsTrue(t.Intersects(ray), "failed p1+shift intersects one point in");

      // crosses the plane but not the triangle
      pp3 = pp1 + shift;
      pp4 = p1 + 2 * (p1 - p0);
      shift_vector = (pp4 - pp3).Normalize();
      ray = new Ray3D(pp3, shift_vector);
      Assert.IsFalse(t.Intersects(ray), "failed p1+shift down intersects plane but not triangle");

      pp3 = pp1 - shift;
      pp4 = p1 + 2 * (p1 - p0);
      shift_vector = (pp3 - pp4).Normalize();
      ray = new Ray3D(pp3, shift_vector);
      Assert.IsFalse(t.Intersects(ray), "failed p1-shift up intersects plane but not triangle");

      pp3 = pp1 + shift;
      pp4 = p2 + 2 * (p2 - p0);
      shift_vector = (pp4 - pp3).Normalize();
      ray = new Ray3D(pp3, shift_vector);
      Assert.IsFalse(t.Intersects(ray), "failed p2+shift down intersects plane but not triangle");

      pp3 = pp1 - shift;
      pp4 = p2 + 2 * (p2 - p0);
      shift_vector = (pp4 - pp3).Normalize();
      ray = new Ray3D(pp3, shift_vector);
      Assert.IsFalse(t.Intersects(ray), "failed p2-shift up intersects plane but not triangle");

      // parallel
      ray = new Ray3D(pp1, parallel_vector1);
      Assert.IsFalse(t.Intersects(ray), "failed p1 parallel1");

      ray = new Ray3D(pp1, parallel_vector2);
      Assert.IsFalse(t.Intersects(ray), "failed p1 parallel2");

      ray = new Ray3D(pp2, parallel_vector1);
      Assert.IsFalse(t.Intersects(ray), "failed p2 parallel1");

      ray = new Ray3D(pp2, parallel_vector2);
      Assert.IsFalse(t.Intersects(ray), "failed p2 parallel2");

      // non-parallel non crossing
      ray = new Ray3D(pp1 + shift, shift_vector);
      Assert.IsFalse(t.Intersects(ray), "failed p1+shift up no intersects");

      ray = new Ray3D(pp1 - shift, -shift_vector);
      Assert.IsFalse(t.Intersects(ray), "failed p1-shift down no intersects");

      ray = new Ray3D(pp2 + shift, shift_vector);
      Assert.IsFalse(t.Intersects(ray), "failed p2+shift up no intersects");

      ray = new Ray3D(pp2 - shift, -shift_vector);
      Assert.IsFalse(t.Intersects(ray), "failed p2-shift down no intersects");
    }

    [RepeatedTestMethod(1)]
    public void TriangleToLineSegment() {
      var random_triangle = RandomGenerator.MakeTriangle3D();
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

      // case 1: intersect
      //    perpendicular to an edge
      lp0 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0 + (lp0 - cm), cm);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through perp on p0-p1 edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0 + (lp0 - cm), cm);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through perp on p1-p2 edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0 + (lp0 - cm), cm);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through perp on p2-p0 edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      //    strike through two edges
      lp0 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through mid points of edges t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through mid points of edges t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t),
                    "segment through mid points of edges t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      //    strike through one point
      U = (t.P2 - t.P1);
      lp0 = t.P0 + U * 2;
      lp1 = t.P0 - U * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t), "segment through a vertex t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      U = (t.P0 - t.P2);
      lp0 = t.P1 + U * 2;
      lp1 = t.P1 - U * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t), "segment through a vertex t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      U = (t.P1 - t.P0);
      lp0 = t.P2 + U * 2;
      lp1 = t.P2 - U * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsTrue(segment.Intersects(t), "segment through a vertex t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      // case 2: parallel (no intersect)
      U = (t.P1 - t.P0);
      lp0 = t.P2 + (cm - t.P2) + U * 2;
      lp1 = t.P2 + (cm - t.P2) - U * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "external segment parallel to an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      U = (t.P2 - t.P1);
      lp0 = t.P0 + (cm - t.P0) + U * 2;
      lp1 = t.P0 + (cm - t.P0) - U * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "external segment parallel to an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      U = (t.P0 - t.P2);
      lp0 = t.P1 + (cm - t.P1) + U * 2;
      lp1 = t.P1 + (cm - t.P1) - U * 2;
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "external segment parallel to an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      // case 3: overlap (no intersect)
      lp0 = t.P0;
      lp1 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "segment overlapping an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = t.P1;
      lp1 = Point3D.FromVector((t.P2.ToVector() + t.P2.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "segment overlapping an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = t.P2;
      lp1 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      segment = LineSegment3D.FromPoints(lp0, lp1);
      Assert.IsFalse(segment.Intersects(t),
                     "segment overlapping an edge t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      //    contained inside the triangle
      lp0 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      lp1 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment3D.FromPoints(lp0 + U / 4, lp1 - U / 4);
      Assert.IsFalse(segment.Intersects(t),
                     "segment contained in the triangle t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      lp1 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment3D.FromPoints(lp0 + U / 4, lp1 - U / 4);
      Assert.IsFalse(segment.Intersects(t),
                     "segment contained in the triangle t=" + t.ToWkt() + ", segment=" + segment.ToWkt());

      lp0 = Point3D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      lp1 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      U = (lp1 - lp0);
      segment = LineSegment3D.FromPoints(lp0 + U / 4, lp1 - U / 4);
      Assert.IsFalse(segment.Intersects(t),
                     "segment contained in the triangle t=" + t.ToWkt() + ", segment=" + segment.ToWkt());
    }
  }
}
