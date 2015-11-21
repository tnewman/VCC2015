// Copyright 2015 Thomas Newman

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            RETURN_TO_START,
            CHOOSE_DIRECTION,
            MOVE,
            PROCESS_COLUMN,
            CALCULATE_DIGIT
        }

        enum Direction
        {
            NONE,
            LEFT,
            RIGHT
        }

        // After the camera moves, contour detection does not pick up all 
        // of the new contours right away because of camera autofocus, so 
        // initial frames need to be ignored for accurate detection.
        private const int INITIAL_FRAMES_IGNORED = 10;

        private const int COLUMN_COUNT = 5;

        private Motor motor = new Motor();
        private State state = State.WAIT_FOR_RUN;
        private IDigitDetectionCallback callback;
        private Direction direction;
        private int initialFrameCount = 0;

        private int leftColumns = 0;
        private int rightColumns = 0;
        private List<PaperColor[]> paperColumns;

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
            this.state = State.MOVE_LEFT;
        }

        public void Stop()
        {
            this.callback.DigitDetectionStopped();
            this.state = State.WAIT_FOR_RUN;
        }

        private State waitForRun(PaperColor[] column)
        {
            Stop();
            return State.WAIT_FOR_RUN;
        }

        private State moveLeft(PaperColor[] column)
        {
            this.motor.turn22DegreesLeft();

            return State.CHECK_LEFT;
        }

        private State checkLeft(PaperColor[] column)
        {
            this.leftColumns = 0;

            foreach(PaperColor row in column)
            {
                if (!row.Equals(PaperColor.UKNOWN))
                {
                    this.leftColumns++;
                }
            }

            return State.MOVE_RIGHT;
        }

        private State moveRight(PaperColor[] column)
        {
            this.motor.turn22DegreesRight();
            this.motor.turn22DegreesRight();

            return State.CHECK_RIGHT;
        }

        private State checkRight(PaperColor[] column)
        {
            this.rightColumns = 0;

            foreach (PaperColor row in column)
            {
                if (!row.Equals(PaperColor.UKNOWN))
                {
                    this.rightColumns++;
                }
            }

            return State.RETURN_TO_START;
        }

        private State returnToStart(PaperColor[] column)
        {
            this.motor.turn22DegreesLeft();

            return State.CHOOSE_DIRECTION;
        }

        private State chooseDirection(PaperColor[] column)
        {
            this.paperColumns = new List<PaperColor[]>();

            if(this.leftColumns == this.rightColumns)
            {
                this.direction = Direction.NONE;
                return State.PROCESS_COLUMN;
            }
            else if (this.leftColumns > this.rightColumns)
            {
                this.direction = Direction.LEFT;
                return State.PROCESS_COLUMN;
            }
            else
            {
                this.direction = Direction.RIGHT;
                return State.PROCESS_COLUMN;
            }
        }

        private State processColumn(PaperColor[] column)
        {
            this.paperColumns.Add(column);

            if(this.paperColumns.Count < COLUMN_COUNT)
            {
                if(this.direction == Direction.LEFT)
                {
                    this.motor.turn90DegreesLeft();
                    this.motor.driveForward();
                    this.motor.turn90DegreesRight();
                }
                else if(this.direction == Direction.RIGHT)
                {
                    this.motor.turn90DegreesRight();
                    this.motor.driveForward();
                    this.motor.turn90DegreesLeft();
                } else
                {
                    return State.WAIT_FOR_RUN;
                }

                return State.PROCESS_COLUMN;
            } else
            {
                return State.CALCULATE_DIGIT;
            }
        }

        private State move(PaperColor[] column)
        {
            return State.CALCULATE_DIGIT;
        }

        public void processDigitDetection(PaperColor[] column)
        {
            if (this.initialFrameCount < INITIAL_FRAMES_IGNORED)
            {
                this.initialFrameCount++;
            }
            else
            {
                this.initialFrameCount = 0;

                switch (this.state)
                {
                    case State.WAIT_FOR_RUN:
                        this.state = this.waitForRun(column);
                        break;
                    case State.MOVE_LEFT:
                        this.state = this.moveLeft(column);
                        break;
                    case State.CHECK_LEFT:
                        this.state = this.checkLeft(column);
                        break;
                    case State.MOVE_RIGHT:
                        this.state = this.moveRight(column);
                        break;
                    case State.CHECK_RIGHT:
                        this.state = this.checkRight(column);
                        break;
                    case State.RETURN_TO_START:
                        this.state = this.returnToStart(column);
                        break;
                    case State.CHOOSE_DIRECTION:
                        this.state = this.chooseDirection(column);
                        break;
                    case State.PROCESS_COLUMN:
                        this.state = this.processColumn(column);
                        break;
                    case State.MOVE:
                        this.state = this.move(column);
                        break;
                    default:
                        this.state = this.waitForRun(column);
                        break;
                }
            }
        }
    }
}
