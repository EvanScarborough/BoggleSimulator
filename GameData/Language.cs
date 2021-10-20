using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Boggle.GameData
{
    public static class Language
    {
        //public static string LangPath => Path.Combine(GameDataManager.DataDir, "lang");
        //public static List<string> Languages { get; private set; } = new List<string>();
        //private static Dictionary<string, string> _strings = new Dictionary<string, string>();

        //public static bool Initialize()
        //{
        //    Languages = Directory.GetFiles(LangPath).Select(f => Path.GetFileNameWithoutExtension(f)).ToList();
        //    if (!LoadLanguage(GameDataManager.Lang))
        //    {
        //        return LoadLanguage("English_US");
        //    }
        //    return true;
        //}

        //public static bool LoadLanguage(string name)
        //{
        //    _strings.Clear();
        //    try
        //    {
        //        string[] lines = File.ReadAllLines(Path.Combine(LangPath, name + ".txt"));
        //        foreach (string line in lines)
        //        {
        //            if (line.Length == 0) continue;
        //            int split = Math.Max(line.IndexOf(' '), line.IndexOf('\t'));
        //            if (split <= 0) continue;
        //            string key = line.Substring(0, split).ToLower();
        //            string val = line.Substring(split).Trim();
        //            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val)) continue;
        //            if (!_strings.ContainsKey(key)) _strings.Add(key, val);
        //            else _strings[key] = val;
        //        }
        //        return true;
        //    }
        //    catch { return false; }
        //}

        //public static bool Has(string key) => _strings.ContainsKey(key);

        //public static string Get(string key, string def = "")
        //{
        //    if (Has(key)) return _strings[key];
        //    return def;
        //}
    }
}
