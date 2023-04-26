using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp {

  /// <summary>
  /// A Mathematical Vector of N-dimentions
  /// </summary>
  public class Angle : IEquatable<Angle> {
    private double _Radians;

    public double Radians {
      get { return _Radians; }
    }

    public double Degrees {
      get { return 180 * _Radians / Math.PI; }
    }

    public Angle(Angle a) => (_Radians) = (a.Radians);

    private Angle(double rads) => (_Radians) = (rads);

    public static Angle FromRadians(double rads) {
      return new Angle(rads);
    }

    public static Angle FromDegrees(double degs) {
      return new Angle(Math.PI * degs / 180);
    }

    public bool AlmostEquals(Angle other, int decimal_precision = Constants.THREE_DECIMALS) =>
        !(other is null) && Math.Round(_Radians - other.Radians, decimal_precision) == 0;

    public bool Equals(Angle other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Angle && this.Equals((Angle)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Angle a, Angle b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Angle a, Angle b) {
      return !a.AlmostEquals(b);
    }

    public string ToWkt(int precision = Constants.THREE_DECIMALS) =>
        string.Format("ANGLE (" + String.Format("{0}0:F{1:D}{2}", "{", precision, "}") + "o)", Degrees);

    public override string ToString() {
      return "ANGLE (" + Degrees + "o)";
    }

    // binary operations, operations between two vectors, or vectors and scalars

    public static Angle operator +(Angle a, Angle b) => new Angle(a.Radians + b.Radians);

    public static Angle operator -(Angle a, Angle b) => new Angle(a.Radians - b.Radians);

    public static Angle operator*(Angle a, double k) => new Angle(a.Radians * k);

    public static Angle operator*(double k, Angle a) => new Angle(a.Radians * k);

    public static Angle operator /(Angle a, double k) =>
        (Math.Round(k, Constants.THREE_DECIMALS) != 0)
            ? new Angle(a.Radians / k)
            : throw new ArithmeticException("Vector.operator/(double) division by zero");

    public static bool operator<(Angle a, Angle b) => Math.Round(a.Radians - b.Radians, Constants.THREE_DECIMALS) < 0;

    public static bool operator>(Angle a, Angle b) => Math.Round(a.Radians - b.Radians, Constants.THREE_DECIMALS) > 0;
  }
}
