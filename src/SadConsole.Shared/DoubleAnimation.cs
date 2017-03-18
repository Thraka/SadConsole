using Microsoft.Xna.Framework;

using System;
using System.Runtime.Serialization;


namespace SadConsole
{
    [DataContract]
    public sealed class DoubleAnimation
    {
        [DataMember]
        TimeSpan _totalTimeAtStart;

        [DataMember(Name="EasingFunction")]
        EasingFunctions.EasingBase _easingFunction = new EasingFunctions.Linear();

        [DataMember]
        double _finishedValue;

        [DataMember]
        public bool IsFinished { get; private set; }

        [DataMember]
        public bool IsStarted { get; private set; }

        [DataMember]
        public double StartingValue { get; set; }

        public double CurrentValue
        {
            get
            {
                if (IsFinished)
                    return _finishedValue;

                double value = GetValueForDuration((Global.GameTimeUpdate.TotalGameTime - _totalTimeAtStart).TotalMilliseconds);

                if (CheckEnd())
                    _finishedValue = value;

                return value;
            }
        }

        [DataMember]
        public double EndingValue { get; set; }

        [DataMember]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// An easing method to apply to the value. The parameters passed in are: calculated value, starting value, ending value, and duration.
        /// </summary>
        public EasingFunctions.EasingBase EasingFunction
        {
            get { return _easingFunction; }
            set { _easingFunction = value; if (_easingFunction == null) _easingFunction = new EasingFunctions.Linear(); }
        }

        public double GetValueForDuration(double time)
        {
            double timeDuration = Duration.TotalMilliseconds;
            if (time > timeDuration)
                time = timeDuration;

            return _easingFunction.Ease(time, StartingValue, EndingValue - StartingValue, timeDuration);
        }

        public void Start()
        {
            _totalTimeAtStart = Global.GameTimeUpdate.TotalGameTime;
            IsFinished = false;
            IsStarted = true;
        }

        private bool CheckEnd()
        {
            double value = (Global.GameTimeUpdate.TotalGameTime - _totalTimeAtStart).TotalMilliseconds;
            if (value >= Duration.TotalMilliseconds)
            {
                IsFinished = true;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            IsStarted = false;
            IsFinished = false;
        }
    }
}
