using System;

namespace GeomSharp.Utils {
  public static class Format {
    public static String FormatPrecision1D(int precision) {
      return String.Format("{0}0:F{1:D}{2}", "{", precision, "}");
    }

    public static String FormatPrecision2D(int precision, string separator = " ") {
      return String.Format("{0}0:F{1:D}{2}{3}{0}1:F{1:D}{2}", "{", precision, "}", separator);
    }

    public static String FormatPrecision3D(int precision, string separator = " ") {
      return String.Format("{0}0:F{1:D}{2}{3}{0}1:F{1:D}{2}{3}{0}2:F{1:D}{2}", "{", precision, "}", separator);
    }
  }
}
