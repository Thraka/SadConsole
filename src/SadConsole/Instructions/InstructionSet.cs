namespace SadConsole.Instructions
{
    using System;
    using System.Collections.Generic;
    using Console = SadConsole.Console;

    /// <summary>
    /// A set of instructions to be executed sequentially.
    /// </summary>
    public class InstructionSet : InstructionBase
    {
        private LinkedListNode<InstructionBase> _currentInstructionNode;

        /// <summary>
        /// All instructions in this set.
        /// </summary>
        public LinkedList<InstructionBase> Instructions { get; } = new LinkedList<InstructionBase>();

        /// <summary>
        /// The name of this instruction to identify it apart from other instruction sets.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the current instruction if this set is currently executing.
        /// </summary>
        public InstructionBase CurrentInstruction => _currentInstructionNode != null ? _currentInstructionNode.Value : null;


        /// <inheritdoc />
        public override void Reset()
        {
            foreach (InstructionBase item in Instructions)
            {
                item.Reset();
            }

            _currentInstructionNode = null;

            base.Reset();
        }

        /// <summary>
        /// Runs the instruction set. Once all instructions are finished, this set will set the <see cref="InstructionBase.IsFinished"/> property will be set to <see langword="true"/>.
        /// </summary>
        public override void Update(Console console, TimeSpan delta)
        {
            if (!IsFinished && Instructions.Count != 0)
            {
                if (_currentInstructionNode == null)
                {
                    _currentInstructionNode = Instructions.First;
                }

                _currentInstructionNode.Value.Update(console, delta);

                if (_currentInstructionNode.Value.IsFinished)
                {
                    _currentInstructionNode = _currentInstructionNode.Next;

                    if (_currentInstructionNode == null)
                    {
                        IsFinished = true;
                    }
                }
            }
            else
            {
                IsFinished = true;
            }

            base.Update(console, delta);
        }

        /// <summary>
        /// Adds a new <see cref="SadConsole.Instructions.Wait"/> instruction with the specified duration to the end of this set.
        /// </summary>
        /// <param name="duration">The time to wait.</param>
        /// <returns>This instruction set.</returns>
        public InstructionSet Wait(TimeSpan duration)
        {
            Instructions.AddLast(new Wait(duration));
            return this;
        }

        /// <summary>
        /// Adds an instruction to the end of this set.
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns>This instruction set.</returns>
        public InstructionSet Instruct(InstructionBase instruction)
        {
            Instructions.AddLast(instruction);
            return this;
        }

        /// <summary>
        /// Adds a new <see cref="CodeInstruction"/> instruction with the specified callback to the end of this set.
        /// </summary>
        /// <param name="expression">The code callback.</param>
        /// <returns>This instruction set.</returns>
        public InstructionSet Code(Func<Console, TimeSpan, bool> expression)
        {
            Instructions.AddLast(new CodeInstruction(expression));
            return this;
        }

        /// <summary>
        /// Adds a new <see cref="PredicateInstruction"/> instruction with the specified callback to the end of this set.
        /// </summary>
        /// <param name="expression">The code callback.</param>
        /// <returns>This instruction set.</returns>
        public InstructionSet WaitTrue(Func<bool> expression)
        {
            Instructions.AddLast(new PredicateInstruction(expression));
            return this;
        }

        /// <summary>
        /// Adds a <see cref="ConcurrentInstructions"/> to the end of this set.
        /// </summary>
        /// <param name="instructions">Instructions to add. Must be two or more instructions.</param>
        /// <returns>This instruction set.</returns>
        public InstructionSet InstructConcurrent(params InstructionBase[] instructions)
        {
            if (instructions.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(instructions), "Two or more instruction must be provided.");
            }

            Instructions.AddLast(new ConcurrentInstructions(instructions));
            return this;
        }
    }
}
