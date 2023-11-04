using System;

namespace GeomSharp.Utils {
  public static class Conversions {
    public static Point2D ToDecimals(this Point2D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        new Point2D(Math.Round(p.U, decimal_precision), Math.Round(p.V, decimal_precision));

    public static Point3D ToDecimals(this Point3D p, int decimal_precision = Constants.THREE_DECIMALS) => new Point3D(
        Math.Round(p.X, decimal_precision), Math.Round(p.Y, decimal_precision), Math.Round(p.Z, decimal_precision));

    public static Vector2D ToDecimals(this Vector2D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        new Vector2D(Math.Round(p.U, decimal_precision), Math.Round(p.V, decimal_precision));

    public static Vector3D ToDecimals(this Vector3D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        new Vector3D(Math.Round(p.X, decimal_precision),
                     Math.Round(p.Y, decimal_precision),
                     Math.Round(p.Z, decimal_precision));
  }
}
