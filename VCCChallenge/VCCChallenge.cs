﻿// Copyright 2015 Thomas Newman

using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace VCCChallenge
{
    public partial class VCCChallenge : Form
    {
        private Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
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
            this.loadThresholds();
            this.setTrackbarsToThresholds();
            this.saveThresholds();

            this.capture.ImageGrabbed += this.DisplayCaptured;
            this.capture.Start();
        }

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

        private int readThresholdOrDefault(string key)
        {
            int result = 0;

            if(this.config.AppSettings.Settings[key] != null)
            {
                int.TryParse(this.config.AppSettings.Settings[key].Value, out result);
            }

            return result;
        }

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

            this.saveThresholds();
        }

        private void yellowMaxHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMaxHueTrack.Value <= this.yellowMinHueTrack.Value)
            {
                this.yellowMinHueTrack.Value = this.yellowMaxHueTrack.Value;
                this.yellowThresholds.HueMin = this.yellowMinHueTrack.Value;
            }

            this.yellowThresholds.HueMax = this.yellowMaxHueTrack.Value;

            this.saveThresholds();
        }

        private void yellowMinSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMinSatTrack.Value >= this.yellowMaxSatTrack.Value)
            {
                this.yellowMaxSatTrack.Value = this.yellowMinSatTrack.Value;
                this.yellowThresholds.SatMax = this.yellowMaxSatTrack.Value;
            }

            this.yellowThresholds.SatMin = this.yellowMinSatTrack.Value;

            this.saveThresholds();
        }

        private void yellowMaxSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMaxSatTrack.Value <= this.yellowMinSatTrack.Value)
            {
                this.yellowMinSatTrack.Value = this.yellowMaxSatTrack.Value;
                this.yellowThresholds.SatMin = this.yellowMinSatTrack.Value;
            }

            this.yellowThresholds.SatMax = this.yellowMaxSatTrack.Value;

            this.saveThresholds();
        }

        private void yellowMinValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMinValTrack.Value >= this.yellowMaxValTrack.Value)
            {
                this.yellowMaxValTrack.Value = this.yellowMinValTrack.Value;
                this.yellowThresholds.ValMax = this.yellowMaxValTrack.Value;
            }

            this.yellowThresholds.ValMin = this.yellowMinValTrack.Value;

            this.saveThresholds();
        }

        private void yellowMaxValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.yellowMaxValTrack.Value <= this.yellowMinValTrack.Value)
            {
                this.yellowMinValTrack.Value = this.yellowMaxValTrack.Value;
                this.yellowThresholds.ValMin = this.yellowMinValTrack.Value;
            }

            this.yellowThresholds.ValMax = this.yellowMaxValTrack.Value;

            this.saveThresholds();
        }

        private void greenMinHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMinHueTrack.Value >= this.greenMaxHueTrack.Value)
            {
                this.greenMaxHueTrack.Value = this.greenMinHueTrack.Value;
                this.greenThresholds.HueMax = this.greenMaxHueTrack.Value;
            }

            this.greenThresholds.HueMin = this.greenMinHueTrack.Value;

            this.saveThresholds();
        }

        private void greenMaxHueTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMaxHueTrack.Value <= this.greenMinHueTrack.Value)
            {
                this.greenMinHueTrack.Value = this.greenMaxHueTrack.Value;
                this.greenThresholds.HueMin = this.greenMinHueTrack.Value;
            }

            this.greenThresholds.HueMax = this.greenMaxHueTrack.Value;

            this.saveThresholds();
        }

        private void greenMinSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMinSatTrack.Value >= this.greenMaxSatTrack.Value)
            {
                this.greenMaxSatTrack.Value = this.greenMinSatTrack.Value;
                this.greenThresholds.SatMax = this.greenMaxSatTrack.Value;
            }

            this.greenThresholds.SatMin = this.greenMinSatTrack.Value;

            this.saveThresholds();
        }

        private void greenMaxSatTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMaxSatTrack.Value <= this.greenMinSatTrack.Value)
            {
                this.greenMinSatTrack.Value = this.greenMaxSatTrack.Value;
                this.greenThresholds.SatMin = this.greenMinSatTrack.Value;
            }

            this.greenThresholds.SatMax = this.greenMaxSatTrack.Value;

            this.saveThresholds();
        }

        private void greenMinValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMinValTrack.Value >= this.greenMaxValTrack.Value)
            {
                this.greenMaxValTrack.Value = this.greenMinValTrack.Value;
                this.greenThresholds.ValMax = this.greenMaxValTrack.Value;
            }

            this.greenThresholds.ValMin = this.greenMinValTrack.Value;

            this.saveThresholds();
        }

        private void greenMaxValTrack_Scroll(object sender, EventArgs e)
        {
            if (this.greenMaxValTrack.Value <= this.greenMinValTrack.Value)
            {
                this.greenMinValTrack.Value = this.greenMaxValTrack.Value;
                this.greenThresholds.ValMin = this.greenMinValTrack.Value;
            }

            this.greenThresholds.ValMax = this.greenMaxValTrack.Value;

            this.saveThresholds();
        }

        private void VCCChallenge_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.capture.Start();
        }
    }
}
