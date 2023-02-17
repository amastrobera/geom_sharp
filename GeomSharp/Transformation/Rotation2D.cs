using System;
using System.Linq;
using System.Collections.Generic;

using GeomSharp.Algebra;

namespace GeomSharp.Transformation {
  public static class Rotation2D {
    public static Matrix GetMatrix(Angle angle) {
      double cos = Math.Cos(angle.Radians);
      double sin = Math.Sin(angle.Radians);

      return new Matrix(new List<List<double>> {// clang-format off
          new List<double> { cos, -sin }, 
          new List<double> { sin, cos } }  // clang-format on
      );
    }
    public static Vector2D Rotate(this Vector2D o, Angle angle) => Vector2D.FromVector(GetMatrix(angle) * o.ToVector());

    public static UnitVector2D Rotate(this UnitVector2D o,
                                      Angle angle) => Vector2D.FromVector(GetMatrix(angle) * o.ToVector()).Normalize();

    public static Point2D Rotate(this Point2D o, Angle angle) => Point2D.FromVector(GetMatrix(angle) * o.ToVector());

    public static Line2D Rotate(this Line2D o, Angle angle) => Line2D.FromDirection(o.Origin,
                                                                                    o.Direction.Rotate(angle));

    public static LineSegment2D Rotate(this LineSegment2D o,
                                       Angle angle) => LineSegment2D.FromPoints(o.P0.Rotate(angle), o.P1.Rotate(angle));
    public static Ray2D Rotate(this Ray2D o, Angle angle) => new Ray2D(o.Origin, o.Direction.Rotate(angle));

    public static PointSet2D Rotate(this PointSet2D o, Angle angle) => new PointSet2D(o.Select(p => p.Rotate(angle)));

    public static LineSegmentSet2D Rotate(this LineSegmentSet2D o,
                                          Angle angle) => new LineSegmentSet2D(o.Select(l => l.Rotate(angle)));

    public static Triangle2D Rotate(this Triangle2D o, Angle angle) => Triangle2D.FromPoints(o.P0.Rotate(angle),
                                                                                             o.P1.Rotate(angle),
                                                                                             o.P2.Rotate(angle));

    public static Polygon2D Rotate(this Polygon2D o, Angle angle) => new Polygon2D(o.Select(p => p.Rotate(angle)));

    public static Polyline2D Rotate(this Polyline2D o, Angle angle) => new Polyline2D(o.Select(p => p.Rotate(angle)));
  }
}
