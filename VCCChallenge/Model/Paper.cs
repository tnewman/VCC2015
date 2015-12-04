using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCChallenge
{
    public class Paper
    {
        public Paper()
        {
            this.Color = PaperColor.UNKNOWN;
            this.XMidPoint = 0;
            this.YMidPoint = 0;
            this.ParentImageWidth = 0;
            this.ParentImageHeight = 0;
        }

        public PaperColor Color { get; set; }
        public int XMidPoint { get; set; }
        public int YMidPoint { get; set; }
        public int ParentImageWidth { get; set; }
        public int ParentImageHeight { get; set; }
    }
}
