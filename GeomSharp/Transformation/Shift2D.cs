using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp.Transformation {
  public static class Shift2D {
    public static Point2D Shift(this Point2D o, Vector2D v) => o + v;

    public static Line2D Shift(this Line2D o, Vector2D v) => Line2D.FromDirection(o.Origin.Shift(v), o.Direction);

    public static LineSegment2D Shift(this LineSegment2D o, Vector2D v) => LineSegment2D.FromPoints(o.P0.Shift(v),
                                                                                                    o.P1.Shift(v));
    public static Ray2D Shift(this Ray2D o, Vector2D v) => new Ray2D(o.Origin.Shift(v), o.Direction);

    public static PointSet2D Shift(this PointSet2D o, Vector2D v) => new PointSet2D(o.Select(p => p.Shift(v)));

    public static LineSegmentSet2D Shift(this LineSegmentSet2D o,
                                         Vector2D v) => new LineSegmentSet2D(o.Select(l => l.Shift(v)));

    public static Triangle2D Shift(this Triangle2D o,
                                   Vector2D v) => Triangle2D.FromPoints(o.P0.Shift(v), o.P1.Shift(v), o.P2.Shift(v));

    public static Polygon2D Shift(this Polygon2D o, Vector2D v) => new Polygon2D(o.Select(p => p.Shift(v)));

    public static Polyline2D Shift(this Polyline2D o, Vector2D v) => new Polyline2D(o.Select(p => p.Shift(v)));
  }
}
