using SadConsole.Input;
using SadRogue.Primitives;
using System;

namespace SadConsole.UI.Controls;

/// <summary>
/// Displays the glyphs associated with a font and allows the user to select one.
/// </summary>
public class CharacterPicker : SurfaceViewer
{
    Mirror _mirrorSetting;

    private Effects.ICellEffect? _selectedCharEffect;
    private int _selectedChar;
    
    /// <summary>
    /// Raised when the <see cref="SelectedCharacter"/> property changes.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<int>> SelectedCharacterChanged;

    /// <summary>
    /// When <see langword="true"/>, indicates that the control should use a mouse click to select a new character; otherwise <see langword="false"/> to indicate that just having the mouse down will select a new character.
    /// </summary>
    public bool UseFullClick { get; set; }

    /// <summary>
    /// Gets the position of the previously selected character. Used by the theme.
    /// </summary>
    public Point OldCharacterLocation { get; private set; } = new Point(-1, -1);

    /// <summary>
    /// Gets the position of the newly selected character. Used by the theme.
    /// </summary>
    public Point NewCharacterLocation { get; private set; } = new Point(-1, -1);

    /// <summary>
    /// Gets or sets the mirror effect to apply in the theme.
    /// </summary>
    public Mirror MirrorSetting
    {
        get { return _mirrorSetting; }
        set
        {
            _mirrorSetting = value;
            IsDirty = true;
        }
    }

    /// <summary>
    /// When true, highlights the selected character with <see cref="SelectedGlyphForeground"/> when <see cref="HighlightSelectedCharacterWithEffect"/> is false;
    /// </summary>
    public bool HighlightSelectedCharacter { get; set; }

    /// <summary>
    /// When true, uses <see cref="SelectedGlyphEffect"/> to highlight the character.
    /// </summary>
    public bool HighlightSelectedCharacterWithEffect { get; set; }

    /// <summary>
    /// Gets the foreground color used when drawing the glyphs.
    /// </summary>
    public Color GlyphForeground { get; private set; }

    /// <summary>
    /// Gets the background color used when drawing the glyphs.
    /// </summary>
    public Color GlyphBackground { get; private set; }

    /// <summary>
    /// Gets the foreground color used when drawing the selected glyph.
    /// </summary>
    public Color SelectedGlyphForeground { get; private set; }

