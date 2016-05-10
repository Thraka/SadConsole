namespace SadConsole.Instructions
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A set of instructions to be executed sequentially.
    /// </summary>
    [DataContract]
    public class InstructionSet : InstructionBase
    {
        /// <summary>
        /// All instructions in this set.
        /// </summary>
        [DataMember]
        public LinkedList<InstructionBase> Instructions = new LinkedList<InstructionBase>();

        /// <summary>
        /// The name of this instruction to identify it apart from other instruction sets.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Represents the current instruction if this set is currently executing.
        /// </summary>
        public InstructionBase CurrentInstruction
        {
            get
            {
                return _currentInstructionNode != null ? _currentInstructionNode.Value : null;
            }
        }

        protected LinkedListNode<InstructionBase> _currentInstructionNode;

        /// <summary>
        /// Resets each instruction's status so that it can be run again.
        /// </summary>
        public override void Reset()
        {
            foreach (var item in Instructions)
                item.Reset();

            base.Reset();
        }

        /// <summary>
        /// Runs the instruction set. Once all instructions are Done, this set will set the <see cref="Done"/> property will be set to true.
        /// </summary>
        public override void Run()
        {
            if (!IsFinished && Instructions.Count != 0)
            {
                if (_currentInstructionNode == null)
                    _currentInstructionNode = Instructions.First;

                _currentInstructionNode.Value.Run();

                if (_currentInstructionNode.Value.IsFinished)
                {
                    _currentInstructionNode = _currentInstructionNode.Next;

                    if (_currentInstructionNode == null)
                        IsFinished = true;
                }

                base.Run();
            }
        }
        
    }
}
