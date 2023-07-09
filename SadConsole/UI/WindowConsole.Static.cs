using System;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI;

public partial class Window
{
    /// <summary>
    /// Displays a window with an input box and an optional validator with a text prompt.
    /// </summary>
    /// <param name="message">The message to print in the window.</param>
    /// <param name="acceptPrompt">The text of the accept button.</param>
    /// <param name="cancelPrompt">The text of the cancel button.</param>
    /// <param name="closedCallback">A delegate called when the window is closed.</param>
    /// <param name="validator">An optional validator to validate the input text.</param>
    /// <param name="defaultValue">An optional default value of the input text box.</param>
    /// <param name="colors">An optional set of colors to apply to the window.</param>
    public static void Ask(string message, string acceptPrompt, string cancelPrompt, Action<bool, string>? closedCallback, StringValidation.Validator? validator = null, string? defaultValue = null, Colors? colors = null) =>
        Ask(new ColoredString(message) { IgnoreBackground = true }, acceptPrompt, cancelPrompt, closedCallback, validator, defaultValue, colors);

    /// <summary>
    /// Displays a window with an input box and an optional validator, using a <see cref="ColoredString"/> as the prompt text.
    /// </summary>
    /// <param name="message">The message to print in the window.</param>
    /// <param name="acceptPrompt">The text of the accept button.</param>
    /// <param name="cancelPrompt">The text of the cancel button.</param>
    /// <param name="closedCallback">A delegate called when the window is closed.</param>
    /// <param name="validator">An optional validator to validate the input text.</param>
    /// <param name="defaultValue">An optional default value of the input text box.</param>
    /// <param name="colors">An optional set of colors to apply to the window.</param>
    public static void Ask(ColoredString message, string acceptPrompt, string cancelPrompt, Action<bool, string>? closedCallback, StringValidation.Validator? validator = null, string? defaultValue = null, Colors? colors = null)
    {
        Button yesButton = new(acceptPrompt.Length + 2, 1);
        Button noButton = new(cancelPrompt.Length + 2, 1);
        Window window = new(message.ToString().Length + 4, 7 + yesButton.Surface.Height);
        TextBox inputBox = new(window.Width - 4) { Text = defaultValue ?? "" };

        if (validator != null)
        {
            inputBox.Validator = validator;
            inputBox.TextValidated += (s, e) =>
            {
                yesButton.IsEnabled = e.IsValid;

                window.Clear(2, 3, window.Width - 4);

                if (!e.IsValid)
                {
                    string message = e.ErrorMessage;

                    if (message.Length > window.Width - 4)
                        message = message.Substring(0, window.Width - 4);

                    window.Print(2, 3, message, window.Controls.GetThemeColors().Red);
                }
            };
            inputBox.TextChanged += (s, e) => yesButton.IsEnabled = validator(inputBox.Text).IsValid;
        }
        if (colors != null) window.Controls.ThemeColors = colors;

        window.Print(2, 2, message);

        yesButton.Position = new Point(2, window.Height - 1 - yesButton.Surface.Height);
        noButton.Position = new Point(window.Width - noButton.Width - 2, window.Height - 1 - yesButton.Surface.Height);
        inputBox.Position = new Point(2, 4);

        yesButton.Text = acceptPrompt;
        noButton.Text = cancelPrompt;

        yesButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); };
        noButton.Click += (o, e) => { window.DialogResult = false; window.Hide(); };

        //window.Controls.Add(printArea);
        window.Controls.Add(inputBox);
        window.Controls.Add(yesButton);
        window.Controls.Add(noButton);

        inputBox.IsFocused = true;

        window.Closed += (o, e) =>
        {
            closedCallback?.Invoke(window.DialogResult, inputBox.Text);
        };

        window.Show(true);
        window.Center();
    }

    /// <summary>
    /// Shows a window prompt with two buttons for the user to click.
    /// </summary>
    /// <param name="message">The text to display.</param>
    /// <param name="yesPrompt">The yes button's text.</param>
    /// <param name="noPrompt">The no button's text.</param>
    /// <param name="closedCallback">Callback with the yes (true) or no (false) result.</param>
    /// <param name="colors">The colors to apply for the message box and buttons. If <see langword="null"/>.</param>
    public static void Prompt(string message, string yesPrompt, string noPrompt, Action<bool>? closedCallback, Colors? colors = null) =>
        Prompt(new ColoredString(message) { IgnoreBackground = true }, yesPrompt, noPrompt, closedCallback, colors);

    /// <summary>
    /// Shows a window prompt with two buttons for the user to click.
    /// </summary>
    /// <param name="message">The text to display. (background color is ignored)</param>
    /// <param name="yesPrompt">The yes button's text.</param>
    /// <param name="noPrompt">The no button's text.</param>
    /// <param name="closedCallback">Callback with the yes (true) or no (false) result.</param>
    /// <param name="colors">The colors to apply for the message box and buttons. If <see langword="null"/>.</param>
    public static void Prompt(ColoredString message, string yesPrompt, string noPrompt, Action<bool>? closedCallback, Colors? colors = null)
    {
        var yesButton = new Button(yesPrompt.Length + 2, 1);
        var noButton = new Button(noPrompt.Length + 2, 1);
        var window = new Window(message.ToString().Length + 4, 5 + yesButton.Surface.Height);

        if (colors != null) window.Controls.ThemeColors = colors;

        window.Print(2, 2, message);

        yesButton.Position = new Point(2, window.Height - 1 - yesButton.Surface.Height);
        noButton.Position = new Point(window.Width - noButton.Width - 2, window.Height - 1 - yesButton.Surface.Height);

        yesButton.Text = yesPrompt;
        noButton.Text = noPrompt;

        yesButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); };
        noButton.Click += (o, e) => { window.DialogResult = false; window.Hide(); };

        window.Controls.Add(yesButton);
        window.Controls.Add(noButton);

        noButton.IsFocused = true;

        window.Closed += (o, e) =>
            {
                closedCallback?.Invoke(window.DialogResult);
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
    /// <param name="colors">The colors to apply for the message box and buttons. If <see langword="null"/>.</param>
    public static void Message(string message, string closeButtonText, Action? closedCallback = null, Colors? colors = null) =>
        Message(new ColoredString(message) { IgnoreBackground = true } , closeButtonText, closedCallback, colors);

    /// <summary>
    /// Displays a dialog to the user with a specific message.
    /// </summary>
    /// <param name="message">The message. (background color is ignored)</param>
    /// <param name="closeButtonText">The text of the dialog's close button.</param>
    /// <param name="closedCallback">A callback indicating the message was dismissed.</param>
    /// <param name="colors">The colors to apply for the message box and buttons. If <see langword="null"/>.</param>
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

        window.Print(2, 2, message);

        closeButton.Position = new Point(2, window.Height - 1 - closeButton.Surface.Height);
        closeButton.Click += (o, e) => { window.DialogResult = true; window.Hide(); closedCallback?.Invoke(); };

        window.Controls.Add(closeButton);
        closeButton.IsFocused = true;
        window.CloseOnEscKey = true;
        window.Show(true);
        window.Center();
    }
}
