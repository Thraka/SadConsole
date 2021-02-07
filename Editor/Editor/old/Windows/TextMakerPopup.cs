using Microsoft.Xna.Framework;
using SadConsole.Controls;
using SadConsole.Surfaces;
using SadConsoleEditor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsoleEditor.Windows
{
    class TextMakerPopup : SadConsole.Window
    {
        DrawingSurface previewPane;
        InputBox textInput;
        Button okButton;
        Button cancelButton;
        ListBox<FontListBoxItem> fontsListbox;
        CheckBox useSpacingCheckbox;

        List<TheDraw.Font> fonts;
        TheDraw.Font selectedFont;

        Point selectedFontTitlePosition;
        Point availableCharsPosition;

        public bool UseTransparentBackground;
        public NoDrawSurface SurfaceResult;

        private bool useSpacing { get { return useSpacingCheckbox.IsSelected; } }

        public TextMakerPopup() : base(Settings.Config.TextMakerSettings.WindowWidth, Settings.Config.TextMakerSettings.WindowHeight)
        {
            Center();
            Title = "Text Maker";

            fontsListbox = new ListBox<FontListBoxItem>(15, Height - 8);
            fontsListbox.Position = new Point(2, 4);
            fontsListbox.HideBorder = true;
            fontsListbox.SelectedItemChanged += FontsListbox_SelectedItemChanged;

            Print(fontsListbox.Bounds.Left, fontsListbox.Bounds.Top - 2, "Fonts", Settings.Color_TitleText);
            Print(fontsListbox.Bounds.Left, fontsListbox.Bounds.Top - 1, new string((char)196, fontsListbox.Width));

            Print(fontsListbox.Bounds.Right + 1, fontsListbox.Bounds.Top - 2, "Selected font: ", Settings.Color_TitleText);
            selectedFontTitlePosition = new Point(fontsListbox.Bounds.Right + 16, fontsListbox.Bounds.Top - 2);

            Print(fontsListbox.Bounds.Right + 1, fontsListbox.Bounds.Top - 1, "Available characters: ", Settings.Color_TitleText);
            availableCharsPosition = new Point(fontsListbox.Bounds.Right + 23, fontsListbox.Bounds.Top - 1);

            Print(fontsListbox.Bounds.Right + 1, fontsListbox.Bounds.Top + 1, "Text: ", Settings.Color_TitleText);

            textInput = new InputBox(Width - fontsListbox.Bounds.Right - 15);
            textInput.Position = new Point(fontsListbox.Bounds.Right + 7, fontsListbox.Bounds.Top + 1);
            textInput.TextChanged += TextInput_TextChanged;

            Print(fontsListbox.Bounds.Right + 1, fontsListbox.Bounds.Top + 3, "Preview", Settings.Color_TitleText);

            previewPane = new DrawingSurface(Width - fontsListbox.Bounds.Right - 3, Height - fontsListbox.Bounds.Top - 8);
            previewPane.Fill(Color.Black, Color.White, 0, SpriteEffects.FlipHorizontally);
            previewPane.Position = new Point(fontsListbox.Bounds.Right + 1, fontsListbox.Bounds.Top + 4);

            okButton = new Button(8);
            okButton.Position = new Point(textSurface.Width - okButton.Width - 2, textSurface.Height - 3);
            okButton.Click += (o, e) => { DialogResult = true; BuildFinalResult(); Hide(); };
            okButton.Text = "Ok";

            cancelButton = new Button(8);
            cancelButton.Position = new Point(2, textSurface.Height - 3);
            cancelButton.Text = "Cancel";
            cancelButton.Click += (o, e) => { DialogResult = false; Hide(); };


            useSpacingCheckbox = new CheckBox(16, 1);
            useSpacingCheckbox.Text = "Use Spacing";
            useSpacingCheckbox.Position = new Point(cancelButton.Bounds.Right + 2, cancelButton.Position.Y);
            useSpacingCheckbox.IsSelectedChanged += (o, e) => DrawText();


            Add(useSpacingCheckbox);
            Add(textInput);
            Add(previewPane);
            Add(fontsListbox);
            Add(okButton);
            Add(cancelButton);

            fonts = new List<TheDraw.Font>();

            foreach (var file in System.IO.Directory.GetFiles("TheDraw"))
                fonts.AddRange(TheDraw.Font.ReadFonts(file));

            foreach (var font in fonts)
                fontsListbox.Items.Add(font);

            fontsListbox.SelectedItem = fontsListbox.Items[0];
            textInput.Text = "Example!";
        }

        private void BuildFinalResult()
        {
            int x = 0;
            int tempHeight = 0;
            int height = 0;
            int width = 0;

            List<TheDraw.Character> characters = new List<TheDraw.Character>(textInput.Text.Length);

            foreach (var item in textInput.Text)
            {
                if (selectedFont.IsCharacterSupported(item))
                {
                    var character = selectedFont.GetCharacter(item);
                    characters.Add(character);
                    width += character.Width + (useSpacing ? selectedFont.LetterSpacing : 0);

                    tempHeight = character.Rows.Length;

                    if (tempHeight > height)
                        height = tempHeight;
                }
            }

            SurfaceResult = new NoDrawSurface(width - 1, height);

            foreach (var item in characters)
            {
                selectedFont.GetSurface(item.GlyphIndex).Copy(SurfaceResult, x, 0);
                x += item.Width + (useSpacing ? selectedFont.LetterSpacing : 0);
            }
        }

        private void TextInput_TextChanged(object sender, EventArgs e)
        {
            DrawText();
        }

        private void FontsListbox_SelectedItemChanged(object sender, ListBox<FontListBoxItem>.SelectedItemEventArgs e)
        {
            selectedFont = (TheDraw.Font)e.Item;

            PrintValidCharacters();
        }

        private void DrawText()
        {
            previewPane.Fill(Color.White, Color.Black, 0, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
            string clearString = new string(' ', Width - 1 - selectedFontTitlePosition.X);
            Print(selectedFontTitlePosition.X, selectedFontTitlePosition.Y, clearString);
            Print(selectedFontTitlePosition.X, selectedFontTitlePosition.Y, selectedFont.Title);

            int x = 0;
            int y = 0;
            int highestHeight = 0;
            foreach (var item in textInput.Text)
            {
                var result = selectedFont.GetSurface(item);

                if (result != null)
                {
                    if (x + result.Width > previewPane.Width)
                    {
                        y += highestHeight;
                        highestHeight = 0;
                        x = 0;
                    }

                    if (y + result.Height > previewPane.Height)
                    {
                        continue;
                    }

                    result.Copy(previewPane.TextSurface, x, y);
                    x += result.Width + (useSpacing ? selectedFont.LetterSpacing : 0);

                    if (result.Height > highestHeight)
                        highestHeight = result.Height;
                }

            }
        }

        private void PrintValidCharacters()
        {
            if (selectedFont != null)
            {
                for (int i = 0; i < 47; i++)
                {
                    SetGlyph(availableCharsPosition.X + i, availableCharsPosition.Y, 33 + i);
                    SetForeground(availableCharsPosition.X + i, availableCharsPosition.Y, selectedFont.CharactersSupported[i] ? Color.Green : Settings.Color_Text);
                    SetGlyph(availableCharsPosition.X + i, availableCharsPosition.Y + 1, 33 + 47 + i);
                    SetForeground(availableCharsPosition.X + i, availableCharsPosition.Y + 1, selectedFont.CharactersSupported[i + 47] ? Color.Green : Settings.Color_Text);
                }

                DrawText();
            }
        }
    }

    class FontListBoxItem : ListBoxItem
    {
        public override void Draw(SurfaceBase surface, Rectangle area)
        {
            string value = ((TheDraw.Font)Item).Title;
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
