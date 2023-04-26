using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections;

namespace GeomSharp.Algebra {
  /// <summary>
  /// A Mathematical Matrix containing any number of double
  /// </summary>
  [Serializable]
  public class Matrix : IEquatable<Matrix>, ISerializable {
    private double[,] _values;
    public readonly int Rows;
    public readonly int Columns;

    /// <summary>
    /// Initialize with any enumerable where the are rows, and for each row list a set of column values
    /// For example
    ///         var m = new Matrix(new List<List<double>>{{1,2,3}, {4,5,6}}")
    /// </summary>
    /// <param name="enumd"></param>
    /// <exception cref="ArgumentException"></exception>
    public Matrix(IEnumerable<IEnumerable<double>> enumd) {
      Rows = enumd.Count();
      if (Rows <= 0) {
        throw new ArgumentException("init vector with Rows <= 0");
      }
      Columns = enumd.First().Count();
      if (Columns <= 0) {
        throw new ArgumentException("init vector with Rows <= 0");
      }
      // check columns are all the same!
      foreach (var col in enumd) {
        if (col.Count() != Columns) {
          throw new ArgumentException("columns do not have the same size");
        }
      }

      _values = new double[Rows, Columns];

      (int i, int j) = (0, 0);
      foreach (IEnumerable<double> row in enumd) {
        foreach (double col_val in row) {
          _values[i, j] = col_val;
          ++j;
        }
        ++i;
      }
    }

