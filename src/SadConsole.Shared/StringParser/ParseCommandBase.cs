using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.StringParser
{
    /// <summary>
    /// Command type of a <see cref="ParseCommandBase"/>.
    /// </summary>
    public enum CommandTypes
    {
        /// <summary>
        /// Command should be added to the <see cref="ParseCommandStacks.Foreground"/> stack.
        /// </summary>
        Foreground,
        /// <summary>
        /// Command should be added to the <see cref="ParseCommandStacks.Background"/> stack.
        /// </summary>
        Background,
        /// <summary>
        /// Command should be added to the <see cref="ParseCommandStacks.Glyph"/> stack.
        /// </summary>
        Glyph,
        /// <summary>
        /// Command should be added to the <see cref="ParseCommandStacks.Mirror"/> stack.
        /// </summary>
        Mirror,
        /// <summary>
        /// Command should be added to the <see cref="ParseCommandStacks.Effect"/> stack.
        /// </summary>
        Effect,
        /// <summary>
        /// Command runs on creation and is not added to anything in <see cref="ParseCommandStacks"/>.
        /// </summary>
        PureCommand,
        /// <summary>
        /// Command is invalid and should not be processed at all.
        /// </summary>
        Invalid
    }

    /// <summary>
    /// Base class for a string processor behavior.
    /// </summary>
    public abstract class ParseCommandBase
    {
        /// <summary>
        /// Type of command.
        /// </summary>
        public CommandTypes CommandType = CommandTypes.Invalid;


        /// <summary>
        /// Builds a glyph.
        /// </summary>
        /// <param name="glyphState">The current glyph being built.</param>
        /// <param name="glyphString">The current string of glyphs that has been processed until now.</param>
        /// <param name="surfaceIndex">Where on the surface this flyph will appear.</param>
        /// <param name="surface">The surface associated with the glyph.</param>
        /// <param name="editor">The editor associated with the surface.</param>
        /// <param name="stringIndex">Where in the original string this glyph is from.</param>
        /// <param name="processedString">The entire string being processed.</param>
        /// <param name="commandStack">The state of commands.</param>
        public abstract void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex, ISurface surface, SurfaceEditor editor, ref int stringIndex, string processedString, ParseCommandStacks commandStack);
    }
}
