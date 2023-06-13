// internal
using GeomSharp;
using GeomSharp.Collections;

// external
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {
  /// <summary>
  /// tests for all the MathNet.Spatial.Euclidean functions I created
  /// </summary>
  [TestClass]
  public class WktTests {
    // 2D
    [RepeatedTestMethod(1)]
    public void Point2D() {
      string file_name = "Point2D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        var p = RandomGenerator.MakePoint2D();
        p.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var p_from_file = Geometry2D.FromFile(file_path) as Point2D;

        Assert.IsTrue(p.AlmostEquals(p_from_file, decimal_precision),
                      p.ToWkt(decimal_precision) + "!=" + p_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    public void Line2D() {
      string file_name = "Line2D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        Line2D line = null;
        while (line is null) {
          line = RandomGenerator.MakeLine2D().Line;
        }

        line.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var line_from_file = Geometry2D.FromFile(file_path) as Line2D;

        Assert.IsTrue(line.AlmostEquals(line_from_file, decimal_precision),
                      line.ToWkt(decimal_precision) + "!=" + line_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    public void Ray2D() {
      string file_name = "Ray2D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        Ray2D ray = null;
        while (ray is null) {
          ray = RandomGenerator.MakeRay2D().Ray;
        }

        ray.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var ray_from_file = Geometry2D.FromFile(file_path) as Ray2D;

        Assert.IsTrue(ray.AlmostEquals(ray_from_file, decimal_precision),
                      ray.ToWkt(decimal_precision) + "!=" + ray_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    public void LineSegment2D() {
      string file_name = "LineSegment2D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        LineSegment2D seg = null;
        while (seg is null) {
          seg = RandomGenerator.MakeLineSegment2D().Segment;
        }

        seg.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var seg_from_file = Geometry2D.FromFile(file_path) as LineSegment2D;

        Assert.IsTrue(seg.AlmostEquals(seg_from_file, decimal_precision),
                      seg.ToWkt(decimal_precision) + "!=" + seg_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    [Ignore("TODO: fix MakeSimplePolyline2D loop never ends")]
    public void Polyline2D() {
      string file_name = "Polyline2D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        Polyline2D polyline = null;
        while (polyline is null) {
          polyline = RandomGenerator.MakeSimplePolyline2D().Polyline;
        }

        polyline.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var polyline_from_file = Geometry2D.FromFile(file_path) as Polyline2D;

        Assert.IsTrue(polyline.AlmostEquals(polyline_from_file, decimal_precision),
                      polyline.ToWkt(decimal_precision) + "!=" + polyline_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    [Ignore("TODO: fix FromWkt polygon")]
    public void Polygon2D() {
      string file_name = "Polygon2D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        Polygon2D polygon = null;
        while (polygon is null) {
          polygon = RandomGenerator.MakeConvexPolygon2D().Polygon;
        }

        polygon.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var polygon_from_file = Geometry2D.FromFile(file_path) as Polygon2D;

        Assert.IsTrue(polygon.AlmostEquals(polygon_from_file, decimal_precision),
                      polygon.ToWkt(decimal_precision) + "!=" + polygon_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    public void Triangle2D() {
      string file_name = "Triangle2D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        Triangle2D triangle = null;
        while (triangle is null) {
          triangle = RandomGenerator.MakeTriangle2D().Triangle;
        }

        triangle.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var triangle_from_file = Geometry2D.FromFile(file_path) as Triangle2D;

        Assert.IsTrue(triangle.AlmostEquals(triangle_from_file, decimal_precision),
                      triangle.ToWkt(decimal_precision) + "!=" + triangle_from_file.ToWkt(decimal_precision));
      }
    }

    // 3D
    [RepeatedTestMethod(1)]
    public void Point3D() {
      string file_name = "Point3D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        var p = RandomGenerator.MakePoint3D();
        p.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var p_from_file = Geometry3D.FromFile(file_path) as Point3D;

        Assert.IsTrue(p.AlmostEquals(p_from_file, decimal_precision),
                      p.ToWkt(decimal_precision) + "!=" + p_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    public void Line3D() {
      string file_name = "Line3D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        Line3D line = null;
        while (line is null) {
          line = RandomGenerator.MakeLine3D().Line;
        }

        line.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var line_from_file = Geometry3D.FromFile(file_path) as Line3D;

        Assert.IsTrue(line.AlmostEquals(line_from_file, decimal_precision),
                      line.ToWkt(decimal_precision) + "!=" + line_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    [Ignore("TODO: fix this test")]
    public void Ray3D() {
      string file_name = "Ray3D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        Ray3D ray = null;
        while (ray is null) {
          ray = RandomGenerator.MakeRay3D().Ray;
        }

        ray.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var ray_from_file = Geometry3D.FromFile(file_path) as Ray3D;

        Assert.IsTrue(ray.AlmostEquals(ray_from_file, decimal_precision),
                      ray.ToWkt(decimal_precision) + "!=" + ray_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    public void LineSegment3D() {
      string file_name = "LineSegment3D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        LineSegment3D seg = null;
        while (seg is null) {
          seg = RandomGenerator.MakeLineSegment3D().Segment;
        }

        seg.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var seg_from_file = Geometry3D.FromFile(file_path) as LineSegment3D;

        Assert.IsTrue(seg.AlmostEquals(seg_from_file, decimal_precision),
                      seg.ToWkt(decimal_precision) + "!=" + seg_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    [Ignore("TODO: fix MakeSimplePolyline3D loop never ends")]
    public void Polyline3D() {
      string file_name = "Polyline3D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        Polyline3D polyline = null;
        while (polyline is null) {
          polyline = RandomGenerator.MakeSimplePolyline3D().Polyline;
        }

        polyline.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var polyline_from_file = Geometry3D.FromFile(file_path) as Polyline3D;

        Assert.IsTrue(polyline.AlmostEquals(polyline_from_file, decimal_precision),
                      polyline.ToWkt(decimal_precision) + "!=" + polyline_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    [Ignore("TODO: fix FromWkt polygon")]
    public void Polygon3D() {
      string file_name = "Polygon3D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        Polygon3D polygon = null;
        while (polygon is null) {
          polygon = RandomGenerator.MakeConvexPolygon3D().Polygon;
        }

        polygon.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var polygon_from_file = Geometry3D.FromFile(file_path) as Polygon3D;

        Assert.IsTrue(polygon.AlmostEquals(polygon_from_file, decimal_precision),
                      polygon.ToWkt(decimal_precision) + "!=" + polygon_from_file.ToWkt(decimal_precision));
      }
    }

    [RepeatedTestMethod(1)]
    [Ignore("TODO: fix this test")]
    public void Triangle3D() {
      string file_name = "Triangle3D.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        Triangle3D triangle = null;
        while (triangle is null) {
          triangle = RandomGenerator.MakeTriangle3D().Triangle;
        }

        triangle.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var triangle_from_file = Geometry3D.FromFile(file_path) as Triangle3D;

        Assert.IsTrue(triangle.AlmostEquals(triangle_from_file, decimal_precision),
                      triangle.ToWkt(decimal_precision) + "!=" + triangle_from_file.ToWkt(decimal_precision));
      }
    }
  }
}
