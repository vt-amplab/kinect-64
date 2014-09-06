//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Rhemyst and Rymix">
//     Open Source. Do with this as you will. Include this statement or 
//     don't - whatever you like.
//
//     No warranty or support given. No guarantees this will work or meet
//     your needs. Some elements of this project have been tailored to
//     the authors' needs and therefore don't necessarily follow best
//     practice. Subsequent releases of this project will (probably) not
//     be compatible with different versions, so whatever you do, don't
//     overwrite your implementation with any new releases of this
//     project!
//
//     Enjoy working with Kinect!
// </copyright>
//-----------------------------------------------------------------------

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace DTWGestureRecognition
{
    using Microsoft.Kinect;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using GestureRecognition;
    using log4net;
    using log4net.Config;
    using System.Linq;
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MainWindow));

        static MainWindow()
        {
            XmlConfigurator.Configure();
        }

        // We want to control how depth data gets converted into false-color data
        // for more intuitive visualization, so we keep 32-bit color frame buffer versions of
        // these, to be updated whenever we receive and process a 16-bit frame.

        /// <summary>
        /// The red index
        /// </summary>
        private const int RedIdx = 2;

        /// <summary>
        /// The green index
        /// </summary>
        private const int GreenIdx = 1;

        /// <summary>
        /// The blue index
        /// </summary>
        private const int BlueIdx = 0;

        /// <summary>
        /// The minumum number of frames in the _video buffer before we attempt to start matching gestures
        /// </summary>
        private const int CaptureCountdownSeconds = 3;

        /// <summary>
        /// Where we will save our gestures to. The app will append a data/time and .txt to this string
        /// </summary>
        private const string GestureSaveFileLocation = @".\";

        /// <summary>
        /// Where we will save our gestures to. The app will append a data/time and .txt to this string
        /// </summary>
        private const string GestureSaveFileNamePrefix = @"RecordedGestures";

        private int gestureCount = 0;

        /// <summary>
        /// Dictionary of all the joints Kinect SDK is capable of tracking. You might not want always to use them all but they are included here for thouroughness.
        /// </summary>
        private readonly Dictionary<JointType, Brush> _jointColors = new Dictionary<JointType, Brush>
        { 
            {JointType.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointType.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointType.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointType.Head, new SolidColorBrush(Color.FromRgb(200, 0, 0))},
            {JointType.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79, 84, 33))},
            {JointType.ElbowLeft, new SolidColorBrush(Color.FromRgb(84, 33, 42))},
            {JointType.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointType.HandLeft, new SolidColorBrush(Color.FromRgb(215, 86, 0))},
            {JointType.ShoulderRight, new SolidColorBrush(Color.FromRgb(33, 79,  84))},
            {JointType.ElbowRight, new SolidColorBrush(Color.FromRgb(33, 33, 84))},
            {JointType.WristRight, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
            {JointType.HandRight, new SolidColorBrush(Color.FromRgb(37,  69, 243))},
            {JointType.HipLeft, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
            {JointType.KneeLeft, new SolidColorBrush(Color.FromRgb(69, 33, 84))},
            {JointType.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointType.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointType.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointType.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222, 76))},
            {JointType.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointType.FootRight, new SolidColorBrush(Color.FromRgb(77, 109, 243))}
        };

        /// <summary>
        /// The depth frame byte array. Only supports 320 * 240 at this time
        /// </summary>
        private readonly byte[] _depthFrame32 = new byte[640 * 480 * 4];

        private GestureRecognizer recognizer;
        private GestureLearner learner;

        /// <summary>
        /// The kinect sensor
        /// </summary>
        private KinectSensor kinectSensor;

        /// <summary>
        /// ArrayList of coordinates which are recorded in sequence to define one gesture
        /// </summary>
        private DateTime _captureCountdown = DateTime.Now;

        /// <summary>
        /// ArrayList of coordinates which are recorded in sequence to define one gesture
        /// </summary>
        private Timer _captureCountdownTimer;

        /// <summary>
        /// Initializes a new instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame that displays different players in different colors
        /// </summary>
        /// <param name="depthFrame16">The depth frame byte array</param>
        /// <returns>A depth frame byte array containing a player image</returns>
        private byte[] ConvertDepthFrame(DepthImagePixel[] depthFrame16)
        {
            for (int i16 = 0, i32 = 0; i16 < depthFrame16.Length && i32 < _depthFrame32.Length; i16 += 2, i32 += 4)
            {
                int player = depthFrame16[i16].PlayerIndex;
                int realDepth = depthFrame16[i16].Depth;
                
                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                var intensity = (byte)(255 - (255 * realDepth / 0x0fff));

                _depthFrame32[i32 + RedIdx] = 0;
                _depthFrame32[i32 + GreenIdx] = 0;
                _depthFrame32[i32 + BlueIdx] = 0;

                // choose different display colors based on player
                switch (player)
                {
                    case 0:
                        _depthFrame32[i32 + RedIdx] = (byte)(intensity / 2);
                        _depthFrame32[i32 + GreenIdx] = (byte)(intensity / 2);
                        _depthFrame32[i32 + BlueIdx] = (byte)(intensity / 2);
                        break;
                    case 1:
                        _depthFrame32[i32 + RedIdx] = intensity;
                        break;
                    case 2:
                        _depthFrame32[i32 + GreenIdx] = intensity;
                        break;
                    case 3:
                        _depthFrame32[i32 + RedIdx] = (byte)(intensity / 4);
                        _depthFrame32[i32 + GreenIdx] = intensity;
                        _depthFrame32[i32 + BlueIdx] = intensity;
                        break;
                    case 4:
                        _depthFrame32[i32 + RedIdx] = intensity;
                        _depthFrame32[i32 + GreenIdx] = intensity;
                        _depthFrame32[i32 + BlueIdx] = (byte)(intensity / 4);
                        break;
                    case 5:
                        _depthFrame32[i32 + RedIdx] = intensity;
                        _depthFrame32[i32 + GreenIdx] = (byte)(intensity / 4);
                        _depthFrame32[i32 + BlueIdx] = intensity;
                        break;
                    case 6:
                        _depthFrame32[i32 + RedIdx] = (byte)(intensity / 2);
                        _depthFrame32[i32 + GreenIdx] = (byte)(intensity / 2);
                        _depthFrame32[i32 + BlueIdx] = intensity;
                        break;
                    case 7:
                        _depthFrame32[i32 + RedIdx] = (byte)(255 - intensity);
                        _depthFrame32[i32 + GreenIdx] = (byte)(255 - intensity);
                        _depthFrame32[i32 + BlueIdx] = (byte)(255 - intensity);
                        break;
                }
            }

            return _depthFrame32;
        }

        /// <summary>
        /// Called when each depth frame is ready
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Image Frame Ready Event Args</param>
        private void NuiDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    DepthImagePixel[] image = new DepthImagePixel[depthFrame.PixelDataLength];
                    depthFrame.CopyDepthImagePixelDataTo(image);

                    byte[] convertedDepthFrame = ConvertDepthFrame(image);

                    depthImage.Source = BitmapSource.Create(
                        depthFrame.Width, depthFrame.Height, 96, 96, PixelFormats.Bgr32, null, convertedDepthFrame, depthFrame.Width * 4);

                    if (recognizer.IsRecognizing)
                    {
                        frameRate.Text = recognizer.AverageFPS.ToString("N2") + " fps";
                    }
                    else if (learner.IsCapturing)
                    {
                        frameRate.Text = learner.AverageFPS.ToString("N2") + " fps";
                    }
                }
            }
        }

        /// <summary>
        /// Gets the display position (i.e. where in the display image) of a Joint
        /// </summary>
        /// <param name="joint">Kinect NUI Joint</param>
        /// <returns>Point mapped location of sent joint</returns>
        private Point GetDisplayPosition(Joint joint)
        {
            float depthX, depthY;
            DepthImagePoint depthPoint = kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(joint.Position, kinectSensor.DepthStream.Format);

            depthX = depthPoint.X;
            depthY = depthPoint.Y;

            depthX = Math.Max(0, Math.Min(depthX * kinectSensor.DepthStream.FrameWidth, kinectSensor.DepthStream.FrameWidth));
            depthY = Math.Max(0, Math.Min(depthY * kinectSensor.DepthStream.FrameHeight, kinectSensor.DepthStream.FrameHeight));

            int colorX, colorY;
            ColorImagePoint colorPoint = kinectSensor.CoordinateMapper.MapDepthPointToColorPoint(kinectSensor.DepthStream.Format, depthPoint, kinectSensor.ColorStream.Format);
            colorX = colorPoint.X;
            colorY = colorPoint.Y;

            return new Point((int)(skeletonCanvas.Width * colorX / 640.0), (int)(skeletonCanvas.Height * colorY / 480));
        }

        /// <summary>
        /// Works out how to draw a line ('bone') for sent Joints
        /// </summary>
        /// <param name="joints">Kinect NUI Joints</param>
        /// <param name="brush">The brush we'll use to colour the joints</param>
        /// <param name="ids">The JointsIDs we're interested in</param>
        /// <returns>A line or lines</returns>
        private Polyline GetBodySegment(JointCollection joints, Brush brush, params JointType[] ids)
        {
            var points = new PointCollection(ids.Length);
            foreach (JointType t in ids)
            {
                points.Add(GetDisplayPosition(joints[t]));
            }

            var polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }

        /// <summary>
        /// Runds every time a skeleton frame is ready. Updates the skeleton canvas with new joint and polyline locations.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Skeleton Frame Event Args</param>
        private void NuiSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            currentBufferFrame.Text = recognizer.CurrentBufferSize.ToString();
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletonData);

                    int iSkeleton = 0;
                    var brushes = new Brush[6];
                    brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
                    brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
                    brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
                    brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

                    skeletonCanvas.Children.Clear();
                    foreach (Skeleton data in skeletonData)
                    {
                        if (SkeletonTrackingState.Tracked == data.TrackingState)
                        {
                            // Draw bones
                            Brush brush = brushes[iSkeleton % brushes.Length];
                            skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointType.HipCenter, JointType.Spine, JointType.ShoulderCenter, JointType.Head));
                            skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft));
                            skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointType.ShoulderCenter, JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight));
                            skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointType.HipCenter, JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft, JointType.FootLeft));
                            skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointType.HipCenter, JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight));

                            // Draw joints
                            foreach (Joint joint in data.Joints)
                            {
                                Point jointPos = GetDisplayPosition(joint);
                                var jointLine = new Line();
                                jointLine.X1 = jointPos.X - 3;
                                jointLine.X2 = jointLine.X1 + 6;
                                jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                                jointLine.Stroke = _jointColors[joint.JointType];
                                if (joint.TrackingState != JointTrackingState.Tracked)
                                {
                                    jointLine.Stroke.Opacity = .5;
                                }
                                jointLine.StrokeThickness = 6;
                                skeletonCanvas.Children.Add(jointLine);
                            }
                        }

                        iSkeleton++;
                    } // for each skeleton
                }
            }
        }

        /// <summary>
        /// Called every time a video (RGB) frame is ready
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Image Frame Ready Event Args</param>
        private void NuiColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            // 32-bit per pixel, RGBA image
            using (ColorImageFrame image = e.OpenColorImageFrame())
            {
                if (image != null)
                {
                    videoImage.Source = BitmapSource.Create(
                        image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null, image.GetRawPixelData(), image.Width * image.BytesPerPixel);
                }
            }
        }

        private void GestureRecognized(GestureRecognizedEventArgs args)
        {
            gestureCount++;
            results.Text = "Recognised as: " + args.Gesture.Label + " (" + gestureCount + ")";
        }

        /// <summary>
        /// Runs after the window is loaded
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Routed Event Args</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

            recognizer = new GestureRecognizer(.9, 3);
            recognizer.GestureRecognized += this.GestureRecognized;

            learner = new GestureLearner();
            learner.GestureCaptured += this.GestureCaptured;

            foreach (var potential in KinectSensor.KinectSensors)
            {
                if (potential.Status == KinectStatus.Connected)
                {
                    this.kinectSensor = potential;
                }
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.SkeletonStream.Enable();

                this.kinectSensor.SkeletonFrameReady += recognizer.FrameReadyHandler;
                this.kinectSensor.SkeletonFrameReady += learner.FrameReadyHandler;
                this.kinectSensor.SkeletonFrameReady += this.NuiSkeletonFrameReady;


                this.kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                this.kinectSensor.DepthFrameReady += this.NuiDepthFrameReady;

                this.kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                this.kinectSensor.ColorFrameReady += this.NuiColorFrameReady;

            }
            else
            {
                Logger.Warn("Kinect sensor is null");
            }

            // Update the debug window with Sequences information
            dtwTextOutput.Text = recognizer.RetrieveText();

            try
            {
                this.kinectSensor.Start();
                Logger.Debug("Started kinect sensor");
            }
            catch (IOException exception)
            {
                this.kinectSensor = null;
                Logger.Debug("Failed starting kinect sensor", exception);
            }

            Logger.Info("Finished loading window");

            recognizer.StartRecognizing();
        }

        /// <summary>
        /// Runs some tidy-up code when the window is closed. This is especially important for our NUI instance because the Kinect SDK is very picky about this having been disposed of nicely.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Event Args</param>
        private void WindowClosed(object sender, EventArgs e)
        {
            Logger.Debug("Stopping kinect sensor");
            this.kinectSensor.Stop();
            Logger.Debug("Kinect sensor stopped");
            Environment.Exit(0);
        }

        /// <summary>
        /// Read mode. Sets our control variables and button enabled states
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Routed Event Args</param>
        private void DtwReadClick(object sender, RoutedEventArgs e)
        {
            // Set the buttons enabled state
            dtwRead.IsEnabled = false;
            dtwCapture.IsEnabled = true;
            dtwStore.IsEnabled = false;

            // Update the status display
            status.Text = "Reading";
        }

        /// <summary>
        /// Starts a countdown timer to enable the player to get in position to record gestures
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Routed Event Args</param>
        private void DtwCaptureClick(object sender, RoutedEventArgs e)
        {
            _captureCountdown = DateTime.Now.AddSeconds(CaptureCountdownSeconds);

            _captureCountdownTimer = new Timer();
            _captureCountdownTimer.Interval = 50;
            _captureCountdownTimer.Start();
            _captureCountdownTimer.Tick += CaptureCountdown;
        }

        /// <summary>
        /// The method fired by the countdown timer. Either updates the countdown or fires the StartCapture method if the timer expires
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Event Args</param>
        private void CaptureCountdown(object sender, EventArgs e)
        {
            if (sender == _captureCountdownTimer)
            {
                if (DateTime.Now < _captureCountdown)
                {
                    status.Text = "Wait " + ((_captureCountdown - DateTime.Now).Seconds + 1) + " seconds";
                }
                else
                {
                    _captureCountdownTimer.Stop();
                    status.Text = "Recording gesture";
                    StartCapture();
                }
            }
        }

        private void GestureCaptured(GestureCapturedEventArgs args)
        {
            foreach (Dictionary<JointType, Point3D> observation in args.Buffer)
            {
                var keysToRemove = observation.Where(kvp => Array.IndexOf(SkeletonNormalizer.Forearms, kvp.Key) < 0)
                    .Select(kvp => kvp.Key).ToArray();
                Logger.Debug("Number of keysToRemove: " + keysToRemove.Length);
                foreach (JointType type in keysToRemove)
                {
                    observation.Remove(type);
                }
            }

            Gesture gesture = new Gesture(args.Buffer, gestureList.Text);

            recognizer.AddOrUpdateGesture(gesture);
            recognizer.StartRecognizing();

            DtwStoreClick(null, null);
        }

        /// <summary>
        /// Capture mode. Sets our control variables and button enabled states
        /// </summary>
        private void StartCapture()
        {
            // Set the buttons enabled state
            dtwRead.IsEnabled = false;
            dtwCapture.IsEnabled = false;
            dtwStore.IsEnabled = true;

            ////_captureCountdownTimer.Dispose();

            status.Text = "Recording gesture" + gestureList.Text;

            recognizer.StopRecognizing();
            learner.StartCapture();
        }

        /// <summary>
        /// Stores our gesture to the DTW sequences list
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Routed Event Args</param>
        private void DtwStoreClick(object sender, RoutedEventArgs e)
        {
            // Set the buttons enabled state
            dtwRead.IsEnabled = false;
            dtwCapture.IsEnabled = true;
            dtwStore.IsEnabled = false;

            status.Text = "Remembering " + gestureList.Text;

            // Add the current video buffer to the dtw sequences list
            //Gesture gesture = new Gesture(_video, gestureList.Text);
            //recognizer.AddOrUpdateGesture(gesture);
            results.Text = "Gesture " + gestureList.Text + " added";


            // Switch back to Read mode
            DtwReadClick(null, null);
        }

        /// <summary>
        /// Stores our gesture to the DTW sequences list
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Routed Event Args</param>
        private void DtwSaveToFile(object sender, RoutedEventArgs e)
        {
            string fileName = GestureSaveFileNamePrefix + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".bin";
            Stream file = File.OpenWrite(fileName);
            recognizer.PersistGesturesInBinary(file);
            file.Flush();
            file.Close();
            status.Text = "Saved to " + fileName;
        }

        /// <summary>
        /// Loads the user's selected gesture file
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Routed Event Args</param>
        private void DtwLoadFile(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog dlg = new OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".bin";
            dlg.Filter = "Binary Documents (*.bin)|*.bin";
            dlg.InitialDirectory = GestureSaveFileLocation;

            // Display OpenFileDialog by calling ShowDialog method
            // Get the selected file name and display in a TextBox
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Open document
                Stream file = File.OpenRead(dlg.FileName);
                recognizer.RebuildGesturesFromBinary(file);
                file.Close();
                dtwTextOutput.Text = recognizer.RetrieveText();
                status.Text = "Gestures loaded!";
            } 
        }

        /// <summary>
        /// Stores our gesture to the DTW sequences list
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Routed Event Args</param>
        private void DtwShowGestureText(object sender, RoutedEventArgs e)
        {
            dtwTextOutput.Text = recognizer.RetrieveText();
        }
    }
}