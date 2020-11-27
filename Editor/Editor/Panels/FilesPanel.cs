using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

namespace SadConsoleEditor.Panels
{
    public class FilesPanel : CustomPanel
    {
        public Button NewButton;
        public Button LoadButton;
        public Button SaveButton;
        public Button SaveAsButton;
        public Button ResizeButton;
        public Button CloseButton;

        private DrawingSurface documentsTitle;
        public ListBox DocumentsListbox;

        internal FilesPanel()
        {
            Title = "File";

            NewButton = new Button(7)
            {
                Text = "New",
                UseKeyboard = false,
            };
            NewButton.Click += (o, e) => MainConsole.Instance.ShowNewEditorPopup();

            LoadButton = new Button(8)
            {
                Text = "Load",
            };
            LoadButton.Click += (o, e) => MainConsole.Instance.ShowLoadEditorPopup();

            SaveButton = new Button(8)
            {
                Text = "Save",
            };
            SaveButton.Click += (o, e) => MainConsole.Instance.SaveEditor();

            SaveAsButton = new Button(10)
            {
                Text = "Save As",
            };
            SaveAsButton.Click += (o, e) => MainConsole.Instance.SaveAsEditor();


            ResizeButton = new Button(10)
            {
                Text = "Resize",
            };
            ResizeButton.Click += (o, e) => MainConsole.Instance.ShowResizeEditorPopup();

            CloseButton = new Button(9)
            {
                Text = "Close",
            };
            CloseButton.Click += (o, e) => MainConsole.Instance.ShowCloseConsolePopup();

            DocumentsListbox = new ListBox(Consoles.ToolPane.PanelWidthControls, 6, new EditorListBoxItem());
            DocumentsListbox.CompareByReference = true;

            DocumentsListbox.SelectedItemChanged += DocumentsListbox_SelectedItemChanged;

            documentsTitle = new DrawingSurface(13, 1);
            documentsTitle.OnDraw = (control, ts) =>
            {
                var colors = control.FindThemeColors();
                control.Surface.Fill(colors.Green, colors.ControlHostBackground, 0, null);
                control.Surface.Print(0, 0, new ColoredString("Opened Files", colors.Green, colors.ControlHostBackground));
            };
            Controls = new ControlBase[] { NewButton, SaveButton, ResizeButton, LoadButton, SaveAsButton, CloseButton, documentsTitle, DocumentsListbox };
            

        }

        private void DocumentsListbox_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
        {
            if (e.Item != null)
            {
                var editor = (Editors.IEditor)e.Item;

                if (MainConsole.Instance.ActiveEditor != editor)
                    MainConsole.Instance.ChangeActiveEditor(editor);
            }
        }

        public override void ProcessMouse(SadConsole.Input.MouseScreenObjectState info) { }

        public override void Loaded() { }

        public override int Redraw(SadConsole.UI.Controls.ControlBase control)
        {
            if (control == NewButton)
            {
                NewButton.Position = new Point(1, NewButton.Position.Y);
                return -1;
            }
            else if (control == SaveButton)
            {
                SaveButton.Position = new Point(NewButton.Bounds.MaxExtentX + 2, NewButton.Position.Y);
                return -1;
            }
            else if (control == ResizeButton)
            {
                ResizeButton.Position = new Point(SaveButton.Bounds.MaxExtentX + 3, NewButton.Position.Y);
                return 0;
            }

            if (control == LoadButton)
            {
                LoadButton.Position = new Point(1, LoadButton.Position.Y);
                return -1;
            }
            else if (control == SaveAsButton)
            {
                SaveAsButton.Position = new Point(SaveButton.Bounds.X, LoadButton.Position.Y);
                return -1;
            }
            else if (control == CloseButton)
            {
                CloseButton.Position = new Point(ResizeButton.Bounds.X, LoadButton.Position.Y);
                return 1;
            }


            return 0;
        }

        public class EditorListBoxItem : ListBoxItemTheme
        {
            public override void Draw(ListBox control, Rectangle area, object item, ControlStates itemState)
            {
                var look = GetStateAppearance(itemState);
                string value = ((Editors.IEditor)item).Metadata.Title??((Editors.IEditor)item).Metadata.Id;
                if (value.Length < area.Width)
                    value += new string(' ', area.Width - value.Length);
                else if (value.Length > area.Width)
                    value = value.Substring(0, area.Width);
                control.Surface.Print(area.X, area.Y, value, look);
            }
        }
    }
}
