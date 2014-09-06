using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using log4net;

namespace GestureRecognition
{
    public class GestureLearner
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GestureLearner));

        #region Private Member Variables
        private SkeletonTracker _tracker;
        private bool _capturing;
        #endregion

        #region Properties

        public bool IsCapturing
        {
            get
            {
                return _capturing;
            }
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
        public delegate void GestureCapturedEventHandler(GestureCapturedEventArgs args);
        public event GestureCapturedEventHandler GestureCaptured;
        #endregion

        #region Constructors
        public GestureLearner() : this(new SkeletonTracker()) { }
        public GestureLearner(SkeletonTracker tracker)
        {
            _capturing = false;
            _tracker = tracker;

            _tracker.BufferLimitReachedEvent += this.TrackerBufferFullHandler;
        }
        #endregion

        #region Public API
        public void StartCapture()
        {
            _tracker.ClearAllBuffers();
            Logger.Info("Gesture capture beginning");
            _capturing = true;
        }

        public void CancelCapture()
        {
            _tracker.ClearAllBuffers();
            _capturing = false;
        }
        #endregion

        #region Private Helper Functions
        private void TrackerBufferFullHandler(SkeletonTracker sender, List<Dictionary<JointType, Point3D>> buffer)
        {
            if (_capturing)
            {
                Logger.Info("Gesture capture finished (buffer full)");
                if (GestureCaptured != null)
                {
                    GestureCaptured(new GestureCapturedEventArgs(buffer));
                }
                sender.ClearAllBuffers();
                _capturing = false;
            }
        }
        #endregion
    }


    public class GestureCapturedEventArgs
    {
        public readonly List<Dictionary<JointType, Point3D>> Buffer;
        public GestureCapturedEventArgs(List<Dictionary<JointType, Point3D>> buffer)
        {
            Buffer = new List<Dictionary<JointType, Point3D>>(buffer);
        }
    }

}
