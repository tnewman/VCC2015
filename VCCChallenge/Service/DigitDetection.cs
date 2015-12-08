// Copyright 2015 Thomas Newman

using System;
using System.Collections.Generic;

namespace VCCChallenge
{
    /// <summary>
    /// State Machine for Digit Detection. The state machine handles 
    /// processing a 3X3 grid of papers, moving the robot from column 
    /// to column, providing corrective movements and calculating the 
    /// digit.
    /// </summary>
    class DigitDetection
    {
        /// <summary>
        /// Digit Detection States
        /// </summary>
        enum State
        {
            /// <summary>
            /// Default state while the robot is waiting for instructions.
            /// </summary>
            WAIT_FOR_RUN,

            /// <summary>
            /// Check what side of the robot the paper is on.
            /// </summary>
            CHECK_DIRECTION,

            /// <summary>
            /// Provide corrective turns before viewing the current column.
            /// </summary>
            COLUMN_ANGLE_CORRECTION,

            /// <summary>
            /// Detect paper in the current column.
            /// </summary>
            COLUMN_DETECTION,

            /// <summary>
            /// Steer to move to the next column.
            /// </summary>
            STEER_TO_MOVE,

            /// <summary>
            /// Provide corrective turns before moving in case the robot 
            /// is not perpendicular to the paper grid.
            /// </summary>
            STEER_ANGLE_CORRECTION,

            /// <summary>
            /// Move forward to the next column.
            /// </summary>
            MOVE_FORWARD,

            /// <summary>
            /// Move forward more if the robot will not be centered with 
            /// the column.
            /// </summary>
            MOVE_FORWARD_CORRECTION,

            /// <summary>
            /// Steer to view the column.
            /// </summary>
            STEER_TO_DETECT,

            /// <summary>
            /// Calculate the digit based on the paper locations and colors 
            /// collected.
            /// </summary>
            CALCULATE_DIGIT,

            /// <summary>
            /// Steer 720 degrees to signal completion.
            /// </summary>
            STEER_720_DEGREES
        }

        /// <summary>
        /// The side of the robot the paper is on when the robot starts.
        /// </summary>
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
        private const double FORWARD_CORRECTION_VERTICAL_THRESHOLD = 0.29;

        private Motor motor = new Motor();
        private State state = State.WAIT_FOR_RUN;
        private IDigitDetectionCallback callback;
        private Direction direction;
        private int initialFrameCount = 0;
        private bool stop = true;
        private PaperColor[,] digitsToMatch;
        private List<PaperColor[]> paperColumns;

        /// <summary>
        /// Callback to send status to the UI.
        /// </summary>
        /// <param name="callback">Callback object.</param>
        public DigitDetection(IDigitDetectionCallback callback)
        {
            this.callback = callback;
            this.setDigitsToMatch();
        }

        /// <summary>
        /// Patterns for each possible digit, in order. For example, 
        /// element 0 represents the digit 0, element 1 represents 
        /// the digit 1 and soforth.
        /// </summary>
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

        /// <summary>
        /// Start digit processing and robot movement.
        /// </summary>
        public void Start()
        {
            this.initialFrameCount = 0;
            this.callback.DigitDetectionStarted();
            this.state = State.CHECK_DIRECTION;
            this.stop = false;
        }

        /// <summary>
        /// Immediately stop digit processing and robot movement.
        /// </summary>
        public void Stop()
        {
            this.callback.DigitDetectionStopped();
            this.state = State.WAIT_FOR_RUN;
            this.stop = true;
        }


        /// <summary>
        /// Do nothing.
        /// See <see cref="State.WAIT_FOR_RUN"/>.
        /// </summary>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns>Next State</returns>
        private State waitForRun(Paper[,] papers)
        {
            Stop();
            this.paperColumns = new List<PaperColor[]>();
            return State.WAIT_FOR_RUN;
        }

        /// <summary>
        /// Check what side of the robot the columns are on and 
        /// sets the direction used throughout the state machine.
        /// See <see cref="State.CHECK_DIRECTION"/>.
        /// </summary>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns><see cref="State.COLUMN_ANGLE_CORRECTION"/></returns>
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

        /// <summary>
        /// Checks of the column is centered in the robot's field of 
        /// vision and makes corrective turns if it isn't.
        /// See <see cref="State.COLUMN_ANGLE_CORRECTION"/>.
        /// </summary>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns><see cref="State.COLUMN_DETECTION"/> if the column 
        /// is centered, otherwise <see cref="State.COLUMN_DETECTION"/>.</returns>
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

