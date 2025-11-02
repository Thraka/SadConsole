using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadConsole.Components;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole;

public partial class AnimatedScreenObject
{
    [DataMember(Name = "Font")]
    [JsonConverter(typeof(SerializedTypes.FontJsonConverter))]
    private IFont _font;
    [DataMember(Name = "FontSize")]
    private Point _fontSize;
    [DataMember(Name = "Tint")]
    private Color _tint = Color.Transparent;
    [DataMember(Name = "UsePixelPositioning")]
    private bool _usePixelPositioning;

    /// <summary>
    /// The width to assign a frame when <see cref="CreateFrame"/> is called.
    /// </summary>
    [DataMember]
    protected int NewFrameWidth;

    /// <summary>
    /// The height to assign a frame when <see cref="CreateFrame"/> is called.
    /// </summary>
    [DataMember]
    protected int NewFrameHeight;

    /// <inheritdoc/>
    public bool ForceRendererRefresh { get; set; }

    /// <inheritdoc/>
    public virtual string DefaultRendererName { get; } = Renderers.Constants.RendererNames.Default;

    /// <inheritdoc/>
    [DataMember]
    [JsonConverter(typeof(SerializedTypes.RendererJsonConverter))]
    public IRenderer? Renderer { get; set; }
    /// <summary>
    /// The surface this screen object represents.
    /// </summary>
    ICellSurface IScreenSurface.Surface
    {
        get => Frames[CurrentFrameIndexValue];
    }

    /// <summary>
    /// When <see langword="true"/>, indicates that the animation needs to be redrawn; otherwise <see langword="false"/>.
    /// </summary>
    public bool IsDirty { get; set; }

    /// <inheritdoc/>
    public IFont Font
    {
        get => _font;

        [MemberNotNull(nameof(_font))]
        set
        {
            if (_font == value) return;

            _font = value;
            FontSize = _font.GetFontSize(IFont.Sizes.One);
            IsDirty = true;
        }
    }

    /// <inheritdoc/>
    public Point FontSize
    {
        get => _fontSize;
        set
        {
            if (_fontSize == value) return;

            _fontSize = value;
            IsDirty = true;
        }
    }

    /// <inheritdoc/>
    public Color Tint
    {
        get => _tint;
        set
        {
            _tint = value;
            IsDirty = true;
        }
    }

    /// <inheritdoc/>
    public Rectangle AbsoluteArea => new(AbsolutePosition.X, AbsolutePosition.Y, WidthPixels, HeightPixels);

    /// <inheritdoc/>
    public bool UsePixelPositioning
    {
        get => _usePixelPositioning;
        set
        {
            _usePixelPositioning = value;
            IsDirty = true;
            UpdateAbsolutePosition();
        }
    }

    /// <inheritdoc/>
    public int WidthPixels => Frames[CurrentFrameIndexValue].View.Width * FontSize.X;


    /// <inheritdoc/>
    public int HeightPixels => Frames[CurrentFrameIndexValue].View.Height * FontSize.Y;

    /// <summary>
    /// The total width of the console.
    /// </summary>
    public int Width => Frames[CurrentFrameIndexValue].Width;


    /// <summary>
    /// The total height of the console.
    /// </summary>
    public int Height => Frames[CurrentFrameIndexValue].Height;

    /// <summary>
    /// Gets or sets the visible width of the surface in cells.
    /// </summary>
    public int ViewWidth { get => Frames[CurrentFrameIndexValue].ViewWidth; set => Frames[CurrentFrameIndexValue].ViewWidth = value; }

    /// <summary>
    /// Gets or sets the visible height of the surface in cells.
    /// </summary>
    public int ViewHeight { get => Frames[CurrentFrameIndexValue].ViewHeight; set => Frames[CurrentFrameIndexValue].ViewHeight = value; }

    /// <summary>
    /// The position of the view within the console.
    /// </summary>
    public Point ViewPosition { get => Frames[CurrentFrameIndexValue].ViewPosition; set => Frames[CurrentFrameIndexValue].ViewPosition = value; }

    /// <summary>
    /// Gets the current animation frame surface. A shortcut for <code>Frames[CurrentFrameIndex]</code>
    /// </summary>
    public ICellSurface CurrentFrame => Frames[CurrentFrameIndexValue];

    /// <summary>
    /// Creates a new animation with the specified name, width, and height.
    /// </summary>
    /// <param name="name">The name of the animation.</param>
    /// <param name="width">The width of each frame this animation will have.</param>
    /// <param name="height">The height of each frame this animation will have.</param>
    public AnimatedScreenObject(string name, int width, int height) : this(name, width, height, GameHost.Instance.DefaultFont, GameHost.Instance.DefaultFont.GetFontSize(GameHost.Instance.DefaultFontSize)) { }

    /// <summary>
    /// Creates a new animation with the specified name and frames.
    /// </summary>
    /// <param name="name">The name of the animation.</param>
    /// <param name="frames">The frames that make up the animation.</param>
    public AnimatedScreenObject(string name, IEnumerable<ICellSurface> frames) : this(name,GameHost.Instance.DefaultFont, GameHost.Instance.DefaultFont.GetFontSize(GameHost.Instance.DefaultFontSize), frames)
    {
        if (Frames.Count == 0) throw new ArgumentOutOfRangeException(nameof(frames), "Collection of frames must have at least 1 element.");

        NewFrameWidth = Frames[0].Width;
        NewFrameHeight = Frames[0].Height;
        UseMouse = false;
        UseKeyboard = false;
    }

