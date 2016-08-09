using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Instructions
{
    /// <summary>
    /// 
    /// </summary>
    public class ConcurrentInstructions : InstructionBase
    {
        private IEnumerable<InstructionBase> _instructions;

        public IEnumerable<InstructionBase> Instructions
        {
            get { return _instructions; }
            set
            {
                if (value == null)
                    throw new NullReferenceException("Instructions cannot be set to null.");

                _instructions = value;
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
