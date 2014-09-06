using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;

namespace GestureRecognition
{
    public class FramesPerSecondCounter
    {
        private Stopwatch _stopwatch;
        private long _totalFrames;
        private long _totalMilliseconds;
        private double _currentFPS;
        private double _averageFPS;

        public const int MaximumSamples = 100;

        private Queue<double> _buffer;

        public double CurrentFramesPerSecond
        {
            get
            {
                return _currentFPS;
            }
        }
        public double AverageFramesPerSecond
        {
            get
            {
                return _averageFPS;
            }
        }
        public double OverallFramesPerSecond
        {
            get
            {
                return _totalFrames / (_totalMilliseconds / 1000.0);
            }
        }

        public long TotalFrameCount
        {
            get
            {
                return _totalFrames;
            }
        }
        public long TotalMilliseconds
        {
            get
            {
                return _totalMilliseconds;
            }
        }

        public FramesPerSecondCounter()
        {
            _stopwatch = new Stopwatch();
            _buffer = new Queue<double>();
            _totalFrames = 0;
            _totalMilliseconds = 0;
            _currentFPS = 0;
            _averageFPS = 0;
        }

        public void Update()
        {
            _totalFrames++;
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                long deltaMilliseconds = _stopwatch.ElapsedMilliseconds;

                _currentFPS = 1.0 / (deltaMilliseconds / 1000.0);

                _buffer.Enqueue(_currentFPS);

                if (_buffer.Count > MaximumSamples)
                {
                    _buffer.Dequeue();
                    _averageFPS = _buffer.Average(i => i);
                }
                else
                {
                    _averageFPS = _currentFPS;
                }

                _totalMilliseconds += deltaMilliseconds;
            }
            _stopwatch.Restart();
        }
    }
}
