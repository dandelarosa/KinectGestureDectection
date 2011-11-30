using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Research.Kinect.Nui;
using Kinect.Toolbox;

namespace KinectGestureDectection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Runtime kinectRuntime;
        private SkeletonDisplayManager skeletonDisplayManager;

        private readonly ColorStreamManager streamManager = new ColorStreamManager();
        private readonly GestureDetector gestureRecognizer = new SimpleSlashGestureDetector();
        private Game game = new Game();

        private System.Windows.Threading.DispatcherTimer dispatcherTimer =
                new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            if (kinectRuntime == null)
                return;

            kinectRuntime.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            kinectRuntime.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            kinectRuntime.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(kinectRuntime_VideoFrameReady);
            kinectRuntime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectRuntime_SkeletonFrameReady);
            skeletonDisplayManager = new SkeletonDisplayManager(kinectRuntime.SkeletonEngine, kinectCanvas);
            
            gestureRecognizer.TraceTo(gesturesCanvas, System.Windows.Media.Color.FromRgb(255, 0, 0));
            gestureRecognizer.OnGestureDetected += new Action<string>(OnGestureDetected);
            gestureRecognizer.MinimalPeriodBetweenGestures = 2000;

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            PrintLine("Ready.");
            
            NextTurn();
        }

        private void NextTurn()
        {
            // Tell the game that it's the next turn
            game.NextTurn();

            // Print Prompt
            PrintLine(game.GetPrompt());

            //  DispatcherTimer setup
            int duration = 5;
            dispatcherTimer.Interval = new TimeSpan(0, 0, duration);
            dispatcherTimer.Start();
        }

        /**
         * TODO
         */
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Tell the game that time is up
            PrintLine("Time's up.");

            // Do another timed event
            NextTurn();
        }

        private void PrintLine(string text)
        {
            textBlock1.Text += text + "\n";
            textScrollView.ScrollToEnd();
        }

        private void OnGestureDetected(string gesture)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.Ticks + " " + gesture);
            PrintLine(DateTime.Now.Ticks + " " + gesture);

            // Process Gesture
            if (game.EnterGesture(gesture))
            {
                // If correct, go to the next turn
                dispatcherTimer.Stop();
                NextTurn();
            }
        }

        private void Clean()
        {
            gestureRecognizer.OnGestureDetected -= OnGestureDetected;

            if (kinectRuntime != null)
            {
                kinectRuntime.VideoFrameReady -= kinectRuntime_VideoFrameReady;
                kinectRuntime.SkeletonFrameReady -= kinectRuntime_SkeletonFrameReady;
                kinectRuntime.Uninitialize();
                kinectRuntime = null;
            }
        }

        private void ProcessFrame(SkeletonFrame frame)
        {
            foreach (var skeleton in frame.Skeletons)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                    continue;

                foreach (Joint joint in skeleton.Joints)
                {
                    if (joint.Position.W < 0.8f || joint.TrackingState != JointTrackingState.Tracked)
                        continue;

                    if (joint.ID == JointID.HandRight)
                    {
                        gestureRecognizer.Add(joint.Position, kinectRuntime.SkeletonEngine);
                    }
                }

                skeletonDisplayManager.Draw(frame);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Runtime kinect in Runtime.Kinects)
                {
                    if (kinect.Status == KinectStatus.Connected)
                    {
                        kinectRuntime = kinect;
                        break;
                    }
                }

                if (Runtime.Kinects.Count == 0)
                    MessageBox.Show("No Kinect Found");
                else
                    Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Clean();
        }

        private void kinectRuntime_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            kinectDisplay.Source = streamManager.Update(e);
        }

        private void kinectRuntime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (e.SkeletonFrame == null)
                return;
            if (!e.SkeletonFrame.Skeletons.Any(s => s.TrackingState != SkeletonTrackingState.NotTracked))
                return;
            ProcessFrame(e.SkeletonFrame);
        }

    }
}
