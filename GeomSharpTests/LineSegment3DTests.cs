﻿// internal
using GeomSharp;

// external
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {

  /// <summary>
  /// tests for all the MathNet.Spatial.Euclidean functions I created
  /// </summary>
  [TestClass]
  public class LineSegment3DTests {
    [RepeatedTestMethod(100)]
    public void Containment() {
      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine3D();

      if (line is null) {
        return;
      }

      // temp data
      Point3D p;
      UnitVector3D u = line.Direction;
      UnitVector3D u_perp =
          u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();

      // must contains the ray points (origin and p1)
      p = p0;
      Assert.IsTrue(line.Contains(p), "contains extremities" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p1;
      Assert.IsTrue(line.Contains(p), "contains extremities" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      (double a, double b) = RandomGenerator.MakeLinearCombo2SumTo1();
      p = Point3D.FromVector(a * p0.ToVector() + b * p1.ToVector());
      Assert.IsTrue(line.Contains(p), "contains random mid" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      // does not contain points aside the ray
      p = p0 + u_perp;
      Assert.IsFalse(line.Contains(p), "point aside" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p0 - u_perp;
      Assert.IsFalse(line.Contains(p), "point aside" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p1 + u_perp;
      Assert.IsFalse(line.Contains(p), "point aside" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p1 - u_perp;
      Assert.IsFalse(line.Contains(p), "point aside" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      // input
      (var seg, var p0, var p1) = RandomGenerator.MakeLineSegment3D();

      if (seg is null) {
        return;
      }
      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = (seg.P1 - seg.P0).Normalize();

      // temp data
      LineSegment3D other;
      Vector3D u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ));

      // case 1: intersect forrreal
      //      in the middle, crossing
      other = LineSegment3D.FromPoints(mid - 2 * u_perp, mid + 2 * u_perp);
      Assert.IsTrue(seg.Intersects(other),
                    "intersect forrreal (mid)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      //      on the first extremity, crossing
      other = LineSegment3D.FromPoints(p0 - 2 * u_perp, p0 + 2 * u_perp);
      Assert.IsTrue(seg.Intersects(other), "intersect forrreal (p0)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      //      on the second extremity, crossing
      other = LineSegment3D.FromPoints(p1 - 2 * u_perp, p1 + 2 * u_perp);
      Assert.IsTrue(seg.Intersects(other), "intersect forrreal (p1)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      //      just one point in the middle, going down
      other = LineSegment3D.FromPoints(mid - 2 * u_perp, mid);
      Assert.IsTrue(seg.Intersects(other),
                    "intersect forrreal (to mid)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      //      just one point in the middle, going up
      other = LineSegment3D.FromPoints(mid, mid + 2 * u_perp);
      Assert.IsTrue(seg.Intersects(other),
                    "intersect forrreal (from mid)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      //      just one point in the the first extremity, going down
      other = LineSegment3D.FromPoints(p0 - 2 * u_perp, p0);
      Assert.IsTrue(seg.Intersects(other),
                    "intersect forrreal (to p0)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      //      just one point in the the first extremity, going up
      other = LineSegment3D.FromPoints(p0 + 2 * u_perp, p0);
      Assert.IsTrue(seg.Intersects(other),
                    "intersect forrreal (from p0)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      //      just one point in the the second extremity, going up
      other = LineSegment3D.FromPoints(p1, p1 + 2 * u_perp);
      Assert.IsTrue(seg.Intersects(other),
                    "intersect forrreal (from p1)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      //      just one point in the the second extremity, going down
      other = LineSegment3D.FromPoints(p1, p1 - 2 * u_perp);
      Assert.IsTrue(seg.Intersects(other),
                    "intersect forrreal (to p1)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector3D shift = RandomGenerator.MakeVector3D();
      other = LineSegment3D.FromPoints(p0 + 2 * shift, p1 + 2 * shift);
      Assert.IsFalse(
          seg.Intersects(other),
          "no intersection (parallel, shift upwards random vector)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      //      downwards
      other = LineSegment3D.FromPoints(p0 - 2 * shift, p1 - 2 * shift);
      Assert.IsFalse(seg.Intersects(other),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + seg.ToWkt() +
                         ", s2=" + other.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void Overlap() {
      // input
      (var seg, var p0, var p1) = RandomGenerator.MakeLineSegment3D();

      if (seg is null) {
        return;
      }
      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = (seg.P1 - seg.P0).Normalize();

      // temp data
      LineSegment3D other;
      Vector3D u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ));

      // case 1: overlap start point
      //      not insersect but overlap
      other = LineSegment3D.FromPoints(mid, mid + seg.Length() * u);
      Assert.IsFalse(seg.Intersects(other), "overlap start point" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());
      Assert.IsTrue(seg.Overlaps(other), "overlap start point" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      // case 2: overlap end point
      //      not insersect but overlap
      other = LineSegment3D.FromPoints(mid - seg.Length() * u, mid);
      Assert.IsFalse(seg.Intersects(other), "overlap end point" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());
      Assert.IsTrue(seg.Overlaps(other), "overlap end point" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      // case 3: overlap both (second segment is contained in the first)
      //      not insersect but overlap
      other = LineSegment3D.FromPoints(p0 + 0.25 * seg.Length() * u, p1 - 0.25 * seg.Length() * u);
      Assert.IsFalse(
          seg.Intersects(other),
          "overlap both (second segment is contained in the first)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());
      Assert.IsTrue(
          seg.Overlaps(other),
          "overlap both (second segment is contained in the first)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());

      // case 4: overlap both (first segment contained in the second)
      //      not insersect but overlap
      other = LineSegment3D.FromPoints(p0 - 0.25 * seg.Length() * u, p1 + 0.25 * seg.Length() * u);
      Assert.IsFalse(
          seg.Intersects(other),
          "overlap both (first segment contained in the second)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());
      Assert.IsTrue(
          seg.Overlaps(other),
          "overlap both (first segment contained in the second)" + "\ns1=" + seg.ToWkt() + ", s2=" + other.ToWkt());
    }
  }
}
