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
    sealed class KinectManager : IDisposable
    {
        private SkeletonDisplayManager skeletonDisplayManager;
        private PathSelectionComponent pathSelector;
        private CombatPostureDetector postureDectector;
        private SimpleSlashGestureDetector gestureDetector;
        private ColorStreamManager displayStreamManager;
        private Runtime kinect;
        private Canvas canvas;
        private Canvas skeletonCanvas;
        private Image display;

        public enum InputState
        {
            Idle,
            CombatPosture,
            CombatGesture,
            PathSelection,
        }

        private InputState currentState;
        public InputState CurrentState
        {
            get { return this.currentState; }
            set
            {
                this.currentState = value;
                switch (currentState)
                {
                    case InputState.PathSelection:
                        pathSelector.IsEnabled = true;
                        break;
                }
            }
        }
        public bool TrackingSkeleton { get; private set; }

        public event Action<string> CombatGestureDectected;
        public event Action<PathSelectionComponent.PathDirection> PathSelected;
        public event Action SkeletonFound;
        public event Action SkeletonLost;

        public KinectManager(Runtime kinect, Image display, Canvas canvas, Canvas skeletonCanvas)
        {
            this.kinect = kinect;
            this.display = display;
            this.canvas = canvas;
            this.skeletonCanvas = skeletonCanvas;

            this.skeletonDisplayManager = new SkeletonDisplayManager(kinect.SkeletonEngine, skeletonCanvas);

            this.pathSelector = new PathSelectionComponent(canvas);
            this.pathSelector.PathSelected += new EventHandler<PathSelectionComponent.PathSelectedEventArgs>(pathSelector_PathSelected);
            this.pathSelector.IsEnabled = false;

            this.postureDectector = new CombatPostureDetector();
            this.postureDectector.PostureDetected += new Action<string>(postureDectector_PostureDetected);

            this.gestureDetector = new SimpleSlashGestureDetector();
            this.gestureDetector.OnGestureDetected += new Action<string>(gestureDetector_OnGestureDetected);
            this.gestureDetector.TraceTo(canvas, System.Windows.Media.Color.FromRgb(255, 0, 0));
            this.gestureDetector.MinimalPeriodBetweenGestures = 2000;

            this.displayStreamManager = new ColorStreamManager();

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
            kinect.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(kinect_VideoFrameReady);

            this.CurrentState = InputState.Idle;
        }

        private void kinect_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            display.Source = displayStreamManager.Update(e);
        }

        public void Dispose()
        {
            kinect.SkeletonFrameReady -= kinect_SkeletonFrameReady;
            kinect.VideoFrameReady -= kinect_VideoFrameReady;
        }

        public void SetPathEnabled(PathSelectionComponent.PathDirection direction, bool isEnabled)
        {
            pathSelector.SetTargetEnabled(direction, isEnabled);
        }

        private Point[] cursors = new Point[2];
        private void updatePathSelection(SkeletonData skeleton)
        {
            float x = 0, y = 0;

            kinect.SkeletonEngine.SkeletonToDepthImage(skeleton.Joints[JointID.HandLeft].Position, out x, out y);
            cursors[0] = new Point(canvas.ActualWidth * x, canvas.ActualHeight * y);

            kinect.SkeletonEngine.SkeletonToDepthImage(skeleton.Joints[JointID.HandRight].Position, out x, out y);
            cursors[1] = new Point(canvas.ActualWidth * x, canvas.ActualHeight * y);

            pathSelector.UpdateCursors(cursors);
        }

        private void updateCombatPosture(SkeletonData skeleton)
        {
            if(skeleton.TrackingState == SkeletonTrackingState.Tracked)
                postureDectector.TrackPostures(skeleton);
        }

        private void updateCombatGesture(SkeletonData skeleton)
        {
            Joint rightHand = skeleton.Joints[JointID.HandRight];
            if (rightHand.TrackingState == JointTrackingState.Tracked)
               gestureDetector.Add(rightHand.Position, kinect.SkeletonEngine);
        }

        private void gestureDetector_OnGestureDetected(string gesture)
        {
            if (CurrentState == InputState.CombatGesture)
            {
                CurrentState = InputState.Idle;
                var handler = CombatGestureDectected;
                if (handler != null)
                    handler(gesture);
            }
        }

        private void postureDectector_PostureDetected(string posture)
        {
            if (CurrentState == InputState.CombatPosture)
            {
                CurrentState = InputState.CombatGesture;
            }
        }

        private void pathSelector_PathSelected(object sender, PathSelectionComponent.PathSelectedEventArgs e)
        {
            if (CurrentState == InputState.PathSelection)
            {
                pathSelector.IsEnabled = false;
                CurrentState = InputState.Idle;
                var handler = PathSelected;
                if (handler != null)
                    handler(e.Direction);
            }
        }

        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (e.SkeletonFrame == null)
                return;

            foreach (SkeletonData skeleton in e.SkeletonFrame.Skeletons)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                    continue;

                if (!TrackingSkeleton)
                {
                    var handler = SkeletonFound;
                    if (handler != null)
                        handler();
                }

                skeletonDisplayManager.Draw(e.SkeletonFrame);

                switch (CurrentState)
                {
                    case InputState.PathSelection:
                        updatePathSelection(skeleton);
                        return;
                    case InputState.CombatPosture:
                        updateCombatPosture(skeleton);
                        return;
                    case InputState.CombatGesture:
                        updateCombatGesture(skeleton);
                        return;
                    default:
                        return;
                }
            }

            if (TrackingSkeleton)
            {
                var handler = SkeletonLost;
                if (handler != null)
                    handler();
            }
        }
    }
}
