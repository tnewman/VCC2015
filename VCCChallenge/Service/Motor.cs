// Copyright 2015 Thomas Newman

using System.Threading;

namespace VCCChallenge
{
    /// <summary>
    /// Provides an interface to the L2 Bot LoCoMoCo by abstracting motor commands into 
    /// commands that make sense for the 2015 VCC problem domain.
    /// </summary>
    /// <remarks>
    /// It is important to note 
    /// that due to a lack of motor encoders, all rotation and distance commands are 
    /// approximate. Several correction commands are provided to allow the L2 Bot's positioning 
    /// to be corrected if it has under-steered or under-rotated. Constants for steering and 
    /// driving were determined by experimentation with a specific robot. This will need to be 
    /// changed for each application.
    /// </remarks>
    class Motor
    {
        private const int MOTOR_FORWARD_SECONDS = 1375;
        private const int MOTOR_FORWARD_CORRECTION_SECONDS = 50;
        private const int DEGREE_3_MOTOR_SECONDS = DEGREE_90_MOTOR_SECONDS / 30;
        private const int DEGREE_90_MOTOR_SECONDS = 1175;
        private const int SLEEP_AFTER_OPERATION_SECONDS = 250;
        private const string PORT = "COM9";

        /// <summary>
        /// Moves the L2 Bot forward approximately one column.
        /// </summary>
        public void driveForward()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.forward();

            Thread.Sleep(MOTOR_FORWARD_SECONDS);

            motorController.stop();
            motorController.close();
        }

        /// <summary>
        /// Move the L2 Bot forward a small amount when the robot has not 
        /// been able to move forward enough to be centered on the column.
        /// </summary>
        public void driveForwardCorrection()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.forward();

            Thread.Sleep(MOTOR_FORWARD_CORRECTION_SECONDS);

            motorController.stop();
            motorController.close();
        }

        /// <summary>
        /// Turns the L2 Bot approximately 90 degrees left.
        /// </summary>
        public void turn90DegreesLeft()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.BACKWARD, LoCoMoCo.LoCoMoCo.FORWARD);

            Thread.Sleep(DEGREE_90_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();
        }

        /// <summary>
        /// Turns the L2 Bot approximately 90 degrees left.
        /// </summary>
        public void turn90DegreesRight()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.FORWARD, LoCoMoCo.LoCoMoCo.BACKWARD);

            Thread.Sleep(DEGREE_90_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();
        }

        /// <summary>
        /// Turns the L2 Bot approximately 3 degrees left. This used used to 
        /// correct the L2 Bot's angle.
        /// </summary>
        public void turn3DegreesLeft()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.BACKWARD, LoCoMoCo.LoCoMoCo.FORWARD);

            Thread.Sleep(DEGREE_3_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();
        }

        /// <summary>
        /// Turns the L2 Bot approximately 3 degrees right. This is used to 
        /// correct the L2 Bot's angle.
        /// </summary>
        public void turn3DegreesRight()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.FORWARD, LoCoMoCo.LoCoMoCo.BACKWARD);

            Thread.Sleep(DEGREE_3_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();
        }
    }
}
