// Copyright 2015 Thomas Newman

using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Drawing;

namespace VCCChallenge
{
    /// <summary>
    /// Create a 3X3 grid of paper colors using a source image and the 
    /// contours for each paper color.
    /// </summary>
    class PaperColorDetection
    {
        private const double LEFT_WIDTH_THRESHOLD = 0.35;
        private const double RIGHT_WIDTH_THRESHOLD = 0.65;
        private const double TOP_HEIGHT_THRESHOLD = 0.15;
        private const double BOTTOM_HEIGHT_THRESHOLD = 0.50;

        /// <summary>
        /// Create a 3X3 grid of paper colors using a source image and the 
        /// contours for each paper color.
        /// </summary>
        /// <param name="contourImage">Source image.</param>
        /// <param name="yellowContours">Yellow paper contours.</param>
        /// <param name="greenContours">Green paper contours.</param>
        /// <returns></returns>
        public Paper[,] detectColumnPaperColors(Image<Bgr, byte> contourImage, List<Contour<Point>> yellowContours, List<Contour<Point>> greenContours)
        {
            Paper[,] papers = new Paper[3,3];

            for (int i = 0; i < papers.GetLength(0); i++)
            {
                for(int j = 0; j < papers.GetLength(1); j++)
                {
                    papers[i, j] = new Paper();
                    papers[i, j].Color = PaperColor.UNKNOWN;
                }
            }

            detectColumns(contourImage, papers, yellowContours, PaperColor.YELLOW);
            detectColumns(contourImage, papers, greenContours, PaperColor.GREEN);

            return papers;
        }

        /// <summary>
        /// Adds paper colors from a contour into the grid.
        /// </summary>
        /// <param name="contourImage">Source image.</param>
        /// <param name="papers">Grid to add paper colors to.</param>
        /// <param name="paperContours">Contours to add to grid.</param>
        /// <param name="paperColor">Color of paper to add to grid.</param>
        private void detectColumns(Image<Bgr, byte> contourImage, Paper[,] papers, List<Contour<Point>> paperContours, PaperColor paperColor)
        {
            foreach (Contour<Point> paperContour in paperContours)
            {
                int x;
                int xMidPoint = paperContour.BoundingRectangle.X + paperContour.BoundingRectangle.Width / 2;

                if (xMidPoint < contourImage.Width * LEFT_WIDTH_THRESHOLD)
                {
                    x = 0;
                }
                else if (xMidPoint > contourImage.Width * RIGHT_WIDTH_THRESHOLD)
                {
                    x = 2;
                }
                else
                {
                    x = 1;
                }

                int y;
                int yMidPoint = paperContour.BoundingRectangle.Y + paperContour.BoundingRectangle.Height / 2;

                if (yMidPoint < contourImage.Height * TOP_HEIGHT_THRESHOLD)
                {
                    y = 0;
                } else if(yMidPoint > contourImage.Height * BOTTOM_HEIGHT_THRESHOLD)
                {
                    y = 2;
                }
                else
                {
                    y = 1;
                }

                Paper currentPaper = papers[y, x];
                currentPaper.Color = paperColor;
                currentPaper.XMidPoint = xMidPoint;
                currentPaper.YMidPoint = yMidPoint;
                currentPaper.ParentImageWidth = contourImage.Width;
                currentPaper.ParentImageHeight = contourImage.Height;
            }
        }
    }
}
