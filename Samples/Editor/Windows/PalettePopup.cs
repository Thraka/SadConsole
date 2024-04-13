using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Microsoft.Xna.Framework.Audio;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public static class PalettePopup
{
    static Color s_ansiColor = Color.AnsiRed;
    static int s_themeColorIndex = 0;
    static string[] s_themeColorNames = Enum.GetNames(typeof(SadConsole.UI.Colors.ColorNames));

    public static bool Show(string popupId, ref Color color)
    {
        bool returnValue = false;

        if (ImGui.BeginPopup(popupId))
        {
            if (ImGui.BeginTabBar("tabs"))
            {
                if (ImGui.BeginTabItem("Ansi"))
                {
                    ImGuiTableFlags flags = ImGuiTableFlags.SizingStretchSame;
                    if (ImGui.BeginTable("colors", 2, flags))
                    {
                        ImGui.TableSetupColumn("one");
                        ImGui.TableSetupColumn("two");
                    }
                    bool clicked = false;

                    GenerateAnsiComboRow("RED DARK", Color.AnsiRed, "RED BRIGHT", Color.AnsiRedBright, ref clicked, ref color);
                    GenerateAnsiComboRow("YELLOW DARK", Color.AnsiYellow, "YELLOW BRIGHT", Color.AnsiYellowBright, ref clicked, ref color);
                    GenerateAnsiComboRow("GREEN DARK", Color.AnsiGreen, "GREEN BRIGHT", Color.AnsiGreenBright, ref clicked, ref color);
                    GenerateAnsiComboRow("CYAN DARK", Color.AnsiCyan, "CYAN BRIGHT", Color.AnsiCyanBright, ref clicked, ref color);
                    GenerateAnsiComboRow("BLUE DARK", Color.AnsiBlue, "BLUE BRIGHT", Color.AnsiBlueBright, ref clicked, ref color);
                    GenerateAnsiComboRow("MAGENTA DARK", Color.AnsiMagenta, "MAGENTA BRIGHT", Color.AnsiMagentaBright, ref clicked, ref color);
                    GenerateAnsiComboRow("BLACK DARK", Color.AnsiBlack, "BLACK BRIGHT", Color.AnsiBlackBright, ref clicked, ref color);
                    GenerateAnsiComboRow("WHITE DARK", Color.AnsiWhite, "WHITE BRIGHT", Color.AnsiWhiteBright, ref clicked, ref color);

                    static void GenerateAnsiComboRow(string name, Color color, string name2, Color color2, ref bool clicked, ref Color selectedColor)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        Vector2 pos = ImGui.GetCursorPos();

                        if (ImGui.Selectable($"##ansi{name}", false, ImGuiSelectableFlags.DontClosePopups))
                        {
                            clicked = true;
                            selectedColor = color;
                        }
                        ImGui.SetCursorPos(pos);
                        ImGui.TextColored(color2.ToVector4(), name);
                        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, color.PackedValue);

                        ImGui.TableSetColumnIndex(1);

                        pos = ImGui.GetCursorPos();
                        if (ImGui.Selectable($"##ansi{name2}", false, ImGuiSelectableFlags.DontClosePopups))
                        {
                            clicked = true;
                            selectedColor = color2;
                        }
                        ImGui.SetCursorPos(pos);
                        ImGui.TextColored(color.ToVector4(), name2);
                        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, color2.PackedValue);
                    }

                    if (clicked)
                    {
                        ImGui.CloseCurrentPopup();
                        returnValue = true;
                    }

                    ImGui.EndTable();

                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("SadConsole Theme"))
                {
                    if (ImGui.BeginListBox("##themelist"))
                    {
                        ImDrawListPtr drawData = ImGui.GetWindowDrawList();

                        foreach (string colorname in s_themeColorNames)
                        {
                            Vector2 pos = ImGui.GetCursorPos();
                            Color parsedColor = UI.Colors.Default.FromColorName(Enum.Parse<UI.Colors.ColorNames>(colorname));
                            if (ImGui.Selectable($"##{colorname}"))
                            {
                                color = parsedColor;
                                returnValue = true;
                                ImGui.CloseCurrentPopup();

                            }
                            drawData.AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), parsedColor.PackedValue);
                            ImGui.SetCursorPos(pos);

                            
                            ImGui.TextColored(new Color(255 - parsedColor.R, 255 - parsedColor.G, 255 - parsedColor.B).ToVector4(), colorname);
                        }

                        ImGui.EndListBox();
                    }

                    //if (ImGui.ListBox("##themelist", ref s_themeColorIndex, s_themeColorNames, 8))

                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Primitives"))
                {
                    if (ImGui.BeginListBox("##primcolorlist"))
                    {
                        ImDrawListPtr drawData = ImGui.GetWindowDrawList();

                        Type colorType = typeof(Color);
                        foreach (FieldInfo item in colorType.GetFields(BindingFlags.Public | BindingFlags.Static).Where((t) => t.FieldType.Name == colorType.Name))
                        {
                            Vector2 pos = ImGui.GetCursorPos();
                            Color parsedColor = (Color)item.GetValue(null)!;
                            if (ImGui.Selectable($"##{item.Name}"))
                            {
                                color = parsedColor;
                                returnValue = true;
                                ImGui.CloseCurrentPopup();

                            }
                            drawData.AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), parsedColor.PackedValue);
                            ImGui.SetCursorPos(pos);


                            ImGui.TextColored(new Color(255 - parsedColor.R, 255 - parsedColor.G, 255 - parsedColor.B).ToVector4(), item.Name);
                        }

                        ImGui.EndListBox();
                    }

                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Custom"))
                {
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }

            ImGui.EndPopup();
        }

        ImGuiCore.State.CheckSetPopupOpen(popupId);

        return returnValue;
    }
}
