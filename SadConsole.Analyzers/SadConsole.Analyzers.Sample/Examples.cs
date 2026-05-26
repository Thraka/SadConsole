// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using SadConsole.Configuration;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.Analyzers.Sample;

// If you don't see warnings, build the Analyzers Project.

public class Examples
{
    public class MyCompanyClass // Try to apply quick fix using the IDE.
    {
    }

    public void TestBuilder()
    {
        Builder.GetBuilder()
            .OnEnd(null)
            //.SetWindowSizeInPixels(200, 200)
            //.SetWindowSizeInCells(10, 10, false)
            .ConfigureWindow((config, builder, host) => { })
            .Run();
    }

    public void TestSC()
    {
        ScreenSurface surf = new(20, 20);

        ColoredGlyphBase cell = new ColoredGlyph();

        CellDecorator decorator1 = new();

        cell.Decorators = new();
        cell.Decorators = [];
        cell.Decorators = new List<CellDecorator>() { decorator1, new CellDecorator() { Color = Color.Purple } };
        surf.Surface[2].Decorators = [decorator1, new CellDecorator() { Color = Color.Purple }];

        CellDecoratorHelpers.SetDecorators(new List<CellDecorator>() { decorator1, new CellDecorator() { Color = Color.Purple } }, cell);
        CellDecoratorHelpers.SetDecorators([decorator1, new CellDecorator() { Color = Color.Purple }], cell);

        cell.Decorators = null;
        CellDecoratorHelpers.RemoveAllDecorators(cell);

        surf.Surface[3].Decorators = null;
        surf.Surface[3].Decorators = [];

    }

    public class control1 : ControlBase
    {
        public control1() : base(22, 22)
        {
            
        }

        public override void UpdateAndRedraw(TimeSpan time)
        {
            IsDirty = false;
        }

        public void something()
        {
            IsDirty = false;
        }
    }
}
