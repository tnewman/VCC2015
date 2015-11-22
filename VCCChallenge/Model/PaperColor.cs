// Copyright 2015 Thomas Newman

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCChallenge
{
    public enum PaperColor
    {
        UNKNOWN,
        YELLOW,
        GREEN
    }

    public static class PaperColorUtils
    {
        public static string PaperColorToString(PaperColor paperColor)
        {
            switch(paperColor)
            {
                case PaperColor.UNKNOWN:
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
