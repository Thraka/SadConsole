using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class ImageToAsciiWindow : ImGuiWindowBase
{
    private enum WindowMode
    {
        FilePicker,
        ImageConverter
    }

    private WindowMode _currentMode = WindowMode.FilePicker;

    // File picker components
    private FileListBox _fileListBox;
    private ImGuiList<IFileHandler> _fileLoaders;

    // Image data
    private Texture2D? _loadedTexture;
    private ImTextureID? _sourceImageTexture;
    private Vector2 _sourceImageSize;

    // Conversion settings
    private bool _matchOutputToImageSize = true;
    private int _outputWidth = 80;
    private int _outputHeight = 25;
    private Vector4 _colorKey = Color.Transparent.ToVector4();
    private bool _useColorKey = true;

    // Conversion style settings
    private int _convertModeIndex = 0;
    private int _backgroundStyleIndex = 0;
    private int _foregroundStyleIndex = 0;

    // ASCII preview
    private ScreenSurface? _previewSurface;
    private ImTextureID? _previewTexture;
    private Vector2 _previewTextureSize;

    // Font info
    private readonly IFont _font;
    private readonly Point _fontSize;

    /// <summary>
    /// The resulting surface after conversion. Available after the dialog is closed with OK.
    /// </summary>
    public ICellSurface? ResultSurface { get; private set; }

    public ImageToAsciiWindow(IFont font, Point fontSize)
    {
        Title = "Image Converter";
        _font = font;
        _fontSize = fontSize;

        _fileListBox = new(Core.State.RootFolder);
        _fileLoaders = new ImGuiList<IFileHandler>(0, [new ImageFile()]);
    }

    protected override void OnOpened()
    {
        base.OnOpened();
        _currentMode = WindowMode.FilePicker;
        ResultSurface = null;
    }

    private void UnloadTextures()
    {
        if (_sourceImageTexture.HasValue)
        {
            ImGuiCore.Renderer.UnbindTexture(_sourceImageTexture.Value);
            _sourceImageTexture = null;
        }

        if (_previewTexture.HasValue)
        {
            ImGuiCore.Renderer.UnbindTexture(_previewTexture.Value);
            _previewTexture = null;
        }

        if (_previewSurface is not null)
        {
            _previewSurface.Dispose();
            _previewSurface = null;
        }

        if (_loadedTexture is not null)
        {
            _loadedTexture.Dispose();
            _loadedTexture = null;
        }
    }

    private void LoadImage(string filePath)
    {
        UnloadTextures();

        using Stream stream = File.OpenRead(filePath);
        _loadedTexture = Texture2D.FromStream(SadConsole.Host.Global.GraphicsDevice, stream);

        _sourceImageTexture = ImGuiCore.Renderer.BindTexture(_loadedTexture);
        _sourceImageSize = new Vector2(_loadedTexture.Width, _loadedTexture.Height);

        // Calculate default output size based on image and font
        _outputWidth = _loadedTexture.Width;
        _outputHeight = _loadedTexture.Height;

        GeneratePreview();
    }

    private void GeneratePreview()
    {
        if (_loadedTexture == null) return;

        // Unbind and dispose old preview
        if (_previewTexture.HasValue)
        {
            ImGuiCore.Renderer.UnbindTexture(_previewTexture.Value);
            _previewTexture = null;
        }

        if (_previewSurface is not null)
        {
            _previewSurface.Dispose();
            _previewSurface = null;
        }

        int width, height;
        if (_matchOutputToImageSize)
        {
            width = _loadedTexture.Width;
            height = _loadedTexture.Height;
            if (width < 1) width = 1;
            if (height < 1) height = 1;
        }
        else
        {
            width = _outputWidth;
            height = _outputHeight;
        }

        // Get selected conversion settings from indices
        TextureConvertMode convertMode = ImGuiListEnum<TextureConvertMode>.GetValueFromIndex(_convertModeIndex);
        TextureConvertBackgroundStyle backgroundStyle = ImGuiListEnum<TextureConvertBackgroundStyle>.GetValueFromIndex(_backgroundStyleIndex);
        TextureConvertForegroundStyle foregroundStyle = ImGuiListEnum<TextureConvertForegroundStyle>.GetValueFromIndex(_foregroundStyleIndex);

        // Determine colorKey - use the selected color if enabled, otherwise transparent (default behavior)
        Color? colorKey = _useColorKey ? _colorKey.ToColor() : null;

        // Use GameTexture to convert to surface
        Host.GameTexture gameTexture = new(_loadedTexture, false);
        ICellSurface surface = gameTexture.ToSurface(
            convertMode,
            width,
            height,
            backgroundStyle,
            foregroundStyle,
            colorKey);

        // Create preview surface with font
        _previewSurface = new ScreenSurface(surface);
        _previewSurface.Font = _font;
        _previewSurface.FontSize = _fontSize;
        _previewSurface.Update(TimeSpan.Zero);
        _previewSurface.Render(TimeSpan.Zero);

        _previewTexture = ImGuiCore.Renderer.BindTexture(
            ((Host.GameTexture)_previewSurface.Renderer.Output).Texture);
        _previewTextureSize = new Vector2(_previewSurface.Renderer.Output.Width, _previewSurface.Renderer.Output.Height);
    }

    private bool _pendingClose = false;

    public override void BuildUI(ImGuiRenderer renderer)
    {
        // Handle deferred unload
        if (_pendingClose)
        {
            base.OnClosed();
            UnloadTextures();
            _pendingClose = false;
            return;
        }
        
        if (!IsOpen) return;

        if (_currentMode == WindowMode.FilePicker)
            BuildFilePickerUI(renderer);
        else
            BuildConverterUI(renderer);
    }

    private void BuildFilePickerUI(ImGuiRenderer renderer)
    {
        ImGui.OpenPopup("Select Image File");

        ImGuiSC.CenterNextWindow();
        ImGui.SetNextWindowSize(new Vector2(550, -1));

        if (ImGui.BeginPopupModal("Select Image File", ref IsOpen))
        {
            if (ImGui.BeginTable("table1", 2, ImGuiTableFlags.BordersInnerV))
            {
                ImGui.TableSetupColumn("one", ImGuiTableColumnFlags.WidthFixed, ImGui.GetContentRegionAvail().X / 3);
                ImGui.TableSetupColumn("two");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.AlignTextToFramePadding();
                ImGui.Text("File Types");
                ImGui.SetNextItemWidth(-1);
                ImGui.ListBox("##handlers", ref _fileLoaders.SelectedItemIndex, _fileLoaders.Names, _fileLoaders.Count, 5);

                ImGui.TableSetColumnIndex(1);
                if (_fileLoaders.IsItemSelected())
                {
                    _fileListBox.SearchPattern = string.Join(';', _fileLoaders.SelectedItem.ExtensionsLoading.Select(ex => $"*.{ex}"));
                    _fileListBox.Begin("listbox", new(-1, 300));
                    _fileListBox.Draw();
                    _fileListBox.End();
                }

                ImGui.EndTable();
            }

            ImGui.Separator();

            if (DrawButtons(out DialogResult, _fileListBox.HighlightedItem == null || _fileListBox.IsHighlightedItemDirectory, "Cancel", "Open"))
            {
                if (DialogResult)
                {
                    FileInfo selectedFile = (FileInfo)_fileListBox.HighlightedItem!;
                    LoadImage(selectedFile.FullName);
                    _currentMode = WindowMode.ImageConverter;
                }
                else
                {
                    Close();
                }
            }

            ImGui.EndPopup();
        }
    }

    private void BuildConverterUI(ImGuiRenderer renderer)
    {
        ImGui.OpenPopup(Title);

        ImGuiSC.CenterNextWindow();
        ImGui.SetNextWindowSize(new Vector2(Core.Settings.WindowSimpleObjectEditor * ImGui.GetFontSize(), -1), ImGuiCond.Appearing);
        if (ImGui.BeginPopupModal(Title, ref IsOpen))
        {
            if (_loadedTexture == null)
            {
                ImGui.Text("No image loaded.");
                ImGui.EndPopup();
                return;
            }

            ImGui.Columns(2);

            float pos = ImGui.GetCursorPosY();

            // Left column: Source image
            ImGui.Text("Source Image");
            ImGui.Text($"Size: {_loadedTexture.Width} x {_loadedTexture.Height} pixels");

            bool regenerate = false;

            if (_sourceImageTexture.HasValue)
            {
                var region = ImGui.GetContentRegionAvail();
                float previewHeight = 200;

                // Scale to fit
                float scale = Math.Min(region.X / _sourceImageSize.X, previewHeight / _sourceImageSize.Y);
                Vector2 scaledSize = new(_sourceImageSize.X * scale, _sourceImageSize.Y * scale);

                // Get the cursor position before drawing the image
                Vector2 imagePos = ImGui.GetCursorScreenPos();

                ImGui.Image(_sourceImageTexture.Value, scaledSize);

                // Check for click on the source image to pick colorkey
                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    Vector2 mousePos = ImGui.GetMousePos();
                    Vector2 relativePos = mousePos - imagePos;

                    // Convert from scaled coordinates to original image coordinates
                    int pixelX = (int)(relativePos.X / scale);
                    int pixelY = (int)(relativePos.Y / scale);

                    // Clamp to valid range
                    pixelX = Math.Clamp(pixelX, 0, _loadedTexture.Width - 1);
                    pixelY = Math.Clamp(pixelY, 0, _loadedTexture.Height - 1);

                    // Get the pixel color from the texture
                    Host.GameTexture gameTexture = new(_loadedTexture, false);
                    Color pickedColor = gameTexture.GetPixel(new Point(pixelX, pixelY));
                    _colorKey = pickedColor.ToVector4();
                    _useColorKey = true;
                    regenerate = true;
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Click to pick colorkey from image");
                }
            }

            // Size settings
            ImGui.Text("Output Settings");

            if (ImGui.Checkbox("Match image size", ref _matchOutputToImageSize))
            {
                regenerate = true;
            }

            ImGui.BeginDisabled(_matchOutputToImageSize);
            {
                ImGui.SetNextItemWidth(100);
                ImGui.InputInt("Width", ref _outputWidth);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    _outputWidth = Math.Clamp(_outputWidth, 1, _loadedTexture.Width);
                    regenerate = true;
                }


                ImGui.SetNextItemWidth(100);
                ImGui.InputInt("Height", ref _outputHeight);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    _outputHeight = Math.Clamp(_outputHeight, 1, _loadedTexture.Height);
                    regenerate = true;
                }
            }
            ImGui.EndDisabled();

            ImGui.Separator();
            ImGui.Text("Conversion Style");

            // Convert Mode combobox
            ImGui.SetNextItemWidth(150);
            if (ImGui.Combo("Mode", ref _convertModeIndex,
                ImGuiListEnum<TextureConvertMode>.Names,
                ImGuiListEnum<TextureConvertMode>.Count))
            {
                regenerate = true;
            }

            // Show appropriate style based on mode
            TextureConvertMode currentMode = ImGuiListEnum<TextureConvertMode>.GetValueFromIndex(_convertModeIndex);

            if (currentMode == TextureConvertMode.Background)
            {
                ImGui.SetNextItemWidth(150);
                if (ImGui.Combo("Background Style", ref _backgroundStyleIndex,
                    ImGuiListEnum<TextureConvertBackgroundStyle>.Names,
                    ImGuiListEnum<TextureConvertBackgroundStyle>.Count))
                {
                    regenerate = true;
                }
            }
            else // Foreground mode
            {
                ImGui.SetNextItemWidth(150);
                if (ImGui.Combo("Foreground Style", ref _foregroundStyleIndex,
                    ImGuiListEnum<TextureConvertForegroundStyle>.Names,
                    ImGuiListEnum<TextureConvertForegroundStyle>.Count))
                {
                    regenerate = true;
                }
            }

            ImGui.Separator();

            if (ImGui.Checkbox("Use Colorkey", ref _useColorKey))
            {
                regenerate = true;
            }

            ImGui.SameLine();
            if (ImGui.ColorEdit4("##colorkey", ref _colorKey, ImGuiColorEditFlags.NoInputs))
                regenerate = true;

            if (regenerate)
            {
                GeneratePreview();
            }

            ImGui.NextColumn();

            ImGui.SetCursorPosY(pos);

            // Right column: ASCII preview
            ImGui.Text("ASCII Preview");

            if (_previewSurface != null)
            {
                ImGui.Text($"Surface: {_previewSurface.Surface.Width} x {_previewSurface.Surface.Height} cells");
            }

            if (_previewTexture.HasValue)
            {
                var region = ImGui.GetContentRegionAvail();
                region.Y -= ImGui.GetTextLineHeightWithSpacing() * 2;
                // Scale to fit while maintaining aspect ratio
                float scale = Math.Min(region.X / _previewTextureSize.X, region.Y / _previewTextureSize.Y);
                if (scale > 1) scale = 1; // Don't scale up
                Vector2 scaledSize = new(_previewTextureSize.X * scale, _previewTextureSize.Y * scale);

                ImGuiSC.DrawTexture("ascii_preview", true, ImGuiSC.ZoomFit,
                    _previewTexture.Value,
                    _previewTextureSize,
                    scaledSize,
                    out _, out _, true);
            }

            ImGui.Columns(1);

            ImGui.Separator();

            if (DrawButtons(out DialogResult, _previewSurface == null, "Cancel", "OK"))
            {
                if (DialogResult && _previewSurface != null)
                {
                    // Copy the surface for the result
                    ResultSurface = new CellSurface(_previewSurface.Surface);
                }

                Close();
            }

            ImGui.EndPopup();
        }
    }

    protected override void OnClosed()
    {
        _pendingClose = true;  // Defer unload to next frame
    }
}
