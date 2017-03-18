namespace SadConsole.Instructions
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an instruction to pause for a specified duration.
    /// </summary>
    [DataContract]
    public class Wait: InstructionBase
    {
        #region Public Settings
        /// <summary>
        /// The duration of the wait.
        /// </summary>
        [DataMember]
        public float Duration { get; set; }
        #endregion

        private double _lastUpdateTime = -1d;

        public override void Run()
        {
            if (_lastUpdateTime == -1d)
                _lastUpdateTime = 0d;
            else
                _lastUpdateTime += Global.GameTimeElapsedUpdate;

            if (_lastUpdateTime > Duration)
                IsFinished = true;

            base.Run();
        }

        public override void Reset()
        {
            _lastUpdateTime = -1d;

            base.Reset();

        }

        public override void Repeat()
        {
            _lastUpdateTime = 0d;

            base.Repeat();
        }
    }
}
