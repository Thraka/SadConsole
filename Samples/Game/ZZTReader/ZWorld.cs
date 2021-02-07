using System.Collections.Generic;

namespace ZReader
{
    public class ZWorld
    {
        public short PlayerTorches;
        public short TorchCycles;
        public short EnergyCycles;
        public short Dead;
        public short PlayerScore;
        public byte WorldNameLength;
        public string WorldName;
        public string Flag0;
        public string Flag1;
        public string Flag2;
        public string Flag3;
        public string Flag4;
        public string Flag5;
        public string Flag6;
        public string Flag7;
        public string Flag8;
        public string Flag9;
        public short TimePassed;
        public short PlayerData;
        public byte Locked;

        public ZWorldStatus Status;
        public ZBoard[] Boards;

        private ZWorld() { }

        public static ZWorld Load(System.IO.Stream fileStream)
        {
            using (System.IO.BinaryReader binreader = new System.IO.BinaryReader(fileStream))
            {
                ZWorldStatus worldStatus = ZWorldStatus.Deserialize(binreader);
                ZWorld world = Deserialize(binreader);

                fileStream.Position = 512;

                var boards = new List<ZBoard>(worldStatus.BoardCount + 1);
                for (int i = 0; i < worldStatus.BoardCount + 1; i++)
                    boards.Add(ZBoard.Deserialize(binreader));

                world.Status = worldStatus;
                world.Boards = boards.ToArray();

                return world;
            }
        }

        public static ZWorld Deserialize(System.IO.BinaryReader reader)
        {
            ZWorld info = new ZWorld();
            info.PlayerTorches = reader.ReadInt16();
            info.TorchCycles = reader.ReadInt16();
            info.EnergyCycles = reader.ReadInt16();
            info.Dead = reader.ReadInt16();
            info.PlayerScore = reader.ReadInt16();
            //info.WorldNameLength = reader.ReadByte();
            info.WorldName = reader.ReadString();
            reader.ReadBytes(20 - info.WorldName.Length);

            info.Flag0 = reader.ReadString();
            reader.ReadBytes(20 - info.Flag0.Length);

            info.Flag1 = reader.ReadString();
            reader.ReadBytes(20 - info.Flag1.Length);

            info.Flag2 = reader.ReadString();
            reader.ReadBytes(20 - info.Flag2.Length);

            info.Flag3 = reader.ReadString();
            reader.ReadBytes(20 - info.Flag3.Length);

            info.Flag4 = reader.ReadString();
            reader.ReadBytes(20 - info.Flag4.Length);

            info.Flag5 = reader.ReadString();
            reader.ReadBytes(20 - info.Flag5.Length);

            info.Flag6 = reader.ReadString();
            reader.ReadBytes(20 - info.Flag6.Length);

            info.Flag7 = reader.ReadString();
            reader.ReadBytes(20 - info.Flag7.Length);

            info.Flag8 = reader.ReadString();
            reader.ReadBytes(20 - info.Flag8.Length);

            info.Flag9 = reader.ReadString();
            reader.ReadBytes(20 - info.Flag9.Length);

            info.TimePassed = reader.ReadInt16();
            info.PlayerData = reader.ReadInt16();
            info.Locked = reader.ReadByte();

            return info;
        }
    }
}
