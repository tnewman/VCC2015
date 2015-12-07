// Copyright 2015 Thomas Newman

namespace VCCChallenge
{
    /// <summary>
    /// UI Callbacks for Digit Detection
    /// </summary>
    interface IDigitDetectionCallback
    {
        /// <summary>
        /// Callback when Digit Detection starts.
        /// </summary>
        void DigitDetectionStarted();

        /// <summary>
        /// Callback when Digit Detection stops.
        /// </summary>
        void DigitDetectionStopped();

        /// <summary>
        /// Callback when a Digit is detected.
        /// </summary>
        /// <param name="digitDetected">Digit detected.</param>
        /// <param name="paperColors">Paper colors detected.</param>
        void DigitDetected(int digitDetected, PaperColor[] paperColors);
    }
}
