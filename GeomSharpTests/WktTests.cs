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
      string file_name = "geometry.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        var p = RandomGenerator.MakePoint2D();
        p.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var p_from_file = Geometry2D.FromFile(file_path) as Point2D;

        Assert.AreEqual(p, p_from_file);
      }
    }

    // 3D
    [RepeatedTestMethod(1)]
    public void Point3D() {
      string file_name = "geometry.wkt";
      string file_path = Path.Combine(Path.GetTempPath(), file_name);

      for (int decimal_precision = 0; decimal_precision < 1 + GeomSharp.Constants.NINE_DECIMALS;
           decimal_precision++) {  // TODO : make this loop a test attribute in Extensions.cs

        var p = RandomGenerator.MakePoint3D();
        p.ToFile(file_path, decimal_precision);

        // System.Console.WriteLine("wkt = " + p.ToWkt());

        var p_from_file = Geometry3D.FromFile(file_path) as Point3D;

        Assert.AreEqual(p, p_from_file);
      }
    }
  }
}
