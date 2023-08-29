using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp.Collections {

  public static class Extensions {
    /// <summary>
    /// Purely a helper function for other routines. Calling (publicly) a sum of points makes no sense
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private static Point2D Sum(this List<Point2D> points) {
      if (points.Count == 0) {
        return Point2D.Zero;
      }

      (double u, double v) = (0, 0);
      foreach (var p in points) {
        u += p.U;
        v += p.V;
      }
      return new Point2D(u, v);
    }

    /// <summary>
    /// Purely a helper function for other routines. Calling (publicly) a sum of points makes no sense
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private static Point3D Sum(this List<Point3D> points) {
      if (points.Count == 0) {
        return Point3D.Zero;
      }

      (double x, double y, double z) = (0, 0, 0);
      foreach (var p in points) {
        x += p.X;
        y += p.Y;
        z += p.Z;
      }
      return new Point3D(x, y, z);
    }

    /// <summary>
    /// Average point in a list
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static Point2D Average(this List<Point2D> points) {
      return points.Sum() / points.Count;
    }

    /// <summary>
    /// Average point in a list
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static Point3D Average(this List<Point3D> points) {
      return points.Sum() / points.Count;
    }

    public static Point2D Centroid(this List<Point2D> points) {
      int n = points.Count;
      if (n == 0) {
        return Point2D.Zero;
      }

      if (n == 1) {
        return points[0];
      }

      if (n == 2) {
        return points.Average();
      }

      double signed_area = 0;

      (double cx, double cy) = (0, 0);

      for (int i = 0; i < n; i++) {
        int j = (i + 1) % n;

        double signed_area_piece = points[i].U * points[j].V - points[i].V * points[j].U;
        signed_area += signed_area_piece;

        cx += (points[i].U + points[j].U) * signed_area_piece;
        cy += (points[i].V + points[j].V) * signed_area_piece;
      }

      if (Math.Round(signed_area, Constants.THREE_DECIMALS) != 0) {
        cx /= 6 * signed_area;
        cy /= 6 * signed_area;

        return new Point2D(cx, cy);
      }

      return points.Average();  // fallback to simple average in case the areas of the polygons eliminate each other
    }

    public static List<Point3D> RemoveDuplicates(this List<Point3D> point_list,
                                                 int decimal_precision = Constants.THREE_DECIMALS) {
      if (point_list.Count == 0) {
        return new List<Point3D>();
      }

      var key_dictionary = new Dictionary<string, int>();
      for (int i = 0; i < point_list.Count; ++i) {
        string key = point_list[i].ToWkt(decimal_precision);
        if (!key_dictionary.ContainsKey(key)) {
          key_dictionary.Add(key, i);
        }
      }

      return key_dictionary.OrderBy(kv => kv.Value).Select(kv => point_list[kv.Value]).ToList();
    }

    public static bool AreCollinear(this Point3D p1,
                                    Point3D p2,
                                    Point3D p3,
                                    int decimal_precision = Constants.THREE_DECIMALS) {
      // check if p2 is on the same line p1->p3, and if so remove it
      if (p1.AlmostEquals(p2, decimal_precision) || p2.AlmostEquals(p3, decimal_precision)) {
        return true;
      }
      // check if the two lines are parallel
      var U = p2 - p1;
      var V = p3 - p1;
      if (U.IsParallel(V, decimal_precision)) {
        return true;
      }

      return false;
    }
    /// <summary>
    /// /// Remove all points that are on the same line, to build the minimum polyline
    /// </summary>
    /// <param name="polyline"></param>
    /// <param name="decimal_precision"></param>
    /// <returns>true if at least one point has been removed </returns>
    public static List<Point3D> RemoveCollinearPoints(this List<Point3D> polyline,
                                                      int decimal_precision = Constants.THREE_DECIMALS) {
      int n = polyline.Count;

      if (n == 2 && polyline[0].AlmostEquals(polyline[1], decimal_precision)) {
        // warning returning empty polyline, only collinear points found
        return new List<Point3D>();
      }

      if (n < 3) {
        return polyline;
        // throw new ArgumentException("RemoveCollinearPoints called with a list of less than 2 points");
      }

      var new_polyline = new List<Point3D>(polyline);

      // remove collinear points
      for (int i = 0; i < n; ++i) {
        int i1 = i % n;
        int i2 = (i1 + 1) % n;
        int i3 = (i2 + 1) % n;
        if (n < 3) {
          break;
        }

        // remove collinear points
        if (new_polyline[i1].AreCollinear(new_polyline[i2], new_polyline[i3], decimal_precision)) {
          // find and remove the point in the middle (extend the edge to the next point)
          if (Math.Round(new_polyline[i1].DistanceTo(new_polyline[i3]) - new_polyline[i1].DistanceTo(new_polyline[i2]),
                         decimal_precision) >= 0) {
            new_polyline.RemoveAt(i2);
          } else {
            new_polyline.RemoveAt(i3);
          }
          --n;  // the size of items has decreased
          --i;  // analyze again the same start point in the next iteration
        }
      }

      if (n == 2 && new_polyline[0].AlmostEquals(new_polyline[1], decimal_precision)) {
        // warning returning empty polyline, only collinear points found
        new_polyline.Clear();
      }

      return new_polyline;
    }

    public static List<Point2D> RemoveDuplicates(this List<Point2D> point_list,
                                                 int decimal_precision = Constants.THREE_DECIMALS) {
      if (point_list.Count == 0) {
        return new List<Point2D>();
      }

      var key_dictionary = new Dictionary<string, int>();
      for (int i = 0; i < point_list.Count; ++i) {
        string key = point_list[i].ToWkt(decimal_precision);
        if (!key_dictionary.ContainsKey(key)) {
          key_dictionary.Add(key, i);
        }
      }

      return key_dictionary.OrderBy(kv => kv.Value).Select(kv => point_list[kv.Value]).ToList();
    }

    /// <summary>
    /// /// Remove all points that are on the same line, to build the minimum polyline
    /// </summary>
    /// <param name="polyline"></param>
    /// <param name="decimal_precision"></param>
    /// <returns>true if at least one point has been removed </returns>
    public static List<Point2D> RemoveCollinearPoints(this List<Point2D> polyline,
                                                      int decimal_precision = Constants.THREE_DECIMALS) {
      int n = polyline.Count;
      if (n == 2 && polyline[0].AlmostEquals(polyline[1], decimal_precision)) {
        // warning returning empty polyline, only collinear points found
        return new List<Point2D>();
      }

      if (n < 3) {
        return polyline;
      }

      var new_polyline = new List<Point2D>(polyline);
      // remove collinear points
      for (int i = 0; i < n; ++i) {
        int i1 = i % n;
        int i2 = (i1 + 1) % n;
        int i3 = (i2 + 1) % n;
        if (n < 3) {
          break;
        }

        // remove equal points
        // check if p2 is on the same line p1->p3, and if so remove it
        if (new_polyline[i3].AlmostEquals(new_polyline[i2], decimal_precision) ||
            new_polyline[i2].AlmostEquals(new_polyline[i1], decimal_precision)) {
          new_polyline.RemoveAt(i2);
          --n;  // the size of items has decreased
          --i;  // analyze again the same start point in the next iteration
        } else {
          // check if p2 is on the same line p1->p3, and if so remove it
          if ((new_polyline[i3] - new_polyline[i1]).IsParallel(new_polyline[i2] - new_polyline[i1])) {
            // find and remove the point in the middle (extend the edge to the next point)
            if (Math.Round(
                    new_polyline[i1].DistanceTo(new_polyline[i3]) - new_polyline[i1].DistanceTo(new_polyline[i2]),
                    decimal_precision) >= 0) {
              new_polyline.RemoveAt(i2);
            } else {
              new_polyline.RemoveAt(i3);
            }
            --n;  // the size of items has decreased
            --i;  // analyze again the same start point in the next iteration
          }
        }
      }

      if (n == 2 && new_polyline[0].AlmostEquals(new_polyline[1], decimal_precision)) {
        // warning returning empty polyline, only collinear points found
        new_polyline.Clear();
      }

      return new_polyline;
    }

    // special sorting

    /// <summary>
    /// Sorts a list of points in counter clockwise order
    /// </summary>
    /// <param name="points"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public static List<Point2D> SortCCW(this List<Point2D> points, int decimal_precision = Constants.THREE_DECIMALS) {
      var centroid = points.Average();
      var u_axis = Vector2D.AxisU;
      var v_axis = Vector2D.AxisV;
      points.Sort((p1, p2) =>
                      ((p1 == p2) ? 0
                                  : (Math.Round((u_axis.AngleTo(p1 - centroid) - u_axis.AngleTo(p2 - centroid)).Radians,
                                                decimal_precision) < 0
                                         ? -1
                                         : 1)));

      return points;
    }

    /// <summary>
    /// Sorts a list of points in  clockwise order
    /// </summary>
    /// <param name="points"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public static List<Point2D> SortCW(this List<Point2D> points, int decimal_precision = Constants.THREE_DECIMALS) {
      var centroid = points.Average();
      var u_axis = Vector2D.AxisU;
      var v_axis = Vector2D.AxisV;
      points.Sort((p1, p2) => ((p1 == p2) ? 0
                               : Math.Round((u_axis.AngleTo(p1 - centroid) - u_axis.AngleTo(p2 - centroid)).Radians,
                                            decimal_precision) > 0
                                   ? -1
                                   : 1));
      return points;
    }

    // special formatting
    public static string ToString(this List<Point2D> plist) => "{" + string.Join(",", plist.Select(v => v.ToString())) +
                                                               "}";

    public static string ToWkt(this List<Point2D> plist, int precision = Constants.THREE_DECIMALS) =>
        (plist.Count == 0)
            ? "LINESTRING EMPTY"
            : "LINESTRING (" +
                  string.Join(",",
                              plist.Select(
                                  v => string.Format(
                                      String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"), v.U, v.V))) +

                  ")";

    public static string ToString(this List<Point3D> plist) => "{" + string.Join(",", plist.Select(v => v.ToString())) +
                                                               "}";

    public static string ToWkt(this List<Point3D> plist, int precision = Constants.THREE_DECIMALS) =>
        (plist.Count == 0)
            ? "LINESTRING EMPTY"
            : "LINESTRING (" +
                  string.Join(",",
                              plist.Select(v => string.Format(
                                               String.Format(
                                                   "{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                                               v.X,
                                               v.Y,
                                               v.Z))) +

                  ")";
  }
}
