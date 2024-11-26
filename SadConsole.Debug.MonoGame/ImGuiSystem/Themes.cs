using System.Numerics;
using Hexa.NET.ImGui;

namespace SadConsole.ImGuiSystem;

public static class Themes
{
    public static void SetCatpuccinMochaColors()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        System.Span<Vector4> colors = style.Colors;

        // Base colors inspired by Catppuccin Mocha
        colors[(int)ImGuiCol.Text] = new Vector4(0.90f, 0.89f, 0.88f, 1.00f);         // Latte
        colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.60f, 0.56f, 0.52f, 1.00f); // Surface2
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.17f, 0.14f, 0.20f, 1.00f);     // Base
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.18f, 0.16f, 0.22f, 1.00f);      // Mantle
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.17f, 0.14f, 0.20f, 1.00f);      // Base
        colors[(int)ImGuiCol.Border] = new Vector4(0.27f, 0.23f, 0.29f, 1.00f);       // Overlay0
        colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.21f, 0.18f, 0.25f, 1.00f);              // Crust
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.24f, 0.20f, 0.29f, 1.00f);       // Overlay1
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.26f, 0.22f, 0.31f, 1.00f);        // Overlay2
        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.14f, 0.12f, 0.18f, 1.00f);              // Mantle
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.17f, 0.15f, 0.21f, 1.00f);        // Mantle
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.14f, 0.12f, 0.18f, 1.00f);     // Mantle
        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.17f, 0.15f, 0.22f, 1.00f);            // Base
        colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.17f, 0.14f, 0.20f, 1.00f);          // Base
        colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.21f, 0.18f, 0.25f, 1.00f);        // Crust
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.24f, 0.20f, 0.29f, 1.00f); // Overlay1
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.26f, 0.22f, 0.31f, 1.00f);  // Overlay2
        colors[(int)ImGuiCol.CheckMark] = new Vector4(0.95f, 0.66f, 0.47f, 1.00f);            // Peach
        colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.82f, 0.61f, 0.85f, 1.00f);           // Lavender
        colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.89f, 0.54f, 0.79f, 1.00f);     // Pink
        colors[(int)ImGuiCol.Button] = new Vector4(0.65f, 0.34f, 0.46f, 1.00f);               // Maroon
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.71f, 0.40f, 0.52f, 1.00f);        // Red
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.76f, 0.46f, 0.58f, 1.00f);         // Pink
        colors[(int)ImGuiCol.Header] = new Vector4(0.65f, 0.34f, 0.46f, 1.00f);               // Maroon
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.71f, 0.40f, 0.52f, 1.00f);        // Red
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.76f, 0.46f, 0.58f, 1.00f);         // Pink
        colors[(int)ImGuiCol.Separator] = new Vector4(0.27f, 0.23f, 0.29f, 1.00f);            // Overlay0
        colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.95f, 0.66f, 0.47f, 1.00f);     // Peach
        colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.95f, 0.66f, 0.47f, 1.00f);      // Peach
        colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.82f, 0.61f, 0.85f, 1.00f);           // Lavender
        colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.89f, 0.54f, 0.79f, 1.00f);    // Pink
        colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.92f, 0.61f, 0.85f, 1.00f);     // Mauve
        colors[(int)ImGuiCol.Tab] = new Vector4(0.21f, 0.18f, 0.25f, 1.00f);                  // Crust
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.82f, 0.61f, 0.85f, 1.00f);           // Lavender
        colors[(int)ImGuiCol.TabSelected] = new Vector4(0.76f, 0.46f, 0.58f, 1.00f);            // Pink
        colors[(int)ImGuiCol.TabDimmed] = new Vector4(0.18f, 0.16f, 0.22f, 1.00f);         // Mantle
        colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.21f, 0.18f, 0.25f, 1.00f);   // Crust
        colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.95f, 0.66f, 0.47f, 0.70f);       // Peach
        colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);       // Base
        colors[(int)ImGuiCol.PlotLines] = new Vector4(0.82f, 0.61f, 0.85f, 1.00f);            // Lavender
        colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.89f, 0.54f, 0.79f, 1.00f);     // Pink
        colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.82f, 0.61f, 0.85f, 1.00f);        // Lavender
        colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.89f, 0.54f, 0.79f, 1.00f); // Pink
        colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.19f, 0.19f, 0.20f, 1.00f);        // Mantle
        colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.27f, 0.23f, 0.29f, 1.00f);    // Overlay0
        colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.23f, 0.23f, 0.25f, 1.00f);     // Surface2
        colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1.00f, 1.00f, 1.00f, 0.06f);  // Surface0
        colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.82f, 0.61f, 0.85f, 0.35f); // Lavender
        colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.95f, 0.66f, 0.47f, 0.90f); // Peach
        //colors[(int)ImGuiCol.NavCursor] = new Vector4(0.82f, 0.61f, 0.85f, 1.00f);   // Lavender
        colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
        colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
        colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);

        // Style adjustments
        style.WindowRounding = 6.0f;
        style.FrameRounding = 4.0f;
        style.ScrollbarRounding = 4.0f;
        style.GrabRounding = 3.0f;
        style.ChildRounding = 4.0f;

        style.WindowTitleAlign = new Vector2(0.50f, 0.50f);
        style.WindowPadding = new Vector2(8.0f, 8.0f);
        style.FramePadding = new Vector2(5.0f, 4.0f);
        style.ItemSpacing = new Vector2(6.0f, 6.0f);
        style.ItemInnerSpacing = new Vector2(6.0f, 6.0f);
        style.IndentSpacing = 22.0f;

        style.ScrollbarSize = 14.0f;
        style.GrabMinSize = 10.0f;

        style.AntiAliasedLines = true;
        style.AntiAliasedFill = true;
    }

    public static void SetGlass()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        System.Span<Vector4> colors = style.Colors;

        // Setting up a dark theme base
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.1f, 0.1f, 0.1f, 0.6f); // Semi-transparent dark background
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.1f, 0.1f, 0.1f, 0.4f);
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.08f, 0.08f, 0.08f, 0.8f);
        colors[(int)ImGuiCol.Border] = new Vector4(0.8f, 0.8f, 0.8f, 0.2f);

        // Text and frames
        colors[(int)ImGuiCol.Text] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.2f, 0.2f, 0.2f, 0.5f); // Semi-transparent for frosted look
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.3f, 0.3f, 0.3f, 0.7f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.3f, 0.3f, 0.3f, 0.9f);

        // Header
        colors[(int)ImGuiCol.Header] = new Vector4(0.3f, 0.3f, 0.3f, 0.7f);
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.4f, 0.4f, 0.4f, 0.8f);
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);

        // Buttons
        colors[(int)ImGuiCol.Button] = new Vector4(0.3f, 0.3f, 0.3f, 0.6f);
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.4f, 0.4f, 0.4f, 0.8f);
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

        // Adjust window rounding and padding to enhance the glass look
        style.WindowRounding = 10.0f;
        style.FrameRounding = 5.0f;
        style.WindowPadding = new Vector2(10, 10);
        style.FramePadding = new Vector2(5, 5);
    }

    public static void SetModernColors()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        System.Span<Vector4> colors = style.Colors;

        // Base color scheme
        colors[(int)ImGuiCol.Text] = new Vector4(0.92f, 0.92f, 0.92f, 1.00f);
        colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.13f, 0.14f, 0.15f, 1.00f);
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.13f, 0.14f, 0.15f, 1.00f);
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.10f, 0.10f, 0.11f, 0.94f);
        colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.20f, 0.21f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.25f, 0.26f, 0.27f, 1.00f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.18f, 0.19f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.15f, 0.15f, 0.16f, 1.00f);
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.15f, 0.15f, 0.16f, 1.00f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.15f, 0.15f, 0.16f, 1.00f);
        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.20f, 0.20f, 0.21f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.20f, 0.21f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.28f, 0.28f, 0.29f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.33f, 0.34f, 0.35f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.40f, 0.40f, 0.41f, 1.00f);
        colors[(int)ImGuiCol.CheckMark] = new Vector4(0.76f, 0.76f, 0.76f, 1.00f);
        colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.28f, 0.56f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.37f, 0.61f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.Button] = new Vector4(0.20f, 0.25f, 0.30f, 1.00f);
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.30f, 0.35f, 0.40f, 1.00f);
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.25f, 0.30f, 0.35f, 1.00f);
        colors[(int)ImGuiCol.Header] = new Vector4(0.25f, 0.25f, 0.25f, 0.80f);
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.30f, 0.30f, 0.30f, 0.80f);
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.35f, 0.35f, 0.35f, 0.80f);
        colors[(int)ImGuiCol.Separator] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.33f, 0.67f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.33f, 0.67f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.28f, 0.56f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.37f, 0.61f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.37f, 0.61f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.Tab] = new Vector4(0.15f, 0.18f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.38f, 0.48f, 0.69f, 1.00f);
        colors[(int)ImGuiCol.TabSelected] = new Vector4(0.28f, 0.38f, 0.59f, 1.00f);
        colors[(int)ImGuiCol.TabDimmed] = new Vector4(0.15f, 0.18f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.15f, 0.18f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.28f, 0.56f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.13f, 0.14f, 0.15f, 1.00f);
        colors[(int)ImGuiCol.PlotLines] = new Vector4(0.61f, 0.61f, 0.61f, 1.00f);
        colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.00f, 0.43f, 0.35f, 1.00f);
        colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
        colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
        colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.19f, 0.19f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.31f, 0.31f, 0.35f, 1.00f);
        colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.23f, 0.23f, 0.25f, 1.00f);
        colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1.00f, 1.00f, 1.00f, 0.06f);
        colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.28f, 0.56f, 1.00f, 0.35f);
        colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.28f, 0.56f, 1.00f, 0.90f);
        colors[(int)ImGuiCol.NavCursor] = new Vector4(0.28f, 0.56f, 1.00f, 1.00f);
        colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
        colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
        colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);

        // Style adjustments
        style.WindowRounding = 5.3f;
        style.FrameRounding = 2.3f;
        style.ScrollbarRounding = 0;

        style.WindowTitleAlign = new Vector2(0.50f, 0.50f);
        style.WindowPadding = new Vector2(8.0f, 8.0f);
        style.FramePadding = new Vector2(5.0f, 5.0f);
        style.ItemSpacing = new Vector2(6.0f, 6.0f);
        style.ItemInnerSpacing = new Vector2(6.0f, 6.0f);
        style.IndentSpacing = 25.0f;
    }

    public static void SetMaterialYouColors()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        System.Span<Vector4> colors = style.Colors;

        // Base colors inspired by Material You (dark mode)
        colors[(int)ImGuiCol.Text] = new Vector4(0.93f, 0.93f, 0.94f, 1.00f);
        colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.15f, 0.15f, 0.15f, 1.00f);
        colors[(int)ImGuiCol.Border] = new Vector4(0.25f, 0.25f, 0.28f, 1.00f);
        colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.18f, 0.18f, 0.18f, 1.00f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.22f, 0.22f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.24f, 0.24f, 0.24f, 1.00f);
        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.16f, 0.16f, 0.16f, 1.00f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.18f, 0.18f, 0.18f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.24f, 0.24f, 0.24f, 1.00f);
        colors[(int)ImGuiCol.CheckMark] = new Vector4(0.45f, 0.76f, 0.29f, 1.00f);
        colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.29f, 0.62f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.29f, 0.66f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.Button] = new Vector4(0.18f, 0.47f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.29f, 0.62f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.22f, 0.52f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.Header] = new Vector4(0.18f, 0.47f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.29f, 0.62f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.29f, 0.66f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.Separator] = new Vector4(0.22f, 0.22f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.29f, 0.62f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.29f, 0.66f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.29f, 0.62f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.29f, 0.66f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.29f, 0.70f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.Tab] = new Vector4(0.18f, 0.18f, 0.18f, 1.00f);
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.29f, 0.62f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.TabSelected] = new Vector4(0.18f, 0.47f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.TabDimmed] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
        colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.18f, 0.47f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.29f, 0.62f, 0.91f, 0.70f);
        colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
        colors[(int)ImGuiCol.PlotLines] = new Vector4(0.61f, 0.61f, 0.61f, 1.00f);
        colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.29f, 0.66f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
        colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
        colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.19f, 0.19f, 0.19f, 1.00f);
        colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.31f, 0.31f, 0.35f, 1.00f);
        colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.23f, 0.23f, 0.25f, 1.00f);
        colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1.00f, 1.00f, 1.00f, 0.06f);
        colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.29f, 0.62f, 0.91f, 0.35f);
        colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.29f, 0.62f, 0.91f, 0.90f);
        colors[(int)ImGuiCol.NavCursor] = new Vector4(0.29f, 0.62f, 0.91f, 1.00f);
        colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
        colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
        colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);

        // Style adjustments
        style.WindowRounding = 8.0f;
        style.FrameRounding = 4.0f;
        style.ScrollbarRounding = 6.0f;
        style.GrabRounding = 4.0f;
        style.ChildRounding = 4.0f;

        style.WindowTitleAlign = new Vector2(0.50f, 0.50f);
        style.WindowPadding = new Vector2(10.0f, 10.0f);
        style.FramePadding = new Vector2(8.0f, 4.0f);
        style.ItemSpacing = new Vector2(8.0f, 8.0f);
        style.ItemInnerSpacing = new Vector2(8.0f, 6.0f);
        style.IndentSpacing = 22.0f;

        style.ScrollbarSize = 16.0f;
        style.GrabMinSize = 10.0f;

        style.AntiAliasedLines = true;
        style.AntiAliasedFill = true;
    }

    public static void SetDarkThemeColors()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        System.Span<Vector4> colors = style.Colors;

        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.1f, 0.105f, 0.11f, 1.0f);

        // Headers
        colors[(int)ImGuiCol.Header] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.3f, 0.305f, 0.31f, 1.0f);
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);

        // Buttons
        colors[(int)ImGuiCol.Button] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.3f, 0.305f, 0.31f, 1.0f);
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);

        // Frame BG
        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.3f, 0.305f, 0.31f, 1.0f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);

        // Tabs
        colors[(int)ImGuiCol.Tab] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.38f, 0.3805f, 0.381f, 1.0f);
        colors[(int)ImGuiCol.TabSelected] = new Vector4(0.28f, 0.2805f, 0.281f, 1.0f);
        colors[(int)ImGuiCol.TabDimmed] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);
        colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);

        // Title
        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);
    }

    public static void SetBessDarkThemeColors()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        System.Span<Vector4> colors = style.Colors;

        // Base colors for a pleasant and modern dark theme with dark accents
        colors[(int)ImGuiCol.Text] = new Vector4(0.92f, 0.93f, 0.94f, 1.00f);                  // Light grey text for readability
        colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.52f, 0.54f, 1.00f);          // Subtle grey for disabled text
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.14f, 0.14f, 0.16f, 1.00f);              // Dark background with a hint of blue
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.16f, 0.16f, 0.18f, 1.00f);               // Slightly lighter for child elements
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.18f, 0.18f, 0.20f, 1.00f);               // Popup background
        colors[(int)ImGuiCol.Border] = new Vector4(0.28f, 0.29f, 0.30f, 0.60f);                // Soft border color
        colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);          // No border shadow
        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.20f, 0.22f, 0.24f, 1.00f);               // Frame background
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.22f, 0.24f, 0.26f, 1.00f);        // Frame hover effect
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.24f, 0.26f, 0.28f, 1.00f);         // Active frame background
        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.14f, 0.14f, 0.16f, 1.00f);               // Title background
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.16f, 0.16f, 0.18f, 1.00f);         // Active title background
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.14f, 0.14f, 0.16f, 1.00f);      // Collapsed title background
        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.20f, 0.20f, 0.22f, 1.00f);             // Menu bar background
        colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.16f, 0.16f, 0.18f, 1.00f);           // Scrollbar background
        colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.24f, 0.26f, 0.28f, 1.00f);         // Dark accent for scrollbar grab
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.28f, 0.30f, 0.32f, 1.00f);  // Scrollbar grab hover
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.32f, 0.34f, 0.36f, 1.00f);   // Scrollbar grab active
        colors[(int)ImGuiCol.CheckMark] = new Vector4(0.46f, 0.56f, 0.66f, 1.00f);             // Dark blue checkmark
        colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.36f, 0.46f, 0.56f, 1.00f);            // Dark blue slider grab
        colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.40f, 0.50f, 0.60f, 1.00f);      // Active slider grab
        colors[(int)ImGuiCol.Button] = new Vector4(0.24f, 0.34f, 0.44f, 1.00f);                // Dark blue button
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.28f, 0.38f, 0.48f, 1.00f);         // Button hover effect
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.32f, 0.42f, 0.52f, 1.00f);          // Active button
        colors[(int)ImGuiCol.Header] = new Vector4(0.24f, 0.34f, 0.44f, 1.00f);                // Header color similar to button
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.28f, 0.38f, 0.48f, 1.00f);         // Header hover effect
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.32f, 0.42f, 0.52f, 1.00f);          // Active header
        colors[(int)ImGuiCol.Separator] = new Vector4(0.28f, 0.29f, 0.30f, 1.00f);             // Separator color
        colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.46f, 0.56f, 0.66f, 1.00f);      // Hover effect for separator
        colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.46f, 0.56f, 0.66f, 1.00f);       // Active separator
        colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.36f, 0.46f, 0.56f, 1.00f);            // Resize grip
        colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.40f, 0.50f, 0.60f, 1.00f);     // Hover effect for resize grip
        colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.44f, 0.54f, 0.64f, 1.00f);      // Active resize grip
        colors[(int)ImGuiCol.Tab] = new Vector4(0.20f, 0.22f, 0.24f, 1.00f);                   // Inactive tab
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.28f, 0.38f, 0.48f, 1.00f);            // Hover effect for tab
        colors[(int)ImGuiCol.TabSelected] = new Vector4(0.24f, 0.34f, 0.44f, 1.00f);             // Active tab color
        colors[(int)ImGuiCol.TabDimmed] = new Vector4(0.20f, 0.22f, 0.24f, 1.00f);          // Unfocused tab
        colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.24f, 0.34f, 0.44f, 1.00f);    // Active but unfocused tab
        colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.24f, 0.34f, 0.44f, 0.70f);        // Docking preview
        colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.14f, 0.14f, 0.16f, 1.00f);        // Empty docking background
        colors[(int)ImGuiCol.PlotLines] = new Vector4(0.46f, 0.56f, 0.66f, 1.00f);             // Plot lines
        colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.46f, 0.56f, 0.66f, 1.00f);      // Hover effect for plot lines
        colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.36f, 0.46f, 0.56f, 1.00f);         // Histogram color
        colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.40f, 0.50f, 0.60f, 1.00f);  // Hover effect for histogram
        colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.20f, 0.22f, 0.24f, 1.00f);         // Table header background
        colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.28f, 0.29f, 0.30f, 1.00f);     // Strong border for tables
        colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.24f, 0.25f, 0.26f, 1.00f);      // Light border for tables
        colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.20f, 0.22f, 0.24f, 1.00f);            // Table row background
        colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(0.22f, 0.24f, 0.26f, 1.00f);         // Alternate row background
        colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.24f, 0.34f, 0.44f, 0.35f);        // Selected text background
        colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.46f, 0.56f, 0.66f, 0.90f);        // Drag and drop target
        colors[(int)ImGuiCol.NavCursor] = new Vector4(0.46f, 0.56f, 0.66f, 1.00f);          // Navigation highlight
        colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f); // Windowing highlight
        colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);     // Dim background for windowing
        colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);      // Dim background for modal windows

        // Style adjustments
        style.WindowRounding = 8.0f;    // Softer rounded corners for windows
        style.FrameRounding = 4.0f;     // Rounded corners for frames
        style.ScrollbarRounding = 6.0f; // Rounded corners for scrollbars
        style.GrabRounding = 4.0f;      // Rounded corners for grab elements
        style.ChildRounding = 4.0f;     // Rounded corners for child windows

        style.WindowTitleAlign = new Vector2(0.50f, 0.50f); // Centered window title
        style.WindowPadding = new Vector2(10.0f, 10.0f);    // Comfortable padding
        style.FramePadding = new Vector2(6.0f, 4.0f);       // Frame padding
        style.ItemSpacing = new Vector2(8.0f, 8.0f);        // Item spacing
        style.ItemInnerSpacing = new Vector2(8.0f, 6.0f);   // Inner item spacing
        style.IndentSpacing = 22.0f;                   // Indentation spacing

        style.ScrollbarSize = 16.0f; // Scrollbar size
        style.GrabMinSize = 10.0f;   // Minimum grab size

        style.AntiAliasedLines = true; // Enable anti-aliased lines
        style.AntiAliasedFill = true;  // Enable anti-aliased fill
    }

    public static void SetFluentUITheme()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        System.Span<Vector4> colors = style.Colors;

        // General window settings
        style.WindowRounding = 5.0f;
        style.FrameRounding = 5.0f;
        style.ScrollbarRounding = 5.0f;
        style.GrabRounding = 5.0f;
        style.TabRounding = 5.0f;
        style.WindowBorderSize = 1.0f;
        style.FrameBorderSize = 1.0f;
        style.PopupBorderSize = 1.0f;
        style.PopupRounding = 5.0f;

        // Setting the colors
        colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.95f, 0.95f, 1.00f);
        colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.60f, 0.60f, 0.60f, 1.00f);
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.13f, 0.13f, 0.13f, 1.00f);
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.10f, 0.10f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.18f, 0.18f, 0.18f, 1.0f);
        colors[(int)ImGuiCol.Border] = new Vector4(0.30f, 0.30f, 0.30f, 1.00f);
        colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.30f, 0.30f, 0.30f, 1.00f);
        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.10f, 0.10f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.10f, 0.10f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.15f, 0.15f, 0.15f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.10f, 0.10f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.30f, 0.30f, 0.30f, 1.00f);

        // Accent colors changed to darker olive-green/grey shades
        colors[(int)ImGuiCol.CheckMark] = new Vector4(0.45f, 0.45f, 0.45f, 1.00f);        // Dark gray for check marks
        colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.45f, 0.45f, 0.45f, 1.00f);       // Dark gray for sliders
        colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f); // Slightly lighter gray when active
        colors[(int)ImGuiCol.Button] = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);           // Button background (dark gray)
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.30f, 0.30f, 0.30f, 1.00f);    // Button hover state
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.35f, 0.35f, 0.35f, 1.00f);     // Button active state
        colors[(int)ImGuiCol.Header] = new Vector4(0.40f, 0.40f, 0.40f, 1.00f);           // Dark gray for menu headers
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.45f, 0.45f, 0.45f, 1.00f);    // Slightly lighter on hover
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);     // Lighter gray when active
        colors[(int)ImGuiCol.Separator] = new Vector4(0.30f, 0.30f, 0.30f, 1.00f);        // Separators in dark gray
        colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.35f, 0.35f, 0.35f, 1.00f);
        colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.40f, 0.40f, 0.40f, 1.00f);
        colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.45f, 0.45f, 0.45f, 1.00f); // Resize grips in dark gray
        colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.55f, 0.55f, 0.55f, 1.00f);
        colors[(int)ImGuiCol.Tab] = new Vector4(0.18f, 0.18f, 0.18f, 1.00f);        // Tabs background
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.40f, 0.40f, 0.40f, 1.00f); // Darker gray on hover
        colors[(int)ImGuiCol.TabSelected] = new Vector4(0.40f, 0.40f, 0.40f, 1.00f);
        colors[(int)ImGuiCol.TabDimmed] = new Vector4(0.18f, 0.18f, 0.18f, 1.00f);
        colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.40f, 0.40f, 0.40f, 1.00f);
        colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.45f, 0.45f, 0.45f, 1.00f); // Docking preview in gray
        colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.18f, 0.18f, 0.18f, 1.00f); // Empty dock background
        // Additional styles
        style.FramePadding = new Vector2(8.0f, 4.0f);
        style.ItemSpacing = new Vector2(8.0f, 4.0f);
        style.IndentSpacing = 20.0f;
        style.ScrollbarSize = 16.0f;
    }

    public static void SetFluentUILightTheme()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        System.Span<Vector4> colors = style.Colors;

        // General window settings
        style.WindowRounding = 5.0f;
        style.FrameRounding = 5.0f;
        style.ScrollbarRounding = 5.0f;
        style.GrabRounding = 5.0f;
        style.TabRounding = 5.0f;
        style.WindowBorderSize = 1.0f;
        style.FrameBorderSize = 1.0f;
        style.PopupBorderSize = 1.0f;
        style.PopupRounding = 5.0f;

        // Setting the colors (Light version)
        colors[(int)ImGuiCol.Text] = new Vector4(0.10f, 0.10f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.60f, 0.60f, 0.60f, 1.00f);
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.95f, 0.95f, 0.95f, 1.00f); // Light background
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.98f, 0.98f, 0.98f, 1.00f);
        colors[(int)ImGuiCol.Border] = new Vector4(0.70f, 0.70f, 0.70f, 1.00f);
        colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.85f, 0.85f, 0.85f, 1.00f); // Light frame background
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.80f, 0.80f, 0.80f, 1.00f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.75f, 0.75f, 0.75f, 1.00f);
        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.85f, 0.85f, 0.85f, 1.00f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);
        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.95f, 0.95f, 0.95f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.80f, 0.80f, 0.80f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.75f, 0.75f, 0.75f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.70f, 0.70f, 0.70f, 1.00f);

        // Accent colors with a soft pastel gray-green
        colors[(int)ImGuiCol.CheckMark] = new Vector4(0.55f, 0.65f, 0.55f, 1.00f); // Soft gray-green for check marks
        colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.55f, 0.65f, 0.55f, 1.00f);
        colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.60f, 0.70f, 0.60f, 1.00f);
        colors[(int)ImGuiCol.Button] = new Vector4(0.85f, 0.85f, 0.85f, 1.00f); // Light button background
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.80f, 0.80f, 0.80f, 1.00f);
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.75f, 0.75f, 0.75f, 1.00f);
        colors[(int)ImGuiCol.Header] = new Vector4(0.75f, 0.75f, 0.75f, 1.00f);
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.70f, 0.70f, 0.70f, 1.00f);
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.65f, 0.65f, 0.65f, 1.00f);
        colors[(int)ImGuiCol.Separator] = new Vector4(0.60f, 0.60f, 0.60f, 1.00f);
        colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.65f, 0.65f, 0.65f, 1.00f);
        colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.70f, 0.70f, 0.70f, 1.00f);
        colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.55f, 0.65f, 0.55f, 1.00f); // Accent color for resize grips
        colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.60f, 0.70f, 0.60f, 1.00f);
        colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.65f, 0.75f, 0.65f, 1.00f);
        colors[(int)ImGuiCol.Tab] = new Vector4(0.85f, 0.85f, 0.85f, 1.00f); // Tabs background
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.80f, 0.80f, 0.80f, 1.00f);
        colors[(int)ImGuiCol.TabSelected] = new Vector4(0.75f, 0.75f, 0.75f, 1.00f);
        colors[(int)ImGuiCol.TabDimmed] = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);
        colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.75f, 0.75f, 0.75f, 1.00f);
        colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.55f, 0.65f, 0.55f, 1.00f); // Docking preview in gray-green
        colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);

        // Additional styles
        style.FramePadding = new Vector2(8.0f, 4.0f);
        style.ItemSpacing = new Vector2(8.0f, 4.0f);
        style.IndentSpacing = 20.0f;
        style.ScrollbarSize = 16.0f;
    }


}
