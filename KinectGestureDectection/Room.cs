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
            Random gen = new Random();
            int result = gen.Next(3);
            if (result == 0)
            {
                currentTurn = new LeftSlashTurn();
            }
            else if (result == 1)
            {
                currentTurn = new RightSlashTurn();
            }
            else if (result == 2)
            {
                currentTurn = new DownSlashTurn();
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
            Random gen = new Random();
            int result = gen.Next(3);
            if (result == 0)
            {
                currentTurn = new HarmlessLeftSlashTurn();
            }
            else if (result == 1)
            {
                currentTurn = new HarmlessRightSlashTurn();
            }
            else if (result == 2)
            {
                currentTurn = new HarmlessDownSlashTurn();
            }
        }
    }
}
