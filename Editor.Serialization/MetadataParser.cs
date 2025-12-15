using SadRogue.Primitives;
using System.Numerics;

namespace SadConsole.Editor.Serialization;

public static class MetadataParser
{
    public static object? ParseMetadataValue(string value, MetadataTypeEnum type)
    {
        return type switch
        {
            MetadataTypeEnum.String => value,
            MetadataTypeEnum.Integer => int.TryParse(value, out int intValue) ? intValue : null,
            MetadataTypeEnum.Float => float.TryParse(value, out float floatValue) ? floatValue : null,
            MetadataTypeEnum.Boolean => bool.TryParse(value, out bool boolValue) ? boolValue : null,
            MetadataTypeEnum.Point => ParsePoint(value),
            MetadataTypeEnum.PointFloat => ParsePointFloat(value),
            MetadataTypeEnum.Color => ParseColor(value),
            _ => null
        };
    }

    public static MetadataTypeEnum GetMetadataType(string value)
    {
        if (int.TryParse(value, out _))
            return MetadataTypeEnum.Integer;
        if (float.TryParse(value, out _))
            return MetadataTypeEnum.Float;
        if (bool.TryParse(value, out _))
            return MetadataTypeEnum.Boolean;
        if (ParseColor(value) is not null)
            return MetadataTypeEnum.Color;
        if (ParsePoint(value) is not null)
            return MetadataTypeEnum.Point;
        if (ParsePointFloat(value) is not null)
            return MetadataTypeEnum.PointFloat;
        return MetadataTypeEnum.String;
    }

    private static Point? ParsePoint(string value)
    {
        var parts = value.Split(',');
        if (parts.Length != 2)
            return null;
        if (int.TryParse(parts[0].Trim(), out int x) && int.TryParse(parts[1].Trim(), out int y))
        {
            return new Point(x, y);
        }
        return null;
    }

    private static Vector2? ParsePointFloat(string value)
    {
        var parts = value.Split(',');
        if (parts.Length != 2)
            return null;
        if (float.TryParse(parts[0].Trim(), out float x) && float.TryParse(parts[1].Trim(), out float y))
        {
            return new Vector2(x, y);
        }
        return null;
    }

    private static Color? ParseColor(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Trim();

        // Check for hex format: #RRGGBBAA or #RRGGBB
        if (value.StartsWith('#'))
        {
            string hex = value[1..];
            
            // Parse #RRGGBBAA format (8 hex digits)
            if (hex.Length == 8 && uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out uint rgba))
            {
                byte r = (byte)((rgba >> 24) & 0xFF);
                byte g = (byte)((rgba >> 16) & 0xFF);
                byte b = (byte)((rgba >> 8) & 0xFF);
                byte a = (byte)(rgba & 0xFF);
                return new Color(r, g, b, a);
            }
            // Parse #RRGGBB format (6 hex digits)
            else if (hex.Length == 6 && uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out uint rgb))
            {
                byte r = (byte)((rgb >> 16) & 0xFF);
                byte g = (byte)((rgb >> 8) & 0xFF);
                byte b = (byte)(rgb & 0xFF);
                return new Color(r, g, b, (byte)255);
            }
        }

        return null;
    }

    public static string ColorToHex(Color color)
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
    }
}

