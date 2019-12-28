using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using SadConsole.Entities;
using SadConsole.Input;
using SadConsoleEditor.Panels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsoleEditor.Tools
{
    class EntityCenterTool : ITool
    {
        public const string ID = "CENTERANIM";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Set Anim. Center"; }
        }
        public char Hotkey { get { return 'r'; } }

        public CustomPanel[] ControlPanels { get; private set; }

        public Entity Brush;

        public override string ToString()
        {
            return Title;
        }

        public EntityCenterTool()
        {
        }

        public void OnSelected()
        {
            Brush = new SadConsole.GameHelpers.Entity(1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            Brush.Animation.DefaultBackground = Color.Black;
            Brush.Animation.DefaultForeground = Color.White;
            Brush.Animation.CreateFrame()[0].Glyph = 42;
            Brush.IsVisible = false;
            RefreshTool();
            MainScreen.Instance.Brush = Brush;

            var editor = MainScreen.Instance.ActiveEditor as Editors.EntityEditor;

            if (editor != null)
                editor.ShowCenterLayer = true;
        }

        public void OnDeselected()
        {
            var editor = MainScreen.Instance.ActiveEditor as Editors.EntityEditor;

            if (editor != null)
                editor.ShowCenterLayer = false;
        }

        public void RefreshTool()
        {
            //_brush.CurrentAnimation.Frames[0].Fill(MainScreen.Instance.Instance.ToolPane.CommonCharacterPickerPanel.SettingForeground, MainScreen.Instance.Instance.ToolPane.CommonCharacterPickerPanel.SettingBackground, 42, null);
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
            if (info.IsOnConsole && info.Mouse.LeftClicked)
            {
                var cell = surface.GetCell(info.ConsolePosition.X, info.ConsolePosition.Y);
                var editor = MainScreen.Instance.ActiveEditor as Editors.EntityEditor;

                if (editor != null)
                {
                    editor.SetAnimationCenter(new Point(info.ConsolePosition.X, info.ConsolePosition.Y));
                }
            }
        }
        
    }
}
