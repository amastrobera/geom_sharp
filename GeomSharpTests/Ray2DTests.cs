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
    public void Intersection() {
      // input
      (var ray, var p0, var p1) = RandomGenerator.MakeRay2D();

      if (ray is null) {
        Assert.AreEqual(p0, p1);
      } else {
        var mid = Point2D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
        var u = ray.Direction;

        // temp data
        Ray2D other;
        Vector2D u_perp = u.Perp();
        UnitVector2D u_perp_norm = u_perp.Normalize();

        // case 1: intersect forrreal
        //      in the middle, crossing
        other = new Ray2D(mid + u_perp, -u_perp_norm);
        Assert.IsTrue(ray.Intersects(other),
                      "intersect forrreal (+mid)" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                          ", ray2=" + other.ToWkt(make_unit_line: true));
        //      in the middle, crossing
        other = new Ray2D(mid - u_perp, u_perp_norm);
        Assert.IsTrue(ray.Intersects(other),
                      "intersect forrreal (+mid)" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                          ", ray2=" + other.ToWkt(make_unit_line: true));

        //      in the middle no intersection (shooting the other way)
        other = new Ray2D(mid + u_perp, u_perp_norm);
        Assert.IsFalse(ray.Intersects(other),
                       "no intersect (+mid)" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                           ", ray2=" + other.ToWkt(make_unit_line: true));

        //      in the middle no intersection (shooting the other way)
        other = new Ray2D(mid - u_perp, -u_perp_norm);
        Assert.IsFalse(ray.Intersects(other),
                       "no intersect (-mid)" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                           ", ray2=" + other.ToWkt(make_unit_line: true));

        //      on the first extremity, crossing
        other = new Ray2D(p0 + u_perp, -u_perp_norm);
        Assert.IsTrue(ray.Intersects(other),
                      "intersect forrreal (+p0)" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                          ", ray2=" + other.ToWkt(make_unit_line: true));
        other = new Ray2D(p0 - u_perp, u_perp_norm);
        Assert.IsTrue(ray.Intersects(other),
                      "intersect forrreal (-p0)" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                          ", ray2=" + other.ToWkt(make_unit_line: true));

        //      just one point in the middle, going down
        other = new Ray2D(mid, -u_perp_norm);
        Assert.IsTrue(ray.Intersects(other),
                      "intersect forrreal (to mid)" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                          ", ray2=" + other.ToWkt(make_unit_line: true));

        //      just one point in the middle, going up
        other = new Ray2D(mid, u_perp_norm);
        Assert.IsTrue(ray.Intersects(other),
                      "intersect forrreal (from mid)" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                          ", ray2=" + other.ToWkt(make_unit_line: true));

        //      just one point in the the first extremity, going down
        other = new Ray2D(p0, u_perp_norm);
        Assert.IsTrue(ray.Intersects(other),
                      "intersect forrreal (from p0+)" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                          ", ray2=" + other.ToWkt(make_unit_line: true));

        //      just one point in the the first extremity, going up
        other = new Ray2D(p0, -u_perp_norm);
        Assert.IsTrue(ray.Intersects(other),
                      "intersect forrreal (from p0-)" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                          ", ray2=" + other.ToWkt(make_unit_line: true));

        // case 2: no intersection (parallel, shift random vector)
        //      upwards
        Vector2D shift = RandomGenerator.MakeVector2D();
        if (Math.Round(shift.Length(), Constants.NINE_DECIMALS) != 0) {
          other = new Ray2D(p0 + shift, u);
          Assert.IsFalse(ray.Intersects(other),
                         "no intersection (parallel, shift upwards random vector)" + "\nray1=" +
                             ray.ToWkt(make_unit_line: true) + ", ray2=" + other.ToWkt(make_unit_line: true));

          //      downwards
          other = new Ray2D(p0 - shift, u);
          Assert.IsFalse(ray.Intersects(other),
                         "no intersection (parallel, shift downwards random vector)" + "\nray1=" +
                             ray.ToWkt(make_unit_line: true) + ", ray2=" + other.ToWkt(make_unit_line: true));
        }
      }
    }

    [RepeatedTestMethod(100)]
    public void Overlap() {
      // input
      (var ray, var p0, var p1) = RandomGenerator.MakeRay2D();

      if (ray is null) {
        Assert.AreEqual(p0, p1);
      } else {
        var mid = Point2D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
        var u = ray.Direction;

        // temp data
        Ray2D other;
        Vector2D u_perp = u.Perp();
        UnitVector2D u_perp_norm = u_perp.Normalize();

        // case 1: overlap start point
        //      not insersect but overlap
        other = new Ray2D(p0 + 2 * u, u);
        Assert.IsFalse(ray.Intersects(other),
                       "overlap start point" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                           ", ray2=" + other.ToWkt(make_unit_line: true));
        Assert.IsTrue(ray.Overlaps(other),
                      "overlap start point" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                          ", ray2=" + other.ToWkt(make_unit_line: true));

        // case 2: overlap end point
        //      not insersect but overlap
        other = new Ray2D(p0 - 2 * u, u);
        Assert.IsFalse(ray.Intersects(other),
                       "overlap end point" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                           ", ray2=" + other.ToWkt(make_unit_line: true));
        Assert.IsTrue(ray.Overlaps(other),
                      "overlap end point" + "\nray1=" + ray.ToWkt(make_unit_line: true) +
                          ", ray2=" + other.ToWkt(make_unit_line: true));

        // case 3: overlap both (first segment contained in the second)
        //      not insersect but overlap
        other = new Ray2D(p0 + u, -u);
        Assert.IsFalse(ray.Intersects(other),
                       "overlap both (first segment contained in the second)" +
                           "\nray1=" + ray.ToWkt(make_unit_line: true) + ", ray2=" + other.ToWkt(make_unit_line: true));
        Assert.IsTrue(ray.Overlaps(other),
                      "overlap both (first segment contained in the second)" +
                          "\nray1=" + ray.ToWkt(make_unit_line: true) + ", ray2=" + other.ToWkt(make_unit_line: true));
        Assert.AreEqual(ray.Overlap(other).ValueType,
                        typeof(LineSegment2D),
                        "overlap both (first segment contained in the second)" + "\nray1=" +
                            ray.ToWkt(make_unit_line: true) + ", ray2=" + other.ToWkt(make_unit_line: true));
        ;
      }
    }
  }
}
