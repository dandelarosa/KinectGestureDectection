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

    class LeftSlashTurn : Turn
    {
        public string GetPrompt()
        {
            return "Slash Left";
        }

        public bool EnterGesture(string gesture)
        {
            if (gesture == "SlashRightToLeft")
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
            return 8;
        }
    }

    class RightSlashTurn : Turn
    {
        public string GetPrompt()
        {
            return "Slash Right";
        }

        public bool EnterGesture(string gesture)
        {
            if (gesture == "SlashLeftToRight")
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
            return 8;
        }
    }

    class DownSlashTurn : Turn
    {
        public string GetPrompt()
        {
            return "Slash Down";
        }

        public bool EnterGesture(string gesture)
        {
            if (gesture == "SlashUpToDown")
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
            return 8;
        }
    }

    class HarmlessHorizontalTurn : Turn
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
            return 0;
        }
    }

    class HarmlessLeftSlashTurn : Turn
    {
        public string GetPrompt()
        {
            return "Slash Left";
        }

        public bool EnterGesture(string gesture)
        {
            if (gesture == "SlashRightToLeft")
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
            return 0;
        }
    }

    class HarmlessRightSlashTurn : Turn
    {
        public string GetPrompt()
        {
            return "Slash Right";
        }

        public bool EnterGesture(string gesture)
        {
            if (gesture == "SlashLeftToRight")
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
            return 0;
        }
    }

    class HarmlessDownSlashTurn : Turn
    {
        public string GetPrompt()
        {
            return "Slash Down";
        }

        public bool EnterGesture(string gesture)
        {
            if (gesture == "SlashUpToDown")
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
            return 0;
        }
    }
}
