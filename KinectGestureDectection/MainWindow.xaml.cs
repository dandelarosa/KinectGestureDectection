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
        private KinectManager kinectManager;
        private AttackIndicator attackIndicator;
        private PathSelectionComponent pathSelector;
        private Game game = new Game();
        private bool isGameStarted = false;

        private System.Windows.Threading.DispatcherTimer dispatcherTimer =
                new System.Windows.Threading.DispatcherTimer();

        private string nextPrompt = String.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        void PostureDetector_PostureDetected(string obj)
        {
            kinectManager.CurrentState = KinectManager.InputState.Gesture;
            attackIndicator.Start(AttackIndicator.AttackType.RightToLeft);
            PrintLine(nextPrompt);
        }

        void GestureDetector_OnGestureDetected(string gesture)
        {
            //System.Diagnostics.Debug.WriteLine(DateTime.Now.Ticks + " " + gesture);
            //PrintLine(DateTime.Now.Ticks + " " + gesture);

            // Don't do anything to the game if it hasn't started yet
            if (!isGameStarted) return;

            // Process Gesture
            if (game.EnterGesture(gesture))
            {
                attackIndicator.Stop();
                // If correct, go to the next turn
                dispatcherTimer.Stop();
                NextTurn();
            }
        }

        void pathSelector_PathSelected(object sender, PathSelectionComponent.PathSelectedEventArgs e)
        {
            pathSelector.IsEnabled = false;
            //MessageBox.Show(e.Direction.ToString());
            // Go to the next room
            game.GoInDirection(e.Direction);
            // Start the room
            NextTurn();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectManager = new KinectManager(kinectDisplay, mainCanvas, kinectCanvas);
            kinectManager.SkeletonFound += new Action(kinectManager_SkeletonFound);
            kinectManager.Initialize();

            //This should probablly be moved into some sort of controller.
            //GestureDetectors and PostureDetectors can be swapped in and out if necessary.
            pathSelector = new PathSelectionComponent(mainCanvas);
            pathSelector.IsEnabled = false;
            pathSelector.PathSelected += new EventHandler<PathSelectionComponent.PathSelectedEventArgs>(pathSelector_PathSelected);
            kinectManager.KinectConnected += kinectManager_KinectConnected;
            kinectManager.PostureDetector = new CombatPostureDetector();
            kinectManager.PostureDetector.PostureDetected += new Action<string>(PostureDetector_PostureDetected);
            kinectManager.GestureDetector = new SimpleSlashGestureDetector();
            kinectManager.GestureDetector.OnGestureDetected += new Action<string>(GestureDetector_OnGestureDetected);
            kinectManager.GestureDetector.TraceTo(mainCanvas, System.Windows.Media.Color.FromRgb(255, 0, 0));
            kinectManager.GestureDetector.MinimalPeriodBetweenGestures = 2000;
            kinectManager.CursorUpdatable = pathSelector;
            /** **/

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            attackIndicator = new AttackIndicator(mainCanvas);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            kinectManager.Dispose();
        }

        void kinectManager_KinectConnected(Runtime kinect)
        {
            PrintLine("Waiting to register skeleton...");
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

            nextPrompt = game.GetPrompt();

            // DEBUG/TEMP FLOW HERE
            if (nextPrompt == "Choose direction to go to")
            {
                pathSelector.IsEnabled = true;
                kinectManager.CurrentState = KinectManager.InputState.Cursor;
            }
            else
            {
                kinectManager.CurrentState = KinectManager.InputState.Posture;
                PrintLine("Bring your hands together to swing your sword.");
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
    }
}
