using System;
using System.Collections.Generic;
using Hexa.NET.ImGui;
using SFML.Graphics;

namespace SadConsole.ImGuiSystem;

/// <summary>
/// Manages the ImGui integration with the SFML game loop.
/// </summary>
public class ImGuiSFMLComponent
{
    private readonly RenderWindow _window;

    /// <summary>
    /// The ImGui renderer used for drawing.
    /// </summary>
    public ImGuiRenderer ImGuiRenderer;

    /// <summary>
    /// Raised when the ImGui host is closed/hidden.
    /// </summary>
    public event EventHandler? HostClosed;

    /// <summary>
    /// Gets whether ImGui wants to capture mouse input.
    /// </summary>
    public bool WantsMouseCapture => ImGuiRenderer.WantsMouseCapture;

    /// <summary>
    /// Gets whether ImGui wants to capture keyboard input.
    /// </summary>
    public bool WantsKeyboardCapture => ImGuiRenderer.WantsKeyboardCapture;

    /// <summary>
    /// The ImGui UI objects to draw each frame.
    /// </summary>
    public List<ImGuiObjectBase> UIComponents { get; } = [];

    /// <summary>
    /// An optional layout object that runs before the new frame layout.
    /// </summary>
    public ImGuiObjectBase? BeforeNewFrameLayoutObject { get; set; }

    /// <summary>
    /// Gets or sets whether the component is enabled (processes update/draw).
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the component is visible (draws ImGui).
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Creates a new ImGui SFML component.
    /// </summary>
    /// <param name="window">The SFML render window.</param>
    /// <param name="enableDocking">Whether to enable ImGui docking.</param>
    public ImGuiSFMLComponent(RenderWindow window, bool enableDocking)
    {
        _window = window;

        ImGuiRenderer = new ImGuiRenderer(window);

        if (enableDocking)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        }
    }

    /// <summary>
    /// Updates ImGui input state. Call this during the update phase of the game loop.
    /// </summary>
    /// <param name="deltaSeconds">Time elapsed since last frame in seconds.</param>
    public void Update(float deltaSeconds)
    {
        if (!Enabled) return;

        ImGuiRenderer.BeforeLayoutInput(deltaSeconds);
        Host.Global.BlockSadConsoleInput = ImGuiRenderer.WantsMouseCapture | ImGuiRenderer.WantsKeyboardCapture;
    }

    /// <summary>
    /// Draws the ImGui UI. Call this during the draw phase of the game loop, after SadConsole has drawn.
    /// </summary>
    public void Draw()
    {
        if (!Enabled || !Visible) return;

        // Call BeforeLayout first to set things up
        if (BeforeNewFrameLayoutObject != null)
            BeforeNewFrameLayoutObject.BuildUI(ImGuiRenderer);

        ImGuiRenderer.BeforeLayout();

        // Draw our UI
        foreach (ImGuiObjectBase canvas in UIComponents.ToArray())
            canvas.BuildUI(ImGuiRenderer);

        // Call AfterLayout now to finish up and draw all the things
        ImGuiRenderer.AfterLayout();

        if (ImGuiRenderer.HideRequested)
        {
            ImGuiRenderer.HideRequested = false;

            Enabled = false;
            Visible = false;

            HostClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
