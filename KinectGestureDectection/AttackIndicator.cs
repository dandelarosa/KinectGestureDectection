using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace KinectGestureDectection
{
    class AttackIndicator
    {
        public enum AttackType
        {
            UpToDown,
            LeftToRight,
            RightToLeft,
        }

        private Arrow arrow;
        private Canvas canvas;
        private DoubleAnimation animation;
        
        public bool IsPlaying { get; private set; }

        public AttackIndicator(Canvas canvas)
        {
            this.canvas = canvas;
            this.arrow = new Arrow       
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                StrokeThickness = 0.5,
                Stroke = Brushes.White,
                Fill = new SolidColorBrush(Color.FromArgb(75, 255, 0, 0)), 
                Width = 100,
                Height = 100,
            };
            this.IsPlaying = false;
            this.animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.75)),
                RepeatBehavior =  RepeatBehavior.Forever,
            };
        }

        public void Start(AttackType attackType)
        {
            if (IsPlaying)
                return;

            double distance = 100;

            canvas.Children.Add(arrow);

            switch (attackType)
            {
                case AttackType.UpToDown:
                    arrow.LayoutTransform = new RotateTransform(90);
                    Canvas.SetLeft(arrow, canvas.ActualWidth / 2 - arrow.Width / 2);
                    animation.From = arrow.Height;
                    animation.To = arrow.Height + distance;
                    arrow.BeginAnimation(Canvas.TopProperty, animation);  
                    break;
                case AttackType.LeftToRight:
                    arrow.LayoutTransform = null;
                    Canvas.SetTop(arrow, canvas.ActualHeight / 2 - arrow.Height / 2);
                    animation.From = arrow.Width;
                    animation.To = arrow.Width + distance;                    
                    arrow.BeginAnimation(Canvas.LeftProperty, animation);  
                    break;
                case AttackType.RightToLeft:
                    arrow.LayoutTransform = new RotateTransform(180);
                    Canvas.SetTop(arrow, canvas.ActualHeight / 2 - arrow.Height / 2);
                    animation.From = canvas.ActualWidth - arrow.Width;
                    animation.To = canvas.ActualWidth - distance - arrow.Width;
                    arrow.BeginAnimation(Canvas.LeftProperty, animation);  
                    break;
            }

            IsPlaying = true;
        }

        public void Stop()
        {
            if (!IsPlaying)
                return;
            arrow.BeginAnimation(Canvas.LeftProperty, null);
            arrow.BeginAnimation(Canvas.TopProperty, null);
            canvas.Children.Remove(arrow);
            IsPlaying = false;
        }
    }
}
