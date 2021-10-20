using System;
using System.IO;
using Boggle.GameData;

namespace Boggle.Analysis
{
    public static class WordBank
    {
        private static DictionaryNode _baseNode;


        public static void Initialize()
        {
            string file = Path.Combine(GameDataManager.DataDir, "dictionary.txt");
            _baseNode = new DictionaryNode("");
            using (StreamReader reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    string word = reader.ReadLine().Trim().ToUpper();
                    if (string.IsNullOrEmpty(word)) continue;
                    _baseNode.Add(word);
                }
            }
        }

        public static bool IsWord(string word)
        {
            word = word.Trim().ToUpper();
            DictionaryNode node = _baseNode.GetNode(word);
            if (node == null) return false;
            return node.IsWord;
        }

        public static bool IsStartOfWord(string word)
        {
            word = word.Trim().ToUpper();
            DictionaryNode node = _baseNode.GetNode(word);
            return node != null;
        }
    }
}
