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
    class ArrowCursorTarget : CursorTarget, ICanvasDrawable
    {
        private readonly Brush nonSelectedFillColor = Brushes.LightBlue;
        private readonly Brush selectedColor = Brushes.OrangeRed;
        private readonly Brush nonSelectedStrokeColor = Brushes.WhiteSmoke;

        public ArrowCursorTarget(Point center, int radius) : base(center, radius) { }

        public Shape Shape { get; private set; }

        protected override void SetBounds(Point center, int radius)
        {
            base.SetBounds(center, radius);
            Shape = new Arrow
            {
                Width = Bounds.Width,
                Height = Bounds.Height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                StrokeThickness = 1.5,
                Stroke = nonSelectedStrokeColor,
                Fill = nonSelectedFillColor,
            };
            Canvas.SetLeft(Shape, Bounds.Left);
            Canvas.SetTop(Shape, Bounds.Top);
        }

        public void Draw(Canvas canvas)
        {
            canvas.Children.Add(Shape);
        }

        protected override void onCursorEnter()
        {
            Shape.Stroke = selectedColor;
            base.onCursorEnter();
        }

        protected override void onCursorSelect()
        {
            Shape.Fill = selectedColor;
            base.onCursorSelect();
        }

        protected override void onCursorLeave()
        {
            Shape.Fill = nonSelectedFillColor;
            Shape.Stroke = nonSelectedStrokeColor;
            base.onCursorLeave();
        }
    }
}
