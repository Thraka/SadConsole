using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Actions
{
    /// <summary>
    /// Represents a result from something an action being run.
    /// </summary>
    public struct ActionResult
    {
        public static readonly ActionResult Empty = new ActionResult(false);
        public static readonly ActionResult Success = new ActionResult(true);
        public static readonly ActionResult Failure = Empty;

        public readonly bool IsSuccess;
        public readonly int Data;

        public ActionResult(bool success, int data = 0)
        {
            IsSuccess = success;
            Data = data;
        }
    }
}
