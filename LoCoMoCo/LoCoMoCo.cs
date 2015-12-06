// Copyright 2015 Lawrence Technological University
// Provided as part of MCS5403 Robotics Programming

using System;
using System.IO.Ports;

namespace LoCoMoCo
{
    /// <summary>
    /// LoCoMoCo provides an API to control the Low Cost Motor 
    /// Controller board on the L2 Bot to control the motors.
    /// </summary>
    public class LoCoMoCo
    {
        /// <summary>
        /// Stop the motor.
        /// </summary>
        public const byte STOP = 0x7F;

        /// <summary>
        /// Allow the motor to float freely.
        /// </summary>
        public const byte FLOAT = 0x0F;

        /// <summary>
        /// Move forward using the motor.
        /// </summary>
        public const byte FORWARD = 0x6f;

        /// <summary>
        /// Move backward using the motor.
        /// </summary>
        public const byte BACKWARD = 0x5F;

        private SerialPort _serialPort;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">Serial port used by LoCoMoco.</param>
        public LoCoMoCo(String port)
        {
            try
            {
                _serialPort = new SerialPort(port);
                _serialPort.BaudRate = 2400;
                _serialPort.DataBits = 8;
                _serialPort.Parity = Parity.None;
                _serialPort.StopBits = StopBits.Two;
                _serialPort.Open();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Move the L2 Bot.
        /// </summary>
        /// <param name="left">Command constant for the left motor.</param>
        /// <param name="right">Command constant for the right motor.</param>
        public void move(byte left, byte right)
        {
            try
            {
                byte[] buffer = { 0x01, left, right };
                _serialPort.Write(buffer, 0, 3);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Stop the L2 Bot.
        /// </summary>
        public void stop()
        {
            move(STOP, STOP);
        }

        /// <summary>
        /// Stop the L2 Bot, allowing the motors to float.
        /// </summary>
        public void floatstop()
        {
            move(FLOAT, FLOAT);
        }

        /// <summary>
        /// Move the L2 Bot straight forward.
        /// </summary>
        public void forward()
        {
            move(FORWARD, FORWARD);
        }

        /// <summary>
        /// Move the L2 Bot straight backward.
        /// </summary>
        public void backward()
        {
            move(BACKWARD, BACKWARD);
        }

        /// <summary>
        /// Turn the L2 Bot right.
        /// </summary>
        public void turnright()
        {
            move(FORWARD, STOP);
        }

        /// <summary>
        /// Turn the L2 Bot left.
        /// </summary>
        public void turnleft()
        {
            move(STOP, FORWARD);
        }

        /// <summary>
        /// Close the serial port used to communicate with the LoCoMoCo board.
        /// </summary>
        public void close()
        {
            _serialPort.Close();
        }
    }
}
