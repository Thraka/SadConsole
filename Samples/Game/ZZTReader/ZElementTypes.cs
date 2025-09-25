namespace ZReader;

public class ZElement
{
    public enum Types : byte
    {
        Empty = 0,
        BoardEdge = 1,
        Messenger = 2,
        Monitor = 3,
        Player = 4,
        Ammo = 5,
        Torch = 6,
        Gem = 7,
        Key = 8,
        Door = 9,
        Scroll = 10,
        Passage = 11,
        Duplicator = 12,
        Bomb = 13,
        Energizer = 14,
        Star = 15,
        Clockwise = 16,
        Counter = 17,
        Bullet = 18,
        Water = 19,
        Forest = 20,
        SolidWall = 21,
        NormalWall = 22,
        BreakableWall = 23,
        Boulder = 24,
        SliderNS = 25,
        SliderEW = 26,
        FakeWall = 27,
        Invisible = 28,
        BlinkWall = 29,
        Transporter = 30,
        Line = 31,
        Ricochet = 32,
        BlinkRayHorizontal = 33,
        Bear = 34,
        Ruffian = 35,
        Object = 36,
        Slime = 37,
        Shark = 38,
        SpinningGun = 39,
        Pusher = 40,
        Lion = 41,
        Tiger = 42,
        BlinkRayVertical = 43,
        Head = 44,
        Segment = 45,

        TextBlue = 47,
        TextGreen = 48,
        TextCyan = 49,
        TextRed = 50,
        TextPurple = 51,
        TextBrown = 52,
        TextBlack = 53
    }

    public enum Categories
    {
        Other,
        Item,
        Creature,
        Terrain
    }

    public Types Type { get; }

    public Categories Category { get; }

    public byte Glyph { get; set; }

    public byte ForeColor { get; set; }

    public byte BackColor { get; set; }

    public byte Data { get; set; }

    public byte GetHighNibble(byte value) => (byte)((value & 0x70) >> 4);

    public byte GetLowNibble(byte value) => (byte)(value & 0x0F);

    public void ParseColor(byte value)
    {
        if (value > 15)
        {
            ForeColor = GetLowNibble(value);
            BackColor = GetHighNibble(value);
        }
        else
            ForeColor = value;
    }

