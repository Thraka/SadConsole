﻿using System;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI;

public partial class Window
{
    /// <summary>
    /// Shows a window prompt with two buttons for the user to click.
    /// </summary>
    /// <param name="message">The text to display.</param>
    /// <param name="yesPrompt">The yes button's text.</param>
    /// <param name="noPrompt">The no button's text.</param>
    /// <param name="resultCallback">Callback with the yes (true) or no (false) result.</param>
    /// <param name="colors">The colors to apply for the message box and buttons. If <see langword="null"/>, then the default <see cref="Library.Colors"/> will be used.</param>
    public static void Prompt(string message, string yesPrompt, string noPrompt, Action<bool> resultCallback, Colors? colors = null) =>
        Prompt(new ColoredString(message), yesPrompt, noPrompt, resultCallback, colors);

    /// <summary>
    /// Shows a window prompt with two buttons for the user to click.
    /// </summary>
    /// <param name="message">The text to display. (background color is ignored)</param>
    /// <param name="yesPrompt">The yes button's text.</param>
    /// <param name="noPrompt">The no button's text.</param>
    /// <param name="resultCallback">Callback with the yes (true) or no (false) result.</param>
    /// <param name="colors">The colors to apply for the message box and buttons. If <see langword="null"/>, then the default <see cref="Library.Colors"/> will be used.</param>
    public static void Prompt(ColoredString message, string yesPrompt, string noPrompt, Action<bool> resultCallback, Colors? colors = null)
    {
        message.IgnoreBackground = true;

        var yesButton = new Button(yesPrompt.Length + 2, 1);
        var noButton = new Button(noPrompt.Length + 2, 1);
        var window = new Window(message.ToString().Length + 4, 5 + yesButton.Surface.Height);

        if (colors != null) window.Controls.ThemeColors = colors;

        var printArea = new DrawingArea(window.Width, window.Height)
        {
            OnDraw = (ds, time) =>
            {
                if (!ds.IsDirty) return;
                ColoredGlyph appearance = ds.Appearance!;
                ds.Surface.Fill(appearance.Foreground, appearance.Background, null);
                ds.Surface.Print(2, 2, message);
                ds.IsDirty = true;
            }
        };

        yesButton.Position = new Point(2, window.Height - 1 - yesButton.Surface.Height);
        noButton.Position = new Point(window.Width - noButton.Width - 2, window.Height - 1 - yesButton.Surface.Height);

        yesButton.Text = yesPrompt;
        noButton.Text = noPrompt;

        yesButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); };
        noButton.Click += (o, e) => { window.DialogResult = false; window.Hide(); };

        window.Controls.Add(printArea);
        window.Controls.Add(yesButton);
        window.Controls.Add(noButton);

        noButton.IsFocused = true;

        window.Closed += (o, e) =>
            {
                resultCallback?.Invoke(window.DialogResult);
            };

        window.Show(true);
        window.Center();
    }

    /// <summary>
    /// Displays a dialog to the user with a specific message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="closeButtonText">The text of the dialog's close button.</param>
    /// <param name="closedCallback">A callback indicating the message was dismissed.</param>
    /// <param name="colors">The colors to apply for the message box and buttons. If <see langword="null"/>, then the default <see cref="Library.Colors"/> will be used.</param>
    public static void Message(string message, string closeButtonText, Action? closedCallback = null, Colors? colors = null) =>
        Message(new ColoredString(message), closeButtonText, closedCallback, colors);

    /// <summary>
    /// Displays a dialog to the user with a specific message.
    /// </summary>
    /// <param name="message">The message. (background color is ignored)</param>
    /// <param name="closeButtonText">The text of the dialog's close button.</param>
    /// <param name="closedCallback">A callback indicating the message was dismissed.</param>
    /// <param name="colors">The colors to apply for the message box and buttons. If <see langword="null"/>, then the default <see cref="Library.Colors"/> will be used.</param>
    public static void Message(ColoredString message, string closeButtonText, Action? closedCallback = null, Colors? colors = null)
    {
        int width = message.ToString().Length + 4;
        int buttonWidth = closeButtonText.Length + 2;

        if (buttonWidth < 9)
            buttonWidth = 9;

        if (width < buttonWidth + 4)
            width = buttonWidth + 4;

        var closeButton = new Button(buttonWidth, 1)
        {
            Text = closeButtonText,
        };

        var window = new Window(width, 5 + closeButton.Surface.Height);

        if (colors != null) window.Controls.ThemeColors = colors;

        message.IgnoreBackground = true;

        var printArea = new DrawingArea(window.Width, window.Height)
        {
            OnDraw = (ds, time) =>
            {
                if (!ds.IsDirty) return;
                ColoredGlyph appearance = ds.Appearance!;
                ds.Surface.Fill(appearance.Foreground, appearance.Background, null);
                ds.Surface.Print(2, 2, message);
                ds.IsDirty = true;
            }
        };
        window.Controls.Add(printArea);

        closeButton.Position = new Point(2, window.Height - 1 - closeButton.Surface.Height);
        closeButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); closedCallback?.Invoke(); };

        window.Controls.Add(closeButton);
        closeButton.IsFocused = true;
        window.CloseOnEscKey = true;
        window.Show(true);
        window.Center();
    }
}
