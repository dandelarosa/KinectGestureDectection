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

            mapBG = new Rectangle();
            mapBG.Height = 100;
            mapBG.Width = 100;
            Canvas.SetTop(mapBG, 100);
            Canvas.SetLeft(mapBG, 100);
            mapBG.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            
            mapTitle = new TextBlock();
            mapTitle.Text = "Map";
            Canvas.SetTop(mapTitle, 100);
            Canvas.SetLeft(mapTitle, 100);
            
            rooms = new Rectangle[Game.mapWidth, Game.mapHeight];

            for (int i = 0; i < Game.mapWidth; i++)
            {
                for (int j = 0; j < Game.mapHeight; j++)
                {
                    rooms[i, j] = new Rectangle();
                    rooms[i, j].Height = 10;
                    rooms[i, j].Width = 10;
                    Canvas.SetTop(rooms[i, j], 200 - 20 * j - 20); // y = 0 is map bottom
                    Canvas.SetLeft(rooms[i, j], 100 + 20 * i + 20);
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
                        rooms[i, j].Fill = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
                    }
                    else
                    {
                        rooms[i, j].Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
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
