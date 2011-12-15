using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;

namespace KinectGestureDectection
{
    class ShapeCursorTarget : CursorTarget
    {
        private readonly Canvas canvas;

        public Brush NonSelectedFill { get; set; }
        public Brush SelectedFill { get; set; }
        public Brush NonSelectedStroke { get; set; }
        public Brush SelectedStroke { get; set; }

        public Shape Shape { get; private set; }

        public ShapeCursorTarget(Shape shape, Canvas canvas, Point center, int radius)
            :this(shape, canvas, center, radius, radius){}

        public ShapeCursorTarget(Shape shape, Canvas canvas, Point center, int width, int height)
        {
            this.canvas = canvas;
            this.Shape = shape;
            this.Shape.Fill = this.NonSelectedFill = Brushes.DarkBlue;
            this.Shape.Stroke = this.NonSelectedStroke = Brushes.WhiteSmoke;
            this.SelectedFill = Brushes.OrangeRed;
            this.SelectedStroke = Brushes.OrangeRed;
            SetBounds(center, width, height);
        }

        public override bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                base.IsEnabled = value;
                if (canvas == null)
                    return;
                bool containsShape = canvas.Children.Contains(Shape);
                if (value)
                {
                    if (!containsShape)
                        canvas.Children.Add(Shape);
                }
                else
                {
                    if (containsShape)
                    {
                        canvas.Children.Remove(Shape);
                        onCursorLeave();
                    }
                }
            }
        }

        protected override void SetBounds(Point center, int width, int height)
        {
            base.SetBounds(center, width, height);
            Shape.Width = Bounds.Width;
            Shape.Height = Bounds.Height;
            Canvas.SetLeft(Shape, Bounds.Left);
            Canvas.SetTop(Shape, Bounds.Top);
        }

        protected override void onCursorEnter()
        {
            Shape.Fill = SelectedFill;
            base.onCursorEnter();
        }

        protected override void onCursorSelect()
        {
            Shape.Stroke = SelectedStroke;
            base.onCursorSelect();
        }

        protected override void onCursorLeave()
        {
            Shape.Fill = NonSelectedFill;
            Shape.Stroke = NonSelectedStroke;
            base.onCursorLeave();
        }
    }
}
