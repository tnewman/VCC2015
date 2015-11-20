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
        private State state = State.WAIT_FOR_RUN;

        enum State
        {
            WAIT_FOR_RUN,
            CHECK_LEFT,
            CHECK_RIGHT,
            PROCESS_ROW,
            CALCULATE_DIGIT
        }

        enum Direction
        {
            LEFT,
            RIGHT
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void processColumn()
        {

        }
    }
}
