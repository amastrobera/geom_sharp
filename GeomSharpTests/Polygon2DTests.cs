// internal
using GeomSharp;
using GeomSharp.Algebra;

// external
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GeomSharpTests {

  [TestClass]
  public class Polygon2DTests {
    // several tests in 2D

    [RepeatedTestMethod(100)]
    public void CenterOfMass() {
      // 2D

      // temporary data
      Polygon2D poly;
      Point2D cm = null;
      Point2D exp_cm = null;

      // test 1: triangle
      poly = RandomGenerator.MakeTrianglePolygon2D();
      if (poly is null) {
        return;
      }
      exp_cm = Point2D.FromVector((poly[0].ToVector() + poly[1].ToVector() + poly[2].ToVector()) / 3);
      cm = poly.CenterOfMass();
      Assert.IsTrue(cm.AlmostEquals(exp_cm, Constants.THREE_DECIMALS),
                    String.Format("area of square different, \n\texp={0}\n\tact={1}", exp_cm.ToWkt(), cm.ToWkt()));
    }

    [RepeatedTestMethod(100)]
    public void Area() {
      // 2D

      // temporary data
      Polygon2D poly;
      double area = 0;
      double exp_area = 0;
      LineSegmentSet2D segments = null;

      // test 1: square
      poly = RandomGenerator.MakeSquare2D();
      if (poly is null) {
        return;
      }
      segments = poly.ToSegments();
      exp_area = Math.Pow(segments[0].Length(), 2);
      area = poly.Area();
      Assert.IsTrue(Math.Round(area - exp_area, Constants.THREE_DECIMALS) == 0,
                    String.Format("area of square different, \n\texp={0:F3}\n\tact={1:F3}" + exp_area, area));

      // test 2: rectangle
      poly = RandomGenerator.MakeSquare2D();
      if (poly is null) {
        return;
      }
      segments = poly.ToSegments();
      exp_area = segments[0].Length() * segments[1].Length();
      area = poly.Area();
      Assert.IsTrue(Math.Round(area - exp_area, Constants.THREE_DECIMALS) == 0,
                    String.Format("area of rectangle different, \n\texp={0:F3}\n\tact={1:F3}" + exp_area, area));
    }

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

      // test 2-3: all vertices are contained, all mid points are contained
      for (int i = 0; i < n; ++i) {
        p = poly[i];
        Assert.IsTrue(poly.Contains(p),
                      "border (vertex " + i.ToString() + "), \n\tt=" + poly.ToWkt() + "\n\tp=" + p.ToWkt());

        p = Point2D.FromVector((poly[i].ToVector() + poly[(i + 1) % n].ToVector()) / 2);
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
        var square = new Polygon2D(new List<Point2D> { poly[0],
                                                       poly[0] + Vector2D.AxisU * radius,
                                                       poly[0] + Vector2D.AxisU * radius + Vector2D.AxisV * radius,
                                                       poly[0] + Vector2D.AxisV * radius });

        p = Point2D.FromVector((square[0].ToVector() + square[1].ToVector()) / 2);
        Assert.IsTrue(square.Contains(p),
                      "square (mid point) parallel ray \n\tt=" + square.ToWkt() + "\n\tp=" + p.ToWkt());

        p = square[0] - Vector2D.AxisU * radius;
        Assert.IsFalse(square.Contains(p),
                       "square (outer point) parallel ray \n\tt=" + square.ToWkt() + "\n\tp=" + p.ToWkt());

        p = square[1] + Vector2D.AxisU * radius;
        Assert.IsFalse(square.Contains(p),
                       "square (outer point 2) parallel ray \n\tt=" + square.ToWkt() + "\n\tp=" + p.ToWkt());
      }
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      // 2D
      (var poly, var cm, double radius, int n) = RandomGenerator.MakeConvexPolygon2D();

      // Console.WriteLine("t = " + t.ToWkt());
      if (poly is null) {
        return;
      }

      // temporary data
      Polygon2D other;
      cm = poly.CenterOfMass();
      n = poly.Size;

      // test 1: a polygon shifted along the radius by radius size, intersects
      for (int i = 0; i < n; i++) {
        var dir = (poly[i] - cm).Normalize();
        other = new Polygon2D(poly.Select(p => p + radius * dir));
        Assert.IsTrue(poly.Intersects(other),
                      "a polygon shifted along the radius by radius size, intersects, \n\tt=" + poly.ToWkt() +
                          "\n\tother=" + other.ToWkt());
      }

      // test 2: a polygon shifted along the radius by 2+radius size, does not intersect
      for (int i = 0; i < n; i++) {
        var dir = (poly[i] - cm).Normalize();
        other = new Polygon2D(poly.Select(p => p + 3 * radius * dir));
        Assert.IsFalse(poly.Intersects(other),
                       "a polygon shifted along the radius by 2+radius size, does not intersect, \n\tt=" +
                           poly.ToWkt() + "\n\tother=" + other.ToWkt());
      }
    }
  }
}