    /// <summary>
    /// Gets the effect to apply when drawing the selected glyph.
    /// </summary>
    public Effects.ICellEffect? SelectedGlyphEffect
    {
        get => _selectedCharEffect;
        set
        {
            _selectedCharEffect = value;
            RefreshSelectedGlyph();
            IsDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the selected glyph character.
    /// </summary>
    public int SelectedCharacter
    {
        get { return _selectedChar; }
        set
        {
            if (_selectedChar == value) return;
            if (value < 0 || value >= Surface.Count) return;

            var old = _selectedChar;
            _selectedChar = value;

            OldCharacterLocation = Point.FromIndex(old, Surface.Width);
            NewCharacterLocation = Point.FromIndex(value, Surface.Width);

            SelectedCharacterChanged?.Invoke(this, new ValueChangedEventArgs<int>(old, value));

            RefreshSelectedGlyph();
            IsDirty = true;
        }
    }

    public CharacterPicker(Color foreground, Color fill, Color selectedCharacterColor, IFont characterFont, int visibleColumns, int visibleRows, int fontColumns)
    : base(visibleColumns, visibleRows, new CellSurface(fontColumns, characterFont.TotalGlyphs / fontColumns) { })
    {
        AlternateFont = characterFont;
        UseMouse = true;

        SelectedGlyphForeground = selectedCharacterColor;
        GlyphForeground = foreground;
        GlyphBackground = fill;
        HighlightSelectedCharacter = true;
        HighlightSelectedCharacterWithEffect = true;

        SetupEffect();

        ScrollBarMode = ScrollBarModes.AsNeeded;

        for (int i = 0; i < Surface.Count; i++)
            Surface[i].Glyph = i;

        RefreshSelectedGlyph();
    }

    /// <summary>
    /// Creates a new picker control with the specified font.
    /// </summary>
    /// <param name="foreground">The default foreground for glyphs.</param>
    /// <param name="fill">The default backround for glyphs.</param>
    /// <param name="selectedCharacterColor">The foreground for the selected glyph.</param>
    /// <param name="characterFont">The font to use with the control.</param>
    /// <param name="visibleColumns">The number of columns to show. The control will be this wide plus 1.</param>
    /// <param name="visibleRows">The number of rows to show. The control will be this high plus 1.</param>
    public CharacterPicker(Color foreground, Color fill, Color selectedCharacterColor, SadFont characterFont, int visibleColumns, int visibleRows)
        : base(visibleColumns, visibleRows, new CellSurface(characterFont.Columns, characterFont.Rows) { })
    {
        AlternateFont = characterFont;
        UseMouse = true;

        SelectedGlyphForeground = selectedCharacterColor;
        GlyphForeground = foreground;
        GlyphBackground = fill;
        HighlightSelectedCharacter = true;
        HighlightSelectedCharacterWithEffect = true;

        SetupEffect();

        ScrollBarMode = ScrollBarModes.AsNeeded;

        for (int i = 0; i < Surface.Count; i++)
            Surface[i].Glyph = i;

        RefreshSelectedGlyph();
    }

    private void SetupEffect()
    {
        _selectedCharEffect = new SadConsole.Effects.Fade()
        {
            FadeForeground = true,
            UseCellBackground = false,
            UseCellForeground = false,
            DestinationForeground = new Gradient(SelectedGlyphForeground, GlyphForeground),
            FadeDuration = System.TimeSpan.FromSeconds(0.5d),
            CloneOnAdd = false,
            AutoReverse = true,
            Repeat = true,
            RestoreCellOnRemoved = true
        };
    }

    /// <inheritdoc/>
    protected override void OnMouseIn(ControlMouseState info)
    {
        // If the mouse is in the area of the child surface content, check for click
        if ((!MouseState_EnteredWithButtonDown || Parent.Host.CapturedControl == this) && info.IsMouseOver && info.OriginalMouseState.Mouse.LeftButtonDown)
        {
            if (!UseFullClick)
            {
                Parent.Host.CaptureControl(this);
                SelectedCharacter = Surface[info.MousePosition.Add(Surface.ViewPosition).ToIndex(Surface.Width)].Glyph;
            }
        }
        else if (Parent.Host.CapturedControl == this)
            Parent.Host.ReleaseControl();

        base.OnMouseIn(info);
    }

    /// <inheritdoc/>
    protected override void OnLeftMouseClicked(ControlMouseState info)
    {
        if (info.IsMouseOver)
            SelectedCharacter = Surface[info.MousePosition.Add(Surface.ViewPosition).ToIndex(Surface.Width)].Glyph;

        base.OnLeftMouseClicked(info);
    }

    /// <inheritdoc/>
    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        if (Parent?.Host?.CapturedControl == this && !state.Mouse.LeftButtonDown)
            Parent.Host.ReleaseControl();

        if (state.Mouse.ScrollWheelValueChange != 0)
            return VerticalScroller.ProcessMouseWheel(state);

        return base.ProcessMouse(state);
    }

    /// <inheritdoc/>
    protected override ICellSurface CreateControlSurface()
    {
        CellSurface surface = new CellSurface(Width, Height)
        {
            DefaultForeground = GlyphForeground,
            DefaultBackground = GlyphBackground
        };
        
        surface.Clear();
        return surface;
    }

    private void RefreshSelectedGlyph()
    {
        if (_selectedCharEffect == null)
        {
            Surface.Effects.RemoveAll();

            Surface.Fill(GlyphForeground, GlyphBackground);

            if (HighlightSelectedCharacter)
                Surface[_selectedChar].Foreground = SelectedGlyphForeground;
                
        }
        else if (HighlightSelectedCharacterWithEffect)
        {
            Surface.Effects.RemoveAll();
            Surface.SetEffect(_selectedChar, _selectedCharEffect);

            _selectedCharEffect.Restart();
        }


    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        Surface.Effects.UpdateEffects(time);

        base.UpdateAndRedraw(time);
    }
}
