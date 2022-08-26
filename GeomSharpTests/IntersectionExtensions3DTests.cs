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
  public class IntersectionExtensions3DTests {
    // Line, LineSegment, Ray cross test
    [RepeatedTestMethod(1)]
    public void LineToRay() {
      Assert.IsTrue(false);
    }

    [RepeatedTestMethod(1)]
    public void LineToLineSegment() {
      Assert.IsTrue(false);
    }

    [RepeatedTestMethod(1)]
    public void LineSegmentToRay() {
      Assert.IsTrue(false);
    }

    // Plane with other basic primitives
    [RepeatedTestMethod(100)]
    public void PlaneToLine() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        Assert.ThrowsException<ArithmeticException>(() => { Plane.FromPoints(p0, p1, p2); });

      } else {
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
    }

    [RepeatedTestMethod(100)]
    public void PlaneToRay() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        Assert.ThrowsException<ArithmeticException>(() => { Plane.FromPoints(p0, p1, p2); });

      } else {
        // cache varibles
        var shift_vector = plane.Normal.IsPerpendicular(Vector3D.AxisZ) ? plane.Normal : Vector3D.AxisZ;

        var parallel_vector1 = plane.AxisU;
        var parallel_vector2 = plane.AxisV;
        var shift = 2 * shift_vector;
        (double a, double b, double c) = (0, 0, 0);
        (Point3D pp1, Point3D pp2) = (null, null);
        Ray3D ray = null;
        UnitVector3D direction = null;

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
    }

    [RepeatedTestMethod(100)]
    public void PlaneToLineSegment() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        Assert.ThrowsException<ArithmeticException>(() => { Plane.FromPoints(p0, p1, p2); });

      } else {
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
        Assert.ThrowsException<ArithmeticException>(() => { Triangle3D.FromPoints(p0, p1, p2); });

      } else {
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
    }

    [RepeatedTestMethod(1)]
    public void TriangleToLineSegment() {
      Assert.IsTrue(false);
    }
  }
}
