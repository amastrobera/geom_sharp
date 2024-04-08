// internal
using GeomSharp;

// external
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GeomSharpTests {

  [TestClass]
  public class Polyline2DTests {
    [RepeatedTestMethod(100)]
    public void Containment() {
      // 2D
      (var pline, var pline_direction, int n) = RandomGenerator.MakeSimplePolyline2D();

      // Console.WriteLine("t = " + t.ToWkt());
      if (pline is null) {
        return;
      }

      // temporary data
      Point2D p;

      // test 1: segments' vertices are contained
      for (int i = 0; i < n; ++i) {
        p = pline[i];
        Assert.IsTrue(pline.Contains(p),
                      "segments' vertices are contained (vertex " + i.ToString() + "), \n\tpline=" + pline.ToWkt() +
                          "\n\tp=" + p.ToWkt());
      }

      // test 2: segments' midpoints are contained
      for (int i = 0; i < n - 1; ++i) {
        p = Point2D.FromVector((pline[i].ToVector() + pline[i + 1].ToVector()) / 2);
        Assert.IsTrue(pline.Contains(p),
                      "segments' midpoints are contained (vertex " + i.ToString() + "), \n\tpline=" + pline.ToWkt() +
                          "\n\tp=" + p.ToWkt());
      }

      // test 3-4: segments' midpoints shifted up (or down) are not contained
      for (int i = 0; i < n - 1; ++i) {
        var mid = Point2D.FromVector((pline[i].ToVector() + pline[i + 1].ToVector()) / 2);

        p = mid + Vector2D.AxisV * 2;
        Assert.IsFalse(pline.Contains(p),
                       "segments' midpoints shifted up are not contained (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt() + "\n\tp=" + p.ToWkt());

        p = mid - Vector2D.AxisV * 2;
        Assert.IsFalse(pline.Contains(p),
                       "segments' midpoints shifted down are not contained (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt() + "\n\tp=" + p.ToWkt());
      }
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      // 2D
      (var pline, var pline_direction, int n) = RandomGenerator.MakeSimplePolyline2D();

      // Console.WriteLine("t = " + t.ToWkt());
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
      Assert.IsTrue(
          pline.Intersects(line),
          "line following the same direction intersects, \n\tpline=" + pline.ToWkt() + "\n\tp=" + line.ToWkt());

      // test 1-2-3: line/ray/segment striking through the polyline's vertices intersect
      for (int i = 0; i < n - 1; ++i) {
        var mid = Point2D.FromVector((pline[i].ToVector() + pline[i + 1].ToVector()) / 2);
        (p1, p2) = (mid + dir_perp * 2, mid - dir_perp * 2);

        line = Line2D.FromPoints(p1, p2);
        Assert.IsTrue(pline.Intersects(line),
                      "line striking through the polyline's vertices intersect (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt() + "\n\tline=" + line.ToWkt());

        ray = new Ray2D(p1, (p2 - p1).Normalize());
        Assert.IsTrue(pline.Intersects(ray),
                      "ray striking through the polyline's vertices intersect (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt() + "\n\tray=" + ray.ToWkt());

        seg = LineSegment2D.FromPoints(p1, p2);
        Assert.IsTrue(pline.Intersects(seg),
                      "segment striking through the polyline's vertices intersect (vertex " + i.ToString() +
                          "), \n\tpline=" + pline.ToWkt() + "\n\tseg=" + seg.ToWkt());
      }

      // test 4-5: segment parallel to the polyline's vertices do not intersect
      for (int i = 0; i < n - 1; ++i) {
        (p1, p2) = (pline[i] + dir_perp * 2, pline[i + 1] + dir_perp * 2);
        seg = LineSegment2D.FromPoints(p1, p2);
        Assert.IsFalse(pline.Intersects(seg),
                       "segment parallel to the polyline's vertices do not intersect (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt() + "\n\tseg=" + seg.ToWkt());

        (p1, p2) = (pline[i] - dir_perp * 2, pline[i + 1] - dir_perp * 2);
        seg = LineSegment2D.FromPoints(p1, p2);
        Assert.IsFalse(pline.Intersects(seg),
                       "segment parallel to the polyline's vertices do not intersect (vertex " + i.ToString() +
                           "), \n\tpline=" + pline.ToWkt() + "\n\tseg=" + seg.ToWkt());
      }
    }

    [RepeatedTestMethod(100)]
    public void GetPoint() {
      // 2D
      (var pline, var pline_direction, int n) = RandomGenerator.MakeSimplePolyline2D();

      // Console.WriteLine("t = " + t.ToWkt());
      if (pline is null) {
        return;
      }

      foreach (int decimal_precision in new List<int> { 0, 1, 2, Constants.THREE_DECIMALS, Constants.NINE_DECIMALS }) {
        // test location pct
        var loc_pct = pline.Select(p => pline.GetPct(p, decimal_precision)).ToList();
        Assert.AreEqual(pline.Size, loc_pct.Count, "polyline size != location points size");
        Assert.IsTrue(Math.Round(loc_pct.First(), decimal_precision) == 0, $"pct (0) is not 0%");
        for (int i = 0; i < loc_pct.Count - 1; ++i) {
          Assert.IsTrue(Math.Round(loc_pct[i] - loc_pct[i + 1], decimal_precision) < 0,
                        $"pct {i} is not smaller than pct {i + 1}");
        }
        Assert.IsTrue(Math.Round(loc_pct.Last(), decimal_precision) == 1, $"pct (n) is not 100%");

        // test equality with original points
        var pts_on_pct_loc = loc_pct.Select(pct => pline.GetPoint(pct, decimal_precision)).ToList();
        Assert.AreEqual(pline.Size, pts_on_pct_loc.Count, "polyline size != pts_on_pct_loc points size");
        for (int i = 0; i < pts_on_pct_loc.Count; ++i) {
          Assert.IsTrue(
              pline[i].AlmostEquals(pts_on_pct_loc[i], decimal_precision),
              $"pline point {i} -> {pline[i].ToWkt(decimal_precision)} is not equal to pont on polyline than pct {Math.Round(100 * loc_pct[i], decimal_precision)}%, {pts_on_pct_loc[i].ToWkt(decimal_precision)}");
        }
      }
    }
  }
}
