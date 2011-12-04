using System;
using System.Collections.Generic;
using System.Linq;
using Kinect.Toolbox;
using Kinect.Toolbox.Record;
using Microsoft.Research.Kinect.Nui;

namespace KinectGestureDectection
{
    class CombatPostureDetector : PostureDetector
    {
        const float Epsilon = 0.2f;
        const float MaxRange = 0.25f;

        public CombatPostureDetector() : base(10) {}



        public override void TrackPostures(ReplaySkeletonData skeleton)
        {
            if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                return;

            Vector3? headPosition = null;
            Vector3? leftHandPosition = null;
            Vector3? rightHandPosition = null;

            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.Position.W < 0.8f || joint.TrackingState != JointTrackingState.Tracked)
                    continue;

                switch (joint.ID)
                {
                    case JointID.Head:
                        headPosition = joint.Position.ToVector3();
                        break;
                    case JointID.HandLeft:
                        leftHandPosition = joint.Position.ToVector3();
                        break;
                    case JointID.HandRight:
                        rightHandPosition = joint.Position.ToVector3();
                        break;
                }
            }

            if (AreHandsJoined(leftHandPosition, rightHandPosition))
            {
                RaisePostureDetected("HandsJoined");
                return;
            }

            Reset();
        }


        bool AreHandsJoined(Vector3? leftHandPosition, Vector3? rightHandPosition)
        {
            if (!leftHandPosition.HasValue || !rightHandPosition.HasValue)
                return false;

            float distance = (leftHandPosition.Value - rightHandPosition.Value).Length;

            if (distance > Epsilon)
                return false;

            return true;
        }      
    }
}
