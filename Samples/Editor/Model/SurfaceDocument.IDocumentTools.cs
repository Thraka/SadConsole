using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Editor.Model;

internal partial class SurfaceDocument : IDocumentTools
{
    IDocumentToolsState IDocumentTools.State { get; } = new IDocumentToolsState();

    bool IDocumentTools.ShowToolsList { get; set; }

    void IDocumentTools.BuildUI(ImGuiRenderer renderer)
    {
        
    }
}
