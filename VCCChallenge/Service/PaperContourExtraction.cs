﻿// Copyright 2015 Thomas Newman

using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Drawing;

namespace VCCChallenge
{
    /// <summary>
    /// Extract paper contours from an HSV image.
    /// </summary>
    class PaperContourExtraction
    {
        private const double CONTOUR_AREA_PERCENTAGE_THRESHOLD = 0.005;

        /// <summary>
        /// Extract paper contours from an HSV image.
        /// </summary>
        /// <param name="binaryImage">HSV image to extract paper contours from.</param>
        /// <returns>Paper contours extracted.</returns>
        public List<Contour<Point>> extractPaperContours(Image<Gray, byte> binaryImage)
        {
            List<Contour<Point>> paperContours = new List<Contour<Point>>();

            using(MemStorage storage = new MemStorage())
            {
                for (Contour<Point> contour = binaryImage.FindContours(); contour != null; contour = contour.HNext)
                {

                    if((contour.BoundingRectangle.Height * contour.BoundingRectangle.Width) >= (binaryImage.Height * binaryImage.Width * CONTOUR_AREA_PERCENTAGE_THRESHOLD))
                    {
                        paperContours.Add(contour);
                    }
                }
            }

            return paperContours;
        }
    }
}
