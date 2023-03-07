using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// Displays files and directories in various colors.
    /// </summary>
    public class FileDirectoryListboxItem : ListBoxItemTheme
    {
        /// <summary>
        /// The appearance of a directory in normal state.
        /// </summary>
        public ColoredGlyph DirectoryAppNormal { get; set; } = new ColoredGlyph(Color.Purple, Color.Black);

        /// <summary>
        /// The appearance of a directory when the mouse is over it.
        /// </summary>
        public ColoredGlyph DirectoryAppMouseOver { get; set; } = new ColoredGlyph(Color.Purple, Color.Black);

        /// <summary>
        /// The appearance of a directory when selected.
        /// </summary>
        public ColoredGlyph DirectoryAppSelected { get; set; } = new ColoredGlyph(new Color(255, 0, 255), Color.Black);

        /// <summary>
        /// The appearance of a directory when selected and the mouse is over it.
        /// </summary>
        public ColoredGlyph DirectoryAppSelectedOver { get; set; } = new ColoredGlyph(new Color(255, 0, 255), Color.Black);

        /// <summary>
        /// The appearance of a file in normal state.
        /// </summary>
        public ColoredGlyph FileAppNormal { get; set; } = new ColoredGlyph(Color.Gray, Color.Black);

        /// <summary>
        /// The appearance of a file when the mouse is over it.
        /// </summary>
        public ColoredGlyph FileAppMouseOver { get; set; } = new ColoredGlyph(Color.Gray, Color.Black);

        /// <summary>
        /// The appearance of a file when selected.
        /// </summary>
        public ColoredGlyph FileAppSelected { get; set; } = new ColoredGlyph(Color.White, Color.Black);

        /// <summary>
        /// The appearance of a file when selected and the mouse is over it.
        /// </summary>
        public ColoredGlyph FileAppSelectedOver { get; set; } = new ColoredGlyph(Color.White, Color.Black);

        /// <summary>
        /// The appearance of a highlighted file in normal state.
        /// </summary>
        public ColoredGlyph HighExtAppNormal { get; set; } = new ColoredGlyph(Color.AnsiYellow, Color.Black);

        /// <summary>
        /// The appearance of a highlighted file when the mouse is over it.
        /// </summary>
        public ColoredGlyph HighExtAppMouseOver { get; set; } = new ColoredGlyph(Color.AnsiYellow, Color.Black);

        /// <summary>
        /// The appearance of a highlighted file when selected.
        /// </summary>
        public ColoredGlyph HighExtAppSelected { get; set; } = new ColoredGlyph(Color.Yellow, Color.Black);

        /// <summary>
        /// The appearance of a highlighted file when selected and the mouse is over it.
        /// </summary>
        public ColoredGlyph HighExtAppSelectedOver { get; set; } = new ColoredGlyph(Color.Yellow, Color.Black);

        /// <inheritdoc/>
        public override void RefreshTheme(Colors themeColors)
        {
            base.RefreshTheme(themeColors);

            DirectoryAppNormal.Background = themeColors.ControlBackgroundNormal;
            DirectoryAppMouseOver.Background = themeColors.ControlBackgroundMouseOver;
            DirectoryAppSelected.Background = themeColors.ControlBackgroundSelected;
            DirectoryAppSelectedOver.Background = themeColors.ControlBackgroundMouseOver;

            FileAppNormal.Background = themeColors.ControlBackgroundNormal;
            FileAppMouseOver.Background = themeColors.ControlBackgroundMouseOver;
            FileAppSelected.Background = themeColors.ControlBackgroundSelected;
            FileAppSelectedOver.Background = themeColors.ControlBackgroundMouseOver;

            HighExtAppNormal.Background = themeColors.ControlBackgroundNormal;
            HighExtAppMouseOver.Background = themeColors.ControlBackgroundMouseOver;
            HighExtAppSelected.Background = themeColors.ControlBackgroundSelected;
            HighExtAppSelectedOver.Background = themeColors.ControlBackgroundMouseOver;
        }


        /// <inheritdoc/>
        public override void Draw(ControlBase control, Rectangle area, object item, ControlStates itemState)
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
                    appearance = DirectoryAppSelectedOver;
                else if (itemState.HasFlag(ControlStates.MouseOver))
                    appearance = DirectoryAppMouseOver;
                else if (itemState.HasFlag(ControlStates.Selected))
                    appearance = DirectoryAppSelected;
                else
                    appearance = DirectoryAppNormal;
            }
            else if (item is System.IO.FileInfo info)
            {
                displayString = info.Name;

                if (itemState.HasFlag(ControlStates.MouseOver) && itemState.HasFlag(ControlStates.Selected))
                    appearance = DirectoryAppSelectedOver;
                else if (itemState.HasFlag(ControlStates.MouseOver))
                    appearance = DirectoryAppMouseOver;
                else if (itemState.HasFlag(ControlStates.Selected))
                    appearance = DirectoryAppSelected;
                else
                    appearance = FileAppNormal;
            }
            else if (item is FileDirectoryListbox.HighlightedExtFile extInfo)
            {
                displayString = extInfo.Name;

                if (itemState.HasFlag(ControlStates.MouseOver) && itemState.HasFlag(ControlStates.Selected))
                    appearance = HighExtAppSelectedOver;
                else if (itemState.HasFlag(ControlStates.MouseOver))
                    appearance = HighExtAppMouseOver;
                else if (itemState.HasFlag(ControlStates.Selected))
                    appearance = HighExtAppSelected;
                else
                    appearance = HighExtAppNormal;
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
