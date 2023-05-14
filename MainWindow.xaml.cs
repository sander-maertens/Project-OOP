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
        private string playerName;


        public MainWindow()
        {
            InitializeComponent();

            // Initialiseer lijsten met knoppen en de generator voor willekeurige getallen
            simonSaysButtons = new List<Button> { RedButton, BlueButton, GreenButton, YellowButton };
            playerButtons = new List<Button>();
            simonSaysSequence = new List<int>();
            random = new Random();

            // Stel handlers voor knopgebeurtenissen in
            RedButton.Click += Button_Click;
            BlueButton.Click += Button_Click;
            GreenButton.Click += Button_Click;
            YellowButton.Click += Button_Click;
            StartButton.Click += StartButton_Click;
            ResetButton.Click += ResetButton_Click;
            RefreshButton.Click += RefreshButton_Click;

            // Stel een timer in voor het afspelen van Simon Says-reeksen
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);

            // Laad het huidige leaderboard uit het JSON-bestand
            string filePath = "leaderboard.json";
            leaderboard = new Leaderboard();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);
            }

            // Bind de leaderboard-gegevens aan de ListBox
            LeaderboardListBox.ItemsSource = leaderboard.Scores;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Laad het leaderboard opnieuw vanuit het JSON-bestand en werk de ListBox bij
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
            // Laad het huidige leaderboard uit het JSON-bestand
            string filePath = "leaderboard.json";
            Leaderboard leaderboard = new Leaderboard();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);
            }

            // Bind de leaderboard-gegevens aan de ListBox
            LeaderboardListBox.ItemsSource = leaderboard.Scores;
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Voeg de knop waarop is geklikt toe aan de spelerslijst
            Button button = sender as Button;
            playerButtons.Add(button);

            // Controleer of de volgorde van de speler overeenkomt met de huidige volgorde van
            // Simon Says
            bool isCorrect = true;
            for (int i = 0; i < playerButtons.Count; i++)
            {
                if (playerButtons[i] != simonSaysButtons[simonSaysSequence[i]])
                {
                    isCorrect = false;
                    break;
                }
            }

            // Als de volgorde van de speler onjuist was, beëindigt u het spel
            if (!isCorrect)
            {
                MessageBox.Show($"Game over! Your score is {score}");
                ResetGame();
            }

            // Als de volgorde van de speler overeenkomt met de hele reeks van Simon Says, voegt u nog
            // een knop toe aan de reeks en speelt u deze opnieuw af
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
            // Voeg een willekeurige knop toe aan de Simon Says-reeks
            int buttonIndex = random.Next(0, 4);
            simonSaysSequence.Add(buttonIndex);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Speel de huidige knop af in de Simon Says-reeks en ga door naar de volgende knop
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
            // Start een nieuw spel Simon Says
            ResetGame();
            AddToSimonSaysSequence();
            currentSequenceIndex = 0;
            timer.Start();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset het spel van Simon Says
            ResetGame();
        }
        private void ResetGame()
        {
            // Laad het huidige leaderboard uit het JSON-bestand
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

            // Bewaar de score van de speler op het scorebord als deze groter is dan 0
            if (score > 0)
            {
                // Add the player's score to the leaderboard
                leaderboard.Scores.Add(new Score("Sander", score));

                // Sorteer het leaderboard op score (aflopend) en neem de top 10 scores
                leaderboard.Scores = leaderboard.Scores.OrderByDescending(s => s.Value).Take(10).ToList();

                // Sla het bijgewerkte leaderboard op in het JSON-bestand
                string updatedJson = JsonConvert.SerializeObject(leaderboard);
                File.WriteAllText(filePath, updatedJson);
            }

            // Reset het spel van Simon Says
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
            // Bewaar de score van de speler op het leaderboard
            Score playerScore = new Score(playerName, score);

            // Laad het huidige leaderboard uit het JSON-bestand
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

            // Voeg de score van de speler toe aan het leaderboard
            leaderboard.Scores.Add(playerScore);

            // Sorteer het leaderboard op score (aflopend) en neem de top 10 scores
            leaderboard.Scores = leaderboard.Scores.OrderByDescending(s => s.Value).Take(10).ToList();

            // Sla het bijgewerkte leaderboard op in het JSON-bestand
            string updatedJson = JsonConvert.SerializeObject(leaderboard);
            File.WriteAllText(filePath, updatedJson);
        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Stel de dekking van alle Simon Says-knoppen in op 0,5
            foreach (Button button in simonSaysButtons)
            {
                button.Opacity = 0.5;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Ruim bronnen op wanneer het venster wordt gesloten
            timer.Stop();
            timer.Tick -= Timer_Tick;
        }

    }
}