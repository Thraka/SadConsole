using SadConsole.Surfaces;
using SadConsole;
using Console = SadConsole.Console;
using System.Linq;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SadConsole.Input;

namespace SadConsoleEditor
{
    class MainScreen : Screen
    {
        public static MainScreen Instance;

        public Dictionary<Type, FileLoaders.IFileLoader[]> EditorFileTypes;
        public Dictionary<string, Editors.Editors> Editors;

        private Console topBarPane;
        private Consoles.BorderConsole borderConsole;
        private Consoles.BrushConsoleContainer brushScreen;
        public Consoles.ToolPane ToolsPane;
        public Consoles.QuickSelectPane QuickSelectPane;

        public List<Editors.IEditor> OpenEditors;

        public Rectangle InnerEmptyBounds;
        public Rectangle InnerEmptyBoundsPixels;
        public Point InnerBorderPosition => borderConsole.Position + new Point(1);

        private string topBarLayerName = "None";
        private string topBarToolName = "None";
        private Point topBarMousePosition;

        private SadConsole.GameHelpers.Entity brush;

        public SadConsole.GameHelpers.Entity Brush
        {
            get { return brush; }
            set
            {
                if (brush != null)
                    brush.Parent = null;

                brush = value;

                if (value != null)
                    value.Parent = borderConsole;
            }
        }
        public bool AllowKeyboardToMoveConsole;
        public bool UseKeyboard;

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

        public  Point SurfaceMouseLocation
        {
            set
            {
                topBarMousePosition = value;
                RefreshBackingPanel();
            }
        }

        public MainScreen()
        {
            MainScreen.Instance = this;
            UseKeyboard = true;

            // Create the basic consoles
            QuickSelectPane = new SadConsoleEditor.Consoles.QuickSelectPane();
            QuickSelectPane.Redraw();
            QuickSelectPane.IsVisible = false;

            topBarPane = new SadConsole.Console(Settings.Config.WindowWidth, 1);
            topBarPane.TextSurface.DefaultBackground = Settings.Color_MenuBack;
            topBarPane.Clear();
            topBarPane.FocusOnMouseClick = false;
            topBarPane.IsVisible = false;

            borderConsole = new SadConsoleEditor.Consoles.BorderConsole(10, 10);
            borderConsole.IsVisible = false;
            //borderConsole.UseMouse = false;
            borderConsole.MouseHandler = ProcessMouseForBrush;
            borderConsole.KeyboardHandler = BorderConsoleKeyboardHandler;

            ToolsPane = new Consoles.ToolPane();
            ToolsPane.Position = new Point(Settings.Config.WindowWidth - ToolsPane.Width - 1, 1);
            ToolsPane.IsVisible = false;

            brushScreen = new Consoles.BrushConsoleContainer();
            

            var boundsLocation = new Point(0, topBarPane.TextSurface.Height).TranslateFont(topBarPane.TextSurface.Font, Settings.Config.ScreenFont) + new Point(1);
            InnerEmptyBounds = new Rectangle(boundsLocation, new Point(1, QuickSelectPane.Position.Y).PixelLocationToConsole(QuickSelectPane.TextSurface.Font.Size.X, QuickSelectPane.TextSurface.Font.Size.Y)  - boundsLocation);
            InnerEmptyBounds.Width = new Point(ToolsPane.Position.X - 1, 0).TranslateFont(SadConsole.Global.FontDefault, Settings.Config.ScreenFont).X - 1;
            InnerEmptyBoundsPixels = new Rectangle(InnerEmptyBounds.Location.ConsoleLocationToPixel(Settings.Config.ScreenFont), InnerEmptyBounds.Size.ConsoleLocationToPixel(Settings.Config.ScreenFont));

            // Add the consoles to the main console list
            Children.Add(borderConsole);
            Children.Add(brushScreen);
            Children.Add(QuickSelectPane);
            Children.Add(topBarPane);
            Children.Add(ToolsPane);
            SadConsole.Global.FocusedConsoles.Set(borderConsole);
            
            // Setup the file types for base editors.
            EditorFileTypes = new Dictionary<Type, FileLoaders.IFileLoader[]>(3);
            OpenEditors = new List<SadConsoleEditor.Editors.IEditor>();
            //EditorFileTypes.Add(typeof(Editors.DrawingEditor), new FileLoaders.IFileLoader[] { new FileLoaders.SadConsole.Surfaces.Basic() });

            // Add valid editors
            Editors = new Dictionary<string, SadConsoleEditor.Editors.Editors>();
            Editors.Add("Console Draw", SadConsoleEditor.Editors.Editors.Console);
            Editors.Add("Animated Game Object", SadConsoleEditor.Editors.Editors.Entity);
            Editors.Add("Game Scene", SadConsoleEditor.Editors.Editors.Scene);
            //Editors.Add("User Interface Console", SadConsoleEditor.Editors.Editors.GUI);
            
        }
        