    public ZElement(Types type, byte data = 0x00)
    {
        Type = type;
        Data = data;

        switch (type)
        {
            case Types.Empty:
                Glyph = 0x20;
                ForeColor = GetHighNibble(0x70);
                break;
            case Types.BoardEdge:
            case Types.Messenger:
                Glyph = 0x20;
                ParseColor(data);
                break;
            case Types.Monitor:
                Glyph = 0x20;
                ForeColor = 0x07;
                break;
            case Types.Player:
                Glyph = 0x02;
                ForeColor = GetLowNibble(0x1F);
                BackColor = GetHighNibble(0x1F);
                break;
            case Types.Ammo:
                Glyph = 0x84;
                ForeColor = 0x03;
                break;
            case Types.Torch:
                Glyph = 0x9D;
                ForeColor = 0x06;
                break;
            case Types.Gem:
                Glyph = 0x04;
                ParseColor(data);
                break;
            case Types.Key:
                Glyph = 0x0C;
                ParseColor(data);
                break;
            case Types.Door:
                Glyph = 0x0A;
                ForeColor = 0x0F;
                BackColor = GetHighNibble(data);
                break;
            case Types.Scroll:
                Glyph = 0xE8;
                ForeColor = 0x0F;
                break;
            case Types.Passage:
                Glyph = 0xF0;
                ForeColor = 0x0F;
                BackColor = GetHighNibble(data);
                break;
            case Types.Duplicator:
                Glyph = 0xFA;
                ForeColor = 0x0F;
                break;
            case Types.Bomb:
                Glyph = 0x0B;
                ParseColor(data);
                break;
            case Types.Energizer:
                Glyph = 0x7F;
                ForeColor = 0x05;
                break;
            case Types.Star:
                Glyph = 0x53;
                ForeColor = 0x0F;
                break;
            case Types.Clockwise:
                Glyph = 0x2F;
                ParseColor(data);
                break;
            case Types.Counter:
                Glyph = 0x5C;
                ParseColor(data);
                break;
            case Types.Bullet:
                Glyph = 0xF8;
                ForeColor = 0x0F;
                break;
            case Types.Water:
                Glyph = 0xB0;
                ParseColor(0xF9);
                break;
            case Types.Forest:
                Glyph = 0xB0;
                ParseColor(0x20);
                break;
            case Types.SolidWall:
                Glyph = 0xDB;
                ParseColor(data);
                break;
            case Types.NormalWall:
                Glyph = 0xB2;
                ParseColor(data);
                break;
            case Types.BreakableWall:
                Glyph = 0xB1;
                ParseColor(data);
                break;
            case Types.Boulder:
                Glyph = 0xFE;
                ParseColor(data);
                break;
            case Types.SliderNS:
                Glyph = 0x12;
                ParseColor(data);
                break;
            case Types.SliderEW:
                Glyph = 0x1D;
                ParseColor(data);
                break;
            case Types.FakeWall:
                Glyph = 0xB2;
                ParseColor(data);
                break;
            case Types.Invisible:
                Glyph = 0xB0;
                ParseColor(data);
                break;
            case Types.BlinkWall:
                Glyph = 0xCE;
                ParseColor(data);
                break;
            case Types.Transporter:
                Glyph = 0xC5;
                ParseColor(data);
                break;
            case Types.Line:
                Glyph = 0xCE;
                ParseColor(data);
                break;
            case Types.Ricochet:
                Glyph = 0x2A;
                ForeColor = 0x0A;
                break;
            case Types.BlinkRayHorizontal:
                Glyph = 0xCD;
                ParseColor(data);
                break;
            case Types.Bear:
                Glyph = 0x99;
                ForeColor = 0x06;
                break;
            case Types.Ruffian:
                Glyph = 0x05;
                ForeColor = 0x0D;
                break;
            case Types.Object:
                Glyph = 0x02;
                ParseColor(data);
                break;
            case Types.Slime:
                Glyph = 0x2A;
                ParseColor(data);
                break;
            case Types.Shark:
                Glyph = 0x5E;
                ForeColor = 0x07;
                break;
            case Types.SpinningGun:
                Glyph = 0x18;
                ParseColor(data);
                break;
            case Types.Pusher:
                Glyph = 0x10;
                ParseColor(data);
                break;
            case Types.Lion:
                Glyph = 0xEA;
                ForeColor = 0x0C;
                break;
            case Types.Tiger:
                Glyph = 0xE3;
                ForeColor = 0x0B;
                break;
            case Types.BlinkRayVertical:
                Glyph = 0xBA;
                ParseColor(data);
                break;
            case Types.Head:
                Glyph = 0xE9;
                ParseColor(data);
                break;
            case Types.Segment:
                Glyph = 0x4F;
                ParseColor(data);
                break;
            case Types.TextBlue:
                Glyph = data;
                ForeColor = 15;
                BackColor = 1;
                break;
            case Types.TextGreen:
                Glyph = data;
                ForeColor = 15;
                BackColor = 2;
                break;
            case Types.TextCyan:
                Glyph = data;
                ForeColor = 15;
                BackColor = 3;
                break;
            case Types.TextRed:
                Glyph = data;
                ForeColor = 15;
                BackColor = 4;
                break;
            case Types.TextPurple:
                Glyph = data;
                ForeColor = 15;
                BackColor = 5;
                break;
            case Types.TextBrown:
                Glyph = data;
                ForeColor = 15;
                BackColor = 6;
                break;
            case Types.TextBlack:
                Glyph = data;
                ForeColor = 15;
                BackColor = 0;
                break;
            default:
                break;
        }

        if ((Type >= Types.Player && Type <= Types.Energizer) ||
            (Type == Types.Clockwise || Type == Types.Counter))
            Category = Categories.Item;

        else if ((Type >= Types.Empty && type <= Types.Monitor) ||
                 (Type >= Types.Water && type <= Types.BlinkRayHorizontal) ||
                 Type == Types.BlinkRayVertical ||
                 (Type >= Types.TextBlue && Type <= Types.TextBlack))
            Category = Categories.Terrain;

        else if (Type == Types.Bullet || type == Types.Star || Type == Types.Head || type == Types.Segment ||
                (Type >= Types.Bear && type <= Types.Tiger))
            Category = Categories.Creature;

    }

    public static bool IsTerrain(Types type) =>
        new ZElement(type, 0).Category == Categories.Terrain;

    public static bool IsObjectType(Types type)
    {
        var element = new ZElement(type, 0);
        return element.Category == Categories.Creature || element.Category == Categories.Item;
    }

    public static bool IsItem(Types type) =>
        new ZElement(type, 0).Category == Categories.Item;

    public static bool IsCreature(Types type) =>
        new ZElement(type, 0).Category == Categories.Creature;

}
