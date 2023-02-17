using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp.Transformation {
  public static class Shift3D {
    public static Point3D Shift(this Point3D o, Vector3D v) => o + v;

    public static Line3D Shift(this Line3D o, Vector3D v) => Line3D.FromDirection(o.Origin.Shift(v), o.Direction);

    public static LineSegment3D Shift(this LineSegment3D o, Vector3D v) => LineSegment3D.FromPoints(o.P0.Shift(v),
                                                                                                    o.P1.Shift(v));
    public static Ray3D Shift(this Ray3D o, Vector3D v) => new Ray3D(o.Origin.Shift(v), o.Direction);

    public static Plane Shift(this Plane o, Vector3D v) => Plane.FromPointAndNormal(o.Origin.Shift(v), o.Normal);

    public static PointSet3D Shift(this PointSet3D o, Vector3D v) => new PointSet3D(o.Select(p => p.Shift(v)));

    public static LineSegmentSet3D Shift(this LineSegmentSet3D o,
                                         Vector3D v) => new LineSegmentSet3D(o.Select(l => l.Shift(v)));

    public static Triangle3D Shift(this Triangle3D o,
                                   Vector3D v) => Triangle3D.FromPoints(o.P0.Shift(v), o.P1.Shift(v), o.P2.Shift(v));

    public static Polygon3D Shift(this Polygon3D o, Vector3D v) => new Polygon3D(o.Select(p => p.Shift(v)));

    public static Polyline3D Shift(this Polyline3D o, Vector3D v) => new Polyline3D(o.Select(p => p.Shift(v)));
  }
}
