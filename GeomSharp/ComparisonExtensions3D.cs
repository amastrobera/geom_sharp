namespace GeomSharp {

  public static class ComparisonExtensions3D {
    public static bool IsPerpendicular(this Plane plane,
                                       Vector3D vec,
                                       int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Normal.IsParallel(vec, decimal_precision) && plane.AxisU.IsPerpendicular(vec, decimal_precision);

    public static bool IsPerpendicular(this Vector3D vec,
                                       Plane plane,
                                       int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.IsPerpendicular(vec, decimal_precision);

    public static bool IsPerpendicular(this Plane plane,
                                       Line3D line,
                                       int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.Normal.IsParallel(line.Direction, decimal_precision) && plane.AxisU.IsPerpendicular(line.Direction,
                                                                                                  decimal_precision);

    public static bool IsPerpendicular(this Line3D line,
                                       Plane plane,
                                       int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.IsPerpendicular(line, decimal_precision);

    public static bool IsPerpendicular(this Plane plane, Ray3D ray, int decimal_precision = Constants.THREE_DECIMALS) =>
        ray.ToLine().IsPerpendicular(plane, decimal_precision);  // && ray.IsAhead(plane.Origin);

    public static bool IsPerpendicular(this Ray3D ray, Plane plane, int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.IsPerpendicular(ray, decimal_precision);

    public static bool IsPerpendicular(this Plane plane,
                                       LineSegment3D segment,
                                       int decimal_precision = Constants.THREE_DECIMALS) =>
        segment.ToLine().IsPerpendicular(plane, decimal_precision);  // && segment.Intersects(plane);

    public static bool IsPerpendicular(this LineSegment3D segment,
                                       Plane plane,
                                       int decimal_precision = Constants.THREE_DECIMALS) =>
        plane.IsPerpendicular(segment, decimal_precision);
  }

}
