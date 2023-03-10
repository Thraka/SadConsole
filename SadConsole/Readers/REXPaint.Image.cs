﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using SadRogueColor = SadRogue.Primitives.Color;

namespace SadConsole.Readers;

/// <summary>
/// A RexPaint image.
/// </summary>
public partial class REXPaintImage
{
    private readonly List<Layer> _layers;

    /// <summary>
    /// The version of RexPaint that created this image.
    /// </summary>
    public int Version { get; private set; } = 1;

    /// <summary>
    /// The width of the image.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The height of the image.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The total number of layers for this image.
    /// </summary>
    public int LayerCount => _layers.Count;

    /// <summary>
    /// A read-only collection of layers.
    /// </summary>
    public System.Collections.ObjectModel.ReadOnlyCollection<Layer> Layers => new System.Collections.ObjectModel.ReadOnlyCollection<Layer>(_layers);

    /// <summary>
    /// Creates a new RexPaint image.
    /// </summary>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    public REXPaintImage(int width, int height)
    {
        Width = width;
        Height = height;
        _layers = new List<Layer>();
        Create();
    }

    /// <summary>
    /// Creates a new layer for the image adding it to the end of the layer stack.
    /// </summary>
    /// <returns>A new layer.</returns>
    public Layer Create()
    {
        var layer = new Layer(Width, Height);
        _layers.Add(layer);
        return layer;
    }

    /// <summary>
    /// Creates a new layer for the image and inserts it at the specified position (0-based).
    /// </summary>
    /// <param name="index">The position to create the new layer at.</param>
    /// <returns>A new layer.</returns>
    public Layer Create(int index)
    {
        var layer = new Layer(Width, Height);
        _layers.Insert(index, layer);
        return layer;
    }

    /// <summary>
    /// Adds an existing layer (must be the same width/height) to the image.
    /// </summary>
    /// <param name="layer">The layer to add.</param>
    public void Add(Layer layer) => _layers.Add(layer);

    /// <summary>
    /// Adds an existing layer (must be the same width/height) to the image and inserts it at the specified position (0-based).
    /// </summary>
    /// <param name="layer">The layer to add.</param>
    /// <param name="index">The position to add the layer.</param>
    public void Add(Layer layer, int index) => _layers.Insert(index, layer);

    /// <summary>
    /// Removes the specified layer.
    /// </summary>
    /// <param name="layer">The layer.</param>
    public void Remove(Layer layer) => _layers.Remove(layer);

    /// <summary>
    /// Converts each layer in this REXPaint image to <see cref="CellSurface"/>.
    /// </summary>
    /// <returns>An array of each converted layer.</returns>
    public ICellSurface[] ToCellSurface()
    {
        var layers = new ICellSurface[LayerCount];
        for (int i = 0; i < LayerCount; i++)
        {
            layers[i] = new CellSurface(Width, Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Cell rexCell = _layers[i][x, y];
                    if (rexCell.IsTransparent())
                        continue;

                    SadConsole.ColoredGlyph newCell = layers[i][x, y];
                    newCell.Foreground = new SadRogueColor(rexCell.Foreground.R, rexCell.Foreground.G, rexCell.Foreground.B, (byte)255);
                    newCell.Background = new SadRogueColor(rexCell.Background.R, rexCell.Background.G, rexCell.Background.B, (byte)255);
                    newCell.Glyph = rexCell.Character;
                }
            }

            layers[i].IsDirty = true;
        }


        return layers;
    }

    /// <summary>
    /// Loads a .xp RexPaint image from a GZip compressed stream.
    /// </summary>
    /// <param name="stream">The GZip stream to load.</param>
    /// <returns>The RexPaint image.</returns>
    public static REXPaintImage Load(Stream stream)
    {
        using GZipStream deflatedStream = new(stream, CompressionMode.Decompress);
        using BufferedStream bufferedStream = new(deflatedStream);
        using BinaryReader reader = new(bufferedStream);

        int version = reader.ReadInt32();
        int layerCount = reader.ReadInt32();
        REXPaintImage? image = null;

        for (int currentLayer = 0; currentLayer < layerCount; currentLayer++)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            Layer layer;

            if (currentLayer == 0)
            {
                image = new REXPaintImage(width, height) { Version = version };
                layer = image._layers[0];
            }
            else
            {
                layer = image!.Create();
            }

            // Process cells (could probably be streamlined into index processing instead of x,y...
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    var cell = new Cell(reader.ReadInt32(),                                                  // character
                                        new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte()),  // foreground
                                        new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte())); // background

                    layer[x, y] = cell;
                }
            }
        }

        return image!;
    }
}
