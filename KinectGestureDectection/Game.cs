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

        public void NextTurn()
        {
            currentRoom.NextTurn();
        }

        public string GetPrompt()
        {
            return currentRoom.GetPrompt();
        }

        public bool EnterGesture(string gesture)
        {
            return currentRoom.EnterGesture(gesture);
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
