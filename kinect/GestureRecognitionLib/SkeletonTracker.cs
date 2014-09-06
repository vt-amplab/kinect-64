//#define USE_PARALLEL_LOOP_FOR_SKELETONS

using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.Threading.Tasks;

namespace GestureRecognition
{
    public class SkeletonTracker
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SkeletonTracker));

        #region Public Default Constants
        public const int DefaultBufferLength = 32;
        public const int DefaultIgnoreFrames = 2;
        #endregion

        #region Private Member Variables
        private List<Dictionary<JointType, Point3D>>[] _buffer;
        private Skeleton[] _skeletonData;
        private int _maxBufferLength;
        private int _ignoreFrames;
        private List<int> _trackedSkeletons;
        private FramesPerSecondCounter _fpsCounter;
        #endregion

        #region Properties
        public int CurrentBufferSize
        {
            get
            {
                int size = 0;
                foreach (int skeleton in _trackedSkeletons)
                {
                    if (_buffer[skeleton].Count > size)
                    {
                        size = _buffer[skeleton].Count;
                    }
                }
                return size;
            }
        }
        public List<int> TrackedSkeletons
        {
            get
            {
                return new List<int>(_trackedSkeletons);
            }
        }

        public double CurrentFramesPerSecond
        {
            get
            {
                return _fpsCounter.CurrentFramesPerSecond;
            }
        }
        public double AverageFramesPerSecond
        {
            get
            {
                return _fpsCounter.AverageFramesPerSecond;
            }
        }
        #endregion

        #region Constructors
        public SkeletonTracker() : this(DefaultBufferLength, DefaultIgnoreFrames) { }

        public SkeletonTracker(int bufferLen, int ignore)
        {
            _buffer = null;
            _maxBufferLength = bufferLen;
            _ignoreFrames = ignore;
            _trackedSkeletons = new List<int>();
            _fpsCounter = new FramesPerSecondCounter();
        }
        #endregion

        #region Events
        public delegate void BufferLimitReachedHandler(SkeletonTracker sender, List<Dictionary<JointType, Point3D>> buffer);
        public event BufferLimitReachedHandler BufferLimitReachedEvent;

        public delegate void BufferUpdatedEventHandler(SkeletonTracker sender,
            List<Dictionary<JointType, Point3D>> buffer, int id, Point3D absolutePosition);
        public event BufferUpdatedEventHandler BufferUpdatedEvent;
        #endregion

        public void SkeletonFrameReadyHandler(object sender, SkeletonFrameReadyEventArgs args)
        {
            using (SkeletonFrame frame = args.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    Logger.Debug("New Skeleton Frame ready");
                    _fpsCounter.Update();

                    if (_fpsCounter.TotalFrameCount % _ignoreFrames == 0)
                    {
                        HandleNewSkeletonFrame(frame);
                    }
                }
            }
        }

        /// <summary>
        /// Clear all tracked skeleton buffers
        /// </summary>
        public void ClearAllBuffers()
        {
            _trackedSkeletons.Clear();
        }

        /// <summary>
        /// Clear an entire buffer
        /// </summary>
        /// <param name="id">The buffer to clear</param>
        public void ClearBuffer(int id)
        {
            if (_trackedSkeletons.Contains(id))
            {
                _trackedSkeletons.Remove(id);
            }
            else
            {
                Logger.Warn("Not currently tracking skeleton " + id + ". Cannot clear buffer");
            }
        }

        /// <summary>
        /// Clear a number of frames from the buffer for id starting from index 0
        /// </summary>
        /// <param name="id">Buffer id to clear</param>
        /// <param name="count">Number of frames to clear</param>
        public void ClearFramesFromBuffer(int id, int count)
        {
            if (_trackedSkeletons.Contains(id))
            {
                _buffer[id].RemoveRange(0, count);
            }
            else
            {
                Logger.Warn("Not currently tracking skeleton " + id + ". Cannot clear buffer");
            }
        }

        #region Private Helper Methods
        private void HandleNewSkeletonFrame(SkeletonFrame frame)
        {
            if (_skeletonData == null || _buffer == null)
            {
                Logger.Debug("Initializing buffers for Skeleton Frame");
                _buffer = new List<Dictionary<JointType, Point3D>>[frame.SkeletonArrayLength];
                _skeletonData = new Skeleton[frame.SkeletonArrayLength];
            }

            frame.CopySkeletonDataTo(_skeletonData);
            
#if USE_PARALLEL_LOOP_FOR_SKELETONS
            Parallel.For(0, _skeletonData.Length, i =>
#else
            for (int i = 0; i < _skeletonData.Length; i++)
#endif
            {
                if (_skeletonData[i].TrackingState == SkeletonTrackingState.Tracked)
                {
                    AddFrameToBuffer(i, _skeletonData[i]);
                }
                else
                {
                    Logger.Debug("Skeleton " + i + " went out of frame");
                    _trackedSkeletons.Remove(i);
                }
            }
#if USE_PARALLEL_LOOP_FOR_SKELETONS
            );
#endif
        }

        private void AddFrameToBuffer(int id, Skeleton data)
        {
            if (_buffer == null)
            {
                Logger.Fatal("Buffer not initialized -- most likely illegal call to this function");
            }

            if (!_trackedSkeletons.Contains(id))
            {
                Logger.Info("Initializing buffer for skeleton " + id);
                _buffer[id] = new List<Dictionary<JointType, Point3D>>();
                _trackedSkeletons.Add(id);
            }

            if (_buffer[id].Count > _maxBufferLength)
            {
                Logger.Debug("Buffer limit reached -- removing oldest observation");
                if (BufferLimitReachedEvent != null)
                {
                    BufferLimitReachedEvent(this, _buffer[id]);
                }
                _buffer[id].RemoveAt(0);
            }

            _buffer[id].Add(SkeletonNormalizer.normalize(data));

            if (BufferUpdatedEvent != null)
            {
                BufferUpdatedEvent(this, _buffer[id], id, data.Joints[JointType.ShoulderCenter].Position);
            }
        }
        #endregion

    }

    public class SkeletonNormalizer
    {
        public static readonly JointType[] Forearms = new JointType[] { 
                                        JointType.HandLeft, JointType.WristLeft, JointType.ElbowLeft, 
                                        JointType.ElbowRight, JointType.WristRight, JointType.HandRight 
                                    };
        public static readonly JointType[] RightForearm = new JointType[] { 
                                        JointType.ElbowRight, JointType.WristRight, JointType.HandRight 
                                    };
        public static readonly JointType[] LeftForearm = new JointType[] { 
                                        JointType.HandLeft, JointType.WristLeft, JointType.ElbowLeft };

        public static readonly JointType[] Arms = new JointType[] { 
                                        JointType.HandLeft, JointType.WristLeft, 
                                        JointType.ElbowLeft, JointType.ShoulderLeft,
                                        JointType.ShoulderRight, JointType.ElbowRight, 
                                        JointType.WristRight, JointType.HandRight 
                                    };
        public static readonly JointType[] RightArm = new JointType[] { 
                                        JointType.ShoulderRight, JointType.ElbowRight, 
                                        JointType.WristRight, JointType.HandRight 
                                    };
        public static readonly JointType[] LeftArm = new JointType[] { 
                                        JointType.HandLeft, JointType.WristLeft, 
                                        JointType.ElbowLeft, JointType.ShoulderLeft
                                    };

        public static readonly JointType[] All = Enum.GetValues(typeof(JointType)).Cast<JointType>().ToArray();

        public static Dictionary<JointType, Point3D> normalize(Skeleton skeleton)
        {
            return normalize(skeleton, true, All);
        }

        public static Dictionary<JointType, Point3D> normalize(Skeleton skeleton, params JointType[] jointsToKeep)
        {
            return normalize(skeleton, true, jointsToKeep);
        }

        public static Dictionary<JointType, Point3D> normalize(Skeleton skeleton, bool keepInferredJoints, params JointType[] jointsToKeep)
        {
            var normalized = new Dictionary<JointType, Point3D>();
            foreach (JointType type in jointsToKeep)
            {
                Joint j = skeleton.Joints[type];
                if (j.TrackingState == JointTrackingState.Tracked || (j.TrackingState == JointTrackingState.Inferred && keepInferredJoints))
                {
                    normalized[type] = skeleton.Joints[type].Position;
                }
            }

            Point3D leftShoulder = skeleton.Joints[JointType.ShoulderLeft].Position;
            Point3D rightShoulder = skeleton.Joints[JointType.ShoulderRight].Position;

            var center = (leftShoulder + rightShoulder) / 2;
            double shoulderDistance = Math.Sqrt(leftShoulder.DistanceSquared(rightShoulder));

            var keys = new List<JointType>(normalized.Keys);

            foreach (JointType type in keys)
            {
                normalized[type] -= center;
                normalized[type] /= shoulderDistance;
            }
            return normalized;
        }


    }
}