        public void ShowStartup()
        {
            Window.Prompt("Create new or open existing?", "New", "Open",
            (b) =>
            {
                if (b)
                {
                    Windows.NewConsolePopup popup = new Windows.NewConsolePopup();
                    popup.Center();
                    popup.Closed += (s, e) => { if (!popup.DialogResult) ShowStartup(); else CreateNewEditor(popup.Editor, popup.SettingWidth, popup.SettingHeight, popup.SettingForeground, popup.SettingBackground); };
                    popup.Show(true);
                }
                else
                {
                    Windows.SelectFilePopup popup = new Windows.SelectFilePopup();
                    popup.Center();
                    popup.Closed += (s, e) => { if (!popup.DialogResult) ShowStartup(); else LoadEditor(popup.SelectedFile, popup.SelectedLoader); };
                    popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.LayeredSurface(), new FileLoaders.SadConsole.Surfaces.Basic(), new FileLoaders.Scene(), new FileLoaders.Entity(), new FileLoaders.Ansi() };
                    popup.Show(true);
                }

            });
        }


        private void CreateNewEditor(Editors.Editors editorType, int width, int height, Color defaultForeground, Color defaultBackground)
        {
            Editors.IEditor editor = null;

            switch (editorType)
            {
                case SadConsoleEditor.Editors.Editors.Console:
                    editor = new Editors.LayeredConsoleEditor();
                    editor.New(defaultForeground, defaultBackground, width, height);
                    break;
                case SadConsoleEditor.Editors.Editors.Entity:
                    editor = new Editors.EntityEditor();
                    editor.New(defaultForeground, defaultBackground, width, height);
                    break;
                case SadConsoleEditor.Editors.Editors.Scene:
                    editor = new Editors.SceneEditor();
                    editor.New(defaultForeground, defaultBackground, width, height);
                    break;
                case SadConsoleEditor.Editors.Editors.GUI:
                    break;
                default:
                    break;
            }

            if (editor != null)
            {
                AddEditor(editor, true);
            }

            topBarPane.IsVisible = true;
            ToolsPane.IsVisible = true;
        }

        private void LoadEditor(string file, FileLoaders.IFileLoader loader)
        {
            Editors.IEditor editor = null;

            if (loader is FileLoaders.LayeredSurface || loader is FileLoaders.SadConsole.Surfaces.Basic || loader is FileLoaders.Ansi)
            {
                editor = new Editors.LayeredConsoleEditor();
                AddEditor(editor, false);
                editor.Load(file, loader);
            }
            else if (loader is FileLoaders.Entity)
            {
                editor = new Editors.EntityEditor();
                AddEditor(editor, false);
                editor.Load(file, loader);
            }
            else if (loader is FileLoaders.Scene)
            {
                editor = new Editors.SceneEditor();
                AddEditor(editor, false);
                editor.Load(file, loader);
            }
            if (editor != null)
            {
                //editor.RenderedConsole.TextSurface.RenderArea = new Rectangle(0, 0, InnerEmptyBounds.Width, InnerEmptyBounds.Height);
                ChangeActiveEditor(editor);
            }

            topBarPane.IsVisible = true;
            ToolsPane.IsVisible = true;
        }

        public void ShowCloseConsolePopup()
        {
            Window.Prompt(new SadConsole.ColoredString("Are you sure? You will lose any unsaved changes."), "Yes", "No", (r) =>
            {
                if (r)
                    RemoveEditor(ActiveEditor);
            });
        }

