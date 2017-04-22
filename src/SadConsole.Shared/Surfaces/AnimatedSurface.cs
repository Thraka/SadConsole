using Microsoft.Xna.Framework;

using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace SadConsole.Surfaces
{
    /// <summary>
    /// Animates a list of frames.
    /// </summary>
    public class AnimatedSurface: BasicSurface
    {
        /// <summary>
        /// Raised when the <see cref="AnimationState"/> changes.
        /// </summary>
        public event System.EventHandler<AnimationStateChangedEventArgs> AnimationStateChanged;

        #region Variables
        /// <summary>
        /// Time counter for the naimation
        /// </summary>
        protected double _addedTime;
        
        /// <summary>
        /// The current frame index being animated.
        /// </summary>
        protected int _currentFrameIndex;

        /// <summary>
        /// The length of the animation
        /// </summary>
        protected float _animatedTime;

        /// <summary>
        /// How much time per animated frame should be used.
        /// </summary>
        protected float _timePerFrame;

        /// <summary>
        /// Indicates the animation is currently animating.
        /// </summary>
        protected bool _isPlaying;

        /// <summary>
        /// The font to use with all frames.
        /// </summary>
        protected Font _font = Global.FontDefault;

        /// <summary>
        /// All frames of the animation
        /// </summary>
        public List<NoDrawSurface> Frames = new List<NoDrawSurface>();

        /// <summary>
        /// The state of the animation.
        /// </summary>
        protected AnimationState state;
        

        #endregion

        #region Properties
        /// <summary>
        /// Center of the animation used in positioning.
        /// </summary>
        public Point Center { get; set; }

        /// <summary>
        /// Indicates whether or not this animation will repeat once it has finished animating.
        /// </summary>
        public bool Repeat { get; set; }

        /// <summary>
        /// When true, the <see cref="Update"/> method will advance the frames.
        /// </summary>
        public bool IsPlaying { get { return _isPlaying; } }

        /// <summary>
        /// The length of the animation.
        /// </summary>
        public float AnimationDuration
        {
            get { return _animatedTime; }
            set { _animatedTime = value; CalculateFrameDuration(); }
        }

        /// <summary>
        /// Gets or sets the current frame index to animate.
        /// </summary>
        public int CurrentFrameIndex
        {
            get { return _currentFrameIndex; }
            set
            {
                if (value < 0 || value >= Frames.Count)
                    _currentFrameIndex = 0;
                else
                    _currentFrameIndex = value;

                UpdateFrameReferences();
            }
        }
        
        /// <summary>
        /// Indicates the animation is empty.
        /// </summary>
        public bool IsEmpty { get { return Frames.Count == 0; } }

        /// <summary>
        /// Gets the name of this animation.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets the currently frame being animated.
        /// </summary>
        public NoDrawSurface CurrentFrame
        {
            get { return Frames[_currentFrameIndex]; }
        }

        /// <summary>
        /// Gets the current animation state.
        /// </summary>
        public AnimationState State
        {
            get { return state; }
            set
            {
                var oldState = state;

                if (value != state)
                {
                    state = value;
                    AnimationStateChanged?.Invoke(this, new AnimationStateChangedEventArgs(oldState, state));
                }
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new animation with the specified name, width, and height.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="width">The width of each frame this animation will have.</param>
        /// <param name="height">The height of each frame this animation will have.</param>
        public AnimatedSurface(string name, int width, int height) : this(name, width, height, Global.FontDefault)
        {
        }

        /// <summary>
        /// Creates a new animation with the specified name, width, and height.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="width">The width of each frame this animation will have.</param>
        /// <param name="height">The height of each frame this animation will have.</param>
        /// <param name="font">The font used with this animation.</param>
        public AnimatedSurface(string name, int width, int height, Font font): base(width, height, font)
        {
            Name = name;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Forces the area of this text surface to always be the full width and height.
        /// </summary>
        protected override void ResetArea()
        {
            // Sub views are not allowed on animated text surfaces. Enforce full view.
            area = new Rectangle(0, 0, width, height);

            base.ResetArea();

            if (Frames.Count > 0)
                UpdateFrameReferences();
        }

        /// <summary>
        /// Updates the base <see cref="TextSurface"/> render references to the current frame.
        /// </summary>
        protected void UpdateFrameReferences()
        {
            var frame = Frames[_currentFrameIndex];
            cells = RenderCells = frame.Cells;
            DefaultBackground = frame.DefaultBackground;
            DefaultForeground = frame.DefaultForeground;
            IsDirty = true;
        }

        /// <summary>
        /// Creates a new frame with the same dimensions as this entity and adds it to the Frames collection of the entity.
        /// </summary>
        /// <returns>The created frame.</returns>
        public NoDrawSurface CreateFrame()
        {
            if (Frames == null)
                Frames = new List<NoDrawSurface>();

            var frame = new NoDrawSurface(Width, Height);
            new SurfaceEditor(frame).Clear();
            Frames.Add(frame);
            UpdateFrameReferences();
            return frame;
        }

        /// <summary>
        /// Calculates the time needed per frame for rendering.
        /// </summary>
        private void CalculateFrameDuration()
        {
            if (IsEmpty || _animatedTime == 0f)
                _timePerFrame = 0f;
            else
                _timePerFrame = _animatedTime / Frames.Count;
        }
        
        /// <summary>
        /// Stops animating.
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            State = AnimationState.Stopped;
        }

        /// <summary>
        /// Starts animating the frames.
        /// </summary>
        public void Start()
        {
            CalculateFrameDuration();
            _isPlaying = true;
            State = AnimationState.Playing;
        }

        /// <summary>
        /// Restarts the animation from the first frame.
        /// </summary>
        public void Restart()
        {
            CalculateFrameDuration();
            _isPlaying = true;
            CurrentFrameIndex = 0;
            State = AnimationState.Restarted;
            State = AnimationState.Playing;
        }

        /// <summary>
        /// Updates the animation frames based on the time passed since the last call to this method.
        /// </summary>
        public void Update()
        {
            if (_isPlaying && _timePerFrame != 0f)
            {
                // TODO: Evaluate if we should change this to calculate current frame based on total time passed,
                // not calculate frame based on individual frame duration on screen.
                _addedTime += Global.GameTimeElapsedUpdate;

                if (_addedTime > _timePerFrame)
                {
                    _addedTime = 0f;
                    _currentFrameIndex++;

                    if (_currentFrameIndex >= Frames.Count)
                    {
                        if (Repeat)
                        {
                            _currentFrameIndex = 0;
                            State = AnimationState.Restarted;
                            State = AnimationState.Playing;
                        }
                        else
                        {
                            _isPlaying = false;
                            _currentFrameIndex--;
                            State = AnimationState.Finished;
                        }
                    }

                    UpdateFrameReferences();
                }
            }
        }

        /// <summary>
        /// Returns the name of the animation.
        /// </summary>
        /// <returns>The name.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        /// <summary>
        /// Saves the animated text surface to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file)
        {
            Serializer.Save((SerializedTypes.AnimatedSurfaceSerialized)this, file);
        }

        /// <summary>
        /// Loads an animated text surface from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static AnimatedSurface Load(string file)
        {
            return Serializer.Load<SerializedTypes.AnimatedSurfaceSerialized>(file);
        }


        /// <summary>
        /// Event args for when the animation state changes
        /// </summary>
        public class AnimationStateChangedEventArgs : System.EventArgs
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
