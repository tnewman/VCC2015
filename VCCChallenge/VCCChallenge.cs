// Copyright 2015 Thomas Newman

using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace VCCChallenge
{
    public partial class VCCChallenge : Form
    {
        private Capture capture = new Capture();
        private BinaryThresholds yellowThresholds = new BinaryThresholds();
        private BinaryThresholds greenThresholds = new BinaryThresholds();
        private HsvImage hsvImage = new HsvImage();

        public VCCChallenge()
        {
            InitializeComponent();
        }

        private void VCCChallenge_Load(object sender, EventArgs e)
        {
            this.capture.ImageGrabbed += this.DisplayCaptured;
            this.capture.Start();
        }

        private void DisplayCaptured(object sender, EventArgs e)
        {
            Image<Bgr, byte> capturedImage = this.capture.RetrieveBgrFrame();

            if (capturedImage != null)
            {
                HsvFilter yellowFilter = this.hsvImage.generateCombinedHSV(capturedImage, this.yellowThresholds);
                HsvFilter greenFilter = this.hsvImage.generateCombinedHSV(capturedImage, this.greenThresholds);

                displayFilters(yellowFilter, greenFilter);

                this.CaptureImgBox.Image = capturedImage.Resize(this.CaptureImgBox.Width, this.CaptureImgBox.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            }
        }

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

        private void yellowMinHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMinHueTrack.Value >= this.yellowMaxHueTrack.Value)
            {
                this.yellowMaxHueTrack.Value = this.yellowMinHueTrack.Value;
                this.yellowThresholds.HueMax = this.yellowMaxHueTrack.Value;
            }

            this.yellowThresholds.HueMin = this.yellowMinHueTrack.Value;
        }

        private void yellowMaxHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMaxHueTrack.Value <= this.yellowMinHueTrack.Value)
            {
                this.yellowMinHueTrack.Value = this.yellowMaxHueTrack.Value;
                this.yellowThresholds.HueMin = this.yellowMinHueTrack.Value;
            }

            this.yellowThresholds.HueMax = this.yellowMaxHueTrack.Value;
        }

        private void yellowMinSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMinSatTrack.Value >= this.yellowMaxSatTrack.Value)
            {
                this.yellowMaxSatTrack.Value = this.yellowMinSatTrack.Value;
                this.yellowThresholds.SatMax = this.yellowMaxSatTrack.Value;
            }

            this.yellowThresholds.SatMin = this.yellowMinSatTrack.Value;
        }

        private void yellowMaxSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMaxSatTrack.Value <= this.yellowMinSatTrack.Value)
            {
                this.yellowMinSatTrack.Value = this.yellowMaxSatTrack.Value;
                this.yellowThresholds.SatMin = this.yellowMinSatTrack.Value;
            }

            this.yellowThresholds.SatMax = this.yellowMaxSatTrack.Value;
        }

        private void yellowMinValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMinValTrack.Value >= this.yellowMaxValTrack.Value)
            {
                this.yellowMaxValTrack.Value = this.yellowMinValTrack.Value;
                this.yellowThresholds.ValMax = this.yellowMaxValTrack.Value;
            }

            this.yellowThresholds.ValMin = this.yellowMinValTrack.Value;
        }

        private void yellowMaxValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMaxValTrack.Value <= this.yellowMinValTrack.Value)
            {
                this.yellowMinValTrack.Value = this.yellowMaxValTrack.Value;
                this.yellowThresholds.ValMin = this.yellowMinValTrack.Value;
            }

            this.yellowThresholds.ValMax = this.yellowMaxValTrack.Value;
        }

        private void greenMinHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMinHueTrack.Value >= this.greenMaxHueTrack.Value)
            {
                this.greenMaxHueTrack.Value = this.greenMinHueTrack.Value;
                this.greenThresholds.HueMax = this.greenMaxHueTrack.Value;
            }

            this.greenThresholds.HueMin = this.greenMinHueTrack.Value;
        }

        private void greenMaxHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMaxHueTrack.Value <= this.greenMinHueTrack.Value)
            {
                this.greenMinHueTrack.Value = this.greenMaxHueTrack.Value;
                this.greenThresholds.HueMin = this.greenMinHueTrack.Value;
            }

            this.greenThresholds.HueMax = this.greenMaxHueTrack.Value;
        }

        private void greenMinSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMinSatTrack.Value >= this.greenMaxSatTrack.Value)
            {
                this.greenMaxSatTrack.Value = this.greenMinSatTrack.Value;
                this.greenThresholds.SatMax = this.greenMaxSatTrack.Value;
            }

            this.greenThresholds.SatMin = this.greenMinSatTrack.Value;
        }

        private void greenMaxSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMaxSatTrack.Value <= this.greenMinSatTrack.Value)
            {
                this.greenMinSatTrack.Value = this.greenMaxSatTrack.Value;
                this.greenThresholds.SatMin = this.greenMinSatTrack.Value;
            }

            this.greenThresholds.SatMax = this.greenMaxSatTrack.Value;
        }

        private void greenMinValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMinValTrack.Value >= this.greenMaxValTrack.Value)
            {
                this.greenMaxValTrack.Value = this.greenMinValTrack.Value;
                this.greenThresholds.ValMax = this.greenMaxValTrack.Value;
            }

            this.greenThresholds.ValMin = this.greenMinValTrack.Value;
        }

        private void greenMaxValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMaxValTrack.Value <= this.greenMinValTrack.Value)
            {
                this.greenMinValTrack.Value = this.greenMaxValTrack.Value;
                this.greenThresholds.ValMin = this.greenMinValTrack.Value;
            }

            this.greenThresholds.ValMax = this.greenMaxValTrack.Value;
        }

        private void VCCChallenge_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.capture.Start();
        }
    }
}
