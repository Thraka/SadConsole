﻿// Taken from https://github.com/mellinoe/ImGui.NET/blob/master/src/ImGui.NET.SampleProgram.XNA/DrawVertDeclaration.cs

using Hexa.NET.ImGui;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.ImGuiSystem;

public static class DrawVertDeclaration
{
    public static readonly VertexDeclaration Declaration;

    public static readonly int Size;

    static DrawVertDeclaration()
    {
        unsafe { Size = sizeof(ImDrawVert); }

        Declaration = new VertexDeclaration(
            Size,

            // Position
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),

            // UV
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),

            // Color
            new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );
    }
}
