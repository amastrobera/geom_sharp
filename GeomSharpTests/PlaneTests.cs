// internal
using GeomSharp;

// external
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GeomSharpTests {

  [TestClass]
  public class PlaneTests {
    [RepeatedTestMethod(100)]
    public void Contructor() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        Assert.ThrowsException<ArithmeticException>(() => { Plane.FromPoints(p0, p1, p2); });

      } else {
        // vectors and points
        Assert.AreEqual(plane.Origin, p0);
        Assert.AreEqual(plane.AxisU, (p1 - p0).Normalize());
        Assert.AreEqual(plane.AxisV, (p2 - p0).Normalize());
        Assert.AreEqual(plane.Normal, (p1 - p0).CrossProduct(p2 - p0).Normalize());
      }
    }

    [RepeatedTestMethod(100)]
    public void Contains() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        Assert.ThrowsException<ArithmeticException>(() => { Plane.FromPoints(p0, p1, p2); });

      } else {
        // generating points
        Assert.IsTrue(plane.Contains(p0));
        Assert.IsTrue(plane.Contains(p1));
        Assert.IsTrue(plane.Contains(p2));

        // linear combination
        (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();
        Assert.IsTrue(plane.Contains(new Point3D(a * p0.X + b * p1.X + c * p2.X,
                                                 a * p0.Y + b * p1.Y + c * p2.Y,
                                                 a * p0.Z + b * p1.Z + c * p2.Z)));
      }
    }

    [RepeatedTestMethod(100)]
    public void Project() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        Assert.ThrowsException<ArithmeticException>(() => { Plane.FromPoints(p0, p1, p2); });

      } else {
        var shift_vector =
            (plane.Normal.CrossProduct(Vector3D.AxisZ).AlmostEquals(Vector3D.Zero)) ? Vector3D.AxisY : Vector3D.AxisZ;
        var shift = 2 * shift_vector;

        // shiting points of the plane up, than projecting and testing for containment
        Assert.IsTrue(plane.Contains(plane.ProjectOnto(p0 + shift)));
        Assert.IsTrue(plane.Contains(plane.ProjectOnto(p1 + shift)));
        Assert.IsTrue(plane.Contains(plane.ProjectOnto(p1 + shift)));

        // linear combination
        (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();
        Assert.IsTrue(plane.Contains(plane.ProjectOnto(new Point3D(a * p0.X + b * p1.X + c * p2.X,
                                                                   a * p0.Y + b * p1.Y + c * p2.Y,
                                                                   a * p0.Z + b * p1.Z + c * p2.Z))));
      }
    }

    [RepeatedTestMethod(100)]
    public void VerticalProject() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        Assert.ThrowsException<ArithmeticException>(() => { Plane.FromPoints(p0, p1, p2); });

      } else {
        if (plane.Normal.CrossProduct(Vector3D.AxisZ).AlmostEquals(Vector3D.Zero)) {
          Console.WriteLine("test abandoned, normal matches the AxisZ, cannot VerticalProject");
          return;
        }

        var shift_vector = Vector3D.AxisZ;
        var shift = 2 * shift_vector;

        // shiting points of the plane up, than projecting and testing for containment
        Assert.IsTrue(plane.Contains(plane.VerticalProjectOnto(p0 + shift)));
        Assert.IsTrue(plane.Contains(plane.VerticalProjectOnto(p1 + shift)));
        Assert.IsTrue(plane.Contains(plane.VerticalProjectOnto(p1 + shift)));

        // linear combination
        (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();
        Assert.IsTrue(plane.Contains(plane.VerticalProjectOnto(new Point3D(a * p0.X + b * p1.X + c * p2.X,
                                                                           a * p0.Y + b * p1.Y + c * p2.Y,
                                                                           a * p0.Z + b * p1.Z + c * p2.Z))));
      }
    }

    [RepeatedTestMethod(100)]
    public void IntersectPlane() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        Assert.ThrowsException<ArithmeticException>(() => { Plane.FromPoints(p0, p1, p2); });

      } else {
        // cache varibles
        Plane other_plane = null;
        Point3D p;
        UnitVector3D shift_vector = null;
        double shift = 2;
        (int i, int max_inter) = (0, 100);

        // parallel, no intersect
        shift_vector = plane.Normal;
        other_plane = Plane.FromPointAndNormal(plane.Origin + shift * shift_vector, plane.Normal);
        Assert.IsFalse(plane.Intersects(other_plane));

        shift_vector = plane.Normal;
        other_plane = Plane.FromPointAndNormal(plane.Origin - shift * shift_vector, plane.Normal);
        Assert.IsFalse(plane.Intersects(other_plane));

        // overlapping, no intersect
        other_plane = Plane.FromPointAndNormal(plane.Origin, plane.Normal);
        Assert.IsFalse(plane.Intersects(other_plane));

        // crossing
        i = 0;
        shift_vector = null;
        while ((shift_vector is null || shift_vector.AlmostEquals(plane.Normal)) && i < max_inter) {
          var vec = RandomGenerator.MakeVector3D();
          shift_vector = (plane.Normal + vec).Normalize();
          ++i;
        }
        if (i == max_inter) {
          throw new Exception("failed to generate vec, test abandoned");
        }
        i = 0;
        p = null;
        while ((p is null || p.AlmostEquals(plane.Origin)) && i < max_inter) {
          p = RandomGenerator.MakePoint3D();
          ++i;
        }
        if (i == max_inter) {
          throw new Exception("failed to generate origin, test abandoned");
        }
        // TODO: Euclidean Transform class (Shift, Rotation ...)
        //       it will help for this test
        other_plane = Plane.FromPointAndNormal(p, shift_vector);
        Assert.IsTrue(plane.Intersects(other_plane));
      }
    }

    [RepeatedTestMethod(1)]
    public void Evaluate() {
      Assert.IsTrue(false);
    }
  }
}
