//#define USE_PARALLEL_LOOP_FOR_GESTURES

using log4net;
using Microsoft.Kinect;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace GestureRecognition
{
    /// <summary>
    /// Dynamic Time Warping sequence comparison class. Used as a gesture recognizer,
    /// but can be used for any vector comparison. Inspired by Rhemyst's implementation called
    /// DtwGestureRecognizer.
    /// </summary>
    public class GestureRecognizer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GestureRecognizer));

        public const int DefaultMinimumFrames = 6;

        #region Private Member Variables
        private readonly double _cullingThreshold;
        private readonly double _globalThreshold;
        private int _minFrames;

        private Dictionary<Guid, Gesture> _gestures;

        private bool _recognizing;
        private SkeletonTracker _tracker;

        #endregion

        #region Properties
        public bool IsRecognizing
        {
            get { return _recognizing; }
        }

        public int CurrentBufferSize
        {
            get
            {
                return _tracker.CurrentBufferSize;
            }
        }

        public double CurrentFPS
        {
            get
            {
                return _tracker.CurrentFramesPerSecond;
            }
        }

        public double AverageFPS
        {
            get
            {
                return _tracker.AverageFramesPerSecond;
            }
        }

        public System.EventHandler<Microsoft.Kinect.SkeletonFrameReadyEventArgs> FrameReadyHandler
        {
            get
            {
                return _tracker.SkeletonFrameReadyHandler;
            }
        }

        #endregion

        #region Events
        public delegate void GestureRecognizedEventHandler(GestureRecognizedEventArgs args);
        public event GestureRecognizedEventHandler GestureRecognized;
        #endregion

        #region Constructors

        public GestureRecognizer(double globalThreshold, double cullingThreshold) : this(globalThreshold, cullingThreshold, DefaultMinimumFrames, new SkeletonTracker()) { }
        public GestureRecognizer(double globalThreshold, double cullingThreshold, SkeletonTracker tracker) : this(globalThreshold, cullingThreshold, DefaultMinimumFrames, tracker) { }
        public GestureRecognizer(double globalThreshold, double cullingThreshold, int minFrames) : this(globalThreshold, cullingThreshold, minFrames, new SkeletonTracker()) { }

        public GestureRecognizer(double globalThreshold, double cullingThreshold, int minFrames, SkeletonTracker tracker)
        {
            this._gestures = new Dictionary<Guid, Gesture>();

            _globalThreshold = globalThreshold;
            _cullingThreshold = cullingThreshold;
            _minFrames = minFrames;

            _recognizing = false;

            _tracker = tracker;

            _tracker.BufferUpdatedEvent += this.TrackerBufferUpdatedHandler;
        }
        #endregion

        #region Public API

        public void ClearAllGestures()
        {
            _gestures.Clear();
        }

        public void StartRecognizing()
        {
            _tracker.ClearAllBuffers();
            Logger.Info("Beginning to recognize gestures");
            _recognizing = true;
        }

        public void StopRecognizing()
        {
            Logger.Info("Ending gesture recognition");
            _recognizing = false;
        }

        public void AddOrUpdateGesture(Gesture gesture)
        {
            _gestures[gesture.ID] = gesture;
            Logger.Info("Gesture successfully added to dictionary");
        }

        public string RetrieveText()
        {
            StringBuilder builder = new StringBuilder();

            foreach (Gesture gesture in _gestures.Values)
            {
                builder.AppendLine(gesture.Label);
                foreach (Dictionary<JointType, Point3D> observation in gesture.Sequence)
                {
                    foreach (JointType joint in observation.Keys)
                    {
                        builder.Append(joint.ToString());
                        builder.Append(":");
                        builder.AppendLine(observation[joint].ToString());
                    }
                    builder.AppendLine("~");
                }

                builder.AppendLine("----");
            }

            return builder.ToString();
        }

        #region Persistence Functions
        public void PersistGesturesInBinary(Stream FileStream)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            Logger.Debug("Attempting to persist data to FileStream");
            serializer.Serialize(FileStream, _gestures);
            Logger.Debug("Sucessfully persisted information to FileStream");
        }

        public bool RebuildGesturesFromBinary(Stream FileStream)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            try
            {
                _gestures = (Dictionary<Guid, Gesture>)serializer.Deserialize(FileStream);
                Logger.Info("Sucessfully rebuilt gesture library from FileStream");
                return true;
            }
            catch (SerializationException exception)
            {
                Logger.Warn("Exception while deserializing FileStream", exception);
            }
            return false;
        }
        #endregion
        #endregion

        #region Private Helper Functions
        private void TrackerBufferUpdatedHandler(SkeletonTracker sender,
            List<Dictionary<JointType, Point3D>> buffer, int id, Point3D absolutePosition)
        {
            if (_recognizing && buffer.Count > _minFrames)
            {
                if (Recognize(buffer, absolutePosition))
                {
                    sender.ClearFramesFromBuffer(id, buffer.Count - _minFrames);
                }
            }
        }

        private bool Recognize(List<Dictionary<JointType, Point3D>> buffer, Point3D absolutePosition)
        {
            bool recognized = false;
#if USE_PARALLEL_LOOP_FOR_GESTURES
            Parallel.ForEach(_gestures.Values, gesture =>
#else
            foreach (Gesture gesture in _gestures.Values)
#endif
            {
                if (gesture.EuclideanDistanceForEndingSamples(buffer) < _cullingThreshold)
                {
                    double distance = gesture.DynamicTimeWarpDistance(buffer);
                    if (distance < _globalThreshold)
                    {
                        Logger.Debug("Gesture Recognized: " + gesture.Label);
                        if (GestureRecognized != null)
                        {
                            GestureRecognized(new GestureRecognizedEventArgs(gesture, absolutePosition));
                            recognized = true;
                        }
                    }
                }
            }
#if USE_PARALLEL_LOOP_FOR_GESTURES
);
#endif
            return recognized;
        }
        #endregion
    }

    public class GestureRecognizedEventArgs
    {
        public readonly Gesture Gesture;
        public readonly Point3D AbsPosition;

        public GestureRecognizedEventArgs(Gesture gesture, Point3D position)
        {
            this.Gesture = gesture;
            this.AbsPosition = position;
        }
    }
}
