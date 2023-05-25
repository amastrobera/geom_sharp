using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GeomSharp {
  /// <summary>
  /// A base class for all geometrical objects
  /// </summary>
  [Serializable]
  public abstract class Geometry3D : IEquatable<Geometry3D>, ISerializable {
    // generic overrides from object class
    public override string ToString() => base.ToString();
    public override int GetHashCode() => base.GetHashCode();

    // comparison, and adjusted comparison
    public override bool Equals(object other) => other != null && other is Geometry3D && this.Equals((Geometry3D)other);
    public abstract bool Equals(Geometry3D other);

    public abstract bool AlmostEquals(Geometry3D other, int decimal_precision = Constants.THREE_DECIMALS);

    // serialization, to binary
    public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

    public void ToBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Create);
        (new BinaryFormatter()).Serialize(fs, this);
        fs.Close();
      } catch (Exception e) {
        // warning failed to deserialize
      }
    }

    // well known text and to/from file
    public abstract string ToWkt(int precision = Constants.THREE_DECIMALS);

    public void ToFile(string wkt_file_path) => File.WriteAllText(wkt_file_path, ToWkt());

    public abstract Geometry3D FromWkt(string wkt);

    public Geometry3D FromFile(string wkt_file_path) => !File.Exists(wkt_file_path)
                                                            ? throw new ArgumentException("file " + wkt_file_path +
                                                                                          " does now exist")
                                                            : FromWkt(File.ReadAllText(wkt_file_path));

    // relationship to all the other geometries
    // plane
    public abstract bool Intersects(Plane other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Plane other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Plane other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Plane other, int decimal_precision = Constants.THREE_DECIMALS);

    // point
    public abstract bool Contains(Point3D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  geometry collection
    public abstract bool Intersects(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(GeometryCollection3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(GeometryCollection3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(GeometryCollection3D other,
                                               int decimal_precision = Constants.THREE_DECIMALS);

    //  line
    public abstract bool Intersects(Line3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Line3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Line3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Line3D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  line segment
    public abstract bool Intersects(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(LineSegment3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(LineSegment3D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  line segment set
    public abstract bool Intersects(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(LineSegmentSet3D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(LineSegmentSet3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(LineSegmentSet3D other,
                                               int decimal_precision = Constants.THREE_DECIMALS);

    //  polygon
    public abstract bool Intersects(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Polygon3D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  polyline
    public abstract bool Intersects(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  ray
    public abstract bool Intersects(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Ray3D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  triangle
    public abstract bool Intersects(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Triangle3D other, int decimal_precision = Constants.THREE_DECIMALS);
  }

}
