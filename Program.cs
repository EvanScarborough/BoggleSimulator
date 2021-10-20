using System;

namespace Boggle
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Boggle())
                game.Run();
        }
    }
}
