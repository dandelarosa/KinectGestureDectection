using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinectGestureDectection
{
    class LifeDisplay
    {
        private Canvas canvas;
        private Game game;
        private Rectangle playerBarBG = new Rectangle();
        private Rectangle enemyBarBG = new Rectangle();
        private Rectangle playerBarFG = new Rectangle();
        private Rectangle enemyBarFG = new Rectangle();
        private TextBlock playerLife = new TextBlock();
        private TextBlock enemyLife = new TextBlock();
        
        public LifeDisplay (Canvas canvas, Game game)
        {
            this.canvas = canvas;
            this.game = game;

            int healthBarY = 15;
            int healthBarWidth = 280;
            int healthBarHeight = 40;

            int playerHealthX = 22;
            Canvas.SetLeft(playerBarBG, playerHealthX);
            Canvas.SetTop(playerBarBG, healthBarY);
            playerBarBG.Width = healthBarWidth;
            playerBarBG.Height = healthBarHeight;
            playerBarBG.Fill = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            canvas.Children.Add(playerBarBG);
            Canvas.SetLeft(playerBarFG, playerHealthX);
            Canvas.SetTop(playerBarFG, healthBarY);
            playerBarFG.Width = healthBarWidth;
            playerBarFG.Height = healthBarHeight;
            playerBarFG.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
            canvas.Children.Add(playerBarFG);
            Canvas.SetLeft(playerLife, playerHealthX + 10);
            Canvas.SetTop(playerLife, healthBarY);
            playerLife.Text = "Player Life";
            playerLife.Width = healthBarWidth;
            playerLife.Height = healthBarHeight;
            playerLife.FontSize = 28;
            playerLife.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            canvas.Children.Add(playerLife);

            int enemyHealthX = 338;
            Canvas.SetLeft(enemyBarBG, enemyHealthX);
            Canvas.SetTop(enemyBarBG, healthBarY);
            enemyBarBG.Width = healthBarWidth;
            enemyBarBG.Height = healthBarHeight;
            enemyBarBG.Fill = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            canvas.Children.Add(enemyBarBG);
            Canvas.SetRight(enemyBarFG, playerHealthX);
            Canvas.SetTop(enemyBarFG, healthBarY);
            enemyBarFG.Width = healthBarWidth;
            enemyBarFG.Height = healthBarHeight;
            enemyBarFG.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
            canvas.Children.Add(enemyBarFG);
            Canvas.SetLeft(enemyLife, enemyHealthX + 10);
            Canvas.SetTop(enemyLife, healthBarY);
            enemyLife.Text = "Enemy Life";
            enemyLife.Width = healthBarWidth;
            enemyLife.Height = healthBarHeight;
            enemyLife.FontSize = 28;
            enemyLife.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            canvas.Children.Add(enemyLife);
        }

        public void update ()
        {
            playerBarFG.Width = playerBarBG.Width * game.currentPlayerLife / game.maxPlayerLife;
            if (game.currentPlayerLife > 0)
            {
                playerLife.Text = "Player Life: " + game.currentPlayerLife + "/" + game.maxPlayerLife;
            }
            else
            {
                playerLife.Text = "Game Over";
            }

            enemyBarFG.Width = enemyBarBG.Width * game.GetCurrentEnemyLife() / game.GetMaxEnemyLife();
            if (game.GetCurrentEnemyLife() > 0)
            {
                enemyLife.Text = "Enemy Life: " + game.GetCurrentEnemyLife() + "/" + game.GetMaxEnemyLife();
            }
            else
            {
                enemyLife.Text = "Enemy Defeated!";
            }
        }
    }
}
