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
        private const int MOTOR_FORWARD_SECONDS = 1575;
        private const int DEGREE_22_MOTOR_SECONDS = DEGREE_90_MOTOR_SECONDS / 4;
        private const int DEGREE_90_MOTOR_SECONDS = 1150;
        private const int SLEEP_AFTER_OPERATION_SECONDS = 1000;
        private const string PORT = "COM4";

        public void driveForward()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.forward();

            Thread.Sleep(MOTOR_FORWARD_SECONDS);

            motorController.stop();
            motorController.close();

            Thread.Sleep(SLEEP_AFTER_OPERATION_SECONDS);
        }

        public void turn90DegreesLeft()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.BACKWARD, LoCoMoCo.LoCoMoCo.FORWARD);

            Thread.Sleep(DEGREE_90_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();

            Thread.Sleep(SLEEP_AFTER_OPERATION_SECONDS);
        }

        public void turn90DegreesRight()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.FORWARD, LoCoMoCo.LoCoMoCo.BACKWARD);

            Thread.Sleep(DEGREE_90_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();

            Thread.Sleep(SLEEP_AFTER_OPERATION_SECONDS);
        }

        public void turn22DegreesLeft()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.BACKWARD, LoCoMoCo.LoCoMoCo.FORWARD);

            Thread.Sleep(DEGREE_22_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();

            Thread.Sleep(SLEEP_AFTER_OPERATION_SECONDS);
        }

        public void turn22DegreesRight()
        {
            LoCoMoCo.LoCoMoCo motorController = new LoCoMoCo.LoCoMoCo(PORT);

            motorController.move(LoCoMoCo.LoCoMoCo.FORWARD, LoCoMoCo.LoCoMoCo.BACKWARD);

            Thread.Sleep(DEGREE_22_MOTOR_SECONDS);

            motorController.stop();
            motorController.close();

            Thread.Sleep(SLEEP_AFTER_OPERATION_SECONDS);
        }
    }
}
