using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.ImGuiSystem
{
    public abstract class ImGuiWindow : ImGuiObjectBase
    {
        public string Title { get; set; } = "";

        public bool IsOpen;

        public event EventHandler Closed;

        public bool DialogResult;

        protected void OnClosed() =>
            Closed?.Invoke(this, EventArgs.Empty);
    }
}
