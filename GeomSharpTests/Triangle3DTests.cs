// internal
using GeomSharp;

// external
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GeomSharpTests {

  [TestClass]
  public class Triangle3DTests {
    [RepeatedTestMethod(100)]
    public void Contructor() {
      var random_triangle = RandomGenerator.MakeTriangle3D();
      var t = random_triangle.Triangle;
      (var p0, var p1, var p2) = (random_triangle.p0, random_triangle.p1, random_triangle.p2);

      if (t is null) {
        return;
      } else {
        // Console.WriteLine("t = " + t.ToWkt());

        // vectors and points
        Assert.AreEqual(t.P0, p0);
        Assert.AreEqual(t.P1, p1);
        Assert.AreEqual(t.P2, p2);

        Assert.AreEqual(t.U, (p1 - p0).Normalize());
        Assert.AreEqual(t.V, (p2 - p0).Normalize());
        var norm_calc = (p1 - p0).CrossProduct(p2 - p0).Normalize();
        Assert.AreEqual(t.Normal, norm_calc, t.Normal.ToWkt() + " vs " + norm_calc.ToWkt());

        // center of mass
        Assert.AreEqual(t.CenterOfMass(),
                        new Point3D((t.P0.X + t.P1.X + t.P2.X) / 3.0,
                                    (t.P0.Y + t.P1.Y + t.P2.Y) / 3.0,
                                    (t.P0.Z + t.P1.Z + t.P2.Z) / 3.0));
      }
    }

    private void TestContains(Triangle3D t) {
      if (t is null) {
        return;
      }

      // Console.WriteLine("t = " + t.ToWkt());

      // temporary data
      var cm = t.CenterOfMass();
      var mid01 = Point3D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      var mid12 = Point3D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      var mid02 = Point3D.FromVector((t.P0.ToVector() + t.P2.ToVector()) / 2.0);
      // results and test data
      Point3D p;

      p = cm;
      Assert.IsTrue(t.Contains(p), "inner (center of mass)" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = mid01;
      Assert.IsTrue(t.Contains(p), "border 1" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = mid12;
      Assert.IsTrue(t.Contains(p), "border 2" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = mid02;
      Assert.IsTrue(t.Contains(p), "border 3" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = t.P0;
      Assert.IsTrue(t.Contains(p), "corner 1" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = t.P1;
      Assert.IsTrue(t.Contains(p), "corner 2" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = t.P2;
      Assert.IsTrue(t.Contains(p), "corner 3" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = mid01 + 2 * (mid01 - cm);
      Assert.IsFalse(t.Contains(p), "outer point 1" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = mid12 + 2 * (mid12 - cm);
      Assert.IsFalse(t.Contains(p), "outer point 2" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = mid02 + 2 * (mid02 - cm);
      Assert.IsFalse(t.Contains(p), "outer point 3" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = cm + 2 * t.Normal;
      Assert.IsFalse(t.Contains(p), "point above" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());

      p = cm - 2 * t.Normal;
      Assert.IsFalse(t.Contains(p), "point below" + "\nt=" + t.ToWkt() + ", p=" + p.ToWkt());
    }

    [TestMethod]
    public void ContainsEdgeCases() {
      // 3D (flat XY)
      TestContains(Triangle3D.FromPoints(new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(0, 1, 0)));

      // 3D (flat YZ)
      TestContains(Triangle3D.FromPoints(new Point3D(0, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1)));

      // 3D (flat ZX)
      TestContains(Triangle3D.FromPoints(new Point3D(0, 0, 0), new Point3D(0, 0, 1), new Point3D(1, 0, 0)));
    }

    [RepeatedTestMethod(100)]
    public void Containment() {
      // 3D
      TestContains(RandomGenerator.MakeTriangle3D().Triangle);
    }

    [Ignore]
    [RepeatedTestMethod(1)]
    public void Intersection() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void Overlap() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void Adjacency() {}

    [Ignore]
    [RepeatedTestMethod(1)]
    public void Touch() {}
  }
}
