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

    [RepeatedTestMethod(100)]
    public void Intersection() {
      // 3D
      (var poly, var cm, double radius, int n) = RandomGenerator.MakeConvexPolygon3D();

      // Console.WriteLine("t = " + t.ToWkt());
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

      // poly = new Polygon3D(new List<Point3D> { new Point3D(-28.134, 6.242, -21.691),
      //                                          new Point3D(-37.417, -3.125, -18.480),
      //                                          new Point3D(-39.062, -13.698, -10.128),
      //                                          new Point3D(-32.300, -20.529, -0.544),
      //                                          new Point3D(-20.295, -20.421, 5.788),
      //                                          new Point3D(-8.663, -13.426, 5.905),
      //                                          new Point3D(-2.848, -2.817, -0.248),
      //                                          new Point3D(-5.571, 6.443, -9.791),
      //                                          new Point3D(-15.557, 10.021, -18.260),
      //                                          new Point3D(-28.134, 6.242, -21.691) });

      // other = new Polygon3D(
      //     new List<Point3D> { new Point3D(-20.108, -5.255, -5.428), new Point3D(-20.111, -4.660, -5.652),
      //                         new Point3D(-20.187, -4.143, -6.014), new Point3D(-20.330, -3.742, -6.485),
      //                         new Point3D(-20.530, -3.485, -7.030), new Point3D(-20.772, -3.393, -7.611),
      //                         new Point3D(-21.038, -3.473, -8.182), new Point3D(-21.308, -3.717, -8.703),
      //                         new Point3D(-21.562, -4.109, -9.134), new Point3D(-21.781, -4.619, -9.443),
      //                         new Point3D(-21.949, -5.209, -9.608), new Point3D(-22.054, -5.835, -9.616),
      //                         new Point3D(-22.088, -6.452, -9.467), new Point3D(-22.048, -7.013 - 9.171),
      //                         new Point3D(-21.937, -7.476, -8.751), new Point3D(-21.764, -7.808, -8.238),
      //                         new Point3D(-21.541, -7.984, -7.670), new Point3D(-21.285, -7.991, -7.088),
      //                         new Point3D(-21.015, -7.827, -6.537), new Point3D(-20.750, -7.506, -6.057),
      //                         new Point3D(-20.511, -7.051, -5.683), new Point3D(-20.316, -6.496, -5.444),
      //                         new Point3D(-20.178, -5.882, -5.356), new Point3D(-20.108, -5.255, -5.428) });

      // Assert.IsTrue(poly.Intersects(other));
      // Console.WriteLine("poly=" + poly.ToWkt() + "\n\tother=" + other.ToWkt());

      // test 1: a polygon shifted along the radius by radius size, but on the same plane, does not intersect (it
      // overlaps)
      for (int i = 0; i < n; i++) {
        var dir = (poly[i] - cm).Normalize();
        other = new Polygon3D(poly.Select(p => p + radius * dir));
        Assert.IsFalse(
            poly.Intersects(other),
            "a polygon shifted along the radius by radius size, but on the same plane, does not intersect (it overlaps), \n\tt = " +
                poly.ToWkt() + "\n\tother=" + other.ToWkt());
      }

      // test 2: a polygon shifted along the radius by 2+radius size, still does not intersect
      for (int i = 0; i < n; i++) {
        var dir = (poly[i] - cm).Normalize();
        other = new Polygon3D(poly.Select(p => p + 3 * radius * dir));
        Assert.IsFalse(poly.Intersects(other),
                       "a polygon shifted along the radius by 2+radius size, still does not intersect, \n\tt=" +
                           poly.ToWkt() + "\n\tother=" + other.ToWkt());
      }

      // test 3-4: a polygon shifted along the plane's normal is parallel and does not intersect
      {
        other = new Polygon3D(poly.Select(p => p + 2 * plane_norm));
        Assert.IsFalse(poly.Intersects(other),
                       "a polygon shifted up along the plane's normal is parallel and does not intersect, \n\tt=" +
                           poly.ToWkt() + "\n\tother=" + other.ToWkt());

        other = new Polygon3D(poly.Select(p => p - 2 * plane_norm));
        Assert.IsFalse(poly.Intersects(other),
                       "a polygon shifted down along the plane's normal is parallel and does not intersect, \n\tt=" +
                           poly.ToWkt() + "\n\tother=" + other.ToWkt());
      }

      // test 5: a polygon crossing another polygon, intersects
      // a plane skewed 45degs crossing with polygons cutting through
      for (int i = 0; i < n; ++i) {
        var p = poly[i];
        var p_axis = (p - cm).Normalize();
        var mid_normal = Vector3D.FromVector((plane_norm.ToVector() + p_axis.ToVector()) / 2).Normalize();

        var xing_plane = Plane.FromPointAndNormal(cm, mid_normal);

        // other = RandomGenerator.MakeConvexPolygon3D(Center: cm, RefPlane: xing_plane).Polygon;

        other = new Polygon3D(poly.Select(v => xing_plane.Evaluate(xing_plane.ProjectInto(v))));

        if (!(other is null)) {
          Assert.IsTrue(poly.Intersects(other),
                        "a plane skewed 45degs crossing with polygons cutting through (iter " + i.ToString() + "/" +
                            n.ToString() + "), intersects, \n\tt = " + poly.ToWkt() + "\n\tother=" + other.ToWkt());
        }
      }

      // test 6: a polygon crossing the plane but not the another polygon, does not intersect
      {
        {
          var mid_normal = Vector3D.FromVector((plane_norm.ToVector() + plane_axis_u.ToVector()) / 2).Normalize();
          var xing_plane = Plane.FromPointAndNormal(plane.Origin, mid_normal);

          other = new Polygon3D(poly.Select(v => xing_plane.Evaluate(xing_plane.ProjectInto(v)))
                                    .Select(v => v + 3 * radius * plane_axis_u));
          Assert.IsFalse(
              poly.Intersects(other),
              "a polygon crossing the plane but not the another polygon  (Norm - Axis U), does not intersect, \n\tt=" +
                  poly.ToWkt() + "\n\tother=" + other.ToWkt());
        }
        {
          var mid_normal = Vector3D.FromVector((plane_norm.ToVector() + plane_axis_v.ToVector()) / 2).Normalize();
          var xing_plane = Plane.FromPointAndNormal(plane.Origin, mid_normal);

          other = new Polygon3D(poly.Select(v => xing_plane.Evaluate(xing_plane.ProjectInto(v)))
                                    .Select(v => v + 3 * radius * plane_axis_v));
          Assert.IsFalse(
              poly.Intersects(other),
              "a polygon crossing the plane but not the another polygon  (Norm - Axis V), does not intersect, \n\tt=" +
                  poly.ToWkt() + "\n\tother=" + other.ToWkt());
        }
      }
    }
  }
}
