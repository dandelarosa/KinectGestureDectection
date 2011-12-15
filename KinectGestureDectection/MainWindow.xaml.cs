using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Research.Kinect.Nui;
using Kinect.Toolbox;
using System.Windows.Controls;

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
        private MapDisplay mapDisplay;
        private LifeDisplay lifeDisplay;
        private Game game = new Game();
        private bool isGameStarted = false;


        private System.Windows.Threading.DispatcherTimer dispatcherTimer =
                new System.Windows.Threading.DispatcherTimer();

        private string nextPrompt = String.Empty;
        private SimpleSlashGestureDetector slashDetector = new SimpleSlashGestureDetector();

        public MainWindow()
        {
            InitializeComponent();
        }

        void PostureDetector_PostureDetected(string obj)
        {
            kinectManager.CurrentState = KinectManager.InputState.Gesture;
            swordCursor.Visibility = System.Windows.Visibility.Visible;
            if (nextPrompt == "Slash Left")
            {
                attackIndicator.Start(AttackType.SlashRightToLeft);
                slashDetector.AttackType = AttackType.SlashRightToLeft;
            }
            else if (nextPrompt == "Slash Right")
            {
                attackIndicator.Start(AttackType.SlashLeftToRight);
                slashDetector.AttackType = AttackType.SlashLeftToRight;
            }
            else if (nextPrompt == "Slash Down")
            {
                attackIndicator.Start(AttackType.SlashUpToDown);
                slashDetector.AttackType = AttackType.SlashUpToDown;
            }
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
                swordCursor.Visibility = System.Windows.Visibility.Hidden;
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
            bool didGo = game.GoInDirection(e.Direction);
            if (didGo) mapDisplay.Hide();
            if (!didGo) PrintLine("Invalid Direction.");
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
            kinectManager.GestureDetector = slashDetector;
            kinectManager.GestureDetector.OnGestureDetected += new Action<string>(GestureDetector_OnGestureDetected);
            kinectManager.GestureDetector.TraceTo(mainCanvas, System.Windows.Media.Color.FromRgb(255, 0, 0));
            kinectManager.GestureDetector.MinimalPeriodBetweenGestures = 2000;
            kinectManager.GestureUpdate += new Action<Point>(kinectManager_GestureUpdate);
            kinectManager.CursorUpdate += new Action<Point, Point>(kinectManager_CursorUpdate);
            kinectManager.CursorUpdatable = pathSelector;
            /** **/

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            attackIndicator = new AttackIndicator(mainCanvas);
            mapDisplay = new MapDisplay(mainCanvas, game);
            lifeDisplay = new LifeDisplay(mainCanvas, game);
        }

        void kinectManager_CursorUpdate(Point leftHandPosition, Point rightHandPosition)
        {
            Canvas.SetLeft(leftHand, leftHandPosition.X);
            Canvas.SetTop(leftHand, leftHandPosition.Y);
            Canvas.SetLeft(rightHand, rightHandPosition.X);
            Canvas.SetTop(rightHand, rightHandPosition.Y);
        }

        void kinectManager_GestureUpdate(Point obj)
        {
            Canvas.SetLeft(swordCursor, obj.X);
            Canvas.SetTop(swordCursor, obj.Y);
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
            // Turn off attack indicator
            attackIndicator.Stop();

            // Display current life 
            lifeDisplay.update();
            
            // Tell the game that it's the next turn
            game.NextTurn();

            nextPrompt = game.GetPrompt();

            // DEBUG/TEMP FLOW HERE
            if (nextPrompt == "Choose direction to go to")
            {
                mapDisplay.Show();
                pathSelector.IsEnabled = true;
                if (game.mapPositionX == 0)
                    pathSelector.SetTargetEnabled(PathSelectionComponent.PathDirection.Left, false);
                else if (game.mapPositionX == 2)
                    pathSelector.SetTargetEnabled(PathSelectionComponent.PathDirection.Right, false);
                if (game.mapPositionY == 0)
                    pathSelector.SetTargetEnabled(PathSelectionComponent.PathDirection.Back, false);
                else if (game.mapPositionY == 2)
                    pathSelector.SetTargetEnabled(PathSelectionComponent.PathDirection.Forward, false);
                kinectManager.CurrentState = KinectManager.InputState.Cursor;
                leftHand.Visibility = System.Windows.Visibility.Visible;
                rightHand.Visibility = System.Windows.Visibility.Visible;
                enemyImage.Visibility = System.Windows.Visibility.Hidden;
                PrintLine("Choose direction to go to");
            }
            else
            {
                kinectManager.CurrentState = KinectManager.InputState.Posture;
                leftHand.Visibility = System.Windows.Visibility.Hidden;
                rightHand.Visibility = System.Windows.Visibility.Hidden;
                enemyImage.Visibility = System.Windows.Visibility.Visible;
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
            PrintLine("Too Late!");

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
