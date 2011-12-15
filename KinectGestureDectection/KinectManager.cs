using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Research.Kinect.Nui;
using System.Windows;
using Kinect.Toolbox;

namespace KinectGestureDectection
{
    /// <summary>
    /// This class manages Kinect devices attatched to the system.
    /// </sumary>
    /// 
    /// It handles all initialization, processing and disposal.
    /// The manager will find the first Kinect connected and use the first
    /// skeleton found for all processing.  GestureDetectors, PostureDetectors
    /// and ICursorUpdatables can be swapped in on the fly.  Changing the 
    /// CurrentState property will determine which actions are performed when 
    /// a new recieved from the device.
    sealed class KinectManager : IDisposable
    {
        // WHen non-null, is the first connected Kinect device.
        private Runtime kinect;

        // Draws skeletons to the given skeleton canvas.
        private SkeletonDisplayManager skeletonDisplayManager;

        // Displays the RGB video stream to the given Image.
        private ColorStreamManager displayStreamManager;

        // Static canvas where cursors will be relative to.
        private Canvas canvas;

        // Canvas to draw the skeleton, cleared every frame.
        private Canvas skeletonCanvas;

        // Image to draw RGB frames from the device.
        private Image display;

        // Possible states of operation.
        public enum InputState
        {
            Idle,
            Posture,
            Gesture,
            Cursor,
        }

        /// <summary>
        /// Current state of the manager.
        /// </summary>
        public InputState CurrentState { get; set; }

        /// <summary>
        /// Returns true when a Kinect device is currently connected.
        /// </summary>
        public bool IsKinectConnected { get; private set; }

        /// <summary>
        /// Returns true when a skeleton is currently tracked.
        /// </summary>
        public bool IsSkeletonTracked { get; private set; }

        /// <summary>
        /// Set true or false to draw skeleton to skeleton canvas.
        /// </summary>
        public bool IsSkeletonVisible { get; set; }

        /// <summary>
        /// Fires when a new Kinect device is found when there was previously none.
        /// </summary>
        public event Action<Runtime> KinectConnected;

        /// <summary>
        /// Fires when a new skeleton is found when there was previously none.
        /// </summary>
        public event Action SkeletonFound;

        /// <summary>
        /// Fires when the previously tracked skeleton is lost.
        /// </summary>
        public event Action SkeletonLost;

        /// <summary>
        /// Gesture detector to be used during InputState.Gesture
        /// </summary>
        public GestureDetector GestureDetector { get; set; }

        /// <summary>
        /// Posture detector to be used during InputState.Posture
        /// </summary>
        public PostureDetector PostureDetector { get; set; }

        /// <summary>
        /// Cursor object to be updated during InputState.Cursor
        /// </summary>
        public ICursorUpdatable CursorUpdatable { get; set; }

        public event Action<Point> GestureUpdate;
        public event Action<Point, Point> CursorUpdate;

        /// <summary>
        /// Creates a new KinectManager instance.
        /// </summary>
        /// <param name="display">Image to display RGB stream</param>
        /// <param name="canvas">Canvas that relates to posture, gestures and cursors</param>
        /// <param name="skeletonCanvas">Canvas to draw skeletons</param>
        public KinectManager(Image display, Canvas canvas, Canvas skeletonCanvas)
        {
            this.display = display;
            this.canvas = canvas;
            this.skeletonCanvas = skeletonCanvas;
        }

        /// <summary>
        /// Initialize the manager for use.
        /// </summary>
        public void Initialize()
        {
            if (kinect != null)
                return;
            
            kinect = Runtime.Kinects.Where(k => k.Status == KinectStatus.Connected).FirstOrDefault();

            if (kinect == null)
                return;
            
            this.kinect.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            this.kinect.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);

            this.skeletonDisplayManager = new SkeletonDisplayManager(kinect.SkeletonEngine, skeletonCanvas);
            this.IsSkeletonVisible = true;

            this.displayStreamManager = new ColorStreamManager();

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
            kinect.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(kinect_VideoFrameReady);

            var handler = KinectConnected;
            if (handler != null)
                handler(kinect);
        }

