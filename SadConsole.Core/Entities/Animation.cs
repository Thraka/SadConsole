namespace SadConsole.Entities
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Animates a list of frames.
    /// </summary>
    [DataContract]
    public class Animation
    {
        #region Variables
        /// <summary>
        /// The list of frames for this animation
        /// </summary>
        protected List<Frame> _animatedFrames = new List<Frame>();

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
        protected Font _font;

        /// <summary>
        /// All frames of the animation
        /// </summary>
        [DataMember]
        public List<Frame> Frames = new List<Frame>();
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
                if (value < 0 || value >= _animatedFrames.Count)
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
            }
        }

        /// <summary>
        /// Indicates the animation is empty.
        /// </summary>
        public bool IsEmpty { get { return _animatedFrames.Count == 0; } }

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
        public Frame CurrentFrame
        {
            get { return _animatedFrames[_currentFrameIndex]; }
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
        public Frame CreateFrame()
        {
            if (Frames == null)
                Frames = new List<Frame>();

            var frame = new Frame(Width, Height);
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
                _timePerFrame = _animatedTime / _animatedFrames.Count;
        }

        /// <summary>
        /// After frames have been changed, use this method to commit those changes to the animation.
        /// </summary>
        /// <remarks>Calling this method also calls the Stop method and resets the current animation frame back to zero.</remarks>
        public void Commit()
        {
            Stop();
            _currentFrameIndex = 0;

            if (Frames == null)
                Frames = new List<Frame>();

            _animatedFrames = new List<Frame>(Frames);
            CalculateFrameDuration();
        }
        
        /// <summary>
        /// Stops animating.
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
        }

        /// <summary>
        /// Starts animating the frames.
        /// </summary>
        public void Start()
        {
            _isPlaying = true;
        }

        /// <summary>
        /// Restarts the animation from the first frame.
        /// </summary>
        public void Restart()
        {
            _isPlaying = true;
            _currentFrameIndex = 0;
        }

        /// <summary>
        /// Updates the animation frames based on the time passed since the last call to this method.
        /// </summary>
        public void Update()
        {
            if (_isPlaying && _timePerFrame != 0f)
            {
                _addedTime += Engine.GameTimeElapsedUpdate;

                if (_addedTime > _timePerFrame)
                {
                    _addedTime = 0f;
                    _currentFrameIndex++;

                    if (_currentFrameIndex >= _animatedFrames.Count)
                    {
                        if (Repeat)
                            _currentFrameIndex = 0;
                        else
                        {
                            Stop();
                            _currentFrameIndex--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resizes all frames in the animation to the specified width and height.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;

            foreach (var frame in _animatedFrames)
                frame.Resize(width, height);

            foreach (var frame in Frames)
                frame.Resize(width, height);
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
            Commit();
        }

        public void Save(string file)
        {
            SadConsole.Serializer.Save<Animation>(this, file);
        }

        public static Animation Load(string file)
        {
            return SadConsole.Serializer.Load<Animation>(file);
        }
    }
}
