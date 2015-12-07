// Copyright 2015 Thomas Newman

using Emgu.CV;
using Emgu.CV.Structure;

namespace VCCChallenge
{
    /// <summary>
    /// Stores a Combined HSV Image and its component images 
    /// for Hue, Saturation and Value.
    /// </summary>
    class HsvFilter
    {
        public Image<Gray, byte> HueFilter { get; set; }
        public Image<Gray, byte> SatFilter { get; set; }
        public Image<Gray, byte> ValFilter { get; set; }
        public Image<Gray, byte> CombinedFilter { get; set; }
    }
}
