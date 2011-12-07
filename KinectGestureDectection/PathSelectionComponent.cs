using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace KinectGestureDectection
{
    /// <summary>
    /// Provides a component to select a direction using CursorTargets.
    /// Possible directions are members of PathDirection.
    /// The directions are visualized as arrows around the screen.
    /// </summary>
    public class PathSelectionComponent : ICursorUpdatable
    {
        public enum PathDirection
        {
            Forward,
            Right,
            Back,
            Left,
        }

        #region Events and Event Handlers

        public event EventHandler<PathSelectedEventArgs> PathSelected;

        public class PathSelectedEventArgs : EventArgs
        {
            public PathDirection Direction { get; private set; }

            public PathSelectedEventArgs(PathDirection direction)
            {
                this.Direction = direction;
            }
        }

        protected void onPathSelected(PathDirection direction)
        {
            var handler = PathSelected;
            if (handler != null)
                handler(this, new PathSelectedEventArgs(direction));
        }

        #endregion

        private Canvas canvas;
        private Dictionary<PathDirection, ArrowCursorTarget> targets;

        private void createArrow(PathDirection direction, Point center, int radius, double rotationAngle)
        {
            var arrow = new ArrowCursorTarget(canvas, center, radius);
            arrow.Shape.LayoutTransform = new RotateTransform(rotationAngle, center.X, center.Y);
            arrow.CursorSelect += () => onPathSelected(direction);
            targets.Add(direction, arrow);
        }

        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                foreach (var t in targets)
                    SetTargetEnabled(t.Key, value);
            }
        }

        public void SetTargetEnabled(PathDirection direction, bool isEnabled)
        {
            targets[direction].IsEnabled =  isEnabled;
        }

        public PathSelectionComponent(Canvas canvas)
        {
            this.canvas = canvas;

            int targetRadius = 40;
            double halfCanvasHeight = canvas.ActualHeight / 2;
            double halfCanvasWidth = canvas.ActualWidth / 2;

            targets = new Dictionary<PathDirection, ArrowCursorTarget>();

            createArrow(PathDirection.Forward, new Point(halfCanvasWidth, targetRadius), targetRadius, -90);
            createArrow(PathDirection.Left, new Point(targetRadius, halfCanvasHeight), targetRadius, 180);
            createArrow(PathDirection.Right, new Point(canvas.ActualWidth - targetRadius, halfCanvasHeight), targetRadius, 0);
            createArrow(PathDirection.Back, new Point(halfCanvasWidth, canvas.ActualHeight - targetRadius), targetRadius, 90);
        }

        public void UpdateCursors(IEnumerable<Point> cursors)
        {
            if(IsEnabled)
                foreach (var target in targets.Values)
                    target.UpdateCursors(cursors);      
        }
    }
}
