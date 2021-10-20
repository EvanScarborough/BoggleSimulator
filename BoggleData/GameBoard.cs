using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Boggle.Analysis;
using Boggle.GameData;
using Boggle.GameUI;
using Boggle.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boggle.BoggleData
{
    public class GameBoard
    {
        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public List<LetterDie> Dice { get; private set; } = new List<LetterDie>();

        public List<List<LetterDie>> Board { get; private set; } = null;

        public int TileSize => (Global.SCREEN_HEIGHT - (OFFSET * 2)) / Math.Max(Width, Height);
        public static readonly int OFFSET = 10;

        private double _rotation = 0;
        public double TargetRotation { get; set; } = 0;

        public List<FoundWord> Words { get; private set; }
        public string LongestWord => Words.Count > 0 ? Words[0].Word : "";

        public GameBoard(AttributeSet atts)
        {
            Name = (string)atts["name"];
            Width = (int)atts["size"][0];
            Height = (int)atts["size"][1];
            foreach (GameAttribute die in atts["dice"])
            {
                Dice.Add(new LetterDie((List<GameAttribute>)die));
            }
            SetBoard();
        }

        public void Shuffle()
        {
            SetBoard();
            TargetRotation += 3.14159 * 6;
        }

        private void SetBoard()
        {
            Board = new List<List<LetterDie>>();
            HashSet<int> di = new HashSet<int>();

            for(int i = 0; i < Width; i++)
            {
                Board.Add(new List<LetterDie>());
                for(int j = 0; j < Height; j++)
                {
                    int ind = 0;
                    int tries = 0;
                    do
                    {
                        ind = RNG.Rand(0, Dice.Count - 1);
                        tries++;
                    }
                    while (di.Contains(ind) && tries < 1000);
                    Dice[ind].Roll();
                    Board[i].Add(Dice[ind]);
                    di.Add(ind);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, FoundWord highlight = null) =>
            Draw(spriteBatch, Global.SCREEN_HEIGHT / 2, Global.SCREEN_HEIGHT / 2, highlight);
        public void Draw(SpriteBatch spriteBatch, int xc, int yc, FoundWord highlight = null)
        {
            if (Board == null) return;

            _rotation += (TargetRotation - _rotation) * 0.1;
            if(TargetRotation > 100)
            {
                TargetRotation -= 3.14159 * 20;
                _rotation -= 3.14159 * 20;
            }

            double a, b, xp, yp;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    a = ((double)Width / 2.0 - (double)i - 0.5) * TileSize;
                    b = ((double)Height / 2.0 - (double)j - 0.5) * TileSize;
                    xp = xc + a * Math.Cos(_rotation) + b * Math.Sin(_rotation);
                    yp = yc + a * Math.Sin(_rotation) - b * Math.Cos(_rotation);
                    bool h = highlight != null && highlight.Dice.Contains(Board[i][j]);
                    UI.DrawTile(spriteBatch, new Rectangle((int)(xp - TileSize / 2 + 1), (int)(yp - TileSize / 2 + 1), TileSize - 2, TileSize - 2), Board[i][j].Letter, h ? UI.HighlightColor : Color.White, Color.Black, 1);
                }
            }
        }


        public bool IsValid(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void Analyze()
        {
            Words = new List<FoundWord>();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    FindWords("", i, j, new List<LetterDie>());
                }
            }
            Words = Words.OrderByDescending(w => w.Word.Length).ToList();
        }
        public void FindWords(string wordSoFar, int x, int y, List<LetterDie> visited)
        {
            if (WordBank.IsWord(wordSoFar) && wordSoFar.Length >= 3)
            {
                if(!Words.Exists(w => w.Word == wordSoFar.Trim().ToUpper()))
                    Words.Add(new FoundWord(wordSoFar.Trim().ToUpper(), visited));
            }
            if (!WordBank.IsStartOfWord(wordSoFar)) return;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (!IsValid(i, j)) continue;
                    if (visited.Contains(Board[i][j])) continue;
                    List<LetterDie> newVisited = new List<LetterDie>(visited);
                    newVisited.Add(Board[i][j]);
                    FindWords(wordSoFar + Board[i][j].Letter, i, j, newVisited);
                }
            }
        }





        public void RunFullAnalysis(int runs = 1000)
        {
            Dictionary<string, int> found = new Dictionary<string, int>();
            for (int i = 0; i < runs; i++)
            {
                Console.WriteLine(i);
                Shuffle();
                Analyze();
                foreach (FoundWord word in Words)
                {
                    if (!found.ContainsKey(word.Word)) found.Add(word.Word, 1);
                    else found[word.Word]++;
                }
            }
            List<KeyValuePair<string, int>> w = found.OrderByDescending(f => f.Value).ToList();
            using(StreamWriter writer = new StreamWriter("analysis.csv"))
            {
                foreach (KeyValuePair<string, int> p in w)
                {
                    writer.WriteLine(p.Key + "," + p.Value.ToString());
                }
            }
        }

        public void GetLongWord(int letters)
        {
            int i = 0;
            do
            {
                Shuffle();
                Analyze();
                i++;
            }
            while (LongestWord.Length < letters && i < 50);
        }
    }
}
