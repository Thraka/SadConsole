using System;
using System.Collections.Generic;
using Console = SadConsole.Console;

namespace SadConsole.Instructions;

/// <summary>
/// Runs one or more instructions at the same time. This instruction completes when all added instructions have finished.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Instruction: Concurrent")]
public class ConcurrentInstructions : InstructionBase
{
    private List<InstructionBase> _instructions;

    /// <summary>
    /// The instructions to run concurrently.
    /// </summary>
    public IEnumerable<InstructionBase> Instructions
    {
        get => _instructions;
        set => _instructions = new List<InstructionBase>(value ?? throw new NullReferenceException("Instructions cannot be set to null."));
    }

    /// <summary>
    /// Creates a new instruction that runs the provided instructions concurrently.
    /// </summary>
    /// <param name="instructions">The instructions</param>
    public ConcurrentInstructions(IEnumerable<InstructionBase> instructions) =>
        _instructions = new List<InstructionBase>(instructions);

    /// <inheritdoc />
    public override void Update(IScreenObject componentHost, TimeSpan delta)
    {
        bool stillRunning = false;

        int count = _instructions.Count;
        for (int i = 0; i < count; i++)
        {
            _instructions[i].Update(componentHost, delta);

            if (!_instructions[i].IsFinished)
                stillRunning = true;
        }

        IsFinished = !stillRunning;

        base.Update(componentHost, delta);
    }

    /// <inheritdoc />
    public override void Repeat()
    {
        int count = _instructions.Count;
        for (int i = 0; i < count; i++)
            _instructions[i].Repeat();

        base.Repeat();
    }

    /// <inheritdoc />
    public override void Reset()
    {
        int count = _instructions.Count;
        for (int i = 0; i < count; i++)
            _instructions[i].Reset();

        base.Reset();
    }
}
