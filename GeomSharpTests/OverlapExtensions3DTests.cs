// internal
using GeomSharp;

// external
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {
  /// <summary>
  /// tests for all the MathNet.Spatial.Euclidean functions I created
  /// </summary>
  [TestClass]
  public class OverlapExtensions3DTests {
    // Line, LineSegment, Ray cross test
    [RepeatedTestMethod(100)]
    public void LineToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var line, var p0, var p1) = RandomGenerator.MakeLine3D(decimal_precision: precision);

      if (line is null) {
        return;
      }

      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = line.Direction;

      // temp data
      Ray3D ray;
      UnitVector3D u_perp =
          u.CrossProduct((u.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "intersect forrreal (mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going down
      ray = new Ray3D(mid, -u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "intersect forrreal (to mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going up
      ray = new Ray3D(mid, u_perp);
      Assert.IsFalse(
          line.Overlaps(ray, precision),
          "intersect forrreal (from mid)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going down
      ray = new Ray3D(p0, -u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "intersect forrreal (to p0)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going up
      ray = new Ray3D(p0, u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "intersect forrreal (from p0)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      ray = new Ray3D(p0 + 2 * u_perp, u);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + line.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      //      downwards
      ray = new Ray3D(p0 - 2 * u_perp, -u);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + line.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "not intersect (mid-)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      in the middle, crossing
      ray = new Ray3D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(line.Overlaps(ray, precision),
                     "not intersect (mid+)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 4: overlap
      //      ray starts from the middle of the line
      ray = new Ray3D(mid, u);
      Assert.IsTrue(line.Overlaps(ray, precision),
                    "overlap (mid+)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      ray = new Ray3D(mid, -u);
      Assert.IsTrue(line.Overlaps(ray, precision),
                    "overlap (mid-)" + "\ns1=" + line.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void LineToLineSegment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var segment, var p0, var p1) = RandomGenerator.MakeLineSegment3D(decimal_precision: precision);

      if (segment is null) {
        return;
      }

      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = segment.ToLine().Direction;

      // temp data
      Line3D line;
      UnitVector3D u_perp =
          u.CrossProduct((u.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      line = Line3D.FromDirection(mid - 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Overlaps(line, precision),
                     "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the middle, going down
      line = Line3D.FromDirection(mid, -u_perp);
      Assert.IsFalse(
          segment.Overlaps(line, precision),
          "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the first extremity, going down
      line = Line3D.FromDirection(p0, -u_perp);
      Assert.IsFalse(
          segment.Overlaps(line, precision),
          "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the first extremity, going up
      line = Line3D.FromDirection(p1, u_perp);
      Assert.IsFalse(
          segment.Overlaps(line, precision),
          "intersect forrreal (from p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      line = Line3D.FromDirection(p0 + 2 * u_perp, u);
      Assert.IsFalse(segment.Overlaps(line, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + line.ToWkt(precision));

      //      downwards
      line = Line3D.FromDirection(p0 - 2 * u_perp, -u);
      Assert.IsFalse(segment.Overlaps(line, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + line.ToWkt(precision));

      // case 3: overlap
      //      just one point in the the first extremity, going down
      line = Line3D.FromDirection(p0, u);
      Assert.IsTrue(segment.Overlaps(line, precision),
                    "overlap (from p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));

      //      just one point in the the first extremity, going up
      line = Line3D.FromDirection(p1, -u);
      Assert.IsTrue(segment.Overlaps(line, precision),
                    "overlap (to p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + line.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void LineSegmentToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // input
      (var segment, var p0, var p1) = RandomGenerator.MakeLineSegment3D(decimal_precision: precision);

      if (segment is null) {
        return;
      }

      var mid = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      var u = segment.ToLine().Direction;

      // temp data
      Ray3D ray;
      UnitVector3D u_perp =
          u.CrossProduct((u.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();

      // case 1: intersect forrreal
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Overlaps(ray, precision),
                     "intersect forrreal (mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the middle, going down
      ray = new Ray3D(mid, -u_perp);
      Assert.IsFalse(
          segment.Overlaps(ray, precision),
          "intersect forrreal (to mid)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the first extremity, going down
      ray = new Ray3D(p0, -u_perp);
      Assert.IsFalse(
          segment.Overlaps(ray, precision),
          "intersect forrreal (to p0)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      just one point in the the second extremity, going up
      ray = new Ray3D(p1, -u_perp);
      Assert.IsFalse(
          segment.Overlaps(ray, precision),
          "intersect forrreal (from p1)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 2: no intersection (parallel, shift random vector)
      //      upwards
      ray = new Ray3D(p0 + 2 * u_perp, u);
      Assert.IsFalse(segment.Overlaps(ray, precision),
                     "no intersection (parallel, shift upwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      //      downwards
      ray = new Ray3D(p0 - 2 * u_perp, -u);
      Assert.IsFalse(segment.Overlaps(ray, precision),
                     "no intersection (parallel, shift downwards random vector)" + "\ns1=" + segment.ToWkt(precision) +
                         ", s2=" + ray.ToWkt(precision));

      // case 3: no intersection (crossing but not visible by the ray)
      //      in the middle, crossing
      ray = new Ray3D(mid - 2 * u_perp, -u_perp);
      Assert.IsFalse(segment.Overlaps(ray, precision),
                     "not intersect (mid-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      in the middle, crossing
      ray = new Ray3D(mid + 2 * u_perp, u_perp);
      Assert.IsFalse(segment.Overlaps(ray, precision),
                     "not intersect (mid+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      // case 4: overlap

      //      in the middle
      ray = new Ray3D(mid, u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (mid+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      in the middle
      ray = new Ray3D(mid, -u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (mid-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      at the beginning
      ray = new Ray3D(p1, -u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (p1-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      at the end
      ray = new Ray3D(p0, u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (p0+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      at the beginning
      ray = new Ray3D(p1, u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (p1+)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));

      //      at the end
      ray = new Ray3D(p0, -u);
      Assert.IsTrue(segment.Overlaps(ray, precision),
                    "overlap (p0-)" + "\ns1=" + segment.ToWkt(precision) + ", s2=" + ray.ToWkt(precision));
    }

    // Plane with other basic primitives
    [RepeatedTestMethod(100)]
    public void PlaneToLine() {
      int precision = RandomGenerator.MakeInt(0, 9);

      (var plane, var p0, var p1, var p2) = RandomGenerator.MakePlane(decimal_precision: precision);
      if (plane is null) {
        return;
      }

      // cache
      Line3D line;

      line = Line3D.FromDirection(plane.Origin, plane.AxisU);
      Assert.IsTrue(plane.Overlaps(line, precision));

      line = Line3D.FromDirection(plane.Origin, plane.AxisV);
      Assert.IsTrue(plane.Overlaps(line, precision));

      line = Line3D.FromPoints(p0, p1);
      Assert.IsTrue(plane.Overlaps(line, precision));

      line = Line3D.FromPoints(p0, p2);
      Assert.IsTrue(plane.Overlaps(line, precision));

      line = Line3D.FromPoints(p1, p2);
      Assert.IsTrue(plane.Overlaps(line, precision));

      (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();

      line = Line3D.FromPoints(
          new Point3D(a * p0.X + b * p1.X + c * p2.X, a * p0.Y + b * p1.Y + c * p2.Y, a * p0.Z + b * p1.Z + c * p2.Z),
          p2);
      Assert.IsTrue(plane.Overlaps(line, precision));
    }

    [RepeatedTestMethod(100)]
    public void PlaneToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      (var plane, var p0, var p1, var p2) = RandomGenerator.MakePlane(decimal_precision: precision);
      if (plane is null) {
        return;
      }

      // cache
      Ray3D ray;

      ray = new Ray3D(plane.Origin, plane.AxisU);
      Assert.IsTrue(plane.Overlaps(ray, precision));

      ray = new Ray3D(plane.Origin, plane.AxisV);
      Assert.IsTrue(plane.Overlaps(ray, precision));

      ray = new Ray3D(p0, (p1 - p0).Normalize());
      Assert.IsTrue(plane.Overlaps(ray, precision));

      ray = new Ray3D(p0, (p2 - p0).Normalize());
      Assert.IsTrue(plane.Overlaps(ray, precision));

      ray = new Ray3D(p1, (p2 - p1).Normalize());
      Assert.IsTrue(plane.Overlaps(ray, precision));

      (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();

      ray = new Ray3D(
          p2,
          (new Point3D(a * p0.X + b * p1.X + c * p2.X, a * p0.Y + b * p1.Y + c * p2.Y, a * p0.Z + b * p1.Z + c * p2.Z) -
           p2)
              .Normalize());
      Assert.IsTrue(plane.Overlaps(ray, precision));
    }

    [RepeatedTestMethod(100)]
    public void PlaneToLineSegment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      (var plane, var p0, var p1, var p2) = RandomGenerator.MakePlane(decimal_precision: precision);
      if (plane is null) {
        return;
      }

      // cache
      LineSegment3D seg;

      seg = LineSegment3D.FromPoints(plane.Origin, plane.Origin + plane.AxisU, precision);
      Assert.IsTrue(plane.Overlaps(seg, precision));

      seg = LineSegment3D.FromPoints(plane.Origin, plane.Origin + plane.AxisV, precision);
      Assert.IsTrue(plane.Overlaps(seg, precision));

      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsTrue(plane.Overlaps(seg, precision));

      seg = LineSegment3D.FromPoints(p0, p2, precision);
      Assert.IsTrue(plane.Overlaps(seg, precision));

      seg = LineSegment3D.FromPoints(p1, p2, precision);
      Assert.IsTrue(plane.Overlaps(seg, precision));

      (double a, double b, double c) = RandomGenerator.MakeLinearCombo3SumTo1();

      seg = LineSegment3D.FromPoints(
          new Point3D(a * p0.X + b * p1.X + c * p2.X, a * p0.Y + b * p1.Y + c * p2.Y, a * p0.Z + b * p1.Z + c * p2.Z),
          p2,
          precision);
      Assert.IsTrue(plane.Overlaps(seg, precision));
    }

    // Triangle with other basic primitives

    [RepeatedTestMethod(100)]
    public void TriangleToPlane() {
      int precision = RandomGenerator.MakeInt(0, 9);

      var random_triangle = RandomGenerator.MakeTriangle3D(decimal_precision: precision);
      var t = random_triangle.Triangle;

      if (t is null) {
        return;
      }
      // cache varibles
      Point3D cm = t.CenterOfMass();
      UnitVector3D n = t.RefPlane().Normal;
      UnitVector3D n_perp =
          n.CrossProduct((n.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();
      Plane plane;

      // no intersection (same plane)
      plane = t.RefPlane();
      Assert.IsTrue(t.Overlaps(plane, precision), "no intersection (same plane)");

      // intersection (crosses)
      plane = Plane.FromPointAndNormal(cm, n_perp);
      Assert.IsFalse(t.Overlaps(plane, precision), "intersection (crosses)");
    }

    [RepeatedTestMethod(100)]
    public void TriangleToLine() {
      int precision = RandomGenerator.MakeInt(0, 9);

      (var t, var p0, var p1, var p2) = RandomGenerator.MakeTriangle3D(decimal_precision: precision);
      if (t is null) {
        return;
      }

      // cache
      Line3D line;
      UnitVector3D u, u_perp;
      Point3D p;

      // overlap (crosses in 2D)
      p = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      u = (p2 - p).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsTrue(t.Overlaps(line, precision));

      p = Point3D.FromVector((p1.ToVector() + p2.ToVector()) / 2.0);
      u = (p0 - p).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsTrue(t.Overlaps(line, precision));

      p = Point3D.FromVector((p2.ToVector() + p0.ToVector()) / 2.0);
      u = (p1 - p).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsTrue(t.Overlaps(line, precision));

      // overlap (same plane, touches an edge)
      // not possible for a line

      // overlap (same plane, touches a vertex)
      p = p2;
      u = (p1 - p0).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsTrue(t.Overlaps(line, precision));

      p = p0;
      u = (p2 - p1).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsTrue(t.Overlaps(line, precision));

      p = p1;
      u = (p0 - p2).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsTrue(t.Overlaps(line, precision));

      // overlap (same plane, overlaps an edge)
      p = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      u = (p1 - p0).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsTrue(t.Overlaps(line, precision));

      p = Point3D.FromVector((p1.ToVector() + p2.ToVector()) / 2.0);
      u = (p2 - p1).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsTrue(t.Overlaps(line, precision));

      p = Point3D.FromVector((p2.ToVector() + p0.ToVector()) / 2.0);
      u = (p0 - p2).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsTrue(t.Overlaps(line, precision));

      // crosses (no overlap)
      u = (p2 - p).Normalize();
      p = t.CenterOfMass();
      u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();
      line = Line3D.FromDirection(p, u_perp);
      Assert.IsFalse(t.Overlaps(line, precision));

      // same plane, no overlap (parallel to an edge)
      p = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      u = (p2 - p).Normalize();
      p -= 2 * u;
      u = (p0 - p1).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsFalse(t.Overlaps(line, precision));

      p = Point3D.FromVector((p1.ToVector() + p2.ToVector()) / 2.0);
      u = (p0 - p).Normalize();
      p -= 2 * u;
      u = (p2 - p1).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsFalse(t.Overlaps(line, precision));

      p = Point3D.FromVector((p0.ToVector() + p2.ToVector()) / 2.0);
      u = (p1 - p).Normalize();
      p -= 2 * u;
      u = (p0 - p2).Normalize();
      line = Line3D.FromDirection(p, u);
      Assert.IsFalse(t.Overlaps(line, precision));
    }

    [RepeatedTestMethod(100)]
    public void TriangleToRay() {
      int precision = RandomGenerator.MakeInt(0, 9);

      (var t, var p0, var p1, var p2) = RandomGenerator.MakeTriangle3D(decimal_precision: precision);
      if (t is null) {
        return;
      }

      // cache
      Ray3D ray;
      UnitVector3D u, u_perp;
      Point3D p;

      // overlap(crosses in 2D)
      p = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      u = (p2 - p).Normalize();
      p -= 2 * u;
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      p = Point3D.FromVector((p1.ToVector() + p2.ToVector()) / 2.0);
      u = (p0 - p).Normalize();
      p -= 2 * u;
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      p = Point3D.FromVector((p2.ToVector() + p0.ToVector()) / 2.0);
      u = (p1 - p).Normalize();
      p -= 2 * u;
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      // overlap (same plane, touches an edge)
      p = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      u = -(p2 - p).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      p = Point3D.FromVector((p1.ToVector() + p2.ToVector()) / 2.0);
      u = -(p0 - p).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      p = Point3D.FromVector((p2.ToVector() + p0.ToVector()) / 2.0);
      u = -(p1 - p).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      // overlap (same plane, touches a vertex)
      p = p2;
      u = (p1 - p0).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      p = p0;
      u = (p1 - p2).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      p = p1;
      u = (p2 - p0).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      // overlap (same plane, overlaps an edge)
      p = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      u = (p1 - p0).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      p = Point3D.FromVector((p1.ToVector() + p2.ToVector()) / 2.0);
      u = (p2 - p1).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      p = Point3D.FromVector((p2.ToVector() + p0.ToVector()) / 2.0);
      u = (p0 - p2).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsTrue(t.Overlaps(ray, precision));

      // crosses (no overlap)
      u = (p2 - p).Normalize();
      p = t.CenterOfMass();
      u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();
      p -= u_perp * 2;
      ray = new Ray3D(p, u_perp);
      Assert.IsFalse(t.Overlaps(ray, precision));

      // same plane, no overlap (parallel to an edge)
      p = Point3D.FromVector((p0.ToVector() + p1.ToVector()) / 2.0);
      u = (p2 - p).Normalize();
      p -= 2 * u;
      u = (p0 - p1).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsFalse(t.Overlaps(ray, precision));

      p = Point3D.FromVector((p1.ToVector() + p2.ToVector()) / 2.0);
      u = (p0 - p).Normalize();
      p -= 2 * u;
      u = (p2 - p1).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsFalse(t.Overlaps(ray, precision));

      p = Point3D.FromVector((p0.ToVector() + p2.ToVector()) / 2.0);
      u = (p1 - p).Normalize();
      p -= 2 * u;
      u = (p0 - p2).Normalize();
      ray = new Ray3D(p, u);
      Assert.IsFalse(t.Overlaps(ray, precision));
    }

    [RepeatedTestMethod(100)]
    public void TriangleToLineSegment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      (var t, var tp0, var tp1, var tp2) = RandomGenerator.MakeTriangle3D(decimal_precision: precision);
      if (t is null) {
        return;
      }

      // cache
      LineSegment3D seg;
      UnitVector3D u, u_perp;
      Point3D p0, p1;

      // overlap (crosses in 2D)
      p0 = Point3D.FromVector((tp0.ToVector() + tp1.ToVector()) / 2.0);
      u = (tp2 - p0).Normalize();
      p0 -= u;
      p1 = p0 + 2 * u;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      (string inter_type, bool p0_in, bool p1_in, bool p_in) = ("", false, false, false);
      {
        var ref_plane = t.RefPlane();
        var triangle = t;
        var segment = seg;
        // from 3D to 2D to compute the intersection points
        var triangle_2D = Triangle2D.FromPoints(ref_plane.ProjectInto(triangle.P0),
                                                ref_plane.ProjectInto(triangle.P1),
                                                ref_plane.ProjectInto(triangle.P2),
                                                precision);

        var segment_2D =
            LineSegment2D.FromPoints(ref_plane.ProjectInto(segment.P0), ref_plane.ProjectInto(segment.P1), precision);

        var inter_res = segment_2D.ToLine().Intersection(triangle_2D, precision);
        if (inter_res.ValueType == typeof(NullValue)) {
          inter_type = "NullValue";
        } else if (inter_res.ValueType == typeof(Point2D)) {
          inter_type = "Point2D";
          if (segment_2D.Contains((Point2D)inter_res.Value, precision)) {
            p_in = true;
          }
        } else if (inter_res.ValueType == typeof(LineSegment2D)) {
          var inter_seg = (LineSegment2D)inter_res.Value;
          (p0_in, p1_in) = (segment_2D.Contains(inter_seg.P0, precision), segment_2D.Contains(inter_seg.P1, precision));
        }
      }
      Assert.IsTrue(t.Overlaps(seg, precision),
                    "\n\tinter_type=" + inter_type + "\n\tp0_in=" + p0_in + "\n\tp1_in=" + p1_in + "\n\tp_in=" + p_in);

      p0 = Point3D.FromVector((tp1.ToVector() + tp2.ToVector()) / 2.0);
      u = (tp0 - p0).Normalize();
      p0 -= u;
      p1 = p0 + 2 * u;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsTrue(t.Overlaps(seg, precision));

      p0 = Point3D.FromVector((tp2.ToVector() + tp0.ToVector()) / 2.0);
      u = (tp1 - p0).Normalize();
      p0 -= u;
      p1 = p0 + 2 * u;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsTrue(t.Overlaps(seg, precision));

      //// overlap (same plane, touches an edge)
      // p0 = Point3D.FromVector((tp0.ToVector() + tp1.ToVector()) / 2.0);
      // u = (tp2 - p0).Normalize();
      // p1 = p0 - 2 * u;
      // seg = LineSegment3D.FromPoints(p0, p1);
      //(inter_type, p0_in, p1_in, p_in) = ("", false, false, false);
      //{
      //   var ref_plane = t.RefPlane();
      //   var triangle = t;
      //   var segment = seg;
      //   int decimal_precision = 3;
      //   // from 3D to 2D to compute the intersection points
      //   var triangle_2D = Triangle2D.FromPoints(ref_plane.ProjectInto(triangle.P0),
      //                                           ref_plane.ProjectInto(triangle.P1),
      //                                           ref_plane.ProjectInto(triangle.P2));

      //  var segment_2D = LineSegment2D.FromPoints(ref_plane.ProjectInto(segment.P0),
      //  ref_plane.ProjectInto(segment.P1));

      //  var inter_res = segment_2D.ToLine().Intersection(triangle_2D, decimal_precision);
      //  if (inter_res.ValueType == typeof(NullValue)) {
      //    inter_type = "NullValue";
      //  } else if (inter_res.ValueType == typeof(Point2D)) {
      //    inter_type = "Point2D";
      //    if (segment_2D.Contains((Point2D)inter_res.Value, decimal_precision)) {
      //      p_in = true;
      //    }
      //  } else if (inter_res.ValueType == typeof(LineSegment2D)) {
      //    var inter_seg = (LineSegment2D)inter_res.Value;
      //    (p0_in, p1_in) = (segment_2D.Contains(inter_seg.P0, decimal_precision),
      //                      segment_2D.Contains(inter_seg.P1, decimal_precision));
      //  }
      //}
      // Assert.IsTrue(t.Overlaps(seg, precision),
      //              "\n\tinter_type=" + inter_type + "\n\tp0_in=" + p0_in + "\n\tp1_in=" + p1_in + "\n\tp_in=" +
      //              p_in);

      // p0 = Point3D.FromVector((tp1.ToVector() + tp2.ToVector()) / 2.0);
      // u = (tp0 - p0).Normalize();
      // p1 = p0 - 2 * u;
      // seg = LineSegment3D.FromPoints(p0, p1);
      // Assert.IsTrue(t.Overlaps(seg, precision));

      // p0 = Point3D.FromVector((tp2.ToVector() + tp0.ToVector()) / 2.0);
      // u = (tp1 - p0).Normalize();
      // p1 = p0 - 2 * u;
      // seg = LineSegment3D.FromPoints(p0, p1);
      // Assert.IsTrue(t.Overlaps(seg, precision));

      // overlap (same plane, touches a vertex)
      u = (tp1 - tp0).Normalize();
      p0 = tp2 - 2 * u;
      p1 = tp2 + 2 * u;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsTrue(t.Overlaps(seg, precision));

      u = (tp1 - tp2).Normalize();
      p0 = tp0 - 2 * u;
      p1 = tp0 + 2 * u;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsTrue(t.Overlaps(seg, precision));

      u = (tp2 - tp0).Normalize();
      p0 = tp1 - 2 * u;
      p1 = tp1 + 2 * u;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsTrue(t.Overlaps(seg, precision));

      // overlap (same plane, overlaps an edge)
      p0 = Point3D.FromVector((tp0.ToVector() + tp1.ToVector()) / 2.0);
      u = (tp1 - tp0).Normalize();
      p1 = p0 + 2 * u;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsTrue(t.Overlaps(seg, precision));

      p0 = Point3D.FromVector((tp1.ToVector() + tp2.ToVector()) / 2.0);
      u = (tp2 - tp1).Normalize();
      p1 = p0 + 2 * u;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsTrue(t.Overlaps(seg, precision));

      p0 = Point3D.FromVector((tp2.ToVector() + tp0.ToVector()) / 2.0);
      u = (tp0 - tp2).Normalize();
      p1 = p0 + 2 * u;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsTrue(t.Overlaps(seg, precision));

      // crosses (no overlap)
      u = (tp2 - p0).Normalize();
      p0 = t.CenterOfMass();
      u_perp = u.CrossProduct((u.IsParallel(Vector3D.AxisZ, precision) ? Vector3D.AxisY : Vector3D.AxisZ)).Normalize();
      p0 -= u_perp * 2;
      p1 = p0 + 4 * u_perp;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsFalse(t.Overlaps(seg, precision));

      // same plane, no overlap (parallel to an edge)
      p0 = Point3D.FromVector((tp0.ToVector() + tp1.ToVector()) / 2.0);
      u = (tp2 - p0).Normalize();
      p0 -= 2 * u;
      u = (tp0 - tp1).Normalize();
      p1 = p0 + u * 4;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsFalse(t.Overlaps(seg, precision));

      p0 = Point3D.FromVector((tp1.ToVector() + tp2.ToVector()) / 2.0);
      u = (tp0 - p0).Normalize();
      p0 -= 2 * u;
      u = (tp2 - tp1).Normalize();
      p1 = p0 + u * 4;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsFalse(t.Overlaps(seg, precision));

      p0 = Point3D.FromVector((tp0.ToVector() + tp2.ToVector()) / 2.0);
      u = (tp1 - p0).Normalize();
      p0 -= 2 * u;
      u = (tp0 - tp2).Normalize();
      p1 = p0 + u * 4;
      seg = LineSegment3D.FromPoints(p0, p1, precision);
      Assert.IsFalse(t.Overlaps(seg, precision));
    }
  }
}
