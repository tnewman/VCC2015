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
            this.state = State.CHECK_DIRECTION;
        }

        public void Stop()
        {
            this.callback.DigitDetectionStopped();
            this.state = State.WAIT_FOR_RUN;
        }

        private State waitForRun(Paper[,] papers)
        {
            Stop();
            this.paperColumns = new List<PaperColor[]>();
            return State.WAIT_FOR_RUN;
        }

        private State calculateDigit(Paper[,] papers)
        {
            if(this.direction == Direction.LEFT)
            {
                this.paperColumns.Reverse();
            }

            PaperColor[] paperCells = new PaperColor[papers.Length];

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
