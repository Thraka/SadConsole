using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SadConsole.Input;
using SadConsole.UI.Controls;

namespace SadConsole.UI.Handlers;

public class TabFocusHandler : IKeyboardHandler
{
    private IKeyboardHandler? _next;

    public TabFocusHandler(IContainer container, IKeyboardHandler? next = null)
    {
        Container = container;
        _next = next;
    }

    /// <summary>
    /// The container this handler operates on.
    /// </summary>
    public IContainer Container { get; }

    private IList<ControlBase> ControlsList => Container;

    public bool ProcessKeyboard(Keyboard info, ControlBase? origin)
    {
        if (
            (info.IsKeyDown(Keys.LeftShift) ||
             info.IsKeyDown(Keys.RightShift) ||
             info.IsKeyReleased(Keys.LeftShift) ||
             info.IsKeyReleased(Keys.RightShift)) &&
            info.IsKeyReleased(Keys.Tab))
        {
            TabPreviousControl(origin);
            return true;
        }

        if (info.IsKeyReleased(Keys.Tab))
        {
            TabNextControl(origin);
            return true;
        }

        return false;
    }

    public void TabNextControl(ControlBase? startControl)
    {
        if (ControlsList.Count == 0)
            return;

        ControlBase? control;

        if (startControl == null)
        {
            if (TabForward(0, ControlsList.Count - 1))
            {
                return;
            }

            if (Container is ControlHost ch)
                ch.TryTabNextConsole();
        }
        else
        {
            int index = ControlsList.IndexOf(startControl);

            // From first control
            if (index == 0)
            {
                if (TabForward(index + 1, ControlsList.Count - 1))
                {
                    return;
                }

                TryTabNextConsole();
            }

            // From last control
            else if (index == ControlsList.Count - 1)
            {
                if (!TryTabNextConsole())
                {
                    if (TabForward(0, ControlsList.Count - 1))
                    {
                        return;
                    }
                }
            }

            // Middle
            else
            {
                // Middle > End
                if (TabForward(index + 1, ControlsList.Count - 1))
                {
                    return;
                }

                // Next console
                if (TryTabNextConsole())
                    return;

                // Start > Middle
                if (TabForward(0, index))
                {
                    return;
                }
            }
        }
    }

    public void TabPreviousControl(ControlBase startControl)
    {
        if (ControlsList.Count == 0)
            return;

        ControlBase? control;

        if (startControl == null)
        {
            if (TabBackward(ControlsList.Count - 1, 0))
            {
                return;
            }

            TryTabPreviousConsole();
        }
        else
        {
            int index = ControlsList.IndexOf(startControl);

            // From first control
            if (index == 0)
            {
                if (!TryTabPreviousConsole())
                {
                    if (TabBackward(ControlsList.Count - 1, 0))
                    {
                        return;
                    }
                }
            }

            // From last control
            else if (index == ControlsList.Count - 1)
            {
                if (TabBackward(index - 1, 0))
                {
                    return;
                }

                TryTabPreviousConsole();
            }

            // Middle
            else
            {
                // Middle -> Start
                if (TabBackward(index - 1, 0))
                {
                    return;
                }

                // Next console
                if (TryTabPreviousConsole())
                    return;

                // End -> Middle
                if (TabBackward(ControlsList.Count - 1, index))
                {
                    return;
                }
            }
        }
    }

    private bool TabForward(int startingIndex, int endingIndex)
    {
        for (int i = startingIndex; i <= endingIndex; i++)
        {
            if (ControlsList[i].TabStop && ControlsList[i].IsEnabled && ControlsList[i].CanFocus)
            {
                if (ControlsList[i].AcquireFocus(FocusDirection.Next))
                    return true;
            }
        }

        return false;
    }

    private bool TabBackward(int startingIndex, int endingIndex)
    {
        for (int i = startingIndex; i >= endingIndex; i--)
        {
            if (ControlsList[i].TabStop && ControlsList[i].IsEnabled && ControlsList[i].CanFocus)
            {
                if (ControlsList[i].AcquireFocus(FocusDirection.Previous))
                    return true;
            }
        }

        return false;
    }

    private bool TryTabNextConsole()
    {
        if (Container is ControlHost ch)
            return ch.TryTabNextConsole();

        return false;
    }

    private bool TryTabPreviousConsole()
    {
        if (Container is ControlHost ch)
            return ch.TryTabPreviousConsole();

        return false;
    }
}
