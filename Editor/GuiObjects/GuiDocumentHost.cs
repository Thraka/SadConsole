using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.ImGuiSystem;
using SadConsole;

namespace SadConsole.Editor.GuiObjects;
public class GuiDocumentsHost: ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.Begin(GuiDockSpace.ID_CENTER_PANEL);

        if (Core.State.HasSelectedDocument)
        {
            //ImGuiSC.DrawTexture(Core.State.SelectedDocument.)
            //ImGuiSC.DrawTexture("output_preview_surface1", true, _zoom ? ImGuiExt.Zoom2x : ImGuiExt.ZoomNormal, gameTexture.Texture, renderer, out _, out _);

            Documents.Document document = Core.State.SelectedDocument!;

            if (document.Options.DrawSelf)
            {
                document.ImGuiDraw(renderer);
            }
            else
            {
                if (ImGui.BeginChild("doc_surface"u8))
                {
                    bool bigX = false;
                    bool isActive = false;
                    bool isHovered = false;
                    bool viewChanged = false;
                    int fontSize = (int)ImGui.GetFontSize();
                    Vector2 padding = ImGui.GetStyle().FramePadding;
                    Vector2 barSize = new Vector2(fontSize + padding.X * 2, fontSize + padding.Y * 2);

                    Vector2 availRegion = ImGui.GetContentRegionAvail() - new Vector2(barSize.X, barSize.Y);
                    Vector2 pixelArea = new (document.EditingSurface.Surface.Width * document.EditorFontSize.X,
                                             document.EditingSurface.Surface.Height * document.EditorFontSize.Y);

                    int previousViewWidth = document.EditingSurface.Surface.ViewWidth;
                    int previousViewHeight = document.EditingSurface.Surface.ViewHeight;

                    // Set view width of surface
                    if (pixelArea.X <= availRegion.X)
                    {
                        document.SetSurfaceView(document.EditingSurface.Surface.ViewPosition.X,
                                                document.EditingSurface.Surface.ViewPosition.Y,
                                                document.EditingSurface.Surface.Width,
                                                document.EditingSurface.Surface.ViewHeight);
                    }
                    else
                    {
                        document.SetSurfaceView(document.EditingSurface.Surface.ViewPosition.X,
                                                document.EditingSurface.Surface.ViewPosition.Y,
                                                Math.Max((int)(availRegion.X / document.EditorFontSize.X), 1),
                                                document.EditingSurface.Surface.ViewHeight);

                        pixelArea.X = document.EditingSurface.Surface.ViewWidth * document.EditorFontSize.X;
                    }

                    // Set view height of surface
                    if (pixelArea.Y <= availRegion.Y)
                    {
                        document.SetSurfaceView(document.EditingSurface.Surface.ViewPosition.X,
                                                document.EditingSurface.Surface.ViewPosition.Y,
                                                document.EditingSurface.Surface.ViewWidth,
                                                document.EditingSurface.Surface.Height);
                    }
                    else
                    {
                        document.SetSurfaceView(document.EditingSurface.Surface.ViewPosition.X,
                                                document.EditingSurface.Surface.ViewPosition.Y,
                                                document.EditingSurface.Surface.ViewWidth,
                                                Math.Max((int)(availRegion.Y / document.EditorFontSize.Y), 1));

                        pixelArea.Y = document.EditingSurface.Surface.ViewHeight * document.EditorFontSize.Y;
                    }

                    // If view size changed, notify tool
                    if (document.EditingSurface.Surface.ViewWidth != previousViewWidth ||
                        document.EditingSurface.Surface.ViewHeight != previousViewHeight)
                    {
                        document.Redraw(true, true);
                        Core.State.Tools.SelectedItem?.DocumentViewChanged(document);
                    }

                    // Draw image
                    ImGuiSC.DrawTexture("output_preview_surface1", true, ImGuiSC.ZoomFit,
                                        document.VisualTextureId,
                                        document.VisualTextureSize,
                                        pixelArea,
                                        out isActive, out isHovered, true);

                    // Draw scrollbars
                    bool enableScrollX = document.EditingSurface.Surface.ViewHeight != document.EditingSurface.Surface.Height;
                    bool enableScrollY = document.EditingSurface.Surface.ViewWidth != document.EditingSurface.Surface.Width;

                    // Print stats
                    Core.State.GuiTopBar.StatusItems.Add((Vector4.Zero, "| ViewPort:"));
                    Core.State.GuiTopBar.StatusItems.Add((Color.Yellow.ToVector4(), document.EditingSurface.Surface.View.ToString()));

                    Point hoveredCellPosition = Point.None;

                    // Handle tool interaction with surface, is mouse over surface?
                    if (isHovered)
                    {
                        Vector2 mousePosition = ImGui.GetMousePos();
                        Vector2 textureScreenPos = ImGui.GetItemRectMin();
                        Vector2 relativeMousePos = mousePosition - textureScreenPos;

                        hoveredCellPosition =
                            new Point((int)relativeMousePos.X, (int)relativeMousePos.Y)
                                .PixelLocationToSurface(document.EditorFontSize) +
                            document.EditingSurface.Surface.ViewPosition;

                        Core.State.GuiTopBar.StatusItems.Add((Vector4.Zero, "| Mouse:"));
                        Core.State.GuiTopBar.StatusItems.Add((Color.Yellow.ToVector4(), hoveredCellPosition.ToString()));
                    }

                    // Handle selected tool
                    if (Core.State.Tools.IsItemSelected())
                        Core.State.Tools.SelectedItem.Process(document, hoveredCellPosition, isHovered, isActive);

                    // Handle scrolling the surface if it's required
                    Rectangle view = document.EditingSurface.Surface.View;

                    int sliderValueY = view.Position.Y;

                    ImGui.SameLine();
                    ImGui.BeginDisabled(document.Options.DisableScrolling || !enableScrollX);
                    if (ImGuiSC.VSliderIntNudges("##height", new Vector2(barSize.X, pixelArea.Y), ref sliderValueY,
                                                document.EditingSurface.Surface.Height - view.Height, 0, ImGuiSliderFlags.AlwaysClamp))
                    {
                        document.SetSurfaceView(document.EditingSurface.Surface.ViewPosition.X,
                                                sliderValueY,
                                                document.EditingSurface.Surface.ViewWidth,
                                                document.EditingSurface.Surface.ViewHeight);

                        viewChanged = true;
                        Core.State.Tools.SelectedItem?.DocumentViewChanged(document);
                    }
                    ImGui.EndDisabled();

                    int sliderValueX = view.Position.X;

                    ImGui.BeginDisabled(document.Options.DisableScrolling || !enableScrollY);
                    if (ImGuiSC.SliderIntNudges("##width", (int)pixelArea.X, ref sliderValueX, 0,
                                               document.EditingSurface.Surface.Width - view.Width, bigX ? "BIG" : "%d", ImGuiSliderFlags.AlwaysClamp))
                    {
                        document.SetSurfaceView(sliderValueX,
                                                document.EditingSurface.Surface.ViewPosition.Y,
                                                document.EditingSurface.Surface.ViewWidth,
                                                document.EditingSurface.Surface.ViewHeight);

                        Core.State.Tools.SelectedItem?.DocumentViewChanged(document);
                    }
                    ImGui.EndDisabled();
                }
                ImGui.EndChild();
            }


        }
        else
        {
            ImGui.Text("No document selected.");
        }

        ImGui.End();
    }
}
