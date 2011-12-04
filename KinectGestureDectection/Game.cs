using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectGestureDectection
{
    public class Game
    {
        // TODO set this to the first room in the game
        private Room currentRoom = new SampleRoom();

        public int currentPlayerLife = 100;
        public int maxPlayerLife = 100;

        private int playerStrength = 10;

        public void NextTurn()
        {
            currentRoom.NextTurn();
        }

        public string GetPrompt()
        {
            if (currentPlayerLife > 0)
            {
                return currentRoom.GetPrompt();
            }
            else
            {
                return "Game Over";
            }
        }

        public int GetTurnDuration()
        {
            return currentRoom.GetTurnDuration();
        }

        public int GetCurrentEnemyLife()
        {
            return currentRoom.currentEnemyLife;
        }

        public int GetMaxEnemyLife()
        {
            return currentRoom.maxEnemyLife;
        }

        public bool EnterGesture(string gesture)
        {
            bool result = currentRoom.EnterGesture(gesture);
            if (result == true)
            {
                currentRoom.currentEnemyLife -= playerStrength;
            }
            return result;
        }

        public void TimeUp()
        {
            currentPlayerLife -= currentRoom.enemyStrength;
        }

        public void GoInDirection(string direction)
        {
            // TODO
            // This function will look for a room placed in the selected 
            // direction from the current room and go there
            // e.g. currentRoom = <something>;
            return;
        }
    }
}
