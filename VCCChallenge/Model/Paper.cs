using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCChallenge
{
    class Paper
    {
        public Paper()
        {
            this.Color = PaperColor.UNKNOWN;
            this.XMidPoint = 0;
            this.YMidPoint = 0;
        }

        public PaperColor Color { get; set; }
        public int XMidPoint { get; set; }
        public int YMidPoint { get; set; }
    }
}
