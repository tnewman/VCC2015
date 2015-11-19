using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCChallenge
{
    class ColumnPaperColors
    {
        private const int COLUMN_COUNT = 3;
        public const int COLUMN_TOP_INDEX = 0;
        public const int COLUMN_MIDDLE_INDEX = 1;
        public const int COLUMN_BOTTOM_INDEX = 2;

        public PaperColor[] detectColumnPaperColors(Image<Bgr, byte> contourImage, List<Contour<Point>> yellowContours, List<Contour<Point>> greenContours)
        {
            PaperColor[] columns = new PaperColor[COLUMN_COUNT];

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = PaperColor.UKNOWN;
            }

            detectColumns(contourImage, columns, yellowContours, PaperColor.YELLOW);
            detectColumns(contourImage, columns, greenContours, PaperColor.GREEN);

            return columns;
        }

        private void detectColumns(Image<Bgr, byte> contourImage, PaperColor[] columns, List<Contour<Point>> paperContours, PaperColor paperColor)
        {
            foreach (Contour<Point> paperContour in paperContours)
            {
                if (paperContour.BoundingRectangle.Top < contourImage.Height * 0.10)
                {
                    columns[COLUMN_TOP_INDEX] = paperColor;
                }
                else if (paperContour.BoundingRectangle.Bottom > contourImage.Height * 0.90)
                {
                    columns[COLUMN_BOTTOM_INDEX] = paperColor;
                }
                else
                {
                    columns[COLUMN_MIDDLE_INDEX] = paperColor;
                }
            }
        }
    }
}
