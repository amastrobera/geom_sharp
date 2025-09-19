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
  public class Line3DTests {
    [RepeatedTestMethod(100)]
    public void Containment() {
      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine3D();

      if (line is null) {
        return;
      }

      // temp data
      Point3D p;
      UnitVector3D u = line.Direction;
      UnitVector3D u_perp =
          u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();

      // must contains the ray points (origin and p1)
      p = p0;
      Assert.IsTrue(line.Contains(p), "contains" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p1;
      Assert.IsTrue(line.Contains(p), "contains" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p0 - 2 * u;
      Assert.IsTrue(line.Contains(p), "contains behind" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      // does not contain points aside the ray
      p = p0 + u_perp;
      Assert.IsFalse(line.Contains(p), "point aside" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p0 - u_perp;
      Assert.IsFalse(line.Contains(p), "point aside" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p1 + u_perp;
      Assert.IsFalse(line.Contains(p), "point aside" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());

      p = p1 - u_perp;
      Assert.IsFalse(line.Contains(p), "point aside" + "\n\tray=" + line.ToWkt() + "\n\tp=" + p.ToWkt());
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine3D();

      if (line is null) {
        return;
      }
      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = (line.P1 - line.P0).Normalize();

      // temp data
      Line3D other;
      Vector3D u_perp = u.Perp();
      IntersectionResult int_res = null;
      Point3D int_pt = null;

      // case 1: intersect forrreal
      //      in the middle, crossing
      other = Line3D.FromPoints_NoThrow(mid - 2 * u_perp, mid + 2 * u_perp);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType != typeof(NullValue),
                      "intersect forrreal (mid)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
        int_pt = (Point3D)int_res.Value;
        Assert.IsTrue(line.Contains(int_pt) && other.Contains(int_pt),
                      "intersect forrreal (mid)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
      }

      //      on the first extremity, crossing
      other = Line3D.FromPoints_NoThrow(p0 - 2 * u_perp, p0 + 2 * u_perp);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType != typeof(NullValue),
                      "intersect forrreal (p0)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
        int_pt = (Point3D)int_res.Value;
        Assert.IsTrue(line.Contains(int_pt) && other.Contains(int_pt),
                      "intersect forrreal (p0)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
      }

      //      on the second extremity, crossing
      other = Line3D.FromPoints_NoThrow(p1 - 2 * u_perp, p1 + 2 * u_perp);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType != typeof(NullValue),
                      "intersect forrreal (p1)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
        int_pt = (Point3D)int_res.Value;
        Assert.IsTrue(line.Contains(int_pt) && other.Contains(int_pt),
                      "intersect forrreal (p1)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
      }

      //      just one point in the middle, going down
      other = Line3D.FromPoints_NoThrow(mid - 2 * u_perp, mid);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType != typeof(NullValue),
                      "intersect forrreal (to mid)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
        int_pt = (Point3D)int_res.Value;
        Assert.IsTrue(line.Contains(int_pt) && other.Contains(int_pt),
                      "intersect forrreal (to mid)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
      }

      //      just one point in the middle, going up
      other = Line3D.FromPoints_NoThrow(mid, mid + 2 * u_perp);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType != typeof(NullValue),
                      "intersect forrreal (from mid)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
        int_pt = (Point3D)int_res.Value;
        Assert.IsTrue(line.Contains(int_pt) && other.Contains(int_pt),
                      "intersect forrreal (from mid)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
      }

      //      just one point in the the first extremity, going down
      other = Line3D.FromPoints_NoThrow(p0 - 2 * u_perp, p0);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType != typeof(NullValue),
                      "intersect forrreal (to p0)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
        int_pt = (Point3D)int_res.Value;
        Assert.IsTrue(line.Contains(int_pt) && other.Contains(int_pt),
                      "intersect forrreal (to p0)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
      }

      //      just one point in the the first extremity, going up
      other = Line3D.FromPoints_NoThrow(p0 + 2 * u_perp, p0);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType != typeof(NullValue),
                      "intersect forrreal (from p0)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
        int_pt = (Point3D)int_res.Value;
        Assert.IsTrue(line.Contains(int_pt) && other.Contains(int_pt),
                      "intersect forrreal (from p0)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
      }

      //      just one point in the the second extremity, going up
      other = Line3D.FromPoints_NoThrow(p1, p1 + 2 * u_perp);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType != typeof(NullValue),
                      "intersect forrreal (from p1)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
        int_pt = (Point3D)int_res.Value;
        Assert.IsTrue(line.Contains(int_pt) && other.Contains(int_pt),
                      "intersect forrreal (from p1)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
      }

      //      just one point in the the second extremity, going down
      other = Line3D.FromPoints_NoThrow(p1, p1 - 2 * u_perp);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType != typeof(NullValue),
                      "intersect forrreal (to p1)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
        int_pt = (Point3D)int_res.Value;
        Assert.IsTrue(line.Contains(int_pt) && other.Contains(int_pt),
                      "intersect forrreal (to p1)" + "\nl1=" + line.ToWkt() + ", l2=" + other.ToWkt());
      }

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector3D shift = RandomGenerator.MakeVector3D();
      other = Line3D.FromPoints_NoThrow(p0 + 2 * shift, p1 + 2 * shift);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType == typeof(NullValue),
                      "no intersection (parallel, shift upwards random vector)" + "\nl1=" + line.ToWkt() +
                          ", l2=" + other.ToWkt());
      }

      //      downwards
      other = Line3D.FromPoints_NoThrow(p0 - 2 * shift, p1 - 2 * shift);
      if (other != null) {
        int_res = line.Intersection(other);
        Assert.IsTrue(int_res.ValueType == typeof(NullValue),
                      "no intersection (parallel, shift downwards random vector)" + "\nl1=" + line.ToWkt() +
                          ", l2=" + other.ToWkt());
      }
    }

    [RepeatedTestMethod(100)]
    public void Overlap() {
      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine3D();

      if (line is null) {
        return;
      }
      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = (line.P1 - line.P0).Normalize();

      // temp data
      Line3D other;
      Vector3D u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ));

      // case 1: overlap start point
      //      not insersect but overlap
      other = Line3D.FromPoints(mid, mid + (line.P1 - line.P0).Length() * u);
      Assert.IsFalse(line.Intersects(other),
                     "overlap start point" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());
      Assert.IsTrue(line.Overlaps(other), "overlap start point" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

      // case 2: overlap end point
      //      not insersect but overlap
      other = Line3D.FromPoints(mid - (line.P1 - line.P0).Length() * u, mid);
      Assert.IsFalse(line.Intersects(other),
                     "overlap end point" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());
      Assert.IsTrue(line.Overlaps(other), "overlap end point" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

      // case 3: overlap both (second segment is contained in the first)
      //      not insersect but overlap
      other =
          Line3D.FromPoints(p0 + 0.25 * (line.P1 - line.P0).Length() * u, p1 - 0.25 * (line.P1 - line.P0).Length() * u);
      Assert.IsFalse(line.Intersects(other),
                     "overlap both (second segment is contained in the first)" + "\n\tl1=" + line.ToWkt() +
                         "\n\tl2=" + other.ToWkt());
      Assert.IsTrue(line.Overlaps(other),
                    "overlap both (second segment is contained in the first)" + "\n\tl1=" + line.ToWkt() +
                        "\n\tl2=" + other.ToWkt());

      // case 4: overlap both (first segment contained in the second)
      //      not insersect but overlap
      other =
          Line3D.FromPoints(p0 - 0.25 * (line.P1 - line.P0).Length() * u, p1 + 0.25 * (line.P1 - line.P0).Length() * u);
      Assert.IsFalse(line.Intersects(other),
                     "overlap both (first segment contained in the second)" + "\n\tl1=" + line.ToWkt() +
                         "\n\tl2=" + other.ToWkt());
      Assert.IsTrue(line.Overlaps(other),
                    "overlap both (first segment contained in the second)" + "\n\tl1=" + line.ToWkt() +
                        "\n\tl2=" + other.ToWkt());
    }
  }

}
