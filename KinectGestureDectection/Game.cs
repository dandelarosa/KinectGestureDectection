using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectGestureDectection
{
    public class Game
    {
        // TODO set this to the first room in the game
        //private Room currentRoom = new SampleRoom();
        private Room currentRoom;

        private Room[,] gameMap = new Room[3,3];
        public int mapPositionX = 1;
        public int mapPositionY = 0;

        public int currentPlayerLife = 100;
        public int maxPlayerLife = 100;

        private int playerStrength = 10;

        public Game()
        {
            gameMap[0, 0] = new HarmlessObstacleRoom();
            gameMap[1, 0] = new HarmlessObstacleRoom();
            gameMap[2, 0] = new HarmlessObstacleRoom();
            gameMap[0, 1] = new HarmlessObstacleRoom();
            gameMap[1, 1] = new HarmlessObstacleRoom();
            gameMap[2, 1] = new HarmlessObstacleRoom();
            gameMap[0, 2] = new SampleRoom();
            gameMap[1, 2] = new SampleRoom();
            gameMap[2, 2] = new SampleRoom();
            currentRoom = gameMap[mapPositionX, mapPositionY];
        }

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

        public bool GoInDirection(PathSelectionComponent.PathDirection direction)
        {
            switch (direction)
            {
                case PathSelectionComponent.PathDirection.Back:
                    {
                        if (mapPositionY > 0) mapPositionY--;
                        else return false;
                    }
                    break;
                case PathSelectionComponent.PathDirection.Forward:
                    {
                        if (mapPositionY < 2) mapPositionY++;
                        else return false;
                    }
                    break;
                case PathSelectionComponent.PathDirection.Left:
                    {
                        if (mapPositionX > 0) mapPositionX--;
                        else return false;
                    }
                    break;
                case PathSelectionComponent.PathDirection.Right:
                    {
                        if (mapPositionX < 2) mapPositionX++;
                        else return false;
                    }
                    break;
            }
            currentRoom = gameMap[mapPositionX, mapPositionY];
            return true;
        }
    }
}
