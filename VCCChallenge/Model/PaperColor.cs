// Copyright 2015 Thomas Newman

namespace VCCChallenge
{
    /// <summary>
    /// Possible paper colors.
    /// </summary>
    public enum PaperColor
    {
        UNKNOWN,
        YELLOW,
        GREEN
    }

    /// <summary>
    /// Utilities for paper colors.
    /// </summary>
    public static class PaperColorUtils
    {
        /// <summary>
        /// Converts a paper color to a string representation.
        /// </summary>
        /// <param name="paperColor">PaperColor to convert to string.</param>
        /// <returns></returns>
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
