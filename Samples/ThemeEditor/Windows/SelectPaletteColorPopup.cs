using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace ThemeEditor.Windows
{
    class SelectPaletteColorPopup : Window
    {
        ListBox colorItemsListBox;

        public Colors.ColorNames SelectedColor { get; private set; }

        public SelectPaletteColorPopup(Colors.ColorNames color) : base(19, 20)
        {
            //Border.AddToWindow(this);
            Title = "Select color";
            CloseOnEscKey = true;
            CanDrag = false;

            colorItemsListBox = new ListBox(Width - 2, Height - 4);
            colorItemsListBox.Position = (1, 1);
            colorItemsListBox.ItemTheme = new ListBoxItemColorTheme();
            Controls.Add(colorItemsListBox);

            colorItemsListBox.Items.Add((Container.EditingColors.White, "White"));
            colorItemsListBox.Items.Add((Container.EditingColors.Black, "Black"));
            colorItemsListBox.Items.Add((Container.EditingColors.Gray, "Gray"));
            colorItemsListBox.Items.Add((Container.EditingColors.GrayDark, "GrayDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Red, "Red"));
            colorItemsListBox.Items.Add((Container.EditingColors.RedDark, "RedDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Green, "Green"));
            colorItemsListBox.Items.Add((Container.EditingColors.GreenDark, "GreenDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Cyan, "Cyan"));
            colorItemsListBox.Items.Add((Container.EditingColors.CyanDark, "CyanDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Blue, "Blue"));
            colorItemsListBox.Items.Add((Container.EditingColors.BlueDark, "BlueDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Purple, "Purple"));
            colorItemsListBox.Items.Add((Container.EditingColors.PurpleDark, "PurpleDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Yellow, "Yellow"));
            colorItemsListBox.Items.Add((Container.EditingColors.YellowDark, "YellowDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Orange, "Orange"));
            colorItemsListBox.Items.Add((Container.EditingColors.OrangeDark, "OrangeDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Brown, "Brown"));
            colorItemsListBox.Items.Add((Container.EditingColors.BrownDark, "BrownDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Gold, "Gold"));
            colorItemsListBox.Items.Add((Container.EditingColors.GoldDark, "GoldDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Silver, "Silver"));
            colorItemsListBox.Items.Add((Container.EditingColors.SilverDark, "SilverDark"));
            colorItemsListBox.Items.Add((Container.EditingColors.Bronze, "Bronze"));
            colorItemsListBox.Items.Add((Container.EditingColors.BronzeDark, "BronzeDark"));

            colorItemsListBox.SelectedItemChanged += (s, e) =>
            {
                if (Container.EditingColors.TryToColorName((((Color itemColor, string title))e.Item).itemColor, out var colorName))
                    SelectedColor = colorName;
            };

            SelectColor(Container.EditingColors.FromColorName(color));

            Button button = new Button(4, 1)
            {
                Text = "OK",
                Position = (Width - 5, Height - 2)
            };
            button.Click += (s, e) => { DialogResult = true; Hide(); };
            Controls.Add(button);

            button = new Button(8, 1)
            {
                Text = "Cancel",
                Position = (1, Height - 2)
            };
            button.Click += (s, e) => { DialogResult = false; Hide(); };
            Controls.Add(button);
        }

        private void SelectColor(Color color)
        {
            foreach ((Color itemColor, string title) item in colorItemsListBox.Items)
                if (item.itemColor == color)
                {
                    colorItemsListBox.SelectedItem = item;
                    colorItemsListBox.ScrollToSelectedItem();
                    return;
                }
        }

        protected override void DrawBorder()
        {
            base.DrawBorder();
            var colors = Controls.GetThemeColors();
            this.DrawLine(new Point(1, Height - 3), new Point(Width - 1, Height - 3), BorderLineStyle[0], colors.Lines);
            this.ConnectLines(BorderLineStyle);
        }
    }
}
