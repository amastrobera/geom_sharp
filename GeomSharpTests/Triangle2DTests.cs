﻿// internal
using GeomSharp;

// external
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GeomSharpTests {

  [TestClass]
  public class Triangle2DTests {
    [RepeatedTestMethod(100)]
    public void Contructor() {
      var random_triangle = RandomGenerator.MakeTriangle2D();
      var t = random_triangle.Triangle;
      (var p0, var p1, var p2) = (random_triangle.p0, random_triangle.p1, random_triangle.p2);

      if (t is null) {
        return;
      }
      // Console.WriteLine("t = " + t.ToWkt());

      // vectors and points
      Assert.AreEqual(t.P0, p0);
      Assert.AreEqual(t.P1, p1);
      Assert.AreEqual(t.P2, p2);

      Assert.AreEqual(t.U, (p1 - p0).Normalize());
      Assert.AreEqual(t.V, (p2 - p0).Normalize());
      Assert.IsTrue(((p1 - p0).CrossProduct(p2 - p0) >= 0) ? t.Orientation == Constants.Orientation.COUNTER_CLOCKWISE
                                                           : t.Orientation == Constants.Orientation.CLOCKWISE);

      // center of mass
      Assert.AreEqual(t.CenterOfMass(),
                      new Point2D((t.P0.U + t.P1.U + t.P2.U) / 3.0, (t.P0.V + t.P1.V + t.P2.V) / 3.0));
    }

    [RepeatedTestMethod(100)]
    public void Contains() {
      // 2D
      var t = RandomGenerator.MakeTriangle2D().Triangle;

      // Console.WriteLine("t = " + t.ToWkt());
      if (t is null) {
        return;
      }

      // temporary data
      var cm = t.CenterOfMass();
      var mid01 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      var mid12 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      var mid02 = Point2D.FromVector((t.P0.ToVector() + t.P2.ToVector()) / 2.0);
      // results and test data
      Point2D p;

      p = cm;
      Assert.IsTrue(t.Contains(p), "inner (center of mass), \n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = mid01;
      Assert.IsTrue(t.Contains(p), "border 1" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = mid12;
      Assert.IsTrue(t.Contains(p), "border 2" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = mid02;
      Assert.IsTrue(t.Contains(p), "border 3" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = t.P0;
      Assert.IsTrue(t.Contains(p), "corner 1" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = t.P1;
      Assert.IsTrue(t.Contains(p), "corner 2" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = t.P2;
      Assert.IsTrue(t.Contains(p), "corner 3" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = mid01 + 2 * (mid01 - cm);
      Assert.IsFalse(t.Contains(p), "outer point 1" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = mid12 + 2 * (mid12 - cm);
      Assert.IsFalse(t.Contains(p), "outer point 2" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = mid02 + 2 * (mid02 - cm);
      Assert.IsFalse(t.Contains(p), "outer point 3" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = cm + 2 * (t.P0 - cm);
      Assert.IsFalse(t.Contains(p), "point above" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());

      p = cm - 2 * (t.P0 - cm);
      Assert.IsFalse(t.Contains(p), "point below" + "\n\tt=" + t.ToWkt() + "\n\tp=" + p.ToWkt());
    }

    [RepeatedTestMethod(1)]
    public void Overlap() {
      // 2D
      var t = RandomGenerator.MakeTriangle2D().Triangle;

      // Console.WriteLine("t = " + t.ToWkt());
      if (t is null) {
        return;
      }

      // temporary data
      var cm = t.CenterOfMass();
      var mid01 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      var mid12 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      var mid02 = Point2D.FromVector((t.P0.ToVector() + t.P2.ToVector()) / 2.0);
      // results and test data
      Triangle2D other_t;

      other_t = Triangle2D.FromPoints(mid01, mid12, mid02);
      Assert.IsTrue(t.Overlaps(other_t), "contained, \n\tt=" + t.ToWkt() + "\n\tp=" + other_t.ToWkt());

      other_t = Triangle2D.FromPoints(mid01, mid12 + 2 * (mid12 - mid01), mid02 + 2 * (mid02 - mid01));
      Assert.IsTrue(t.Overlaps(other_t),
                    "one inside another (mid1, mid2), \n\tt=" + t.ToWkt() + "\n\tp=" + other_t.ToWkt());

      other_t = Triangle2D.FromPoints(cm, mid12 + 3 * (mid12 - mid01), mid02 + 3 * (mid02 - mid01));
      Assert.IsTrue(t.Overlaps(other_t),
                    "one inside another (mid1, mid2), \n\tt=" + t.ToWkt() + "\n\tp=" + other_t.ToWkt());

      other_t = Triangle2D.FromPoints(mid01 + 2 * (mid01 - cm), t.P1, t.P0);
      Assert.IsTrue(t.Overlaps(other_t), "overlap one sided 01, \n\tt=" + t.ToWkt() + "\n\tp=" + other_t.ToWkt());

      other_t = Triangle2D.FromPoints(mid02 + 2 * (mid02 - cm), t.P2, t.P0);
      Assert.IsTrue(t.Overlaps(other_t), "overlap one sided 02, \n\tt=" + t.ToWkt() + "\n\tp=" + other_t.ToWkt());

      other_t = Triangle2D.FromPoints(mid12 + 2 * (mid12 - cm), t.P1, t.P2);
      Assert.IsTrue(t.Overlaps(other_t), "overlap one sided 12, \n\tt=" + t.ToWkt() + "\n\tp=" + other_t.ToWkt());
    }
  }
}