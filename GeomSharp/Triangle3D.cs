using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  public class Triangle3D : IEquatable<Triangle3D> {
    public Point3D P0 { get; }
    public Point3D P1 { get; }
    public Point3D P2 { get; }

    public UnitVector3D Normal { get; }
    public UnitVector3D U { get; }
    public UnitVector3D V { get; }

    private Triangle3D(Point3D p0, Point3D p1, Point3D p2) {
      P0 = p0;
      P1 = p1;
      P2 = p2;
      // unit vectors (normalized)
      U = (P1 - P0).Normalize();
      V = (P2 - P0).Normalize();

      Normal = U.CrossProduct(V).Normalize();
    }

    public static Triangle3D FromPoints(Point3D p0, Point3D p1, Point3D p2) {
      if ((p1 - p0).IsParallel(p2 - p0)) {
        throw new ArithmeticException("tried to construct a Triangle with collinear points");
      }

      var t = new Triangle3D(p0, p1, p2);

      if (t.Area() == 0) {
        throw new ArithmeticException("tried to construct a Triangle of nearly zero-area");
      }

      return t;
    }

    public double Area() => Math.Round((P1 - P0).CrossProduct(P2 - P0).Length() / 2, Constants.NINE_DECIMALS);

    public Point3D CenterOfMass() => Point3D.FromVector((P0.ToVector() + P1.ToVector() + P2.ToVector()) / 3);

    public Plane RefPlane() => Plane.FromPoints(P0, P1, P2);

    /// <summary>
    /// Tells whether a point is inside a triangle T.
    /// First tests if a point is on the same plaane of the triangle. The projects everything in 2D and tests for
    /// containment over there.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool Contains(Point3D point) {
      // first test: is it on the same plane ?
      var plane = RefPlane();
      if (!Plane.FromPoints(P0, P1, P2).Contains(point)) {
        return false;
      }

      // project into 2D plane and fint out whether there is containment
      var triangle_2d = Triangle2D.FromPoints(plane.ProjectInto(P0), plane.ProjectInto(P1), plane.ProjectInto(P2));
      var point_2d = plane.ProjectInto(point);

      return triangle_2d.Contains(point_2d);
    }

    /// <summary>
    /// Tells whether a point is inside a triangle T. This includes points on the borders, dT.
    /// Uses baricentric coordinate test
    /// Computes the solution with Ruffini's rule
    /// P = M * x
    /// | xp | = | x_p0 x_p1 x_p2 |   | a |
    /// | yp | = | y_p0 y_p1 y_p2 | * | b |
    /// | zp | = | z_p0 z_p1 z_p2 |   | c |
    /// Find (a,b,c)
    /// If all of (a,b,c) is in the range [0,1] the point belongs to the triangle
    /// It has a problem with points (P0, P1, P2) where one of them is a linear combination of another
    /// (i.e. P2 = -2*P1): the determinant becomes zero, one column is removed from the matrix A, and the system becomes
    /// impossible (system of 2 equations and 3 unknowns).
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool Contains2(Point3D point) {
      // first test: is it on the same plane ?
      if (!Plane.FromPoints(P0, P1, P2).Contains(point)) {
        return false;
      }

      // second test: can I obtain its baricentric coordinates with all coefficients in the range [0,1] ?
      var mtx = Matrix<double>.Build.DenseOfColumnArrays(new double[][] { P0.ToArray(), P1.ToArray(), P2.ToArray() });

      // handle special case that would zero the determinant: a zero-row, replaced with one-row
      for (int i = 0; i < 3; ++i) {
        var row = mtx.Row(i);
        if (Math.Round(row.L1Norm(), Constants.NINE_DECIMALS) == 0) {
          mtx.SetRow(i, Vector<double>.Build.Dense(new double[] { 1, 1, 1 }));
          break;
        }
      }

      double d = mtx.Determinant();
      if (Math.Round(d, Constants.NINE_DECIMALS) == 0) {
        throw new Exception(String.Format(
            "matrix of points P0={0}, P1={1}, P2={2} make a linearly dependent column, cannot solve this system",
            P0.ToWkt(Constants.THREE_DECIMALS),
            P1.ToWkt(Constants.THREE_DECIMALS),
            P2.ToWkt(Constants.THREE_DECIMALS)));
      }

      // I could do Ruffini's rule manually, but why ? I can use the solver below
      var abc = mtx.Solve(point.ToVector());  // QR Factorization based solution from Math.Net
      (double a, double b, double c) = (Math.Round(abc[0], Constants.NINE_DECIMALS),
                                        Math.Round(abc[1], Constants.NINE_DECIMALS),
                                        Math.Round(abc[2], Constants.NINE_DECIMALS));

      return a >= 0 && a <= 1 && b >= 0 && b <= 1 && c >= 0 && c <= 1;
    }

    public bool Equals(Triangle3D other) {
      if (!Normal.Equals(other.Normal)) {
        return false;
      }
      var point_list = new List<Point3D>() { P0, P1, P2 };
      if (!point_list.Contains(other.P0)) {
        return false;
      }
      if (!point_list.Contains(other.P1)) {
        return false;
      }
      if (!point_list.Contains(other.P2)) {
        return false;
      }
      // no check on point order (CCW or CW) is needed, since the constructor guarantees the Normal to be contructed
      // by points, and therefore incorporates this information
      return true;
    }

    public override bool Equals(object other) => other != null && other is Triangle3D && this.Equals((Triangle3D)other);

    public override int GetHashCode() => new { P0, P1, P2, Normal }.GetHashCode();

    public static bool operator ==(Triangle3D a, Triangle3D b) {
      return a.Equals(b);
    }

    public static bool operator !=(Triangle3D a, Triangle3D b) {
      return !a.Equals(b);
    }

    public override string ToString() {
      return "{" + P0.ToString() + ", " + P1.ToString() + ", " + P2.ToString() + "}";
    }

    public bool Intersects(Triangle3D other) => Intersection(other).ValueType != typeof(NullValue);

    public IntersectionResult Intersection(Triangle3D other) {
      var plane_inter = RefPlane().Intersection(other.RefPlane());
      if (plane_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      var inter_line = (Line3D)plane_inter.Value;

      // TODO: line to triangle 1, triangle 2, check if points match

      // TODO
      return new IntersectionResult();
    }

    /// <summary>
    /// Tells whether two triangles overlap.
    /// To overlap the triangles must lie on the same plane, to begin with. Then it is sufficient for one point of a
    /// triangle to be contained in the other triangle.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Triangle3D other) {
      if (!RefPlane().AlmostEquals(other.RefPlane())) {
        return false;
      }
      return (Contains(other.P0) || Contains(other.P1) || Contains(other.P2) || other.Contains(P0) ||
              other.Contains(P1) || other.Contains(P2));
    }

    /// <summary>
    /// Finds the overlap of two triangles. The overlap can be a Point3D, or a LineSegment3D, a Triangle3D or a
    /// Polygon3D.
    /// To overlap the triangles must lie on the same plane, to begin with. Then it is sufficient for one
    /// point of a triangle to be contained in the other triangle.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Overlap(Triangle3D other) {
      if (!RefPlane().AlmostEquals(other.RefPlane())) {
        return new IntersectionResult();
      }

      (bool p0_in, bool p1_in, bool p2_in, bool other_p0_in, bool other_p1_in, bool other_p2_in) = (Contains(other.P0),
                                                                                                    Contains(other.P1),
                                                                                                    Contains(other.P2),
                                                                                                    other.Contains(P0),
                                                                                                    other.Contains(P1),
                                                                                                    other.Contains(P2));

      // the two triangles match or t1 is contained in t2
      if (p0_in && p1_in && p2_in) {
        return new IntersectionResult(this);
      }

      // t2 is contained in t1
      if (!p0_in && !p1_in && !p2_in && other_p0_in && other_p1_in && other_p2_in) {
        return new IntersectionResult(other);
      }

      // TODO
      // if (!p0_in && !p1_in && !p2_in && other_p0_in && other_p1_in && other_p2_in) {
      //  return new IntersectionResult(other);
      //}

      return new IntersectionResult();
    }

    public string ToWkt() {
      return string.Format(
          "POLYGON (({0:F2} {1:F2} {2:F2}, {3:F2} {4:F2} {5:F2}, {6:F2} {7:F2} {8:F2}, {0:F2} {1:F2} {2:F2}))",
          P0.X,
          P0.Y,
          P0.Z,
          P1.X,
          P1.Y,
          P1.Z,
          P2.X,
          P2.Y,
          P2.Z);
    }
  }

}
