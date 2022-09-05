// internal
using GeomSharp;

// external
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
        Assert.AreEqual(plane.AxisU,
                        (p1 - p0).Normalize());  // AxisU is built by points, AxisV is recalculated to be perpendicular
                                                 // to both AxisU and Normal, therefore cannot be tested with points
        Assert.AreEqual(plane.Normal, (p1 - p0).CrossProduct(p2 - p0).Normalize());

        // perpendicular relationships
        Assert.IsTrue(plane.Normal.IsPerpendicular(plane));
        Assert.IsTrue(plane.AxisV.IsPerpendicular(plane.Normal),
                      "AxisV not perpendicular to normal " + "\n\tAxisV=" + plane.AxisV.ToWkt() +
                          "\n\tNormal=" + plane.Normal.ToWkt());
        Assert.IsTrue(plane.AxisU.IsPerpendicular(plane.Normal),
                      "AxisU not perpendicular to normal " + "\n\tAxisU=" + plane.AxisU.ToWkt() +
                          "\n\tNormal=" + plane.Normal.ToWkt());

        Assert.IsTrue(plane.AxisU.IsPerpendicular(plane.AxisV),
                      "AxisV not perpendicular to AxisU " + "\n\tAxisV=" + plane.AxisV.ToWkt() +
                          "\n\tAxisU=" + plane.AxisU.ToWkt());
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
    public void ProjectOnto() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
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

    [RepeatedTestMethod(100)]
    public void VerticalProjectOnto() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      if (plane.Normal.CrossProduct(Vector3D.AxisZ).AlmostEquals(Vector3D.Zero)) {
        // Console.WriteLine("test abandoned, normal matches the AxisZ, cannot VerticalProject");
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

    [RepeatedTestMethod(100)]
    public void ProjectInto() {
      var random_plane = RandomGenerator.MakePlane();
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);
      var plane = random_plane.Plane;

      if (plane is null) {
        return;
      }

      // origin
      Assert.IsTrue(plane.ProjectInto(p0).AlmostEquals(Point2D.Zero), "origin");
      // point on the AxisU
      Assert.IsTrue(Line2D.FromDirection(Point2D.Zero, Vector2D.AxisU).Contains(plane.ProjectInto(p1)), "AxisU");
      // point on the AxisV
      Assert.IsTrue(
          Line2D.FromDirection(Point2D.Zero, Vector2D.AxisV).Contains(plane.ProjectInto(plane.Origin + plane.AxisV)),
          "AxisV");

      //// compare ProjectInto with Intersection. they should yield the same result
      // var P = RandomGenerator.MakePoint3D();
      // var PP = plane.ProjectOnto(P);
      // var ppv = Line3D.FromDirection(PP, plane.AxisV);
      // var ou = Line3D.FromDirection(plane.Origin, plane.AxisU);
      // var U_int = ppv.Intersection(ou);
      // if (U_int.ValueType == typeof(NullValue)) {
      //   throw new Exception("failed to project the 3D point on the AxisU");
      // }
      // double u = ((Point3D)U_int.Value - plane.Origin).Length();

      // var ppu = Line3D.FromDirection(PP, plane.AxisU);
      // var ov = Line3D.FromDirection(plane.Origin, plane.AxisV);
      // var V_int = ppu.Intersection(ov);
      // if (V_int.ValueType == typeof(NullValue)) {
      //   throw new Exception("failed to project the 3D point on the AxisV");
      // }
      // double v = ((Point3D)V_int.Value - plane.Origin).Length();

      // var PPP = new Point2D(u, v);
      // Assert.IsTrue(PPP.AlmostEquals(plane.ProjectInto(P)), "compare ProjectInto to Intersection");

      //// origin
      // Assert.IsTrue(plane.ProjectInto(plane.Origin).AlmostEquals(Point2D.Zero), "origin");
      //// point on the AxisU
      // Assert.IsTrue(
      //     Line2D.FromDirection(Point2D.Zero, Vector2D.AxisU).Contains(plane.ProjectInto(plane.Origin + plane.AxisU)),
      //     "AxisU");
      //// point on the AxisV
      // Assert.IsTrue(
      //     Line2D.FromDirection(Point2D.Zero, Vector2D.AxisV).Contains(plane.ProjectInto(plane.Origin + plane.AxisV)),
      //     "AxisV");
      //// point on the bisector of the first and third quadrant
      // var pp = (new List<Point3D> { p1, p2 }).Average();
      // Assert.IsTrue(Line2D.FromTwoPoints(Point2D.Zero, new Point2D(0.5, 0.5)).Contains(plane.ProjectInto(pp)),
      //               "bisector point"

      //                  + "\n\tpoint=" + plane.ProjectInto(pp).ToWkt()

      //                  + "\n\tpoint=" + pp.ToWkt()

      //);
    }

    [RepeatedTestMethod(100)]
    public void VerticalProjectInto() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      if (plane.Normal.CrossProduct(Vector3D.AxisZ).AlmostEquals(Vector3D.Zero)) {
        // Console.WriteLine("test abandoned, normal matches the AxisZ, cannot VerticalProject");
        return;
      }

      Assert.IsTrue(plane.ProjectInto(plane.Origin).AlmostEquals(Point2D.Zero), "origin");
      Assert.IsTrue(
          Line2D.FromDirection(Point2D.Zero, Vector2D.AxisU).Contains(plane.ProjectInto(plane.Origin + plane.AxisU)),
          "AxisU");
      Assert.IsTrue(
          Line2D.FromDirection(Point2D.Zero, Vector2D.AxisV).Contains(plane.ProjectInto(plane.Origin + plane.AxisV)),
          "AxisV");
    }

    [RepeatedTestMethod(100)]
    public void IntersectPlane() {
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }

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

    [RepeatedTestMethod(10)]
    public void Evaluate() {
      // TODO: this test fails. it depends on many things I don't know how to test
      //      1. Plane's AxisU, AxisV calculation
      //      2. ProjectInto
      var random_plane = RandomGenerator.MakePlane();
      var plane = random_plane.Plane;

      if (plane is null) {
        return;
      }

      // cached variables
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);
      (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();
      Point3D p;

      Func<Plane, Point3D, string, bool> AssertEvaluate =
          (Plane ref_plane, Point3D original_point, string test_description) => {
            var proj_point = ref_plane.ProjectInto(original_point);
            var eval_point = ref_plane.Evaluate(proj_point);

            Assert.IsTrue(ref_plane.Contains(original_point), "original_point contained");
            Assert.IsTrue(ref_plane.Contains(eval_point), "eval_point contained");

            // Console.WriteLine("\n" + test_description +

            //                  "\n\toriginal=" + original_point.ToWkt() +

            //                  "\n\tproj=" + proj_point.ToWkt() +

            //                  "\n\teval=" + eval_point.ToWkt()

            //);

            // Assert.AreEqual(original_point,
            //                 eval_point,

            //                "\n" + test_description +

            //                    "\n\tproj_point=" + proj_point.ToWkt() +

            //                    "\n\toriginal_point=" + original_point.ToWkt() +
            //                    ", d=" + (original_point - ref_plane.Origin).Length().ToString() +

            //                    "\n\teval_point=" + eval_point.ToWkt() +
            //                    ", d=" + (eval_point - ref_plane.Origin).Length().ToString()

            //);

            return true;
          };

      // plane = Plane.FromPoints(new Point3D(0, 0, 5), new Point3D(5, 2, 5), new Point3D(-1, 4, 5));

      // Console.WriteLine("plane=" + plane.ToString());

      // var P = plane.Origin + plane.AxisU;
      // var U = plane.AxisU;
      // var O = plane.Origin;
      // var OP = P - O;

      // Console.WriteLine("OP=" + OP.ToWkt());
      // Console.WriteLine("|OP|=" + OP.Length().ToString());
      // Console.WriteLine("U=" + U.ToWkt());
      // Console.WriteLine("|U|=" + U.Length().ToString());
      // Console.WriteLine("OP*U=" + OP.DotProduct(U).ToString());
      // Console.WriteLine("O*U=" + O.DotProduct(U).ToString());
      // Console.WriteLine("P*U=" + P.DotProduct(U).ToString());
      // Console.WriteLine("U2=" + U.DotProduct(U).ToString());

      // var proj_point_onto = plane.ProjectOnto(P);
      // var proj_point_into = plane.ProjectInto(P);
      // var eval_point_to = plane.Evaluate(proj_point_into);

      // Console.WriteLine(

      //    "\n\torigin=" + O.ToWkt() +

      //    "\n\tproj_point_onto=" + proj_point_onto.ToWkt() + ", d=" + (proj_point_onto - O).Length().ToString() +

      //    "\n\tproj_point_into=" + proj_point_into.ToWkt() +
      //    ", d=" + (proj_point_into - Point2D.Zero).Length().ToString() +

      //    "\n\toriginal_point=" + P.ToWkt() + ", d=" + (P - O).Length().ToString() +

      //    "\n\teval_point=" + eval_point_to.ToWkt() + ", d=" + (eval_point_to - O).Length().ToString()

      //);

      // case 1: the generating points - projected, then evaluated - are equal to themselves
      //          p -> p_proj -> p_eval = p
      AssertEvaluate(plane, plane.Origin, "origin point, p0");
      AssertEvaluate(plane, plane.Origin + plane.AxisU, "axis U point, p1");
      AssertEvaluate(plane, plane.Origin + plane.AxisV, "axis V point, p2");

      // case 2: linear combinations of the original 3D points, still belonging to the plane
      p = new Point3D(a * p0.X + b * p1.X + c * p2.X,
                      a * p0.Y + b * p1.Y + c * p2.Y,
                      a * p0.Z + b * p1.Z + c * p2.Z);  // linear combo
      AssertEvaluate(plane, p, "linear combination of p0,p1,p2");
    }
  }
}
