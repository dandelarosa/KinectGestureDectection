using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectGestureDectection
{
    public interface Turn
    {
        string GetPrompt();
        bool EnterGesture(string gesture);
        int GetDuration();
    }

    class HorizontalSlashTurn : Turn
    {
        public string GetPrompt()
        {
            return "Slash Horizontal";
        }

        public bool EnterGesture(string gesture)
        {
            if (gesture == "SlashLeftToRight" || gesture == "SlashRightToLeft")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetDuration()
        {
            return 5;
        }
    }
}
