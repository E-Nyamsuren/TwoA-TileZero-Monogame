namespace MonoGame1
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A frame counter.
    /// 
    /// See http://stackoverflow.com/questions/20676185/xna-monogame-getting-the-frames-per-second
    /// 
    /// This work had no explicit license specified.
    /// </summary>
    public class FrameCounter
    {
        /// <summary>
        /// Initializes a new instance of the MonoGame1.FrameCounter class.
        /// </summary>
        public FrameCounter()
        {
        }

        /// <summary>
        /// Gets the total number of frames.
        /// </summary>
        ///
        /// <value>
        /// The total number of frames.
        /// </value>
        public long TotalFrames
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the total number of seconds.
        /// </summary>
        ///
        /// <value>
        /// The total number of seconds.
        /// </value>
        public float TotalSeconds
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the average frames per second.
        /// </summary>
        ///
        /// <value>
        /// The average frames per second.
        /// </value>
        public float AverageFramesPerSecond
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current frames per second.
        /// </summary>
        ///
        /// <value>
        /// The current frames per second.
        /// </value>
        public float CurrentFramesPerSecond
        {
            get;
            private set;
        }

        /// <summary>
        /// The maximum samples.
        /// </summary>
        public const int MAXIMUM_SAMPLES = 100;

        private Queue<float> _sampleBuffer = new Queue<float>();

        /// <summary>
        /// Updates the given deltaTime.
        /// </summary>
        ///
        /// <param name="deltaTime"> The delta time. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool Update(float deltaTime)
        {
            CurrentFramesPerSecond = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
            return true;
        }
    }
}
