namespace SadConsole.Editor;

public record Blueprint(string Name, string FilePath)
{
    public CellSurface GetSurface()
    {
        using BinaryReader reader = new(File.OpenRead(FilePath));

        string name = reader.ReadString();
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        Color defaultForeground = new(reader.ReadUInt32());
        Color defaultBackground = new(reader.ReadUInt32());
        int defaultGlyph = reader.ReadInt32();

        Rectangle view = new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

        ColoredGlyph[] cells = new ColoredGlyph[width * height];

        for (int counter = 0; counter < width * height; counter++)
        {
            cells[counter] = new(
                foreground: new(reader.ReadUInt32()),
                background: new(reader.ReadUInt32()),
                glyph: reader.ReadInt32(),
                mirror: (Mirror)reader.ReadByte(),
                isVisible: reader.ReadBoolean());

            byte decoratorCount = reader.ReadByte();
            if (decoratorCount != 0)
            {
                CellDecorator[] decorators = new CellDecorator[decoratorCount];
                for (int i = 0; i < decoratorCount; i++)
                {
                    decorators[i] = new CellDecorator(
                        color: new Color(reader.ReadUInt32()),
                        glyph: reader.ReadInt32(),
                        mirror: (Mirror)reader.ReadByte());
                }

                CellDecoratorHelpers.SetDecorators(decorators, cells[counter]);
            }
        }

        CellSurface surface = new(view.Width, view.Height, width, height, cells)
        {
            DefaultBackground = defaultBackground,
            DefaultForeground = defaultForeground,
            DefaultGlyph = defaultGlyph
        };

        surface.ViewPosition = view.Position;

        return surface;
    }

    public static Blueprint CreateAndSave(string name, string filePath, CellSurface surface)
    {
        using MemoryStream baseStream = new();
        using BinaryWriter writer = new(baseStream);

        writer.Write(name);
        writer.Write(surface.Width);
        writer.Write(surface.Height);
        writer.Write(surface.DefaultForeground.PackedValue);
        writer.Write(surface.DefaultBackground.PackedValue);
        writer.Write(surface.DefaultGlyph);
        writer.Write(surface.ViewPosition.X);
        writer.Write(surface.ViewPosition.Y);
        writer.Write(surface.ViewWidth);
        writer.Write(surface.ViewHeight);
        foreach (ColoredGlyphBase cell in surface)
        {
            writer.Write(cell.Foreground.PackedValue);
            writer.Write(cell.Background.PackedValue);
            writer.Write(cell.Glyph);
            writer.Write((byte)cell.Mirror);
            writer.Write(cell.IsVisible);

            if (cell.Decorators is null or { Count: 0 })
                writer.Write((byte)0);

            else
            {
                writer.Write(cell.Decorators.Count);
                foreach (var decorator in cell.Decorators)
                {
                    writer.Write(decorator.Color.PackedValue);
                    writer.Write(decorator.Glyph);
                    writer.Write((byte)decorator.Mirror);
                }
            }
        }

        writer.Flush();

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        using FileStream fileStream = File.Create(filePath);
        baseStream.WriteTo(fileStream);

        return new Blueprint(name, filePath);
    }

    public override string ToString() =>
        Name;
}
