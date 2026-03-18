using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Windows;

public static class BlueprintImportWindow
{
    public static void Show(ImGuiRenderer renderer, Action onLoaded)
    {
        Instance instance = new(onLoaded);
        renderer.UIObjects.Add(instance);
    }

    protected class Instance : ImGuiObjectBase
    {
        private ImTextureRef? _previewTexture;
        private Vector2 _previewTextureSize;
        private ScreenSurface? _previewTextureWrapper;
        private Action _onLoaded;
        private bool _firstShow = true;

        public Instance(Action onLoaded)
        {
            Core.State.Blueprints.SelectedItemIndex = -1;
            _onLoaded = onLoaded;
        }

        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (_firstShow)
            {
                ImGui.OpenPopup("Open blueprint"u8);
                _firstShow = false;
            }

            ImGuiSC.CenterNextWindowOnAppearing(new System.Numerics.Vector2(Core.Settings.WindowSimpleObjectEditor * ImGui.GetFontSize(), -1));

            if (ImGui.BeginPopupModal("Open blueprint"u8))
            {
                bool closed = false;
                ReadOnlySpan<byte> confirmDeleteID = "ConfirmDeleteBP"u8;

                ImGui.GetStyle().GetSpacing(out Vector2 framePadding, out Vector2 itemSpacing);

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
                    UnloadTexture();
                    closed = true;
                    ImGui.CloseCurrentPopup();
                    renderer.UIObjects.Remove(this);
                    _onLoaded();
                }
                ImGui.EndDisabled();

                float pos = ImGui.CalcTextSize("Cancel").X + framePadding.X / 2;
                ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);

                if (ImGui.Button("Cancel"))
                {
                    UnloadTexture();
                    closed = true;
                    ImGui.CloseCurrentPopup();
                    renderer.UIObjects.Remove(this);
                }

                ImGui.NextColumn();

                if (!closed && Core.State.Blueprints.IsItemSelected()) // second check for IsOpen because closing destroys texture which we need below
                {
                    if (ImGui.Button("Delete"))
                        ImGui.OpenPopup(confirmDeleteID);

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

                ImGuiSC.ConfirmPopup(confirmDeleteID, "Are you sure you want to delete this object?"u8, () => Core.State.Blueprints.Objects.Remove(Core.State.Blueprints.SelectedItem));
                ImGui.EndPopup();
            }


        }

        private void UnloadTexture()
        {
            if (_previewTexture.HasValue)
            {
                Core.ImGuiComponent.ImGuiRenderer.UnbindTexture(_previewTexture.Value.TexID);
                _previewTexture = null;
            }

            if (_previewTextureWrapper is not null)
            {
                _previewTextureWrapper.Dispose();
                _previewTextureWrapper = null;
            }
        }

        private void RenderBlueprintTexture()
        {
            UnloadTexture();
            CellSurface surface = Core.State.Blueprints.SelectedItem!.GetSurface();
            _previewTextureWrapper = new ScreenSurface(surface);
            _previewTextureWrapper.Font = Core.State.SelectedDocument!.EditingSurfaceFont;
            _previewTextureWrapper.Update(TimeSpan.Zero);
            _previewTextureWrapper.Render(TimeSpan.Zero);

            //TODO are we generating drawcalls? We should clear those out, yes?
            _previewTexture = Core.ImGuiComponent.ImGuiRenderer.BindTexture(
                ((SadConsole.Host.GameTexture)_previewTextureWrapper.Renderer.Output).Texture);

            _previewTextureSize = new Vector2(_previewTextureWrapper.Renderer.Output.Width, _previewTextureWrapper.Renderer.Output.Height);
        }
    }
}
