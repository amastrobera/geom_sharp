// internal
using GeomSharp;
using GeomSharp.Collections;

// external
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {
  /// <summary>
  /// tests for all the MathNet.Spatial.Euclidean functions I created
  /// </summary>
  [TestClass]
  public class ExtensionsTests {
    [RepeatedTestMethod(100)]
    public void Point3DAverage() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var p1 = RandomGenerator.MakePoint3D();
      var p2 = RandomGenerator.MakePoint3D();
      var p3 = RandomGenerator.MakePoint3D();

      Assert.AreEqual(new Point3D((p1.X + p2.X + p3.X) / 3, (p1.Y + p2.Y + p3.Y) / 3, (p1.Z + p2.Z + p3.Z) / 3),
                      (new List<Point3D> { p1, p2, p3 }).Average());
    }

    [RepeatedTestMethod(100)]
    public void Point2DAverage() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var p1 = RandomGenerator.MakePoint2D();
      var p2 = RandomGenerator.MakePoint2D();
      var p3 = RandomGenerator.MakePoint2D();

      Assert.AreEqual(new Point2D((p1.U + p2.U + p3.U) / 3, (p1.V + p2.V + p3.V) / 3),
                      (new List<Point2D> { p1, p2, p3 }).Average());
    }

    [RepeatedTestMethod(100)]
    public void RemoveCollinearPoints2D() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var p1 = RandomGenerator.MakePoint2D();
      var p2 = RandomGenerator.MakePoint2D();
      var p3 = p1 + (p2 - p1) * 3;
      var p4 = p2 + (p3 - p2) * 4;

      var orig_list = new List<Point2D>() { p1, p2, p3, p4 };

      var new_list = orig_list.RemoveCollinearPoints(precision);

      Assert.IsTrue(orig_list.Count() > new_list.Count());
      Assert.AreEqual(2, new_list.Count);
      Assert.IsTrue(new_list.Contains(p1));
      Assert.IsTrue(new_list.Contains(p4));
    }

    [RepeatedTestMethod(100)]
    public void RemoveCollinearPoints3D() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var p1 = RandomGenerator.MakePoint3D();
      var p2 = RandomGenerator.MakePoint3D();
      var p3 = p1 + (p2 - p1) * 3;
      var p4 = p2 + (p3 - p2) * 4;

      var orig_list = new List<Point3D>() { p1, p2, p3, p4 };

      // Console.WriteLine(orig_list.ToWkt());

      var new_list = orig_list.RemoveCollinearPoints(precision);

      Assert.IsTrue(orig_list.Count > new_list.Count);
      Assert.AreEqual(2, new_list.Count);
      Assert.IsTrue(new_list.Contains(p1));
      Assert.IsTrue(new_list.Contains(p4));
    }
  }
}
