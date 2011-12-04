﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace KinectGestureDectection
{
    /// <summary>
    /// Provides a base class to track multiple cursors within a rectangular area.
    /// </summary>
    abstract class CursorTarget : ICursorUpdatable
    {
        // Defines the rectangular target area.
        private Point center;
        private int radius;

        // The number of consecutive times a cursor must remain in the target area.
        private int numHitsForSelection = 25;
        private int previousPositiveChecks = 0;

        // Helper to create the bounding rectangle when center or radius change.
        protected virtual void SetBounds(Point center, int radius)
        {
            this.center = center;
            this.radius = radius;
            this.Bounds = new Rect(center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }

        /// <summary>
        /// Raises CursorEnter when a cursor enters the target area where there was previously not a cursor.
        /// </summary>
        protected virtual void onCursorEnter()
        {
            IsCursorInside = true;
            var handler = CursorEnter;
            if (handler != null)
                handler();
        }

        /// <summary>
        /// Raises CursorSelect by calling Select() when a cursor remains inside the target area 
        /// for numHitsForSelection consecutive times.
        /// </summary>
        protected virtual void onCursorSelect()
        {
            IsSelected = true;
            Select();
        }

        /// <summary>
        /// Raises CursorLeave when all cursors have exited to target area.
        /// </summary>
        protected virtual void onCursorLeave()
        {
            IsCursorInside = false;
            IsSelected = false;
            var handler = CursorLeave;
            if (handler != null)
                handler();
        }

        public event Action CursorEnter;
        public event Action CursorSelect;
        public event Action CursorLeave;

        /// <summary>
        /// When enabled, will detect cursors in the target area.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Returns true if there is a cursor inside the target area.
        /// </summary>
        public bool IsCursorInside { get; private set; }

        /// <summary>
        /// Returns true if there has been a cursor in the target area for at
        /// least NumHitsForSelection consecutive frames.
        /// </summary>
        public bool IsSelected { get; private set; }

        /// <summary>
        /// The Rectanglular bounding box foe the target area.
        /// </summary>
        public Rect Bounds { get; private set; }

        /// <summary>
        /// The center of the target area.
        /// </summary>
        public Point Center
        {
            get { return this.center; }
            set { SetBounds(value, radius); }
        }

        /// <summary>
        /// The length and width of the target area.
        /// </summary>
        public int Radius
        {
            get { return this.radius; }
            set { SetBounds(center, value); }
        }

        /// <summary>
        /// Number of consecutive hits required to select the target.
        /// </summary>
        public int NumHitsForSelection
        {
            get { return this.numHitsForSelection; }
            set
            {
                this.numHitsForSelection = value;
                this.previousPositiveChecks = 0;
            }
        }


        /// <summary>
        /// Initializes a new CursorTarget.
        /// </summary>
        /// <param name="center">Center of the target area.</param>
        /// <param name="radius">Length and width of the target area.</param>
        public CursorTarget(Point center, int radius)
        {
            SetBounds(center, radius);
            this.IsEnabled = true;
        }

        /// <summary>
        /// Checks the target area for the provided points and adjusts state as necessary.
        /// </summary>
        /// <param name="cursorLocations">Location of all cursors to check.</param>
        public void UpdateCursors(IEnumerable<Point> cursorLocations)
        {
            if(!IsEnabled)
                return;

            foreach (var point in cursorLocations)
            {
                if (Bounds.Contains(point))
                {
                    if (!IsCursorInside)
                        onCursorEnter();

                    if (!IsSelected && ++previousPositiveChecks == numHitsForSelection)
                        onCursorSelect();

                    return;
                }
            }

            previousPositiveChecks = 0;

            if (IsCursorInside)
                onCursorLeave();
        }

        /// <summary>
        /// Raises CursorSelect event.
        /// This can be called manually or through the actual cursors.
        /// </summary>
        public void Select()
        {
            if (!IsEnabled)
                return;
            var handler = CursorSelect;
            if (handler != null)
                handler();
        }
    }

}