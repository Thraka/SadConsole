using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZReader
{
    public class ZStatusElement
    {
        public byte LocationX;
        public byte LocationY;
        public short StepX;
        public short StepY;
        public short Cycle;
        public byte Parameter1;
        public byte Parameter2;
        public byte Parameter3;
        public short Follower;
        public short Leader;
        public byte UnderID;
        public byte UnderColor;
        public int Pointer;
        public short CurrentInstruction;
        public short InstructionsLength;
        public byte[] CodeBlock;


        public static ZStatusElement Deserialize(System.IO.BinaryReader reader)
        {
            ZStatusElement info = new ZStatusElement();
            info.LocationX = reader.ReadByte();
            info.LocationY = reader.ReadByte();
            info.StepX = reader.ReadInt16();
            info.StepY = reader.ReadInt16();
            info.Cycle = reader.ReadInt16();
            info.Parameter1 = reader.ReadByte();
            info.Parameter2 = reader.ReadByte();
            info.Parameter3 = reader.ReadByte();
            info.Follower = reader.ReadInt16();
            info.Leader = reader.ReadInt16();
            info.UnderID = reader.ReadByte();
            info.UnderColor = reader.ReadByte();
            info.Pointer = reader.ReadInt32();
            info.CurrentInstruction = reader.ReadInt16();
            info.InstructionsLength = reader.ReadInt16();
            reader.ReadBytes(8);

            if (info.InstructionsLength != 0)
                info.CodeBlock = reader.ReadBytes(info.InstructionsLength);

            return info;
        }
    }
}
