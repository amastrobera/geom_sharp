// internal
using GeomSharp;

// external
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GeomSharpTests {

  [TestClass]
  public class Polygon3DTests {
    [RepeatedTestMethod(100)]
    public void Containment() {
      // 3D
      (var poly, var _c, var radius, int _n) = RandomGenerator.MakeConvexPolygon3D();

      // Console.WriteLine("t = " + t.ToWkt());
      if (poly is null) {
        return;
      }

      // temporary data
      Point3D p;
      var cm = poly.CenterOfMass();
      var n = poly.Size;

      // test 1: centroid is contained
      p = poly.CenterOfMass();
      Assert.IsTrue(poly.Contains(p), "inner (center of mass), \n\tt=" + poly.ToWkt() + "\n\tp=" + p.ToWkt());

      // test 2-3: all vertices are contained, all mid points are contained
      for (int i = 0; i < n; ++i) {
        p = poly[i];
        Assert.IsTrue(poly.Contains(p),
                      "border (vertex " + i.ToString() + "), \n\tt=" + poly.ToWkt() + "\n\tp=" + p.ToWkt());

        p = Point3D.FromVector((poly[i].ToVector() + poly[(i + 1) % n].ToVector()) / 2);
        Assert.IsTrue(poly.Contains(p),
                      "border (mid " + i.ToString() + "), \n\tt=" + poly.ToWkt() + "\n\tp=" + p.ToWkt());
      }

      // test 4: a point further away than a vertex on the center-vertex line, is not contained
      for (int i = 0; i < n; ++i) {
        p = poly[i] + (poly[i] - cm);
        Assert.IsFalse(poly.Contains(p),
                       "outside (vertex " + i.ToString() + "), \n\tt=" + poly.ToWkt() + "\n\tp=" + p.ToWkt());
      }

      // test 5-6-7: a point on the edge, with (horizontal) ray parallel to the edge
      {
        var plane = poly.RefPlane();
        var p0 = plane.ProjectInto(poly[0]);

        var square =
            new Polygon3D(plane.Evaluate(new List<Point2D> { p0,
                                                             p0 + Vector2D.AxisU * radius,
                                                             p0 + Vector2D.AxisU * radius + Vector2D.AxisV * radius,
                                                             p0 + Vector2D.AxisV * radius }));

        p = Point3D.FromVector((square[0].ToVector() + square[1].ToVector()) / 2);
        Assert.IsTrue(square.Contains(p),
                      "square (mid point) parallel ray \n\tt=" + square.ToWkt() + "\n\tp=" + p.ToWkt());

        p = square[0] - 2 * (square[1] - square[0]);
        Assert.IsFalse(square.Contains(p),
                       "square (outer point) parallel ray \n\tt=" + square.ToWkt() + "\n\tp=" + p.ToWkt());

        p = square[1] + 2 * (square[1] - square[0]);
        Assert.IsFalse(square.Contains(p),
                       "square (outer point 2) parallel ray \n\tt=" + square.ToWkt() + "\n\tp=" + p.ToWkt());
      }

      // test 8-9: a point outside of the plane is outside of the polygon too
      {
        p = cm + poly.Normal * 2;
        Assert.IsFalse(poly.Contains(p), "point above the plane \n\tt=" + poly.ToWkt() + "\n\tp=" + p.ToWkt());

        p = cm - poly.Normal * 2;
        Assert.IsFalse(poly.Contains(p), "point below the plane \n\tt=" + poly.ToWkt() + "\n\tp=" + p.ToWkt());
      }
    }
  }
}
