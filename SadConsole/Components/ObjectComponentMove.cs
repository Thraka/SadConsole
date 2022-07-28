using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.Components;

/// <summary>
/// Moves a <see cref="IScreenObject"/> with the arrow keys.
/// </summary>
public class ObjectComponentMove : KeyboardConsoleComponent
{
    private Point _leftAmount;
    private Point _rightAmount;
    private Point _upAmount;
    private Point _downAmount;
    private int _amount;

    /// <summary>
    /// The amount to move the object by.
    /// </summary>
    public int Amount
    {
        get => _amount;
        set
        {
            _amount = value;
            _leftAmount = (new Point(0, 0) + Direction.Left) * value;
            _rightAmount = (new Point(0, 0) + Direction.Right) * value;
            _upAmount = (new Point(0, 0) + Direction.Up) * value;
            _downAmount = (new Point(0, 0) + Direction.Down) * value;
        }
    }

    /// <summary>
    /// The key to move the object <see cref="Direction.Left"/> by <see cref="Amount"/>.
    /// </summary>
    public Keys LeftKey { get; set; } = Keys.Left;

    /// <summary>
    /// The key to move the object <see cref="Direction.Right"/> by <see cref="Amount"/>.
    /// </summary>
    public Keys RightKey { get; set; } = Keys.Right;

    /// <summary>
    /// The key to move the object <see cref="Direction.Up"/> by <see cref="Amount"/>.
    /// </summary>
    public Keys UpKey { get; set; } = Keys.Up;

    /// <summary>
    /// The key to move the object <see cref="Direction.Down"/> by <see cref="Amount"/>.
    /// </summary>
    public Keys DownKey { get; set; } = Keys.Down;

    /// <summary>
    /// Creates a new instance of the object with an <see cref="Amount"/> of 1.
    /// </summary>
    public ObjectComponentMove() =>
        Amount = 1;

    /// <summary>
    /// Moves the <paramref name="host"/> by <see cref="Amount"/> when the appropriate key is pressed.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="keyboard"></param>
    /// <param name="handled"></param>
    public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
    {
        handled = false;

        if (keyboard.IsKeyPressed(LeftKey))
        {
            host.Position += _leftAmount;
            handled = true;
        }
        else if (keyboard.IsKeyPressed(RightKey))
        {
            host.Position += _rightAmount;
            handled = true;
        }

        if (keyboard.IsKeyPressed(UpKey))
        {
            host.Position += _upAmount;
            handled = true;
        }
        else if (keyboard.IsKeyPressed(DownKey))
        {
            host.Position += _downAmount;
            handled = true;
        }
    }
}
