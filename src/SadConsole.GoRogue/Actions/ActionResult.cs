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
        /// <summary>
        /// A success result without any supplemental data.
        /// </summary>
        public static readonly ActionResult Success = new ActionResult(true);

        /// <summary>
        /// A failure result withotu any supplemental data.
        /// </summary>
        public static readonly ActionResult Failure = new ActionResult(false);

        /// <summary>
        /// When <see langword="true"/>, indicates that this result was successful; otherwise <see langword="false"/>
        /// </summary>
        public readonly bool IsSuccess;

        /// <summary>
        /// A user supplied value to dictate more information or state that is associated with this result.
        /// </summary>
        public readonly int Data;

        /// <summary>
        /// Creates a new <see cref="ActionResult"/> with the specified success value.
        /// </summary>
        /// <param name="success">Indicates that this result was succesful or not.</param>
        /// <param name="data">Supplemental data associated with the result.</param>
        public ActionResult(bool success, int data = 0)
        {
            IsSuccess = success;
            Data = data;
        }
    }
}