        /// <summary>
        /// Copies the RGB frame onto the display.
        /// </summary>
        private void kinect_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            display.Source = displayStreamManager.Update(e);
        }

        /// <summary>
        /// Disposes of all internal resources used by this manager.
        /// </summary>
        public void Dispose()
        {
            if (IsKinectConnected)
            {
                kinect.SkeletonFrameReady -= kinect_SkeletonFrameReady;
                kinect.VideoFrameReady -= kinect_VideoFrameReady;
                kinect.Uninitialize();
                GestureDetector = null;
                PostureDetector = null;
                CursorUpdatable = null;
                kinect = null;
            }
        }

        /// <summary>
        /// Updates the posture detector with the first skeleton found.
        /// </summary>
        /// <param name="skeleton">First skeleton</param>
        private void updatePosture(SkeletonData skeleton)
        {
            var postureDetector = PostureDetector;
            if (postureDetector != null && skeleton.TrackingState == SkeletonTrackingState.Tracked)
                postureDetector.TrackPostures(skeleton);
        }

        /// <summary>
        /// Updates the gesture detector with the first skeleton found 
        /// using the position of the right hand.
        /// </summary>
        /// <param name="skeleton"></param>
        private void updateGesture(SkeletonData skeleton)
        {
            var gestureDetector = GestureDetector;
            Joint rightHand = skeleton.Joints[JointID.HandRight];
            if (gestureDetector != null && rightHand.TrackingState == JointTrackingState.Tracked)
            {
                gestureDetector.Add(rightHand.Position, kinect.SkeletonEngine);
                var handler = GestureUpdate;
                if (handler != null)
                {
                    float x = 0, y = 0;
                    kinect.SkeletonEngine.SkeletonToDepthImage(skeleton.Joints[JointID.HandRight].Position, out x, out y);
                    Point position = new Point(canvas.ActualWidth * x, canvas.ActualHeight * y);
                    handler(position);
                }
            }
        }
        
        // Internal buffer used to store cursor locations for updating.
        private Point[] cursors = new Point[2];

        /// <summary>
        /// Updates the cursors locations, passing them to the ICursorUpdatables.
        /// Includes the locations of the right and left hands of the first 
        /// skeleton found only.
        /// </summary>
        /// <param name="skeleton">First skeleton found</param>
        private void updateCursors(SkeletonData skeleton)
        {
            var cursorUpdatable = CursorUpdatable;
            if (cursorUpdatable == null)
                return;

            float x = 0, y = 0;

            kinect.SkeletonEngine.SkeletonToDepthImage(skeleton.Joints[JointID.HandLeft].Position, out x, out y);
            cursors[0] = new Point(canvas.ActualWidth * x, canvas.ActualHeight * y);

            kinect.SkeletonEngine.SkeletonToDepthImage(skeleton.Joints[JointID.HandRight].Position, out x, out y);
            cursors[1] = new Point(canvas.ActualWidth * x, canvas.ActualHeight * y);

            cursorUpdatable.UpdateCursors(cursors);
            var handler = CursorUpdate;
            if (handler != null)
                handler(cursors[0], cursors[1]);
        }

        // Called each skeleton frame, dispataches appropriate update based on current state.
        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (e.SkeletonFrame == null)
                return;

            foreach (SkeletonData skeleton in e.SkeletonFrame.Skeletons)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                    continue;

                if (!IsSkeletonTracked)
                {
                    var handler = SkeletonFound;
                    if (handler != null)
                        handler();
                }

                if(IsSkeletonVisible)
                    skeletonDisplayManager.Draw(e.SkeletonFrame);

                switch (CurrentState)
                {
                    case InputState.Cursor:
                        updateCursors(skeleton);
                        return;
                    case InputState.Posture:
                        updatePosture(skeleton);
                        return;
                    case InputState.Gesture:
                        updateGesture(skeleton);
                        return;
                    default:
                        return;
                }
            }

            if (IsSkeletonTracked)
            {
                var handler = SkeletonLost;
                if (handler != null)
                    handler();
            }
        }
    }
}
