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
  public class Ray3DTests {
    [RepeatedTestMethod(100)]
    public void Containment() {
      // input
      (var ray, var p0, var p1) = RandomGenerator.MakeRay3D();

      if (ray is null) {
        return;
      }

      // temp data
      Point3D p;
      UnitVector3D u = ray.Direction;
      UnitVector3D u_perp =
          u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();

      // must contains the ray points (origin and p1)
      p = p0;
      Assert.IsTrue(ray.Contains(p), "contains" + "\n\tray=" + ray.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p1;
      Assert.IsTrue(ray.Contains(p), "contains" + "\n\tray=" + ray.ToWkt() + "\n\tp=" + p.ToWkt());

      // does not contain points aside the ray
      p = p0 + u_perp;
      Assert.IsFalse(ray.Contains(p), "point aside" + "\n\tray=" + ray.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p0 - u_perp;
      Assert.IsFalse(ray.Contains(p), "point aside" + "\n\tray=" + ray.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p1 + u_perp;
      Assert.IsFalse(ray.Contains(p), "point aside" + "\n\tray=" + ray.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p1 - u_perp;
      Assert.IsFalse(ray.Contains(p), "point aside" + "\n\tray=" + ray.ToWkt() + "\n\tp=" + p.ToWkt());

      // does not contain a point behind the ray
      p = p0 - 2 * u;
      Assert.IsFalse(ray.Contains(p), "point behind" + "\n\tray=" + ray.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p0 - 2 * u - u_perp;
      Assert.IsFalse(ray.Contains(p), "point behind right, ray=" + ray.ToWkt() + ", p=" + p.ToWkt());

      p = p0 - 2 * u + u_perp;
      Assert.IsFalse(ray.Contains(p), "point behind left, ray=" + ray.ToWkt() + ", p=" + p.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      // input
      (var ray, var p0, var p1) = RandomGenerator.MakeRay3D();

      if (ray is null) {
        return;
      }
      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = ray.Direction;

      // temp data
      Ray3D other;
      Vector3D u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ));
      UnitVector3D u_perp_norm = u_perp.Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      other = new Ray3D(mid + u_perp, -u_perp_norm);
      Assert.IsTrue(ray.Intersects(other),
                    "intersect forrreal (+mid)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());
      //      in the middle, crossing
      other = new Ray3D(mid - u_perp, u_perp_norm);
      Assert.IsTrue(ray.Intersects(other),
                    "intersect forrreal (+mid)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());

      //      in the middle no intersection (shooting the other way)
      other = new Ray3D(mid + u_perp, u_perp_norm);
      Assert.IsFalse(ray.Intersects(other),
                     "no intersect (+mid)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());

      //      in the middle no intersection (shooting the other way)
      other = new Ray3D(mid - u_perp, -u_perp_norm);
      Assert.IsFalse(ray.Intersects(other),
                     "no intersect (-mid)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());

      //      on the first extremity, crossing
      other = new Ray3D(p0 + u_perp, -u_perp_norm);
      Assert.IsTrue(ray.Intersects(other),
                    "intersect forrreal (+p0)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());
      other = new Ray3D(p0 - u_perp, u_perp_norm);
      Assert.IsTrue(ray.Intersects(other),
                    "intersect forrreal (-p0)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());

      //      just one point in the middle, going down
      other = new Ray3D(mid, -u_perp_norm);
      Assert.IsTrue(ray.Intersects(other),
                    "intersect forrreal (to mid)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());

      //      just one point in the middle, going up
      other = new Ray3D(mid, u_perp_norm);
      Assert.IsTrue(ray.Intersects(other),
                    "intersect forrreal (from mid)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());

      //      just one point in the the first extremity, going down
      other = new Ray3D(p0, u_perp_norm);
      Assert.IsTrue(ray.Intersects(other),
                    "intersect forrreal (from p0+)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());

      //      just one point in the the first extremity, going up
      other = new Ray3D(p0, -u_perp_norm);
      Assert.IsTrue(ray.Intersects(other),
                    "intersect forrreal (from p0-)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector3D shift = RandomGenerator.MakeVector3D();
      if (Math.Round(shift.Length(), Constants.NINE_DECIMALS) == 0) {
        other = new Ray3D(p0 + shift, u);
        Assert.IsFalse(ray.Intersects(other),
                       "no intersection (parallel, shift upwards random vector)" + "\nray1=" + ray.ToWkt() +
                           ", ray2=" + other.ToWkt());

        //      downwards
        other = new Ray3D(p0 - shift, u);
        Assert.IsFalse(ray.Intersects(other),
                       "no intersection (parallel, shift downwards random vector)" + "\nray1=" + ray.ToWkt() +
                           ", ray2=" + other.ToWkt());
      }
    }

    [RepeatedTestMethod(100)]
    public void Overlap() {
      // input
      (var ray, var p0, var p1) = RandomGenerator.MakeRay3D();

      if (ray is null) {
        return;
      }
      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = ray.Direction;

      // temp data
      Ray3D other;
      Vector3D u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ));
      UnitVector3D u_perp_norm = u_perp.Normalize();

      // case 1: overlap start point
      //      not insersect but overlap
      other = new Ray3D(p0 + 2 * u, u);
      Assert.IsFalse(ray.Intersects(other),
                     "overlap start point" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());
      Assert.IsTrue(ray.Overlaps(other), "overlap start point" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());

      // case 2: overlap end point
      //      not insersect but overlap
      other = new Ray3D(p0 - 2 * u, u);
      Assert.IsFalse(ray.Intersects(other), "overlap end point" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());
      Assert.IsTrue(ray.Overlaps(other), "overlap end point" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());

      // case 3: overlap both (first segment contained in the second)
      //      not insersect but overlap
      other = new Ray3D(p0 + u, -u);
      Assert.IsFalse(
          ray.Intersects(other),
          "overlap both (first segment contained in the second)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());
      Assert.IsTrue(
          ray.Overlaps(other),
          "overlap both (first segment contained in the second)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());
      Assert.AreEqual(
          ray.Overlap(other).ValueType,
          typeof(LineSegment3D),
          "overlap both (first segment contained in the second)" + "\nray1=" + ray.ToWkt() + ", ray2=" + other.ToWkt());
    }
  }
}
