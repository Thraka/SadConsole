using System;
using SadConsole.StringParser;

namespace SadConsole;

public partial class ColoredString
{
    /*
            https://github.com/Thraka/SadConsole/discussions/272

            New replaceable method named Parse. Must be generic enough to not accept an existing stack of commands.
            Shouldn't talk to surface itself. But how would it then do something like encoded commands that did something like
            clear the surface or even move the cursor around, like ansi sequences? Generally the cursor parses these though. So
            where should that design be?
    
            Either way, the string parser needs to be more dynamic in some case. If someone wanted BBCode, they should be able
            to replace the main Parse method with their own and the existing Cursor and other things that call coloredstring.parse
            should still function as normal.
    */

    /// <summary>
    /// The string parser to use for transforming strings into <see cref="ColoredString"/>.
    /// </summary>
    public static IParser Parser { get; set; } = new Default();
}
