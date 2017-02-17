using Microsoft.Xna.Framework.Input;

using SadConsole.Themes;
using System;
using System.Runtime.Serialization;

namespace SadConsole.Controls
{
    /// <summary>
    /// Provides a button-like control that changes focus to a designated previous or next selection button when the arrow keys are pushed.
    /// </summary>
    [DataContract]
    public class SelectionButton: Button
    {
        /// <summary>
        /// The selection button to focus when the UP key is pressed or the SelectPrevious() method is called.
        /// </summary>
        public SelectionButton PreviousSelection { get; set; }

        /// <summary>
        /// The selection button to focus when the UP key is pressed or the SelectNext() method is called.
        /// </summary>
        public SelectionButton NextSelection { get; set; }
        

        /// <summary>
        /// Creates a new Selection Button with a specific width and height.
        /// </summary>
        /// <param name="width">The width of the selection button.</param>
        public SelectionButton(int width) : base(width) { defaultTheme = Library.Default.SelectionButtonTheme; }

        /// <summary>
        /// Sets the next selection button and optionally sets the previous of the referenced selection to this button.
        /// </summary>
        /// <param name="nextSelection">The selection button to be used as next.</param>
        /// <param name="setPreviousOnNext">Sets the PreviousSelection property on the <paramref name="nextSelection"/> instance to current selection button. Defaults to true.</param>
        /// <returns></returns>
        public SelectionButton SetNextSelection(ref SelectionButton nextSelection, bool setPreviousOnNext = true)
        {
            NextSelection = nextSelection;

            if (setPreviousOnNext)
                nextSelection.PreviousSelection = this;

            return nextSelection;
        }

        /// <summary>
        /// Focuses the previous or next selection button depending on if the UP or DOWN arrow keys were pressed.
        /// </summary>
        /// <param name="info">The keyboard state.</param>
        public override bool ProcessKeyboard(Input.Keyboard info)
        {
            base.ProcessKeyboard(info);

            if (info.IsKeyReleased(Keys.Up) && PreviousSelection != null)
            {
                PreviousSelection.IsFocused = true;

                return true;
            }

            else if (info.IsKeyReleased(Keys.Down) && NextSelection != null)
            {
                NextSelection.IsFocused = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Selects the previous selection button.
        /// </summary>
        /// <returns>Returns the previous selection button.</returns>
        public SelectionButton SelectPrevious()
        {
            if (PreviousSelection != null)
                PreviousSelection.IsFocused = true;

            return PreviousSelection;
        }

        /// <summary>
        /// Selects the next selection button.
        /// </summary>
        /// <returns>Returns the next selection button.</returns>
        public SelectionButton SelectNext()
        {
            if (NextSelection != null)
                NextSelection.IsFocused = true;

            return NextSelection;
        }
    }
}
