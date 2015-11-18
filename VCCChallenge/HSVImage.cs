// Copyright 2015 Thomas Newman

using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCChallenge
{
    class HsvImage
    {
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
