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
    class ArrowCursorTarget : CursorTarget
    {
        private readonly Brush nonSelectedFillColor = Brushes.LightBlue;
        private readonly Brush selectedColor = Brushes.OrangeRed;
        private readonly Brush nonSelectedStrokeColor = Brushes.WhiteSmoke;
        private readonly Canvas canvas;
        private Shape shape;

        public ArrowCursorTarget(Canvas canvas, Point center, int radius) : base(center, radius)
        {
            this.canvas = canvas;
        }

        public Transform LayoutTransform
        {
            get { return shape.LayoutTransform; }
            set { shape.LayoutTransform = value; }
        }

        public override bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                base.IsEnabled = value;
                if (canvas == null) 
                    return;
                bool containsShape = canvas.Children.Contains(shape);
                if (value)
                {
                    if (!containsShape)
                        canvas.Children.Add(shape);
                }
                else
                {
                    if (containsShape)
                        canvas.Children.Remove(shape);
                }
            }
        }

        protected override void SetBounds(Point center, int radius)
        {
            base.SetBounds(center, radius);
            if (shape == null)
            {
                shape = new Arrow
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    StrokeThickness = 1.5,
                    Stroke = nonSelectedStrokeColor,
                    Fill = nonSelectedFillColor,
                };
            }
            shape.Width = Bounds.Width;
            shape.Height = Bounds.Height;
            Canvas.SetLeft(shape, Bounds.Left);
            Canvas.SetTop(shape, Bounds.Top);
        }

        protected override void onCursorEnter()
        {
            shape.Stroke = selectedColor;
            base.onCursorEnter();
        }

        protected override void onCursorSelect()
        {
            shape.Fill = selectedColor;
            base.onCursorSelect();
        }

        protected override void onCursorLeave()
        {
            shape.Fill = nonSelectedFillColor;
            shape.Stroke = nonSelectedStrokeColor;
            base.onCursorLeave();
        }
    }
}
