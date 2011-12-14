using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Kinect.Toolbox;

namespace KinectGestureDectection
{
    class SimpleSlashGestureDetector : GestureDetector
    {
        protected const float SwipeMinimalLength = 0.50f;
        protected const int SwipeMinimalDuration = 100;
        protected const int SwipeMaximalDuration = 500;

        public AttackType AttackType { get; set; }

        public SimpleSlashGestureDetector()
        {
            this.AttackType = KinectGestureDectection.AttackType.SlashLeftToRight;
        }

        protected bool ScanPositions(Func<Vector3, Vector3, bool> directionFunction, Func<Vector3, Vector3, bool> lengthFunction, int minTime, int maxTime)
        {
            int start = 0;

            for (int index = 1; index < Entries.Count - 1; index++)
            {
                if (!directionFunction(Entries[index].Position, Entries[index + 1].Position))
                {
                    start = index;
                }

                if (lengthFunction(Entries[index].Position, Entries[start].Position))
                {
                    double totalMilliseconds = (Entries[index].Time - Entries[start].Time).TotalMilliseconds;
                    if (totalMilliseconds >= minTime && totalMilliseconds <= maxTime)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override void LookForGesture()
        {
            switch (AttackType)
            {
                case KinectGestureDectection.AttackType.SlashUpToDown:
                    if (ScanPositions(
                            (p1, p2) => p2.Y - p1.Y < 0.01f,
                            (p1, p2) => Math.Abs(p2.Y - p1.Y) > SwipeMinimalLength,
                            SwipeMinimalDuration, SwipeMaximalDuration))
                        RaiseGestureDetected("SlashUpToDown");
                    return;
                case KinectGestureDectection.AttackType.SlashLeftToRight:
                    if (ScanPositions(
                            (p1, p2) => p2.X - p1.X > -0.01f,
                            (p1, p2) => Math.Abs(p2.X - p1.X) > SwipeMinimalLength,
                            SwipeMinimalDuration, SwipeMaximalDuration))
                        RaiseGestureDetected("SlashLeftToRight");
                    return;
                case KinectGestureDectection.AttackType.SlashRightToLeft:
                    if (ScanPositions(
                            (p1, p2) => p2.X - p1.X < 0.01f,
                            (p1, p2) => Math.Abs(p2.X - p1.X) > SwipeMinimalLength,
                            SwipeMinimalDuration, SwipeMaximalDuration))
                        RaiseGestureDetected("SlashRightToLeft");
                    return;
            }

            /**
            if (ScanPositions(
                    (p1, p2) => p2.Y - p1.Y > -0.01f,
                    (p1, p2) => Math.Abs(p2.Y - p1.Y) > SwipeMinimalLength,
                    SwipeMinimalDuration, SwipeMaximalDuration))
            {
                RaiseGestureDetected("SlashDownToUp");
                return;
            }
            **/
        }
    }
}
