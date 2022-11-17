using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Editor.Model
{
    internal class SurfaceDocument : Document
    {
        private int _sliderValueY;
        private int _sliderValueX;

        public int Width = 80;
        public int Height = 25;

        public Vector4 DefaultForeground = Color.White.ToVector4();
        public Vector4 DefaultBackground = Color.Black.ToVector4();

        public GuiParts.GuiToolsSection ToolsSection;

        public IScreenSurface Surface;
        public GuiParts.SurfacePreview GuiSurfaceWindow;

        public SurfaceDocument()
        {
            DocumentType = Types.Surface;

            ToolsSection = new GuiParts.GuiToolsSection()
            {
                ToolNames = new string[] { "Pencil" },
                ToolObjects = new Tools.ITool[] { new Tools.Pencil() }
            };
        }

        public override void BuildUINew(ImGuiRenderer renderer)
        {
            float paddingX = ImGui.GetStyle().WindowPadding.X;
            float windowWidth = ImGui.GetWindowWidth();

            ImGui.Text("Width: ");
            ImGui.SameLine(ImGui.CalcTextSize("Height: ").X + (ImGui.GetStyle().ItemSpacing.X * 2));
            ImGui.InputInt("##docwidth", ref Width);

            ImGui.Text("Height: ");
            ImGui.SameLine();
            ImGui.InputInt("##docheight", ref Height);

            ImGui.Text("Def. Foreground: ");
            ImGui.SameLine(windowWidth - paddingX - ImGuiCore.State.LayoutInfo.ColorEditBoxWidth);
            ImGui.ColorEdit4("##fore", ref DefaultForeground, ImGuiColorEditFlags.NoInputs);
            ImGuiCore.State.LayoutInfo.ColorEditBoxWidth = ImGui.GetItemRectSize().X;

            ImGui.Text("Def. Background: ");
            ImGui.SameLine(windowWidth - paddingX - ImGuiCore.State.LayoutInfo.ColorEditBoxWidth);
            ImGui.ColorEdit4("##back", ref DefaultBackground, ImGuiColorEditFlags.NoInputs);
        }

        public override void BuildUIEdit(ImGuiRenderer renderer, bool readOnly)
        {
            float paddingX = ImGui.GetStyle().WindowPadding.X;
            float windowWidth = ImGui.GetWindowWidth();

            ImGui.Text("Width: ");
            ImGui.SameLine(windowWidth - paddingX - 100);
            ImGui.SetNextItemWidth(100);
            ImGui.Text(Width.ToString());

            ImGui.Text("Height: ");
            ImGui.SameLine(windowWidth - paddingX - 100);
            ImGui.SetNextItemWidth(100);
            ImGui.Text(Height.ToString());

            ToolsSection.BuildUI(renderer);
        }

        public override void BuildUIDocument(ImGuiRenderer renderer)
        {
            //ImGui.TabItemButton($"{Name}")
            //GuiSurfaceWindow.IsOpen = true;
            GuiSurfaceWindow.BuildUI(renderer);

            ImGui.BeginTabBar("document_host_tabbar", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton);

            if (ImGui.BeginTabItem($"{Name}##{UniqueIdentifier}"))
            {
                var region = ImGui.GetContentRegionAvail();
                var imageSize = new Vector2(Surface.Renderer.Output.Width, Surface.Renderer.Output.Height);
                ImGui.BeginChild("surface_container");

                // TODO: Need to detect if available area for image + scrollbars is too small, if so, change
                //       the surface's viewport to be small enough to hold the image + scrollbars
                // 
                ImGuiExt.DrawTextureChild("output_preview_surface1", true, ImGuiExt.ZoomNormal, ((SadConsole.Host.GameTexture)Surface.Renderer.Output).Texture, imageSize, renderer);

                Vector2 mousePosition = ImGui.GetMousePos();
                Vector2 pos = mousePosition - ImGui.GetItemRectMin();
                if (Surface.AbsoluteArea.WithPosition((0, 0)).Contains(new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y)))
                {
                    SadRogue.Primitives.Point cellPosition = new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y).PixelLocationToSurface(Surface.FontSize) + Surface.Surface.ViewPosition;

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 4f);
                        ImGui.BeginTooltip();

                        var fontTexture = renderer.BindTexture(((Host.GameTexture)Surface.Font.Image).Texture);
                        var rect = Surface.Font.GetGlyphSourceRectangle(Surface.Surface[cellPosition].Glyph);
                        var textureSize = new SadRogue.Primitives.Point(Surface.Font.Image.Width, Surface.Font.Image.Height);

                        ImGui.Image(fontTexture, Surface.Font.GetFontSize(IFont.Sizes.Two).ToVector2(), rect.Position.ToUV(textureSize), (rect.Position + rect.Size).ToUV(textureSize));
                        ImGui.SameLine();
                        ImGui.SameLine();
                        ImGui.BeginGroup();
                        {
                            ImGui.Text($"Glyph: {Surface.Surface[cellPosition].Glyph}");
                            ImGui.Text($"Foreground: ");
                            ImGui.SameLine();
                            ImGui.ColorButton("surface_cell_color", Surface.Surface[cellPosition].Foreground.ToVector4(), ImGuiColorEditFlags.NoPicker);
                            ImGui.Text($"Background: ");
                            ImGui.SameLine();
                            ImGui.ColorButton("surface_cell_color", Surface.Surface[cellPosition].Background.ToVector4(), ImGuiColorEditFlags.NoPicker);
                        }
                        ImGui.EndGroup();

                        ImGui.EndTooltip();
                        ImGui.PopStyleVar();
                    }
                }

                ImGui.SameLine();
                ImGui.VSliderInt("##height", new Vector2(25, imageSize.Y), ref _sliderValueY, 0, 200);
                ImGui.SetNextItemWidth(imageSize.X);
                ImGui.SliderInt("##width", ref _sliderValueX, 0, 10, "%d", ImGuiSliderFlags.AlwaysClamp);

                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }

        public override void OnShow(ImGuiRenderer renderer)
        {

        }

        public override void OnHide(ImGuiRenderer renderer)
        {

        }

        public override void Create()
        {
            Surface = new ScreenSurface(Width, Height);
            Surface.Surface.DefaultBackground = DefaultBackground.ToColor();
            Surface.Surface.DefaultForeground = DefaultForeground.ToColor();
            Surface.Surface.Clear();
            Surface.Surface.FillWithRandomGarbage(200);
            Surface.Render(TimeSpan.Zero);

            GuiSurfaceWindow = new GuiParts.SurfacePreview(Surface);
        }

        public static SurfaceDocument FromSettings(int width, int height, Color foreground, Color background)
        {
            SurfaceDocument doc = new SurfaceDocument()
            {
                Width = width,
                Height = height,
                DefaultForeground = foreground.ToVector4(),
                DefaultBackground = background.ToVector4(),
            };

            doc.Create();

            return doc;
        }
    }
}
