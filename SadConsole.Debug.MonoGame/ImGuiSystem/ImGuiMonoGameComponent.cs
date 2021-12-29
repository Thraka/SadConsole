using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.ImGuiSystem
{
    public class ImGuiMonoGameComponent : DrawableGameComponent
    {
        private readonly Microsoft.Xna.Framework.Game _game;
        private GraphicsDeviceManager _graphics;
        public ImGuiRenderer ImGuiRenderer;
        public float fontSize = 14f;
        public string Font = "";

        public event EventHandler HostClosed;

        public bool WantsMouseCapture => ImGuiRenderer.WantsMouseCapture;

        public bool WantsKeyboardCapture => ImGuiRenderer.WantsKeyboardCapture;

        public List<ImGuiObjectBase> UIComponents { get; private set; } = new List<ImGuiObjectBase>();

        public ImGuiMonoGameComponent(GraphicsDeviceManager graphics, Microsoft.Xna.Framework.Game game, bool enableDocking): base(game)
        {
            // Run after (and thus draw on top of) the normal SadConsole MonoGame component
            DrawOrder = 7;
            // Run before the normal SadConsole MonoGame component
            UpdateOrder = 3;

            _graphics = graphics;
            _game = game;

            ImGuiRenderer = new ImGuiRenderer(_game);
            ImGuiRenderer.RebuildFontAtlas();

            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags = io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        }

        public override void Draw(GameTime gameTime)
        {
            // Call BeforeLayout first to set things up
            ImGuiRenderer.BeforeLayout(gameTime);

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

        public IntPtr BindMonoGameTexture(Texture2D texture) =>
            ImGuiRenderer.BindTexture(texture);

        public void UnbindMonoGameTexture(IntPtr texturePointer) =>
            ImGuiRenderer.UnbindTexture(texturePointer);

    }
}
