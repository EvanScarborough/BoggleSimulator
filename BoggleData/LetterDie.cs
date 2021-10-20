using System;
using System.Collections.Generic;
using Boggle.GameData;
using Boggle.Helpers;

namespace Boggle.BoggleData
{
    public class LetterDie
    {
        public List<string> Sides { get; private set; } = new List<string>();
        public int SideOn { get; private set; } = 0;
        public string Letter => GetSide(SideOn);

        public string GetSide(int i)
        {
            if (i < 0 || i >= Sides.Count) return "";
            return Sides[i];
        }

        public void Roll()
        {
            SideOn = RNG.Rand(0, Sides.Count - 1);
        }

        public LetterDie(List<GameAttribute> atts)
        {
            foreach (GameAttribute a in atts)
            {
                Sides.Add((string)a);
            }
            Roll();
        }
    }
}
