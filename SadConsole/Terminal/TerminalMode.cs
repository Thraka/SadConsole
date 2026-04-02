namespace SadConsole.Terminal;

/// <summary>
/// Selects the terminal emulation behavior profile.
/// </summary>
public enum TerminalMode
{
    /// <summary>CTerm/SyncTERM-compatible with full extensions. Default.</summary>
    CTerm,
    /// <summary>ANSI-BBS compatible (IBM CP437 C0 glyphs, BBS key sequences).</summary>
    AnsiBbs,
    /// <summary>Strict VT102 — no BBS or CTerm extensions.</summary>
    VT102,
    /// <summary>XTerm conventions.</summary>
    XTerm
}
