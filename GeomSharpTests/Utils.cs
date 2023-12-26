﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeomSharp;
using GeomSharp.Algebra;
using GeomSharp.Collections;
using GeomSharp.Transformation;
using GeomSharp.Utils;

namespace GeomSharpTests {

  class RandomGenerator {
    public static readonly Random Seed = new Random();

    public static int MakeInt(int IMin = 0, int IMax = 10) => Seed.Next(IMin, IMax);

    // 2D objects

    public static Point2D MakePoint2D(int IMin = -10,
                                      int IMax = 10,
                                      int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      return new Point2D(IMin + Seed.NextDouble() * (IMax - IMin), IMin + Seed.NextDouble() * (IMax - IMin))
          .ToDecimals(decimal_precision);
    }

    public static Vector2D MakeVector2D(int IMin = -10,
                                        int IMax = 10,
                                        int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      return new Vector2D(IMin + Seed.NextDouble() * (IMax - IMin), IMin + Seed.NextDouble() * (IMax - IMin))
          .ToDecimals(decimal_precision);
    }

    public static (LineSegment2D Segment, Point2D p0, Point2D p1)
        MakeLineSegment2D(int IMin = -10, int IMax = 10, int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      var p0 = MakePoint2D(IMin, IMax, decimal_precision);
      var p1 = MakePoint2D(IMin, IMax, decimal_precision);

      try {
        return (LineSegment2D.FromPoints(p0, p1, decimal_precision), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (Line2D Line, Point2D p0, Point2D p1)
        MakeLine2D(int IMin = -10, int IMax = 10, int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      var p0 = MakePoint2D(IMin, IMax, decimal_precision);
      var p1 = MakePoint2D(IMin, IMax, decimal_precision);

      try {
        return (Line2D.FromPoints(p0, p1, decimal_precision), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (Ray2D Ray, Point2D p0, Point2D p1)
        MakeRay2D(int IMin = -10, int IMax = 10, int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      var p0 = MakePoint2D(IMin, IMax, decimal_precision);
      var p1 = MakePoint2D(IMin, IMax, decimal_precision);

      try {
        return (new Ray2D(p0, (p1 - p0).Normalize()), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (Triangle2D Triangle, Point2D p0, Point2D p1, Point2D p2)
        MakeTriangle2D(int IMin = -10, int IMax = 10, int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      var p0 = MakePoint2D(IMin, IMax, decimal_precision);
      var p1 = MakePoint2D(IMin, IMax, decimal_precision);
      var p2 = MakePoint2D(IMin, IMax, decimal_precision);

      try {
        return (Triangle2D.FromPoints(p0, p1, p2, decimal_precision), p0, p1, p2);
      } catch (Exception) {
      }
      return (null, p0, p1, p2);
    }

    public static Polygon2D MakeTrianglePolygon2D(int IMin = -10,
                                                  int IMax = 10,
                                                  int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      // construct polygon based on a center and radius, and number of points to approximate a circle
      try {
        var t = MakeTriangle2D(IMin, IMax, decimal_precision);
        if (t.Triangle is null) {
          return null;
        }
        return new Polygon2D(t.Triangle, decimal_precision);
      } catch (Exception) {
      }
      return null;
    }

    public static Polygon2D MakeSquare2D(int IMin = -10,
                                         int IMax = 10,
                                         Point2D Center = null,
                                         int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      // construct polygon based on a center and radius, and number of points to approximate a circle
      try {
        var c = (Center is null) ? MakePoint2D(IMin, IMax, decimal_precision) : Center;
        double radius = Seed.NextDouble() * (IMax - IMin);

        var start_angle = Angle.FromRadians(Seed.NextDouble());
        var start_vector = new Vector2D(Math.Cos(start_angle.Radians), Math.Sin(start_angle.Radians));

        var angle_shift = Angle.FromRadians(Math.PI / 2);

        return new Polygon2D(
            new Point2D[4] { (c + start_vector * radius).ToDecimals(decimal_precision),
                             (c + start_vector.Rotate(1 * angle_shift) * radius).ToDecimals(decimal_precision),
                             (c + start_vector.Rotate(2 * angle_shift) * radius).ToDecimals(decimal_precision),
                             (c + start_vector.Rotate(3 * angle_shift) * radius).ToDecimals(decimal_precision) },
            decimal_precision);

      } catch (Exception) {
      }
      return null;
    }

    public static Polygon2D MakeRectangle2D(int IMin = -10,
                                            int IMax = 10,
                                            Point2D Center = null,
                                            int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      // construct polygon based on a center and radius, and number of points to approximate a circle
      try {
        var c = (Center is null) ? MakePoint2D(IMin, IMax, decimal_precision) : Center;
        double b = Seed.NextDouble() * (IMax - IMin);
        double h = Seed.NextDouble() * (IMax - IMin);

        var start_angle = Angle.FromRadians(Seed.NextDouble());
        var start_vector = new Vector2D(Math.Cos(start_angle.Radians), Math.Sin(start_angle.Radians));

        var angle_shift = Angle.FromRadians(Math.PI / 2);

        return new Polygon2D(
            new Point2D[4] { (c + start_vector * b).ToDecimals(decimal_precision),
                             (c + start_vector.Rotate(1 * angle_shift) * h).ToDecimals(decimal_precision),
                             (c + start_vector.Rotate(2 * angle_shift) * b).ToDecimals(decimal_precision),
                             (c + start_vector.Rotate(3 * angle_shift) * h).ToDecimals(decimal_precision) },
            decimal_precision);

      } catch (Exception) {
      }
      return null;
    }

    public static (Polygon2D Polygon, Point2D Center, double Radius, int Size)
        MakeConvexPolygon2D(int IMin = -10,
                            int IMax = 10,
                            int NMax = 30,
                            Point2D Center = null,
                            int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      // construct polygon based on a center and radius, and number of points to approximate a circle
      try {
        var c = (Center is null) ? MakePoint2D(IMax, IMin, decimal_precision) : Center;
        double r = Seed.NextDouble() * (IMax - IMin);
        int n = Seed.Next(NMax);
        if (n < 4) {
          throw new Exception("polygon generated with less than 4 points");
        }
        double rads = Math.PI * 2 / n;
        double start_rads = Seed.NextDouble() * Math.PI * 2;

        var cv_points = new List<Point2D>();
        for (int i = 0; i < n; ++i) {
          cv_points.Add(
              new Point2D(c.U + r * Math.Cos(start_rads + (i * rads)), c.V + r * Math.Sin(start_rads + (i * rads)))
                  .ToDecimals(decimal_precision));
        }

        return (Polygon2D.ConvexHull(cv_points, decimal_precision), c, r, n);
      } catch (Exception) {
      }
      return (null, null, 0, 0);
    }

    public static (Polyline2D Polyline, UnitVector2D Direction, int Size) MakeSimplePolyline2D(
        int IMin = -10, int IMax = 10, int NMax = 30, int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      // construct polyline with no self-intersections
      // build a p0 + sin approximated line
      try {
        var p0 = MakePoint2D(IMax, IMin, decimal_precision);
        var direction = MakeVector2D(IMin, IMax, decimal_precision).Normalize();

        double length = Seed.NextDouble() * (IMax - IMin);
        int n = Seed.Next(NMax);
        double r = 1 / n * length;
        double rads = Math.PI * 2 / n;
        double start_rads = Seed.NextDouble() * Math.PI * 2;

        var cv_points = new List<Point2D>();
        for (int i = 0; i < n; ++i) {
          var pi = p0 + i / n * length * direction;
          cv_points.Add(
              new Point2D(pi.U + r * Math.Cos(start_rads + (i * rads)), pi.V + r * Math.Sin(start_rads + (i * rads)))
                  .ToDecimals(decimal_precision));
        }

        // System.Console.WriteLine("cv_points=" + cv_points.ToWkt());

        return (new Polyline2D(cv_points, decimal_precision), direction, n);
      } catch (Exception ex) {
        // System.Console.WriteLine("ex=" + ex.Message);
      }
      return (null, null, 0);
    }

    //  3D objects

    public static Point3D MakePoint3D(int IMin = -10,
                                      int IMax = 10,
                                      int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      return new Point3D(Math.Round(IMin + Seed.NextDouble() * (IMax - IMin), decimal_precision),
                         Math.Round(IMin + Seed.NextDouble() * (IMax - IMin), decimal_precision),
                         Math.Round(IMin + Seed.NextDouble() * (IMax - IMin), decimal_precision));
    }

    public static Vector3D MakeVector3D(int IMin = -10,
                                        int IMax = 10,
                                        int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      return new Vector3D(Math.Round(IMin + Seed.NextDouble() * (IMax - IMin), decimal_precision),
                          Math.Round(IMin + Seed.NextDouble() * (IMax - IMin), decimal_precision),
                          Math.Round(IMin + Seed.NextDouble() * (IMax - IMin), decimal_precision));
    }

    public static (Line3D Line, Point3D p0, Point3D p1)
        MakeLine3D(int IMin = -10, int IMax = 10, int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      var p0 = MakePoint3D(IMin, IMax, decimal_precision);
      var p1 = MakePoint3D(IMin, IMax, decimal_precision);

      try {
        return (Line3D.FromPoints(p0, p1, decimal_precision), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (LineSegment3D Segment, Point3D p0, Point3D p1)
        MakeLineSegment3D(int IMin = -10, int IMax = 10, int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      var p0 = MakePoint3D(IMax, IMin, decimal_precision);
      var p1 = MakePoint3D(IMax, IMin, decimal_precision);

      try {
        return (LineSegment3D.FromPoints(p0, p1, decimal_precision), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (Ray3D Ray, Point3D p0, Point3D p1)
        MakeRay3D(int IMin = -10, int IMax = 10, int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      var p0 = MakePoint3D(IMax, IMin, decimal_precision);
      var p1 = MakePoint3D(IMax, IMin, decimal_precision);

      try {
        return (new Ray3D(p0, (p1 - p0).Normalize()), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (Triangle3D Triangle, Point3D p0, Point3D p1, Point3D p2)
        MakeTriangle3D(int IMin = -10, int IMax = 10, int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      var p0 = MakePoint3D(IMax, IMin, decimal_precision);
      var p1 = MakePoint3D(IMax, IMin, decimal_precision);
      var p2 = MakePoint3D(IMax, IMin, decimal_precision);

      try {
        return (Triangle3D.FromPoints(p0, p1, p2, decimal_precision), p0, p1, p2);
      } catch (Exception) {
      }
      return (null, p0, p1, p2);
    }

    public static (Plane Plane, Point3D p0, Point3D p1, Point3D p2)
        MakePlane(int IMin = -10, int IMax = 10, int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      var p0 = MakePoint3D(IMax, IMin, decimal_precision);
      var p1 = MakePoint3D(IMax, IMin, decimal_precision);
      var p2 = MakePoint3D(IMax, IMin, decimal_precision);
      try {
        return (Plane.FromPoints(p0, p1, p2, decimal_precision), p0, p1, p2);
      } catch (Exception) {
      }
      return (null, p0, p1, p2);
    }

    public static (double a, double b) MakeLinearCombo2SumTo1() {
      double a = (double)Seed.Next(0, 100) / 100;
      return (a, 1 - a);
    }

    public static (double a, double b, double c) MakeLinearCombo3SumTo1() {
      double a = (double)Seed.Next(0, 100) / 100;
      double b = (double)Seed.Next(0, 100) / 100;
      return (a, b, 1 - a - b);
    }

    public static (Polygon3D Polygon, Point3D Center, double Radius, int Size)
        MakeConvexPolygon3D(int IMin = -10,
                            int IMax = 10,
                            int NMax = 30,
                            Point3D Center = null,
                            Plane RefPlane = null,
                            int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      var ref_plane = (RefPlane is null) ? MakePlane(IMin, IMax, decimal_precision).Plane : RefPlane;
      if (ref_plane is null) {
        return (null, null, 0, 0);
      }

      var random_poly_2d = MakeConvexPolygon2D(
          IMin,
          IMax,
          NMax,
          (Center is null) ? MakePoint2D(IMin, IMax, decimal_precision) : ref_plane.ProjectInto(Center),
          decimal_precision);
      if (random_poly_2d.Polygon is null) {
        return (null, null, 0, 0);
      }

      // construct polygon based on a center and radius, and number of points to approximate a circle
      try {
        (var c, double r, int n) =
            (ref_plane.Evaluate(random_poly_2d.Center), random_poly_2d.Radius, random_poly_2d.Size);
        var points = new List<Point3D>();
        foreach (var p in random_poly_2d.Polygon) {
          points.Add(ref_plane.Evaluate(p).ToDecimals(decimal_precision));
        }
        return (Polygon3D.ConvexHull(points, decimal_precision), c, r, n);

      } catch (Exception) {
      }
      return (null, null, 0, 0);
    }

    public static (Polyline3D Polyline, UnitVector3D DirectionLongitudinal, UnitVector3D DirectionLateral, int Size)
        MakeSimplePolyline3D(int IMin = -10,
                             int IMax = 10,
                             int NMax = 30,
                             int decimal_precision = GeomSharp.Constants.THREE_DECIMALS) {
      // construct polyline with no self-intersections
      // build a p0 + sin approximated line
      var p0 = MakePoint3D(IMax, IMin);
      var direction_longitudinal = MakeVector3D(IMin, IMax, decimal_precision).Normalize();
      var direction_elevation = (direction_longitudinal.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ);
      var direction_lateral = direction_longitudinal.CrossProduct(direction_elevation).Normalize();

      try {
        double length = Seed.NextDouble() * (IMax - IMin);
        int n = Seed.Next(NMax);
        double r = 1 / n * length;
        double rads_xy = Math.PI / n;
        double start_rads_xy = Seed.NextDouble() * Math.PI;
        double rads_yz = Math.PI * 2 / n;
        double start_rads_yz = Seed.NextDouble() * Math.PI * 2;

        var cv_points = new List<Point3D>();
        for (int i = 0; i < n; ++i) {
          var pi = p0 + i / n * length * direction_longitudinal;
          cv_points.Add(
              new Point3D(pi.X + r * Math.Sin(start_rads_yz + (i * rads_yz)) * Math.Cos(start_rads_xy + (i * rads_xy)),
                          pi.Y + r * Math.Sin(start_rads_yz + (i * rads_yz)) * Math.Sin(start_rads_xy + (i * rads_xy)),
                          pi.Z + r * Math.Cos(start_rads_yz + (i * rads_yz)) * Math.Sin(start_rads_xy + (i * rads_xy)))
                  .ToDecimals(decimal_precision));
        }

        return (new Polyline3D(cv_points, decimal_precision), direction_longitudinal, direction_lateral, n);
      } catch (Exception) {
      }
      return (null, null, null, 0);
    }
  }

}
