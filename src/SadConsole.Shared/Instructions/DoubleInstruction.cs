using Microsoft.Xna.Framework;

using System;
using System.Runtime.Serialization;

namespace SadConsole.Instructions
{

    [DataContract]
    public class DoubleInstruction : InstructionBase
    {
        /// <summary>
        /// Friendly ID to help track what this code instruction was created from since it cannot be fully serialized.
        /// </summary>
        [DataMember]
        public string ID { get; set; }

        /// <summary>
        /// The code to execute when this instruction is run.
        /// </summary>
        [DataMember]
        public DoubleAnimation DoubleAnimationObject { get; set; }

        /// <summary>
        /// The code to execute when this instruction is run.
        /// </summary>
        public Action<double, DoubleInstruction> CodeCallback { get; set; }

        public DoubleInstruction() { }

        /// <summary>
        /// Runs this instruction.
        /// </summary>
        public override void Run()
        {
            if (!DoubleAnimationObject.IsStarted)
                DoubleAnimationObject.Start();

            CodeCallback?.Invoke(DoubleAnimationObject.CurrentValue, this);

            if (DoubleAnimationObject.IsFinished)
                IsFinished = true;

            base.Run();
        }

        public override void Repeat()
        {
            DoubleAnimationObject.Reset();
            base.Repeat();
        }

        public override void Reset()
        {
            DoubleAnimationObject.Reset();
            base.Reset();
        }
    }
}
