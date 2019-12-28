namespace SadConsoleEditor.Tools
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Input;
    using Panels;
    using System.Collections.Generic;
    using System.Linq;
    using SadConsole.Surfaces;
    using SadConsole.Entities;
    using Settings = SadConsoleEditor.Settings;

    class HotspotTool : ITool
    {
        public const string ID = "HOTSPOT";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Place/Remove Hotspots"; }
        }
        public char Hotkey { get { return 'h'; } }

        public CustomPanel[] ControlPanels { get; private set; }

        public Entity Brush;
        //private DisplayObjectToolPanel _mouseOverObjectPanel;

		private Entity _currentEntity;

        public HotspotTool()
        {
			//_mouseOverObjectPanel = new DisplayObjectToolPanel("Mouse Object");

            //ControlPanels = new CustomPanel[] { _panel, _mouseOverObjectPanel };
            ControlPanels = new CustomPanel[] { };
        }

        public override string ToString()
        {
            return Title;
        }

        public void OnSelected()
        {
            Brush = new Entity(1, 1, Settings.Config.ScreenFont);
            Brush.Animation.CreateFrame();
            Brush.IsVisible = false;
            RefreshTool();
            MainScreen.Instance.Brush = Brush;

            ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).ShowDarkLayer = true;
            ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).HighlightType = Editors.SceneEditor.HighlightTypes.HotSpot;
        }


        public void OnDeselected()
        {
            ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).ShowDarkLayer = false;
        }

        public void RefreshTool()
        {
            Settings.QuickEditor.TextSurface = Brush.Animation.CurrentFrame;
            var editor = MainScreen.Instance.ActiveEditor as Editors.SceneEditor;

            if (editor != null)
            {
                if (editor.HotspotPanel.SelectedObject != null)
                {
                    Settings.QuickEditor.SetCell(0, 0, editor.HotspotPanel.SelectedObject.DebugAppearance);
                    Settings.QuickEditor.SetGlyph(0, 0, editor.HotspotPanel.SelectedObject.DebugAppearance.Glyph);
                }
                else
                {
                    Settings.QuickEditor.SetGlyph(0, 0, 0, Color.White, Color.Transparent);
                }
            }
        }


        public void Update()
        {
        }

        public bool ProcessKeyboard(Keyboard info, SurfaceBase surface)
        {
            return false;
        }

        public void ProcessMouse(MouseConsoleState info, SurfaceBase surface, bool isInBounds)
        {
            RefreshTool();

            if (MainScreen.Instance.ActiveEditor is Editors.SceneEditor && info.IsOnConsole)
            {
                var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;
                var point = new Point(info.CellPosition.X, info.CellPosition.Y);
                Hotspot mouseSpot = null;

                foreach (var spot in editor.Hotspots)
                {
                    // Spot under our mouse
                    if (spot.Contains(point))
                    {
                        mouseSpot = spot;
                        break;
                    }
                }

                if (info.Mouse.RightClicked)
                {
                    // Suck up the object
                    if (mouseSpot != null)
                    {
                        editor.HotspotPanel.SelectedObject = mouseSpot;
                        RefreshTool();
                    }
                }

                // Stamp object
                else if (info.Mouse.LeftButtonDown)
                {
                    
                    // SHIFT+CTRL -- Delete any hotspot here
                    if ((Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
                     && (Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl)))
                    {
                        foreach (var spots in editor.Hotspots)
                        {
                            spots.Positions.Remove(info.CellPosition);
                        }
                    }

                    // SHIFT -- Delete only the select type
                    else if (Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
                    {
                        if (mouseSpot != null && mouseSpot == editor.HotspotPanel.SelectedObject)
                            mouseSpot.Positions.Remove(info.CellPosition);
                    }

                    

                    // Normal -- Place
                    else
                    {
                        if (mouseSpot != null)
                        {
                            // Remove the spot that exists here
                            foreach (var spots in editor.Hotspots)
                            {
                                spots.Positions.Remove(info.CellPosition);
                            }
                        }

                        // Place
                        if (editor.HotspotPanel.SelectedObject != null && !editor.HotspotPanel.SelectedObject.Contains(info.CellPosition))
                        {
                            editor.HotspotPanel.SelectedObject.Positions.Add(info.CellPosition);
                        }
                    }
                }
                
                // Display info about object
                else if (mouseSpot != null)
                {
                    //_mouseOverObjectPanel.DisplayedObject = editor.SelectedEntitys[point];
                    //MainScreen.Instance.Instance.ToolPane.RefreshControls();
                }
                else
                {
                    //_mouseOverObjectPanel.DisplayedObject = null;
                    //MainScreen.Instance.Instance.ToolPane.RefreshControls();
                }
            }

        }
    }
}
