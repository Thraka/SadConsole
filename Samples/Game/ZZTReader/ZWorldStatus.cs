using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZReader
{
    public class ZWorldStatus
    {
        public short TypeOfWorld;
        public short BoardCount;
        public short PlayersAmmo;
        public short PlayersGems;
        public byte[] PlayersKeys;
        public short PlayersHealth;
        public short PlayersBoard;

        public static ZWorldStatus Deserialize(System.IO.BinaryReader reader)
        {
            ZWorldStatus info = new ZWorldStatus();
            info.TypeOfWorld = reader.ReadInt16();
            info.BoardCount = reader.ReadInt16();
            info.PlayersAmmo = reader.ReadInt16();
            info.PlayersGems = reader.ReadInt16();
            info.PlayersKeys = reader.ReadBytes(7);
            info.PlayersHealth = reader.ReadInt16();
            info.PlayersBoard = reader.ReadInt16();
            return info;
        }
    }
}
