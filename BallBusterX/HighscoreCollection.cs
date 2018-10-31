using AgateLib.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace BallBusterX
{
    public class HighscoreCollection
    {
        private List<Highscore> highscores = new List<Highscore>();
        private readonly UserStorage storage;

        public HighscoreCollection(UserStorage storage)
        {
            this.storage = storage;

            SetDefaultHighscores();
        }

        public const int MaxCount = 7;

        public int Count => highscores.Count;

        public Highscore this[int index] => highscores[index];

        public IReadOnlyList<Highscore> Highscores => highscores;

        public bool IsNewHighscore(int score)
        {
            if (score <= 0)
                return false;

            if (Count < MaxCount)
                return true;

            for (int i = 0; i < highscores.Count; i++)
            {
                if (score > highscores[i].score)
                {
                    return true;
                }
            }

            return false;
        }

        public void SaveHighscores()
        {
            using (var hs = new StreamWriter(storage.OpenFile("highscores", FileMode.Create)))
            {
                for(int i = 0; i < 9 && i < highscores.Count; i ++)
                {
                    hs.WriteLine(highscores[i].ToString());
                }
            }
        }

        public void LoadHighscores()
        {
            try
            {
                using (var hs = new StreamReader(storage.OpenFile("highscores", FileMode.Open)))
                {
                    highscores.Clear();

                    while (hs.EndOfStream == false)
                    {
                        Highscore myscore;

                        string buffer = hs.ReadLine();

                        if (buffer[0] != 0)
                        {
                            myscore = new Highscore(buffer);

                            highscores.Add(myscore);
                        }
                    }
                }
            }
            catch
            {
                // highscores file isn't there or is corrupted.. 
                SetDefaultHighscores();

                return;
            }
        }

        private void SetDefaultHighscores()
        {
            highscores.Clear();

            highscores.Add(new Highscore("Wizard", 200000));
            highscores.Add(new Highscore("Skel1",  100000));
            highscores.Add(new Highscore("Ariana",  50000));
            highscores.Add(new Highscore("John",    25000));
            highscores.Add(new Highscore("Dave",    10000));
            highscores.Add(new Highscore("Scott",    5000));
        }

        public void Insert(int index, Highscore newScore)
        {
            highscores.Insert(index, newScore);

            while (highscores.Count > MaxCount)
                highscores.RemoveAt(highscores.Count - 1);
        }
    }
}