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

    [Ignore]
    [RepeatedTestMethod(1)]
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

      // test 5-6-7-8: a polygon crossing another polygon, intersects
      // perfectly perpendicular plane, polygon cutting through the other
      {
        other = RandomGenerator.MakeConvexPolygon3D(Center: cm, RefPlane: Plane.FromPointAndNormal(cm, plane_axis_u))
                    .Polygon;
        if (!(other is null)) {
          Assert.IsTrue(
              poly.Intersects(other),
              "a perfectly perpendicular plane, polygon cutting through the other (Axis U), intersects, \n\tt=" +
                  poly.ToWkt() + "\n\tother=" + other.ToWkt());
        }

        other = RandomGenerator.MakeConvexPolygon3D(Center: cm, RefPlane: Plane.FromPointAndNormal(cm, plane_axis_v))
                    .Polygon;
        if (!(other is null)) {
          Assert.IsTrue(
              poly.Intersects(other),
              "a perfectly perpendicular plane, polygon cutting through the other (Axis V), intersects, \n\tt = " +
                  poly.ToWkt() + "\n\tother=" + other.ToWkt());
        }

        // a plane skewed 45degs crossing with polygons cutting through
        {
          var mid_normal = Vector3D.FromVector((plane_norm.ToVector() + plane_axis_u.ToVector()) / 2).Normalize();
          var xing_plane = Plane.FromPointAndNormal(plane.Origin, mid_normal);

          var other_points = poly.Select(v => xing_plane.Evaluate(xing_plane.ProjectInto(v)));
          other = new Polygon3D(other_points);
          Assert.IsTrue(
              poly.Intersects(other),
              "a plane skewed 45degs crossing with polygons cutting through (Norm - Axis U), intersects, \n\tt = " +
                  poly.ToWkt() + "\n\tother=" + other.ToWkt());
        }

        {
          var mid_normal = Vector3D.FromVector((plane_norm.ToVector() + plane_axis_v.ToVector()) / 2).Normalize();
          var xing_plane = Plane.FromPointAndNormal(plane.Origin, mid_normal);

          var other_points = poly.Select(v => xing_plane.Evaluate(xing_plane.ProjectInto(v)));
          other = new Polygon3D(other_points);
          Assert.IsTrue(
              poly.Intersects(other),
              "a plane skewed 45degs crossing with polygons cutting through (Norm - Axis V), intersects,\n\tt = " +
                  poly.ToWkt() + "\n\tother=" + other.ToWkt());
        }
      }

      // test 9: a polygon crossing the plane but not the another polygon, does not intersect
      {
        {
          var mid_normal = Vector3D.FromVector((plane_norm.ToVector() + plane_axis_u.ToVector()) / 2).Normalize();
          var xing_plane = Plane.FromPointAndNormal(plane.Origin, mid_normal);

          var other_points = poly.Select(v => xing_plane.Evaluate(xing_plane.ProjectInto(v)))
                                 .Select(v => v + 3 * radius * (v - cm).Normalize());
          other = new Polygon3D(other_points);
          Assert.IsFalse(
              poly.Intersects(other),
              "a polygon crossing the plane but not the another polygon  (Norm - Axis U), does not intersect, \n\tt=" +
                  poly.ToWkt() + "\n\tother=" + other.ToWkt());
        }
        {
          var mid_normal = Vector3D.FromVector((plane_norm.ToVector() + plane_axis_v.ToVector()) / 2).Normalize();
          var xing_plane = Plane.FromPointAndNormal(plane.Origin, mid_normal);

          var other_points = poly.Select(v => xing_plane.Evaluate(xing_plane.ProjectInto(v)))
                                 .Select(v => v + 3 * radius * (v - cm).Normalize());
          other = new Polygon3D(other_points);
          Assert.IsFalse(
              poly.Intersects(other),
              "a polygon crossing the plane but not the another polygon  (Norm - Axis V), does not intersect, \n\tt=" +
                  poly.ToWkt() + "\n\tother=" + other.ToWkt());
        }
      }
    }
  }
}