        /// <summary>
        /// Detects and stores the center columns in the current paper grid.
        /// See <see cref="State.COLUMN_DETECTION"/>.
        /// </summary>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns><see cref="State.STEER_TO_MOVE"/> if there are more 
        /// columns. <see cref="State.CALCULATE_DIGIT"/> if all columns 
        /// have now been checked.</returns>
        private State columnDetection(Paper[,] papers)
        {
            PaperColor[] paperColorColumn = new PaperColor[3];

            // Top Column
            paperColorColumn[0] = papers[0, 1].Color;

            // Middle Column
            paperColorColumn[1] = papers[1, 1].Color;

            // Bottom Column
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

        /// <summary>
        /// Steer to move to the next column.
        /// See <see cref="State.STEER_TO_MOVE"/>.
        /// </summary>
        /// <remarks>This always applies a static 
        /// steering correction when moving left because the robot would 
        /// not turn adequately under testing. This should be revisited 
        /// for specific applications.</remarks>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns><see cref="State.STEER_ANGLE_CORRECTION"/></returns>
        private State steerToMove(Paper[,] papers)
        {
            if (this.direction == Direction.LEFT)
            {
                this.motor.turn90DegreesLeft();
                this.motor.turn3DegreesLeft();
            }
            else
            {
                this.motor.turn90DegreesRight();
            }

            return State.STEER_ANGLE_CORRECTION;
        }

        /// <summary>
        /// Perform a corrective turn, so the robot drives perpendicular 
        /// to the paper.
        /// See <see cref="State.STEER_ANGLE_CORRECTION"/>.
        /// </summary>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns><see cref="State.STEER_ANGLE_CORRECTION"/> if a 
        /// correction is made. <see cref="State.MOVE_FORWARD"/> if 
        /// no correction was made.</returns>
        private State steerAngleCorrection(Paper[,] papers)
        {
            if (this.direction == Direction.LEFT)
            {
                if (papers[2, 2].Color != PaperColor.UNKNOWN)
                {
                    this.motor.turn3DegreesLeft();
                    return State.STEER_ANGLE_CORRECTION;
                }
            }
            else
            {
                if (papers[2, 0].Color != PaperColor.UNKNOWN)
                {
                    this.motor.turn3DegreesRight();
                    return State.STEER_ANGLE_CORRECTION;
                }
            }

            return State.MOVE_FORWARD;
        }

        /// <summary>
        /// Drives forward to the next column.
        /// See <see cref="State.MOVE_FORWARD"/>.
        /// </summary>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns><see cref="State.MOVE_FORWARD_CORRECTION"/></returns>
        private State moveForward(Paper[,] papers)
        {
            this.motor.driveForward();

            if (this.direction == Direction.RIGHT)
            {
                this.motor.driveForwardCorrection();
                this.motor.driveForwardCorrection();
                this.motor.driveForwardCorrection();
            }

            return State.MOVE_FORWARD_CORRECTION;
        }

        /// <summary>
        /// Drives forward slightly if the robot did not travel 
        /// an adequate distance.
        /// See <see cref="State.MOVE_FORWARD_CORRECTION"/>.
        /// </summary>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns><see cref="State.MOVE_FORWARD_CORRECTION"/> 
        /// if a correction is made, otherwise 
        /// <see cref="State.STEER_TO_DETECT"/></returns>
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

                    return State.MOVE_FORWARD_CORRECTION;
                }
            }

            return State.STEER_TO_DETECT;
        }

        /// <summary>
        /// Steer to view the papers in a column.
        /// See <see cref="State.STEER_TO_DETECT"/>.
        /// </summary>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns><see cref="State.COLUMN_ANGLE_CORRECTION"/></returns>
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

        /// <summary>
        /// Calculates the digit represented by all of the 
        /// paper color columns. The detected digit is sent 
        /// to the UI using 
        /// <see cref="IDigitDetectionCallback.DigitDetected(int, PaperColor[])"/>.
        /// </summary>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns><see cref="State.STEER_720_DEGREES"/></returns>
        private State calculateDigit(Paper[,] papers)
        {
            // The columns are read top to bottom; however, the digit needs to 
            // be  bottom to top, which translate into left to right when the 
            // digit is compared.
            for (int i = 0; i < this.paperColumns.Count; i++)
            {
                PaperColor temp = this.paperColumns[i][0];
                this.paperColumns[i][0] = this.paperColumns[i][2];
                this.paperColumns[i][2] = temp;
            }

            if (this.direction == Direction.LEFT)
            {
                this.paperColumns.Reverse();
            }

            // Convert paper color columns to an array
            PaperColor[] paperColors = new PaperColor[this.paperColumns.Count * 3];

            for (int i = 0; i < paperColumns.Count; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    paperColors[i * papers.GetLength(0) + j] = this.paperColumns[i][j];
                }
            }

            int matchNumber = 0;
            int matchCount = 0;

            // Find the digit that is the closest match. The idea behind 
            // this algorithm is that the closest match is the digit with 
            // the most papers in common with the digit represented by 
            // the paper viewed by the robot. In the event that the robot 
            // is unable to read a sheet of paper, this algorithm assumes 
            // that it would have been a match.
            for(int i = 0; i < this.digitsToMatch.GetLength(0); i++)
            {
                int currentMatchCount = 0;

                for(int j = 0; j < this.digitsToMatch.GetLength(1); j++)
                {
                    if(paperColors[j] == PaperColor.UNKNOWN)
                    {
                        currentMatchCount++;
                    } else if(paperColors[j] == this.digitsToMatch[i, j])
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

            this.callback.DigitDetected(matchNumber, paperColors);

            return State.STEER_720_DEGREES;
        }

        /// <summary>
        /// Steer 720 Degrees to indicate that the digit has been 
        /// detected.
        /// See <see cref="State.WAIT_FOR_RUN"/>.
        /// </summary>
        /// <param name="papers">Current Paper Grid</param>
        /// <returns></returns>
        private State steer720Degrees(Paper[,] papers)
        {
            // 8 90 degree turns is 720 degrees
            for(int i = 0; i < 8; i++)
            {
                this.motor.turn90DegreesRight();
            }

            return State.WAIT_FOR_RUN;
        }

        /// <summary>
        /// Run the state machine for the current grid of papers detected.
        /// </summary>
        /// <param name="papers">Current grid of papers detected.</param>
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
                    case State.STEER_720_DEGREES:
                        this.state = this.steer720Degrees(papers);
                        break;
                    default:
                        this.state = this.waitForRun(papers);
                        break;
                }
            }
        }
    }
}
