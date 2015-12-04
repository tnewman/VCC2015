// Copyright 2015 Thomas Newman

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoCoMoCo;
using System.Threading;

namespace VCCChallenge
{
    class Motor
    {
        private const int MOTOR_FORWARD_SECONDS = 1450;
        private const int DEGREE_3_MOTOR_SECONDS = DEGREE_90_MOTOR_SECONDS / 30;
        private const int DEGREE_90_MOTOR_SECONDS = 1200;
        private const int SLEEP_AFTER_OPERATION_SECONDS = 250;
        private const string PORT = "COM9";

        public void driveForward()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.forward();

            Thread.Sleep(MOTOR_FORWARD_SECONDS);

            motorController.stop();
            motorController.close();
        }

        public void turn90DegreesLeft()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.BACKWARD, LoCoMoCo.LoCoMoCo.FORWARD);

            Thread.Sleep(DEGREE_90_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();
        }

        public void turn90DegreesRight()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.FORWARD, LoCoMoCo.LoCoMoCo.BACKWARD);

            Thread.Sleep(DEGREE_90_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();
        }

        public void turn3DegreesLeft()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.BACKWARD, LoCoMoCo.LoCoMoCo.FORWARD);

            Thread.Sleep(DEGREE_3_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();
        }

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
