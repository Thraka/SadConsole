﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ImGuiNET;
using SadConsole.ImGuiSystem;

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

                if (ImGui.Begin(Title, ref IsOpen))
                {
                    if (SadConsole.Host.Global.RenderOutput != null)
                    {
                        var texture = renderer.BindTexture(SadConsole.Host.Global.RenderOutput);

                        if (ImGui.RadioButton("Normal", ref _mode, ModeNormal)) { _mode = ModeNormal; } ImGui.SameLine();
                        if (ImGui.RadioButton("2x", ref _mode, Mode2X)) { _mode = Mode2X; } ImGui.SameLine();
                        if (ImGui.RadioButton("Fit", ref _mode, ModeFit)) { _mode = ModeFit; }

                        ImGuiExt.DrawTextureChild("output_preview_image", true, _mode, texture, new Vector2(Host.Global.RenderOutput.Width, Host.Global.RenderOutput.Height), out _, out _);
                    }
                    else
                        ImGui.Text("Rendering output hasn't been created.");
                }
                ImGui.End();
            }
        }
    }
}
