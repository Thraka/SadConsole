using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole.UI.Controls;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.UI.Controls;

namespace SadConsole.UI.Themes
{
    public class FileDirectoryListboxItem : ListBoxItemTheme
    {
        private ColoredGlyph _directoryAppNormal = new ColoredGlyph(Color.Purple, Color.Black);
        private ColoredGlyph _directoryAppMouseOver = new ColoredGlyph(Color.Purple, Color.Black);
        private ColoredGlyph _directoryAppSelected = new ColoredGlyph(new Color(255, 0, 255), Color.Black);
        private ColoredGlyph _directoryAppSelectedOver = new ColoredGlyph(new Color(255, 0, 255), Color.Black);
        private ColoredGlyph _fileAppNormal = new ColoredGlyph(Color.Gray, Color.Black);
        private ColoredGlyph _fileAppMouseOver = new ColoredGlyph(Color.Gray, Color.Black);
        private ColoredGlyph _fileAppSelected = new ColoredGlyph(Color.White, Color.Black);
        private ColoredGlyph _fileAppSelectedOver = new ColoredGlyph(Color.White, Color.Black);
        private ColoredGlyph _highExtAppNormal = new ColoredGlyph(Color.AnsiYellow, Color.Black);
        private ColoredGlyph _highExtAppMouseOver = new ColoredGlyph(Color.AnsiYellow, Color.Black);
        private ColoredGlyph _highExtAppSelected = new ColoredGlyph(Color.Yellow, Color.Black);
        private ColoredGlyph _highExtAppSelectedOver = new ColoredGlyph(Color.Yellow, Color.Black);

        public override void RefreshTheme(Colors themeColors)
        {
            base.RefreshTheme(themeColors);

            _directoryAppNormal.Background = themeColors.ControlBackgroundNormal;
            _directoryAppMouseOver.Background = themeColors.ControlBackgroundMouseOver;
            _directoryAppSelected.Background = themeColors.ControlBackgroundSelected;
            _directoryAppSelectedOver.Background = themeColors.ControlBackgroundMouseOver;

            _fileAppNormal.Background = themeColors.ControlBackgroundNormal;
            _fileAppMouseOver.Background = themeColors.ControlBackgroundMouseOver;
            _fileAppSelected.Background = themeColors.ControlBackgroundSelected;
            _fileAppSelectedOver.Background = themeColors.ControlBackgroundMouseOver;

            _highExtAppNormal.Background = themeColors.ControlBackgroundNormal;
            _highExtAppMouseOver.Background = themeColors.ControlBackgroundMouseOver;
            _highExtAppSelected.Background = themeColors.ControlBackgroundSelected;
            _highExtAppSelectedOver.Background = themeColors.ControlBackgroundMouseOver;
        }


        public override void Draw(ListBox control, Rectangle area, object item, ControlStates itemState)
        {
            ColoredGlyph appearance;
            string displayString;

            if (item is System.IO.DirectoryInfo || item is FileDirectoryListbox.FauxDirectory)
            {
                if (item is System.IO.DirectoryInfo info)
                    displayString = "<" + info.Name + ">";
                else
                    displayString = "<" + ((FileDirectoryListbox.FauxDirectory)item).Name + ">";

                if (itemState.HasFlag(ControlStates.MouseOver) && itemState.HasFlag(ControlStates.Selected))
                    appearance = _directoryAppSelectedOver;
                else if (itemState.HasFlag(ControlStates.MouseOver))
                    appearance = _directoryAppMouseOver;
                else if (itemState.HasFlag(ControlStates.Selected))
                    appearance = _directoryAppSelected;
                else
                    appearance = _directoryAppNormal;
            }
            else if (item is System.IO.FileInfo info)
            {
                displayString = info.Name;

                if (itemState.HasFlag(ControlStates.MouseOver) && itemState.HasFlag(ControlStates.Selected))
                    appearance = _fileAppSelectedOver;
                else if (itemState.HasFlag(ControlStates.MouseOver))
                    appearance = _fileAppMouseOver;
                else if (itemState.HasFlag(ControlStates.Selected))
                    appearance = _fileAppSelected;
                else
                    appearance = _fileAppNormal;
            }
            else if (item is FileDirectoryListbox.HighlightedExtFile extInfo)
            {
                displayString = extInfo.Name;

                if (itemState.HasFlag(ControlStates.MouseOver) && itemState.HasFlag(ControlStates.Selected))
                    appearance = _highExtAppSelectedOver;
                else if (itemState.HasFlag(ControlStates.MouseOver))
                    appearance = _highExtAppMouseOver;
                else if (itemState.HasFlag(ControlStates.Selected))
                    appearance = _highExtAppSelected;
                else
                    appearance = _highExtAppNormal;
            }
            else
            {
                appearance = GetStateAppearance(itemState);
                displayString = item.ToString();
            }

            control.Surface.Print(area.X, area.Y, displayString.Align(SadConsole.HorizontalAlignment.Left, area.Width), appearance);
        }

    }
}
