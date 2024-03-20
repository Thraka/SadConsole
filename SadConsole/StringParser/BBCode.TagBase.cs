using System.Collections.Generic;

namespace SadConsole.StringParser.BBCode;

/// <summary>
/// The base class for a BBCode command.
/// </summary>
public abstract class BBCodeCommandBase : ParseCommandBase
{
    /// <summary>
    /// The tag used to create this command.
    /// </summary>
    public string Tag { get; protected set; }

    /// <summary>
    /// Provides the information about the tag that triggered this command.
    /// </summary>
    /// <param name="tag">The BBCode tag condition that triggered the command.</param>
    /// <param name="value">Optional value provided by the BBCode.</param>
    /// <param name="parameters">Optional parameters provided by the BBCode.</param>
    public abstract void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters);
}
