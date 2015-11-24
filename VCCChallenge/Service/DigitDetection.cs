﻿// Copyright 2015 Thomas Newman

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

        public enum Direction
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
        private PaperColor[,] digitsToMatch;
        private List<PaperColor[]> paperColumns;

        public DigitDetection(IDigitDetectionCallback callback)
        {
            this.callback = callback;
            this.setDigitsToMatch();
        }

        private void setDigitsToMatch()
        {
            this.digitsToMatch = new PaperColor[,]{
                {PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW},
                {PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN},
                {PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW},
                {PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW},
                {PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW},
                {PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW},
                {PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW},
                {PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW},
                {PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW},
                {PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW, PaperColor.GREEN, PaperColor.GREEN, PaperColor.YELLOW}
            };
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

        private State waitForRun(PaperColor[] column, Direction rotationCorrectionDirection)
        {
            Stop();
            return State.WAIT_FOR_RUN;
        }

        private State moveLeft(PaperColor[] column, Direction rotationCorrectionDirection)
        {
            this.motor.turn22DegreesLeft();

            return State.CHECK_LEFT;
        }

        private State checkLeft(PaperColor[] column, Direction rotationCorrectionDirection)
        {
            this.leftColumns = 0;

            foreach(PaperColor row in column)
            {
                if (!row.Equals(PaperColor.UNKNOWN))
                {
                    this.leftColumns++;
                }
            }

            return State.MOVE_RIGHT;
        }

        private State moveRight(PaperColor[] column, Direction rotationCorrectionDirection)
        {
            this.motor.turn22DegreesRight();
            this.motor.turn22DegreesRight();

            return State.CHECK_RIGHT;
        }

        private State checkRight(PaperColor[] column, Direction rotationCorrectionDirection)
        {
            this.rightColumns = 0;

            foreach (PaperColor row in column)
            {
                if (!row.Equals(PaperColor.UNKNOWN))
                {
                    this.rightColumns++;
                }
            }

            return State.RETURN_TO_START;
        }

        private State returnToStart(PaperColor[] column, Direction rotationCorrectionDirection)
        {
            this.motor.turn22DegreesLeft();

            return State.CHOOSE_DIRECTION;
        }

        private State chooseDirection(PaperColor[] column, Direction rotationCorrectionDirection)
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

        private State processColumn(PaperColor[] column, Direction rotationCorrectionDirection)
        {
            if (rotationCorrectionDirection == Direction.LEFT)
            {
                this.motor.turn6DegreesRight();

                return State.PROCESS_COLUMN;
            }
            else if (rotationCorrectionDirection == Direction.RIGHT)
            {
                this.motor.turn6DegreesLeft();

                return State.PROCESS_COLUMN;
            }
            else
            {
                this.paperColumns.Add(column);

                if (this.paperColumns.Count < COLUMN_COUNT)
                {
                    if (this.direction == Direction.LEFT)
                    {
                        this.motor.turn90DegreesLeft();
                        this.motor.driveForward();
                        this.motor.turn90DegreesRight();
                    }
                    else if (this.direction == Direction.RIGHT)
                    {
                        this.motor.turn90DegreesRight();
                        this.motor.driveForward();
                        this.motor.turn90DegreesLeft();
                    }
                    else
                    {
                        return State.WAIT_FOR_RUN;
                    }

                    return State.PROCESS_COLUMN;
                }
                else
                {
                    return State.CALCULATE_DIGIT;
                }
            }
        }

        private State calculateDigit(PaperColor[] column, Direction rotationCorrectionDirection)
        {
            if(this.direction == Direction.LEFT)
            {
                this.paperColumns.Reverse();
            }

            PaperColor[] paperCells = new PaperColor[this.paperColumns.Count * column.Length];

            for (int i = 0; i < this.paperColumns.Count; i++)
            {
                for(int j = 0; j < column.Length; j++)
                {
                    paperCells[i * column.Length + j] = this.paperColumns[i][j];
                }
            }

            int matchNumber = 0;
            int matchCount = 0;

            for(int i = 0; i < this.digitsToMatch.GetLength(0); i++)
            {
                int currentMatchCount = 0;

                for(int j = 0; j < this.digitsToMatch.GetLength(1); j++)
                {
                    if(paperCells[j] == PaperColor.UNKNOWN)
                    {
                        currentMatchCount++;
                    } else if(paperCells[j] == this.digitsToMatch[i, j])
                    {
                        currentMatchCount++;
                    }
                }

                if(currentMatchCount > matchCount)
                {
                    matchNumber = i;
                    matchCount = currentMatchCount;
                }
            }

            this.callback.DigitDetected(matchNumber);

            return State.WAIT_FOR_RUN;
        }

        public void processDigitDetection(PaperColor[] column, Direction rotationCorrectionDirection)
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
                        this.state = this.waitForRun(column, rotationCorrectionDirection);
                        break;
                    case State.MOVE_LEFT:
                        this.state = this.moveLeft(column, rotationCorrectionDirection);
                        break;
                    case State.CHECK_LEFT:
                        this.state = this.checkLeft(column, rotationCorrectionDirection);
                        break;
                    case State.MOVE_RIGHT:
                        this.state = this.moveRight(column, rotationCorrectionDirection);
                        break;
                    case State.CHECK_RIGHT:
                        this.state = this.checkRight(column, rotationCorrectionDirection);
                        break;
                    case State.RETURN_TO_START:
                        this.state = this.returnToStart(column, rotationCorrectionDirection);
                        break;
                    case State.CHOOSE_DIRECTION:
                        this.state = this.chooseDirection(column, rotationCorrectionDirection);
                        break;
                    case State.PROCESS_COLUMN:
                        this.state = this.processColumn(column, rotationCorrectionDirection);
                        break;
                    case State.CALCULATE_DIGIT:
                        this.state = this.calculateDigit(column, rotationCorrectionDirection);
                        break;
                    default:
                        this.state = this.waitForRun(column, rotationCorrectionDirection);
                        break;
                }
            }
        }
    }
}
