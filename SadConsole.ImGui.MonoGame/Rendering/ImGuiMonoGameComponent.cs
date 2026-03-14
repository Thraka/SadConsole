using System;
using System.Collections.Generic;
using System.Text;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.ImGuiSystem.Rendering;

public class ImGuiMonoGameComponent : DrawableGameComponent
{
    private readonly Microsoft.Xna.Framework.Game _game;

    public ImGuiRenderer ImGuiRenderer;

    public bool WantsMouseCapture => ImGuiRenderer.WantsMouseCapture;

    public bool WantsKeyboardCapture => ImGuiRenderer.WantsKeyboardCapture;

    public List<ImGuiObjectBase> UIComponents { get; } = [ ];

    public ImGuiObjectBase? BeforeNewFrameLayoutObject { get; set; }

    public ImGuiMonoGameComponent(GraphicsDeviceManager graphics, Microsoft.Xna.Framework.Game game, bool enableDocking): base(game)
    {
        // Run after (and thus draw on top of) the normal SadConsole MonoGame component
        DrawOrder = 7;
        // Run before the normal SadConsole MonoGame component
        UpdateOrder = 3;

        _game = game;

        ImGuiRenderer = new ImGuiRenderer(_game);
        //ImGuiRenderer.RebuildFontAtlas();

        if (enableDocking)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        }
    }

    public override void Update(GameTime gameTime)
    {
        ImGuiRenderer.BeforeLayoutInput((float)gameTime.ElapsedGameTime.TotalSeconds);
        Host.Global.BlockSadConsoleInput = ImGuiRenderer.WantsMouseCapture | ImGuiRenderer.WantsKeyboardCapture;
    }

    public override void Draw(GameTime gameTime)
    {
        // Call BeforeLayout first to set things up
        if (BeforeNewFrameLayoutObject != null)
            BeforeNewFrameLayoutObject.BuildUI(ImGuiRenderer);

        ImGuiRenderer.BeforeLayout();

        // Draw our UI
        foreach (ImGuiObjectBase canvas in UIComponents.ToArray())
            canvas.BuildUI(ImGuiRenderer);

        // Call AfterLayout now to finish up and draw all the things
        ImGuiRenderer.AfterLayout();
    }
}
