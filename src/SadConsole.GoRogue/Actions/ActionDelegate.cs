using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Actions
{
    /// <summary>
    /// Action that calls a delegate.
    /// </summary>
    public class ActionDelegate: ActionBase
    {
        private readonly Action<TimeSpan> _actionCode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="run"></param>
        public ActionDelegate(Action<TimeSpan> run) => _actionCode = run;

        public override void Run(TimeSpan timeElapsed) => _actionCode(timeElapsed);
    }
}
