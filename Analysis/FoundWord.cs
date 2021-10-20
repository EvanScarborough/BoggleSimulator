using System;
using System.Collections.Generic;
using Boggle.BoggleData;

namespace Boggle.Analysis
{
    public class FoundWord
    {
        public string Word { get; private set; }
        public List<LetterDie> Dice { get; private set; }

        public FoundWord(string word, List<LetterDie> dice)
        {
            Word = word;
            Dice = dice;
        }
    }
}
