using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;


namespace SadConsole.Instructions
{

    [DataContract]
    public class CodeInstruction : InstructionBase
    {
        /// <summary>
        /// Friendly ID to help track what this code instruction was created from since it cannot be fully serialized.
        /// </summary>
        [DataMember]
        public string ID { get; set; }

        /// <summary>
        /// The code to execute when this instruction is run.
        /// </summary>
        public Action<CodeInstruction> CodeCallback { get; set; }

        public CodeInstruction() { }

        /// <summary>
        /// Runs this instruction.
        /// </summary>
        public override void Run()
        {
            CodeCallback(this);

            base.Run();
        }
    }
}
