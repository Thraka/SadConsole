using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Animates a collection of frames.
    /// </summary>
    [DataContract]
    public class AnimatedScreenSurface : ScreenSurface
    {
        [DataMember(Name = "AnimationDuration")]
        private float _animatedTime;
        private AnimationState _state;
        [DataMember(Name = "Width")]
        private int _width;
        [DataMember(Name = "Height")]
        private int _height;

        /// <summary>
        /// Raised when the <see cref="AnimationState"/> changes.
        /// </summary>
        public event EventHandler<AnimationStateChangedEventArgs> AnimationStateChanged;

        /// <summary>
        /// The frames of animation.
        /// </summary>
        /// <remarks>If this collection changes, <see cref="CurrentFrameIndexValue"/>, <see cref="UpdateFrameReferences"/>, and <see cref="TimePerFrame"/> should all be recalculated.</remarks>
        [DataMember]
        protected internal List<ICellSurface> FramesList = new List<ICellSurface>();

        /// <summary>
        /// Time counter for the animation
        /// </summary>
        protected double AddedTime;

        /// <summary>
        /// The current frame index being animated.
        /// </summary>
        [DataMember]
        protected int CurrentFrameIndexValue;

        /// <summary>
        /// How much time per animated frame should be used.
        /// </summary>
        protected float TimePerFrame;

        /// <summary>
        /// All frames of the animation.
        /// </summary>
        public ReadOnlyCollection<ICellSurface> Frames => FramesList.AsReadOnly();

        /// <summary>
        /// The total number of frames.
        /// </summary>
        public int FrameCount => FramesList.Count;

        /// <summary>
        /// Center of the animation used in positioning.
        /// </summary>
        [DataMember]
        public Point Center { get; set; }

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
        public float AnimationDuration
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
                if (value < 0 || value >= FramesList.Count)
                {
                    CurrentFrameIndexValue = 0;
                }
                else
                {
                    CurrentFrameIndexValue = value;
                }

                UpdateFrameReferences();
            }
        }

        /// <summary>
        /// Indicates the animation is empty.
        /// </summary>
        public bool IsEmpty => FramesList.Count == 0;

        /// <summary>
        /// The width of the animation.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// The height of the animation.
        /// </summary>
        public int Height => _height;

        /// <summary>
        /// Gets the name of this animation.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets the current frame being animated.
        /// </summary>
        public ICellSurface CurrentFrame => FramesList[CurrentFrameIndexValue];

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

        #region Constructors
        /// <summary>
        /// Creates a new animation with the specified name, width, and height.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="width">The width of each frame this animation will have.</param>
        /// <param name="height">The height of each frame this animation will have.</param>
        public AnimatedScreenSurface(string name, int width, int height) : base(width, height)
        {
            Name = name;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Creates a new animation with the specified name, width, and height.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="width">The width of each frame this animation will have.</param>
        /// <param name="height">The height of each frame this animation will have.</param>
        /// <param name="font">The font used with this animation.</param>
        /// <param name="fontSize">The size of the font.</param>
        public AnimatedScreenSurface(string name, int width, int height, IFont font, Point fontSize) : base(width, height)
        {
            Name = name;
            Font = font;
            FontSize = fontSize;
            _width = width;
            _height = height;
            UseMouse = false;
            UseKeyboard = false;
        }

        [JsonConstructor]
        private AnimatedScreenSurface(ICellSurface surface, IFont font = null, Point? fontSize = null):base(surface, font, fontSize) { }
        #endregion


        /// <summary>
        /// Updates the visible surface according to <see cref="CurrentFrameIndex"/>.
        /// </summary>
        protected void UpdateFrameReferences()
        {
            ICellSurface frame = FramesList[CurrentFrameIndexValue];
            //Surface.SetSurface(frame.Cells, _width, _height, _width, _height);
            //Surface.DefaultBackground = frame.DefaultBackground;
            //Surface.DefaultForeground = frame.DefaultForeground;
            //Surface.DefaultGlyph = frame.DefaultGlyph;
            Surface = frame;
            IsDirty = true;
        }

        /// <summary>
        /// Creates a new frame with the same dimensions as this entity and adds it to the Frames collection of the entity.
        /// </summary>
        /// <returns>The created frame.</returns>
        public ICellSurface CreateFrame()
        {
            if (FramesList == null)
            {
                FramesList = new List<ICellSurface>();
            }

            var frame = new CellSurface(Width, Height) { DefaultBackground = Surface.DefaultBackground, DefaultForeground = Surface.DefaultForeground, DefaultGlyph = Surface.DefaultGlyph };
            frame.Clear();
            FramesList.Add(frame);
            UpdateFrameReferences();
            return frame;
        }

        /// <summary>
        /// Calculates the time needed per frame for rendering.
        /// </summary>
        private void CalculateFrameDuration()
        {
            if (IsEmpty || _animatedTime == 0)
            {
                TimePerFrame = 0f;
            }
            else
            {
                TimePerFrame = _animatedTime / FramesList.Count;
            }
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
        /// Updates the animation frames and calls update on the base class.
        /// </summary>
        /// <param name="delta">The time that has elapsed since this method was last called.</param>
        public override void Update(TimeSpan delta)
        {
            if (IsPlaying && TimePerFrame != 0)
            {
                // TODO: Evaluate if we should change this to calculate current frame based on total time passed, \\not calculate frame based on individual frame duration on screen.
                AddedTime += delta.TotalSeconds;

                if (AddedTime > TimePerFrame)
                {
                    AddedTime = 0f;
                    CurrentFrameIndexValue++;

                    if (CurrentFrameIndexValue >= FramesList.Count)
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

                    UpdateFrameReferences();
                }
            }

            base.Update(delta);
        }

        /// <summary>
        /// Returns the name of the animation prefixed with "Animation - ".
        /// </summary>
        /// <returns>The name.</returns>
        public override string ToString() =>
            $"Animation - {Name}";

        /// <inheritdoc />
        public override void UpdateAbsolutePosition()
        {
            if (UsePixelPositioning)
                AbsolutePosition = Position - (FontSize * Center) + (Parent?.AbsolutePosition ?? new Point(0, 0));
            else
                AbsolutePosition = (FontSize * Position) - (FontSize * Center) + (Parent?.AbsolutePosition ?? new Point(0, 0));

            foreach (IScreenObject child in Children)
                child.UpdateAbsolutePosition();
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="context">Nothing.</param>
        [OnSerializing]
        protected void OnSerializingMethod2(StreamingContext context)
        {
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="context">Nothing.</param>
        [OnSerialized]
        protected void OnSerializedMethod(StreamingContext context)
        {
        }

        /// <summary>
        /// Calls <see cref="UpdateFrameReferences"/>.
        /// </summary>
        /// <param name="context">Nothing.</param>
        [OnDeserialized]
        protected void OnDeserializedMethod(StreamingContext context)
        {
            UpdateFrameReferences();
        }

        /// <summary>
        /// Creates an animated surface that looks like static.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="frames">How many frames the animation should have.</param>
        /// <param name="blankChance">Chance a character will be blank. Characters are between index 48-158. Chance is evaluated versus <see cref="System.Random.NextDouble"/>.</param>
        /// <param name="background">The background color of the animation. Defaults to transparent.</param>
        /// <param name="foreground">The foreground color of the animation. Defaults to white.</param>
        /// <returns>An animation.</returns>
        public static AnimatedScreenSurface CreateStatic(int width, int height, int frames, double blankChance, Color? background = null, Color? foreground = null)
        {
            var animation = new AnimatedScreenSurface("default", width, height);
            animation.Surface.DefaultBackground = background ?? Color.Black;

            var foregroundColor = foreground ?? Color.White;
            for (int f = 0; f < frames; f++)
            {
                ICellSurface frame = animation.CreateFrame();

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

            animation.AnimationDuration = 1;
            animation.Repeat = true;

            animation.Start();

            return animation;
        }

        /// <summary>
        /// Converts an image file containing frames to an instance of AnimatedScreenSurface.
        /// </summary>
        /// <param name="name">Name for the animation.</param>
        /// <param name="filePath">File path to the image file.</param>
        /// <param name="frameLayout">Layout of frames in the image file: X number of columns, Y number of rows.</param>
        /// <param name="pixelPadding">Amount of pixels in the image file separating frames: X between the columns, Y between the rows.</param>
        /// <param name="frameDuration">Duration for a frame in the animation.</param>
        /// <param name="font">Font to be used when creating an instance of the <see cref="AnimatedScreenSurface"/>.</param>
        /// <param name="callback">If specified, this will be applied to each <see cref="ColoredGlyph"/> when creating a frame.</param>
        /// <param name="firstFrame">If specified, animation will start at this frame (zero indexed).</param>
        /// <param name="lastFrame">If specified, animation will finish at this frame (zero indexed).</param>
        /// <returns>An instance of <see cref="AnimatedScreenSurface"/> with converted frames.</returns>
        /// <remarks>Remarks:<br></br>
        /// This method assumes the image file contains only frames and optional padding between the frames, no border space.<br></br>
        /// Number of frames is calculated by multiplying rows and columns from the frame layout or by specifying start and finish indexes.<br></br>
        /// Frame size and the subsequent AnimatedScreenSurface size is calculated from the size of the image file, count of frames, padding and the font size ratio.<br></br></remarks>
        public static AnimatedScreenSurface ConvertImageFile(string name, string filePath, Point frameLayout, Point pixelPadding, float frameDuration, IFont font, 
            Action<ColoredGlyph> callback = null, int firstFrame = 0, int lastFrame = 0)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"Invalid file path.");
            }

            if (font is null)
            {
                throw new ArgumentException("Font has to be specified.");
            }

            // get info about the font
            var fontSize = font.GetFontSize(IFont.Sizes.One);
            var fontSizeRatio = font.GetGlyphRatio(fontSize);

            // load the image
            using ITexture image = GameHost.Instance.GetTexture(filePath);
            Point imageSize = (image.Width, image.Height);

            // convert the image to surface
            Point surfaceSize = ApplyFontSizeRatio(imageSize, fontSizeRatio);
            var convertedImage = image.ToSurface(TextureConvertMode.Foreground, surfaceSize.X, surfaceSize.Y);
            pixelPadding = ApplyFontSizeRatio(pixelPadding, fontSizeRatio);

            // calculate the number of frames
            int totalFrameCount = frameLayout.X * frameLayout.Y,
                frameCount = lastFrame - firstFrame + 1;
            bool customFrameCountIsValid = lastFrame > 0 && frameCount > 0 && frameCount <= totalFrameCount && firstFrame >= 0 && lastFrame <= totalFrameCount;
            frameCount = customFrameCountIsValid ? frameCount : totalFrameCount;

            // calculate the frame size and create an instance of an animated screen surface
            Point frameSize = ((surfaceSize.X - (pixelPadding.X * (frameLayout.X - 1))) / frameLayout.X, (surfaceSize.Y - (pixelPadding.Y * (frameLayout.Y - 1))) / frameLayout.Y);
            var clip = new AnimatedScreenSurface(name, frameSize.X, frameSize.Y, font, fontSize)
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
                                {
                                    return;
                                }
                                else CopyFrameToClip();
                            }
                        }
                        else CopyFrameToClip();

                        // take into account padding between the frames, not outside them
                        int paddingX = x + 1 == frameLayout.X ? 0 : pixelPadding.X;
                        currentFrameArea = currentFrameArea.ChangeX(frameSize.X + paddingX);
                    }

                    // take into account padding between the frames, not outside them
                    int paddingY = y + 1 == frameLayout.Y ? 0 : pixelPadding.Y;
                    currentFrameArea = currentFrameArea.ChangeY(frameSize.Y + paddingY);
                }
            }

            void CopyFrameToClip()
            {
                CellSurface frame = (CellSurface)clip.CreateFrame();
                convertedImage.Copy(currentFrameArea, frame, 0, 0);
                if (callback != null) Array.ForEach(frame.Cells, callback);
            }

            Point ApplyFontSizeRatio(Point point, (float X, float Y) sizeRatio) =>
                (ApplySizeRatio(point.X, sizeRatio.X), ApplySizeRatio(point.Y, sizeRatio.Y));

            int ApplySizeRatio(int x, float sizeRatio, bool inverted = false) => !inverted ?
                sizeRatio < 1 ? x : (int)Math.Floor(x / sizeRatio) :
                sizeRatio > 1 ? (int)Math.Floor(x * sizeRatio) : x;
        }

        /// <summary>
        /// Saves the <see cref="AnimatedScreenSurface"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file) => Serializer.Save(this, file, Settings.SerializationIsCompressed);

        /// <summary>
        /// Loads a <see cref="AnimatedScreenSurface"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns>The animated surface.</returns>
        public static AnimatedScreenSurface Load(string file) => Serializer.Load<AnimatedScreenSurface>(file, Settings.SerializationIsCompressed);

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
}
