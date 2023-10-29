using SadConsole.Effects;
using SadConsole.Input;
using SadRogue.Primitives.GridViews;

namespace SadConsole.Examples;

internal class DemoCustomCellsRenderer : IDemo
{
    public string Title => "Custom glyph objects";

    public string Description => "This demo creates a custom rendering step and draw call which can rotate consoles.\r\n\r\n" +
                                 "As SadConsole renders objects, it builds a list of 'DrawCalls' that compose the console or surface. Such as, [c:r f:violet:3](1) clear the output texture [c:r f:violet:3](2) draw all the glyphs [c:r f:violet:3](3) apply tinting [c:r f:violet:3](4) render to the screen.\r\n";

    public string CodeFile => "DemoShapes.MonoGame.cs";

    public IScreenSurface CreateDemoScreen() =>
        new CustomSurface();

    public override string ToString() =>
        Title;

    void IDemo.PostCreateDemoScreen(SadConsole.IScreenSurface demoScreen)
    {
    }
}

class CustomSurface : ScreenSurface
{
    public CustomSurface() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        UseKeyboard = false;
        UseMouse = true;

        Surface = new OffsetCellsSurface(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height);
        Surface.FillWithRandomGarbage(Font);

        // Randomly delete some items
        for (int i = 0; i < 1000; i++)
        {
            Point location = new Point(GameHost.Instance.Random.Next(0, GameSettings.ScreenDemoBounds.Width),
                                       GameHost.Instance.Random.Next(0, GameSettings.ScreenDemoBounds.Height));
            Surface[location].Background = Surface.DefaultBackground;
            Surface[location].Glyph = Surface.DefaultGlyph;
        }

        // Get rid of the old one and assign a new one.
        Renderer?.Dispose();
        Renderer = new OffsetGlyphRenderer();
    }

    protected override void OnMouseMove(MouseScreenObjectState state)
    {
        base.OnMouseMove(state);

        if (state.IsOnScreenObject && state.Cell!.Glyph != 0 && ((ColoredGlyphOffset)state.Cell).RenderingOffset == Point.Zero)
        {
            Surface.SetEffect(state.Cell, new ShakeEffect(TimeSpan.FromMilliseconds(300)));
        }
    }
}

class ShakeEffect : CellEffectBase
{
    Instructions.AnimatedValue _animatedValueX;
    Instructions.AnimatedValue _animatedValueY;

    public ShakeEffect(TimeSpan shakeTime)
    {
        _animatedValueX = new(shakeTime, SadConsole.GameHost.Instance.Random.NextDouble() * -3, SadConsole.GameHost.Instance.Random.NextDouble() * 3, new SadConsole.EasingFunctions.Bounce() { Mode = EasingFunctions.EasingMode.InOut });
        _animatedValueY = new(shakeTime, SadConsole.GameHost.Instance.Random.NextDouble() * -3, SadConsole.GameHost.Instance.Random.NextDouble() * 3, new SadConsole.EasingFunctions.Bounce() { Mode = EasingFunctions.EasingMode.InOut });

        _animatedValueX.RepeatCount = 1;
        _animatedValueY.RepeatCount = 1;

        EventHandler repating = (s, e) =>
        {
            Instructions.AnimatedValue animated = (Instructions.AnimatedValue)s!;
            animated.Reset(shakeTime, SadConsole.GameHost.Instance.Random.NextDouble() * -3, SadConsole.GameHost.Instance.Random.NextDouble() * 3, new SadConsole.EasingFunctions.Bounce() { Mode = EasingFunctions.EasingMode.InOut });
        };

        _animatedValueX.Repeating += repating;
        _animatedValueY.Repeating += repating;

        RestoreCellOnRemoved = true;
        RemoveOnFinished = true;
    }

    public override bool ApplyToCell(ColoredGlyphBase cell, ColoredGlyphBase originalState)
    {
        Point newValue = new((int)_animatedValueX.Value, (int)_animatedValueY.Value);

        if (((ColoredGlyphOffset)cell).RenderingOffset != newValue)
        {
            ((ColoredGlyphOffset)cell).RenderingOffset = newValue;
            return true;
        }

        return false;
    }

    public override ICellEffect Clone() =>
        new ShakeEffect(_animatedValueX.Duration);

    public override void Update(TimeSpan delta)
    {
        if (!IsFinished)
        {
            _animatedValueX.Update(null!, delta);
            _animatedValueY.Update(null!, delta);

            if (_animatedValueX.IsFinished && _animatedValueY.IsFinished)
                IsFinished = true;

            base.Update(delta);
        }
    }
}

class OffsetCellsSurface : CellSurface
{
    public OffsetCellsSurface(int width, int height) : this(width, height, width, height, null)
    {
    }

    public OffsetCellsSurface(int width, int height, ColoredGlyphBase[] initialCells) : this(width, height, width, height, initialCells)
    {
    }

    public OffsetCellsSurface(int viewWidth, int viewHeight, int totalWidth, int totalHeight) : this(viewWidth, viewHeight, totalWidth, totalHeight, null)
    {
    }