        public void ShowNewEditorPopup()
        {
            Windows.NewConsolePopup popup = new Windows.NewConsolePopup();
            popup.Center();
            popup.Closed += (s, e) => { if (popup.DialogResult) CreateNewEditor(popup.Editor, popup.SettingWidth, popup.SettingHeight, popup.SettingForeground, popup.SettingBackground); };
            popup.Show(true);
        }

        public void ShowLoadEditorPopup()
        {
            Windows.SelectFilePopup popup = new Windows.SelectFilePopup();
            popup.Center();
            popup.Closed += (s, e) => { if (popup.DialogResult) LoadEditor(popup.SelectedFile, popup.SelectedLoader); };
            popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.LayeredSurface(), new FileLoaders.SadConsole.Surfaces.Basic(), new FileLoaders.Scene(), new FileLoaders.Entity(), new FileLoaders.Ansi() };
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

        public void RefreshBorder()
        {
            borderConsole.SetContent(ActiveEditor.Surface, ActiveEditor.Renderer);
            CenterEditor();
        }

        public void ChangeActiveEditor(Editors.IEditor editor)
        {
            AllowKeyboardToMoveConsole = true;

            if (ActiveEditor != null)
            {
                ActiveEditor.OnDeselected();
                borderConsole.IsVisible = false;
            }

            if (OpenEditors.Contains(editor))
            {
                ActiveEditor = editor;
                ActiveEditor.OnSelected();
                ToolsPane.RedrawPanels();

                borderConsole.IsVisible = true;
                borderConsole.SetContent(ActiveEditor.Surface, ActiveEditor.Renderer);

                //Consoles.Children.Insert(0, ActiveEditor.RenderedConsole);
                RefreshBorder();


                if (ToolsPane.PanelFiles.DocumentsListbox.SelectedItem != editor)
                    ToolsPane.PanelFiles.DocumentsListbox.SelectedItem = editor;
            }
        }

        public void SaveEditor()
        {
            if (ActiveEditor != null)
                ActiveEditor.Save();
        }

        public void CenterEditor()
        {
            Point position = new Point();

            //if (ActiveEditor.Width > InnerEmptyBounds.Width || ActiveEditor.Height > InnerEmptyBounds.Height)
            //{
            //    // Need scrolling console
            //    if (ActiveEditor.Width > InnerEmptyBounds.Width && ActiveEditor.Height > InnerEmptyBounds.Height)
            //    {
            //        position = InnerEmptyBounds.Location;
            //        ActiveEditor.RenderedConsole.TextSurface.RenderArea = new Rectangle(0, 0, InnerEmptyBounds.Width, InnerEmptyBounds.Height);
            //    }
            //    else if (ActiveEditor.Width > InnerEmptyBounds.Width)
            //    {
            //        position = new Point(InnerEmptyBounds.Location.X, (InnerEmptyBounds.Height + InnerEmptyBounds.Y - ActiveEditor.Height) / 2);
            //        ActiveEditor.RenderedConsole.TextSurface.RenderArea = new Rectangle(0, 0, InnerEmptyBounds.Width, ActiveEditor.Height);
            //    }
            //    else if (ActiveEditor.Height > InnerEmptyBounds.Height)
            //    {
            //        position = new Point((InnerEmptyBounds.Width + InnerEmptyBounds.X - ActiveEditor.Width) / 2, InnerEmptyBounds.Location.Y);
            //        ActiveEditor.RenderedConsole.TextSurface.RenderArea = new Rectangle(0, 0, ActiveEditor.Width, InnerEmptyBounds.Height);
            //    }
            //}
            //else
            //{
            //    // Center normal
            //    position = new Point((InnerEmptyBounds.Width + InnerEmptyBounds.X - ActiveEditor.Width) / 2, (InnerEmptyBounds.Height + InnerEmptyBounds.Y - ActiveEditor.Height) / 2);
            //}

            //if (position.X < InnerEmptyBounds.Left)
            //    position.X = InnerEmptyBounds.Left;

            //if (position.Y < InnerEmptyBounds.Top)
            //    position.Y = InnerEmptyBounds.Top;

            //ActiveEditor.Move(position.X, position.Y);

            borderConsole.Position = new Point((InnerEmptyBounds.Width / 2) - (borderConsole.Width / 2), (InnerEmptyBounds.Height / 2) - (borderConsole.Height / 2)) + InnerEmptyBounds.Location;
        }

