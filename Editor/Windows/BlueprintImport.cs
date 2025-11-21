using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class BlueprintImport : ImGuiWindowBase
{
    private ImTextureID? _previewTexture;
    private Vector2 _previewTextureSize;
    private ScreenSurface? _previewTextureWrapper;

    public BlueprintImport()
    {
        Title = "Blueprints";
    }

    protected override void OnOpened()
    {
        base.OnOpened();
        Core.State.Blueprints.SelectedItem = null;
    }

    private void UnloadTexture()
    {
        if (_previewTexture.HasValue)
        {
            ImGuiCore.Renderer.UnbindTexture(_previewTexture.Value);
            _previewTexture = null;
        }

        if (_previewTextureWrapper is not null)
        {
            _previewTextureWrapper.Dispose();
            _previewTextureWrapper = null;
        }
    }

    void RenderBlueprintTexture()
    {
        UnloadTexture();
        CellSurface surface = Core.State.Blueprints.SelectedItem.GetSurface();
        _previewTextureWrapper = new ScreenSurface(surface);
        _previewTextureWrapper.Font = Core.State.Documents.SelectedItem.EditingSurfaceFont;
        _previewTextureWrapper.Update(TimeSpan.Zero);
        _previewTextureWrapper.Render(TimeSpan.Zero);

        //TODO are we generating drawcalls? We should clear those out, yes?
        _previewTexture = ImGuiCore.Renderer.BindTexture(
            ((SadConsole.Host.GameTexture)_previewTextureWrapper.Renderer.Output).Texture);

        _previewTextureSize = new Vector2(_previewTextureWrapper.Renderer.Output.Width, _previewTextureWrapper.Renderer.Output.Height);
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.GetStyle().GetSpacing(out Vector2 framePadding, out Vector2 itemSpacing);

            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(Core.Settings.WindowSimpleObjectEditor * ImGui.GetFontSize(), -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                ImGui.Columns(2);
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Items");
                ImGui.SetNextItemWidth(-1);
                if (ImGui.ListBox("##blueprints", ref Core.State.Blueprints.SelectedItemIndex, Core.State.Blueprints.Names, Core.State.Blueprints.Count, 10))
                {
                    RenderBlueprintTexture();
                }

                ImGui.BeginDisabled(!Core.State.Blueprints.IsItemSelected());
                if (ImGui.Button("Select"))
                {
                    DialogResult = true;
                    Close();
                }
                ImGui.EndDisabled();

                float pos = ImGui.CalcTextSize("Cancel").X + framePadding.X / 2;
                ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);

                if (ImGui.Button("Cancel"))
                {
                    DialogResult = false;
                    Close();
                }

                ImGui.NextColumn();

                if (Core.State.Blueprints.IsItemSelected() && IsOpen) // second check for IsOpen because closing destroys texture which we need below
                {
                    if (ImGui.Button("Delete"))
                        ImGui.OpenPopup("ConfirmDelete");

                    var region = ImGui.GetContentRegionAvail();

                    // Calculate scaled region to maintain aspect ratio
                    var scaledRegion = region;
                    var textureAspectRatio = _previewTextureSize.X / _previewTextureSize.Y;
                    var regionAspectRatio = region.X / region.Y;

                    if (textureAspectRatio > regionAspectRatio)
                    {
                        // Texture is wider than region
                        var newHeight = region.X / textureAspectRatio;
                        if (_previewTextureSize.Y < region.Y || newHeight < region.Y)
                        {
                            scaledRegion = new Vector2(region.X, newHeight);
                        }
                    }
                    else
                    {
                        // Texture is taller than region
                        var newWidth = region.Y * textureAspectRatio;
                        if (_previewTextureSize.X < region.X || newWidth < region.X)
                        {
                            scaledRegion = new Vector2(newWidth, region.Y);
                        }
                    }

                    // Do draw
                    ImGuiSC.DrawTexture("blueprint_preview", true, ImGuiSC.ZoomFit,
                                        _previewTexture.Value,
                                        _previewTextureSize,
                                        scaledRegion,
                                        out _, out _, true);
                }

                ImGui.Columns(1);

                ImGuiSC.CenterNextWindow();
                if (ImGui.BeginPopup("ConfirmDelete"))
                {
                    ImGui.Text("Are you sure you want to delete this object?");

                    if (ImGui.Button("Cancel"))
                        ImGui.CloseCurrentPopup();

                    pos = ImGui.CalcTextSize("Yes").X + framePadding.X / 2;
                    ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);
                    if (ImGui.Button("Yes"))
                    {
                        Core.State.Blueprints.Objects.Remove(Core.State.Blueprints.SelectedItem);
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }
                ImGui.EndPopup();
            }
        }
    }

    protected override void OnClosed()
    {
        UnloadTexture();
        base.OnClosed();
    }
}
