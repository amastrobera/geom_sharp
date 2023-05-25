using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp.Transformation {
  public static class Projections {
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
  }
}
