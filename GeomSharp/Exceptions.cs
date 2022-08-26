using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp {
  public class NullLengthException : Exception {
    public NullLengthException(string message = "") : base(message) {}
  }
}
