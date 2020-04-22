using System;
using System.Collections.Generic;
using Console = SadConsole.Console;

namespace SadConsole.Instructions
{
    /// <summary>
    /// Runs one or more instructions at the same time. This instruction completes when all added instructions have finished.
    /// </summary>
    public class ConcurrentInstructions : InstructionBase
    {
        private IEnumerable<InstructionBase> _instructions;

        /// <summary>
        /// The instructions to run concurrently.
        /// </summary>
        public IEnumerable<InstructionBase> Instructions
        {
            get => _instructions;
            set => _instructions = value ?? throw new NullReferenceException("Instructions cannot be set to null.");
        }

        /// <summary>
        /// Creates a new instruction that runs the provided instructions concurrently.
        /// </summary>
        /// <param name="instructions">The instructions</param>
        public ConcurrentInstructions(IEnumerable<InstructionBase> instructions) =>
            _instructions = instructions;

        /// <inheritdoc />
        public override void Update(IScreenObject componentHost, TimeSpan delta)
        {
            bool stillRunning = false;

            foreach (InstructionBase item in _instructions)
            {
                item.Update(componentHost, delta);

                if (!item.IsFinished)
                {
                    stillRunning = true;
                }
            }

            IsFinished = !stillRunning;

            base.Update(componentHost, delta);
        }

        /// <inheritdoc />
        public override void Repeat()
        {
            foreach (InstructionBase item in _instructions)
            {
                item.Repeat();
            }

            base.Repeat();
        }

        /// <inheritdoc />
        public override void Reset()
        {
            foreach (InstructionBase item in _instructions)
            {
                item.Reset();
            }

            base.Reset();
        }
    }
}
