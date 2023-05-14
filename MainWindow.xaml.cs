using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;
using Project_OOP;

namespace SimonSays
{

    public partial class MainWindow : Window
    {
        private List<Button> simonSaysButtons;
        private List<Button> playerButtons;
        private List<int> simonSaysSequence;
        private int currentSequenceIndex;
        private Random random;
        private DispatcherTimer timer;
        private int score;
        private Leaderboard leaderboard;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize lists of buttons and the random number generator
            simonSaysButtons = new List<Button> { RedButton, BlueButton, GreenButton, YellowButton };
            playerButtons = new List<Button>();
            simonSaysSequence = new List<int>();
            random = new Random();

            // Set up button event handlers
            RedButton.Click += Button_Click;
            BlueButton.Click += Button_Click;
            GreenButton.Click += Button_Click;
            YellowButton.Click += Button_Click;
            StartButton.Click += StartButton_Click;
            ResetButton.Click += ResetButton_Click;
            RefreshButton.Click += RefreshButton_Click;

            // Set up timer for Simon Says sequence playback
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);

            // Load the current leaderboard from the JSON file
            string filePath = "leaderboard.json";
            leaderboard = new Leaderboard();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);
            }

            // Bind the leaderboard data to the ListBox
            LeaderboardListBox.ItemsSource = leaderboard.Scores;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Reload the leaderboard from the JSON file and update the ListBox
            leaderboard = new Leaderboard();
            string filePath = "leaderboard.json";
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);
        }
            LeaderboardListBox.ItemsSource = leaderboard.Scores;
        }


        private void ShowLeaderboard()
        {
            // Load the current leaderboard from the JSON file
            string filePath = "leaderboard.json";
            Leaderboard leaderboard = new Leaderboard();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);
            }

            // Bind the leaderboard data to the ListBox
            LeaderboardListBox.ItemsSource = leaderboard.Scores;
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Add the button that was clicked to the player's list
            Button button = sender as Button;
            playerButtons.Add(button);

            // Check if the player's sequence matches the current Simon Says sequence
            bool isCorrect = true;
            for (int i = 0; i < playerButtons.Count; i++)
            {
                if (playerButtons[i] != simonSaysButtons[simonSaysSequence[i]])
                {
                    isCorrect = false;
                    break;
                }
            }

            // If the player's sequence was incorrect, end the game
            if (!isCorrect)
            {
                MessageBox.Show($"Game over! Your score is {score}");
                ResetGame();
            }

            // If the player's sequence matches the entire Simon Says sequence, add another button to the sequence and replay it
            if (playerButtons.Count == simonSaysSequence.Count)
            {
                score += 10;
                Console.WriteLine($"Score: {score}");
                AddToSimonSaysSequence();
                currentSequenceIndex = 0;
                timer.Start();
                playerButtons.Clear();
            }
        }



        private void AddToSimonSaysSequence()
        {
            // Add a random button to the Simon Says sequence
            int buttonIndex = random.Next(0, 4);
            simonSaysSequence.Add(buttonIndex);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Play back the current button in the Simon Says sequence and advance to the next button
            simonSaysButtons[simonSaysSequence[currentSequenceIndex]].Opacity = 1;
            currentSequenceIndex++;
            if (currentSequenceIndex >= simonSaysSequence.Count)
            {
                timer.Stop();
            }
            else
            {
                simonSaysButtons[simonSaysSequence[currentSequenceIndex - 1]].Opacity = 0.5;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Start a new game of Simon Says
            ResetGame();
            AddToSimonSaysSequence();
            currentSequenceIndex = 0;
            timer.Start();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset the game of Simon Says
            ResetGame();
        }
        private void ResetGame()
        {
            // Load the current leaderboard from the JSON file
            string filePath = "leaderboard.json";
            Leaderboard leaderboard = new Leaderboard();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);
            }
            else
            {
                leaderboard.Scores = new List<Score>();
            }

            // Save the player's score to the leaderboard if it is greater than 0
            if (score > 0)
            {
                // Add the player's score to the leaderboard
                leaderboard.Scores.Add(new Score("Player", score));

                // Sort the leaderboard by score (descending) and take the top 10 scores
                leaderboard.Scores = leaderboard.Scores.OrderByDescending(s => s.Value).Take(10).ToList();

                // Save the updated leaderboard to the JSON file
                string updatedJson = JsonConvert.SerializeObject(leaderboard);
                File.WriteAllText(filePath, updatedJson);
            }

            // Reset the game of Simon Says
            simonSaysSequence.Clear();
            playerButtons.Clear();
            timer.Stop();
            foreach (Button button in simonSaysButtons)
            {
                button.Opacity = 0.5;
            }
            score = 0;
        }



        private void SaveScoreToLeaderboard(int score)
        {
            // Save the player's score to the leaderboard
            string playerName = "Player"; // Change this to the actual player name
            Score playerScore = new Score(playerName, score);

            // Load the current leaderboard from the JSON file
            string filePath = "leaderboard.json";
            Leaderboard leaderboard = new Leaderboard();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);
            }
            else
            {
                leaderboard.Scores = new List<Score>();
        }

            // Add the player's score to the leaderboard
            leaderboard.Scores.Add(playerScore);

            // Sort the leaderboard by score (descending) and take the top 10 scores
            leaderboard.Scores = leaderboard.Scores.OrderByDescending(s => s.Value).Take(10).ToList();

            // Save the updated leaderboard to the JSON file
            string updatedJson = JsonConvert.SerializeObject(leaderboard);
            File.WriteAllText(filePath, updatedJson);
        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the opacity of all Simon Says buttons to 0.5
            foreach (Button button in simonSaysButtons)
            {
                button.Opacity = 0.5;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Clean up resources when the window is closing
            timer.Stop();
            timer.Tick -= Timer_Tick;
        }

    }
}
