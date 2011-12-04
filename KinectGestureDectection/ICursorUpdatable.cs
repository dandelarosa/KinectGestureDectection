using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace KinectGestureDectection
{
    interface ICursorUpdatable
    {
        void UpdateCursors(IEnumerable<Point> cursorLocations);
    }
}
