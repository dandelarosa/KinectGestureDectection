using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectGestureDectection
{
    public abstract class Room
    {
        protected Turn currentTurn;
        public int currentEnemyLife;
        public int maxEnemyLife;

        public abstract void NextTurn();
        public abstract string GetPrompt();
        public abstract bool EnterGesture(string gesture);
    }

    public class SampleRoom : Room
    {
        public SampleRoom()
        {
            currentEnemyLife = 100;
            maxEnemyLife = 100;
        }

        public override void NextTurn()
        {
            // Ideally, we would a diverse sequence of turns, but for now, 
            // just have the game ask for horizontal slashes each turn
            currentTurn = new HorizontalSlashTurn();
        }
        public override string GetPrompt()
        {
            return currentTurn.GetPrompt();
        }
        public override bool EnterGesture(string gesture)
        {
            return currentTurn.EnterGesture(gesture);
        }
    }
}