    public OffsetCellsSurface(IGridView<ColoredGlyphBase> surface, int visibleWidth = 0, int visibleHeight = 0) : base(surface, visibleWidth, visibleHeight)
    {
        ColoredGlyphOffset[] CustomGlyphs = new ColoredGlyphOffset[Width * Height];

        for (int i = 0; i < Width * Height; i++)
            CustomGlyphs[i] = new ColoredGlyphOffset(Surface.DefaultForeground, Surface.DefaultBackground, Surface.DefaultGlyph);

        Cells = CustomGlyphs;
    }

    public OffsetCellsSurface(int viewWidth, int viewHeight, int totalWidth, int totalHeight, ColoredGlyphBase[]? initialCells) : base(viewWidth, viewHeight, totalWidth, totalHeight, initialCells)
    {
        ColoredGlyphOffset[] CustomGlyphs = new ColoredGlyphOffset[Width * Height];

        for (int i = 0; i < Width * Height; i++)
            CustomGlyphs[i] = new ColoredGlyphOffset(Surface.DefaultForeground, Surface.DefaultBackground, Surface.DefaultGlyph);

        Cells = CustomGlyphs;
    }
}

class ColoredGlyphOffset : ColoredGlyphBase
{
    private Point _renderingOffset = Point.Zero;

    public Point RenderingOffset
    {
        get => _renderingOffset;
        set { _renderingOffset = value; IsDirty = true; }
    }

    /// <summary>
    /// Creates a cell with a white foreground, black background, glyph 0, and no mirror effect.
    /// </summary>
    public ColoredGlyphOffset() : this(Color.White, Color.Black, 0, Mirror.None) { }

    /// <summary>
    /// Creates a cell with the specified foreground, black background, glyph 0, and no mirror effect.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    public ColoredGlyphOffset(Color foreground) : this(foreground, Color.Black, 0, Mirror.None) { }

    /// <summary>
    /// Creates a cell with the specified foreground, specified background, glyph 0, and no mirror effect.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    public ColoredGlyphOffset(Color foreground, Color background) : this(foreground, background, 0, Mirror.None) { }

    /// <summary>
    /// Creates a cell with the specified foreground, background, and glyph, with no mirror effect.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <param name="glyph">The glyph index.</param>
    public ColoredGlyphOffset(Color foreground, Color background, int glyph) : this(foreground, background, glyph, Mirror.None) { }

    /// <summary>
    /// Creates a cell with the specified foreground, background, glyph, and mirror effect.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <param name="glyph">The glyph index.</param>
    /// <param name="mirror">The mirror effect.</param>
    public ColoredGlyphOffset(Color foreground, Color background, int glyph, Mirror mirror)
    {
        Foreground = foreground;
        Background = background;
        Glyph = glyph;
        Mirror = mirror;
    }

    /// <summary>
    /// Creates a cell with the specified foreground, background, glyph, mirror, and visibility.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <param name="glyph">The glyph index.</param>
    /// <param name="mirror">The mirror effect.</param>
    /// <param name="isVisible">The visiblity of the glyph.</param>
    public ColoredGlyphOffset(Color foreground, Color background, int glyph, Mirror mirror, bool isVisible)
    {
        Foreground = foreground;
        Background = background;
        Glyph = glyph;
        Mirror = mirror;
        IsVisible = isVisible;
    }

    /// <summary>
    /// Creates a cell with the specified foreground, background, glyph, mirror effect, visibility and decorators.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <param name="glyph">The glyph index.</param>
    /// <param name="mirror">The mirror effect.</param>
    /// <param name="isVisible">The visiblity of the glyph.</param>
    /// <param name="decorators">Decorators for the cell.</param>
    public ColoredGlyphOffset(Color foreground, Color background, int glyph, Mirror mirror, bool isVisible, List<CellDecorator> decorators)
    {
        Foreground = foreground;
        Background = background;
        Glyph = glyph;
        Mirror = mirror;
        IsVisible = isVisible;
        Decorators = decorators;
    }
    /// <inheritdoc/>
    public override void Clear()
    {
        Foreground = Color.White;
        Background = Color.Black;
        Glyph = 0;
        Mirror = Mirror.None;
        Decorators = null;
    }

    public override void CopyAppearanceTo(ColoredGlyphBase cell, bool deepCopy = true)
    {
        base.CopyAppearanceTo(cell, deepCopy);

        if (cell is ColoredGlyphOffset cellOffset)
            cellOffset.RenderingOffset = RenderingOffset;
    }

    public override void CopyAppearanceFrom(ColoredGlyphBase cell, bool deepCopy = true)
    {
        base.CopyAppearanceFrom(cell, deepCopy);

        if (cell is ColoredGlyphOffset cellOffset)
            RenderingOffset = cellOffset.RenderingOffset;
    }

    public override ColoredGlyphBase Clone()
    {
        ColoredGlyphOffset glyph = new(Foreground, Background, Glyph, Mirror)
        {
            RenderingOffset = RenderingOffset,
            IsVisible = IsVisible,
        };
        CellDecoratorHelpers.SetDecorators(Decorators, glyph);
        return glyph;
    }
}

