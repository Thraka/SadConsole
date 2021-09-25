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

        public TheDrawConsole(): base (Program.MainWidth, Program.MainHeight)
        {
            SadConsole.UI.Controls.Label label = new SadConsole.UI.Controls.Label("Type here: ")
            {
                TextColor = Controls.GetThemeColors().Title,
                Position = (0, 1)
            };
            Controls.Add(label);

            SadConsole.UI.Controls.TextBox box = new SadConsole.UI.Controls.TextBox(Width - label.Bounds.MaxExtentX - 2)
            {
                Position = (label.Bounds.MaxExtentX + 1, label.Position.Y)
            };
            Controls.Add(box);

            _drawArea = new Rectangle(0, 2, Width, Height - 2);

            

            var fonts = SadConsole.Readers.TheDrawFont.ReadFonts("./TheDraw/TDFONTS0.TDF").ToArray();
            _selectedFont = fonts[0];

            box.TextChanged += Box_TextChanged;
            box.EditingTextChanged += Box_EditingTextChanged;
            box.EditModeExit += Box_TextChanged;

        }

        private void Box_EditingTextChanged(object sender, EventArgs e)
        {
            Surface.Clear(_drawArea);
            PrintTheDrawString(((SadConsole.UI.Controls.TextBox)sender).EditingText);
        }

        private void Box_TextChanged(object sender, EventArgs e)
        {
            Surface.Clear(_drawArea);
            PrintTheDrawString(((SadConsole.UI.Controls.TextBox)sender).Text);
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
                    surfaceCharacter.Copy(this, xPos, yPos);

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
