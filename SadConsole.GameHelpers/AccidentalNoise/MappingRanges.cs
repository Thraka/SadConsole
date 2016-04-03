using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public class MappingRanges
    {
        public static readonly MappingRanges Default = new MappingRanges();

        public Double MapX0 = -1;
        public Double MapY0 = -1;
        public Double MapZ0 = -1;
        public Double MapX1 = 1;
        public Double MapY1 = 1;
        public Double MapZ1 = 1;

        public Double LoopX0 = -1;
        public Double LoopY0 = -1;
        public Double LoopZ0 = -1;
        public Double LoopX1 = 1;
        public Double LoopY1 = 1;
        public Double LoopZ1 = 1;
    }
}