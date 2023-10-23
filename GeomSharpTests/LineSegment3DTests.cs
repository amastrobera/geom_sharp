// internal
using GeomSharp;

// external
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {

  /// <summary>
  /// tests for all the MathNet.Spatial.Euclidean functions I created
  /// </summary>
  [TestClass]
  public class LineSegment3DTests {
    [RepeatedTestMethod(100)]
    public void Containment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine3D(decimal_precision: precision);

      if (line is null) {
        return;
      }

      // temp data
      Point3D p;
      UnitVector3D u = line.Direction;
      UnitVector3D u_perp =
          u.CrossProduct((u.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();

      // must contains the ray points (origin and p1)
      p = p0;
      Assert.IsTrue(line.Contains(p, precision),
                    "contains extremities" + "\n\tray=" + line.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = p1;
      Assert.IsTrue(line.Contains(p, precision),
                    "contains extremities" + "\n\tray=" + line.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      (double a, double b) = RandomGenerator.MakeLinearCombo2SumTo1();
      p = Point3D.FromVector(a * p0.ToVector() + b * p1.ToVector());
      Assert.IsTrue(line.Contains(p, precision),
                    "contains random mid" + "\n\tray=" + line.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      // does not contain points aside the ray
      p = p0 + u_perp;
      Assert.IsFalse(line.Contains(p, precision),
                     "point aside" + "\n\tray=" + line.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = p0 - u_perp;
      Assert.IsFalse(line.Contains(p, precision),
                     "point aside" + "\n\tray=" + line.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = p1 + u_perp;
      Assert.IsFalse(line.Contains(p, precision),
                     "point aside" + "\n\tray=" + line.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = p1 - u_perp;
      Assert.IsFalse(line.Contains(p, precision),
                     "point aside" + "\n\tray=" + line.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      int precision = RandomGenerator.MakeInt(0, 9);
      // input
      (var seg, var p0, var p1) = RandomGenerator.MakeLineSegment3D(decimal_precision: precision);

      if (seg is null) {
        return;
      }
      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = (seg.P1 - seg.P0).Normalize();

      // temp data
      LineSegment3D other;
      Vector3D u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ));

      // case 1: intersect forrreal
      //      in the middle, crossing
      other = LineSegment3D.FromPoints(mid - 2 * u_perp, mid + 2 * u_perp, precision);
      Assert.IsTrue(seg.Intersects(other, precision),
                    "intersect forrreal (mid)" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      //      on the first extremity, crossing
      other = LineSegment3D.FromPoints(p0 - 2 * u_perp, p0 + 2 * u_perp, precision);
      Assert.IsTrue(seg.Intersects(other, precision),
                    "intersect forrreal (p0)" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      //      on the second extremity, crossing
      other = LineSegment3D.FromPoints(p1 - 2 * u_perp, p1 + 2 * u_perp, precision);
      Assert.IsTrue(seg.Intersects(other, precision),
                    "intersect forrreal (p1)" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      //      just one point in the middle, going down
      other = LineSegment3D.FromPoints(mid - 2 * u_perp, mid, precision);
      Assert.IsTrue(seg.Intersects(other, precision),
                    "intersect forrreal (to mid)" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      //      just one point in the middle, going up
      other = LineSegment3D.FromPoints(mid, mid + 2 * u_perp, precision);
      Assert.IsTrue(
          seg.Intersects(other, precision),
          "intersect forrreal (from mid)" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      //      just one point in the the first extremity, going down
      other = LineSegment3D.FromPoints(p0 - 2 * u_perp, p0, precision);
      Assert.IsTrue(seg.Intersects(other, precision),
                    "intersect forrreal (to p0)" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      //      just one point in the the first extremity, going up
      other = LineSegment3D.FromPoints(p0 + 2 * u_perp, p0, precision);
      Assert.IsTrue(seg.Intersects(other, precision),
                    "intersect forrreal (from p0)" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      //      just one point in the the second extremity, going up
      other = LineSegment3D.FromPoints(p1, p1 + 2 * u_perp, precision);
      Assert.IsTrue(seg.Intersects(other, precision),
                    "intersect forrreal (from p1)" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      //      just one point in the the second extremity, going down
      other = LineSegment3D.FromPoints(p1, p1 - 2 * u_perp, precision);
      Assert.IsTrue(seg.Intersects(other, precision),
                    "intersect forrreal (to p1)" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      Vector3D shift = RandomGenerator.MakeVector3D(decimal_precision: precision);
      other = LineSegment3D.FromPoints(p0 + 2 * shift, p1 + 2 * shift, precision);
      Assert.IsFalse(seg.Intersects(other, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + seg.ToWkt(precision) +
                         ", s2=" + other.ToWkt(precision));

      //      downwards
      other = LineSegment3D.FromPoints(p0 - 2 * shift, p1 - 2 * shift, precision);
      Assert.IsFalse(seg.Intersects(other, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + seg.ToWkt(precision) +
                         ", s2=" + other.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void Overlap() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var seg, var p0, var p1) = RandomGenerator.MakeLineSegment3D(decimal_precision: precision);

      if (seg is null) {
        return;
      }
      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = (seg.P1 - seg.P0).Normalize();

      // temp data
      LineSegment3D other;
      Vector3D u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ));

      // case 1: overlap start point
      //      not insersect but overlap
      other = LineSegment3D.FromPoints(mid, mid + seg.Length() * u, precision);
      Assert.IsFalse(seg.Intersects(other, precision),
                     "overlap start point" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));
      Assert.IsTrue(seg.Overlaps(other),
                    "overlap start point" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      // case 2: overlap end point
      //      not insersect but overlap
      other = LineSegment3D.FromPoints(mid - seg.Length() * u, mid, precision);
      Assert.IsFalse(seg.Intersects(other, precision),
                     "overlap end point" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));
      Assert.IsTrue(seg.Overlaps(other),
                    "overlap end point" + "\ns1=" + seg.ToWkt(precision) + ", s2=" + other.ToWkt(precision));

      // case 3: overlap both (second segment is contained in the first)
      //      not insersect but overlap
      other = LineSegment3D.FromPoints(p0 + 0.25 * seg.Length() * u, p1 - 0.25 * seg.Length() * u, precision);
      Assert.IsFalse(seg.Intersects(other, precision),
                     "overlap both (second segment is contained in the first)" + "\ns1=" + seg.ToWkt(precision) +
                         ", s2=" + other.ToWkt(precision));
      Assert.IsTrue(seg.Overlaps(other),
                    "overlap both (second segment is contained in the first)" + "\ns1=" + seg.ToWkt(precision) +
                        ", s2=" + other.ToWkt(precision));

      // case 4: overlap both (first segment contained in the second)
      //      not insersect but overlap
      other = LineSegment3D.FromPoints(p0 - 0.25 * seg.Length() * u, p1 + 0.25 * seg.Length() * u, precision);
      Assert.IsFalse(seg.Intersects(other, precision),
                     "overlap both (first segment contained in the second)" + "\ns1=" + seg.ToWkt(precision) +
                         ", s2=" + other.ToWkt(precision));
      Assert.IsTrue(seg.Overlaps(other),
                    "overlap both (first segment contained in the second)" + "\ns1=" + seg.ToWkt(precision) +
                        ", s2=" + other.ToWkt(precision));
    }
  }
}
