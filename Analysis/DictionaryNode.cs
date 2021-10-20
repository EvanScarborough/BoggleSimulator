using System;
using System.Collections.Generic;

namespace Boggle.Analysis
{
    public class DictionaryNode
    {
        public string Name { get; private set; }
        public bool IsWord { get; private set; }
        private Dictionary<char, DictionaryNode> _nodes = new Dictionary<char, DictionaryNode>();

        public DictionaryNode(string baseName, char? letter = null)
        {
            Name = baseName;
            if (letter.HasValue) Name += letter.Value;
        }

        public void Add(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                // that was the end of the word, so this is a word!
                IsWord = true;
                return;
            }
            char firstLetter = word[0];
            string rest = word.Substring(1);

            if (!_nodes.ContainsKey(firstLetter))
            {
                _nodes.Add(firstLetter, new DictionaryNode(Name, firstLetter));
            }
            _nodes[firstLetter].Add(rest);
        }

        public DictionaryNode GetNode(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return this;
            }
            char firstLetter = word[0];
            string rest = word.Substring(1);
            if (!_nodes.ContainsKey(firstLetter)) return null;
            return _nodes[firstLetter].GetNode(rest);
        }
    }
}
