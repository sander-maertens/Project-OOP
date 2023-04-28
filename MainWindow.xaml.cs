using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SimonSays
{
    public partial class MainWindow
    {
        private readonly List<Color> _sequence = new List<Color>();
        private int _currentStep = 0;
        private readonly Random _random = new Random();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private bool _inputEnabled = false;

        public MainWindow()
        {
            InitializeComponent();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            NewGame();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private void NewGame()
        {
            _sequence.Clear();
            _currentStep = 0;
            AddStepToSequence();
            _inputEnabled = false;
            _timer.Start();
        }

        public void AddStepToSequence()
        {
            var colors = new[] { Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow };
            _sequence.Add(colors[_random.Next(colors.Length)]);
        }

        private void ShowSequence()
        {
            _inputEnabled = false;
            _currentStep = 0;
            var i = 0;
            _timer.Tick += (sender, args) =>
            {
                if (i < _sequence.Count)
                {
                    LightUpButton(_sequence[i]);
                    i++;
                }
                else
                {
                    _timer.Stop();
                    _inputEnabled = true;
                }
            };
            _timer.Start();
        }

        private void CheckInput(Color color)
        {
            if (!_inputEnabled) return;
            LightUpButton(color);
            if (_sequence[_currentStep] == color)
            {
                _currentStep++;
                if (_currentStep == _sequence.Count)
                {
                    AddStepToSequence();
                    ShowSequence();
                }
            }
            else
            {
                MessageBox.Show("Game over!");
                NewGame();
            }
        }

        public void LightUpButton(Color color)
        {
            var button = color switch
            {
                Colors.Red => RedButton,
                Colors.Blue => BlueButton,
                Colors.Green => GreenButton,
                Colors.Yellow => YellowButton,
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
            var originalColor = button.Background;
            button.Background = new SolidColorBrush(color);
            _timer.Tick += (sender, args) => button.Background = originalColor;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            if (!_inputEnabled)
            {
                LightUpButton(Colors.Black);
                ShowSequence();
            }
        }

        private void RedButton_Click(object sender, RoutedEventArgs e) => CheckInput(Colors.Red);

        private void BlueButton_Click(object sender, RoutedEventArgs e) => CheckInput(Colors.Blue);

        private void GreenButton_Click(object sender, RoutedEventArgs e) => CheckInput(Colors.Green);

        private void YellowButton_Click(object sender, RoutedEventArgs e) => CheckInput(Colors.Yellow);
    }
}
