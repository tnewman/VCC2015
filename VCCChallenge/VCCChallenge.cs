// Copyright 2015 Thomas Newman

using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace VCCChallenge
{
    /// <summary>
    /// 2015 VCC Challenge UI
    /// </summary>
    public partial class VCCChallenge : Form, IDigitDetectionCallback
    {
        private Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private Capture capture = new Capture();
        private BinaryThresholds yellowThresholds = new BinaryThresholds();
        private BinaryThresholds greenThresholds = new BinaryThresholds();
        private HsvImage hsvImage = new HsvImage();
        private PaperContourExtraction paperContourExtraction = new PaperContourExtraction();
        private PaperColorDetection columnDetection = new PaperColorDetection();
        private Motor motor = new Motor();
        private DigitDetection digitDetection;

        /// <summary>
        /// Constructor
        /// </summary>
        public VCCChallenge()
        {
            InitializeComponent();
            this.digitDetection = new DigitDetection(this);
        }

        /// <summary>
        /// Initializes the form, including loading slider values from XML.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void VCCChallenge_Load(object sender, EventArgs e)
        {
            this.loadThresholds();
            this.setTrackbarsToThresholds();
            this.saveThresholds();

            this.capture.ImageGrabbed += this.DisplayCaptured;
            this.capture.Start();
        }

        /// <summary>
        /// Load threshold slider values from XML.
        /// </summary>
        private void loadThresholds()
        {
            this.yellowThresholds.HueMin = readThresholdOrDefault("YellowHueMin");
            this.yellowThresholds.HueMax = readThresholdOrDefault("YellowHueMax");
            this.yellowThresholds.SatMin = readThresholdOrDefault("YellowSatMin");
            this.yellowThresholds.SatMax = readThresholdOrDefault("YellowSatMax");
            this.yellowThresholds.ValMin = readThresholdOrDefault("YellowValMin");
            this.yellowThresholds.ValMax = readThresholdOrDefault("YellowValMax");

            this.greenThresholds.HueMin = readThresholdOrDefault("GreenHueMin");
            this.greenThresholds.HueMax = readThresholdOrDefault("GreenHueMax");
            this.greenThresholds.SatMin = readThresholdOrDefault("GreenSatMin");
            this.greenThresholds.SatMax = readThresholdOrDefault("GreenSatMax");
            this.greenThresholds.ValMin = readThresholdOrDefault("GreenValMin");
            this.greenThresholds.ValMax = readThresholdOrDefault("GreenValMax");
        }

        /// <summary>
        /// Read a slider threshold from XML, and supply a default value of 0 if 
        /// the slider value could not be read from XML.
        /// </summary>
        /// <param name="key">Key for slider threshold to read.</param>
        /// <returns>Slider threshold value.</returns>
        private int readThresholdOrDefault(string key)
        {
            int result = 0;

            if(this.config.AppSettings.Settings[key] != null)
            {
                int.TryParse(this.config.AppSettings.Settings[key].Value, out result);
            }

            return result;
        }

        /// <summary>
        /// Sets all of the trackbars to the threshold values read from XML.
        /// </summary>
        private void setTrackbarsToThresholds()
        {
            this.yellowMinHueTrack.Value = this.yellowThresholds.HueMin;
            this.yellowMaxHueTrack.Value = this.yellowThresholds.HueMax;
            this.yellowMinSatTrack.Value = this.yellowThresholds.SatMin;
            this.yellowMaxSatTrack.Value = this.yellowThresholds.SatMax;
            this.yellowMinValTrack.Value = this.yellowThresholds.ValMin;
            this.yellowMaxValTrack.Value = this.yellowThresholds.ValMax;

            this.greenMinHueTrack.Value = this.greenThresholds.HueMin;
            this.greenMaxHueTrack.Value = this.greenThresholds.HueMax;
            this.greenMinSatTrack.Value = this.greenThresholds.SatMin;
            this.greenMaxSatTrack.Value = this.greenThresholds.SatMax;
            this.greenMinValTrack.Value = this.greenThresholds.ValMin;
            this.greenMaxValTrack.Value = this.greenThresholds.ValMax;
        }

        /// <summary>
        /// Sets all of the current trackbar threshold values to XML.
        /// </summary>
        private void saveThresholds()
        {
            this.saveConfigValue(this.config.AppSettings, "YellowHueMin", this.yellowThresholds.HueMin.ToString());
            this.saveConfigValue(this.config.AppSettings, "YellowHueMax", this.yellowThresholds.HueMax.ToString());
            this.saveConfigValue(this.config.AppSettings, "YellowSatMin", this.yellowThresholds.SatMin.ToString());
            this.saveConfigValue(this.config.AppSettings, "YellowSatMax", this.yellowThresholds.SatMax.ToString());
            this.saveConfigValue(this.config.AppSettings, "YellowValMin", this.yellowThresholds.ValMin.ToString());
            this.saveConfigValue(this.config.AppSettings, "YellowValMax", this.yellowThresholds.ValMax.ToString());

            this.saveConfigValue(this.config.AppSettings, "GreenHueMin", this.greenThresholds.HueMin.ToString());
            this.saveConfigValue(this.config.AppSettings, "GreenHueMax", this.greenThresholds.HueMax.ToString());
            this.saveConfigValue(this.config.AppSettings, "GreenSatMin", this.greenThresholds.SatMin.ToString());
            this.saveConfigValue(this.config.AppSettings, "GreenSatMax", this.greenThresholds.SatMax.ToString());
            this.saveConfigValue(this.config.AppSettings, "GreenValMin", this.greenThresholds.ValMin.ToString());
            this.saveConfigValue(this.config.AppSettings, "GreenValMax", this.greenThresholds.ValMax.ToString());

            config.Save();
        }

        /// <summary>
        /// Saves a threshold vlaue to XML.
        /// </summary>
        /// <param name="settingsSection">AppSettings to use for saving to XML.</param>
        /// <param name="key">Key to use when saving this trackbar value to XML.</param>
        /// <param name="value">Value to save to XML.</param>
        private void saveConfigValue(AppSettingsSection settingsSection, string key, string value)
        {
            if(settingsSection.Settings[key] == null)
            {
                settingsSection.Settings.Add(key, value);
            } else
            {
                settingsSection.Settings[key].Value = value;
            }
        }

        /// <summary>
        /// Invoked when a frame is read from the camera. Performs image processing 
        /// for each frame.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void DisplayCaptured(object sender, EventArgs e)
        {
            Image<Bgr, byte> capturedImage = this.capture.RetrieveBgrFrame();

            if (capturedImage != null)
            {
                HsvFilter yellowFilter = this.hsvImage.generateCombinedHSV(capturedImage, this.yellowThresholds);
                HsvFilter greenFilter = this.hsvImage.generateCombinedHSV(capturedImage, this.greenThresholds);

                List<Contour<Point>> yellowPaperContours = this.paperContourExtraction.extractPaperContours(yellowFilter.CombinedFilter);
                List<Contour<Point>> greenPaperContours = this.paperContourExtraction.extractPaperContours(greenFilter.CombinedFilter);
                Paper[,] papers = this.columnDetection.detectColumnPaperColors(capturedImage, yellowPaperContours, greenPaperContours);
                this.digitDetection.processDigitDetection(papers);

                this.displayFilters(yellowFilter, greenFilter);
                this.displayPapers(papers);

                foreach(Contour<Point> yellowPaperContour in yellowPaperContours)
                {
                    capturedImage.Draw(yellowPaperContour.BoundingRectangle, new Bgr(Color.Red), 5);
                }

                foreach (Contour<Point> greenPaperContour in greenPaperContours)
                {
                    capturedImage.Draw(greenPaperContour.BoundingRectangle, new Bgr(Color.Blue), 5);
                }

                this.CaptureImgBox.Image = capturedImage.Resize(this.CaptureImgBox.Width, this.CaptureImgBox.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            }
        }

        delegate void DisplayPaperCallback(Paper[,] papers);

        /// <summary>
        /// Display the current grid of paper colors on the UI.
        /// </summary>
        /// <param name="papers">Current paper grid.</param>
        private void displayPapers(Paper[,] papers)
        {
            if (this.InvokeRequired)
            {
                DisplayPaperCallback callback = new DisplayPaperCallback(displayPapers);

                try
                {
                    this.Invoke(callback, new object[] { papers });
                }
                catch (ObjectDisposedException)
                {
                    // No Op
                }
            }
            else
            {
                this.y0x0ColTxt.Text = PaperColorUtils.PaperColorToString(papers[0, 0].Color);
                this.y0x1ColTxt.Text = PaperColorUtils.PaperColorToString(papers[0, 1].Color);
                this.y0x2ColTxt.Text = PaperColorUtils.PaperColorToString(papers[0, 2].Color);
                this.y1x0ColTxt.Text = PaperColorUtils.PaperColorToString(papers[1, 0].Color);
                this.y1x1ColTxt.Text = PaperColorUtils.PaperColorToString(papers[1, 1].Color);
                this.y1x2ColTxt.Text = PaperColorUtils.PaperColorToString(papers[1, 2].Color);
                this.y2x0ColTxt.Text = PaperColorUtils.PaperColorToString(papers[2, 0].Color);
                this.y2x1ColTxt.Text = PaperColorUtils.PaperColorToString(papers[2, 1].Color);
                this.y2x2ColTxt.Text = PaperColorUtils.PaperColorToString(papers[2, 2].Color);
            }
        }

        delegate void DigitDetectionCallback();
        delegate void DigitDetectionCallbackSetDigit(int digitDetected, PaperColor[] paperColors);

        /// <summary>
        /// Disable the start button and enable the stop button when digit 
        /// detection is started.
        /// </summary>
        public void DigitDetectionStarted()
        {
            if (this.InvokeRequired)
            {
                DigitDetectionCallback callback = new DigitDetectionCallback(DigitDetectionStarted);

                try
                {
                    this.Invoke(callback, new object[] { });
                }
                catch (ObjectDisposedException)
                {
                    // No Op
                }
            }
            else
            {
                this.startDetectBtn.Enabled = false;
                this.stopDetectBtn.Enabled = true;
            }
        }

        /// <summary>
        /// Enable the start button and disable the stop button when digit 
        /// detection is stopped.
        /// </summary>
        public void DigitDetectionStopped()
        {
            if (this.InvokeRequired)
            {
                DigitDetectionCallback callback = new DigitDetectionCallback(DigitDetectionStopped);

                try
                {
                    this.Invoke(callback, new object[] { });
                }
                catch (ObjectDisposedException)
                {
                    // No Op
                }
            }
            else
            {
                this.startDetectBtn.Enabled = true;
                this.stopDetectBtn.Enabled = false;
            }
        }

        /// <summary>
        /// Display the detected digit and coloumns of paper colors on the UI.
        /// </summary>
        /// <param name="digitDetected">Detected Digit</param>
        /// <param name="paperColors">All Paper Colors Detected</param>
        public void DigitDetected(int digitDetected, PaperColor[] paperColors)
        {
            if (this.InvokeRequired)
            {
                DigitDetectionCallbackSetDigit callback = new DigitDetectionCallbackSetDigit(DigitDetected);

                try
                {
                    this.Invoke(callback, new object[] { digitDetected, paperColors });
                }
                catch (ObjectDisposedException)
                {
                    // No Op
                }
            }
            else
            {
                this.detectedDigitLbl.Text = digitDetected.ToString();

                this.cell0Txt.Text = PaperColorUtils.PaperColorToString(paperColors[0]);
                this.cell1Txt.Text = PaperColorUtils.PaperColorToString(paperColors[1]);
                this.cell2Txt.Text = PaperColorUtils.PaperColorToString(paperColors[2]);
                this.cell3Txt.Text = PaperColorUtils.PaperColorToString(paperColors[3]);
                this.cell4Txt.Text = PaperColorUtils.PaperColorToString(paperColors[4]);
                this.cell5Txt.Text = PaperColorUtils.PaperColorToString(paperColors[5]);
                this.cell6Txt.Text = PaperColorUtils.PaperColorToString(paperColors[6]);
                this.cell7Txt.Text = PaperColorUtils.PaperColorToString(paperColors[7]);
                this.cell8Txt.Text = PaperColorUtils.PaperColorToString(paperColors[8]);
                this.cell9Txt.Text = PaperColorUtils.PaperColorToString(paperColors[9]);
                this.cell10Txt.Text = PaperColorUtils.PaperColorToString(paperColors[10]);
                this.cell11Txt.Text = PaperColorUtils.PaperColorToString(paperColors[11]);
                this.cell12Txt.Text = PaperColorUtils.PaperColorToString(paperColors[12]);
                this.cell13Txt.Text = PaperColorUtils.PaperColorToString(paperColors[13]);
                this.cell14Txt.Text = PaperColorUtils.PaperColorToString(paperColors[14]);
            }
        }

        /// <summary>
        /// Display all HSV filters.
        /// </summary>
        /// <param name="yellowFilter">Yellow HSV Filters to display.</param>
        /// <param name="greenFilter">Green HSV Filters to display.</param>
        private void displayFilters(HsvFilter yellowFilter, HsvFilter greenFilter)
        {
            this.YellowHueImgBox.Image = yellowFilter.HueFilter.Resize(this.YellowHueImgBox.Width, this.YellowHueImgBox.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            this.YellowSatImgBox.Image = yellowFilter.SatFilter.Resize(this.YellowSatImgBox.Width, this.YellowSatImgBox.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            this.YellowValImgBox.Image = yellowFilter.ValFilter.Resize(this.YellowValImgBox.Width, this.YellowValImgBox.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

            this.YellowComImgBox.Image = yellowFilter.CombinedFilter.Resize(this.YellowComImgBox.Width, this.YellowComImgBox.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

            this.GreenHueImgBox.Image = greenFilter.HueFilter.Resize(this.GreenHueImgBox.Width, this.GreenHueImgBox.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            this.GreenSatImgBox.Image = greenFilter.SatFilter.Resize(this.GreenSatImgBox.Width, this.GreenSatImgBox.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            this.GreenValImgBox.Image = greenFilter.ValFilter.Resize(this.GreenValImgBox.Width, this.GreenValImgBox.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

            this.GreenComImgBox.Image = greenFilter.CombinedFilter.Resize(this.GreenComImgBox.Width, this.GreenComImgBox.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
        }

        /// <summary>
        /// Trackbar Event for Minimum Yellow Hue.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void yellowMinHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMinHueTrack.Value >= this.yellowMaxHueTrack.Value)
            {
                this.yellowMaxHueTrack.Value = this.yellowMinHueTrack.Value;
                this.yellowThresholds.HueMax = this.yellowMaxHueTrack.Value;
            }

            this.yellowThresholds.HueMin = this.yellowMinHueTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Maximum Yellow Hue.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void yellowMaxHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMaxHueTrack.Value <= this.yellowMinHueTrack.Value)
            {
                this.yellowMinHueTrack.Value = this.yellowMaxHueTrack.Value;
                this.yellowThresholds.HueMin = this.yellowMinHueTrack.Value;
            }

            this.yellowThresholds.HueMax = this.yellowMaxHueTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Minimum Yellow Saturation.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void yellowMinSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMinSatTrack.Value >= this.yellowMaxSatTrack.Value)
            {
                this.yellowMaxSatTrack.Value = this.yellowMinSatTrack.Value;
                this.yellowThresholds.SatMax = this.yellowMaxSatTrack.Value;
            }

            this.yellowThresholds.SatMin = this.yellowMinSatTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Maximum Yellow Saturation.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void yellowMaxSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMaxSatTrack.Value <= this.yellowMinSatTrack.Value)
            {
                this.yellowMinSatTrack.Value = this.yellowMaxSatTrack.Value;
                this.yellowThresholds.SatMin = this.yellowMinSatTrack.Value;
            }

            this.yellowThresholds.SatMax = this.yellowMaxSatTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Minimum Yellow Value.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void yellowMinValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMinValTrack.Value >= this.yellowMaxValTrack.Value)
            {
                this.yellowMaxValTrack.Value = this.yellowMinValTrack.Value;
                this.yellowThresholds.ValMax = this.yellowMaxValTrack.Value;
            }

            this.yellowThresholds.ValMin = this.yellowMinValTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Maximum Yellow Value.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void yellowMaxValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMaxValTrack.Value <= this.yellowMinValTrack.Value)
            {
                this.yellowMinValTrack.Value = this.yellowMaxValTrack.Value;
                this.yellowThresholds.ValMin = this.yellowMinValTrack.Value;
            }

            this.yellowThresholds.ValMax = this.yellowMaxValTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Minimum Green Hue.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void greenMinHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMinHueTrack.Value >= this.greenMaxHueTrack.Value)
            {
                this.greenMaxHueTrack.Value = this.greenMinHueTrack.Value;
                this.greenThresholds.HueMax = this.greenMaxHueTrack.Value;
            }

            this.greenThresholds.HueMin = this.greenMinHueTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Maximum Green Hue.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void greenMaxHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMaxHueTrack.Value <= this.greenMinHueTrack.Value)
            {
                this.greenMinHueTrack.Value = this.greenMaxHueTrack.Value;
                this.greenThresholds.HueMin = this.greenMinHueTrack.Value;
            }

            this.greenThresholds.HueMax = this.greenMaxHueTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Minimum Green Saturation.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void greenMinSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMinSatTrack.Value >= this.greenMaxSatTrack.Value)
            {
                this.greenMaxSatTrack.Value = this.greenMinSatTrack.Value;
                this.greenThresholds.SatMax = this.greenMaxSatTrack.Value;
            }

            this.greenThresholds.SatMin = this.greenMinSatTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Maximum Green Saturation.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void greenMaxSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMaxSatTrack.Value <= this.greenMinSatTrack.Value)
            {
                this.greenMinSatTrack.Value = this.greenMaxSatTrack.Value;
                this.greenThresholds.SatMin = this.greenMinSatTrack.Value;
            }

            this.greenThresholds.SatMax = this.greenMaxSatTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Minimum Green Value.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void greenMinValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMinValTrack.Value >= this.greenMaxValTrack.Value)
            {
                this.greenMaxValTrack.Value = this.greenMinValTrack.Value;
                this.greenThresholds.ValMax = this.greenMaxValTrack.Value;
            }

            this.greenThresholds.ValMin = this.greenMinValTrack.Value;
        }

        /// <summary>
        /// Trackbar Event for Maximum Green Value.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void greenMaxValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMaxValTrack.Value <= this.greenMinValTrack.Value)
            {
                this.greenMinValTrack.Value = this.greenMaxValTrack.Value;
                this.greenThresholds.ValMin = this.greenMinValTrack.Value;
            }

            this.greenThresholds.ValMax = this.greenMaxValTrack.Value;
        }

        /// <summary>
        /// Stop capturing and persist threshold values to XML when the form 
        /// closes.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void VCCChallenge_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.capture.Stop();
            this.capture.Dispose();

            this.saveThresholds();
        }

        /// <summary>
        /// Test Forward Movement button click.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void testForwardBtn_Click(object sender, EventArgs e)
        {
            this.motor.driveForward();
        }

        /// <summary>
        /// Test Steer Left 90 Degrees button click.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void testSteerLeft90Btn_Click(object sender, EventArgs e)
        {
            this.motor.turn90DegreesLeft();
        }

        /// <summary>
        /// Test Steer Right 90 Degrees button click.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void testSteerRight90Btn_Click(object sender, EventArgs e)
        {
            this.motor.turn90DegreesRight();
        }

        /// <summary>
        /// Test Steer Left 3 Degrees button click.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void testSteerLeft3Btn_Click(object sender, EventArgs e)
        {
            this.motor.turn3DegreesLeft();
        }

        /// <summary>
        /// Test Steer Right 3 Degrees button click.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void testSteerRight3Btn_Click(object sender, EventArgs e)
        {
            this.motor.turn3DegreesRight();
        }

        /// <summary>
        /// Start Digit Detection button click.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void startDetectBtn_Click(object sender, EventArgs e)
        {
            this.digitDetection.Start();
        }

        /// <summary>
        /// Stop Digit Detection button click.
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void stopDetectBtn_Click(object sender, EventArgs e)
        {
            this.digitDetection.Stop();
        }
    }
}
