using System;
using System.Linq;
using System.Collections.Generic;

using GeomSharp.Algebra;

namespace GeomSharp.Transformation {
  public static class Rotation3D {
    public static Matrix GetMatrixX(Angle angle) {
      double cos = Math.Cos(angle.Radians);
      double sin = Math.Sin(angle.Radians);

      return new Matrix(new List<List<double>> {
        // clang-format off
        new List<double> { 1, 0, 0 },
        new List<double> { 0, cos, -sin },
        new List<double> { 0, sin, cos },
        // clang-format on
      });
    }

    public static Matrix GetMatrixY(Angle angle) {
      double cos = Math.Cos(angle.Radians);
      double sin = Math.Sin(angle.Radians);

      return new Matrix(new List<List<double>> {
        // clang-format off
      new List<double> { cos, 0, sin },
      new List<double> { 0, 1, 0 },
      new List<double> { -sin, 0, cos },
        // clang-format on
      });
    }

    public static Matrix GetMatrixZ(Angle angle) {
      double cos = Math.Cos(angle.Radians);
      double sin = Math.Sin(angle.Radians);

      return new Matrix(new List<List<double>> {
        // clang-format off
      new List<double> { cos, -sin, 0 },
      new List<double> { sin, cos, 0 },
      new List<double> { 0, 0, 1 },
        // clang-format on
      });
    }

    public static Matrix GetMatrix(UnitVector3D axis, Angle angle) {
      double cos = Math.Cos(angle.Radians);
      double sin = Math.Sin(angle.Radians);

      (double ux2, double ux_uy, double ux_uz, double uy2, double uy_uz, double uz2, double ux, double uy, double uz) =
          (axis.X * axis.X,
           axis.X * axis.Y,
           axis.X * axis.Z,
           axis.Y * axis.Y,
           axis.Y * axis.Z,
           axis.Z * axis.Z,
           axis.X,
           axis.Y,
           axis.Z);

      return new Matrix(new List<List<double>> {
        // clang-format off
      new List<double> { cos + ux2*(1-cos), ux_uy*(1-cos) - uz*sin, ux_uz*(1-cos) + uy*sin },
      new List<double> { ux_uy*(1-cos) + uz*sin, cos + uy2*(1-cos), uy_uz*(1-cos) - ux*sin },
      new List<double> { ux_uz*(1-cos) - uy*sin, uy_uz*(1-cos) + ux*sin, cos + uz2*(1-cos)},
        // clang-format on
      });
    }

    // rotation around a custom axis

    public static Vector3D Rotate(this Vector3D o,
                                  UnitVector3D axis,
                                  Angle angle) => Vector3D.FromVector(GetMatrix(axis, angle) * o.ToVector());

    public static UnitVector3D Rotate(this UnitVector3D o, UnitVector3D axis, Angle angle) =>
        Vector3D.FromVector(GetMatrix(axis, angle) * o.ToVector()).Normalize();

    public static Point3D Rotate(this Point3D o,
                                 UnitVector3D axis,
                                 Angle angle) => Point3D.FromVector(GetMatrix(axis, angle) * o.ToVector());

    public static Line3D Rotate(this Line3D o,
                                UnitVector3D axis,
                                Angle angle) => Line3D.FromDirection(o.Origin, o.Direction.Rotate(axis, angle));

    public static LineSegment3D Rotate(this LineSegment3D o, UnitVector3D axis, Angle angle) =>
        LineSegment3D.FromPoints(o.P0.Rotate(axis, angle), o.P1.Rotate(axis, angle));
    public static Ray3D Rotate(this Ray3D o,
                               UnitVector3D axis,
                               Angle angle) => new Ray3D(o.Origin, o.Direction.Rotate(axis, angle));

    public static PointSet3D Rotate(this PointSet3D o,
                                    UnitVector3D axis,
                                    Angle angle) => new PointSet3D(o.Select(p => p.Rotate(axis, angle)));

    public static LineSegmentSet3D Rotate(this LineSegmentSet3D o,
                                          UnitVector3D axis,
                                          Angle angle) => new LineSegmentSet3D(o.Select(l => l.Rotate(axis, angle)));

    public static Triangle3D Rotate(this Triangle3D o, UnitVector3D axis, Angle angle) =>
        Triangle3D.FromPoints(o.P0.Rotate(axis, angle), o.P1.Rotate(axis, angle), o.P2.Rotate(axis, angle));

    public static Polygon3D Rotate(this Polygon3D o,
                                   UnitVector3D axis,
                                   Angle angle) => new Polygon3D(o.Select(p => p.Rotate(axis, angle)));

    public static Polyline3D Rotate(this Polyline3D o,
                                    UnitVector3D axis,
                                    Angle angle) => new Polyline3D(o.Select(p => p.Rotate(axis, angle)));

    // rotate around the X axis
    public static Vector3D RotateX(this Vector3D o,

                                   Angle angle) => Vector3D.FromVector(GetMatrixX(angle) * o.ToVector());

    public static UnitVector3D RotateX(this UnitVector3D o, Angle angle) =>
        Vector3D.FromVector(GetMatrixX(angle) * o.ToVector()).Normalize();

    public static Point3D RotateX(this Point3D o,

                                  Angle angle) => Point3D.FromVector(GetMatrixX(angle) * o.ToVector());

