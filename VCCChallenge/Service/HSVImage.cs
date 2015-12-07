// Copyright 2015 Thomas Newman

using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace VCCChallenge
{
    /// <summary>
    /// Hue/Saturation/Value Image Processing
    /// </summary>
    class HsvImage
    {
        /// <summary>
        /// Create a combined Hue/Saturation/Value image from an existing 
        /// image using supplied thresholds for hue, saturation and value.
        /// </summary>
        /// <param name="image">Image to convert to HSV.</param>
        /// <param name="thresholds">Thresholds for hue, saturation and value</param>
        /// <returns>HSV image along with the component images for hue, 
        /// saturation and value.</returns>
        public HsvFilter generateCombinedHSV(Image<Bgr, byte> image, BinaryThresholds thresholds)
        {
            Image<Hsv, Byte> hsvFrame = image.Convert<Hsv, Byte>();
            Image<Gray, Byte>[] channels = hsvFrame.Split();

            Image<Gray, byte> hueImage = channels[0];
            Image<Gray, byte> satImage = channels[1];
            Image<Gray, byte> valImage = channels[2];

            Image<Gray, byte> hueFilter = hueImage.InRange(new Gray(thresholds.HueMin), new Gray(thresholds.HueMax));
            Image<Gray, byte> satFilter = satImage.InRange(new Gray(thresholds.SatMin), new Gray(thresholds.SatMax));
            Image<Gray, byte> valFilter = valImage.InRange(new Gray(thresholds.ValMin), new Gray(thresholds.ValMax));
            Image<Gray, byte> combinedFilter = hueFilter.And(satFilter).And(valFilter).SmoothMedian(5);

            HsvFilter hsvFilter = new HsvFilter();
            hsvFilter.HueFilter = hueFilter;
            hsvFilter.SatFilter = satFilter;
            hsvFilter.ValFilter = valFilter;
            hsvFilter.CombinedFilter = combinedFilter;

            return hsvFilter;
        }
    }
}
