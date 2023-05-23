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
  }

}
