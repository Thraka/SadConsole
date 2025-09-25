using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Examples;

internal class DemoTheDraw : IDemo
{
    public string Title => "TheDraw Fonts";

    public string Description => "Example that demonstrates rendering TheDraw text.";

    public string CodeFile => "DemoTheDraw.cs";

    public IScreenSurface CreateDemoScreen() =>
        new TheDrawConsole();

    public override string ToString() =>
        Title;
}

class TheDrawConsole : ControlsConsole
{
    Readers.TheDrawFont _selectedFont;
    Rectangle _drawArea;
    TextBox _writeTextbox;

    public TheDrawConsole() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Label label = new Label("Type here: ")
        {
            TextColor = Controls.GetThemeColors().Title,
            Position = (0, 1)
        };
        Controls.Add(label);

        Button selectFont = new Button(11, 1);
        selectFont.Position = (Width - selectFont.Width - 1, 1);
        selectFont.Text = "Font...";
        selectFont.Click += SelectFont_Click;
        Controls.Add(selectFont);

        _writeTextbox = new TextBox(selectFont.Position.X - label.Bounds.MaxExtentX - 2)
        {
            Position = (label.Bounds.MaxExtentX + 1, label.Position.Y)
        };
        Controls.Add(_writeTextbox);

        _drawArea = new Rectangle(0, 2, Width, Height - 2);

        var fonts = SadConsole.Readers.TheDrawFont.ReadFonts("./Res/TheDraw/TDFONTS0.TDF").ToArray();
        _selectedFont = fonts[0];

        _writeTextbox.TextChanged += Box_TextChanged;
        _writeTextbox.TextChangedPreview += Box_EditingTextChanged;
        //_writeTextbox.EditModeExit += Box_TextChanged;

    }

    private void SelectFont_Click(object sender, EventArgs e)
    {
        TheDrawWindow window = new();
        window.Center();
        window.Closed += (s2, e2) =>
        {
            if (window.DialogResult)
            {
                _selectedFont = window.SelectedFont;
                Box_TextChanged(_writeTextbox, EventArgs.Empty);
            }
        };
        window.Show(true);
    }

    private void Box_EditingTextChanged(object sender, EventArgs e)
    {
        if (!_writeTextbox.DisableKeyboard)
        {
            Surface.Clear(_drawArea);
            var text = ((TextBox)sender).Text;
            Surface.PrintTheDraw(0, 4, text, _selectedFont);
            //PrintTheDrawString(((SadConsole.UI.Controls.TextBox)sender).EditingText);
        }
    }

    private void Box_TextChanged(object sender, EventArgs e)
    {
        Surface.Clear(_drawArea);
        var text = ((TextBox)sender).Text;
        Surface.PrintTheDraw(0, 4, text, _selectedFont);
        //PrintTheDrawString(((SadConsole.UI.Controls.TextBox)sender).Text);
    }

    private void PrintTheDrawString(string text)
    {
        int xPos = 0;
        int yPos = 4;
        int tempHeight = 0;

        foreach (var item in text.ToAscii())
        {
            if (_selectedFont.IsCharacterSupported(item))
            {
                var charInfo = _selectedFont.GetCharacter(item);

                if (xPos + charInfo.Width >= Width)
                {
                    yPos += tempHeight + 1;
                    xPos = 0;
                }

                if (yPos >= Height)
                    break;

                var surfaceCharacter = _selectedFont.GetSurface(item);
                surfaceCharacter.Copy(Surface, xPos, yPos);

                if (surfaceCharacter.Height > tempHeight)
                    tempHeight = surfaceCharacter.Height;

                xPos += charInfo.Width;
            }
            else if (item == ' ')
            {
                // If the space character isn't supported, try to use some others
                if (_selectedFont.IsCharacterSupported('i'))
                    xPos += _selectedFont.GetCharacter('i').Width;
                else if (_selectedFont.IsCharacterSupported('1'))
                    xPos += _selectedFont.GetCharacter('1').Width;
                else if (_selectedFont.IsCharacterSupported('a'))
                    xPos += _selectedFont.GetCharacter('a').Width;
            }
        }
    }

    class TheDrawWindow : Window
    {
        private ListBox _childFontsListbox;
        private Readers.TheDrawFont[] _selectedFonts;

        public Readers.TheDrawFont SelectedFont { get; private set; }

        public TheDrawWindow() : base(40, 20)
        {
            Title = "Select TheDraw Font";
            Colors colors = Controls.GetThemeColors();

            Cursor.PrintAppearanceMatchesHost = false;
            Cursor.DisableWordBreak = true;
            Cursor.SetPrintAppearance(colors.Title, Surface.DefaultBackground);

            FileDirectoryListbox fileListBox = new FileDirectoryListbox(18, Height - 5);
            fileListBox.FileFilter = "*.tdf";
            fileListBox.OnlyRootAndSubDirs = true;
            fileListBox.HideNonFilterFiles = true;
            fileListBox.CurrentFolder = "./Res/TheDraw/";
            fileListBox.Position = (1, 2);
            //((SadConsole.UI.Themes.ListBoxTheme)fileListBox.Theme).DrawBorder = true;
            Controls.Add(fileListBox);

            Surface.DrawLine((fileListBox.Bounds.MaxExtentX + 1, 1),
                             (fileListBox.Bounds.MaxExtentX + 1, Height - 2),
                             ICellSurface.ConnectedLineThin[3], colors.Lines);

            Surface.ConnectLines(ICellSurface.ConnectedLineThin);

            Cursor.Move(fileListBox.Position.X, fileListBox.Position.Y - 1)
                  .Print("Files")
                  .Move(fileListBox.Bounds.MaxExtentX + 2, fileListBox.Position.Y - 1)
                  .Print("Fonts in file");

            _childFontsListbox = new ListBox(Width - fileListBox.Bounds.MaxExtentX - 4, fileListBox.Height)
            {
                Position = (fileListBox.Bounds.MaxExtentX + 2, fileListBox.Position.Y)
            };
            _childFontsListbox.SelectedItemChanged += _childFontsListbox_SelectedItemChanged;
            Controls.Add(_childFontsListbox);

            fileListBox.SelectedItemChanged += FileListBox_SelectedItemChanged;
            fileListBox.SelectedIndex = 0;

            Button cancelButton = new Button(10, 1)
            {
                Text = "Cancel",
                Position = (1, Height - 2)
            };
            cancelButton.Click += (s, e) => { DialogResult = false; Hide(); };
            Controls.Add(cancelButton);

            Button okButton = new Button(6, 1)
            {
                Text = "OK",
                Position = (Width - 1 - 7, Height - 2)
            };
            okButton.Click += (s, e) => { DialogResult = true; Hide(); };
            Controls.Add(okButton);
        }

        private void _childFontsListbox_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
        {
            if (e.Item == null)
                SelectedFont = null;
            else
                SelectedFont = (Readers.TheDrawFont)((ValueTuple<string, object>)e.Item).Item2;
        }

        private void FileListBox_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
        {
            _selectedFonts = SadConsole.Readers.TheDrawFont.ReadFonts(e.Item.ToString()).ToArray();
            PrintFontInformation();
        }

        private void PrintFontInformation()
        {
            _childFontsListbox.Items.Clear();

            foreach (var item in _selectedFonts)
                _childFontsListbox.Items.Add((item.Title, (object)item));

            _childFontsListbox.SelectedIndex = 0;
        }
    }
}
