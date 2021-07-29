using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ImGuiNET;

namespace SadConsole.Debug.MonoGame
{
    public class FinalOutputWindow : ImGuiWindow
    {
        private bool _windowOpenState;
        private int _mode = ModeNormal;

        private const int ModeNormal = 0;
        private const int Mode2X = 1;
        private const int ModeFit = 2;

        public FinalOutputWindow(string title, bool isOpen)
        {
            Title = title;
            IsOpen = isOpen;
        }
        
        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (IsOpen)
            {
                ImGui.SetNextWindowBgAlpha(1f);
                //ImGui.SetNextWindowSizeConstraints(new Vector2(200, 200), new Vector2(10000, 10000));

                if (ImGui.Begin(Title, ref IsOpen))
                {
                    if (SadConsole.Host.Global.RenderOutput != null)
                    {
                        var texture = renderer.BindTexture(SadConsole.Host.Global.RenderOutput);
                        //var drawPointer = ImGui.GetBackgroundDrawList();
                        //var startPos = ImGui.GetWindowPos();

                        //var textureSize = new Vector2(SadConsole.Host.Global.RenderOutput.Width, SadConsole.Host.Global.RenderOutput.Height);


                        /*
                         * float heightRatio = GraphicsDevice.PresentationParameters.BackBufferHeight / (float)SadConsole.Settings.Rendering.RenderHeight;
                float widthRatio = GraphicsDevice.PresentationParameters.BackBufferWidth / (float)SadConsole.Settings.Rendering.RenderWidth;

                float fitHeight = SadConsole.Settings.Rendering.RenderHeight * widthRatio;
                float fitWidth = SadConsole.Settings.Rendering.RenderWidth * heightRatio;

                if (fitHeight <= GraphicsDevice.PresentationParameters.BackBufferHeight)
                         */

                        //ImGui.BeginChild("output_preview_options", );
                        {
                            if (ImGui.RadioButton("Normal", ref _mode, ModeNormal)) { _mode = ModeNormal; } ImGui.SameLine();
                            if (ImGui.RadioButton("2x", ref _mode, Mode2X)) { _mode = Mode2X; } ImGui.SameLine();
                            if (ImGui.RadioButton("Fit", ref _mode, ModeFit)) { _mode = ModeFit; }
                        }
                        //ImGui.EndChild();

                        ImGuiWindowFlags flags = ImGuiWindowFlags.None;

                        if (_mode == ModeNormal || (int)_mode == Mode2X)
                            flags = ImGuiWindowFlags.HorizontalScrollbar;
                        else // ModeFit
                            flags = ImGuiWindowFlags.NoScrollbar;

                        //ImGui.BeginChild("output_preview_image", flags);
                        {
                            Vector2 textureSize;

                            if (_mode == Mode2X)
                                textureSize = new Vector2(Host.Global.RenderOutput.Width * 2, Host.Global.RenderOutput.Height * 2);
                            else if (_mode == ModeFit)
                            {
                                textureSize = ImGui.GetContentRegionAvail();
                            }
                            else // ModeNormal
                                textureSize = new Vector2(Host.Global.RenderOutput.Width, Host.Global.RenderOutput.Height);

                            //drawPointer.AddRectFilled(startPos, startPos + ImGui.GetWindowSize(), ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));

                            ImGui.Image(texture, textureSize, Vector2.Zero, Vector2.One, Vector4.One, Vector4.One);
                        }
                        //ImGui.EndChild();
                    }
                    else
                        ImGui.Text("Rendering output hasn't been created.");
                }
                ImGui.End();
            }
        }
    }
}