    public static Line3D RotateX(this Line3D o,

                                 Angle angle) => Line3D.FromDirection(o.Origin, o.Direction.RotateX(angle));

    public static LineSegment3D RotateX(this LineSegment3D o,
                                        Angle angle) => LineSegment3D.FromPoints(o.P0.RotateX(angle),
                                                                                 o.P1.RotateX(angle));
    public static Ray3D RotateX(this Ray3D o,

                                Angle angle) => new Ray3D(o.Origin, o.Direction.RotateX(angle));

    public static PointSet3D RotateX(this PointSet3D o,

                                     Angle angle) => new PointSet3D(o.Select(p => p.RotateX(angle)));

    public static LineSegmentSet3D RotateX(this LineSegmentSet3D o,

                                           Angle angle) => new LineSegmentSet3D(o.Select(l => l.RotateX(angle)));

    public static Triangle3D RotateX(this Triangle3D o, Angle angle) => Triangle3D.FromPoints(o.P0.RotateX(angle),
                                                                                              o.P1.RotateX(angle),
                                                                                              o.P2.RotateX(angle));

    public static Polygon3D RotateX(this Polygon3D o,

                                    Angle angle) => new Polygon3D(o.Select(p => p.RotateX(angle)));

    public static Polyline3D RotateX(this Polyline3D o,

                                     Angle angle) => new Polyline3D(o.Select(p => p.RotateX(angle)));

    // rotate around the Y axis
    public static Vector3D RotateY(this Vector3D o,

                                   Angle angle) => Vector3D.FromVector(GetMatrixY(angle) * o.ToVector());

    public static UnitVector3D RotateY(this UnitVector3D o, Angle angle) =>
        Vector3D.FromVector(GetMatrixY(angle) * o.ToVector()).Normalize();

    public static Point3D RotateY(this Point3D o,

                                  Angle angle) => Point3D.FromVector(GetMatrixY(angle) * o.ToVector());

    public static Line3D RotateY(this Line3D o,

                                 Angle angle) => Line3D.FromDirection(o.Origin, o.Direction.RotateY(angle));

    public static LineSegment3D RotateY(this LineSegment3D o,
                                        Angle angle) => LineSegment3D.FromPoints(o.P0.RotateY(angle),
                                                                                 o.P1.RotateY(angle));
    public static Ray3D RotateY(this Ray3D o,

                                Angle angle) => new Ray3D(o.Origin, o.Direction.RotateY(angle));

    public static PointSet3D RotateY(this PointSet3D o,

                                     Angle angle) => new PointSet3D(o.Select(p => p.RotateY(angle)));

    public static LineSegmentSet3D RotateY(this LineSegmentSet3D o,

                                           Angle angle) => new LineSegmentSet3D(o.Select(l => l.RotateY(angle)));

    public static Triangle3D RotateY(this Triangle3D o, Angle angle) => Triangle3D.FromPoints(o.P0.RotateY(angle),
                                                                                              o.P1.RotateY(angle),
                                                                                              o.P2.RotateY(angle));

    public static Polygon3D RotateY(this Polygon3D o,

                                    Angle angle) => new Polygon3D(o.Select(p => p.RotateY(angle)));

    public static Polyline3D RotateY(this Polyline3D o,

                                     Angle angle) => new Polyline3D(o.Select(p => p.RotateY(angle)));

    // rotate around the Z axis
    public static Vector3D RotateZ(this Vector3D o,

                                   Angle angle) => Vector3D.FromVector(GetMatrixZ(angle) * o.ToVector());

    public static UnitVector3D RotateZ(this UnitVector3D o, Angle angle) =>
        Vector3D.FromVector(GetMatrixZ(angle) * o.ToVector()).Normalize();

    public static Point3D RotateZ(this Point3D o,

                                  Angle angle) => Point3D.FromVector(GetMatrixZ(angle) * o.ToVector());

    public static Line3D RotateZ(this Line3D o,

                                 Angle angle) => Line3D.FromDirection(o.Origin, o.Direction.RotateZ(angle));

    public static LineSegment3D RotateZ(this LineSegment3D o,
                                        Angle angle) => LineSegment3D.FromPoints(o.P0.RotateZ(angle),
                                                                                 o.P1.RotateZ(angle));
    public static Ray3D RotateZ(this Ray3D o,

                                Angle angle) => new Ray3D(o.Origin, o.Direction.RotateZ(angle));

    public static PointSet3D RotateZ(this PointSet3D o,

                                     Angle angle) => new PointSet3D(o.Select(p => p.RotateZ(angle)));

    public static LineSegmentSet3D RotateZ(this LineSegmentSet3D o,

                                           Angle angle) => new LineSegmentSet3D(o.Select(l => l.RotateZ(angle)));

    public static Triangle3D RotateZ(this Triangle3D o, Angle angle) => Triangle3D.FromPoints(o.P0.RotateZ(angle),
                                                                                              o.P1.RotateZ(angle),
                                                                                              o.P2.RotateZ(angle));

    public static Polygon3D RotateZ(this Polygon3D o,

                                    Angle angle) => new Polygon3D(o.Select(p => p.RotateZ(angle)));

    public static Polyline3D RotateZ(this Polyline3D o,

                                     Angle angle) => new Polyline3D(o.Select(p => p.RotateZ(angle)));
  }
}
