using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectGestureDectection
{
    public abstract class Room
    {
        protected Turn currentTurn;
        public int currentEnemyLife = 100;
        public int maxEnemyLife = 100;
        public int enemyStrength = 10;

        public abstract void NextTurn();

        public string GetPrompt()
        {
            if (currentEnemyLife > 0)
            {
                return currentTurn.GetPrompt();
            }
            else
            {
                return "Choose direction to go to";
            }
        }

        public int GetTurnDuration()
        {
            if (currentEnemyLife > 0)
            {
                return currentTurn.GetDuration();
            }
            else
            {
                return 0;
            }
        }

        public bool EnterGesture(string gesture)
        {
            return currentTurn.EnterGesture(gesture);
        }
    }

    public class SampleRoom : Room
    {
        public override void NextTurn()
        {
            // Ideally, we would a diverse sequence of turns, but for now, 
            // just have the game ask for horizontal slashes each turn
            Random gen = new Random();
            int result = gen.Next(2);
            if (result == 0)
            {
                currentTurn = new LeftSlashTurn();
            }
            else
            {
                currentTurn = new RightSlashTurn();
            }
        }
    }

    public class HarmlessObstacleRoom : Room
    {
        public HarmlessObstacleRoom()
        {
            currentEnemyLife = 30;
            maxEnemyLife = 30;
            enemyStrength = 0;
        }
        public override void NextTurn()
        {
            // Ideally, we would a diverse sequence of turns, but for now, 
            // just have the game ask for horizontal slashes each turn
            Random gen = new Random();
            int result = gen.Next(2);
            if (result == 0)
            {
                currentTurn = new HarmlessLeftSlashTurn();
            }
            else
            {
                currentTurn = new HarmlessRightSlashTurn();
            }
        }
    }
}
