using GeomSharp.Algebra;
using GeomSharp.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GeomSharp {

  /// <summary>
  /// New class that extends the MathNet.Spatial.Euclidean namespace
  /// </summary>
  [Serializable]
  public class Polygon3D : Geometry3D, IEquatable<Polygon3D>, IEnumerable<Point3D>, ISerializable {
    public List<Point3D> Vertices { get; }
    public readonly int Size;
    public UnitVector3D Normal { get; }

    // constructors
    private Polygon3D(Point3D[] points, int decimal_precision = Constants.THREE_DECIMALS) {
      if (points.Length < 3) {
        throw new ArgumentException("tried to initialize a polygon with less than 3 points");
      }
      Vertices = (new List<Point3D>(points)).RemoveCollinearPoints(decimal_precision);
      // input adjustment: correcting mistake of passing collinear points to a polygon
      if (Vertices.Count < 3) {
        throw new ArgumentException("tried to initialize a polygon with less than 3 non-collinear points");
      }
      // adding the size
      Size = Vertices.Count;

      // adding the normal
      var centroid = Vertices.Average();
      Normal = (Vertices[0] - centroid).CrossProduct((Vertices[1] - centroid)).Normalize();

      // verify that all points are on the same plane!
      for (int i1 = 2; i1 < Size; i1++) {
        int i2 = (i1 + 1) % Size;
        var local_normal = (Vertices[0] - centroid).CrossProduct((Vertices[1] - centroid));
        if (!local_normal.CrossProduct(Normal).AlmostEquals(Vector3D.Zero, decimal_precision)) {
          throw new ArgumentException(
              "tried to initialize a Polygon3D with a set of points that do not belong to the same plane");
        }
      }
    }

    public Polygon3D(Triangle3D triangle, int decimal_precision = Constants.THREE_DECIMALS)
        : this(new Point3D[3] { triangle.P0, triangle.P1, triangle.P2 }, decimal_precision) {}

    public Polygon3D(IEnumerable<Point3D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    // enumeration interface implementation
    public Point3D this[int i] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return Vertices[i];
      }
    }
    public IEnumerator<Point3D> GetEnumerator() {
      return Vertices.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    // generic overrides from object class
    public override string ToString() => (Vertices.Count == 0)
                                             ? "{}"
                                             : "{" + string.Join(",", Vertices.Select(v => v.ToString())) +
                                                   "," + Vertices[0].ToString() + "}";
    public override int GetHashCode() => ToWkt().GetHashCode();

    // equality interface, and base class overrides
    public override bool Equals(object other) => other != null && other is Polygon3D && this.Equals((Polygon3D)other);
    public override bool Equals(Geometry3D other) => other.GetType() == typeof(Polygon3D) &&
                                                     this.Equals(other as Polygon3D);
    public bool Equals(Polygon3D other) => this.AlmostEquals(other);
    public override bool AlmostEquals(Geometry3D other, int decimal_precision = 3) =>
        other.GetType() == typeof(Polygon3D) && this.AlmostEquals(other as Polygon3D, decimal_precision);

    public bool AlmostEquals(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other is null) {
        return false;
      }
      // different number of points, different polygon (we assume they have been built removing collinear points and
      // duplicates (constructor guarantees that)
      if (other.Size != Size) {
        return false;
      }
      // different normal, different plane, different polygon
      if (!Normal.AlmostEquals(other.Normal, decimal_precision)) {
        return false;
      }

      // different number of points, different polygon
      if (other.Size != Size) {
        return false;
      }

      // different set of points, different polygons
      var point_count = new Dictionary<string, int>();
      foreach (var p in Vertices) {
        string key = p.ToWkt(decimal_precision);
        if (!point_count.ContainsKey(key)) {
          point_count[key] = 0;
        }
        point_count[key] += 1;
      }

      foreach (var p in other.Vertices) {
        string key = p.ToWkt(decimal_precision);
        if (point_count.ContainsKey(key) && point_count[key] > 0) {
          point_count[key] -= 1;
        }
      }

      int num_different = point_count.Sum(kv => kv.Value);
      System.Console.WriteLine("num_different=" + num_different.ToString());
      if (num_different > 0) {
        return false;
      }

      return true;
    }

    // comparison operators
    public static bool operator ==(Polygon3D a, Polygon3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Polygon3D a, Polygon3D b) {
      return !a.AlmostEquals(b);
    }

    // serialization interface implementation and base class overrides
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Size", Size, typeof(int));
      info.AddValue("Vertices", Vertices, typeof(List<Point3D>));
    }

    public Polygon3D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Size = (int)info.GetValue("Size", typeof(int));
      Vertices = (List<Point3D>)info.GetValue("Vertices", typeof(List<Point3D>));
    }

    public static Polygon3D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Polygon3D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    // well known text base class overrides
    public override string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Vertices.Count == 0)
            ? "POLYGON EMPTY"
            : "POLYGON ((" +
                  string.Join(
                      ",",
                      Vertices.Select(v => string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}",
                                                                       "{",
                                                                       precision,
                                                                       "}"),
                                                         v.X,
                                                         v.Y,
                                                         v.Z))) +
                  ", " +
                  string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                                Vertices[0].X,
                                Vertices[0].Y,
                                Vertices[0].Z) +
                  "))";

    // relationship to all the other geometries

    //  plane
    public override bool Intersects(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Plane other, int decimal_precision = Constants.THREE_DECIMALS) {
      var poly_plane = RefPlane();
      var plane_inter = poly_plane.Intersection(other, decimal_precision);
      if (plane_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }
      var plane_line = (Line3D)plane_inter.Value;

      // change the problem into a 2D one
      //      intersect in 2D one polygon with the line, the result is either empty or a multi line set
      var poly_2D = new Polygon2D(Vertices.Select(p => poly_plane.ProjectInto(p)), decimal_precision);
      var plane_line_2D = Line2D.FromPoints(poly_plane.ProjectInto(plane_line.P0),
                                            poly_plane.ProjectInto(plane_line.P1),
                                            decimal_precision);
      var poly_inter_set_2D = poly_2D.Intersection(plane_line_2D, decimal_precision);
      if (poly_inter_set_2D.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      if (poly_inter_set_2D.ValueType == typeof(LineSegment2D)) {
        return new IntersectionResult(poly_plane.Evaluate((LineSegment2D)poly_inter_set_2D.Value));
      }

      if (poly_inter_set_2D.ValueType == typeof(LineSegmentSet2D)) {
        var mline = new List<LineSegment3D>();
        foreach (var seg in (LineSegmentSet2D)poly_inter_set_2D.Value) {
          mline.Add(poly_plane.Evaluate(seg));
        }
        return new IntersectionResult(new LineSegmentSet3D(mline));
      }

      throw new Exception("Plane to Polygon intersection, unkown return type " +
                          poly_inter_set_2D.ValueType.ToString());
    }
    public override bool Overlaps(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Plane other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    // point
    public override bool Contains(Point3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      // first check to save time, if the point is outside the plane, then it's outside the polygon too
      // improvement to the algorithm's big-O
      var plane = Plane.FromPointAndNormal(Vertices[0], Normal, decimal_precision);
      if (!plane.Contains(other, decimal_precision)) {
        return false;
      }

      // transform the problem into a 2D one (a polygon is a planar geometry after all)
      return new Polygon2D(Vertices.Select(v => plane.ProjectInto(v)))
          .Contains(plane.ProjectInto(other), decimal_precision);
    }

    //  geometry collection
    public override bool Intersects(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(GeometryCollection3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(GeometryCollection3D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line
    public override bool Intersects(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = RefPlane();
      var plane_inter = ref_plane.Intersection(this, decimal_precision);
      if (plane_inter.ValueType != typeof(Point3D)) {
        return new IntersectionResult();
      }

      var plane_point_inter = (Point3D)plane_inter.Value;

      if (Contains(plane_point_inter, decimal_precision)) {
        return new IntersectionResult(plane_point_inter);
      }

      return new IntersectionResult();
    }
    public override bool Overlaps(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Line3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line segment
    public override bool Intersects(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(LineSegment3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = RefPlane();
      var plane_inter = ref_plane.Intersection(other, decimal_precision);
      if (plane_inter.ValueType != typeof(Point3D)) {
        return new IntersectionResult();
      }

      var plane_point_inter = (Point3D)plane_inter.Value;

      if (Contains(plane_point_inter, decimal_precision)) {
        return new IntersectionResult(plane_point_inter);
      }

      return new IntersectionResult();
    }
    public override bool Overlaps(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  line segment set
    public override bool Intersects(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(LineSegmentSet3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override bool Overlaps(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(LineSegmentSet3D other,
                                               int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polygon
    public override bool Intersects(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var plane1 = RefPlane();
      var plane2 = other.RefPlane();

      // check if planes intersect into a line
      var plane_inter = plane1.Intersection(plane2, decimal_precision);
      if (plane_inter.ValueType != typeof(Line3D)) {
        return new IntersectionResult();
      }
      var plane_line = (Line3D)plane_inter.Value;

      // check if the line intersection of the two planes passes through both polygons
      var mline1 = new List<LineSegment3D>();
      {
        //    intersection with polygon 1, in 2D
        var line_inter_2D_on_poly1 = Line2D.FromPoints(plane1.ProjectInto(plane_line.Origin),
                                                       plane1.ProjectInto(plane_line.Origin + plane_line.Direction),
                                                       decimal_precision);
        var poly1_2D = new Polygon2D(Vertices.Select(v => plane1.ProjectInto(v)));

        var inter_on_poly1_2D = poly1_2D.Intersection(line_inter_2D_on_poly1, decimal_precision);
        if (inter_on_poly1_2D.ValueType == typeof(NullValue)) {
          return new IntersectionResult();
        }

        if (inter_on_poly1_2D.ValueType == typeof(LineSegment2D)) {
          var inter_on_poly1_3D = (LineSegment2D)inter_on_poly1_2D.Value;
          mline1.Add(LineSegment3D.FromPoints(plane1.Evaluate(inter_on_poly1_3D.P0),
                                              plane1.Evaluate(inter_on_poly1_3D.P1),
                                              decimal_precision));
        } else if (inter_on_poly1_2D.ValueType == typeof(LineSegmentSet2D)) {
          foreach (var ml_2D in (LineSegmentSet2D)inter_on_poly1_2D.Value) {
            mline1.Add(
                LineSegment3D.FromPoints(plane1.Evaluate(ml_2D.P0), plane1.Evaluate(ml_2D.P1), decimal_precision));
          }
        }
      }
      if (mline1.Count == 0) {
        return new IntersectionResult();
      }

      // check if the line intersection of the two planes passes through both polygons
      var mline2 = new List<LineSegment3D>();
      {
        //    intersection with polygon 1, in 2D
        var line_inter_2D_on_poly2 = Line2D.FromPoints(plane2.ProjectInto(plane_line.Origin),
                                                       plane2.ProjectInto(plane_line.Origin + plane_line.Direction),
                                                       decimal_precision);
        var poly2_2D = new Polygon2D(other.Vertices.Select(v => plane2.ProjectInto(v)));
        var inter_on_poly2_2D = poly2_2D.Intersection(line_inter_2D_on_poly2, decimal_precision);
        if (inter_on_poly2_2D.ValueType == typeof(NullValue)) {
          return new IntersectionResult();
        }

        if (inter_on_poly2_2D.ValueType == typeof(LineSegment2D)) {
          var inter_on_poly2_3D = (LineSegment2D)inter_on_poly2_2D.Value;
          mline2.Add(LineSegment3D.FromPoints(plane2.Evaluate(inter_on_poly2_3D.P0),
                                              plane2.Evaluate(inter_on_poly2_3D.P1),
                                              decimal_precision));
        } else if (inter_on_poly2_2D.ValueType == typeof(LineSegmentSet2D)) {
          foreach (var ml_2D in (LineSegmentSet2D)inter_on_poly2_2D.Value) {
            mline2.Add(
                LineSegment3D.FromPoints(plane2.Evaluate(ml_2D.P0), plane2.Evaluate(ml_2D.P1), decimal_precision));
          }
        }
      }
      if (mline2.Count == 0) {
        return new IntersectionResult();
      }

      var mlines = new List<LineSegment3D>();
      foreach (var ml1 in mline1) {
        foreach (var ml2 in mline2) {
          var ml_ovp = ml1.Overlap(ml2, decimal_precision);
          if (ml_ovp.ValueType != typeof(LineSegment3D)) {
            continue;
          }
          mlines.Add((LineSegment3D)ml_ovp.Value);
        }
      }
      if (mlines.Count == 0) {
        return new IntersectionResult();
      }

      return new IntersectionResult(new LineSegmentSet3D(mlines));
    }
    public override bool Overlaps(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  polyline
    public override bool Intersects(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Polyline3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = RefPlane();
      var plane_inter = ref_plane.Intersection(other, decimal_precision);

      if (plane_inter.ValueType == typeof(NullValue)) {
        return new IntersectionResult();
      }

      if (plane_inter.ValueType == typeof(Point3D)) {
        var plane_point_inter = (Point3D)plane_inter.Value;

        if (Contains(plane_point_inter, decimal_precision)) {
          return new IntersectionResult(plane_point_inter);
        }
        return new IntersectionResult();
      }

      if (plane_inter.ValueType == typeof(PointSet3D)) {
        var inter_set = (PointSet3D)plane_inter.Value;

        var mpoint = inter_set.Where(p => Contains(p, decimal_precision));

        if (mpoint.Count() == 0) {
          return new IntersectionResult();
        }

        return new IntersectionResult(new PointSet3D(mpoint));
      }

      throw new ArithmeticException("unknown intsection type of polygon to polyline: " +
                                    plane_inter.ValueType.ToString());
    }
    public override bool Overlaps(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  ray
    public override bool Intersects(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Intersection(other, decimal_precision).ValueType != typeof(NullValue);
    public override IntersectionResult Intersection(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      var ref_plane = RefPlane();
      var plane_inter = ref_plane.Intersection(other, decimal_precision);
      if (plane_inter.ValueType != typeof(Point3D)) {
        return new IntersectionResult();
      }

      var plane_point_inter = (Point3D)plane_inter.Value;

      if (Contains(plane_point_inter, decimal_precision)) {
        return new IntersectionResult(plane_point_inter);
      }

      return new IntersectionResult();
    }
    public override bool Overlaps(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    //  triangle
    public override bool Intersects(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Intersection(
        Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) => throw new NotImplementedException("");
    public override bool Overlaps(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");
    public override IntersectionResult Overlap(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        throw new NotImplementedException("");

    // own functions
    public Plane RefPlane() => Plane.FromPointAndNormal(Vertices[0], Normal);

    public double Area() {
      var plane = Plane.FromPointAndNormal(Vertices[0], Normal);

      // transform the problem into a 2D one (a polygon is a planar geometry after all)
      return new Polygon2D(Vertices.Select(v => plane.ProjectInto(v))).Area();
    }

    public Point3D CenterOfMass() {
      var plane = Plane.FromPointAndNormal(Vertices[0], Normal);

      // transform the problem into a 2D one (a polygon is a planar geometry after all)
      return plane.Evaluate(new Polygon2D(Vertices.Select(v => plane.ProjectInto(v))).CenterOfMass());
    }

    public LineSegmentSet3D ToSegments(int decimal_precision = Constants.THREE_DECIMALS) {
      var lineset = new List<LineSegment3D>();

      for (int i = 0; i < Vertices.Count; i++) {
        int j = (1 + i) % Vertices.Count;
        lineset.Add(LineSegment3D.FromPoints(Vertices[i], Vertices[j], decimal_precision));
      }

      return new LineSegmentSet3D(lineset);
    }

    public List<Triangle3D> Triangulate() {
      // TODO: add this function

      if (Vertices.Count == 3) {
        return new List<Triangle3D> { Triangle3D.FromPoints(Vertices[0], Vertices[1], Vertices[2]) };
      }

      throw new NotImplementedException("triangulation not implemented yet");
    }

    public (Point3D Min, Point3D Max)
        BoundingBox() => (new Point3D(Vertices.Min(v => v.X), Vertices.Min(v => v.Y), Vertices.Min(v => v.Z)),
                          new Point3D(Vertices.Max(v => v.X), Vertices.Max(v => v.Y), Vertices.Max(v => v.Z)));

    public static Plane ApproxPlane(IEnumerable<Point3D> points, int decimal_precision = Constants.THREE_DECIMALS) {
      var point_list = points.ToList().RemoveDuplicates(decimal_precision).RemoveCollinearPoints(decimal_precision);
      var center = point_list.Average();
      int n = point_list.Count();
      var avg_norm_vec = new Vector(new double[] { 0, 0, 0 });
      int divisor = n;
      for (int i = 0; i <= n; i++) {
        (int i1, int i2) = (i % n, (i + 1) % n);
        var plane = Plane.TryFromPoints(center, point_list[i1], point_list[i2], decimal_precision);
        if (plane is null) {
          ++i;
          --divisor;
        } else {
          var norm = plane.Normal;
          bool is_norm_negative = (Math.Round(norm.Z, decimal_precision) < 0);
          avg_norm_vec +=
              norm.ToVector() *
              (is_norm_negative ? -1 : 1);  // force all normals to point up! we must have a CCW sorted set of points
        }
      }

      if (divisor < 3) {
        throw new ArgumentException(
            "ApproxPlane: too many duplicate points (less than 3 unique points with decimal_precision=" +
            decimal_precision.ToString());
      }
      avg_norm_vec /= divisor;
      return Plane.FromPointAndNormal(center, Vector3D.FromVector(avg_norm_vec).Normalize(), decimal_precision);
    }

    /// <summary>
    /// Sorts a list of points in CCW order and creates a polygon out of it
    /// If the list of points is not all lying on a planar surface, this planar surface will be approximated by
    /// the average plane crossing all points
    /// </summary>
    /// <param name="points">any enumeration of 3D Points</param>
    /// <returns></returns>
    public static Polygon3D ConcaveHull(IEnumerable<Point3D> points) {
      if (points.Count() < 3) {
        throw new ArgumentException("tried to create a Concave Hull with less than 3 points");
      }

      var plane = ApproxPlane(points);

      var poly_2d = Polygon2D.ConcaveHull(points.Select(p => plane.ProjectInto(p)));

      return (poly_2d is null) ? null : new Polygon3D(poly_2d.Select(p => plane.Evaluate(p)));
    }

    /// <summary>
    /// Computes the convex hull of any point enumeration
    /// If the list of points is not all lying on a planar surface, this planar surface will be approximated by
    /// the average plane crossing all points
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static Polygon3D ConvexHull(List<Point3D> points, int decimal_precision = Constants.THREE_DECIMALS) {
      if (points.Count < 3) {
        throw new ArgumentException("tried to create a Convex Hull with less than 3 points");
      }

      var plane = ApproxPlane(points, decimal_precision);

      var poly_2d = Polygon2D.ConvexHull(points.Select(p => plane.ProjectInto(p)).ToList(), decimal_precision);

      return (poly_2d is null) ? null : new Polygon3D(poly_2d.Select(p => plane.Evaluate(p)), decimal_precision);
    }
  }
}
