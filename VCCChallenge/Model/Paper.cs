using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCChallenge
{
    /// <summary>
    /// Represents a single piece of paper.
    /// </summary>
    public class Paper
    {
        /// <summary>
        /// Default Constructor. Initializes the color to unknown and all 
        /// image sizes and midpoints to 0.
        /// </summary>
        public Paper()
        {
            this.Color = PaperColor.UNKNOWN;
            this.XMidPoint = 0;
            this.YMidPoint = 0;
            this.ParentImageWidth = 0;
            this.ParentImageHeight = 0;
        }

        /// <summary>
        /// The color of the paper.
        /// </summary>
        public PaperColor Color { get; set; }

        /// <summary>
        /// The X midpoint of the paper in the image.
        /// </summary>
        public int XMidPoint { get; set; }

        /// <summary>
        /// The Y midpoint of the paper in the image.
        /// </summary>
        public int YMidPoint { get; set; }

        /// <summary>
        /// The width of the image containing the paper.
        /// </summary>
        public int ParentImageWidth { get; set; }

        /// <summary>
        /// The height of the image containing the paper.
        /// </summary>
        public int ParentImageHeight { get; set; }
    }
}
