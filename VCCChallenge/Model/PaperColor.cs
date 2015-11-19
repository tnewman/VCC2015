using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCChallenge
{
    public enum PaperColor
    {
        UKNOWN,
        YELLOW,
        GREEN
    }

    public static class PaperColorUtils
    {
        public static string PaperColorToString(PaperColor paperColor)
        {
            switch(paperColor)
            {
                case PaperColor.UKNOWN:
                    return "Unknown";
                case PaperColor.YELLOW:
                    return "Yellow";
                case PaperColor.GREEN:
                    return "Green";
                default:
                    return "Invalid Enum Value";
            }
        }
    }
}
