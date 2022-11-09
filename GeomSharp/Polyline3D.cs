using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp {
  /// <summary>
  /// A Curve made of straight lines in 3D, each line bound by a pair of vertices
  /// </summary>
  public class Polyline3D : IEquatable<Polyline3D>, IEnumerable<Point3D> {
    public List<Point3D> Nodes { get; }
    public readonly int Size;

    public Polyline3D(Point3D[] points, int decimal_precision = Constants.THREE_DECIMALS) {
      if (points.Length < 2) {
        throw new ArgumentException("tried to initialize a polyline with less than 2 points");
      }

      Nodes = (new List<Point3D>(points)).RemoveCollinearPoints(decimal_precision);
      // input adjustment: correcting mistake of passing collinear points to a polygon
      if (Nodes.Count < 2) {
        throw new ArgumentException("tried to initialize a polyline with less than 2 non-collinear points");
      }
      // set the size
      Size = Nodes.Count;
    }

    public Polyline3D(IEnumerable<Point3D> points, int decimal_precision = Constants.THREE_DECIMALS)
        : this(points.ToArray(), decimal_precision) {}

    public bool AlmostEquals(Polyline3D other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other.Size != Size) {
        return false;
      }

      if (Math.Round(Length() - other.Length(), decimal_precision) != 0) {
        return false;
      }

      for (int i = 0; i < Size; ++i) {
        if (!Nodes[i].AlmostEquals(other.Nodes[i], decimal_precision)) {
          return false;
        }
      }

      return true;
    }

    public bool Equals(Polyline3D other) => this.AlmostEquals(other);
    public override bool Equals(object other) => other != null && other is Polyline3D && this.Equals((Polyline3D)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Polyline3D a, Polyline3D b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Polyline3D a, Polyline3D b) {
      return !a.AlmostEquals(b);
    }

    public IEnumerator<Point3D> GetEnumerator() {
      return Nodes.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    public (Point3D Min, Point3D Max)
        BoundingBox() => (new Point3D(Nodes.Min(v => v.X), Nodes.Min(v => v.Y), Nodes.Min(v => v.Z)),
                          new Point3D(Nodes.Max(v => v.X), Nodes.Max(v => v.Y), Nodes.Max(v => v.Z)));

    public double Length() {
      double d = 0;
      for (int i1 = 0; i1 < Nodes.Count - 1; i1++) {
        int i2 = i1 + 1;
        d += Nodes[i2].DistanceTo(Nodes[i1]);
      }
      return d;
    }

    private int IndexOfNearestSegmentToPoint(Point3D point) {
      (int i_min, double d_min) = (-1, double.MaxValue);
      for (int i1 = 0; i1 < Nodes.Count - 1; i1++) {
        int i2 = i1 + 1;
        var d = Line3D.FromPoints(Nodes[i1], Nodes[i2]).DistanceTo(point);
        if (d < d_min) {
          d_min = d;
          i_min = i1;
        }
      }
      if (i_min < 0) {
        throw new Exception("IndexOfNearestSegmentToPoint failed to find i");
      }

      return i_min;
    }

    public Point3D ProjectOnto(Point3D p) {
      int i_nearest = IndexOfNearestSegmentToPoint(p);
      return Line3D.FromPoints(Nodes[i_nearest], Nodes[i_nearest + 1]).ProjectOnto(p);
    }

    /// <summary>
    /// Returns the relative position of a point onto a curve in % [0,1]
    /// Returns double.MaxValue and warns an error if the point does not belong to the curve at all
    /// It's in 3D, and works on a linearized version of the curve
    /// It's the twin function of GetPointOnPolyline
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public double LocationPct(Point3D point) {
      double curve_len = Length();
      double len_done = 0;
      for (int i1 = 0; i1 < Nodes.Count - 1; ++i1) {
        int i2 = i1 + 1;
        var piece_line = LineSegment3D.FromPoints(Nodes[i1], Nodes[i2]);
        double piece_len = piece_line.Length();
        if (piece_line.Contains(point)) {
          double t = Math.Round(piece_line.P0.DistanceTo(point) / Length(), Constants.NINE_DECIMALS);
          if (t >= 0 && t <= 1) {
            return (len_done + t * piece_len) / curve_len;
          }
        }
        len_done += piece_len;
      }

      // no segment of the polyline contains the point, throw
      // throw new Exception("LocationAlongTheLine no polyline segment contains the point");
      return double.MaxValue;
    }

    /// <summary>
    /// Returns the point corresponding to the %len of the curve
    /// Gives a warning and returns null in case of errors
    /// It's the twin function of LocationAlongTheLine
    /// </summary>
    /// <param name="pct">must be in the range [0, 1] of throws</param>
    /// <returns></returns>
    public Point3D GetPointOnPolyline(double pct) {
      if (Math.Round(pct, Constants.NINE_DECIMALS) < 0 || Math.Round(pct, Constants.NINE_DECIMALS) > 1) {
        throw new ArgumentException("pct should be in the range [0,1]");
      }

      double curve_len = Math.Round(Length(), Constants.NINE_DECIMALS);
      double curve_todo = Math.Round(pct * curve_len, Constants.NINE_DECIMALS);
      double len_done = 0;
      double pct_done = 0;
      int n = Nodes.Count;
      for (int i = 0; i < n - 1; ++i) {
        var piece_line = LineSegment3D.FromPoints(Nodes[i], Nodes[i + 1]);
        double piece_len = piece_line.Length();
        if (Math.Round(len_done + piece_len, Constants.NINE_DECIMALS) >= curve_todo) {
          // found segment containing the point
          double tI = ((pct - pct_done) * curve_len) / piece_len;
          var PI = piece_line.P0 + tI * (piece_line.P1 - piece_line.P0);
          return PI;
        }
        len_done += piece_len;
        pct_done = len_done / curve_len;
      }

      // string.Format("GetPointOnPolyline did not find the point on the curve for {0:F4}%", pct)
      return null;
    }

    // special formatting
    public override string ToString() => "{" + string.Join(",", Nodes.Select(v => v.ToString())) + "}";

    public string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        (Nodes.Count == 0)
            ? "LINESTRING EMPTY"
            : "LINESTRING (" +
                  string.Join(",",
                              Nodes.Select(v => string.Format(
                                               String.Format(
                                                   "{0}0:F{1:D}{2} {0}1:F{1:D}{2} {0}2:F{1:D}{2}", "{", precision, "}"),
                                               v.X,
                                               v.Y,
                                               v.Z))) +

                  ")";
  }
}
