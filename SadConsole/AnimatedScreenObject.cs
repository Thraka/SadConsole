using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// A <see cref="ScreenObject"/> that displays an animated set of <see cref="ICellSurface"/> surfaces.
/// </summary>
[DataContract]
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public partial class AnimatedScreenObject : ScreenObject, IScreenSurface
{
    private TimeSpan _animatedTime;
    private AnimationState _state;
    private Point _center;

    /// <summary>
    /// Raised when the <see cref="AnimationState"/> changes.
    /// </summary>
    public event EventHandler<AnimationStateChangedEventArgs>? AnimationStateChanged;

    /// <summary>
    /// Center of the animation used in positioning.
    /// </summary>
    [DataMember]
    public Point Center
    {
        get => _center;
        set
        {
            _center = value;
            IsDirty = true;
            UpdateAbsolutePosition();
        }
    }


    /// <summary>
    /// Indicates whether or not this animation will repeat once it has finished animating.
    /// </summary>
    [DataMember]
    public bool Repeat { get; set; }

    /// <summary>
    /// When true, Indicates the animation is currently animating. The <see cref="Update"/> method will advance the frames.
    /// </summary>
    [DataMember]
    public bool IsPlaying { get; protected set; }

    /// <summary>
    /// The length of the animation.
    /// </summary>
    [DataMember]
    public TimeSpan AnimationDuration
    {
        get => _animatedTime;
        set { _animatedTime = value; CalculateFrameDuration(); }
    }

    /// <summary>
    /// Gets or sets the current frame index to animate.
    /// </summary>
    public int CurrentFrameIndex
    {
        get => CurrentFrameIndexValue;
        set
        {
            if (value < 0 || value >= Frames.Count)
            {
                CurrentFrameIndexValue = 0;
                IsDirty = true;
            }

            else if (value != CurrentFrameIndexValue)
            {
                CurrentFrameIndexValue = value;
                IsDirty = true;
            }
        }
    }

    /// <summary>
    /// Indicates the animation is empty.
    /// </summary>
    public bool IsEmpty => Frames.Count == 0;

    /// <summary>
    /// The frames of the animated surface.
    /// </summary>
    [DataMember]
    public List<ICellSurface> Frames { get; }

    /// <summary>
    /// Gets the name of this animation.
    /// </summary>
    [DataMember]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the current animation state.
    /// </summary>
    public AnimationState State
    {
        get => _state;
        set
        {
            AnimationState oldState = _state;

            if (value != _state)
            {
                _state = value;
                AnimationStateChanged?.Invoke(this, new AnimationStateChangedEventArgs(oldState, _state));
            }
        }
    }

    /// <summary>
    /// Time counter for the animation
    /// </summary>
    protected TimeSpan AddedTime;

    /// <summary>
    /// The current frame index being animated.
    /// </summary>
    protected int CurrentFrameIndexValue;

    /// <summary>
    /// How much time per animated frame should be used.
    /// </summary>
    protected TimeSpan TimePerFrame;

    /// <summary>
    /// Creates a new frame with the same dimensions as this entity and adds it to the Frames collection of the entity.
    /// </summary>
    /// <returns>The created frame.</returns>
    public ICellSurface CreateFrame()
    {
        var frame = new CellSurface(NewFrameWidth, NewFrameHeight);

        if (Frames.Count > 0)
        {
            frame.DefaultBackground = Frames[0].DefaultBackground;
            frame.DefaultForeground = Frames[0].DefaultForeground;
            frame.DefaultGlyph = Frames[0].DefaultGlyph;
        }

        frame.Clear();

        Frames.Add(frame);
        IsDirty = true;
        return frame;
    }

    /// <summary>
    /// Calculates the time needed per frame for rendering.
    /// </summary>
    private void CalculateFrameDuration()
    {
        if (IsEmpty || _animatedTime == TimeSpan.Zero)
            TimePerFrame = TimeSpan.Zero;
        else
            TimePerFrame = _animatedTime / Frames.Count;
    }

    /// <summary>
    /// Stops animating.
    /// </summary>
    public void Stop()
    {
        IsPlaying = false;
        State = AnimationState.Stopped;
    }

    /// <summary>
    /// Starts animating the frames.
    /// </summary>
    public void Start()
    {
        CalculateFrameDuration();
        IsPlaying = true;
        State = AnimationState.Playing;
    }

    /// <summary>
    /// Restarts the animation from the first frame.
    /// </summary>
    public void Restart()
    {
        CalculateFrameDuration();
        IsPlaying = true;
        CurrentFrameIndex = 0;
        State = AnimationState.Restarted;
        State = AnimationState.Playing;
    }

    /// <summary>
    /// Changes the <see cref="CurrentFrame"/> to the next frame.
    /// </summary>
    /// <param name="circular">If true and the current frame is the last, sets the current frame to the first frame.</param>
    public void MoveNext(bool circular = false)
    {
        if (circular)
        {
            if (CurrentFrameIndex == Frames.Count - 1)
                CurrentFrameIndex = 0;
            else
                CurrentFrameIndex++;
        }
        else
            CurrentFrameIndex++;
    }

    /// <summary>
    /// Changes the <see cref="CurrentFrame"/> to the previous frame.
    /// </summary>
    /// <param name="circular">If true and the current frame is the first, sets the current frame to the last frame.</param>
    public void MovePrevious(bool circular = false)
    {
        if (circular)
        {
            if (CurrentFrameIndex == 0)
                CurrentFrameIndex = Frames.Count - 1;
            else
                CurrentFrameIndex--;
        }
        else
            CurrentFrameIndex--;
    }

    /// <summary>
    /// Changes the <see cref="CurrentFrame"/> to the last frame.
    /// </summary>
    public void MoveEnd() =>
        CurrentFrameIndex = Frames.Count - 1;

    /// <summary>
    /// Changes the <see cref="CurrentFrame"/> to the first frame.
    /// </summary>
    public void MoveStart() =>
        CurrentFrameIndex = 0;

    /// <summary>
    /// Creates an animated surface that looks like static noise.
    /// </summary>
    /// <param name="width">The width of the surface.</param>
    /// <param name="height">The height of the surface.</param>
    /// <param name="frames">How many frames the animation should have.</param>
    /// <param name="blankChance">Chance a character will be blank. Characters are between index 48-158. Chance is evaluated versus <see cref="System.Random.NextDouble"/>.</param>
    /// <param name="background">The background color of the animation. Defaults to transparent.</param>
    /// <param name="foreground">The foreground color of the animation. Defaults to white.</param>
    /// <returns>An animation.</returns>
    public static AnimatedScreenObject CreateStatic(int width, int height, int frames, double blankChance, Color? background = null, Color? foreground = null)
    {
        var animation = new AnimatedScreenObject("default", width, height);

        Color foregroundColor = foreground ?? Color.White;
        for (int f = 0; f < frames; f++)
        {
            ICellSurface frame = animation.CreateFrame();

            // First frame is the template for the rest of CreateFrame
            if (f == 0)
            {
                frame.DefaultBackground = background ?? Color.Black;
                frame.Clear();
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int character = GameHost.Instance.Random.Next(48, 168);

                    if (GameHost.Instance.Random.NextDouble() <= blankChance)
                    {
                        character = 32;
                    }

                    frame.SetGlyph(x, y, character);
                    frame.SetForeground(x, y, foregroundColor * (float)(GameHost.Instance.Random.NextDouble() * (1.0d - 0.5d) + 0.5d));
                }
            }

        }

        animation.AnimationDuration = TimeSpan.FromSeconds(0.5);
        animation.Repeat = true;

        animation.Start();

        return animation;
    }

    /// <summary>
    /// Converts an image file containing frames to an instance of <see cref="AnimatedScreenObject"/>.
    /// </summary>
    /// <param name="name">Name for the animation.</param>
    /// <param name="filePath">File path to the image file.</param>
    /// <param name="frameLayout">Layout of frames in the image file: X number of columns, Y number of rows.</param>
    /// <param name="frameDuration">Duration for a frame in the animation.</param>
    /// <param name="pixelPadding">Pixel padding separating frames: X between the columns, Y between the rows.</param>
    /// <param name="frameStartAndFinish">Limits the number of frames copied to the animation. X first frame index, Y last frame index.</param>
    /// <param name="font"> <see cref="IFont"/> to be used when creating the <see cref="AnimatedScreenObject"/>.</param>
    /// <param name="action">Callback that will be applied to each <see cref="ColoredGlyphBase"/> when creating a frame.</param>
    /// <param name="convertMode">The mode used when converting the texture to a surface.</param>
    /// <param name="convertBackgroundStyle">The style to use when <paramref name="convertMode"/> is <see cref="TextureConvertMode.Background"/>.</param>
    /// <param name="convertForegroundStyle">The style to use when <paramref name="convertMode"/> is <see cref="TextureConvertMode.Foreground"/>.</param>
    /// 
    /// <returns>An instance of <see cref="AnimatedScreenObject"/> with converted frames.</returns>
    /// 
    /// <remarks>This method assumes the image file contains only frames and optional padding between the frames, no border space.
    /// 
    /// Frame count is calculated by multiplying rows and columns from the frame layout. It can by limited by specifying frame start and finish indexes.
    /// 
    /// Frame size and the subsequent AnimatedScreenSurface size is calculated from the size of the image file, number of frames, padding and the font size ratio.
    /// </remarks>
    public static AnimatedScreenObject FromImage(string name, string filePath, Point frameLayout, TimeSpan frameDuration,
        Point? pixelPadding = null, Point? frameStartAndFinish = null, IFont? font = null, Action<ColoredGlyphBase>? action = null,
        TextureConvertMode convertMode = TextureConvertMode.Foreground, TextureConvertForegroundStyle convertForegroundStyle = TextureConvertForegroundStyle.Block,
        TextureConvertBackgroundStyle convertBackgroundStyle = TextureConvertBackgroundStyle.Pixel)
    {
        // set defaults
        font ??= GameHost.Instance.DefaultFont;
        Point padding = pixelPadding ?? (0, 0);
        frameStartAndFinish ??= (0, 0);

        int firstFrame = frameStartAndFinish.Value.X;
        int lastFrame = frameStartAndFinish.Value.Y;

        // get info about the font
        Point fontSize = font.GetFontSize(IFont.Sizes.One);
        (float X, float Y) fontSizeRatio = font.GetGlyphRatio(fontSize);

        // load the image
        using ITexture image = GameHost.Instance.GetTexture(filePath);
        Point imageSize = (image.Width, image.Height);

        // convert the image to surface
        Point surfaceSize = ApplyFontSizeRatio(imageSize, fontSizeRatio);
        ICellSurface convertedImage = image.ToSurface(convertMode, surfaceSize.X, surfaceSize.Y,
                                                      backgroundStyle: convertBackgroundStyle, foregroundStyle: convertForegroundStyle);
        padding = ApplyFontSizeRatio(padding, fontSizeRatio);

        // calculate the number of frames
        int totalFrameCount = frameLayout.X * frameLayout.Y;
        bool customFrameCountIsValid = firstFrame >= 0 && lastFrame > firstFrame && lastFrame < totalFrameCount;
        int frameCount = customFrameCountIsValid ? lastFrame - firstFrame + 1 : totalFrameCount;

        // calculate the frame size and create an instance of an animated screen surface
        Point frameSize = ((surfaceSize.X - (padding.X * (frameLayout.X - 1))) / frameLayout.X, (surfaceSize.Y - (padding.Y * (frameLayout.Y - 1))) / frameLayout.Y);
        var clip = new AnimatedScreenObject(name, frameSize.X, frameSize.Y, font, fontSize)
        {
            AnimationDuration = frameCount * frameDuration
        };

        // copy frames from the converted image data
        var currentFrameArea = new Rectangle(0, 0, frameSize.X, frameSize.Y);
        CreateFrames();

        return clip;

        // nested loops extracted to a function to be able to get out from anywhere, rather than break.
        void CreateFrames()
        {
            for (int y = 0; y < frameLayout.Y; y++)
            {
                currentFrameArea = currentFrameArea.WithX(0);
                for (int x = 0; x < frameLayout.X; x++)
                {
                    if (customFrameCountIsValid)
                    {
                        int frameIndex = y * frameLayout.X + x;
                        if (frameIndex >= firstFrame)
                        {
                            if (frameIndex > lastFrame)
                                return;
                            else
                                CopyFrameToClip();
                        }
                    }
                    else
                        CopyFrameToClip();

                    // take into account padding between the frames, not outside them
                    int paddingX = x + 1 == frameLayout.X ? 0 : padding.X;
                    currentFrameArea = currentFrameArea.ChangeX(frameSize.X + paddingX);
                }

                // take into account padding between the frames, not outside them
                int paddingY = y + 1 == frameLayout.Y ? 0 : padding.Y;
                currentFrameArea = currentFrameArea.ChangeY(frameSize.Y + paddingY);
            }
        }

        void CopyFrameToClip()
        {
            CellSurface frame = (CellSurface)clip.CreateFrame();
            convertedImage.Copy(currentFrameArea, frame, 0, 0);
            if (action != null) Array.ForEach(frame.Cells, action);
        }

        static Point ApplyFontSizeRatio(Point point, (float X, float Y) sizeRatio) =>
            (ApplySizeRatio(point.X, sizeRatio.X), ApplySizeRatio(point.Y, sizeRatio.Y));

        static int ApplySizeRatio(int x, float sizeRatio, bool inverted = false) => !inverted ?
            sizeRatio < 1 ? x : (int)Math.Floor(x / sizeRatio) :
            sizeRatio > 1 ? (int)Math.Floor(x * sizeRatio) : x;
    }

    /// <summary>
    /// Saves the <see cref="AnimatedScreenObject"/> to a file.
    /// </summary>
    /// <param name="file">The destination file.</param>
    public void Save(string file) => Serializer.Save(this, file, Settings.SerializationIsCompressed);

    /// <summary>
    /// Loads a <see cref="AnimatedScreenObject"/> from a file.
    /// </summary>
    /// <param name="file">The source file.</param>
    /// <returns>The animated surface.</returns>
    public static AnimatedScreenObject Load(string file) => Serializer.Load<AnimatedScreenObject>(file, Settings.SerializationIsCompressed);

    /// <summary>
    /// Event args for when the animation state changes
    /// </summary>
    public class AnimationStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The previous state.
        /// </summary>
        public readonly AnimationState PreviousState;

        /// <summary>
        /// The new state.
        /// </summary>
        public readonly AnimationState NewState;

        /// <summary>
        /// Creates a new instance of the event args.
        /// </summary>
        /// <param name="previousState">The previous state.</param>
        /// <param name="newState">The new state.</param>
        public AnimationStateChangedEventArgs(AnimationState previousState, AnimationState newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }
    }

    /// <summary>
    /// Represents what state the animation is in.
    /// </summary>
    public enum AnimationState
    {
        /// <summary>
        /// The animation has never been played or was forcibly stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// The animation is currently playing.
        /// </summary>
        Playing,

        /// <summary>
        /// The animation was either manually restarted or repeated.
        /// </summary>
        Restarted,

        /// <summary>
        /// The animation was played and completed.
        /// </summary>
        Finished,

        /// <summary>
        /// The animation is now the current animation for an entity.
        /// </summary>
        Activated,

        /// <summary>
        /// The animation is no longer the current animation for an entity.
        /// </summary>
        Deactivated
    }
}
