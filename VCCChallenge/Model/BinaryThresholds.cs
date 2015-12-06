// Copyright 2015 Thomas Newman

namespace VCCChallenge
{
    /// <summary>
    /// Represents Binary Thresholds used for contour extraction.
    /// </summary>
    class BinaryThresholds
    {
        public BinaryThresholds()
        {
            this.HueMin = 0;
            this.HueMax = 0;
            this.SatMin = 0;
            this.SatMax = 0;
            this.ValMin = 0;
            this.ValMax = 0;
        }

        /// <summary>
        /// Minimum Hue
        /// </summary>
        public int HueMin { get; set; }

        /// <summary>
        /// Maximum Hue
        /// </summary>
        public int HueMax { get; set; }

        /// <summary>
        /// Minimum Saturation
        /// </summary>
        public int SatMin { get; set; }

        /// <summary>
        /// Maximum Saturation
        /// </summary>
        public int SatMax { get; set; }

        /// <summary>
        /// Minimum Value
        /// </summary>
        public int ValMin { get; set; }

        /// <summary>
        /// Maximum Value
        /// </summary>
        public int ValMax { get; set; }
    }
}
