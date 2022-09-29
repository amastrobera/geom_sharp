namespace GeomSharp {

  public static class ComparisonExtensions3D {
    public static bool IsPerpendicular(this Plane plane, Vector3D vec) =>
        plane.Normal.IsParallel(vec) && plane.AxisU.IsPerpendicular(vec);

    public static bool IsPerpendicular(this Vector3D vec, Plane plane) => plane.IsPerpendicular(vec);

    public static bool IsPerpendicular(this Plane plane, Line3D line) =>
        plane.Normal.IsParallel(line.Direction) && plane.AxisU.IsPerpendicular(line.Direction);

    public static bool IsPerpendicular(this Line3D line, Plane plane) => plane.IsPerpendicular(line);

    public static bool IsPerpendicular(this Plane plane,
                                       Ray3D ray) =>
        ray.ToLine().IsPerpendicular(plane);  // && ray.IsAhead(plane.Origin);

    public static bool IsPerpendicular(this Ray3D ray, Plane plane) => plane.IsPerpendicular(ray);

    public static bool IsPerpendicular(this Plane plane, LineSegment3D segment) =>
        segment.ToLine().IsPerpendicular(plane);  // && segment.Intersects(plane);

    public static bool IsPerpendicular(this LineSegment3D segment, Plane plane) => plane.IsPerpendicular(segment);
  }

}
