using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectGestureDectection
{
    public class Game
    {
        private TurnInfo currentTurnInfo;

        public int currentLife = 100;
        public int maxLife = 100;

        public void NextTurn()
        {
            // Ideally, we would a diverse sequence of turns, but for now, 
            // just have the game ask for horizontal slashes each turn
            currentTurnInfo = new HorizontalSlashTurn();
        }

        public string GetPrompt()
        {
            return currentTurnInfo.GetPrompt();
        }

        public bool EnterGesture(string gesture)
        {
            return currentTurnInfo.EnterGesture(gesture);
        }
    }

    interface TurnInfo
    {
        string GetPrompt();
        bool EnterGesture(string gesture);
    }

    class HorizontalSlashTurn : TurnInfo
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
    }
}
