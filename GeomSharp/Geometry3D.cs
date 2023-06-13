using System;
using System.IO;
using System.Linq;
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

    public void ToFile(string wkt_file_path,
                       int decimal_precision = Constants.THREE_DECIMALS) => File.WriteAllText(wkt_file_path,
                                                                                              ToWkt(decimal_precision));

    public static Geometry3D FromWkt(string wkt) {
      string known_wkt_string = "";
      string known_empty = "EMPTY";

      try {
        // Point3D
        known_wkt_string = "POINT";
        if (wkt.StartsWith(known_wkt_string, StringComparison.InvariantCultureIgnoreCase)) {
          if (wkt.Trim() == known_wkt_string + " " + known_empty) {
            return null;
          }

          var data = FromWktPointListBlock(StripWktFromBrackets(wkt));
          if (data.Length > 1) {
            throw new Exception("more than one point");
          }
          var numbers = data[0];
          return new Point3D(numbers[0], numbers[1], numbers[2]);
        }

        // LineSegment3D
        // Polyline3D
        known_wkt_string = "LINESTRING";
        if (wkt.StartsWith(known_wkt_string, StringComparison.InvariantCultureIgnoreCase)) {
          if (wkt.Trim() == known_wkt_string + " " + known_empty) {
            return null;
          }

          var data = FromWktPointListBlock(StripWktFromBrackets(wkt));
          if (data.Length < 2) {
            throw new Exception("less than one point");
          }

          if (data.Length == 2) {
            // TODO: assess the file's decimal precision
            return LineSegment3D.FromPoints(new Point3D(data[0][0], data[0][1], data[0][2]),
                                            new Point3D(data[1][0], data[1][1], data[1][2]));
          }

          if (data.Length > 2) {
            // TODO: assess the file's decimal precision
            return new Polyline3D(data.Select(d => new Point3D(d[0], d[1], d[2])));
          }
        }

        // Line3D
        known_wkt_string = "LINE";
        if (wkt.StartsWith(known_wkt_string, StringComparison.InvariantCultureIgnoreCase)) {
          if (wkt.Trim() == known_wkt_string + " " + known_empty) {
            return null;
          }

          var geoms = StripWktFromBrackets(wkt).Split(',');
          if (geoms.Length != 2) {
            throw new Exception("number of geometries != 2");
          }

          (var geom1, var geom2) = (FromWkt(geoms[0]), FromWkt(geoms[1]));

          if (geom1 is Point3D && geom2 is null) {
            var point = (geom1 as Point3D);
            var null_geom = geoms[1];
            if (null_geom.StartsWith("VECTOR", StringComparison.InvariantCultureIgnoreCase)) {
              var vec_data = FromWktPointBlock(StripWktFromBrackets(null_geom));
              return Line3D.FromDirection(point, (new Vector3D(vec_data[0], vec_data[1], vec_data[2]).Normalize()));
            }
            throw new Exception("unexpected second geometry type: " + null_geom);
          }

          if (geom1 is null && geom2 is Point3D) {
            var point = (geom2 as Point3D);
            var null_geom = geoms[0];
            if (null_geom.StartsWith("VECTOR", StringComparison.InvariantCultureIgnoreCase)) {
              var vec_data = FromWktPointBlock(StripWktFromBrackets(null_geom));
              return Line3D.FromDirection(point, (new Vector3D(vec_data[0], vec_data[1], vec_data[2]).Normalize()));
            }
            throw new Exception("unexpected first geometry type: " + null_geom);
          }

          throw new Exception("unexpected pair of geometries: " + string.Join(",", geoms));
        }

        // Ray3D
        known_wkt_string = "RAY";
        if (wkt.StartsWith(known_wkt_string, StringComparison.InvariantCultureIgnoreCase)) {
          if (wkt.Trim() == known_wkt_string + " " + known_empty) {
            return null;
          }

          var geoms = StripWktFromBrackets(wkt).Split(',');
          if (geoms.Length != 2) {
            throw new Exception("number of geometries != 2");
          }

          (var geom1, var geom2) = (FromWkt(geoms[0]), FromWkt(geoms[1]));

          if (geom1 is Point3D && geom2 is null) {
            var point = (geom1 as Point3D);
            var null_geom = geoms[1];
            if (null_geom.StartsWith("VECTOR", StringComparison.InvariantCultureIgnoreCase)) {
              var vec_data = FromWktPointBlock(StripWktFromBrackets(null_geom));
              return new Ray3D(point, (new Vector3D(vec_data[0], vec_data[1]).Normalize()));
            }
            throw new Exception("unexpected second geometry type: " + null_geom);
          }

          if (geom1 is null && geom2 is Point3D) {
            var point = (geom2 as Point3D);
            var null_geom = geoms[0];
            if (null_geom.StartsWith("VECTOR", StringComparison.InvariantCultureIgnoreCase)) {
              var vec_data = FromWktPointBlock(StripWktFromBrackets(null_geom));
              return new Ray3D(point, (new Vector3D(vec_data[0], vec_data[1]).Normalize()));
            }
            throw new Exception("unexpected first geometry type: " + null_geom);
          }

          throw new Exception("unexpected pair of geometries: " + string.Join(",", geoms));
        }

        // Triangle3D
        known_wkt_string = "TRIANGLE";
        if (wkt.StartsWith(known_wkt_string, StringComparison.InvariantCultureIgnoreCase)) {
          if (wkt.Trim() == known_wkt_string + " " + known_empty) {
            return null;
          }

          var data = FromWktPointListBlock(StripWktFromBrackets(wkt));
          if (data.Length != 3) {
            throw new Exception("not 3 points");
          }

          // TODO: assess decimal precision
          return Triangle3D.FromPoints(new Point3D(data[0][0], data[0][1], data[0][2]),
                                       new Point3D(data[1][0], data[1][1], data[1][2]),
                                       new Point3D(data[2][0], data[2][1], data[2][2]));
        }

        // Polygon3D
        // Triangle3D
        known_wkt_string = "POLYGON";
        if (wkt.StartsWith(known_wkt_string, StringComparison.InvariantCultureIgnoreCase)) {
          if (wkt.Trim() == known_wkt_string + " " + known_empty) {
            return null;
          }

          var data = FromWktPolygonSetBlock(StripWktFromBrackets(wkt));
          if (data.Length == 1 && data[0].Length == 3) {
            // it's a triangle
            // TODO: assess decimal precision
            return Triangle3D.FromPoints(new Point3D(data[0][0][0], data[0][0][1], data[0][0][2]),
                                         new Point3D(data[0][1][0], data[0][1][1], data[0][1][2]),
                                         new Point3D(data[0][2][0], data[0][2][1], data[0][2][2]));
          }

          if (data.Length > 1) {
            // TODO: make a loop over data, to add the holes
            throw new NotSupportedException("polygon holes not yet supported");
          }

          // TODO: assess decimal precision
          return new Polygon3D(data[0].Select(d => new Point3D(d[0], d[1], d[2])));
        }

        // PointSet3D
        known_wkt_string = "MULTIPOINT";
        if (wkt.StartsWith(known_wkt_string, StringComparison.InvariantCultureIgnoreCase)) {
          if (wkt.Trim() == known_wkt_string + " " + known_empty) {
            return null;
          }

          var point_blocks = StripWktFromBrackets(wkt).Split(',');
          var data = new double [point_blocks.Length][];
          for (int i = 0; i < point_blocks.Length; ++i) {
            data[i] = FromWktPointBlock(StripWktFromBrackets(point_blocks[i]));
          }

          // TODO: assess decimal precision
          return new PointSet3D(data.Select(d => new Point3D(d[0], d[1], d[2])));
        }

        // LineSegmentSet3D
        known_wkt_string = "MULTILINESTRING";
        if (wkt.StartsWith(known_wkt_string, StringComparison.InvariantCultureIgnoreCase)) {
          if (wkt.Trim() == known_wkt_string + " " + known_empty) {
            return null;
          }

          var line_blocks = StripWktFromBrackets(wkt).Split(',');
          var line_data = new double [line_blocks.Length][][];
          for (int i = 0; i < line_blocks.Length; ++i) {
            line_data[i] = FromWktPointListBlock(StripWktFromBrackets(line_blocks[i]));
          }

          // TODO: assess decimal precision
          return new LineSegmentSet3D(
              line_data.Select(data => LineSegment3D.FromPoints(new Point3D(data[0][0], data[0][1], data[0][2]),
                                                                new Point3D(data[1][0], data[1][1], data[1][2]))));
        }

        // GeometryCollection3D
        known_wkt_string = "GEOMETRYCOLLECTION";
        if (wkt.StartsWith(known_wkt_string, StringComparison.InvariantCultureIgnoreCase)) {
          if (wkt.Trim() == known_wkt_string + " " + known_empty) {
            return null;
          }
          return new GeometryCollection3D(StripWktFromBrackets(wkt).Split(',').Select(gstr => FromWkt(gstr)));
        }

      } catch (Exception ex) {
        throw new Exception("bad format " + known_wkt_string + "(3D): " + ex.Message);
      }
      return null;
    }

    public static Geometry3D FromFile(string wkt_file_path) => !File.Exists(wkt_file_path)
                                                                   ? throw new ArgumentException("file " +
                                                                                                 wkt_file_path +
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

    // private functions

    private static string StripWktFromBrackets(string wkt) {
      int first_bracket = wkt.IndexOf('(');
      if (first_bracket < 0) {
        throw new Exception("missing first bracket");
      }
      int last_bracket = wkt.LastIndexOf(')');
      if (last_bracket < 0) {
        throw new Exception("missing last bracket");
      }

      if (last_bracket <= first_bracket + 1) {
        throw new Exception("() contain no data");
      }

      return wkt.Substring(first_bracket + 1, last_bracket - first_bracket - 1);
    }

    private static double[][][] FromWktPolygonSetBlock(string wkt,
                                                       char polygon_delim = ',',
                                                       char point_delim = ',',
                                                       char num_delim = ' ',
                                                       int decimal_precision = Constants.THREE_DECIMALS) {
      var poly_blocks = wkt.Split(polygon_delim);

      var polys = new double [poly_blocks.Length][][];
      for (int i = 0; i < poly_blocks.Length; ++i) {
        polys[i] = FromWktPointListBlock(StripWktFromBrackets(poly_blocks[i]), point_delim, num_delim);

        // TODO: add decimal_precision check
        if (Math.Round(polys[i][0][0] - polys[i][polys[i].Length][0], decimal_precision) != 0 ||
            Math.Round(polys[i][0][1] - polys[i][polys[i].Length][1], decimal_precision) != 0) {
          throw new Exception("first point != last point in polygon");
        }
      }

      return polys;
    }

    private static double[][] FromWktPointListBlock(string wkt, char point_delim = ',', char num_delim = ' ') {
      var point_blocks = wkt.Split(point_delim);

      var points = new double [point_blocks.Length][];
      for (int i = 0; i < point_blocks.Length; ++i) {
        points[i] = FromWktPointBlock(point_blocks[i], num_delim);
      }

      return points;
    }

    private static double[] FromWktPointBlock(string wkt, char num_delim = ' ') {
      var numbers = wkt.Split(num_delim);

      if (numbers.Length != 3) {
        throw new Exception("no three doubles");
      }

      return new double[] { double.Parse(numbers[0]),
                            double.Parse(numbers[1]),
                            double.Parse(numbers[2]) };  // can throw too
    }
  }

}
