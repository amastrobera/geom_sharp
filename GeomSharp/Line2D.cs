using System;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GeomSharp {
  /// <summary>
  /// A Line on an arbitrary 2D plane
  /// It's either defined as an infinite straight line passing by two points
  /// or an infinite straight line passing by a point, with a given direction
  /// </summary>
  [Serializable]
  public class Line2D : IEquatable<Line2D>, ISerializable {
    public Point2D P0 { get; }
    public Point2D P1 { get; }
    public Point2D Origin { get; }
    public UnitVector2D Direction { get; }

    public static Line2D FromPoints(Point2D p0, Point2D p1, int decimal_precision = Constants.THREE_DECIMALS) =>
        p0.AlmostEquals(p1, decimal_precision)
            ? throw new NullLengthException("trying to initialize a line with two identical points")
            : new Line2D(p0, p1);

    public static Line2D FromDirection(Point2D origin, UnitVector2D direction) => new Line2D(origin, direction);

    private Line2D(Point2D p0, Point2D p1) {
      P0 = p0;
      P1 = p1;
      Origin = P0;
      Direction = (p1 - p0).Normalize();
    }

    private Line2D(Point2D origin, UnitVector2D direction) {
      Origin = origin;
      Direction = direction;
      P0 = Origin;
      P1 = Origin + 1 * Direction;
    }

    public bool AlmostEquals(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) && Direction.AlmostEquals(other.Direction, decimal_precision);

    public bool Equals(Line2D other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Line2D && this.Equals((Line2D)other);

    public override int GetHashCode() => Direction.ToWkt().GetHashCode();

    public static bool operator ==(Line2D a, Line2D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Line2D a, Line2D b) {
      return !a.AlmostEquals(b);
    }

    public bool IsParallel(Line2D other,
                           int decimal_precision = Constants.THREE_DECIMALS) => Direction.IsParallel(other.Direction,
                                                                                                     decimal_precision);

    public bool IsPerpendicular(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Direction.IsPerpendicular(other.Direction, decimal_precision);

    public bool Contains(Point2D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        Location(p, decimal_precision) == Constants.Location.ON_LINE;

    /// <summary>
    /// Projects a Point onto a line
    /// The fomula is
    /// P(b) = P0 + b*(P1-P0) = P0 + W*vl *vl
    /// where
    /// b = d(P0, Pb) / d(P0,P1) = W*Vl / Vl*Vl = W*vl / Vl
    /// W=p-P0, Vl=P1-P0, vl=Vl/|Vl| unit vector
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Point2D ProjectOnto(Point2D p) => Origin + (p - Origin).DotProduct(Direction) * Direction;

    /// <summary>
    /// Projects a Point into a line (2D to 1D change)
    /// The result is a double, indicating the distance of a point from the origin (along the line) - negative, if if
    /// preceeds the Origin, positive, if it follows the origin
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public double ProjectInto(Point2D p, int decimal_precision = Constants.THREE_DECIMALS) =>
        ((Math.Round((P1 - P1).DotProduct(p - P0), decimal_precision) < 0) ? -1 : 1) *
        ProjectOnto(p).DistanceTo(Origin);

    /// <summary>
    /// Distance from a point
    /// In 3D the formula is d(p,L) = | Vl x W | / |Vl| = |vl x W|
    /// where Vl=P1-P0, vl = Vl/|Vl|, W=p-P0, and (P0,P1) are the points of the line
    /// In 2D the formula of the cross-product is replaced by the PerpProduct, which returns a (signed) double, and must
    /// be taken as its absolute value
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public double DistanceTo(Point2D p) => Math.Abs(SignedDistanceTo(p));

    /// <summary>
    /// (Signed) distance from a point
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public double SignedDistanceTo(Point2D p) => Direction.PerpProduct(p - Origin);

    /// <summary>
    /// Tells what the location is for a Point relative to the Line, on the 2D plane
    /// Can be done in 3D too, by creating a plane of the line + point, and projecting all into it, then using this
    /// method
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Constants.Location Location(Point2D p, int decimal_precision = Constants.THREE_DECIMALS) {
      var perp_prod = Math.Round(Direction.PerpProduct(p - Origin), decimal_precision);
      if (perp_prod == 0) {
        return Constants.Location.ON_LINE;
      }
      if (perp_prod > 0) {
        return Constants.Location.LEFT;
      }
      return Constants.Location.RIGHT;
    }

    /// <summary>
    /// Tells if two lines intersect (one of them cuts throw the other, splitting it in two)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Intersects(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      return !IsParallel(other, decimal_precision);  // in 2D you only have two chances: parallel or intersecting
    }

    /// <summary>
    /// If two lines intersect, this return the point in which one of the is stroke through
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Intersection(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!Intersects(other, decimal_precision)) {
        return new IntersectionResult();
      }

      // TODO: can be put this code in a common place, and avoid duplicating it over and over ?
      var V = other.Direction;
      var U = Direction;
      var W = Origin - other.Origin;

      double sI = -V.PerpProduct(W) / V.PerpProduct(U);  // guaranteed non-zero if non-parallel

      var Ps = Origin + sI * Direction;
      if (!(Contains(Ps, decimal_precision) && other.Contains(Ps, decimal_precision))) {
        throw new Exception(String.Format("Intersection({0}) miscalculated Ps", GetType().ToString().ToString()));
      }

      return new IntersectionResult(Ps);
    }

    /// <summary>
    /// Tells if two lines overlap (they share a common section)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (!IsParallel(other, decimal_precision)) {
        return false;
      }
      if (Contains(other.Origin, decimal_precision)) {
        return true;
      }
      return false;
    }

    /// <summary>
    /// If two lines overlap, this function returns the shared section between them
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public IntersectionResult Overlap(Line2D other, int decimal_precision = Constants.THREE_DECIMALS) =>
        Overlaps(other, decimal_precision) ? new IntersectionResult(this) : new IntersectionResult();

    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      (var p1, var p2) = (Origin - 2 * Direction, Origin + 2 * Direction);
      return "GEOMETRYCOLLECTION (" +

             "POINT (" +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"), Origin.U, Origin.V) +
             ")"

             + "," +

             "LINESTRING (" +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"), p1.U, p1.V) + "," +
             string.Format(String.Format("{0}0:F{1:D}{2} {0}1:F{1:D}{2}", "{", precision, "}"), p2.U, p2.V) + ")" +

             ")";
    }

    // serialization functions
    // Implement this method to serialize data. The method is called on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("P0", P0, typeof(Point2D));
      info.AddValue("P1", P1, typeof(Point2D));
      info.AddValue("Origin", Origin, typeof(Point2D));
      info.AddValue("Direction", Direction, typeof(UnitVector2D));
    }
    // The special constructor is used to deserialize values.
    public Line2D(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      P0 = (Point2D)info.GetValue("P0", typeof(Point2D));
      P1 = (Point2D)info.GetValue("P1", typeof(Point2D));
      Origin = (Point2D)info.GetValue("Origin", typeof(Point2D));
      Direction = (UnitVector2D)info.GetValue("Direction", typeof(UnitVector2D));
    }

    public static Line2D FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Line2D)(new BinaryFormatter().Deserialize(fs));
        return output;
      } catch (Exception e) {
        // warning failed to deserialize
      }
      return null;
    }

    public void ToBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Create);
        (new BinaryFormatter()).Serialize(fs, this);
        fs.Close();
      } catch (Exception e) {
        // warning failed to deserialize
      }
    }
  }

}
