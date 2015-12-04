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
            CHECK_DIRECTION,
            COLUMN_ANGLE_CORRECTION,
            COLUMN_DETECTION,
            STEER_TO_MOVE,
            STEER_ANGLE_CORRECTION,
            MOVE_FORWARD,
            MOVE_FORWARD_CORRECTION,
            STEER_TO_DETECT,
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

        private const double COLUMN_ANGLE_LEFT_CORRECTION_THRESHOLD = 0.48;
        private const double COLUMN_ANGLE_RIGHT_CORRECTION_THRESHOLD = 0.52;
        private const double FORWARD_CORRECTION_VERTICAL_THRESHOLD = 0.35;

        private Motor motor = new Motor();
        private State state = State.WAIT_FOR_RUN;
        private IDigitDetectionCallback callback;
        private Direction direction;
        private int initialFrameCount = 0;
        private bool stop = true;
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
            this.initialFrameCount = 0;
            this.callback.DigitDetectionStarted();
            this.state = State.CHECK_DIRECTION;
            this.stop = false;
        }

        public void Stop()
        {
            this.callback.DigitDetectionStopped();
            this.state = State.WAIT_FOR_RUN;
            this.stop = true;
        }

        private State waitForRun(Paper[,] papers)
        {
            Stop();
            this.paperColumns = new List<PaperColor[]>();
            return State.WAIT_FOR_RUN;
        }

        private State checkDirection(Paper[,] papers)
        {
            int leftPaperCount = 0;
            int rightPaperCount = 0;

            for(int i = 0; i < papers.GetLength(0); i++)
            {
                for(int j = 0; j < papers.GetLength(1); j++)
                {
                    if(papers[i,j].Color != PaperColor.UNKNOWN)
                    {
                        if(j == 0)
                        {
                            leftPaperCount++;
                        } else if(j == 2)
                        {
                            rightPaperCount++;
                        }
                    }
                }

                if (leftPaperCount > rightPaperCount)
                {
                    this.direction = Direction.LEFT;
                }
                else
                {
                    this.direction = Direction.RIGHT;
                }
            }

            return State.COLUMN_ANGLE_CORRECTION;
        }

        private State columnAngleCorrection(Paper[,] papers)
        {
            Paper topCenterCellPaper = papers[0,1];

            if(topCenterCellPaper.XMidPoint < topCenterCellPaper.ParentImageWidth * COLUMN_ANGLE_LEFT_CORRECTION_THRESHOLD)
            {
                this.motor.turn3DegreesLeft();
            }
            else if (topCenterCellPaper.XMidPoint > topCenterCellPaper.ParentImageWidth * COLUMN_ANGLE_RIGHT_CORRECTION_THRESHOLD)
            {
                this.motor.turn3DegreesRight();
            } else
            {
                return State.COLUMN_DETECTION;
            }

            return State.COLUMN_ANGLE_CORRECTION;
        }

        private State columnDetection(Paper[,] papers)
        {
            PaperColor[] paperColorColumn = new PaperColor[3];
            paperColorColumn[0] = papers[0, 1].Color;
            paperColorColumn[1] = papers[1, 1].Color;
            paperColorColumn[2] = papers[2, 1].Color;

            this.paperColumns.Add(paperColorColumn);

            if (this.paperColumns.Count < COLUMN_COUNT)
            {
                return State.STEER_TO_MOVE;
            }
            else
            {
                return State.CALCULATE_DIGIT;
            }
        }

        private State steerToMove(Paper[,] papers)
        {
            if (this.direction == Direction.LEFT)
            {
                this.motor.turn90DegreesLeft();
            }
            else
            {
                this.motor.turn90DegreesRight();
            }

            return State.STEER_ANGLE_CORRECTION;
        }

        private State steerAngleCorrection(Paper[,] papers)
        {
            this.motor.turn3DegreesLeft();
            this.motor.turn3DegreesLeft();

            return State.MOVE_FORWARD;
        }

        private State moveForward(Paper[,] papers)
        {
            this.motor.driveForward();

            return State.MOVE_FORWARD_CORRECTION;
        }

        private State moveForwardCorrection(Paper[,] papers)
        {
            Paper paper;

            if (this.direction == Direction.LEFT)
            {
                paper = papers[1, 2];
            }
            else
            {
                paper = papers[1, 0];
            }

            if (paper.Color != PaperColor.UNKNOWN)
            {
                if (paper.YMidPoint < paper.ParentImageHeight * FORWARD_CORRECTION_VERTICAL_THRESHOLD)
                {
                    this.motor.driveForwardCorrection();
                }
            }

            return State.STEER_TO_DETECT;
        }

        private State steerToDetect(Paper[,] papers)
        {
            if(this.direction == Direction.LEFT)
            {
                this.motor.turn90DegreesRight();
            }
            else
            {
                this.motor.turn90DegreesLeft();
            }

            return State.COLUMN_ANGLE_CORRECTION;
        }

        private State calculateDigit(Paper[,] papers)
        {
            if(this.direction == Direction.LEFT)
            {
                this.paperColumns.Reverse();
            }

            PaperColor[] paperCells = new PaperColor[this.paperColumns.Count * 3];

            for (int i = 0; i < papers.GetLength(0); i++)
            {
                for(int j = 0; j < papers.GetLength(1); j++)
                {
                    paperCells[i * papers.GetLength(0) + j] = this.paperColumns[i][j];
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

        public void processDigitDetection(Paper[,] papers)
        {
            if(this.stop)
            {
                this.state = State.WAIT_FOR_RUN;
            }

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
                        this.state = this.waitForRun(papers);
                        break;
                    case State.CHECK_DIRECTION:
                        this.state = this.checkDirection(papers);
                        break;
                    case State.COLUMN_ANGLE_CORRECTION:
                        this.state = this.columnAngleCorrection(papers);
                        break;
                    case State.COLUMN_DETECTION:
                        this.state = this.columnDetection(papers);
                        break;
                    case State.STEER_TO_MOVE:
                        this.state = this.steerToMove(papers);
                        break;
                    case State.STEER_ANGLE_CORRECTION:
                        this.state = this.steerAngleCorrection(papers);
                        break;
                    case State.MOVE_FORWARD:
                        this.state = this.moveForward(papers);
                        break;
                    case State.MOVE_FORWARD_CORRECTION:
                        this.state = this.moveForwardCorrection(papers);
                        break;
                    case State.STEER_TO_DETECT:
                        this.state = this.steerToDetect(papers);
                        break;
                    case State.CALCULATE_DIGIT:
                        this.state = this.calculateDigit(papers);
                        break;
                    default:
                        this.state = this.waitForRun(papers);
                        break;
                }
            }
        }
    }
}
