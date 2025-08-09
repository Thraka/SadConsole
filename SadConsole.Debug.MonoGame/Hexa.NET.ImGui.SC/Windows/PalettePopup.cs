using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using SadRogue.Primitives;
using UI = SadConsole.UI;

namespace Hexa.NET.ImGui.SC.Windows;

public static class PalettePopup
{
    static Color s_ansiColor = Color.AnsiRed;
    static int s_themeColorIndex = 0;
    static string[] s_themeColorNames = Enum.GetNames(typeof(UI.Colors.ColorNames));
    static string s_filter = string.Empty;

    public static List<Tuple<string, UI.NamedColor[]>> ExtraPalettes = new();

    public static bool Show(string popupId, ref Color color)
    {
        bool returnValue = false;

        if (ImGui.BeginPopup(popupId))
        {
            Vector2 padding = ImGui.GetStyle().FramePadding;
            Vector2 spacing = ImGui.GetStyle().ItemSpacing;

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

                        if (ImGui.Selectable($"##ansi{name}", false, ImGuiSelectableFlags.NoAutoClosePopups))
                        {
                            clicked = true;
                            selectedColor = color;
                        }

                        ImGui.SetCursorPos(pos);
                        ImGui.TextColored(color2.ToVector4(), name);
                        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, color.PackedValue);

                        ImGui.TableSetColumnIndex(1);

                        pos = ImGui.GetCursorPos();
                        if (ImGui.Selectable($"##ansi{name2}", false, ImGuiSelectableFlags.NoAutoClosePopups))
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

                if (ImGui.BeginTabItem("SadConsole"))
                {
                    ImGui.SetNextItemWidth(-1);
                    if (ImGui.BeginListBox("##themelist"))
                    {
                        ImDrawListPtr drawData = ImGui.GetWindowDrawList();

                        foreach (string colorname in s_themeColorNames)
                        {
                            Vector2 pos = ImGui.GetCursorPos();
                            Color parsedColor = UI.Colors.Default.FromColorName(Enum.Parse<UI.Colors.ColorNames>(colorname));

                            if (GenerateSelectableColor(colorname, parsedColor, drawData))
                            {
                                color = parsedColor;
                                returnValue = true;
                                ImGui.CloseCurrentPopup();
                            }
                        }

                        ImGui.EndListBox();
                    }

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Primitives"))
                {
                    Vector2 area = ImGui.GetContentRegionAvail();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Filter:");
                    ImGui.SameLine();
                    //ImGui.SetNextItemWidth(area.X - ImGui.GetCursorPosX() - ImGui.CalcTextSize("X").X - (spacing.X * 2));
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - spacing.X - ImGui.CalcTextSize("X").X - padding.X * 2);
                    ImGui.InputText("##Filter", ref s_filter, 50);
                    ImGui.SameLine();
                    if (ImGui.Button("X"))
                        s_filter = string.Empty;

                    ImGui.SetNextItemWidth(-1);
                    if (ImGui.BeginListBox("##primcolorlist"))
                    {
                        ImDrawListPtr drawData = ImGui.GetWindowDrawList();

                        Type colorType = typeof(Color);
                        foreach (FieldInfo item in colorType.GetFields(BindingFlags.Public | BindingFlags.Static).Where((t) =>
                                     t.FieldType.Name == colorType.Name && t.Name.Contains(s_filter, StringComparison.OrdinalIgnoreCase)))
                        {
                            Color parsedColor = (Color)item.GetValue(null)!;

                            if (GenerateSelectableColor(item.Name, parsedColor, drawData))
                            {
                                color = parsedColor;
                                returnValue = true;
                                ImGui.CloseCurrentPopup();
                            }
                        }

                        ImGui.EndListBox();
                    }

                    ImGui.EndTabItem();
                }

                // Custom colors
                foreach (Tuple<string, UI.NamedColor[]> colorTab in ExtraPalettes)
                {
                    if (ImGui.BeginTabItem(colorTab.Item1))
                    {
                        ImGui.SetNextItemWidth(-1);
                        if (ImGui.BeginListBox($"##themelist_{colorTab.Item1}"))
                        {
                            ImDrawListPtr drawData = ImGui.GetWindowDrawList();

                            for (int i = 0; i < colorTab.Item2.Length; i++)
                            {
                                Vector2 pos = ImGui.GetCursorPos();

                                if (GenerateSelectableColor(colorTab.Item2[i].Name, colorTab.Item2[i].Color, drawData))
                                {
                                    color = colorTab.Item2[i].Color;
                                    returnValue = true;
                                    ImGui.CloseCurrentPopup();
                                }
                            }

                            ImGui.EndListBox();
                        }

                        ImGui.EndTabItem();
                    }
                }

                ImGui.EndTabBar();
            }

            bool GenerateSelectableColor(string name, Color color, ImDrawListPtr drawData)
            {
                bool returnValue = false;
                Vector2 pos = ImGui.GetCursorPos();
                returnValue = ImGui.Selectable($"##{name}");

                Vector2 topLeft = ImGui.GetItemRectMin();
                Vector2 bottomRight = ImGui.GetItemRectMax();
                Vector2 bottomRightMax = ImGui.GetItemRectMax();

                bottomRight = topLeft + new Vector2(bottomRight.Y - topLeft.Y, bottomRight.Y - topLeft.Y);

                topLeft += padding;
                bottomRight.Y -= padding.Y;
                bottomRight.X = topLeft.X + bottomRight.Y - topLeft.Y;

                pos += new Vector2(bottomRight.X - topLeft.X + spacing.X, 0f);

                ImGui.SetCursorPos(pos);
                ImGui.Text(name);

                drawData.AddRectFilled(topLeft, bottomRight, color.PackedValue);

                pos += new Vector2(bottomRight.X - topLeft.X + spacing.X, 0f);

                topLeft.X = ImGui.CalcTextSize(name).X + bottomRight.X + spacing.X + spacing.X;
                bottomRight = bottomRightMax;
                bottomRight -= padding;

                drawData.AddRectFilled(topLeft, bottomRight, color.PackedValue);

                return returnValue;
            }

            ImGui.EndPopup();
        }

        return returnValue;
    }
}

