namespace SadConsole
{
    using System;
    using System.Collections.Generic;
    using SadConsole.Input;

    /// <summary>
    /// A <see cref="Console"/> that only processes children and does not render anything.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Container")]
    public class ContainerConsole : Console
    {
        public ContainerConsole()
            : base()
        {

        }

        public override void Draw(TimeSpan timeElapsed)
        {
            if (!IsVisible)
            {
                return;
            }

            foreach (Components.IConsoleComponent component in ComponentsDraw.ToArray())
            {
                component.Draw(this, timeElapsed);
            }

            var copyList = new List<Console>(Children);

            foreach (Console child in copyList)
            {
                child.Draw(timeElapsed);
            }
        }

        public override void Update(TimeSpan timeElapsed)
        {
            if (IsPaused)
            {
                return;
            }

            foreach (Components.IConsoleComponent component in ComponentsUpdate.ToArray())
            {
                component.Update(this, timeElapsed);
            }

            var copyList = new List<Console>(Children);

            foreach (Console child in copyList)
            {
                child.Update(timeElapsed);
            }
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            if (!IsVisible)
            {
                return false;
            }

            foreach (Components.IConsoleComponent component in ComponentsMouse.ToArray())
            {
                component.ProcessMouse(this, state, out bool isHandled);

                if (isHandled)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            foreach (Components.IConsoleComponent component in ComponentsKeyboard.ToArray())
            {
                component.ProcessKeyboard(this, info, out bool isHandled);

                if (isHandled)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
