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
    public void Intersection() {
      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine3D();

      if (line is null) {
        Assert.AreEqual(p0, p1);
      } else {
        var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
        var u = (line.P1 - line.P0).Normalize();

        // temp data
        Line3D other;
        Vector3D u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ));

        // case 1: intersect forrreal
        //      in the middle, crossing
        other = Line3D.FromPoints(mid - 2 * u_perp, mid + 2 * u_perp);
        Assert.IsTrue(line.Intersects(other),
                      "intersect forrreal (mid)" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        //      on the first extremity, crossing
        other = Line3D.FromPoints(p0 - 2 * u_perp, p0 + 2 * u_perp);
        Assert.IsTrue(line.Intersects(other),
                      "intersect forrreal (p0)" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        //      on the second extremity, crossing
        other = Line3D.FromPoints(p1 - 2 * u_perp, p1 + 2 * u_perp);
        Assert.IsTrue(line.Intersects(other),
                      "intersect forrreal (p1)" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        //      just one point in the middle, going down
        other = Line3D.FromPoints(mid - 2 * u_perp, mid);
        Assert.IsTrue(line.Intersects(other),
                      "intersect forrreal (to mid)" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        //      just one point in the middle, going up
        other = Line3D.FromPoints(mid, mid + 2 * u_perp);
        Assert.IsTrue(line.Intersects(other),
                      "intersect forrreal (from mid)" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        //      just one point in the the first extremity, going down
        other = Line3D.FromPoints(p0 - 2 * u_perp, p0);
        Assert.IsTrue(line.Intersects(other),
                      "intersect forrreal (to p0)" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        //      just one point in the the first extremity, going up
        other = Line3D.FromPoints(p0 + 2 * u_perp, p0);
        Assert.IsTrue(line.Intersects(other),
                      "intersect forrreal (from p0)" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        //      just one point in the the second extremity, going up
        other = Line3D.FromPoints(p1, p1 + 2 * u_perp);
        Assert.IsTrue(line.Intersects(other),
                      "intersect forrreal (from p1)" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        //      just one point in the the second extremity, going down
        other = Line3D.FromPoints(p1, p1 - 2 * u_perp);
        Assert.IsTrue(line.Intersects(other),
                      "intersect forrreal (to p1)" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        // case 2: no intersection (parallel, shift random vector)
        //      upwards
        Vector3D shift = RandomGenerator.MakeVector3D();
        other = Line3D.FromPoints(p0 + 2 * shift, p1 + 2 * shift);
        Assert.IsFalse(line.Intersects(other),
                       "no intersection (parallel, shift upwards random vector)" + "\n\tl1=" + line.ToWkt() +
                           "\n\tl2=" + other.ToWkt());

        //      downwards
        other = Line3D.FromPoints(p0 - 2 * shift, p1 - 2 * shift);
        Assert.IsFalse(line.Intersects(other),
                       "no intersection (parallel, shift downwards random vector)" + "\n\tl1=" + line.ToWkt() +
                           "\n\tl2=" + other.ToWkt());
      }
    }

    [RepeatedTestMethod(100)]
    public void Overlap() {
      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine3D();

      if (line is null) {
        Assert.AreEqual(p0, p1);
      } else {
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
        Assert.IsTrue(line.Overlaps(other),
                      "overlap start point" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        // case 2: overlap end point
        //      not insersect but overlap
        other = Line3D.FromPoints(mid - (line.P1 - line.P0).Length() * u, mid);
        Assert.IsFalse(line.Intersects(other),
                       "overlap end point" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());
        Assert.IsTrue(line.Overlaps(other), "overlap end point" + "\n\tl1=" + line.ToWkt() + "\n\tl2=" + other.ToWkt());

        // case 3: overlap both (second segment is contained in the first)
        //      not insersect but overlap
        other = Line3D.FromPoints(p0 + 0.25 * (line.P1 - line.P0).Length() * u,
                                  p1 - 0.25 * (line.P1 - line.P0).Length() * u);
        Assert.IsFalse(line.Intersects(other),
                       "overlap both (second segment is contained in the first)" + "\n\tl1=" + line.ToWkt() +
                           "\n\tl2=" + other.ToWkt());
        Assert.IsTrue(line.Overlaps(other),
                      "overlap both (second segment is contained in the first)" + "\n\tl1=" + line.ToWkt() +
                          "\n\tl2=" + other.ToWkt());

        // case 4: overlap both (first segment contained in the second)
        //      not insersect but overlap
        other = Line3D.FromPoints(p0 - 0.25 * (line.P1 - line.P0).Length() * u,
                                  p1 + 0.25 * (line.P1 - line.P0).Length() * u);
        Assert.IsFalse(line.Intersects(other),
                       "overlap both (first segment contained in the second)" + "\n\tl1=" + line.ToWkt() +
                           "\n\tl2=" + other.ToWkt());
        Assert.IsTrue(line.Overlaps(other),
                      "overlap both (first segment contained in the second)" + "\n\tl1=" + line.ToWkt() +
                          "\n\tl2=" + other.ToWkt());
      }
    }
  }

}
