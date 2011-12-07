using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinectGestureDectection
{
    class ArrowCursorTarget : ShapeCursorTarget
    {
        public ArrowCursorTarget(Canvas canvas, Point center, int radius) :
            base(new Arrow
             {
                 HorizontalAlignment = HorizontalAlignment.Left,
                 VerticalAlignment = VerticalAlignment.Top,
                 StrokeThickness = 1.5,
             }, canvas, center, radius)
        { }
    }
}