        private void RefreshBackingPanel()
        {
            topBarPane.Clear();

            var text = new SadConsole.ColoredString("   X: ", Settings.Appearance_Text) + new SadConsole.ColoredString(topBarMousePosition.X.ToString(), Settings.Appearance_TextValue) +
                       new SadConsole.ColoredString(" Y: ", Settings.Appearance_Text) + new SadConsole.ColoredString(topBarMousePosition.Y.ToString(), Settings.Appearance_TextValue) +
                       new SadConsole.ColoredString("   Layer: ", Settings.Appearance_Text) + new SadConsole.ColoredString(topBarLayerName, Settings.Appearance_TextValue) +
                       new SadConsole.ColoredString("   Tool: ", Settings.Appearance_Text) + new SadConsole.ColoredString(topBarToolName, Settings.Appearance_TextValue);

            topBarPane.Print(0, 0, text);
        }


        private bool ProcessMouseForBrush(IConsole console, MouseConsoleState state)
        {
            // This is not currently used. It may be in the future.
            return false;
        }

        private bool BorderConsoleKeyboardHandler(IConsole console, Keyboard state)
        {
            if (UseKeyboard)
            {
                bool movekeyPressed = false;
                var position = new Point(borderConsole.Position.X + 1, borderConsole.Position.Y + 1);
                //var result = base.ProcessKeyboard(info);
                if (AllowKeyboardToMoveConsole && ActiveEditor != null && ActiveEditor.Surface != null)
                {
                    bool shifted = Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift);
                    var oldRenderArea = ActiveEditor.Surface.RenderArea;

                    if (!shifted && Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                        ActiveEditor.Surface.RenderArea = new Rectangle(ActiveEditor.Surface.RenderArea.Left - 1, ActiveEditor.Surface.RenderArea.Top, InnerEmptyBounds.Width, InnerEmptyBounds.Height);

                    else if (!shifted && Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                        ActiveEditor.Surface.RenderArea = new Rectangle(ActiveEditor.Surface.RenderArea.Left + 1, ActiveEditor.Surface.RenderArea.Top, InnerEmptyBounds.Width, InnerEmptyBounds.Height);

                    if (!shifted && Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                        ActiveEditor.Surface.RenderArea = new Rectangle(ActiveEditor.Surface.RenderArea.Left, ActiveEditor.Surface.RenderArea.Top - 1, InnerEmptyBounds.Width, InnerEmptyBounds.Height);

                    else if (!shifted && Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                        ActiveEditor.Surface.RenderArea = new Rectangle(ActiveEditor.Surface.RenderArea.Left, ActiveEditor.Surface.RenderArea.Top + 1, InnerEmptyBounds.Width, InnerEmptyBounds.Height);

                    movekeyPressed = oldRenderArea != ActiveEditor.Surface.RenderArea;

                }

                if (!movekeyPressed)
                {
                    //if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Subtract))
                    //{
                    //	SelectedEditor.Surface.ResizeCells(SelectedEditor.Surface.CellSize.X / 2, SelectedEditor.Surface.CellSize.Y / 2);
                    //}
                    //else if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Add))
                    //{
                    //	SelectedEditor.Surface.ResizeCells(SelectedEditor.Surface.CellSize.X * 2, SelectedEditor.Surface.CellSize.Y * 2);
                    //}
                    //else
                    {
                        // Look for tool hotkeys
                        if (ToolsPane.ProcessKeyboard(Global.KeyboardState))
                        {
                        }
                        // Look for quick select F* keys
                        else if (QuickSelectPane.ProcessKeyboard(Global.KeyboardState))
                        {
                        }
                        else if (ActiveEditor != null)
                        {
                            ActiveEditor.ProcessKeyboard(Global.KeyboardState);
                        }
                    }
                }
            }

            // Always return true so that the virtual cursor doesn't start working.
            return true;
        }
        

    }
}