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

    [RepeatedTestMethod(100)]
    public void Adjacency() {
      var t1 = RandomGenerator.MakeTriangle3D().Triangle;

      // Console.WriteLine("t = " + t.ToWkt());
      if (t1 is null) {
        return;
      }

      // temporary data
      var cm = t1.CenterOfMass();
      var mid01 = Point3D.FromVector((t1.P0.ToVector() + t1.P1.ToVector()) / 2.0);
      var mid12 = Point3D.FromVector((t1.P1.ToVector() + t1.P2.ToVector()) / 2.0);
      var mid20 = Point3D.FromVector((t1.P2.ToVector() + t1.P0.ToVector()) / 2.0);
      // results and test data
      Triangle3D t2;
      IntersectionResult res;
      Point3D p0, p1, p2;

      // adjacent (same side)
      p0 = t1.P0;
      p1 = t1.P1;
      p2 = mid01 + (mid01 - cm);
      t2 = Triangle3D.FromPoints(p0, p1, p2);
      Assert.IsTrue(t1.IsAdjacent(t2), "adjacent (same side)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt());
      res = t1.AdjacentSide(t2);
      Assert.IsTrue(((LineSegment3D)res.Value).IsSameSegment(LineSegment3D.FromPoints(p0, p1)),
                    "adjacent (same side)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt() + "\n\ttype=" + res);
      Assert.IsTrue(((LineSegment3D)res.Value).IsSameSegment(LineSegment3D.FromPoints(p1, p0)),
                    "adjacent (same side)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt() + "\n\ttype=" + res);

      p0 = t1.P1;
      p1 = t1.P2;
      p2 = mid12 + (mid12 - cm);
      t2 = Triangle3D.FromPoints(p0, p1, p2);
      Assert.IsTrue(t1.IsAdjacent(t2), "adjacent (same side)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt());
      res = t1.AdjacentSide(t2);
      Assert.IsTrue(((LineSegment3D)res.Value).IsSameSegment(LineSegment3D.FromPoints(p0, p1)),
                    "adjacent (same side)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt() + "\n\ttype=" + res);

      p0 = t1.P2;
      p1 = t1.P0;
      p2 = mid20 + (mid20 - cm);
      t2 = Triangle3D.FromPoints(p0, p1, p2);
      Assert.IsTrue(t1.IsAdjacent(t2), "adjacent (same side)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt());
      res = t1.AdjacentSide(t2);
      Assert.IsTrue(((LineSegment3D)res.Value).IsSameSegment(LineSegment3D.FromPoints(p0, p1)),
                    "adjacent (same side)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt() + "\n\ttype=" + res);

      // adjacent (planes intersecting)
      p0 = t1.P0;
      p1 = t1.P1;
      p2 = cm - t1.Normal * 3;
      t2 = Triangle3D.FromPoints(p0, p1, p2);
      Assert.IsTrue(t1.IsAdjacent(t2), "adjacent (planes intersecting)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt());

      // internal (one side in common)
      p0 = t1.P0;
      p1 = t1.P1;
      p2 = cm;
      t2 = Triangle3D.FromPoints(p0, p1, p2);
      Assert.IsFalse(t1.IsAdjacent(t2), "internal (one side in common)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt());

      p0 = t1.P1;
      p1 = t1.P2;
      p2 = cm;
      t2 = Triangle3D.FromPoints(p0, p1, p2);
      Assert.IsFalse(t1.IsAdjacent(t2), "internal (one side in common)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt());

      p0 = t1.P2;
      p1 = t1.P0;
      p2 = cm;
      t2 = Triangle3D.FromPoints(p0, p1, p2);
      Assert.IsFalse(t1.IsAdjacent(t2), "internal (one side in common)\n\tt1=" + t1.ToWkt() + "\n\tt2=" + t2.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void Touch() {
      var t1 = RandomGenerator.MakeTriangle3D().Triangle;

      if (t1 is null) {
        return;
      }

      // Console.WriteLine("t = " + t.ToWkt());

      // temporary data
      var cm = t1.CenterOfMass();
      var mid01 = Point3D.FromVector((t1.P0.ToVector() + t1.P1.ToVector()) / 2.0);
      var mid12 = Point3D.FromVector((t1.P1.ToVector() + t1.P2.ToVector()) / 2.0);
      var mid20 = Point3D.FromVector((t1.P2.ToVector() + t1.P0.ToVector()) / 2.0);
      // results and test data
      Triangle3D t2;

      // touches (hourglass)
      t2 = Triangle3D.FromPoints(t1.P0, t1.P0 + (t1.P0 - t1.P1), t1.P0 + (t1.P0 - t1.P2));
      Assert.IsTrue(t1.Touches(t2), "touches (hourglass)");

      t2 = Triangle3D.FromPoints(t1.P1, t1.P1 + (t1.P1 - t1.P2), t1.P1 + (t1.P1 - t1.P0));
      Assert.IsTrue(t1.Touches(t2), "touches (hourglass)");

      t2 = Triangle3D.FromPoints(t1.P2, t1.P2 + (t1.P2 - t1.P0), t1.P2 + (t1.P2 - t1.P1));
      Assert.IsTrue(t1.Touches(t2), "touches (hourglass)");

      // touches (double arrows)
      t2 = Triangle3D.FromPoints(mid12, mid12 + (t1.P1 - t1.P0), mid12 + (t1.P2 - t1.P0));
      Assert.IsTrue(t1.Touches(t2), "touches (double arrows)");

      t2 = Triangle3D.FromPoints(mid20, mid20 + (t1.P2 - t1.P1), mid20 + (t1.P0 - t1.P1));
      Assert.IsTrue(t1.Touches(t2), "touches (double arrows)");

      t2 = Triangle3D.FromPoints(mid01, mid01 + (t1.P0 - t1.P2), mid01 + (t1.P1 - t1.P2));
      Assert.IsTrue(t1.Touches(t2), "touches (double arrows)");

      // touches (cm perpendicular)
      t2 = Triangle3D.FromPoints(cm, t1.P0 - t1.Normal * 3, t1.P1 - t1.Normal * 3);
      Assert.IsTrue(t1.Touches(t2), "touches (cm perpendicular)");

      // touches (mid perpendicular)
      t2 = Triangle3D.FromPoints(mid12, t1.P0 - t1.Normal * 3, t1.P1 - t1.Normal * 3);
      Assert.IsTrue(t1.Touches(t2), "touches (mid perpendicular)");

      // no touch (split double arrows)
      t2 = Triangle3D.FromPoints(mid12 + (mid12 - cm),
                                 mid12 + (mid12 - cm) + (t1.P1 - t1.P0),
                                 mid12 + (mid12 - cm) + (t1.P2 - t1.P0));
      Assert.IsFalse(t1.Touches(t2), "no touch (split double arrows)");

      t2 = Triangle3D.FromPoints(mid20 + (mid20 - cm),
                                 mid20 + (mid20 - cm) + (t1.P2 - t1.P1),
                                 mid20 + (mid20 - cm) + (t1.P0 - t1.P1));
      Assert.IsFalse(t1.Touches(t2), "no touch (split double arrows)");

      t2 = Triangle3D.FromPoints(mid01 + (mid01 - cm),
                                 mid01 + (mid01 - cm) + (t1.P0 - t1.P2),
                                 mid01 + (mid01 - cm) + (t1.P1 - t1.P2));
      Assert.IsFalse(t1.Touches(t2), "no touch (split double arrows)");
    }
  }
}
