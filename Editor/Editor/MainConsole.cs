using SadConsole;
using Console = SadConsole.Console;
using System.Linq;
using System;
using SadRogue.Primitives;
using System.Collections.Generic;
using SadConsole.Input;
using Microsoft.Extensions.DependencyInjection;

namespace SadConsoleEditor
{
    public class MainConsole : Console
    {
        public static MainConsole Instance;

        public Dictionary<string, Editors.IEditorMetadata> EditorTypes;
        public Dictionary<string, FileLoaders.IFileLoader> FileLoaders;

        private Console topBarPane;
        public Consoles.ToolPane ToolsPane;
        public Consoles.QuickSelectPane QuickSelectPane;
        private Console _borderConsole;

        public List<Editors.IEditor> OpenEditors;

        public Rectangle InnerEmptyBounds;
        public Rectangle InnerEmptyBoundsPixels;

        public Rectangle BorderBounds;

        private string topBarLayerName = "None";
        private string topBarToolName = "None";
        private Point topBarMousePosition;

        public bool DisableBrush = false;

        private SadConsole.Entities.Entity brush;

        public SadConsole.Entities.Entity Brush
        {
            get { return brush; }
            set
            {
                if (brush != null)
                    brush.Parent = null;

                brush = value;
                Children.Add(brush);

                //if (value != null)
                //    value.Parent = borderConsole;
            }
        }
        public bool AllowKeyboardToMoveConsole;
        public new bool UseKeyboard;

        public Editors.IEditor ActiveEditor { get; private set; }

        public string LayerName
        {
            set
            {
                topBarLayerName = value;
                RefreshBackingPanel();
            }
        }

        public string ToolName
        {
            set
            {
                topBarToolName = value;
                RefreshBackingPanel();
            }
        }

        public Point SurfaceMouseLocation
        {
            set
            {
                topBarMousePosition = value;
                RefreshBackingPanel();
            }
        }

