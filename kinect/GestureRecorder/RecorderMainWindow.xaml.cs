using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using Microsoft.Kinect;
using log4net;
using log4net.Config;
using GestureRecognition;
using GestureController;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace GestureRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RecorderMainWindow : Window
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RecorderMainWindow));

        private const string GestureSaveFileLocation = @".\";

        static RecorderMainWindow()
        {
            XmlConfigurator.Configure();
        }

        #region Private Member Variables
        private KinectSensor _sensor;
        private byte[] _depthFrame32;
        private WriteableBitmap _colorBitmap;
        private WriteableBitmap _depthBitmap;
        private GestureLearner _learner;
        private GestureRecognizer _recognizer;

        private Dictionary<string, Gesture> _gestures;

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

        private const int CaptureCountdownSeconds = 3;
        private DateTime _captureCountdown = DateTime.Now;
        private Timer _captureCountdownTimer;

        #endregion

        public RecorderMainWindow()
        {
            InitializeComponent();

            _jointSelection.ItemsSource = Enum.GetValues(typeof(JointType)).Cast<JointType>().ToArray();
            _jointSelection.SelectAll();

            _actionSelector.ItemsSource = GestureToActionMapper.Actions.Keys;
            _actionSelector.SelectedIndex = 0;

            _gestures = new Dictionary<string, Gesture>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this._sensor = null;
            // Loop through potential kinects and choose one
            foreach (var potential in KinectSensor.KinectSensors)
            {
                if (potential.Status == KinectStatus.Connected)
                {
                    this._sensor = potential;
                }
            }

            if (this._sensor == null)
            {
                Logger.Warn("No kinect sensors attached");
                this._statusText.Text += "No Kinect sensor found -- Please connect a sensor and restart the application";
            }
            else
            {

                _learner = new GestureLearner();
                _learner.GestureCaptured += this.GestureCaptureFinishedHandler;

                _recognizer = new GestureRecognizer(.9, 3);
                _recognizer.GestureRecognized += this.GestureRecognizedHandler;

                this._sensor.SkeletonStream.Enable();
                this._sensor.SkeletonFrameReady += this.SkeletonFrameReadyHandler;
                this._sensor.SkeletonFrameReady += _learner.FrameReadyHandler;
                this._sensor.SkeletonFrameReady += _recognizer.FrameReadyHandler;

                this._sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                this._sensor.ColorFrameReady += this.ColorFrameReadyHandler;

                _colorBitmap = new WriteableBitmap(this._sensor.ColorStream.FrameWidth, this._sensor.ColorStream.FrameHeight,
                    96.0, 96.0, PixelFormats.Bgr32, null);
                videoImage.Source = _colorBitmap;

                this._sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                this._sensor.DepthFrameReady += this.DepthFrameReadyHandler;

                _depthFrame32 = new byte[this._sensor.DepthStream.FramePixelDataLength * sizeof(int)];
                _depthBitmap = new WriteableBitmap(this._sensor.DepthStream.FrameWidth, this._sensor.DepthStream.FrameHeight,
                    96.0, 96.0, PixelFormats.Bgr32, null);
                depthImage.Source = _depthBitmap;

                try
                {
                    this._sensor.Start();
                    Logger.Debug("Started kinect sensor");
                }
                catch (IOException exception)
                {
                    Logger.Error("Kinect failed to start", exception);
                }
            }

            Logger.Info("Finished loading window");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Logger.Debug("Stopping kinect sensor");
            this._sensor.Stop();
            Logger.Debug("Kinect sensor stopped");
            Environment.Exit(0);
        }

        private void SkeletonFrameReadyHandler(object sender, SkeletonFrameReadyEventArgs args)
        {
            _fpsDisplay.Text = _recognizer.AverageFPS.ToString("N2");
            _learnerBufferCount.Text = _learner.CurrentBufferSize + " frames";
            _recognizerBufferCount.Text = _recognizer.CurrentBufferSize + " frames";
            using (SkeletonFrame frame = args.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    Skeleton[] skeletonData = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletonData);

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
                        if (data.TrackingState == SkeletonTrackingState.Tracked)
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
                else
                {
                    this._statusText.Text += "Frame is null";
                }
            }
        }

        private void DepthFrameReadyHandler(object sender, DepthImageFrameReadyEventArgs args)
        {
            using (DepthImageFrame frame = args.OpenDepthImageFrame())
            {
                if (frame != null)
                {
                    DepthImagePixel[] image = new DepthImagePixel[frame.PixelDataLength];
                    frame.CopyDepthImagePixelDataTo(image);

                    byte[] convertedDepthFrame = ConvertDepthFrame(image);

                    this._depthBitmap.WritePixels(
                            new Int32Rect(0, 0, this._depthBitmap.PixelWidth, this._depthBitmap.PixelHeight),
                            this._depthFrame32,
                            this._depthBitmap.PixelWidth * sizeof(int),
                            0
                        );
                }
            }
        }

        /// <summary>
        /// Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame that displays different players in different colors
        /// </summary>
        /// <param name="depthFrame16">The depth frame byte array</param>
        /// <returns>A depth frame byte array containing a player image</returns>
        private byte[] ConvertDepthFrame(DepthImagePixel[] depthFrame16)
        {
            int RedIdx = 2;
            int BlueIdx = 0;
            int GreenIdx = 1;
            
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

        private void ColorFrameReadyHandler(object sender, ColorImageFrameReadyEventArgs args)
        {
            using (ColorImageFrame frame = args.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    this._colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this._colorBitmap.PixelWidth, this._colorBitmap.PixelHeight),
                        frame.GetRawPixelData(),
                        this._colorBitmap.PixelWidth * sizeof(int),
                        0
                        );
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
            DepthImagePoint depthPoint = _sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(joint.Position, _sensor.DepthStream.Format);

            depthX = depthPoint.X;
            depthY = depthPoint.Y;

            depthX = Math.Max(0, Math.Min(depthX * _sensor.DepthStream.FrameWidth,
                _sensor.DepthStream.FrameWidth));
            depthY = Math.Max(0, Math.Min(depthY * _sensor.DepthStream.FrameHeight, 
                _sensor.DepthStream.FrameHeight));

            int colorX, colorY;
            ColorImagePoint colorPoint = _sensor.CoordinateMapper.MapDepthPointToColorPoint(_sensor.DepthStream.Format, depthPoint, _sensor.ColorStream.Format);
            colorX = colorPoint.X;
            colorY = colorPoint.Y;

            return new Point((int)(skeletonCanvas.Width * colorX / _sensor.ColorStream.FrameWidth), (int)(skeletonCanvas.Height * colorY / _sensor.ColorStream.FrameHeight));
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

        private void CaptureGesture_Click(object sender, RoutedEventArgs e)
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
                    _statusText.Text = "Wait " + ((_captureCountdown - DateTime.Now).Seconds + 1) + " seconds";
                }
                else
                {
                    _captureCountdownTimer.Stop();
                    _statusText.Text = "Recording gesture";
                    StartCapture();
                }
            }
        }

        private void DisableButtons()
        {
            _actionSelector.IsEnabled = false;
            _lengthSlider.IsEnabled = false;
            _jointSelection.IsEnabled = false;
            _testAllGestures.IsEnabled = false;
            _testGesture.IsEnabled = false;
            _recordGesture.IsEnabled = false;
            _stop.IsEnabled = true;
        }

        private void EnableButtons()
        {
            _actionSelector.IsEnabled = true;
            _lengthSlider.IsEnabled = true;
            _jointSelection.IsEnabled = true;
            _testAllGestures.IsEnabled = true;
            _testGesture.IsEnabled = true;
            _recordGesture.IsEnabled = true;
            _stop.IsEnabled = false;
        }

        private void StartCapture()
        {
            DisableButtons();
            _statusText.Text = "Recording gesture for " + _actionSelector.Text;
            _recognizer.StopRecognizing();
            _learner.StartCapture();
        }

        private void GestureCaptureFinishedHandler(GestureCapturedEventArgs args)
        {
            _statusText.Text = "Gesture capture finished";



            foreach (Dictionary<JointType, Point3D> observation in args.Buffer)
            {
                var keysToRemove = observation.Where(kvp => Array.IndexOf(_jointSelection.SelectedItems.Cast<JointType>().ToArray(), kvp.Key) < 0)
                    .Select(kvp => kvp.Key).ToArray();
                Logger.Debug("Number of keysToRemove: " + keysToRemove.Length);
                foreach (JointType type in keysToRemove)
                {
                    observation.Remove(type);
                }
            }

            Gesture gesture = new Gesture(args.Buffer, _actionSelector.Text);

            _gestures[gesture.Label] = gesture;

            EnableButtons();
        }

        private void GestureRecognizedHandler(GestureRecognizedEventArgs args)
        {
            _statusText.Text = "Recognized " + args.Gesture.Label;
        }

        private void _testGesture_Click(object sender, RoutedEventArgs e)
        {
            _recognizer.ClearAllGestures();
            Gesture gesture;
            if (_gestures.TryGetValue(_actionSelector.Text, out gesture))
            {
                DisableButtons();
                _recognizer.AddOrUpdateGesture(gesture);
                _statusText.Text = _recognizer.RetrieveText();

                _recognizer.StartRecognizing();
            }
        }

        private void _testAllGestures_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();

            _recognizer.ClearAllGestures();
            foreach (Gesture gesture in _gestures.Values)
            {
                _recognizer.AddOrUpdateGesture(gesture);
            }

            _statusText.Text += _recognizer.RetrieveText();

            _recognizer.StartRecognizing();
        }

        private void _stop_Click(object sender, RoutedEventArgs e)
        {
            _recognizer.StopRecognizing();
            _learner.CancelCapture();
            EnableButtons();
        }

        private void _saveFileButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = _saveFileName.Text + ".bin";
            Stream file = File.OpenWrite(fileName);

            foreach (Gesture gesture in _gestures.Values)
            {
                _recognizer.AddOrUpdateGesture(gesture);
            }

            _recognizer.PersistGesturesInBinary(file);
            file.Flush();
            file.Close();
            _statusText.Text += "Saved to " + fileName;
        }
    }
}
