using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Research.Kinect.Nui;
using Kinect.Toolbox;

namespace KinectGestureDectection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Runtime kinectRuntime;
        private Game game = new Game();
        private bool isGameStarted = false;

        private System.Windows.Threading.DispatcherTimer dispatcherTimer =
                new System.Windows.Threading.DispatcherTimer();

        private KinectManager kinectManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            if (kinectRuntime == null)
                return;

            kinectRuntime.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            kinectRuntime.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);

            kinectManager = new KinectManager(kinectRuntime, kinectDisplay, mainCanvas, kinectCanvas);
            kinectManager.SkeletonFound += new Action(kinectManager_SkeletonFound);
            kinectManager.CombatGestureDectected += new Action<string>(kinectManager_CombatGestureDectected);

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            PrintLine("Waiting to register skeleton...");
        }

        void kinectManager_CombatGestureDectected(string gesture)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.Ticks + " " + gesture);
            PrintLine(DateTime.Now.Ticks + " " + gesture);

            // Don't do anything to the game if it hasn't started yet
            if (!isGameStarted) return;

            // Process Gesture
            if (game.EnterGesture(gesture))
            {
                // If correct, go to the next turn
                dispatcherTimer.Stop();
                NextTurn();
            }
        }

        void kinectManager_SkeletonFound()
        {
            // If the skeleton is ready, start the game
            if (!isGameStarted)
            {
                isGameStarted = true;
                PrintLine("Game Started");
                NextTurn();
            }
        }

        private void NextTurn()
        {
            // Display current life 
            playerLife.Text = "Player Life: " + game.currentPlayerLife + "/" + game.maxPlayerLife;
            enemyLife.Text = "Enemy Life: " + game.GetCurrentEnemyLife() + "/" + game.GetMaxEnemyLife();
            // Print current life
            //PrintLine("Player Life: " + game.currentPlayerLife + "/" + game.maxPlayerLife);
            //PrintLine("Enemy Life: " + game.GetCurrentEnemyLife() + "/" + game.GetMaxEnemyLife());

            // Tell the game that it's the next turn
            game.NextTurn();

            string prompt = game.GetPrompt();

            // Print Prompt
            PrintLine(prompt);

            if (prompt == "Choose direction to go to")
            {
                kinectManager.CurrentState = KinectManager.InputState.PathSelection;
                kinectManager.SetPathEnabled(PathSelectionComponent.PathDirection.Back, false);
                kinectManager.SetPathEnabled(PathSelectionComponent.PathDirection.Left, false);
            }
            else
            {
                kinectManager.CurrentState = KinectManager.InputState.CombatPosture;
            }

            //  DispatcherTimer setup
            if (game.currentPlayerLife > 0 && game.GetCurrentEnemyLife() > 0)
            {
                int duration = game.GetTurnDuration();
                if (duration > 0)
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, duration);
                    dispatcherTimer.Start();
                }
                else
                {
                    dispatcherTimer.Stop();
                }
            }
            else
            {
                dispatcherTimer.Stop();
            }
        }

        /**
         * TODO
         */
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Tell the game that time is up
            PrintLine("Time's up.");

            // Hurt the player
            game.TimeUp();

            NextTurn();
        }

        private void PrintLine(string text)
        {
            textBlock1.Text += text + "\n";
            textScrollView.ScrollToEnd();
        }

        private void Clean()
        {
            if (kinectRuntime != null)
            {
                kinectRuntime.Uninitialize();
                kinectRuntime = null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Runtime kinect in Runtime.Kinects)
                {
                    if (kinect.Status == KinectStatus.Connected)
                    {
                        kinectRuntime = kinect;
                        break;
                    }
                }

                if (Runtime.Kinects.Count == 0)
                    MessageBox.Show("No Kinect Found");
                else
                    Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Clean();
        }

    }
}
