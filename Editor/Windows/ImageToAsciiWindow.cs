using System.Numerics;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Windows;

public static class ImageToAsciiWindow
{
    public static void Show(ImGuiRenderer renderer, IFont font, Point fontSize, Action<ICellSurface> onImported, Action? onCancelled)
    {
        Instance instance = new(font, fontSize, onImported, onCancelled);
        renderer.UIObjects.Add(instance);
    }

    private class Instance : ImGuiObjectBase
    {
        private enum WindowMode
        {
            FilePicker,
            ImageConverter
        }

        private WindowMode _currentMode = WindowMode.FilePicker;
        private bool _firstShow = true;

        // File picker components
        private FileListBox _fileListBox;
        private ImGuiList<IFileHandler> _fileLoaders;

        // Image data
        private Texture2D? _loadedTexture;
        private ImTextureRef? _sourceImageTexture;
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
        private ImTextureRef? _previewTexture;
        private Vector2 _previewTextureSize;

        // Font info
        private readonly IFont _font;
        private readonly Point _fontSize;

        // Callbacks
        Action<ICellSurface> _onImported;
        Action? _onCancelled;

        public Instance(IFont font, Point fontSize, Action<ICellSurface> OnImported, Action? OnCancelled)
        {
            _font = font;
            _fontSize = fontSize;

            _fileListBox = new(Core.State.RootFolder);
            _fileLoaders = new ImGuiList<IFileHandler>(0, [new ImageFile()]);

            _onImported = OnImported;
            _onCancelled = OnCancelled;
        }

        private void UnloadTextures()
        {
            if (_sourceImageTexture.HasValue)
            {
                Core.ImGuiComponent.ImGuiRenderer.UnbindTexture(_sourceImageTexture.Value.TexID);
                _sourceImageTexture = null;
            }

            if (_previewTexture.HasValue)
            {
                Core.ImGuiComponent.ImGuiRenderer.UnbindTexture(_previewTexture.Value.TexID);
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

            _sourceImageTexture = Core.ImGuiComponent.ImGuiRenderer.BindTexture(_loadedTexture);
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
                Core.ImGuiComponent.ImGuiRenderer.UnbindTexture(_previewTexture.Value.TexID);
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

            _previewTexture = Core.ImGuiComponent.ImGuiRenderer.BindTexture(((Host.GameTexture)_previewSurface.Renderer!.Output).Texture);
            _previewTextureSize = new Vector2(_previewSurface.Renderer.Output.Width, _previewSurface.Renderer.Output.Height);
        }

        private bool _pendingClose = false;

        public override void BuildUI(ImGuiRenderer renderer)
        {
            // Handle deferred unload
            if (_pendingClose)
            {
                UnloadTextures();
                renderer.UIObjects.Remove(this);
                return;
            }

            if (_currentMode == WindowMode.FilePicker)
                BuildFilePickerUI(renderer);
            else
                BuildConverterUI(renderer);
        }

        private void BuildFilePickerUI(ImGuiRenderer renderer)
        {
            if (_firstShow)
            {
                ImGui.OpenPopup("Select Image File"u8);
                _firstShow = false;
            }

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new Vector2(550, -1));

            if (ImGui.BeginPopupModal("Select Image File"u8, ImGuiWindowFlags.NoResize))
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

                if (ImGuiSC.WindowDrawButtons(out bool dialogResult, _fileListBox.HighlightedItem == null || _fileListBox.IsHighlightedItemDirectory, "Cancel", "Open"))
                {
                    if (dialogResult)
                    {
                        FileInfo selectedFile = (FileInfo)_fileListBox.HighlightedItem!;
                        LoadImage(selectedFile.FullName);
                        ImGui.CloseCurrentPopup();
                        _firstShow = true;
                        _currentMode = WindowMode.ImageConverter;
                    }
                    else
                    {
                        ImGui.CloseCurrentPopup();
                        _pendingClose = true;
                        _onCancelled?.Invoke();
                    }
                }
                ImGui.EndPopup();
            }
        }

        private void BuildConverterUI(ImGuiRenderer renderer)
        {
            if (_firstShow)
            {
                ImGui.OpenPopup("Image Converter"u8);
                _firstShow = false;
            }

            ImGuiSC.CenterNextWindowOnAppearing(new Vector2(Core.Settings.WindowSimpleObjectEditor * ImGui.GetFontSize(), -1));
            if (ImGui.BeginPopupModal("Image Converter"u8, ImGuiWindowFlags.NoResize))
            {
                if (_loadedTexture == null)
                    ImGui.Text("No image loaded.");

                else
                {
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

                    if (ImGuiSC.WindowDrawButtons(out bool dialogResult, _previewSurface == null, "Cancel", "OK"))
                    {
                        ImGui.CloseCurrentPopup();
                        _pendingClose = true; // Defered unloading

                        if (dialogResult && _previewSurface != null)
                            _onImported(new CellSurface(_previewSurface.Surface));
                        else
                            _onCancelled?.Invoke();
                    }
                }
                ImGui.EndPopup();
            }
        }
    }
}
