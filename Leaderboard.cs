using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Project_OOP
{
    public class Leaderboard
    {
        public List<Score> Scores { get; set; }

        public Leaderboard()
        {
            Scores = new List<Score>();
        }

        public void AddScore(Score score)
        {
            Scores.Add(score);
            Scores.Sort((s1, s2) => s2.Value.CompareTo(s1.Value));
        }

        public void SaveToFile(string filePath)
        {
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(filePath, json);
        }

        public static Leaderboard LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Leaderboard>(json);
            }
            else
            {
                return new Leaderboard();
            }
        }
    }

    public class Score
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public Score(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }

}
