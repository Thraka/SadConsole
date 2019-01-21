namespace SadConsole.Instructions
{
    using System;
    using Console = SadConsole.Console;

    /// <summary>
    /// Instruction that waits until the code callback returns <see langword="true"/>.
    /// </summary>
    public class PredicateInstruction : InstructionBase
    {
        private Func<bool> _callback;

        /// <summary>
        /// Friendly ID to help track what this code instruction was created from since it cannot be fully serialized.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Creates a new instruction with the specified callback.
        /// </summary>
        /// <param name="callback">The code invoked by this instruction. Return <see langword="true"/> to set <see cref="InstructionBase.IsFinished"/>.</param>
        public PredicateInstruction(Func<bool> callback) =>
            _callback = callback;

        private PredicateInstruction() { }

        /// <inheritdoc />
        public override void Update(Console console, TimeSpan delta)
        {
            IsFinished = _callback();

            base.Update(console, delta);
        }

        /// <summary>
        /// Sets the callback used by the instruction.
        /// </summary>
        /// <param name="callback">The code invoked by this instruction. Return <see langword="true"/> to set <see cref="InstructionBase.IsFinished"/>.</param>
        public void SetCallback(Func<bool> callback) =>
            _callback = callback;
    }
}
