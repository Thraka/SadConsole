#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;

    /// <summary>
    /// Animates a list of frames.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Console (Animated)")]
    [JsonConverter(typeof(SerializedTypes.AnimatedConsoleConverterJson))]
    public class AnimatedConsole : Console
    {
        private float _animatedTime;
        private AnimationState _state;

        /// <summary>
        /// Raised when the <see cref="AnimationState"/> changes.
        /// </summary>
        public event EventHandler<AnimationStateChangedEventArgs> AnimationStateChanged;

        /// <summary>
        /// The frames of animation.
        /// </summary>
        /// <remarks>If this collection changes, <see cref="CurrentFrameIndexValue"/>, <see cref="UpdateFrameReferences"/>, and <see cref="TimePerFrame"/> should all be recalculated.</remarks>
        protected internal List<CellSurface> FramesList = new List<CellSurface>();

        /// <summary>
        /// Time counter for the animation
        /// </summary>
        protected double AddedTime;

        /// <summary>
        /// The current frame index being animated.
        /// </summary>
        protected int CurrentFrameIndexValue;

        /// <summary>
        /// How much time per animated frame should be used.
        /// </summary>
        protected float TimePerFrame;

        /// <summary>
        /// All frames of the animation
        /// </summary>
        public ReadOnlyCollection<CellSurface> Frames => FramesList.AsReadOnly();

        /// <summary>
        /// Center of the animation used in positioning.
        /// </summary>
        public Point Center { get; set; }

        /// <summary>
        /// Indicates whether or not this animation will repeat once it has finished animating.
        /// </summary>
        public bool Repeat { get; set; }

        /// <summary>
        /// When true, Indicates the animation is currently animating. The <see cref="Update"/> method will advance the frames.
        /// </summary>
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
        /// Gets the name of this animation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the currently frame being animated.
        /// </summary>
        public CellSurface CurrentFrame => FramesList[CurrentFrameIndexValue];

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
        public AnimatedConsole(string name, int width, int height) : this(name, width, height, Global.FontDefault)
        {
        }

        /// <summary>
        /// Creates a new animation with the specified name, width, and height.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="width">The width of each frame this animation will have.</param>
        /// <param name="height">The height of each frame this animation will have.</param>
        /// <param name="font">The font used with this animation.</param>
        public AnimatedConsole(string name, int width, int height, Font font) : base(width, height, font)
        {
            UseMouse = false;
            Name = name;
        }
        #endregion

        /// <summary>
        /// Forces the area of this text surface to always be the full width and height.
        /// </summary>
        public override void SetRenderCells()
        {
            base.SetRenderCells();

            if (FramesList.Count > 0)
            {
                UpdateFrameReferences();
            }
        }

        /// <summary>
        /// Updates the base <see cref="CellSurface.Cells"/> references to the current frame.
        /// </summary>
        protected void UpdateFrameReferences()
        {
            CellSurface frame = FramesList[CurrentFrameIndexValue];
            Cells = RenderCells = frame.Cells;
            DefaultBackground = frame.DefaultBackground;
            DefaultForeground = frame.DefaultForeground;
            IsDirty = true;
        }

        /// <summary>
        /// Creates a new frame with the same dimensions as this entity and adds it to the Frames collection of the entity.
        /// </summary>
        /// <returns>The created frame.</returns>
        public CellSurface CreateFrame()
        {
            if (FramesList == null)
            {
                FramesList = new List<CellSurface>();
            }

            var frame = new CellSurface(Width, Height) { DefaultBackground = DefaultBackground, DefaultForeground = DefaultForeground };
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
        /// <param name="timeElapsed">The time elapsed since the last call in seconds.</param>
        public override void Update(TimeSpan timeElapsed)
        {
            if (IsPlaying && TimePerFrame != 0)
            {
                // TODO: Evaluate if we should change this to calculate current frame based on total time passed, \\not calculate frame based on individual frame duration on screen.
                AddedTime += timeElapsed.TotalSeconds;

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

            base.Update(timeElapsed);
        }

        /// <summary>
        /// Returns the name of the animation.
        /// </summary>
        /// <returns>The name.</returns>
        public override string ToString() => Name;

        /// <inheritdoc />
        public override void OnCalculateRenderPosition()
        {
            if (UsePixelPositioning)
            {
                CalculatedPosition = Position - Center + (Parent?.CalculatedPosition ?? Point.Zero);
            }
            else
            {
                CalculatedPosition = Position.ConsoleLocationToPixel(Font) - Center.ConsoleLocationToPixel(Font) + (Parent?.CalculatedPosition ?? Point.Zero);
            }

            foreach (Console child in Children)
            {
                child.OnCalculateRenderPosition();
            }
        }

        /// <summary>
        /// Creates an animated surface that looks like static.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="frames">How many frames the animation should have.</param>
        /// <param name="blankChance">Chance a character will be blank. Characters are between index 48-158. Chance is evaluated versus <see cref="System.Random.NextDouble"/>.</param>
        /// <returns>An animation.</returns>
        public static AnimatedConsole CreateStatic(int width, int height, int frames, double blankChance)
        {
            var animation = new AnimatedConsole("default", width, height, Global.FontDefault)
            {
                DefaultBackground = Color.Black
            };
            for (int f = 0; f < frames; f++)
            {
                CellSurface frame = animation.CreateFrame();

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int character = Global.Random.Next(48, 168);

                        if (Global.Random.NextDouble() <= blankChance)
                        {
                            character = 32;
                        }

                        frame.SetGlyph(x, y, character);
                        frame.SetForeground(x, y, Color.White * (float)(Global.Random.NextDouble() * (1.0d - 0.5d) + 0.5d));
                    }
                }

            }

            animation.AnimationDuration = 1;
            animation.Repeat = true;

            animation.Start();

            return animation;
        }

        /// <summary>
        /// Saves the <see cref="AnimatedConsole"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public new void Save(string file) => Serializer.Save(this, file, Settings.SerializationIsCompressed);

        /// <summary>
        /// Loads a <see cref="AnimatedConsole"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static new AnimatedConsole Load(string file) => Serializer.Load<AnimatedConsole>(file, Settings.SerializationIsCompressed);

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
