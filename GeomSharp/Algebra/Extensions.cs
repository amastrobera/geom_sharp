using System;
using System.Linq;
using System.Collections.Generic;

namespace GeomSharp.Algebra {

  public static class Extensions {
    // equality

    public enum EqualityMethod { SUM_OF_SQUARES, BY_MEMBERS }

    // algaebric operations

    public static Vector DotProduct(this Vector v, Matrix m) {
      if (v.Size != m.Rows) {
        throw new ArgumentException("vector and matrix not size correctly for dot product");
      }

      return new Vector(m.GetColumns().Select(col => v.DotProduct(col)));
    }

    public static Vector DotProduct(this Matrix m, Vector v) {
      if (v.Size != m.Columns) {
        throw new ArgumentException("vector and matrix not size correctly for dot product");
      }

      return new Vector(m.GetRows().Select(row => row.DotProduct(v)));
    }
  }

}