        public MainConsole(): base(Config.Program.WindowWidth, Config.Program.WindowHeight)
        {
            Renderer = null;
            MainConsole.Instance = this;
            UseKeyboard = true;

            // Create the basic consoles
            QuickSelectPane = new SadConsoleEditor.Consoles.QuickSelectPane();
            QuickSelectPane.Redraw();
            QuickSelectPane.IsVisible = false;

            topBarPane = new SadConsole.Console(Config.Program.WindowWidth, 1);
            topBarPane.DefaultBackground = SadConsole.UI.Themes.Library.Default.Colors.ControlHostBackground;
            topBarPane.Clear();
            topBarPane.FocusOnMouseClick = false;
            topBarPane.IsVisible = false;

            ToolsPane = new Consoles.ToolPane();
            ToolsPane.Position = new Point(Config.Program.WindowWidth - ToolsPane.Width - 1, 1);
            ToolsPane.IsVisible = false;

            // Get the whitespace in the middle of the screen
            var boundsLocation = new Point(1, topBarPane.Height).TranslateFont(topBarPane.FontSize, Config.Program.ScreenFontSize);
            boundsLocation = boundsLocation.WithY(boundsLocation.Y + 1);

            InnerEmptyBounds = new Rectangle(boundsLocation,
                                    new Point(
                                        new Point(ToolsPane.Position.X, 0).TranslateFont(SadConsole.GameHost.Instance.DefaultFont.GetFontSize(SadConsole.GameHost.Instance.DefaultFontSize), Config.Program.ScreenFontSize).X - boundsLocation.X - 1,
                                        new Point(0, QuickSelectPane.Position.Y).PixelLocationToSurface(Config.Program.ScreenFontSize).Y - 1 - boundsLocation.Y));

            InnerEmptyBoundsPixels = new Rectangle(InnerEmptyBounds.Position.SurfaceLocationToPixel(Config.Program.ScreenFontSize), InnerEmptyBounds.Size.SurfaceLocationToPixel(Config.Program.ScreenFontSize));

            _borderConsole = new Console(InnerEmptyBounds.Width, InnerEmptyBounds.Height);
            _borderConsole.Font = Config.Program.ScreenFont;
            _borderConsole.FontSize = Config.Program.ScreenFontSize;
            _borderConsole.Position = InnerEmptyBounds.Position;
            
            // Add the consoles to the main console list
            Children.Add(_borderConsole);
            Children.Add(QuickSelectPane);
            Children.Add(topBarPane);
            Children.Add(ToolsPane);

            // Setup the file types for base editors.
            OpenEditors = new List<SadConsoleEditor.Editors.IEditor>();

            // Load tools
            var collection = new ServiceCollection();
            var assemblies = new List<System.Reflection.Assembly>();

            // Scan plugins
            var pluginsDir = System.IO.Path.Combine(AppContext.BaseDirectory, "Plugins");
            foreach (var dir in System.IO.Directory.GetDirectories(pluginsDir))
            {
                var dirName = System.IO.Path.GetFileName(dir);
                var pluginDll = System.IO.Path.Combine(dir, dirName + ".dll");
                if (System.IO.File.Exists(pluginDll))
                {
                    assemblies.Add(System.Reflection.Assembly.LoadFile(pluginDll));

                    var settingsFile = System.IO.Path.GetRelativePath(AppContext.BaseDirectory, System.IO.Path.Combine(dir, dirName + ".json"));
                    
                    if (!System.IO.File.Exists(settingsFile))
                        throw new Exception($"Plugin must have a settings file: {settingsFile}");

                    var settings = SadConsole.Serializer.Load<EditorSettings[]>(settingsFile, false);
                    foreach (var setting in settings)
                        Config.Program.AddSettings(setting.EditorId, setting);
                }
            }

            collection.Scan(scan =>
                scan.FromAssemblies(assemblies)
                    .AddClasses(classes => classes.AssignableTo<Tools.ITool>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()

                    .AddClasses(classes => classes.AssignableTo<SadConsoleEditor.Editors.IEditorMetadata>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()

                    .AddClasses(classes => classes.AssignableTo<SadConsoleEditor.FileLoaders.IFileLoader>()) 
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()

                    .AddClasses(classes => classes.AssignableTo<IPlugin>())
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()
                );

            // Create an instance of plugin types
            var provider = collection.BuildServiceProvider();
            var plugins = provider.GetServices<IPlugin>();
            var tools = provider.GetServices<Tools.ITool>();

            EditorTypes = provider.GetServices<SadConsoleEditor.Editors.IEditorMetadata>().ToDictionary(i => i.Id);
            FileLoaders = provider.GetServices<SadConsoleEditor.FileLoaders.IFileLoader>().ToDictionary(i => i.Id);

            foreach (var plugin in plugins)
                plugin.Register();

            foreach (var tool in tools)
                ToolsPane.RegisterTool(tool);

            IsExclusiveMouse = true;
            SadConsole.GameHost.Instance.FocusedScreenObjects.Set(this);
        }

        public void ShowStartup()
        {
            SadConsole.UI.Window.Prompt("Create new or open existing?", "New", "Open",
            (b) =>
            {
                if (b)
                {
                    var popup = new Windows.NewConsolePopup();
                    popup.Center();
                    popup.Closed += (s, e) => { if (!popup.DialogResult) ShowStartup(); else CreateNewEditor(popup.Editor, popup.SettingWidth, popup.SettingHeight, popup.SettingForeground, popup.SettingBackground); };
                    popup.Show(true);
                }
                else
                {
                    Windows.SelectEditorFilePopup popup = new Windows.SelectEditorFilePopup();
                    popup.Center();
                    popup.Closed += (s, e) => { if (!popup.DialogResult) ShowStartup(); else LoadEditor(popup.SelectedFile, popup.SelectedEditor, popup.SelectedLoader); };
                    popup.Show(true);
                }

            });
        }


        private void CreateNewEditor(SadConsoleEditor.Editors.IEditorMetadata editor, int width, int height, Color defaultForeground, Color defaultBackground)
        {
            var createdEditor = editor.Create();
            createdEditor.New(defaultForeground, defaultBackground, width, height);
            AddEditor(createdEditor, true);
            topBarPane.IsVisible = true;
            ToolsPane.IsVisible = true;
        } 

        private void LoadEditor(string file, Editors.IEditorMetadata editor, FileLoaders.IFileLoader loader)
        {
            var createdEditor = editor.Create();
            createdEditor.Load(file, loader);
            createdEditor.Metadata.FilePath = file;
            createdEditor.Metadata.IsLoaded = true;
            createdEditor.Metadata.Title = System.IO.Path.GetFileNameWithoutExtension(file);
            createdEditor.Metadata.LastLoader = loader;
            AddEditor(createdEditor, true);

            topBarPane.IsVisible = true;
            ToolsPane.IsVisible = true;
        }

        public void ShowCloseConsolePopup() =>
            SadConsole.UI.Window.Prompt(new SadConsole.ColoredString("Are you sure? You will lose any unsaved changes."), "Yes", "No", (r) => { if (r) RemoveEditor(ActiveEditor); });

        public void ShowNewEditorPopup()
        {
            Windows.NewConsolePopup popup = new Windows.NewConsolePopup();
            popup.Center();
            popup.Closed += (s, e) => 
            { 
                if (popup.DialogResult) 
                    CreateNewEditor(popup.Editor, popup.SettingWidth, popup.SettingHeight, popup.SettingForeground, popup.SettingBackground); 
            };
            popup.Show(true);
        }

        public void ShowLoadEditorPopup()
        {
            Windows.SelectEditorFilePopup popup = new Windows.SelectEditorFilePopup();
            popup.Center();
            popup.Closed += (s, e) => 
            {
                if (popup.DialogResult)
                    LoadEditor(popup.SelectedFile, popup.SelectedEditor, popup.SelectedLoader);
                else if (ActiveEditor == null)
                    ShowStartup(); 
                
            };

            popup.Show(true);
        }

        public void ShowResizeEditorPopup()
        {
            if (ActiveEditor != null)
            {
                Windows.ResizeSurfacePopup popup = new Windows.ResizeSurfacePopup(ActiveEditor.Width, ActiveEditor.Height);
                popup.Center();
                popup.Closed += (s, e) =>
                {
                    if (popup.DialogResult)
                    {
                        ActiveEditor.Resize(popup.SettingWidth, popup.SettingHeight);
                    }
                };
                popup.Show(true);
            }
        }

        public void AddEditor(Editors.IEditor editor, bool show)
        {
            OpenEditors.Add(editor);
            ToolsPane.PanelFiles.DocumentsListbox.Items.Add(editor);

            if (show)
                ChangeActiveEditor(editor);
        }

        public void RemoveEditor(Editors.IEditor editor)
        {
            ToolsPane.PanelFiles.DocumentsListbox.Items.Remove(editor);
            editor.OnClosed();
            OpenEditors.Remove(editor);

            if (OpenEditors.Count == 0)
                ShowStartup();
            else
                ChangeActiveEditor(OpenEditors[0]);
        }

        public void ChangeActiveEditor(Editors.IEditor editor)
        {
            AllowKeyboardToMoveConsole = true;

            if (ActiveEditor != null)
            {
                ActiveEditor.OnDeselected();
                ActiveEditor.Object.Parent = null;
                _borderConsole.Clear();
            }

            if (OpenEditors.Contains(editor))
            {
                ActiveEditor = editor;
                ActiveEditor.OnSelected();
                Children.Add(ActiveEditor.Object);
                ToolsPane.RedrawPanels();
                
                CenterEditor();

                Children.MoveToTop(brush);

                if (ToolsPane.PanelFiles.DocumentsListbox.SelectedItem != editor)
                    ToolsPane.PanelFiles.DocumentsListbox.SelectedItem = editor;
            }
        }

        public void SaveEditor()
        {
            if (ActiveEditor != null)
            {
                if (!ActiveEditor.Metadata.IsLoaded && !ActiveEditor.Metadata.IsSaved)
                    SaveAsEditor();
                else
                {
                    ActiveEditor.Save(ActiveEditor.Metadata.FilePath, ActiveEditor.Metadata.LastLoader);
                }
            }
        }
        public void SaveAsEditor()
        {
            var popup = new Windows.SelectEditorFilePopup(ActiveEditor.Metadata);
            popup.Center();
            popup.SkipFileExistCheck = true;
            popup.Closed += (s, e) =>
            {
                if (popup.DialogResult)
                {
                    if (ActiveEditor.Save(popup.SelectedFile, popup.SelectedLoader))
                    {
                        ActiveEditor.Metadata.IsSaved = true;
                        ActiveEditor.Metadata.FilePath = popup.SelectedFile;
                        ActiveEditor.Metadata.LastLoader = popup.SelectedLoader;
                    }
                }
            };

            popup.SelectButtonText = "Save";
            popup.Show(true);
        }

        public void ShowSaveErrorPopup() =>
            SadConsole.UI.Window.Message("Unable to save file", "Close");

        public void CenterEditor()
        {
            Point position = new Point();
            var editorBounds = InnerEmptyBounds;
            editorBounds = editorBounds.Expand(-1, -1);

            if (ActiveEditor.Width > editorBounds.Width || ActiveEditor.Height > editorBounds.Height)
            {
                // Need scrolling console
                if (ActiveEditor.Width > editorBounds.Width && ActiveEditor.Height > editorBounds.Height)
                {
                    position = editorBounds.Position;
                    ActiveEditor.Object.Surface.View = new Rectangle(0, 0, editorBounds.Width, editorBounds.Height);
                }
                else if (ActiveEditor.Width > editorBounds.Width)
                {
                    position = new Point(editorBounds.Position.X, (editorBounds.Height + editorBounds.Y - ActiveEditor.Height) / 2);
                    ActiveEditor.Object.Surface.View = new Rectangle(0, 0, editorBounds.Width, ActiveEditor.Height);
                }
                else if (ActiveEditor.Height > editorBounds.Height)
                {
                    position = new Point((editorBounds.Width + editorBounds.X - ActiveEditor.Width) / 2, editorBounds.Position.Y);
                    ActiveEditor.Object.Surface.View = new Rectangle(0, 0, ActiveEditor.Width, editorBounds.Height);
                }
            }
            else
            {
                // Center normal
                position = new Point((editorBounds.Width + editorBounds.X - ActiveEditor.Width) / 2, (editorBounds.Height + editorBounds.Y - ActiveEditor.Height) / 2);
            }

            if (position.X < editorBounds.X)
                position = position.WithX(editorBounds.X);

            if (position.Y < editorBounds.Y)
                position = position.WithY(editorBounds.Y);

            ActiveEditor.Object.Position = position;
            BorderBounds = new Rectangle(ActiveEditor.Object.Position.X - 1 - _borderConsole.Position.X, ActiveEditor.Object.Position.Y - 1 - _borderConsole.Position.Y, ActiveEditor.Object.Surface.View.Width + 2, ActiveEditor.Object.Surface.View.Height + 2);
            _borderConsole.Clear();
            _borderConsole.DrawBox(BorderBounds, new ColoredGlyph(Surface.DefaultForeground, Color.Black, 177));
        }

        private void RefreshBackingPanel()
        {
            topBarPane.Clear();

            var text = new SadConsole.ColoredString("   X: ", SadConsole.UI.Themes.Library.Default.Colors.Title, Color.Transparent)     + new SadConsole.ColoredString(topBarMousePosition.X.ToString(), SadConsole.UI.Themes.Library.Default.Colors.ControlHostForeground, Color.Transparent) +
                       new SadConsole.ColoredString(" Y: ", SadConsole.UI.Themes.Library.Default.Colors.Title, Color.Transparent)       + new SadConsole.ColoredString(topBarMousePosition.Y.ToString(), SadConsole.UI.Themes.Library.Default.Colors.ControlHostForeground, Color.Transparent) +
                       new SadConsole.ColoredString("   Layer: ", SadConsole.UI.Themes.Library.Default.Colors.Title, Color.Transparent) + new SadConsole.ColoredString(topBarLayerName, SadConsole.UI.Themes.Library.Default.Colors.ControlHostForeground, Color.Transparent) +
                       new SadConsole.ColoredString("   Tool: ", SadConsole.UI.Themes.Library.Default.Colors.Title, Color.Transparent)  + new SadConsole.ColoredString(topBarToolName, SadConsole.UI.Themes.Library.Default.Colors.ControlHostForeground, Color.Transparent);
            text.IgnoreBackground = true;
            topBarPane.Print(0, 0, text);
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            ActiveEditor?.Update();
        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            if (brush != null && !DisableBrush)
            {
                if (MainConsole.Instance.InnerEmptyBounds.Contains(state.WorldCellPosition))
                {
                    brush.IsVisible = true;
                    brush.Position = state.WorldCellPosition;
                    if (MainConsole.Instance.ActiveEditor != null)
                    {
                        state = new MouseScreenObjectState(MainConsole.Instance.ActiveEditor.Object, state.Mouse);
                        SurfaceMouseLocation = state.CellPosition;
                        return MainConsole.Instance.ActiveEditor.ProcessMouse(state, true);
                    }
                }
                else
                {
                    brush.IsVisible = false;
                }
            }

            if (ToolsPane.ProcessMouse(new MouseScreenObjectState(ToolsPane, state.Mouse)))
                return true;

            return false;
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            if (UseKeyboard)
            {
                bool movekeyPressed = false;

                if (AllowKeyboardToMoveConsole && ActiveEditor != null && ActiveEditor.Object != null)
                {
                    bool shifted = SadConsole.GameHost.Instance.Keyboard.IsKeyDown(Keys.LeftShift) || SadConsole.GameHost.Instance.Keyboard.IsKeyDown(Keys.RightShift);
                    var oldRenderArea = ActiveEditor.Object.Surface.View;

                    if (!shifted && SadConsole.GameHost.Instance.Keyboard.IsKeyDown(Keys.Left))
                        ActiveEditor.Object.Surface.View = new Rectangle(ActiveEditor.Object.Surface.View.X - 1, ActiveEditor.Object.Surface.View.Y, BorderBounds.Width - 2, BorderBounds.Height - 2);

                    else if (!shifted && SadConsole.GameHost.Instance.Keyboard.IsKeyDown(Keys.Right))
                        ActiveEditor.Object.Surface.View = new Rectangle(ActiveEditor.Object.Surface.View.X + 1, ActiveEditor.Object.Surface.View.Y, BorderBounds.Width - 2, BorderBounds.Height - 2);

                    if (!shifted && SadConsole.GameHost.Instance.Keyboard.IsKeyDown(Keys.Up))
                        ActiveEditor.Object.Surface.View = new Rectangle(ActiveEditor.Object.Surface.View.X, ActiveEditor.Object.Surface.View.Y - 1, BorderBounds.Width - 2, BorderBounds.Height - 2);

                    else if (!shifted && SadConsole.GameHost.Instance.Keyboard.IsKeyDown(Keys.Down))
                        ActiveEditor.Object.Surface.View = new Rectangle(ActiveEditor.Object.Surface.View.X, ActiveEditor.Object.Surface.View.Y + 1, BorderBounds.Width - 2, BorderBounds.Height - 2);

                    movekeyPressed = oldRenderArea != ActiveEditor.Object.Surface.View;

                }

                if (!movekeyPressed)
                {
                    //if (info.IsKeyReleased(Keys.Subtract))
                    //{
                    //	SelectedEditor.Surface.ResizeCells(SelectedEditor.Surface.CellSize.X / 2, SelectedEditor.Surface.CellSize.Y / 2);
                    //}
                    //else if (info.IsKeyReleased(Keys.Add))
                    //{
                    //	SelectedEditor.Surface.ResizeCells(SelectedEditor.Surface.CellSize.X * 2, SelectedEditor.Surface.CellSize.Y * 2);
                    //}
                    //else
                    {
                        // Look for tool hotkeys
                        if (ToolsPane.ProcessKeyboard(SadConsole.GameHost.Instance.Keyboard))
                        {
                        }
                        // Look for quick select F* keys
                        else if (QuickSelectPane.ProcessKeyboard(SadConsole.GameHost.Instance.Keyboard))
                        {
                        }
                        else if (ActiveEditor != null)
                        {
                            ActiveEditor.ProcessKeyboard(SadConsole.GameHost.Instance.Keyboard);
                        }
                    }
                }
            }

            // Always return true so that the virtual cursor doesn't start working.
            return true;
        }


    }
}
