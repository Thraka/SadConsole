namespace SadConsole.Entities
{
    using Consoles;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Animates a list of frames.
    /// </summary>
    [DataContract]
    public class Animation
    {
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
        [DataMember(Name="AnimationDuration")]
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
        protected Font _font = Engine.DefaultFont;

        /// <summary>
        /// All frames of the animation
        /// </summary>
        [DataMember]
        public List<TextSurface> Frames = new List<TextSurface>();

        /// <summary>
        /// The state of the animation.
        /// </summary>
        protected AnimationState state;
        

        #endregion

        #region Properties
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
            }
        }

        [DataMember]
        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;

                for (int i = 0; i < Frames.Count; i++)
                    Frames[i].Font = _font;
            }
        }

        /// <summary>
        /// Indicates the animation is empty.
        /// </summary>
        public bool IsEmpty { get { return Frames.Count == 0; } }

        /// <summary>
        /// Gets the name of this animation.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets the width of the animation frames.
        /// </summary>
        [DataMember]
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the animation frames.
        /// </summary>
        [DataMember]
        public int Height { get; private set; }

        /// <summary>
        /// Gets the currently frame being animated.
        /// </summary>
        public TextSurface CurrentFrame
        {
            get { return Frames[_currentFrameIndex]; }
        }

        /// <summary>
        /// Gets the current animation state.
        /// </summary>
        public AnimationState State
        {
            get { return state; }
            internal set
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
        /// <param name="width">The width of each frame this animation wil have.</param>
        /// <param name="height">The height of each frame this animation wil have.</param>
        public Animation(string name, int width, int height)
        {
            Width = width;
            Height = height;
            Name = name;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new frame with the same dimensions as this entity and adds it to the Frames collection of the entity.
        /// </summary>
        /// <returns>The created frame.</returns>
        public TextSurface CreateFrame()
        {
            if (Frames == null)
                Frames = new List<TextSurface>();

            var frame = new TextSurface(Width, Height, _font);
            Frames.Add(frame);
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
            _currentFrameIndex = 0;
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
                _addedTime += Engine.GameTimeElapsedUpdate;

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

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            AnimationDuration = _animatedTime;
        }

        public void Save(string file)
        {
            Serializer.Save(this, file, new System.Type[] { typeof(List<TextSurface>) });
        }

        public static Animation Load(string file)
        {
            return Serializer.Load<Animation>(file, new System.Type[] { typeof(List<TextSurface>) });
        }
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
