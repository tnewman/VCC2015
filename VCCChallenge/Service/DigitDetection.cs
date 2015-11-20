// Copyright 2015 Thomas Newman

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCChallenge
{
    class DigitDetection
    {
        enum State
        {
            WAIT_FOR_RUN,
            MOVE_LEFT,
            CHECK_LEFT,
            MOVE_RIGHT,
            CHECK_RIGHT,
            CHOOSE_DIRECTION,
            MOVE,
            PROCESS_COLUMN,
            CALCULATE_DIGIT
        }

        enum Direction
        {
            LEFT,
            RIGHT
        }

        // After the camera moves, contour detection does not pick up all 
        // of the new contours right away, so initial frames need to be 
        // ignored for accurate detection.
        private const int INITIAL_FRAMES_IGNORED = 5;

        private const int COLUMN_COUNT = 5;

        private State state = State.WAIT_FOR_RUN;
        private IDigitDetectionCallback callback;

        public DigitDetection()
        {
            this.callback = null;
        }

        public DigitDetection(IDigitDetectionCallback callback)
        {
            this.callback = callback;
        }

        public void Start()
        {
            this.callback.DigitDetectionStarted();
            this.state = State.CHECK_LEFT;
        }

        public void Stop()
        {
            this.callback.DigitDetectionStopped();
            this.state = State.WAIT_FOR_RUN;
        }

        private State waitForRun()
        {
            Stop();
            return State.WAIT_FOR_RUN;
        }

        private State moveLeft()
        {
            return State.CHECK_RIGHT;
        }

        private State checkLeft()
        {
            return State.MOVE_RIGHT;
        }

        private State moveRight()
        {
            return State.CHECK_RIGHT;
        }

        private State checkRight()
        {
            return State.CHOOSE_DIRECTION;
        }

        private State chooseDirection()
        {
            return State.PROCESS_COLUMN;
        }

        private State processColumn()
        {
            return State.MOVE;
        }

        private State move()
        {
            return State.CALCULATE_DIGIT;
        }

        public void processDigitDetection()
        {
            switch (this.state)
            {
                case State.WAIT_FOR_RUN:
                    this.state = this.waitForRun();
                    break;
                case State.MOVE_LEFT:
                    this.state = this.moveLeft();
                    break;
                case State.CHECK_LEFT:
                    this.state = this.checkLeft();
                    break;
                case State.MOVE_RIGHT:
                    this.state = this.moveRight();
                    break;
                case State.CHECK_RIGHT:
                    this.state = this.checkRight();
                    break;
                case State.CHOOSE_DIRECTION:
                    this.state = this.chooseDirection();
                    break;
                case State.PROCESS_COLUMN:
                    this.state = this.processColumn();
                    break;
                case State.MOVE:
                    this.state = this.move();
                    break;
                default:
                    this.state = this.waitForRun();
                    break;
            }
        }
    }
}
