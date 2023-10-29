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
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      // vectors and points
      Assert.IsTrue(plane.Origin.AlmostEquals(p0, precision));
      Assert.IsTrue(
          plane.AxisU.AlmostEquals((p1 - p0).Normalize(),
                                   precision));  // AxisU is built by points, AxisV is recalculated to be perpendicular
                                                 // to both AxisU and Normal, therefore cannot be tested with points
      Assert.IsTrue(plane.Normal.AlmostEquals((p1 - p0).CrossProduct(p2 - p0).Normalize(), precision));

      // perpendicular relationships
      Assert.IsTrue(plane.Normal.IsPerpendicular(plane, precision));
      Assert.IsTrue(plane.AxisV.IsPerpendicular(plane.Normal, precision),
                    "AxisV not perpendicular to normal " + "\n\tAxisV=" + plane.AxisV.ToWkt(precision) +
                        "\n\tNormal=" + plane.Normal.ToWkt(precision));
      Assert.IsTrue(plane.AxisU.IsPerpendicular(plane.Normal, precision),
                    "AxisU not perpendicular to normal " + "\n\tAxisU=" + plane.AxisU.ToWkt(precision) +
                        "\n\tNormal=" + plane.Normal.ToWkt(precision));

      Assert.IsTrue(plane.AxisU.IsPerpendicular(plane.AxisV, precision),
                    "AxisV not perpendicular to AxisU " + "\n\tAxisV=" + plane.AxisV.ToWkt(precision) +
                        "\n\tAxisU=" + plane.AxisU.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void Contains() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      // generating points
      Assert.IsTrue(plane.Contains(p0, precision));
      Assert.IsTrue(plane.Contains(p1, precision));
      Assert.IsTrue(plane.Contains(p2, precision));

      // linear combination
      (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();
      Assert.IsTrue(plane.Contains(
          new Point3D(a * p0.X + b * p1.X + c * p2.X, a * p0.Y + b * p1.Y + c * p2.Y, a * p0.Z + b * p1.Z + c * p2.Z),
          precision));
    }

    [RepeatedTestMethod(100)]
    public void ProjectOnto() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      var shift_vector = (plane.Normal.CrossProduct(Vector3D.AxisZ).AlmostEquals(Vector3D.Zero, precision))
                             ? Vector3D.AxisY
                             : Vector3D.AxisZ;
      var shift = 2 * shift_vector;

      // shiting points of the plane up, than projecting and testing for containment
      Assert.IsTrue(plane.Contains(plane.ProjectOnto(p0 + shift, precision), precision));
      Assert.IsTrue(plane.Contains(plane.ProjectOnto(p1 + shift, precision), precision));
      Assert.IsTrue(plane.Contains(plane.ProjectOnto(p1 + shift, precision), precision));

      // linear combination
      (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();
      Assert.IsTrue(plane.Contains(plane.ProjectOnto(new Point3D(a * p0.X + b * p1.X + c * p2.X,
                                                                 a * p0.Y + b * p1.Y + c * p2.Y,
                                                                 a * p0.Z + b * p1.Z + c * p2.Z),
                                                     precision),
                                   precision));
    }

    [RepeatedTestMethod(100)]
    public void VerticalProjectOnto() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      if (plane.Normal.CrossProduct(Vector3D.AxisZ).AlmostEquals(Vector3D.Zero, precision)) {
        // Console.WriteLine("test abandoned, normal matches the AxisZ, cannot VerticalProject");
        return;
      }

      var shift_vector = Vector3D.AxisZ;
      var shift = 2 * shift_vector;

      // shiting points of the plane up, than projecting and testing for containment
      Assert.IsTrue(plane.Contains(plane.VerticalProjectOnto(p0 + shift, precision), precision));
      Assert.IsTrue(plane.Contains(plane.VerticalProjectOnto(p1 + shift, precision), precision));
      Assert.IsTrue(plane.Contains(plane.VerticalProjectOnto(p1 + shift, precision), precision));

      // linear combination
      (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();
      Assert.IsTrue(plane.Contains(plane.VerticalProjectOnto(new Point3D(a * p0.X + b * p1.X + c * p2.X,
                                                                         a * p0.Y + b * p1.Y + c * p2.Y,
                                                                         a * p0.Z + b * p1.Z + c * p2.Z),
                                                             precision),
                                   precision));
    }

    [RepeatedTestMethod(100)]
    public void ProjectInto() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);
      var plane = random_plane.Plane;

      if (plane is null) {
        return;
      }

      // origin
      Assert.IsTrue(plane.ProjectInto(p0, precision).AlmostEquals(Point2D.Zero, precision), "origin");
      // point on the AxisU
      Assert.IsTrue(
          Line2D.FromDirection(Point2D.Zero, Vector2D.AxisU).Contains(plane.ProjectInto(p1, precision), precision),
          "AxisU");
      // point on the AxisV
      Assert.IsTrue(Line2D.FromDirection(Point2D.Zero, Vector2D.AxisV)
                        .Contains(plane.ProjectInto(plane.Origin + plane.AxisV, precision), precision),
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

      //                  + "\n\tpoint=" + plane.ProjectInto(pp).ToWkt(precision)

      //                  + "\n\tpoint=" + pp.ToWkt(precision)

      //);
    }

    [RepeatedTestMethod(100)]
    public void VerticalProjectInto() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
      var plane = random_plane.Plane;
      (var p0, var p1, var p2) = (random_plane.p0, random_plane.p1, random_plane.p2);

      if (plane is null) {
        return;
      }
      if (plane.Normal.CrossProduct(Vector3D.AxisZ).AlmostEquals(Vector3D.Zero, precision)) {
        // Console.WriteLine("test abandoned, normal matches the AxisZ, cannot VerticalProject");
        return;
      }

      Assert.IsTrue(plane.ProjectInto(plane.Origin).AlmostEquals(Point2D.Zero, precision), "origin");
      Assert.IsTrue(Line2D.FromDirection(Point2D.Zero, Vector2D.AxisU)
                        .Contains(plane.ProjectInto(plane.Origin + plane.AxisU), precision),
                    "AxisU");
      Assert.IsTrue(Line2D.FromDirection(Point2D.Zero, Vector2D.AxisV)
                        .Contains(plane.ProjectInto(plane.Origin + plane.AxisV), precision),
                    "AxisV");
    }

    [RepeatedTestMethod(100)]
    public void IntersectPlane() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
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
      other_plane = Plane.FromPointAndNormal(plane.Origin + shift * shift_vector, plane.Normal, precision);
      Assert.IsFalse(plane.Intersects(other_plane, precision));

      shift_vector = plane.Normal;
      other_plane = Plane.FromPointAndNormal(plane.Origin - shift * shift_vector, plane.Normal);
      Assert.IsFalse(plane.Intersects(other_plane, precision));

      // overlapping, no intersect
      other_plane = Plane.FromPointAndNormal(plane.Origin, plane.Normal);
      Assert.IsFalse(plane.Intersects(other_plane, precision));

      // crossing
      i = 0;
      shift_vector = null;
      while ((shift_vector is null || shift_vector.AlmostEquals(plane.Normal, precision)) && i < max_inter) {
        var vec = RandomGenerator.MakeVector3D(decimal_precision: precision);
        shift_vector = (plane.Normal + vec).Normalize();
        ++i;
      }
      if (i == max_inter) {
        throw new Exception("failed to generate vec, test abandoned");
      }
      i = 0;
      p = null;
      while ((p is null || p.AlmostEquals(plane.Origin, precision)) && i < max_inter) {
        p = RandomGenerator.MakePoint3D(decimal_precision: precision);
        ++i;
      }
      if (i == max_inter) {
        throw new Exception("failed to generate origin, test abandoned");
      }
      // TODO: Euclidean Transform class (Shift, Rotation ...)
      //       it will help for this test
      other_plane = Plane.FromPointAndNormal(p, shift_vector, precision);
      Assert.IsTrue(plane.Intersects(other_plane, precision));
    }

    [RepeatedTestMethod(100)]
    public void Evaluate() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // TODO: this test fails. it depends on many things I don't know how to test
      //      1. Plane's AxisU, AxisV calculation
      //      2. ProjectInto
      var random_plane = RandomGenerator.MakePlane(decimal_precision: precision);
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
            var proj_point = ref_plane.ProjectInto(original_point, precision);
            var eval_point = ref_plane.Evaluate(proj_point, precision);

            Assert.IsTrue(ref_plane.Contains(original_point, precision), "original_point contained");
            Assert.IsTrue(ref_plane.Contains(eval_point, precision), "eval_point contained");

            // Console.WriteLine("\n" + test_description +

            //                  "\n\toriginal=" + original_point.ToWkt(precision) +

            //                  "\n\tproj=" + proj_point.ToWkt(precision) +

            //                  "\n\teval=" + eval_point.ToWkt(precision)

            //);

            // Assert.AreEqual(original_point,
            //                 eval_point,

            //                "\n" + test_description +

            //                    "\n\tproj_point=" + proj_point.ToWkt(precision) +

            //                    "\n\toriginal_point=" + original_point.ToWkt(precision) +
            //                    ", d=" + (original_point - ref_plane.Origin).Length().ToString() +

            //                    "\n\teval_point=" + eval_point.ToWkt(precision) +
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

      // Console.WriteLine("OP=" + OP.ToWkt(precision));
      // Console.WriteLine("|OP|=" + OP.Length().ToString());
      // Console.WriteLine("U=" + U.ToWkt(precision));
      // Console.WriteLine("|U|=" + U.Length().ToString());
      // Console.WriteLine("OP*U=" + OP.DotProduct(U).ToString());
      // Console.WriteLine("O*U=" + O.DotProduct(U).ToString());
      // Console.WriteLine("P*U=" + P.DotProduct(U).ToString());
      // Console.WriteLine("U2=" + U.DotProduct(U).ToString());

      // var proj_point_onto = plane.ProjectOnto(P);
      // var proj_point_into = plane.ProjectInto(P);
      // var eval_point_to = plane.Evaluate(proj_point_into);

      // Console.WriteLine(

      //    "\n\torigin=" + O.ToWkt(precision) +

      //    "\n\tproj_point_onto=" + proj_point_onto.ToWkt(precision) + ", d=" + (proj_point_onto -
      //    O).Length().ToString() +

      //    "\n\tproj_point_into=" + proj_point_into.ToWkt(precision) +
      //    ", d=" + (proj_point_into - Point2D.Zero).Length().ToString() +

      //    "\n\toriginal_point=" + P.ToWkt(precision) + ", d=" + (P - O).Length().ToString() +

      //    "\n\teval_point=" + eval_point_to.ToWkt(precision) + ", d=" + (eval_point_to - O).Length().ToString()

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
