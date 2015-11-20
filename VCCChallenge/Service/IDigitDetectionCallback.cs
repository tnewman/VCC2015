// Copyright 2015 Thomas Newman

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCChallenge
{
    interface IDigitDetectionCallback
    {
        void DigitDetectionStarted();
        void DigitDetectionStopped();
        void DigitDetected();
    }
}
