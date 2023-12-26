// internal
using GeomSharp;

// external
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using GeomSharp.Algebra;

namespace GeomSharpTests {
  /// <summary>
  /// tests for all the MathNet.Spatial.Euclidean functions I created
  /// </summary>
  [TestClass]
  public class WktTests {
    // 2D
    [RepeatedTestMethod(100)]
    public void Point2D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      var p = RandomGenerator.MakePoint2D();
      p.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + p.ToWkt(decimal_precision));

      var p_from_file = Geometry2D.FromFile(file_path, decimal_precision) as Point2D;

      Assert.IsTrue(p.AlmostEquals(p_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + p.ToWkt(decimal_precision) +
                        " != " + p_from_file.ToWkt(decimal_precision));
    }

    [RepeatedTestMethod(100)]
    public void Vector2D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      var p = RandomGenerator.MakeVector2D();
      p.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + p.ToWkt(decimal_precision));

      var p_from_file = GeomSharp.Vector2D.FromFile(file_path, decimal_precision);

      // System.Console.WriteLine("from file = " + p_from_file.ToWkt(decimal_precision));

      Assert.IsTrue(
          p.AlmostEquals(p_from_file, decimal_precision),
          $"precision={decimal_precision}: " +
              $"\n{p.ToWkt(decimal_precision)} != {p_from_file.ToWkt(decimal_precision)}"

              + "\n" +
              string.Join(
                  ",",
                  p.ToVector().Select(
                      (n, i) =>
                          $"{i}={Math.Round(Math.Round(n, decimal_precision) - Math.Round(p_from_file.ToVector()[i], decimal_precision))}"))

      );
    }

    [RepeatedTestMethod(100)]
    public void Line2D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      Line2D line = null;
      while (line is null) {
        line = RandomGenerator.MakeLine2D(decimal_precision: decimal_precision).Line;
      }

      line.ToFile(file_path, decimal_precision);

      System.Console.WriteLine("wkt = " + line.ToWkt(decimal_precision));

      var line_from_file = Geometry2D.FromFile(file_path, decimal_precision) as Line2D;

      System.Console.WriteLine("from_file = " + line_from_file.ToWkt(decimal_precision));

      // System.Console.WriteLine("this.Contains(other.origin)=" +
      //                          line.Contains(line_from_file.Origin, decimal_precision));

      System.Console.WriteLine("Direction.AlmostEquals(other.Direction, decimal_precision)=" +
                               line.Direction.AlmostEquals(line_from_file.Direction, decimal_precision));

      // System.Console.WriteLine("Direction.AlmostEquals(-1 * other.Direction, decimal_precision)=" +
      //                          line.Direction.AlmostEquals(-1 * line_from_file.Direction, decimal_precision));

      var line_vec = line.Direction.ToVector();
      var other_vec = line_from_file.Direction.ToVector();
      var diff = new Vector(line_vec.Select((v, i) => v - other_vec[i]));
      double sqrt = Math.Round(diff.Length(), decimal_precision);
      System.Console.WriteLine("sqrt=" + Math.Round(sqrt, decimal_precision).ToString());

      for (int i = 0; i < line_vec.Size; i++) {
        if (Math.Round(Math.Round(line_vec[i], decimal_precision) - Math.Round(other_vec[i], decimal_precision),
                       decimal_precision) != 0) {
          System.Console.WriteLine(
              i.ToString() + ") differs by " +
              Math.Round(Math.Round(line_vec[i], decimal_precision) - Math.Round(other_vec[i], decimal_precision),
                         decimal_precision)
                  .ToString());
        }
      }

      Assert.IsTrue(line.AlmostEquals(line_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + line.ToWkt(decimal_precision) +
                        " != " + line_from_file.ToWkt(decimal_precision));
    }

    [RepeatedTestMethod(100)]
    public void Ray2D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      Ray2D ray = null;
      while (ray is null) {
        ray = RandomGenerator.MakeRay2D(decimal_precision: decimal_precision).Ray;
      }

      ray.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + ray.ToWkt(decimal_precision));

      var ray_from_file = Geometry2D.FromFile(file_path, decimal_precision) as Ray2D;

      // System.Console.WriteLine("ray_from_file = " + ray_from_file.ToWkt(decimal_precision));

      // clang-format off
      Assert.IsTrue(ray.AlmostEquals(ray_from_file, decimal_precision),
                    $"precision={decimal_precision}:\nray={ray.ToWkt(decimal_precision)}" +
                    "           \n!=                " + 
                    $"\nraw file=" + File.ReadAllText(file_path) +
                    $"\nfile={ray_from_file.ToWkt(decimal_precision)}");
      // clang-format on
    }

    [RepeatedTestMethod(100)]
    public void LineSegment2D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      LineSegment2D seg = null;
      while (seg is null) {
        seg = RandomGenerator.MakeLineSegment2D(decimal_precision: decimal_precision).Segment;
      }

      seg.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + p.ToWkt(decimal_precision));

      var seg_from_file = Geometry2D.FromFile(file_path, decimal_precision) as LineSegment2D;

      Assert.IsTrue(seg.AlmostEquals(seg_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + seg.ToWkt(decimal_precision) +
                        " != " + seg_from_file.ToWkt(decimal_precision));
    }

    [TestMethod]
    //[Ignore("TODO: fix MakeSimplePolyline2D loop never ends")]
    public void Polyline2D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      // int decimal_precision = RandomGenerator.MakeInt(0, 9);
      //  Polyline2D polyline = null;
      //  while (polyline is null) {
      //    polyline = RandomGenerator.MakeSimplePolyline2D(NMax: 3, decimal_precision:decimal_precision).Polyline;
      //  }

      int decimal_precision = GeomSharp.Constants.THREE_DECIMALS;
      var polyline = new Polyline2D(new Point2D[] { new Point2D(0.23, 45.1),
                                                    new Point2D(13.21, 2.5),
                                                    new Point2D(23, -10),
                                                    new Point2D(15.3, 0.34) },
                                    decimal_precision);

      polyline.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("polyline = " + polyline.ToWkt(decimal_precision));

      var polyline_from_file = Geometry2D.FromFile(file_path, decimal_precision) as Polyline2D;

      Assert.IsFalse(polyline_from_file is null);

      // System.Console.WriteLine("polyline_from_file = " + polyline_from_file.ToWkt(decimal_precision));

      Assert.IsTrue(polyline.AlmostEquals(polyline_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + polyline.ToWkt(decimal_precision) +
                        " != " + polyline_from_file.ToWkt(decimal_precision));
    }

    [TestMethod]
    public void Polygon2D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      System.Console.WriteLine("Polygon2D");

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      System.Console.WriteLine("\tdecimal_precision=" + decimal_precision.ToString());

      Polygon2D polygon = null;
      while (polygon is null) {
        polygon = RandomGenerator.MakeConvexPolygon2D(decimal_precision: decimal_precision).Polygon;
      }

      System.Console.WriteLine("\t-> random_polygon=" + polygon.ToWkt(decimal_precision));

      polygon.ToFile(file_path, decimal_precision);

      var polygon_from_file = Geometry2D.FromFile(file_path, decimal_precision) as Polygon2D;

      System.Console.WriteLine("\t-> polygon_from_file=" + polygon_from_file.ToWkt(decimal_precision));

      Assert.IsTrue(polygon.AlmostEquals(polygon_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " +
                        "\n\toriginal: " + polygon.ToWkt(decimal_precision) +
                        "\n\t!=" + "\n\tfrom_file: " + polygon_from_file.ToWkt(decimal_precision));
    }

    [TestMethod]
    //[Ignore("TODO: fix FromWkt parsing error")]
    public void MultiPolygon2D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      // int num_polys = RandomGenerator.MakeInt(2, 12);
      // var polys = new List<Polygon2D>();
      // for (int i = 0; i < num_polys; ++i) {
      //   Polygon2D polygon = null;
      //   while (polygon is null) {
      //     polygon = RandomGenerator.MakeConvexPolygon2D(decimal_precision: decimal_precision).Polygon;
      //   }
      //   polys.Add(polygon);
      // }

      var polys = new List<Polygon2D>() {
        Geometry2D.FromWkt("POLYGON ((102.0 2.0, 103.0 2.0, 103.0 3.0, 102.0 3.0, 102.0 2.0))") as Polygon2D,
        Geometry2D.FromWkt("POLYGON((100.0 0.0, 101.0 0.0, 101.0 1.0, 100.0 1.0, 100.0 0.0))") as Polygon2D,
      };

      var multi_polygon = new MultiPolygon2D(polys, decimal_precision);

      multi_polygon.ToFile(file_path, decimal_precision);

      string string_wkt_file = File.ReadAllText(file_path);
      System.Console.WriteLine("file=" + string_wkt_file);

      var multi_polygon_from_file = Geometry2D.FromFile(file_path, decimal_precision) as MultiPolygon2D;

      Assert.IsFalse(multi_polygon_from_file is null, "from file returned null");

      // System.Console.WriteLine("is wkt and file equal ? " + (polygon.ToWkt(decimal_precision) == string_wkt_file));

      // System.Console.WriteLine("are wkt equal? " +
      //                          (polygon.ToWkt(decimal_precision) == polygon_from_file.ToWkt(decimal_precision)));

      Assert.IsTrue(multi_polygon.AlmostEquals(multi_polygon_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " +
                        "\n\toriginal: " + multi_polygon.ToWkt(decimal_precision) +
                        "\n\t!=" + "\n\tfrom_file: " + multi_polygon_from_file.ToWkt(decimal_precision));
    }

    [RepeatedTestMethod(100)]
    public void Triangle2D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      Triangle2D triangle = null;
      while (triangle is null) {
        triangle = RandomGenerator.MakeTriangle2D(decimal_precision: decimal_precision).Triangle;
      }

      triangle.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + p.ToWkt(decimal_precision));

      var triangle_from_file = Geometry2D.FromFile(file_path, decimal_precision) as Triangle2D;

      Assert.IsTrue(triangle.AlmostEquals(triangle_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + triangle.ToWkt(decimal_precision) +
                        " != " + triangle_from_file.ToWkt(decimal_precision));
    }

    [TestMethod]
    [Ignore("TODO: fix FromWkt parsing error")]
    public void GeometryCollection2D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      Func<int, Geometry2D> RandomGeometryFactory = (int _idx) => {
        int _max_iter = 100;
        switch (_idx) {
          case 0:
            return RandomGenerator.MakePoint2D();

          case 1:
            Line2D _line = null;
            while (_line is null && _max_iter > 0) {
              _line = RandomGenerator.MakeLine2D(decimal_precision: decimal_precision).Line;
              --_max_iter;
            }
            return _line;

          case 2:
            LineSegment2D _seg = null;
            while (_seg is null && _max_iter > 0) {
              _seg = RandomGenerator.MakeLineSegment2D(decimal_precision: decimal_precision).Segment;
              --_max_iter;
            }
            return _seg;

          case 3:
            Ray2D _ray = null;
            while (_ray is null && _max_iter > 0) {
              _ray = RandomGenerator.MakeRay2D(decimal_precision: decimal_precision).Ray;
              --_max_iter;
            }
            return _ray;

          case 4:
            Polyline2D _pline = null;
            while (_pline is null && _max_iter > 0) {
              _pline = RandomGenerator.MakeSimplePolyline2D(decimal_precision: decimal_precision).Polyline;
              --_max_iter;
            }
            return _pline;

          case 5:
            Polygon2D _poly = null;
            while (_poly is null && _max_iter > 0) {
              _poly = RandomGenerator.MakeConvexPolygon2D(decimal_precision: decimal_precision).Polygon;
              --_max_iter;
            }
            return _poly;

          case 6:
            Triangle2D _tri = null;
            while (_tri is null && _max_iter > 0) {
              _tri = RandomGenerator.MakeTriangle2D().Triangle;
              --_max_iter;
            }
            return _tri;

          case 7:
            throw new NotImplementedException("case 7: multi polygon not yet managed");

          case 8:
            throw new NotImplementedException("case 8: geometry collection not yet managed");

          default:
            break;
        }

        throw new Exception("index " + _idx.ToString() + " not in expected range [0,8]");
      };

      int num_geoms = RandomGenerator.MakeInt(2, 12);
      var geoms = new List<Geometry2D>();
      for (int i = 0; i < num_geoms; ++i) {
        int idx_geom = RandomGenerator.MakeInt(0, 6);  // 7,8 not yet managed
        geoms.Add(RandomGeometryFactory(idx_geom));
      }

      var geom_coll = new GeometryCollection2D(geoms);

      geom_coll.ToFile(file_path, decimal_precision);

      // string string_wkt_file = File.ReadAllText(file_path);
      // System.Console.WriteLine("file=" + string_wkt_file);

      var geom_coll_from_file = Geometry2D.FromFile(file_path, decimal_precision) as GeometryCollection2D;

      // System.Console.WriteLine("is wkt and file equal ? " + (polygon.ToWkt(decimal_precision) ==
      // string_wkt_file));

      // System.Console.WriteLine("are wkt equal? " +
      //                          (polygon.ToWkt(decimal_precision) ==
      //                          polygon_from_file.ToWkt(decimal_precision)));

      Assert.IsTrue(geom_coll.AlmostEquals(geom_coll_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " +
                        "\n\toriginal: " + geom_coll.ToWkt(decimal_precision) +
                        "\n\t!=" + "\n\tfrom_file: " + geom_coll_from_file.ToWkt(decimal_precision));
    }

    // 3D
    [RepeatedTestMethod(100)]
    public void Point3D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      var p = RandomGenerator.MakePoint3D();
      p.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + p.ToWkt(decimal_precision));

      var p_from_file = Geometry3D.FromFile(file_path, decimal_precision) as Point3D;

      Assert.IsTrue(p.AlmostEquals(p_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + p.ToWkt(decimal_precision) +
                        " != " + p_from_file.ToWkt(decimal_precision));
    }

    [RepeatedTestMethod(100)]
    public void Vector3D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      var p = RandomGenerator.MakeVector3D();
      p.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + p.ToWkt(decimal_precision));

      var p_from_file = GeomSharp.Vector3D.FromFile(file_path, decimal_precision);

      Assert.IsTrue(
          p.AlmostEquals(p_from_file, decimal_precision),
          $"precision={decimal_precision}: " +
              $"\n{p.ToWkt(decimal_precision)} != {p_from_file.ToWkt(decimal_precision)}"

              + "\n" +
              string.Join(
                  ",",
                  p.ToVector().Select(
                      (n, i) =>
                          $"{i}={Math.Round(Math.Round(n, decimal_precision) - Math.Round(p_from_file.ToVector()[i], decimal_precision))}"))

      );
    }

    [RepeatedTestMethod(100)]
    public void Line3D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      Line3D line = null;
      while (line is null) {
        line = RandomGenerator.MakeLine3D(decimal_precision: decimal_precision).Line;
      }

      line.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + p.ToWkt(decimal_precision));

      var line_from_file = Geometry3D.FromFile(file_path, decimal_precision) as Line3D;

      Assert.IsTrue(line.AlmostEquals(line_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + line.ToWkt(decimal_precision) +
                        " != " + line_from_file.ToWkt(decimal_precision));
    }

    [RepeatedTestMethod(100)]
    public void Ray3D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);
      Ray3D ray = null;
      while (ray is null) {
        ray = RandomGenerator.MakeRay3D(decimal_precision: decimal_precision).Ray;
      }

      ray.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + p.ToWkt(decimal_precision));

      var ray_from_file = Geometry3D.FromFile(file_path, decimal_precision) as Ray3D;

      Assert.IsTrue(ray.AlmostEquals(ray_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + ray.ToWkt(decimal_precision) +
                        " != " + ray_from_file.ToWkt(decimal_precision));
    }

    [RepeatedTestMethod(100)]
    public void LineSegment3D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      LineSegment3D seg = null;
      while (seg is null) {
        seg = RandomGenerator.MakeLineSegment3D(decimal_precision: decimal_precision).Segment;
      }

      seg.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + p.ToWkt(decimal_precision));

      var seg_from_file = Geometry3D.FromFile(file_path, decimal_precision) as LineSegment3D;

      Assert.IsTrue(seg.AlmostEquals(seg_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + seg.ToWkt(decimal_precision) +
                        " != " + seg_from_file.ToWkt(decimal_precision));
    }

    [TestMethod]
    //[Ignore("TODO: fix MakeSimplePolyline3D loop never ends")]
    public void Polyline3D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      // int decimal_precision = RandomGenerator.MakeInt(0, 9);
      //  Polyline3D polyline = null;
      //  while (polyline is null) {
      //    polyline = RandomGenerator.MakeSimplePolyline3D(decimal_precision: decimal_precision).Polyline;
      //  }

      int decimal_precision = GeomSharp.Constants.THREE_DECIMALS;
      var polyline = new Polyline3D(new Point3D[] { new Point3D(0.23, 45.1, 0),
                                                    new Point3D(13.21, 2.5, 15.2),
                                                    new Point3D(23, -10, -7.654),
                                                    new Point3D(15.3, 0.34, -2.64) },
                                    decimal_precision);

      polyline.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + polyline.ToWkt(decimal_precision));

      var polyline_from_file = Geometry3D.FromFile(file_path, decimal_precision) as Polyline3D;

      Assert.IsFalse(polyline_from_file is null);

      // System.Console.WriteLine("polyline_from_file = " + polyline_from_file.ToWkt(decimal_precision));

      Assert.IsTrue(polyline.AlmostEquals(polyline_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + polyline.ToWkt(decimal_precision) +
                        " != " + polyline_from_file.ToWkt(decimal_precision));
    }

    [TestMethod]
    //[Ignore("TODO: fix FromWkt polygon")]
    public void Polygon3D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      Polygon3D polygon = null;
      while (polygon is null) {
        polygon = RandomGenerator.MakeConvexPolygon3D(decimal_precision: decimal_precision).Polygon;
      }

      polygon.ToFile(file_path, decimal_precision);

      var polygon_from_file = Geometry3D.FromFile(file_path, decimal_precision) as Polygon3D;

      // System.Console.WriteLine("wkt = " + polygon.ToWkt(decimal_precision));

      Assert.IsFalse(polygon_from_file is null);

      // System.Console.WriteLine("polygon_from_file = " + polyline_from_file.ToWkt(decimal_precision));

      System.Console.WriteLine("polygon=" + polygon.ToWkt(decimal_precision) +
                               "\nfile_text=" + File.ReadAllText(file_path) +
                               "\npolygon_from_file=" + polygon_from_file.ToWkt(decimal_precision));

      Assert.IsTrue(polygon.AlmostEquals(polygon_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " +
                        "\n\toriginal: " + polygon.ToWkt(decimal_precision) +
                        "\n\t!=" + "\n\tfrom_file: " + polygon_from_file.ToWkt(decimal_precision));
    }

    [RepeatedTestMethod(100)]
    public void Triangle3D() {
      string file_name = new StackTrace(false).GetFrame(0).GetMethod().Name + ".wkt";  // function name.wkt
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      int decimal_precision = RandomGenerator.MakeInt(0, 9);

      Triangle3D triangle = null;
      while (triangle is null) {
        triangle = RandomGenerator.MakeTriangle3D(decimal_precision: decimal_precision).Triangle;
      }

      triangle.ToFile(file_path, decimal_precision);

      // System.Console.WriteLine("wkt = " + p.ToWkt(decimal_precision));

      var triangle_from_file = Geometry3D.FromFile(file_path, decimal_precision) as Triangle3D;

      Assert.IsTrue(triangle.AlmostEquals(triangle_from_file, decimal_precision),
                    "precision=" + decimal_precision.ToString() + ": " + triangle.ToWkt(decimal_precision) +
                        " != " + triangle_from_file.ToWkt(decimal_precision));
    }
  }
}
