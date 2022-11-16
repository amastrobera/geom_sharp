using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomSharp {

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

    public static List<Point3D> RemoveDuplicates(this List<Point3D> point_list,
                                                 int decimal_precision = Constants.THREE_DECIMALS) {
      if (point_list.Count == 0) {
        return new List<Point3D>();
      }

      var key_dictionary = new Dictionary<string, Point3D>();
      foreach (var p in point_list) {
        string key = p.ToWkt(decimal_precision);
        if (!key_dictionary.ContainsKey(key)) {
          key_dictionary.Add(key, p);
        }
      }

      return key_dictionary.Select(pr => pr.Value).ToList();
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

    public static List<Point2D> RemoveDuplicates(this List<Point2D> point_list,
                                                 int decimal_precision = Constants.THREE_DECIMALS) {
      if (point_list.Count == 0) {
        return new List<Point2D>();
      }

      var key_dictionary = new Dictionary<string, Point2D>();
      foreach (var p in point_list) {
        string key = p.ToWkt(decimal_precision);
        if (!key_dictionary.ContainsKey(key)) {
          key_dictionary.Add(key, p);
        }
      }
      return key_dictionary.Select(pr => pr.Value).ToList();
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

    // projections
    public static Point2D ToXY(this Point3D v) => new Point2D(v.X, v.Y);
    public static Point2D ToYZ(this Point3D v) => new Point2D(v.Y, v.Z);
    public static Point2D ToZX(this Point3D v) => new Point2D(v.Z, v.X);

    public static Vector2D ToXY(this Vector3D v) => new Vector2D(v.X, v.Y);
    public static Vector2D ToYZ(this Vector3D v) => new Vector2D(v.Y, v.Z);
    public static Vector2D ToZX(this Vector3D v) => new Vector2D(v.Z, v.X);

    public static ProjectionResult ToXY(this Line3D v) {
      var proj_orig = v.Origin.ToXY();
      var proj_dir = v.Direction.ToXY();
      if (Math.Round(proj_dir.Length(), Constants.NINE_DECIMALS) == 0) {
        // it's a point!
        return new ProjectionResult(new Point2D(proj_orig));
      }
      return new ProjectionResult(Line2D.FromDirection(proj_orig, proj_dir.Normalize()));
    }
    public static ProjectionResult ToYZ(this Line3D v) {
      var proj_orig = v.Origin.ToZX();
      var proj_dir = v.Direction.ToZX();
      if (Math.Round(proj_dir.Length(), Constants.NINE_DECIMALS) == 0) {
        // it's a point!
        return new ProjectionResult(new Point2D(proj_orig));
      }
      return new ProjectionResult(Line2D.FromDirection(proj_orig, proj_dir.Normalize()));
    }
    public static ProjectionResult ToZX(this Line3D v) {
      var proj_orig = v.Origin.ToZX();
      var proj_dir = v.Direction.ToZX();
      if (Math.Round(proj_dir.Length(), Constants.NINE_DECIMALS) == 0) {
        // it's a point!
        return new ProjectionResult(new Point2D(proj_orig));
      }
      return new ProjectionResult(Line2D.FromDirection(proj_orig, proj_dir.Normalize()));
    }

    public static ProjectionResult ToXY(this LineSegment3D v) {
      var proj_p0 = v.P0.ToXY();
      var proj_p1 = v.P1.ToXY();
      try {
        var proj_line = Line2D.FromPoints(proj_p0, proj_p1);
        return new ProjectionResult(proj_line);
      } catch (NullLengthException) {
        return new ProjectionResult(new Point2D(proj_p0));
      }
    }
    public static ProjectionResult ToYZ(this LineSegment3D v) {
      var proj_p0 = v.P0.ToYZ();
      var proj_p1 = v.P1.ToYZ();
      try {
        var proj_line = Line2D.FromPoints(proj_p0, proj_p1);
        return new ProjectionResult(proj_line);
      } catch (NullLengthException) {
        return new ProjectionResult(new Point2D(proj_p0));
      }
    }
    public static ProjectionResult ToZX(this LineSegment3D v) {
      var proj_p0 = v.P0.ToZX();
      var proj_p1 = v.P1.ToZX();
      try {
        var proj_line = Line2D.FromPoints(proj_p0, proj_p1);
        return new ProjectionResult(proj_line);
      } catch (NullLengthException) {
        return new ProjectionResult(new Point2D(proj_p0));
      }
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
