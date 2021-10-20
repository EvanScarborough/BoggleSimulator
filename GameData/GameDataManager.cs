using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Boggle.GameData
{
    public class GameDataManager
    {
        public static string WorkingDir { get; private set; }
        public static string ContentDir { get => Path.Combine(WorkingDir, "Content"); }
        public static string DataDir { get => Path.Combine(ContentDir, "Data"); }
        public static string RelativeContentDir { get => "Content"; }

        public static bool Initialize()
        {
            WorkingDir = Directory.GetCurrentDirectory();
            return true;
        }




        public static bool Exists(string path)
        {
            return Exists(path.Split(new char[] { '/', '\\' }));
        }
        public static bool Exists(params string[] path)
        {
            string file = Path.Combine(path);
            if (Exists(file, DataDir)) return true;
            return false;
        }
        private static bool Exists(string file, string baseDir)
        {
            string fullPath = Path.Combine(baseDir, file) + (file.Contains(".json") ? "" : ".json");
            return File.Exists(fullPath);
        }




        public static AttributeSet Load(string path)
        {
            return Load(path.Split(new char[] { '/', '\\' }));
        }
        public static AttributeSet Load(params string[] path)
        {
            string file = Path.Combine(path);
            AttributeSet ret;
            if (Load(file, DataDir, out ret)) return ret;
            return new AttributeSet();
        }
        private static bool Load(string file, string baseDir, out AttributeSet aset, bool data = true)
        {
            aset = null;
            string fullPath = (data ? Path.Combine(baseDir, file) : Path.Combine(baseDir, file))
                + (file.Contains(".json") ? "" : ".json");
            if (!File.Exists(fullPath)) return false;
            try
            {
                using (StreamReader reader = new StreamReader(fullPath))
                {
                    aset = AttributeSet.FromJSON(reader.ReadToEnd());
                    return true;
                }
            }
            catch { }
            return false;
        }




        public static bool Save(AttributeSet aset, string path)
        {
            return Save(aset, path.Split(new char[] { '/', '\\' }));
        }
        public static bool Save(AttributeSet aset, params string[] path)
        {
            string fullPath = Path.Combine(DataDir, Path.Combine(path));
            fullPath += (fullPath.Contains(".json") ? "" : ".json");
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                }
                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    writer.Write(aset.ToJSON());
                }
                return true;
            }
            catch
            {
                Console.WriteLine("failed to save!!");
            }
            return false;
        }
    }
}
