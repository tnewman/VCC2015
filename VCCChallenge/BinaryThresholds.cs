// Copyright 2015 Thomas Newman

namespace VCCChallenge
{
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

        public int HueMin { get; set; }
        public int HueMax { get; set; }
        public int SatMin { get; set; }
        public int SatMax { get; set; }
        public int ValMin { get; set; }
        public int ValMax { get; set; }
    }
}
