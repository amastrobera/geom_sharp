using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GeomSharp {
  /// <summary>
  /// A base class for all geometrical objects
  /// </summary>
  [Serializable]
  public abstract class Geometry2D : IEquatable<Geometry2D>, ISerializable {
    // generic overrides from object class
    public override string ToString() => base.ToString();
    public override int GetHashCode() => base.GetHashCode();

    // comparison, and adjusted comparison
    public override bool Equals(object other) => other != null && other is Geometry2D && this.Equals((Geometry2D)other);
    public abstract bool Equals(Geometry2D other);

    public abstract bool AlmostEquals(Geometry2D other, int decimal_precision = Constants.THREE_DECIMALS);

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

    public abstract Geometry2D FromWkt(string wkt);

    public Geometry2D FromFile(string wkt_file_path) => !File.Exists(wkt_file_path)
                                                            ? throw new ArgumentException("file " + wkt_file_path +
                                                                                          " does now exist")
                                                            : FromWkt(File.ReadAllText(wkt_file_path));

    // relationship to all the other geometries
    // point
    public abstract bool Contains(Point2D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  geometry collection
    public abstract bool Intersects(GeometryCollection2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(GeometryCollection2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(GeometryCollection2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(GeometryCollection2D other,
                                               int decimal_precision = Constants.THREE_DECIMALS);

    //  line
    public abstract bool Intersects(Line2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Line2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Line2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Line2D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  line segment
    public abstract bool Intersects(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(LineSegment2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(LineSegment2D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  line segment set
    public abstract bool Intersects(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(LineSegmentSet2D other,
                                                    int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(LineSegmentSet2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(LineSegmentSet2D other,
                                               int decimal_precision = Constants.THREE_DECIMALS);

    //  polygon
    public abstract bool Intersects(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Polygon2D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  polyline
    public abstract bool Intersects(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Polyline2D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  ray
    public abstract bool Intersects(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Ray2D other, int decimal_precision = Constants.THREE_DECIMALS);

    //  triangle
    public abstract bool Intersects(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Intersection(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract bool Overlaps(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS);
    public abstract IntersectionResult Overlap(Triangle2D other, int decimal_precision = Constants.THREE_DECIMALS);
  }

}
