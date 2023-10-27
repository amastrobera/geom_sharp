// internal
using GeomSharp;

// external
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {
  /// <summary>
  /// tests for all the MathNet.Spatial.Euclidean functions I created
  /// </summary>
  [TestClass]
  public class Ray2DTests {
    [RepeatedTestMethod(100)]
    public void Containment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var ray, var p0, var p1) = RandomGenerator.MakeRay2D(decimal_precision: precision);

      if (ray is null) {
        return;
      }

      // temp data
      Point2D p;
      UnitVector2D u = ray.Direction;
      UnitVector2D u_perp = u.Perp().Normalize();

      // must contains the ray points (origin and p1)
      p = p0;
      Assert.IsTrue(ray.Contains(p, precision),
                    "contains" + "\n\tray=" + ray.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = p1;
      Assert.IsTrue(ray.Contains(p, precision),
                    "contains" + "\n\tray=" + ray.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      // does not contain points aside the ray
      p = p0 + u_perp;
      Assert.IsFalse(ray.Contains(p, precision),
                     "point aside" + "\n\tray=" + ray.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = p0 - u_perp;
      Assert.IsFalse(ray.Contains(p, precision),
                     "point aside" + "\n\tray=" + ray.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = p1 + u_perp;
      Assert.IsFalse(ray.Contains(p, precision),
                     "point aside" + "\n\tray=" + ray.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = p1 - u_perp;
      Assert.IsFalse(ray.Contains(p, precision),
                     "point aside" + "\n\tray=" + ray.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      // does not contain a point behind the ray
      p = p0 - 2 * u;
      Assert.IsFalse(ray.Contains(p, precision),
                     "point behind" + "\n\tray=" + ray.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = p0 - 2 * u - u_perp;
      Assert.IsFalse(ray.Contains(p, precision),
                     "point behind right, ray=" + ray.ToWkt(precision) + ", p=" + p.ToWkt(precision));

      p = p0 - 2 * u + u_perp;
      Assert.IsFalse(ray.Contains(p, precision),
                     "point behind left, ray=" + ray.ToWkt(precision) + ", p=" + p.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var ray, var p0, var p1) = RandomGenerator.MakeRay2D(decimal_precision: precision);

      if (ray is null) {
        return;
      }
      var mid = Point2D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = ray.Direction;

      // temp data
      Ray2D other;
      Vector2D u_perp = u.Perp();
      UnitVector2D u_perp_norm = u_perp.Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      other = new Ray2D(mid + u_perp, -u_perp_norm);
      Assert.IsTrue(
          ray.Intersects(other, precision),
          "intersect forrreal (+mid)" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));
      //      in the middle, crossing
      other = new Ray2D(mid - u_perp, u_perp_norm);
      Assert.IsTrue(
          ray.Intersects(other, precision),
          "intersect forrreal (+mid)" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));

      //      in the middle no intersection (shooting the other way)
      other = new Ray2D(mid + u_perp, u_perp_norm);
      Assert.IsFalse(ray.Intersects(other, precision),
                     "no intersect (+mid)" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));

      //      in the middle no intersection (shooting the other way)
      other = new Ray2D(mid - u_perp, -u_perp_norm);
      Assert.IsFalse(ray.Intersects(other, precision),
                     "no intersect (-mid)" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));

      //      on the first extremity, crossing
      other = new Ray2D(p0 + u_perp, -u_perp_norm);
      Assert.IsTrue(ray.Intersects(other, precision),
                    "intersect forrreal (+p0)" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));
      other = new Ray2D(p0 - u_perp, u_perp_norm);
      Assert.IsTrue(ray.Intersects(other, precision),
                    "intersect forrreal (-p0)" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));

      //      just one point in the middle, going down
      other = new Ray2D(mid, -u_perp_norm);
      Assert.IsTrue(
          ray.Intersects(other, precision),
          "intersect forrreal (to mid)" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));

      //      just one point in the middle, going up
      other = new Ray2D(mid, u_perp_norm);
      Assert.IsTrue(
          ray.Intersects(other, precision),
          "intersect forrreal (from mid)" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));

      //      just one point in the the first extremity, going down
      other = new Ray2D(p0, u_perp_norm);
      Assert.IsTrue(
          ray.Intersects(other, precision),
          "intersect forrreal (from p0+)" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));

      //      just one point in the the first extremity, going up
      other = new Ray2D(p0, -u_perp_norm);
      Assert.IsTrue(
          ray.Intersects(other, precision),
          "intersect forrreal (from p0-)" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector2D shift = RandomGenerator.MakeVector2D(decimal_precision: precision);
      if (Math.Round(shift.Length(), Constants.NINE_DECIMALS) != 0) {
        other = new Ray2D(p0 + shift, u);
        Assert.IsFalse(ray.Intersects(other, precision),
                       "no intersection (parallel, shift upwards random vector)" + "\nray1=" + ray.ToWkt(precision) +
                           ", ray2=" + other.ToWkt(precision));

        //      downwards
        other = new Ray2D(p0 - shift, u);
        Assert.IsFalse(ray.Intersects(other, precision),
                       "no intersection (parallel, shift downwards random vector)" + "\nray1=" + ray.ToWkt(precision) +
                           ", ray2=" + other.ToWkt(precision));
      }
    }

    [RepeatedTestMethod(100)]
    public void Overlap() {
      int precision = RandomGenerator.MakeInt(0, 9);
      // input
      (var ray, var p0, var p1) = RandomGenerator.MakeRay2D(decimal_precision: precision);

      if (ray is null) {
        return;
      }
      var mid = Point2D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = ray.Direction;

      // temp data
      Ray2D other;
      Vector2D u_perp = u.Perp();
      UnitVector2D u_perp_norm = u_perp.Normalize();

      // case 1: overlap start point
      //      not insersect but overlap
      other = new Ray2D(p0 + 2 * u, u);
      Assert.IsFalse(ray.Intersects(other, precision),
                     "overlap start point" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));
      Assert.IsTrue(ray.Overlaps(other, precision),
                    "overlap start point" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));

      // case 2: overlap end point
      //      not insersect but overlap
      other = new Ray2D(p0 - 2 * u, u);
      Assert.IsFalse(ray.Intersects(other, precision),
                     "overlap end point" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));
      Assert.IsTrue(ray.Overlaps(other, precision),
                    "overlap end point" + "\nray1=" + ray.ToWkt(precision) + ", ray2=" + other.ToWkt(precision));

      // case 3: overlap both (first segment contained in the second)
      //      not insersect but overlap
      other = new Ray2D(p0 + u, -u);
      Assert.IsFalse(ray.Intersects(other, precision),
                     "overlap both (first segment contained in the second)" + "\nray1=" + ray.ToWkt(precision) +
                         ", ray2=" + other.ToWkt(precision));
      Assert.IsTrue(ray.Overlaps(other, precision),
                    "overlap both (first segment contained in the second)" + "\nray1=" + ray.ToWkt(precision) +
                        ", ray2=" + other.ToWkt(precision));
      Assert.AreEqual(ray.Overlap(other, precision).ValueType,
                      typeof(LineSegment2D),
                      "overlap both (first segment contained in the second)" + "\nray1=" + ray.ToWkt(precision) +
                          ", ray2=" + other.ToWkt(precision));
    }
  }
}
