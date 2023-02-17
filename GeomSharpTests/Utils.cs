using System;
using System.Collections.Generic;

using GeomSharp;
using GeomSharp.Algebra;
using GeomSharp.Transformation;

namespace GeomSharpTests {

  class RandomGenerator {
    static Random seed = new Random();

    // 2D objects

    public static Point2D MakePoint2D(int IMin = -10, int IMax = 10) {
      return new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));
    }

    public static Vector2D MakeVector2D(int IMin = -10, int IMax = 10) {
      return new Vector2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));
    }

    public static (LineSegment2D Segment, Point2D p0, Point2D p1) MakeLineSegment2D(int IMin = -10, int IMax = 10) {
      var p0 = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p1 = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));

      try {
        return (LineSegment2D.FromPoints(p0, p1), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (Line2D Line, Point2D p0, Point2D p1) MakeLine2D(int IMin = -10, int IMax = 10) {
      var p0 = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p1 = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));

      try {
        return (Line2D.FromPoints(p0, p1), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (Ray2D Ray, Point2D p0, Point2D p1) MakeRay2D(int IMin = -10, int IMax = 10) {
      var p0 = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p1 = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));

      try {
        return (new Ray2D(p0, (p1 - p0).Normalize()), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (Triangle2D Triangle, Point2D p0, Point2D p1, Point2D p2)
        MakeTriangle2D(int IMin = -10, int IMax = 10) {
      var p0 = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p1 = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p2 = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));

      try {
        return (Triangle2D.FromPoints(p0, p1, p2), p0, p1, p2);
      } catch (Exception) {
      }
      return (null, p0, p1, p2);
    }

    public static Polygon2D MakeTrianglePolygon2D(int IMin = -10, int IMax = 10) {
      // construct polygon based on a center and radius, and number of points to approximate a circle
      try {
        var t = MakeTriangle2D(IMin, IMax);
        if (t.Triangle is null) {
          return null;
        }
        return new Polygon2D(t.Triangle);
      } catch (Exception) {
      }
      return null;
    }

    public static Polygon2D MakeSquare2D(int IMin = -10, int IMax = 10, Point2D Center = null) {
      // construct polygon based on a center and radius, and number of points to approximate a circle
      try {
        var c = (Center is null) ? new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax)) : Center;
        double radius = seed.NextDouble() * (IMax - IMin);

        var start_angle = Angle.FromRadians(seed.NextDouble());
        var start_vector = new Vector2D(Math.Cos(start_angle.Radians), Math.Sin(start_angle.Radians));

        var angle_shift = Angle.FromRadians(Math.PI / 2);

        return new Polygon2D(new Point2D[4] { c + start_vector * radius,
                                              c + start_vector.Rotate(1 * angle_shift) * radius,
                                              c + start_vector.Rotate(2 * angle_shift) * radius,
                                              c + start_vector.Rotate(3 * angle_shift) * radius });

      } catch (Exception) {
      }
      return null;
    }

    public static Polygon2D MakeRectangle2D(int IMin = -10, int IMax = 10, Point2D Center = null) {
      // construct polygon based on a center and radius, and number of points to approximate a circle
      try {
        var c = (Center is null) ? new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax)) : Center;
        double b = seed.NextDouble() * (IMax - IMin);
        double h = seed.NextDouble() * (IMax - IMin);

        var start_angle = Angle.FromRadians(seed.NextDouble());
        var start_vector = new Vector2D(Math.Cos(start_angle.Radians), Math.Sin(start_angle.Radians));

        var angle_shift = Angle.FromRadians(Math.PI / 2);

        return new Polygon2D(new Point2D[4] { c + start_vector * b,
                                              c + start_vector.Rotate(1 * angle_shift) * h,
                                              c + start_vector.Rotate(2 * angle_shift) * b,
                                              c + start_vector.Rotate(3 * angle_shift) * h });

      } catch (Exception) {
      }
      return null;
    }

    public static (Polygon2D Polygon, Point2D Center, double Radius, int Size)
        MakeConvexPolygon2D(int IMin = -10, int IMax = 10, int NMax = 30, Point2D Center = null) {
      // construct polygon based on a center and radius, and number of points to approximate a circle
      try {
        var c = (Center is null) ? new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax)) : Center;
        double r = seed.NextDouble() * (IMax - IMin);
        int n = seed.Next(NMax);
        if (n < 4) {
          throw new Exception("polygon generated with less than 4 points");
        }
        double rads = Math.PI * 2 / n;
        double start_rads = seed.NextDouble() * Math.PI * 2;

        var cv_points = new List<Point2D>();
        for (int i = 0; i < n; ++i) {
          cv_points.Add(
              new Point2D(c.U + r * Math.Cos(start_rads + (i * rads)), c.V + r * Math.Sin(start_rads + (i * rads))));
        }

        return (new Polygon2D(cv_points), c, r, n);
      } catch (Exception) {
      }
      return (null, null, 0, 0);
    }

    public static (Polyline2D Polyline, UnitVector2D Direction, int Size)
        MakeSimplePolyline2D(int IMin = -10, int IMax = 10, int NMax = 30) {
      // construct polyline with no self-intersections
      // build a p0 + sin approximated line
      try {
        var p0 = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));
        var direction = MakeVector2D(IMin, IMax).Normalize();

        double length = seed.NextDouble() * (IMax - IMin);
        int n = seed.Next(NMax);
        double r = 1 / n * length;
        double rads = Math.PI * 2 / n;
        double start_rads = seed.NextDouble() * Math.PI * 2;

        var cv_points = new List<Point2D>();
        for (int i = 0; i < n; ++i) {
          var pi = p0 + i / n * length * direction;
          cv_points.Add(
              new Point2D(pi.U + r * Math.Cos(start_rads + (i * rads)), pi.V + r * Math.Sin(start_rads + (i * rads))));
        }

        return (new Polyline2D(cv_points), direction, n);
      } catch (Exception) {
      }
      return (null, null, 0);
    }

    //  3D objects

    public static Point3D MakePoint3D(int IMin = -10, int IMax = 10) {
      return new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
    }

    public static Vector3D MakeVector3D(int IMin = -10, int IMax = 10) {
      return new Vector3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
    }

    public static (Line3D Segment, Point3D p0, Point3D p1) MakeLine3D(int IMin = -10, int IMax = 10) {
      var p0 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p1 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));

      try {
        return (Line3D.FromPoints(p0, p1), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (LineSegment3D Segment, Point3D p0, Point3D p1) MakeLineSegment3D(int IMin = -10, int IMax = 10) {
      var p0 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p1 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));

      try {
        return (LineSegment3D.FromPoints(p0, p1), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (Ray3D Ray, Point3D p0, Point3D p1) MakeRay3D(int IMin = -10, int IMax = 10) {
      var p0 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p1 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));

      try {
        return (new Ray3D(p0, (p1 - p0).Normalize()), p0, p1);
      } catch (Exception) {
      }
      return (null, p0, p1);
    }

    public static (Triangle3D Triangle, Point3D p0, Point3D p1, Point3D p2)
        MakeTriangle3D(int IMin = -10, int IMax = 10) {
      var p0 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p1 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p2 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      try {
        return (Triangle3D.FromPoints(p0, p1, p2), p0, p1, p2);
      } catch (Exception) {
      }
      return (null, p0, p1, p2);
    }

    public static (Plane Plane, Point3D p0, Point3D p1, Point3D p2) MakePlane(int IMin = -10, int IMax = 10) {
      var p0 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p1 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var p2 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      try {
        return (Plane.FromPoints(p0, p1, p2), p0, p1, p2);
      } catch (Exception) {
      }
      return (null, p0, p1, p2);
    }

    public static (double a, double b) MakeLinearCombo2SumTo1() {
      double a = (double)seed.Next(0, 100) / 100;
      return (a, 1 - a);
    }

    public static (double a, double b, double c) MakeLinearCombo3SumTo1() {
      double a = (double)seed.Next(0, 100) / 100;
      double b = (double)seed.Next(0, 100) / 100;
      return (a, b, 1 - a - b);
    }

    public static (Polygon3D Polygon, Point3D Center, double Radius, int Size) MakeConvexPolygon3D(
        int IMin = -10, int IMax = 10, int NMax = 30, Point3D Center = null, Plane RefPlane = null) {
      var ref_plane = (RefPlane is null) ? MakePlane(IMin, IMax).Plane : RefPlane;
      if (ref_plane is null) {
        return (null, null, 0, 0);
      }

      var random_poly_2d =
          MakeConvexPolygon2D(IMin,
                              IMax,
                              NMax,
                              (Center is null) ? MakePoint2D(IMin, IMax) : ref_plane.ProjectInto(Center));
      if (random_poly_2d.Polygon is null) {
        return (null, null, 0, 0);
      }

      // construct polygon based on a center and radius, and number of points to approximate a circle
      try {
        (var c, double r, int n) =
            (ref_plane.Evaluate(random_poly_2d.Center), random_poly_2d.Radius, random_poly_2d.Size);
        var points = new List<Point3D>();
        foreach (var p in random_poly_2d.Polygon) {
          points.Add(ref_plane.Evaluate(p));
        }
        return (new Polygon3D(points), c, r, n);

      } catch (Exception) {
      }
      return (null, null, 0, 0);
    }

    public static (Polyline3D Polyline, UnitVector3D DirectionLongitudinal, UnitVector3D DirectionLateral, int Size)
        MakeSimplePolyline3D(int IMin = -10, int IMax = 10, int NMax = 30) {
      // construct polyline with no self-intersections
      // build a p0 + sin approximated line
      var p0 = new Point3D(seed.Next(IMin, IMax), seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      var direction_longitudinal = MakeVector3D(IMin, IMax).Normalize();
      var direction_elevation = (direction_longitudinal.IsParallel(Vector3D.AxisZ) ? Vector3D.AxisY : Vector3D.AxisZ);
      var direction_lateral = direction_longitudinal.CrossProduct(direction_elevation).Normalize();

      try {
        double length = seed.NextDouble() * (IMax - IMin);
        int n = seed.Next(NMax);
        double r = 1 / n * length;
        double rads_xy = Math.PI / n;
        double start_rads_xy = seed.NextDouble() * Math.PI;
        double rads_yz = Math.PI * 2 / n;
        double start_rads_yz = seed.NextDouble() * Math.PI * 2;

        var cv_points = new List<Point3D>();
        for (int i = 0; i < n; ++i) {
          var pi = p0 + i / n * length * direction_longitudinal;
          cv_points.Add(new Point3D(
              pi.X + r * Math.Sin(start_rads_yz + (i * rads_yz)) * Math.Cos(start_rads_xy + (i * rads_xy)),
              pi.Y + r * Math.Sin(start_rads_yz + (i * rads_yz)) * Math.Sin(start_rads_xy + (i * rads_xy)),
              pi.Z + r * Math.Cos(start_rads_yz + (i * rads_yz)) * Math.Sin(start_rads_xy + (i * rads_xy))));
        }

        return (new Polyline3D(cv_points), direction_longitudinal, direction_lateral, n);
      } catch (Exception) {
      }
      return (null, null, null, 0);
    }
  }

}
