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
    [RepeatedTestMethod(1)]  // TODO use a file wkt
    [Ignore("test not written")]
    public void Polygonize() {
      // 2D
    }

    [RepeatedTestMethod(1)]  // TODO use a file wkt
    [Ignore("not yet implemented")]
    public void Triangulate() {
      // 2D
    }

    [RepeatedTestMethod(100)]
    public void Containment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // 3D
      (var poly, var _c, var radius, int _n) = RandomGenerator.MakeConvexPolygon3D(decimal_precision: precision);

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (poly is null) {
        return;
      }

      // temporary data
      Point3D p;
      var cm = poly.CenterOfMass();
      var n = poly.Size;

      // test 1: centroid is contained
      p = poly.CenterOfMass();
      Assert.IsTrue(poly.Contains(p, precision),
                    "inner (center of mass), \n\tt=" + poly.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      // test 2-3: all vertices are contained, all mid points are contained
      for (int i = 0; i < n; ++i) {
        p = poly[i];
        Assert.IsTrue(
            poly.Contains(p, precision),
            "border (vertex " + i.ToString() + "), \n\tt=" + poly.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

        p = Point3D.FromVector((poly[i].ToVector() + poly[(i + 1) % n].ToVector()) / 2);
        Assert.IsTrue(
            poly.Contains(p, precision),
            "border (mid " + i.ToString() + "), \n\tt=" + poly.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }

      // test 4: a point further away than a vertex on the center-vertex line, is not contained
      for (int i = 0; i < n; ++i) {
        p = poly[i] + (poly[i] - cm);
        Assert.IsFalse(
            poly.Contains(p, precision),
            "outside (vertex " + i.ToString() + "), \n\tt=" + poly.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }

      // test 5-6-7: a point on the edge, with (horizontal) ray parallel to the edge
      {
        var plane = poly.RefPlane();
        var p0 = plane.ProjectInto(poly[0]);

        var square =
            new Polygon3D(plane.Evaluate(new List<Point2D> { p0,
                                                             p0 + Vector2D.AxisU * radius,
                                                             p0 + Vector2D.AxisU * radius + Vector2D.AxisV * radius,
                                                             p0 + Vector2D.AxisV * radius }),
                          precision);

        p = Point3D.FromVector((square[0].ToVector() + square[1].ToVector()) / 2);
        Assert.IsTrue(
            square.Contains(p, precision),
            "square (mid point) parallel ray \n\tt=" + square.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

        p = square[0] - 2 * (square[1] - square[0]);
        Assert.IsFalse(
            square.Contains(p, precision),
            "square (outer point) parallel ray \n\tt=" + square.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

        p = square[1] + 2 * (square[1] - square[0]);
        Assert.IsFalse(
            square.Contains(p, precision),
            "square (outer point 2) parallel ray \n\tt=" + square.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }

      // test 8-9: a point outside of the plane is outside of the polygon too
      {
        p = cm + poly.Normal * 2;
        Assert.IsFalse(poly.Contains(p, precision),
                       "point above the plane \n\tt=" + poly.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

        p = cm - poly.Normal * 2;
        Assert.IsFalse(poly.Contains(p, precision),
                       "point below the plane \n\tt=" + poly.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // 3D
      (var poly, var cm, double radius, int n) = RandomGenerator.MakeConvexPolygon3D(decimal_precision: precision);

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (poly is null) {
        return;
      }

      // temporary data
      Polygon3D other;
      var plane = poly.RefPlane();
      var plane_norm = plane.Normal;
      var plane_axis_u = plane.AxisU;
      var plane_axis_v = plane.AxisV;

      cm = poly.CenterOfMass();
      n = poly.Size;

      // test 1: a polygon shifted along the radius by radius size, but on the same plane, does not intersect (it
      // overlaps)
      for (int i = 0; i < n; i++) {
        var dir = (poly[i] - cm).Normalize();
        other = new Polygon3D(poly.Select(p => p + radius * dir), precision);
        Assert.IsFalse(
            poly.Intersects(other, precision),
            "a polygon shifted along the radius by radius size, but on the same plane, does not intersect (it overlaps), \n\tt = " +
                poly.ToWkt(precision) + "\n\tother=" + other.ToWkt(precision));
      }

      // test 2: a polygon shifted along the radius by 2+radius size, still does not intersect
      for (int i = 0; i < n; i++) {
        var dir = (poly[i] - cm).Normalize();
        other = new Polygon3D(poly.Select(p => p + 3 * radius * dir), precision);
        Assert.IsFalse(poly.Intersects(other, precision),
                       "a polygon shifted along the radius by 2+radius size, still does not intersect, \n\tt=" +
                           poly.ToWkt(precision) + "\n\tother=" + other.ToWkt(precision));
      }

      // test 3-4: a polygon shifted along the plane's normal is parallel and does not intersect
      {
        other = new Polygon3D(poly.Select(p => p + 2 * plane_norm), precision);
        Assert.IsFalse(poly.Intersects(other, precision),
                       "a polygon shifted up along the plane's normal is parallel and does not intersect, \n\tt=" +
                           poly.ToWkt(precision) + "\n\tother=" + other.ToWkt(precision));

        other = new Polygon3D(poly.Select(p => p - 2 * plane_norm), precision);
        Assert.IsFalse(poly.Intersects(other, precision),
                       "a polygon shifted down along the plane's normal is parallel and does not intersect, \n\tt=" +
                           poly.ToWkt(precision) + "\n\tother=" + other.ToWkt(precision));
      }

      // test 5: a polygon crossing another polygon, intersects
      // a plane skewed 45degs crossing with polygons cutting through
      for (int i = 0; i < n; ++i) {
        var p = poly[i];
        var p_axis = (p - cm).Normalize();
        var mid_normal = Vector3D.FromVector((plane_norm.ToVector() + p_axis.ToVector()) / 2).Normalize();

        var xing_plane = Plane.FromPointAndNormal(cm, mid_normal, precision);

        // other = RandomGenerator.MakeConvexPolygon3D(Center: cm, RefPlane: xing_plane).Polygon;

        other = new Polygon3D(poly.Select(v => xing_plane.Evaluate(xing_plane.ProjectInto(v))), precision);

        if (!(other is null)) {
          Assert.IsTrue(poly.Intersects(other, precision),
                        "a plane skewed 45degs crossing with polygons cutting through (iter " + i.ToString() + "/" +
                            n.ToString() + "), intersects, \n\tt = " + poly.ToWkt(precision) +
                            "\n\tother=" + other.ToWkt(precision));
        }
      }

      // test 6: a polygon crossing the plane but not the another polygon, does not intersect
      {
        {
          var mid_normal = Vector3D.FromVector((plane_norm.ToVector() + plane_axis_u.ToVector()) / 2).Normalize();
          var xing_plane = Plane.FromPointAndNormal(plane.Origin, mid_normal, precision);

          other = new Polygon3D(poly.Select(v => xing_plane.Evaluate(xing_plane.ProjectInto(v)))
                                    .Select(v => v + 3 * radius * plane_axis_u),
                                precision);
          Assert.IsFalse(
              poly.Intersects(other, precision),
              "a polygon crossing the plane but not the another polygon  (Norm - Axis U), does not intersect, \n\tt=" +
                  poly.ToWkt(precision) + "\n\tother=" + other.ToWkt(precision));
        }
        {
          var mid_normal = Vector3D.FromVector((plane_norm.ToVector() + plane_axis_v.ToVector()) / 2).Normalize();
          var xing_plane = Plane.FromPointAndNormal(plane.Origin, mid_normal, precision);

          other = new Polygon3D(poly.Select(v => xing_plane.Evaluate(xing_plane.ProjectInto(v)))
                                    .Select(v => v + 3 * radius * plane_axis_v),
                                precision);
          Assert.IsFalse(
              poly.Intersects(other, precision),
              "a polygon crossing the plane but not the another polygon  (Norm - Axis V), does not intersect, \n\tt=" +
                  poly.ToWkt(precision) + "\n\tother=" + other.ToWkt(precision));
        }
      }
    }
  }
}
