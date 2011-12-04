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
    class PathSelectionComponent : ICanvasDrawable, ICursorUpdatable
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

        private Dictionary<PathDirection, ArrowCursorTarget> targets;

        private void createArrow(PathDirection direction, Point center, int radius, double rotationAngle)
        {
            var arrow = new ArrowCursorTarget(center, radius);
            arrow.Shape.LayoutTransform = new RotateTransform(rotationAngle, center.X, center.Y);
            arrow.CursorSelect += () => onPathSelected(direction);
            targets.Add(direction, arrow);
        }

        public bool IsEnabled { get; set; }

        public void SetTargetEnabled(PathDirection direction, bool isEnabled)
        {
            targets[direction].IsEnabled = isEnabled;
        }

        public PathSelectionComponent(double canvasWidth, double canvasHeight)
        {
            int targetRadius = 40;
            double halfCanvasHeight = canvasHeight / 2;
            double halfCanvasWidth = canvasWidth / 2;

            targets = new Dictionary<PathDirection, ArrowCursorTarget>();

            createArrow(PathDirection.Forward, new Point(halfCanvasWidth, targetRadius), targetRadius, -90);
            createArrow(PathDirection.Left, new Point(targetRadius, halfCanvasHeight), targetRadius, 180);
            createArrow(PathDirection.Right, new Point(canvasWidth - targetRadius, halfCanvasHeight), targetRadius, 0);
            createArrow(PathDirection.Back, new Point(halfCanvasWidth, canvasHeight - targetRadius), targetRadius, 90);

            IsEnabled = true;
        }

        public void UpdateCursors(IEnumerable<Point> cursors)
        {
            if (!IsEnabled)
                return;
            foreach (var target in targets.Values)
                if (target.IsEnabled)
                    target.UpdateCursors(cursors);
        }

        public void Draw(Canvas canvas)
        {
            if (!IsEnabled)
                return;
            foreach (var target in targets.Values)
                if(target.IsEnabled)
                    target.Draw(canvas);
        }
    }
}
