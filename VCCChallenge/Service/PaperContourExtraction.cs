﻿// Copyright 2015 Thomas Newman

using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Drawing;

namespace VCCChallenge
{
    class PaperContourExtraction
    {
        private const double CONTOUR_AREA_PERCENTAGE_THRESHOLD = 0.005;
        private const double MIDPOINT_LEFT_THRESHOLD = 0.40;
        private const double MIDPOINT_RIGHT_THRESHOLD = 0.60;

        public List<Contour<Point>> extractPaperContours(Image<Gray, byte> binaryImage)
        {
            List<Contour<Point>> paperContours = new List<Contour<Point>>();

            using(MemStorage storage = new MemStorage())
            {
                for (Contour<Point> contour = binaryImage.FindContours(); contour != null; contour = contour.HNext)
                {
                    if((contour.BoundingRectangle.Height * contour.BoundingRectangle.Width) >= (binaryImage.Height * binaryImage.Width * CONTOUR_AREA_PERCENTAGE_THRESHOLD))
                    {
                        int xMidPoint = contour.BoundingRectangle.X + (contour.BoundingRectangle.Width / 2);

                        if (xMidPoint >= binaryImage.Width * MIDPOINT_LEFT_THRESHOLD && xMidPoint <= binaryImage.Width * MIDPOINT_RIGHT_THRESHOLD)
                        {
                            paperContours.Add(contour);
                        }
                    }
                }
            }

            return paperContours;
        }
    }
}
