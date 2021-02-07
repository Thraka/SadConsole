using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Panels
{
    class FilesPanel : CustomPanel
    {
        public Button NewButton;
        public Button LoadButton;
        public Button SaveButton;
        public Button ResizeButton;
        public Button CloseButton;

        private DrawingSurface documentsTitle;
        public ListBox<EditorListBoxItem> DocumentsListbox;

        public FilesPanel()
        {
            Title = "File";

            NewButton = new Button(7)
            {
                Text = "New",
                UseKeyboard = false,
            };
            NewButton.Click += (o, e) => MainScreen.Instance.ShowNewEditorPopup();

            LoadButton = new Button(8)
            {
                Text = "Load",
            };
            LoadButton.Click += (o, e) => MainScreen.Instance.ShowLoadEditorPopup();

            SaveButton = new Button(8)
            {
                Text = "Save",
            };
            SaveButton.Click += (o, e) => MainScreen.Instance.SaveEditor();

            ResizeButton = new Button(10)
            {
                Text = "Resize",
            };
            ResizeButton.Click += (o, e) => MainScreen.Instance.ShowResizeEditorPopup();

            CloseButton = new Button(9)
            {
                Text = "Close",
            };
            CloseButton.Click += (o, e) => MainScreen.Instance.ShowCloseConsolePopup();

            DocumentsListbox = new ListBox<EditorListBoxItem>(Consoles.ToolPane.PanelWidthControls, 6);
            DocumentsListbox.HideBorder = true;
            DocumentsListbox.CompareByReference = true;

            DocumentsListbox.SelectedItemChanged += DocumentsListbox_SelectedItemChanged;

            documentsTitle = new DrawingSurface(13, 1);
            documentsTitle.Fill(Settings.Green, Settings.Color_MenuBack, 0, null);
            documentsTitle.Print(0, 0, new ColoredString("Opened Files", Settings.Green, Settings.Color_MenuBack));

            Controls = new ControlBase[] { NewButton, LoadButton, SaveButton, ResizeButton, CloseButton, documentsTitle, DocumentsListbox };
            

        }

        private void DocumentsListbox_SelectedItemChanged(object sender, ListBox<EditorListBoxItem>.SelectedItemEventArgs e)
        {
            if (e.Item != null)
            {
                var editor = (Editors.IEditor)e.Item;
                //MainScreen.Instance.Instance.ChangeEditor((Editors.IEditor)e.Item);
                if (MainScreen.Instance.ActiveEditor != editor)
                    MainScreen.Instance.ChangeActiveEditor(editor);
            }
        }

        public override void ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {
            
        }

        public override int Redraw(SadConsole.Controls.ControlBase control)
        {
            if (control == NewButton)
                NewButton.Position = new Point(1, NewButton.Position.Y);
            else if (control == LoadButton)
            {
                LoadButton.Position = new Point(NewButton.Bounds.Right + 2, NewButton.Position.Y);
            }
            else if (control == SaveButton)
                SaveButton.Position = new Point(1, SaveButton.Position.Y);
            else if (control == ResizeButton)
            {
                ResizeButton.Position = new Point(SaveButton.Bounds.Right + 2, SaveButton.Position.Y);
                return -1;
            }
            else if (control == CloseButton)
            {
                CloseButton.Position = new Point(ResizeButton.Bounds.Right + 2, SaveButton.Position.Y);
            }
            else if (control == documentsTitle)
                return 1;

            return 0;
        }

        public override void Loaded()
        {
            
        }

        public class EditorListBoxItem : ListBoxItem
        {
            public override void Draw(SurfaceBase surface, Rectangle area)
            {
                string value = ((Editors.IEditor)Item).Title??((Editors.IEditor)Item).EditorTypeName;
                if (value.Length < area.Width)
                    value += new string(' ', area.Width - value.Length);
                else if (value.Length > area.Width)
                    value = value.Substring(0, area.Width);
                var editor = new SurfaceEditor(surface);
                editor.Print(area.Left, area.Top, value, _currentAppearance);
                _isDirty = false;
            }
        }
    }
}
