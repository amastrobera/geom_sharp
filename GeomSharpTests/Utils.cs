using System;
using System.Collections.Generic;
using GeomSharp;

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
        return (Line2D.FromTwoPoints(p0, p1), p0, p1);
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

    public static (Polygon2D Polygon, Point2D Center, double Radius, int Size)
        MakeConvexPolygon2D(int IMin = -10, int IMax = 10, int NMax = 20, int RMax = 30) {
      // construct polygon based on a center and radius, and number of points to approximate a circle
      var c = new Point2D(seed.Next(IMin, IMax), seed.Next(IMin, IMax));
      double r = seed.NextDouble() * RMax;
      int n = seed.Next(NMax);
      double rads = Math.PI * 2 / n;

      var cv_points = new List<Point2D>();
      for (int i = 0; i < n; ++i) {
        cv_points.Add(new Point2D(c.U + r * Math.Cos(i * rads), c.V + r * Math.Sin(i * rads)));
      }

      try {
        return (new Polygon2D(cv_points), c, r, n);
      } catch (Exception) {
      }
      return (null, c, r, n);
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
  }

}