    public Matrix(int n, int m) {
      Rows = n;
      Columns = m;
      if (Rows <= 0) {
        throw new ArgumentException("init vector with Rows <= 0");
      }
      if (Columns <= 0) {
        throw new ArgumentException("init vector with Cols <= 0");
      }

      _values = new double[Rows, Columns];
      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Columns; j++) {
          _values[i, j] = 0;
        }
      }
    }

    public double this[int i, int j] {
      // IndexOutOfRangeException already managed by the List class
      get {
        return _values[i, j];
      }
      set {
        _values[i, j] = value;
      }
    }

    public Matrix Clone() => new Matrix(this);

    public double[,] ToArray() => _values;

    public List<List<double>> ToList() {
      var l1 = new List<List<double>>();
      for (int i = 0; i < Rows; ++i) {
        var l2 = new List<double>();
        for (int j = 0; j < Columns; ++j) {
          l2.Add(_values[i, j]);
        }
        l1.Add(l2);
      }
      return l1;
    }

    private Matrix(Matrix cp) {
      Rows = cp.Rows;
      Columns = cp.Columns;
      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Columns; j++) {
          _values[i, j] = cp._values[i, j];
        }
      }
    }

    // unary methods

    public Vector GetRow(int i) {
      if (i < 0 || i >= Rows) {
        throw new ArgumentException("index out of bounds");
      }

      var row = new Vector(Columns);
      for (int j = 0; j < Columns; ++j) {
        row[j] = _values[i, j];
      }

      return row;
    }

    public Vector GetColumn(int j) {
      if (j < 0 || j >= Columns) {
        throw new ArgumentException("index out of bounds");
      }

      var col = new Vector(Columns);
      for (int i = 0; i < Rows; ++i) {
        col[i] = _values[i, j];
      }

      return col;
    }

    public List<Vector> GetRows() {
      var rows = new List<Vector>();
      for (int i = 0; i < Rows; ++i) {
        rows.Add(GetRow(i));
      }
      return rows;
    }

    public List<Vector> GetColumns() {
      var cols = new List<Vector>();
      for (int j = 0; j < Columns; ++j) {
        cols.Add(GetColumn(j));
      }
      return cols;
    }

    private bool IsSameSize(Matrix other) => Rows == other.Rows && Columns == other.Columns;

    private bool CanMultiply(Matrix other) => Rows == other.Columns && Columns == other.Rows;

    /// <summary>
    /// Equality check with custom tolerance adjustment
    /// </summary>
    /// <param name="other"></param>
    /// <param name="decimal_precision"></param>
    /// <returns></returns>
    public bool AlmostEquals(Matrix other, int decimal_precision = Constants.THREE_DECIMALS) {
      if (other is null) {
        return false;
      }
      if (!IsSameSize(other)) {
        return false;
      }

      for (int i = 0; i < Rows; ++i) {
        for (int j = 0; j < Columns; ++j) {
          if (Math.Round(_values[i, j] - other._values[i, j], decimal_precision) != 0) {
            return false;
          }
        }
      }
      return true;
    }

    public bool Equals(Matrix other) => this.AlmostEquals(other);

    public override bool Equals(object other) => other != null && other is Matrix && this.Equals((Matrix)other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Matrix a, Matrix b) {
      return a.AlmostEquals(b);
    }

    public static bool operator !=(Matrix a, Matrix b) {
      return !a.AlmostEquals(b);
    }

    // binary operations

    // arithmetics with Matrixs
    public static Matrix operator +(Matrix a, Matrix b) {
      if (!a.IsSameSize(b)) {
        throw new ArgumentException("operator+ on matrices of different size");
      }

      var v = new Matrix(a.Rows, a.Columns);
      for (int i = 0; i < v.Rows; ++i) {
        for (int j = 0; j < v.Columns; ++j) {
          v._values[i, j] = a._values[i, j] + b._values[i, j];
        }
      }
      return v;
    }

    public static Matrix operator +(Matrix a, double k) {
      var v = new Matrix(a.Rows, a.Columns);
      for (int i = 0; i < v.Rows; ++i) {
        for (int j = 0; j < v.Columns; ++j) {
          v._values[i, j] = a._values[i, j] + k;
        }
      }
      return v;
    }

    public static Matrix operator -(Matrix a, double k) => a + (-k);

    public static Matrix operator -(Matrix a, Matrix b) => a + (-b);

    public static Matrix operator -(Matrix a) {
      var v = new Matrix(a.Rows, a.Columns);
      for (int i = 0; i < v.Rows; ++i) {
        for (int j = 0; j < v.Columns; ++j) {
          v._values[i, j] *= -1;
        }
      }
      return v;
    }

    public static Matrix operator*(Matrix a, Matrix b) => a.DotProduct(b);

    public static Vector operator*(Matrix m, Vector v) => m.DotProduct(v);

    public static Matrix operator*(Matrix a, double k) {
      var v = new Matrix(a.Rows, a.Columns);
      for (int i = 0; i < v.Rows; ++i) {
        for (int j = 0; j < v.Columns; ++j) {
          v._values[i, j] *= k;
        }
      }
      return v;
    }

    public static Matrix operator*(double k, Matrix b) => b * k;

    public static Matrix operator /(Matrix a,
                                    double k) => a * (1 / k);  // DivisionByZeroException already managed internally

    public Matrix DotProduct(Matrix other) {
      if (!CanMultiply(other)) {
        throw new ArgumentException("DotProduct on matrices of different size");
      }

      var v = new Matrix(Rows, other.Columns);
      for (int i = 0; i < Rows; ++i) {
        var row = GetRow(i);
        for (int j = 0; j < Columns; ++j) {
          var col = other.GetColumn(j);
          v._values[i, j] = row.DotProduct(col);
        }
      }
      return v;
    }

    // special formatting
    public override string ToString() => ToWkt();
    public string ToWkt(int precision = Constants.THREE_DECIMALS) {
      var mlist = ToList();
      return "MATRIX (" +
             string.Join(
                 ",",
                 mlist.Select(
                     r => "(" +
                          string.Join(
                              ",",
                              r.Select(c => string.Format(String.Format("{0}0:F{1:D}{2}", "{", precision, "}"), c))) +
                          ")")) +
             ")";
    }

    // serialization functions
    // Implement this method to serialize data. The method is called on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("rows", Rows, typeof(int));
      info.AddValue("cols", Columns, typeof(int));
      info.AddValue("values", _values, typeof(double[,]));
    }
    // The special constructor is used to deserialize values.
    public Matrix(SerializationInfo info, StreamingContext context) {
      // Reset the property value using the GetValue method.
      Rows = (int)info.GetValue("rows", typeof(int));
      Columns = (int)info.GetValue("cols", typeof(int));
      _values = (double[,])info.GetValue("values", typeof(double[,]));
    }

    public static Matrix FromBinary(string file_path) {
      try {
        var fs = new FileStream(file_path, FileMode.Open);
        var output = (Matrix)(new BinaryFormatter().Deserialize(fs));
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
