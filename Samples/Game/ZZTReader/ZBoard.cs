namespace ZReader;

public class ZBoard
{
    public const int DataType_Count = 0;
    public const int DataType_ElementId = 1;
    public const int DataType_Data = 2;

    public short BoardSize;
    public string BoardName;
    public byte[][] BoardData;
    public byte MaxPlayerShots;
    public byte IsDark;
    public byte ExitNorth;
    public byte ExitSouth;
    public byte ExitWest;
    public byte ExitEast;
    public byte RestartOnZap;
    public string Message;
    public byte PlayerEnterX;
    public byte PlayerEnterY;
    public short Timelimit;
    public short StatElementCount;

    public ZStatusElement Player;
    public ZStatusElement[] Elements;

    public static ZBoard Deserialize(System.IO.BinaryReader reader)
    {
        ZBoard info = new ZBoard();
        info.BoardSize = reader.ReadInt16();
        info.BoardName = reader.ReadString();
        reader.ReadBytes(50 - info.BoardName.Length);

        List<byte[]> bytes = new List<byte[]>(10);
        int tileCounter = 0;
        do
        {
            byte[] byteData;
            byteData = reader.ReadBytes(3);
            if (byteData[0] == 0)
                tileCounter += 256;
            else
                tileCounter += byteData[0];
            bytes.Add(byteData);

        } while (tileCounter != 1500);
        info.BoardData = bytes.ToArray();

        info.MaxPlayerShots = reader.ReadByte();
        info.IsDark = reader.ReadByte();
        info.ExitNorth = reader.ReadByte();
        info.ExitSouth = reader.ReadByte();
        info.ExitWest = reader.ReadByte();
        info.ExitEast = reader.ReadByte();
        info.RestartOnZap = reader.ReadByte();
        info.Message = reader.ReadString();
        reader.ReadBytes(58 - info.Message.Length);
        info.PlayerEnterX = reader.ReadByte();
        info.PlayerEnterY = reader.ReadByte();
        info.Timelimit = reader.ReadInt16();
        reader.ReadBytes(16);
        info.StatElementCount = reader.ReadInt16();

        info.Player = ZStatusElement.Deserialize(reader);

        List<ZStatusElement> elements = new List<ZStatusElement>(info.StatElementCount);
        for (int i = 0; i < info.StatElementCount; i++)
        {
            elements.Add(ZStatusElement.Deserialize(reader));
        }
        info.Elements = elements.ToArray();

        return info;
    }
}
