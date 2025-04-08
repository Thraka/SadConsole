// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;

namespace SadConsole.Analyzers.Sample;

// If you don't see warnings, build the Analyzers Project.

public class Examples
{
    public class MyCompanyClass // Try to apply quick fix using the IDE.
    {
    }

    public void ToStars()
    {
        var spaceship = new Spaceship();
        spaceship.SetSpeed(300000000); // Invalid value, it should be highlighted.
        spaceship.SetSpeed(42);
    }

    public void TestSC()
    {
        ColoredGlyphBase cell = new ColoredGlyph();
        cell.Decorators = new List<CellDecorator>();
        cell.Decorators = [];
        cell.Decorators = null;
        CellDecoratorHelpers.RemoveAllDecorators(cell);
    }
}
