// internal
using GeomSharp;

// external
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GeomSharpTests {

  [TestClass]
  public class Polygon2DTests {
    [RepeatedTestMethod(100)]
    public void Containment() {
      // 2D
      (var poly, var cm, double radius, int n) = RandomGenerator.MakeConvexPolygon2D();

      // Console.WriteLine("t = " + t.ToWkt());
      if (poly is null) {
        return;
      }

      // temporary data
      Point2D p;

      // test 1: centroid is contained
      p = poly.CenterOfMass();
      Assert.IsTrue(poly.Contains(p), "inner (center of mass), \n\tt=" + poly.ToWkt() + "\n\tp=" + p.ToWkt());

      // test 2: all vertices are contained
      for (int i = 0; i < n; ++i) {
        p = poly[i];
        Assert.IsTrue(poly.Contains(p),
                      "border (vertex " + i.ToString() + "), \n\tt=" + poly.ToWkt() + "\n\tp=" + p.ToWkt());
      }

      // test 3: a point further away than a vertex on the center-vertex line, is not contained
      for (int i = 0; i < n; ++i) {
        p = poly[i] + (poly[i] - cm);
        Assert.IsFalse(poly.Contains(p),
                       "outside (vertex " + i.ToString() + "), \n\tt=" + poly.ToWkt() + "\n\tp=" + p.ToWkt());
      }
    }
  }
}
