// internal
using GeomSharp;

// external
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {

  [TestClass]
  public class Polyline2DTests {
    [RepeatedTestMethod(100)]
    public void Containment() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // 2D
      (var pline, var pline_direction, int n) = RandomGenerator.MakeSimplePolyline2D(decimal_precision: precision);

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (pline is null) {
        return;
      }

      // temporary data
      Point2D p;

      // test 1: segments' vertices are contained
      for (int i = 0; i < n; ++i) {
        p = pline[i];
        Assert.IsTrue(pline.Contains(p, precision),
                      "segments' vertices are contained (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }

      // test 2: segments' midpoints are contained
      for (int i = 0; i < n - 1; ++i) {
        p = Point2D.FromVector((pline[i].ToVector() + pline[i + 1].ToVector()) / 2);
        Assert.IsTrue(pline.Contains(p, precision),
                      "segments' midpoints are contained (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }

      // test 3-4: segments' midpoints shifted up (or down) are not contained
      for (int i = 0; i < n - 1; ++i) {
        var mid = Point2D.FromVector((pline[i].ToVector() + pline[i + 1].ToVector()) / 2);

        p = mid + Vector2D.AxisV * 2;
        Assert.IsFalse(pline.Contains(p, precision),
                       "segments' midpoints shifted up are not contained (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

        p = mid - Vector2D.AxisV * 2;
        Assert.IsFalse(pline.Contains(p, precision),
                       "segments' midpoints shifted down are not contained (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
      }
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      int precision = RandomGenerator.MakeInt(0, 9);

      // 2D
      (var pline, var pline_direction, int n) = RandomGenerator.MakeSimplePolyline2D(decimal_precision: precision);

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (pline is null) {
        return;
      }

      // temporary data
      Point2D p1, p2;
      UnitVector2D dir;
      UnitVector2D dir_perp = pline_direction.Perp().Normalize();
      Line2D line;
      LineSegment2D seg;
      Ray2D ray;

      // test 0: line following the same direction intersects
      p1 = pline[0];
      dir = pline_direction;
      line = Line2D.FromDirection(p1, dir);
      Assert.IsTrue(pline.Intersects(line, precision),
                    "line following the same direction intersects, \n\tpline=" + pline.ToWkt(precision) +
                        "\n\tp=" + line.ToWkt(precision));

      // test 1-2-3: line/ray/segment striking through the polyline's vertices intersect
      for (int i = 0; i < n - 1; ++i) {
        var mid = Point2D.FromVector((pline[i].ToVector() + pline[i + 1].ToVector()) / 2);
        (p1, p2) = (mid + dir_perp * 2, mid - dir_perp * 2);

        line = Line2D.FromPoints(p1, p2, precision);
        Assert.IsTrue(pline.Intersects(line, precision),
                      "line striking through the polyline's vertices intersect (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt(precision) + "\n\tline=" + line.ToWkt(precision));

        ray = new Ray2D(p1, (p2 - p1).Normalize());
        Assert.IsTrue(pline.Intersects(ray),
                      "ray striking through the polyline's vertices intersect (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt(precision) + "\n\tray=" + ray.ToWkt(precision));

        seg = LineSegment2D.FromPoints(p1, p2, precision);
        Assert.IsTrue(pline.Intersects(seg),
                      "segment striking through the polyline's vertices intersect (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt(precision) + "\n\tseg=" + seg.ToWkt(precision));
      }

      // test 4-5: segment parallel to the polyline's vertices do not intersect
      for (int i = 0; i < n - 1; ++i) {
        (p1, p2) = (pline[i] + dir_perp * 2, pline[i + 1] + dir_perp * 2);
        seg = LineSegment2D.FromPoints(p1, p2, precision);
        Assert.IsFalse(pline.Intersects(seg),
                       "segment parallel to the polyline's vertices do not intersect (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt(precision) + "\n\tseg=" + seg.ToWkt(precision));

        (p1, p2) = (pline[i] - dir_perp * 2, pline[i + 1] - dir_perp * 2);
        seg = LineSegment2D.FromPoints(p1, p2, precision);
        Assert.IsFalse(pline.Intersects(seg),
                       "segment parallel to the polyline's vertices do not intersect (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt(precision) + "\n\tseg=" + seg.ToWkt(precision));
      }
    }
  }
}
