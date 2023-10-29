// internal
using GeomSharp;

// external
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {

  [TestClass]
  public class Polyline3DTests {
    [RepeatedTestMethod(100)]
    public void Containment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // 3D

      (var pline, var pline_longitudinal, var pline_lateral, int n) =
          RandomGenerator.MakeSimplePolyline3D(decimal_precision: precision);

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (pline is null) {
        return;
      }

      // temporary data
      Point3D p;

      // test 1: segments' vertices are contained
      for (int i = 0; i < n; ++i) {
        p = pline[i];
        Assert.IsTrue(pline.Contains(p, precision),
                      "segments' vertices are contained (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }

      // test 2: segments' midpoints are contained
      for (int i = 0; i < n - 1; ++i) {
        p = Point3D.FromVector((pline[i].ToVector() + pline[i + 1].ToVector()) / 2);
        Assert.IsTrue(pline.Contains(p, precision),
                      "segments' midpoints are contained (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }

      // test 3-4: segments' midpoints shifted up (or down) are not contained
      for (int i = 0; i < n - 1; ++i) {
        var mid = Point3D.FromVector((pline[i].ToVector() + pline[i + 1].ToVector()) / 2);

        p = mid + pline_lateral * 2;
        Assert.IsFalse(pline.Contains(p, precision),
                       "segments' midpoints shifted up are not contained (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

        p = mid - pline_lateral * 2;
        Assert.IsFalse(pline.Contains(p, precision),
                       "segments' midpoints shifted down are not contained (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // 3D
      (var pline, var pline_longitudinal, var pline_lateral, int n) =
          RandomGenerator.MakeSimplePolyline3D(decimal_precision: precision);

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (pline is null) {
        return;
      }

      // temporary data
      Point3D p1, p2;
      UnitVector3D dir;
      UnitVector3D dir_perp = pline_lateral;
      Line3D line;
      LineSegment3D seg;
      Ray3D ray;

      // test 0: line following the same direction intersects
      p1 = pline[0];
      dir = pline_longitudinal;
      line = Line3D.FromDirection(p1, dir);
      Assert.IsTrue(pline.Intersects(line, precision),
                    "line following the same direction intersects, \n\tpline=" + pline.ToWkt(precision) +
                        "\n\tp=" + line.ToWkt(precision));

      // test 1-2-3: line/ray/segment striking through the polyline's vertices intersect
      for (int i = 0; i < n - 1; ++i) {
        var mid = Point3D.FromVector((pline[i].ToVector() + pline[i + 1].ToVector()) / 2);
        (p1, p2) = (mid + dir_perp * 2, mid - dir_perp * 2);

        line = Line3D.FromPoints(p1, p2, precision);
        Assert.IsTrue(pline.Intersects(line, precision),
                      "line striking through the polyline's vertices intersect (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt(precision) + "\n\tline=" + line.ToWkt(precision));

        ray = new Ray3D(p1, (p2 - p1).Normalize());
        Assert.IsTrue(pline.Intersects(ray, precision),
                      "ray striking through the polyline's vertices intersect (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt(precision) + "\n\tray=" + ray.ToWkt(precision));

        seg = LineSegment3D.FromPoints(p1, p2, precision);
        Assert.IsTrue(pline.Intersects(seg, precision),
                      "segment striking through the polyline's vertices intersect (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt(precision) + "\n\tseg=" + seg.ToWkt(precision));
      }

      // test 4-5: segment parallel to the polyline's vertices do not intersect
      for (int i = 0; i < n - 1; ++i) {
        (p1, p2) = (pline[i] + dir_perp * 2, pline[i + 1] + dir_perp * 2);
        seg = LineSegment3D.FromPoints(p1, p2, precision);
        Assert.IsFalse(pline.Intersects(seg, precision),
                       "segment parallel to the polyline's vertices do not intersect (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt(precision) + "\n\tseg=" + seg.ToWkt(precision));

        (p1, p2) = (pline[i] - dir_perp * 2, pline[i + 1] - dir_perp * 2);
        seg = LineSegment3D.FromPoints(p1, p2, precision);
        Assert.IsFalse(pline.Intersects(seg, precision),
                       "segment parallel to the polyline's vertices do not intersect (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt(precision) + "\n\tseg=" + seg.ToWkt(precision));
      }
    }

    [RepeatedTestMethod(100)]
    public void GetPointOnPolyline() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // 3D

      (var pline, var _, var _, int n) = RandomGenerator.MakeSimplePolyline3D(decimal_precision: precision);

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (pline is null) {
        return;
      }

      // temporary data
      Point3D p;

      // test 1: polyline points!
      for (int i = 0; i < n; ++i) {
        p = pline[i];

        double pct = pline.LocationPct(p, precision);
        var q = pline.GetPointOnPolyline(pct, precision);

        Assert.IsTrue(q.AlmostEquals(q, precision),
                      "point (" + i.ToString() + ") could not be found on polyline.\n\tpline=" +
                          pline.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }
    }
  }
}
