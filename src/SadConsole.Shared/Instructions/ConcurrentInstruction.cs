namespace SadConsole.Instructions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class ConcurrentInstructions : InstructionBase
    {
        private IEnumerable<InstructionBase> _instructions;

        public IEnumerable<InstructionBase> Instructions
        {
            get => _instructions;
            set
            {
                _instructions = value ?? throw new NullReferenceException("Instructions cannot be set to null.");
            }
        }

        public ConcurrentInstructions()
        {
            _instructions = new List<InstructionBase>();
        }

        public override void Run()
        {
            base.Run();

            foreach (var item in _instructions)
            {
                item.Run();
            }
        }

        public override void Repeat()
        {
            foreach (var item in _instructions)
            {
                item.Repeat();
            }

            base.Repeat();
        }

        public override void Reset()
        {
            foreach (var item in _instructions)
            {
                item.Reset();
            }

            base.Reset();
        }
    }
}
