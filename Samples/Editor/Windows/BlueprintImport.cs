using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class BlueprintImport : ImGuiWindowBase
{
    public BlueprintImport()
    {
        Title = "Blueprints";
    }

    protected override void OnOpened()
    {
        base.OnOpened();
        Core.State.Blueprints.SelectedItem = null;
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

                if (Core.State.Blueprints.IsItemSelected())
                {
                    if (ImGui.Button("Delete"))
                        ImGui.OpenPopup("ConfirmDelete");
                    //ColoredGlyph visual = _objects.SelectedItem.Visual;

                    //Vector4 foreground = visual.Foreground.ToVector4();
                    //Vector4 background = visual.Background.ToVector4();
                    //int glyph = visual.Glyph;
                    //ImGuiTypes.Mirror mirror = ImGuiTypes.MirrorConverter.FromSadConsoleMirror(visual.Mirror);
                    //string name = _objects.SelectedItem.Name;

                    //if (SettingsTable.BeginTable("pencilsettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
                    //{
                    //    SettingsTable.DrawString("Name", ref name, 255);

                    //    SettingsTable.DrawCommonSettings(true, true, true, true, true,
                    //        ref foreground, _defaultForeground,
                    //        ref background, _defaultBackground,
                    //        ref mirror,
                    //        ref glyph, _font, ImGuiCore.Renderer);

                    //    SettingsTable.EndTable();
                    //}

                    //visual.Foreground = foreground.ToColor();
                    //visual.Background = background.ToColor();
                    //visual.Mirror = ImGuiTypes.MirrorConverter.ToSadConsoleMirror(mirror);
                    //visual.Glyph = glyph;
                    //_objects.SelectedItem.Name = name;
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
}
