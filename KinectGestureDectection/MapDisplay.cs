using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinectGestureDectection
{
    class MapDisplay
    {
        private Canvas canvas;
        private Game game;
        private Rectangle mapBG;
        private Rectangle[,] rooms;
        private TextBlock mapTitle;

        public MapDisplay (Canvas canvas, Game game)
        {
            this.canvas = canvas;
            this.game = game;

            // TODO don't know if this should be hardcoded or calculated by some other means
            int panelWidth = 300;
            int panelHeight = 300;

            // Calculate map position
            int panelLeftX = ((int)canvas.Width - panelWidth) >> 1;
            int panelTopY = ((int)canvas.Height - panelHeight) >> 1;

            mapBG = new Rectangle();
            mapBG.Height = panelHeight;
            mapBG.Width = panelWidth;
            Canvas.SetTop(mapBG, panelTopY);
            Canvas.SetLeft(mapBG, panelLeftX);
            mapBG.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

            int titleHeight = 50;
            mapTitle = new TextBlock();
            mapTitle.Text = "Map";
            mapTitle.FontSize = 28;
            mapTitle.Width = panelWidth;
            mapTitle.Height = titleHeight;
            Canvas.SetTop(mapTitle, panelTopY);
            Canvas.SetLeft(mapTitle, panelLeftX);
            mapTitle.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            
            rooms = new Rectangle[Game.mapWidth, Game.mapHeight];
            int gapWidth = 20;
            int cellWidth = (panelWidth - (gapWidth * (Game.mapWidth + 1))) / Game.mapWidth;
            int gapHeight = 20;
            int cellHeight = (panelHeight - (gapHeight * (Game.mapHeight + 1)) - titleHeight) / Game.mapHeight;

            for (int i = 0; i < Game.mapWidth; i++)
            {
                for (int j = 0; j < Game.mapHeight; j++)
                {
                    rooms[i, j] = new Rectangle();
                    rooms[i, j].Height = cellHeight;
                    rooms[i, j].Width = cellWidth;
                    // y = 0 is map bottom
                    Canvas.SetTop(rooms[i, j], panelTopY + panelHeight - j * (cellHeight + gapHeight) - gapHeight - cellHeight);
                    Canvas.SetLeft(rooms[i, j], panelLeftX + i * (cellWidth + gapWidth) + gapWidth);
                }
            }
        }

        public void Show ()
        {
            canvas.Children.Add(mapBG);
            canvas.Children.Add(mapTitle);
            for (int i = 0; i < Game.mapWidth; i++)
            {
                for (int j = 0; j < Game.mapHeight; j++)
                {
                    if (i == game.mapPositionX && j == game.mapPositionY)
                    {
                        rooms[i, j].Fill = new SolidColorBrush(Color.FromArgb(255, 0, 128, 0));
                    }
                    else if (game.gameMap[i, j].currentEnemyLife > 0)
                    {
                        rooms[i, j].Fill = new SolidColorBrush(Color.FromArgb(255, 128, 0, 0));
                    }
                    else
                    {
                        rooms[i, j].Fill = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
                    }
                    canvas.Children.Add(rooms[i, j]);
                }
            }
        }

        public void Hide ()
        {
            canvas.Children.Remove(mapBG);
            canvas.Children.Remove(mapTitle);
            for (int i = 0; i < Game.mapWidth; i++)
            {
                for (int j = 0; j < Game.mapHeight; j++)
                {
                    canvas.Children.Remove(rooms[i, j]);
                }
            }
        }
    }
}
