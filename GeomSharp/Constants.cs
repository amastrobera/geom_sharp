using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSharp {
  public static class Constants {
    /// <summary>
    /// Number of Decimals
    /// </summary>
    public const int THREE_DECIMALS = 3;
    public const int NINE_DECIMALS = 9;

    public static double THREE_DP_TOLERANCE => Math.Pow(10, -THREE_DECIMALS);
    public static double NINE_DP_TOLERANCE => Math.Pow(10, -NINE_DECIMALS);

    public enum Orientation { UNKNOWN = -1, CLOCKWISE, COUNTER_CLOCKWISE }

    public enum Location { UNKNOWN = -1, LEFT, RIGHT, ABOVE, BELOW, AHEAD, BEHIND, ON_SEGMENT, ON_LINE }
  }
}
