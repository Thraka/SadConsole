using System;
using SadConsole.Components;
using SadConsole.Input;

namespace SadConsole.Configuration;

/// <summary>
/// Extensions to enable the ImGui debug UI.
/// </summary>
public static class DebugExtensions
{
    /// <summary>
    /// Adds a <see cref="GameHost.RootComponents"/> component that uses the specified hotkey to invoke <see cref="Debug.MonoGame.Debugger.Start"/>.
    /// </summary>
    /// <param name="builder">The config builder.</param>
    /// <param name="hotkey">The keyboard key to start the debugger.</param>
    /// <returns>The config builder.</returns>
    public static Builder EnableImGuiDebug(this Builder builder, Keys hotkey)
    {
        ImGuiDebugConfig config = builder.GetOrCreateConfig<ImGuiDebugConfig>();
        config.HotKey = hotkey;
        return builder;
    }
}

internal class ImGuiDebugConfig : RootComponent, IConfigurator
{
    public Keys HotKey { get; set; }

    public void Run(Builder config, GameHost game)
    {
        game.RootComponents.Add(this);
    }

    public override void Run(TimeSpan delta)
    {
        if (Game.Instance.FrameNumber != 0 && Game.Instance.Keyboard.IsKeyReleased(HotKey) && !Debug.MonoGame.Debugger.IsOpened)
            Debug.MonoGame.Debugger.Start();
    }
}