    /// <summary>
    /// Creates a new animation with the specified name, width, and height.
    /// </summary>
    /// <param name="name">The name of the animation.</param>
    /// <param name="width">The width of each frame this animation will have.</param>
    /// <param name="height">The height of each frame this animation will have.</param>
    /// <param name="font">The font used with this animation.</param>
    /// <param name="fontSize">The size of the font.</param>
    public AnimatedScreenObject(string name, int width, int height, IFont font, Point fontSize)
    {
        Name = name;
        Font = font;
        FontSize = fontSize;
        UseMouse = false;
        UseKeyboard = false;
        NewFrameWidth = width;
        NewFrameHeight = height;

        Frames = new List<ICellSurface>();

        Renderer = GameHost.Instance.GetRenderer(DefaultRendererName);
    }

    /// <summary>
    /// Creates a new animation with the specified name, font, font size, and frames.
    /// </summary>
    /// <param name="name">The name of the animation.</param>
    /// <param name="font">The font used by the animation.</param>
    /// <param name="fontSize">The size of the font.</param>
    /// <param name="frames">The frames that make up the animation.</param>
    [JsonConstructor]
    public AnimatedScreenObject(string name, IFont font, Point fontSize, IEnumerable<ICellSurface> frames)
    {
        Name = name;
        Font = font;
        FontSize = fontSize;

        Frames = new List<ICellSurface>(frames);
        Renderer = GameHost.Instance.GetRenderer(DefaultRendererName);
    }

    /// <inheritdoc />
    public override void UpdateAbsolutePosition()
    {
        if (UsePixelPositioning)
            AbsolutePosition = Position - (FontSize * Center);
        else
            AbsolutePosition = (FontSize * Position) - (FontSize * Center);

        if (!IgnoreParentPosition)
            AbsolutePosition += Parent?.AbsolutePosition ?? Point.Zero;
    }

    /// <summary>
    /// Draws the animation's current frame and all <see cref="ScreenObject.SadComponents"/> and <see cref="ScreenObject.Children"/>.
    /// </summary>
    /// <param name="delta">The time that has elapsed since the last call.</param>
    public override void Render(TimeSpan delta)
    {
        // Components first
        IComponent[] components = ComponentsRender.ToArray();
        int count = components.Length;
        for (int i = 0; i < count; i++)
            components[i].Render(this, delta);

        // This object second
        if (Renderer != null)
        {
            Renderer.Refresh(this, ForceRendererRefresh);
            Renderer.Render(this);
            ForceRendererRefresh = false;
        }

        // Children last
        IScreenObject[] children = Children.ToArray();
        count = children.Length;
        for (int i = 0; i < count; i++)
            if (children[i].IsVisible)
                children[i].Render(delta);
    }

    /// <summary>
    /// Updates the <see cref="AnimatedScreenObject"/> effects and all <see cref="ScreenObject.SadComponents"/> and <see cref="ScreenObject.Children"/>.
    /// </summary>
    /// <param name="delta">The time that has elapsed since this method was last called.</param>
    public override void Update(TimeSpan delta)
    {
        // Components first
        if (ComponentsUpdate.Count > 0)
        {
            IComponent[] components = ComponentsUpdate.ToArray();
            int count = components.Length;
            for (int i = 0; i < count; i++)
                components[i].Update(this, delta);
        }

        // This object second. Start with current animated frame, update those effects.
        if (!IsEmpty)
            Frames[CurrentFrameIndexValue].Effects.UpdateEffects(delta);

        // Run animation time and set new frame if needed
        if (IsPlaying && TimePerFrame != TimeSpan.Zero)
        {
            // TODO: Evaluate if we should change this to calculate current frame based on total time passed, \\not calculate frame based on individual frame duration on screen.
            AddedTime += delta;

            if (AddedTime > TimePerFrame)
            {
                AddedTime = TimeSpan.Zero;
                CurrentFrameIndexValue++;

                if (CurrentFrameIndexValue >= Frames.Count)
                {
                    if (Repeat)
                    {
                        CurrentFrameIndexValue = 0;
                        State = AnimationState.Restarted;
                        State = AnimationState.Playing;
                    }
                    else
                    {
                        IsPlaying = false;
                        CurrentFrameIndexValue--;
                        State = AnimationState.Finished;
                    }
                }

                IsDirty = true;
            }
        }

        // Children last
        if (Children.Count > 0)
        {
            IScreenObject[] children = Children.ToArray();
            int count = children.Length;
            for (int i = 0; i < count; i++)
                if (children[i].IsEnabled)
                    children[i].Update(delta);
        }
    }

    /// <summary>
    /// Returns the value "ScreenSurface".
    /// </summary>
    /// <returns>The string "ScreenSurface".</returns>
    public override string ToString() =>
        "ScreenSurface";

    public void ResyncFrameSize()
    {
        NewFrameWidth = Frames[0].Width;
        NewFrameHeight = Frames[0].Height;
    }

    /// <inheritdoc/>
    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        UpdateAbsolutePosition();
    }

    #region IDisposable Support
    private bool _disposedValue = false; // To detect redundant calls

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects).
            }

            Renderer?.Dispose();
            Renderer = null;

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Disposes <see cref="Renderer"/>.
    /// </summary>
    ~AnimatedScreenObject() =>
        Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
