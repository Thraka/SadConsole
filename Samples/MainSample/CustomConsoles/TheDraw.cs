using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI;
using SadConsole;
using SadRogue.Primitives;
using System.Linq;

namespace FeatureDemo.CustomConsoles
{
    public class TheDrawConsole: ControlsConsole
    {
        SadConsole.Readers.TheDrawFont _selectedFont;
        Rectangle _drawArea;
        SadConsole.UI.Controls.TextBox _writeTextbox;

        public TheDrawConsole(): base (Program.MainWidth, Program.MainHeight)
        {
            SadConsole.UI.Controls.Label label = new SadConsole.UI.Controls.Label("Type here: ")
            {
                TextColor = Controls.GetThemeColors().Title,
                Position = (0, 1)
            };
            Controls.Add(label);

            SadConsole.UI.Controls.Button selectFont = new SadConsole.UI.Controls.Button(11, 1);
            selectFont.Position = (Width - selectFont.Width - 1, 1);
            selectFont.Text = "Font...";
            selectFont.Click += SelectFont_Click;
            Controls.Add(selectFont);

            _writeTextbox = new SadConsole.UI.Controls.TextBox(selectFont.Position.X - label.Bounds.MaxExtentX - 2)
            {
                Position = (label.Bounds.MaxExtentX + 1, label.Position.Y)
            };
            Controls.Add(_writeTextbox);

            _drawArea = new Rectangle(0, 2, Width, Height - 2);

            var fonts = SadConsole.Readers.TheDrawFont.ReadFonts("./TheDraw/TDFONTS0.TDF").ToArray();
            _selectedFont = fonts[0];

            _writeTextbox.TextChanged += Box_TextChanged;
            _writeTextbox.EditingTextChanged += Box_EditingTextChanged;
            _writeTextbox.EditModeExit += Box_TextChanged;

        }

        private void SelectFont_Click(object sender, EventArgs e)
        {
            Windows.TheDrawWindow window = new();
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
                var text = ((SadConsole.UI.Controls.TextBox)sender).EditingText;
                Surface.PrintTheDraw(0, 4, text, _selectedFont);
                //PrintTheDrawString(((SadConsole.UI.Controls.TextBox)sender).EditingText);
            }
        }

        private void Box_TextChanged(object sender, EventArgs e)
        {
            Surface.Clear(_drawArea);
            var text = ((SadConsole.UI.Controls.TextBox)sender).Text;
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
    }
}
