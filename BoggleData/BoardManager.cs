using System;
using System.Collections.Generic;
using Boggle.GameData;

namespace Boggle.BoggleData
{
    public static class BoardManager
    {
        public static Dictionary<string, GameBoard> Boards { get; private set; } = new Dictionary<string, GameBoard>();

        public static bool Initialize()
        {
            AttributeSet aset = GameDataManager.Load("boggle");
            foreach(GameAttribute layout in aset["layouts"])
            {
                GameBoard board = new GameBoard(layout);
                if (Boards.ContainsKey(board.Name)) Boards[board.Name] = board;
                Boards.Add(board.Name, board);
            }
            return true;
        }
    }
}
